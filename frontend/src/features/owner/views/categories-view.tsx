import { useState } from 'react';
import { FolderPen, FolderPlus, FolderTree } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { ConfirmAction } from '@/components/ui/alert-dialog';
import { Card } from '@/components/ui/card';
import { EmptyState, ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import {
  useCreateCategoryMutation,
  useDeactivateCategoryMutation,
  useGetCategoryHierarchyQuery,
  useGetStoreCategoriesQuery,
  useMoveCategoryMutation,
  useReactivateCategoryMutation,
  useUpdateCategoryMutation,
} from '@/shared/api/generated/api.generated';
import { CategoryEditForm } from '../components/category-edit-form';
import { CategoryFormCard } from '../components/category-form';
import { CategoryHierarchy } from '../components/category-hierarchy';
import { toCreateCategory } from '../model/category-schema';

export function OwnerCategoriesPage() {
  const [create, createStatus] = useCreateCategoryMutation();
  const [update, updateStatus] = useUpdateCategoryMutation();
  const [move, moveStatus] = useMoveCategoryMutation();
  const [deactivate, deactivateStatus] = useDeactivateCategoryMutation();
  const [reactivate, reactivateStatus] = useReactivateCategoryMutation();
  const categories = useGetStoreCategoriesQuery();
  const hierarchy = useGetCategoryHierarchyQuery();
  const [editingId, setEditingId] = useState<number | null>(null);
  const editing = categories.data?.find((category) => category.categoryId === editingId);

  return (
    <div className="space-y-8">
      <PageHeader
        eyebrow="CATALOG STRUCTURE"
        title="Categories"
        description="Organize electronics by product family, edit category details, and review the IDs used by products."
      />
      <div className="grid gap-6 xl:grid-cols-[1.35fr_.9fr]">
        <Card className="p-6 sm:p-8">
          <div className="mb-7 flex items-center gap-3">
            <FolderPlus size={24} className="text-blue-600" />
            <h2 className="text-xl font-bold">Create a category</h2>
          </div>
          <CategoryFormCard
            pending={createStatus.isLoading}
            onSubmit={async (values) => {
              try {
                await create({ body: toCreateCategory(values) }).unwrap();
              } catch {
                /* createStatus.error is displayed below */
              }
            }}
          />
          <div className="mt-6">
            <FormError error={createStatus.error} />
            {createStatus.isSuccess && (
              <p role="status" className="rounded-xl bg-emerald-50 p-4 text-sm text-emerald-800">
                Category created. ID: {createStatus.data?.categoryId ?? 'see API response'} —{' '}
                {createStatus.data?.name}
              </p>
            )}
          </div>
        </Card>
        <Card className="p-6 sm:p-8">
          <div className="flex items-center gap-3">
            <FolderTree className="text-blue-600" size={24} />
            <h2 className="text-xl font-bold">Existing categories</h2>
          </div>
          {categories.isLoading ? (
            <LoadingState message="Loading categories…" />
          ) : categories.isError ? (
            <ErrorState error={categories.error} onRetry={categories.refetch} />
          ) : categories.data?.length ? (
            <>
              <div className="mt-6 overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead className="border-b border-line text-xs uppercase tracking-wide text-stone-500">
                    <tr>
                      <th className="pb-3 pr-4">Name</th>
                      <th className="pb-3 pr-4">ID</th>
                      <th className="pb-3">Parent</th>
                      <th className="pb-3">
                        <span className="sr-only">Actions</span>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {categories.data.map((category) => (
                      <tr key={category.categoryId} className="border-b border-line last:border-0">
                        <td className="py-3 pr-4 font-medium text-ink">
                          {category.name ?? 'Unnamed category'}
                          {category.isActive === false && (
                            <span className="ml-2 text-xs font-normal text-stone-500">
                              Inactive
                            </span>
                          )}
                        </td>
                        <td className="py-3 pr-4 text-stone-600">{category.categoryId ?? '—'}</td>
                        <td className="py-3 text-stone-600">{category.parentCategoryId ?? '—'}</td>
                        <td className="py-3 text-right">
                          {category.categoryId !== undefined && category.categoryId !== null && (
                            <>
                              <Button
                                size="sm"
                                variant="ghost"
                                onClick={() => setEditingId(category.categoryId ?? null)}
                              >
                                <FolderPen /> Edit
                              </Button>
                              {category.isActive !== false && (
                                <ConfirmAction
                                  destructive
                                  title="Deactivate category?"
                                  description={`Customers will no longer be able to browse “${category.name ?? 'this category'}”.`}
                                  trigger={
                                    <Button size="sm" variant="ghost">
                                      Deactivate
                                    </Button>
                                  }
                                  onConfirm={() => {
                                    void deactivate({ categoryId: category.categoryId! })
                                      .unwrap()
                                      .then(() => {
                                        if (editingId === category.categoryId) setEditingId(null);
                                      })
                                      .catch(() => undefined);
                                  }}
                                />
                              )}
                              {category.isActive === false && (
                                <Button
                                  size="sm"
                                  variant="ghost"
                                  onClick={() => {
                                    void reactivate({ categoryId: category.categoryId! })
                                      .unwrap()
                                      .catch(() => undefined);
                                  }}
                                >
                                  Reactivate
                                </Button>
                              )}
                            </>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              {hierarchy.data?.length ? <CategoryHierarchy categories={hierarchy.data} /> : null}
              <FormError error={hierarchy.error} />
              {editing && (
                <CategoryEditForm
                  key={editing.categoryId}
                  category={editing}
                  pending={updateStatus.isLoading || moveStatus.isLoading}
                  onCancel={() => setEditingId(null)}
                  onSave={async (values) => {
                    try {
                      await update({
                        categoryId: editing.categoryId!,
                        body: { name: values.name },
                      }).unwrap();
                      if (values.parentCategoryId !== (editing.parentCategoryId ?? null)) {
                        await move({
                          categoryId: editing.categoryId!,
                          body: { parentCategoryId: values.parentCategoryId },
                        }).unwrap();
                      }
                      setEditingId(null);
                    } catch {
                      /* updateStatus.error is displayed below */
                    }
                  }}
                />
              )}
              <FormError
                error={
                  updateStatus.error ??
                  moveStatus.error ??
                  deactivateStatus.error ??
                  reactivateStatus.error
                }
              />
            </>
          ) : (
            <EmptyState
              icon={<FolderTree size={30} />}
              title="No categories yet"
              description="Create your first category to start organizing products."
            />
          )}
        </Card>
      </div>
    </div>
  );
}
