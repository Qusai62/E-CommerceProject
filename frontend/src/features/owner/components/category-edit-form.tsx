import { useState } from 'react';
import type { CategoryListItemResponse } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { Field } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';

interface CategoryEditFormProps {
  category: CategoryListItemResponse;
  pending: boolean;
  onCancel(): void;
  onSave(values: { name: string; parentCategoryId: number | null }): Promise<void>;
}

export function CategoryEditForm({ category, pending, onCancel, onSave }: CategoryEditFormProps) {
  const [name, setName] = useState(category.name ?? '');
  const [parentCategoryId, setParentCategoryId] = useState(
    category.parentCategoryId?.toString() ?? '',
  );

  return (
    <form
      className="mt-6 space-y-4 border-t border-line pt-6"
      onSubmit={(event) => {
        event.preventDefault();
        const parent = parentCategoryId.trim();
        if (!name.trim() || (parent && (!/^\d+$/.test(parent) || Number(parent) < 1))) return;
        void onSave({ name: name.trim(), parentCategoryId: parent ? Number(parent) : null });
      }}
    >
      <h3 className="text-sm font-bold">Edit category #{category.categoryId}</h3>
      <Field label="Category name">
        <Input
          value={name}
          onChange={(event) => setName(event.target.value)}
          required
          minLength={2}
          maxLength={120}
        />
      </Field>
      <Field label="Parent category ID (optional)">
        <Input
          type="number"
          min="1"
          step="1"
          value={parentCategoryId}
          onChange={(event) => setParentCategoryId(event.target.value)}
        />
      </Field>
      <div className="flex gap-3">
        <Button type="submit" disabled={pending}>
          {pending ? 'Saving…' : 'Save changes'}
        </Button>
        <Button type="button" variant="secondary" onClick={onCancel} disabled={pending}>
          Cancel
        </Button>
      </div>
    </form>
  );
}
