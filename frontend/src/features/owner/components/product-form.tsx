import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import type { ReactNode } from 'react';
import {
  productFormSchema,
  productFormDefaults,
  type ProductFormValues,
} from '../model/product-form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Field } from '@/components/ui/feedback';
import { ProductClassificationFields } from './product-classification-fields';

interface ProductFormProps {
  onSave: (values: ProductFormValues) => Promise<void>;
  submitting: boolean;
  error?: ReactNode;
}
// UI receives behavior as a dependency: it knows nothing about RTK Query, routes or HTTP.
export function ProductForm({ onSave, submitting, error }: ProductFormProps) {
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<ProductFormValues>({
    resolver: zodResolver(productFormSchema),
    defaultValues: productFormDefaults,
  });
  return (
    <form
      className="grid gap-5 sm:grid-cols-2"
      noValidate
      onSubmit={(event) => void handleSubmit(onSave)(event)}
    >
      <div className="sm:col-span-2">
        <Field label="Product name" error={errors.name?.message}>
          <Input
            required
            maxLength={200}
            placeholder="e.g. Apex 14 Pro Laptop"
            {...register('name')}
          />
        </Field>
      </div>
      <ProductClassificationFields
        brandId={watch('brandId')}
        categoryId={watch('categoryId')}
        onBrandIdChange={(value) => setValue('brandId', value, { shouldValidate: true })}
        onCategoryIdChange={(value) => setValue('categoryId', value, { shouldValidate: true })}
        disabled={submitting}
        brandError={errors.brandId?.message}
        categoryError={errors.categoryId?.message}
      />
      <div className="sm:col-span-2">
        <Field label="Description" error={errors.description?.message}>
          <Textarea
            maxLength={2000}
            rows={5}
            placeholder="What makes this item stand out?"
            {...register('description')}
          />
        </Field>
      </div>
      <div className="sm:col-span-2">
        {error}
        <Button type="submit" disabled={submitting}>
          {submitting ? 'Creating product…' : 'Save draft product'}
        </Button>
      </div>
    </form>
  );
}
