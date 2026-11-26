import { z } from 'zod';
export const categorySchema = z.object({
  name: z.string().trim().min(2, 'Enter a category name').max(120),
  description: z.string().trim().max(1000),
  parentCategoryId: z.string().refine(value => !value || (/^\d+$/.test(value) && Number.isSafeInteger(Number(value)) && Number(value) > 0), 'Use a valid positive category ID'),
  sortOrder: z.coerce.number<number>().int().min(0),
});
export type CategoryForm = z.infer<typeof categorySchema>;
export function toCreateCategory(values: CategoryForm) {
  return { name: values.name, description: values.description,
    parentCategoryId: values.parentCategoryId.trim() ? Number(values.parentCategoryId) : null,
    sortOrder: values.sortOrder };
}
