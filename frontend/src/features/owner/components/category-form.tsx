import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Field } from '@/components/ui/feedback';
import { categorySchema, type CategoryForm } from '../model/category-schema';

interface Props {
  pending: boolean;
  onSubmit(values: CategoryForm): Promise<void>;
}
export function CategoryFormCard({ pending, onSubmit }: Props) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<CategoryForm>({
    resolver: zodResolver(categorySchema),
    defaultValues: { name: '', description: '', parentCategoryId: '', sortOrder: 0 },
  });
  return (
    <form noValidate onSubmit={(event) => void handleSubmit(onSubmit)(event)} className="space-y-5">
      <Field label="Category name" error={errors.name?.message}>
        <Input placeholder="e.g. Laptops & computers" {...register('name')} />
      </Field>
      <Field label="Description" error={errors.description?.message}>
        <Textarea
          rows={4}
          placeholder="What shoppers will find here"
          {...register('description')}
        />
      </Field>
      <div className="grid gap-5 sm:grid-cols-2">
        <Field
          label="Parent category ID (optional)"
          hint="Use a numeric ID only if the parent exists."
        >
          <Input
            type="number"
            min="1"
            step="1"
            placeholder="No parent"
            {...register('parentCategoryId')}
          />
        </Field>
        <Field label="Display order" error={errors.sortOrder?.message}>
          <Input type="number" min="0" step="1" {...register('sortOrder')} />
        </Field>
      </div>
      <Button type="submit" disabled={pending || isSubmitting}>
        {pending || isSubmitting ? 'Creating category…' : 'Create category'}
      </Button>
    </form>
  );
}
