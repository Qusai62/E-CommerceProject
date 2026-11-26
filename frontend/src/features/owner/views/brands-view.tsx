import { useState, type FormEvent } from 'react';
import { Pencil, Search, Tag } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { EmptyState, ErrorState, Field, FormError, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import {
  useCreateBrandMutation,
  useGetBrandsQuery,
  useUpdateBrandMutation,
} from '@/shared/api/generated/api.generated';

type ActivityFilter = 'all' | 'active' | 'inactive';

export function OwnerBrandsPage() {
  const [name, setName] = useState('');
  const [searchDraft, setSearchDraft] = useState('');
  const [search, setSearch] = useState('');
  const [activity, setActivity] = useState<ActivityFilter>('all');
  const [page, setPage] = useState(1);
  const [editingBrandId, setEditingBrandId] = useState<number | null>(null);
  const [editingName, setEditingName] = useState('');
  const [create, createStatus] = useCreateBrandMutation();
  const [update, updateStatus] = useUpdateBrandMutation();
  const brands = useGetBrandsQuery({
    PageNumber: page,
    PageSize: 10,
    Search: search || undefined,
    IsActive: activity === 'all' ? undefined : activity === 'active',
  });
  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const value = name.trim();
    if (!value) return;
    try {
      await create({ body: { name: value } }).unwrap();
      setName('');
    } catch {
      /* createStatus.error is displayed below */
    }
  };
  const applySearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSearch(searchDraft.trim());
    setPage(1);
  };
  const saveEdit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const value = editingName.trim();
    if (editingBrandId === null || !value) return;
    try {
      await update({ brandId: editingBrandId, body: { name: value } }).unwrap();
      setEditingBrandId(null);
    } catch {
      /* updateStatus.error is displayed below */
    }
  };

  return (
    <div className="space-y-8">
      <PageHeader
        eyebrow="CATALOG STRUCTURE"
        title="Brands"
        description="Add and browse the manufacturers and labels used by your product catalog."
      />
      <div className="grid gap-6 xl:grid-cols-[.8fr_1.2fr]">
        <Card className="h-fit p-6 sm:p-8">
          <div className="mb-7 flex items-center gap-3">
            <Tag size={24} className="text-blue-600" />
            <h2 className="text-xl font-bold">Create a brand</h2>
          </div>
          <form className="space-y-5" onSubmit={(event) => void submit(event)}>
            <Field label="Brand name">
              <Input
                value={name}
                onChange={(event) => setName(event.target.value)}
                placeholder="e.g. Apple"
                required
                minLength={2}
                maxLength={120}
              />
            </Field>
            <Button type="submit" disabled={createStatus.isLoading}>
              {createStatus.isLoading ? 'Creating brand…' : 'Create brand'}
            </Button>
          </form>
          <div className="mt-6">
            <FormError error={createStatus.error} />
            {createStatus.isSuccess && (
              <p role="status" className="rounded-xl bg-emerald-50 p-4 text-sm text-emerald-800">
                Brand created. ID: {createStatus.data?.brandId ?? 'see API response'} —{' '}
                {createStatus.data?.name}
              </p>
            )}
          </div>
        </Card>
        <Card className="p-6 sm:p-8">
          <div className="flex items-center gap-3">
            <Tag className="text-blue-600" size={24} />
            <h2 className="text-xl font-bold">Brands</h2>
          </div>
          <form className="mt-6 flex flex-col gap-3 sm:flex-row" onSubmit={applySearch}>
            <div className="relative flex-1">
              <Input
                value={searchDraft}
                onChange={(event) => setSearchDraft(event.target.value)}
                placeholder="Search brands"
                className="pr-10"
              />
              <Search
                className="pointer-events-none absolute right-3 top-3 text-slate-400"
                size={18}
              />
            </div>
            <select
              aria-label="Filter brands by status"
              value={activity}
              onChange={(event) => {
                setActivity(event.target.value as ActivityFilter);
                setPage(1);
              }}
              className="h-11 rounded-xl border border-line bg-white px-3 text-sm"
            >
              <option value="all">All statuses</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
            </select>
            <Button type="submit" variant="secondary">
              Search
            </Button>
          </form>
          {brands.isLoading ? (
            <LoadingState message="Loading brands…" />
          ) : brands.isError ? (
            <ErrorState error={brands.error} onRetry={brands.refetch} />
          ) : brands.data?.items?.length ? (
            <>
              <div className="mt-6 overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead className="border-b border-line text-xs uppercase tracking-wide text-stone-500">
                    <tr>
                      <th className="pb-3 pr-4">Name</th>
                      <th className="pb-3 pr-4">ID</th>
                      <th className="pb-3">Status</th>
                      <th className="pb-3">
                        <span className="sr-only">Actions</span>
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {brands.data.items.map((brand) => (
                      <tr key={brand.brandId} className="border-b border-line last:border-0">
                        <td className="py-3 pr-4 font-medium text-ink">
                          {brand.name ?? 'Unnamed brand'}
                        </td>
                        <td className="py-3 pr-4 text-stone-600">{brand.brandId ?? '—'}</td>
                        <td className="py-3 text-stone-600">
                          {brand.isActive === false ? 'Inactive' : 'Active'}
                        </td>
                        <td className="py-3 text-right">
                          {brand.brandId !== undefined && brand.brandId !== null && (
                            <Button
                              size="sm"
                              variant="outline"
                              className="border-blue-200 bg-blue-50 text-blue-700 hover:bg-blue-100 hover:text-blue-800"
                              aria-label={`Edit ${brand.name ?? 'brand'}`}
                              onClick={() => {
                                setEditingBrandId(brand.brandId);
                                setEditingName(brand.name ?? '');
                              }}
                            >
                              <Pencil size={14} /> Edit
                            </Button>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              {editingBrandId !== null && (
                <div
                  className="fixed inset-0 z-40 bg-slate-950/55 backdrop-blur-sm"
                  aria-hidden="true"
                />
              )}
              {editingBrandId !== null && (
                <form
                  className="fixed left-1/2 top-1/2 z-50 w-[min(92vw,460px)] -translate-x-1/2 -translate-y-1/2 space-y-5 rounded-3xl border border-slate-100 bg-white p-6 shadow-2xl sm:p-8"
                  onSubmit={(event) => void saveEdit(event)}
                >
                  <div className="flex items-center gap-3 border-b border-slate-100 pb-5">
                    <span className="grid size-11 place-items-center rounded-xl bg-blue-100 text-blue-700">
                      <Pencil size={20} />
                    </span>
                    <div>
                      <h3 className="text-xl font-bold text-slate-900">Edit brand</h3>
                      <p className="mt-0.5 text-sm text-slate-500">
                        Update the name for brand #{editingBrandId}.
                      </p>
                    </div>
                  </div>
                  <Field label="Brand name">
                    <Input
                      value={editingName}
                      onChange={(event) => setEditingName(event.target.value)}
                      required
                      minLength={2}
                      maxLength={120}
                    />
                  </Field>
                  <FormError error={updateStatus.error} />
                  <div className="flex justify-end gap-3 border-t border-slate-100 pt-5">
                    <Button type="submit" disabled={updateStatus.isLoading}>
                      {updateStatus.isLoading ? 'Saving…' : 'Save changes'}
                    </Button>
                    <Button
                      type="button"
                      variant="secondary"
                      onClick={() => setEditingBrandId(null)}
                      disabled={updateStatus.isLoading}
                    >
                      Cancel
                    </Button>
                  </div>
                </form>
              )}
              <div className="mt-6 flex items-center justify-between gap-3">
                <p className="text-sm text-slate-500">
                  Page {brands.data.pageNumber ?? page} of{' '}
                  {Math.max(1, brands.data.totalPages ?? 1)} · {brands.data.totalCount ?? 0} brands
                </p>
                <div className="flex gap-2">
                  <Button
                    size="sm"
                    variant="secondary"
                    disabled={!brands.data.hasPreviousPage}
                    onClick={() => setPage((current) => current - 1)}
                  >
                    Previous
                  </Button>
                  <Button
                    size="sm"
                    variant="secondary"
                    disabled={!brands.data.hasNextPage}
                    onClick={() => setPage((current) => current + 1)}
                  >
                    Next
                  </Button>
                </div>
              </div>
            </>
          ) : (
            <EmptyState
              icon={<Tag size={30} />}
              title="No brands found"
              description="Try another search or create a new brand."
            />
          )}
        </Card>
      </div>
    </div>
  );
}
