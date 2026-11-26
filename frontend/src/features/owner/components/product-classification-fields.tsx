import { Field } from '@/components/ui/feedback';
import {
  useGetBrandsQuery,
  useGetStoreCategoriesQuery,
} from '@/shared/api/generated/api.generated';

type Props = {
  brandId: string;
  categoryId: string;
  onBrandIdChange: (value: string) => void;
  onCategoryIdChange: (value: string) => void;
  disabled?: boolean;
  brandError?: string;
  categoryError?: string;
};

const selectClassName =
  'h-11 w-full rounded-xl border border-line bg-white px-3 text-sm disabled:cursor-not-allowed disabled:opacity-60';

export function ProductClassificationFields({
  brandId,
  categoryId,
  onBrandIdChange,
  onCategoryIdChange,
  disabled,
  brandError,
  categoryError,
}: Props) {
  const brands = useGetBrandsQuery({ PageNumber: 1, PageSize: 100 });
  const categories = useGetStoreCategoriesQuery();
  return (
    <div className="grid gap-5 sm:grid-cols-2">
      <Field
        label="Brand"
        error={brandError}
        hint={brands.isError ? 'Brands could not be loaded.' : undefined}
      >
        <select
          aria-label="Brand"
          className={selectClassName}
          value={brandId}
          onChange={(event) => onBrandIdChange(event.target.value)}
          disabled={disabled || brands.isLoading}
        >
          <option value="">No brand</option>
          {(brands.data?.items ?? []).map((brand) => (
            <option key={brand.brandId} value={brand.brandId ?? ''}>
              {brand.name ?? `Brand #${brand.brandId}`}
            </option>
          ))}
        </select>
      </Field>
      <Field
        label="Category"
        error={categoryError}
        hint={categories.isError ? 'Categories could not be loaded.' : undefined}
      >
        <select
          aria-label="Category"
          className={selectClassName}
          value={categoryId}
          onChange={(event) => onCategoryIdChange(event.target.value)}
          disabled={disabled || categories.isLoading}
        >
          <option value="">No category</option>
          {(categories.data ?? []).map((category) => (
            <option key={category.categoryId} value={category.categoryId ?? ''}>
              {category.name ?? `Category #${category.categoryId}`}
            </option>
          ))}
        </select>
      </Field>
    </div>
  );
}
