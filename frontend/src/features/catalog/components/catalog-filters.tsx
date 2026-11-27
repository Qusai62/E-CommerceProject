import { useForm } from 'react-hook-form';
import { Search, SlidersHorizontal } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import type { CatalogSearch, CatalogFilterChange } from '../model/catalog-filters';
import { parseOptionalNumber } from '../model/catalog-filters';

type FilterForm = { q: string; min: string; max: string; brand: string; category: string };
interface CatalogFiltersProps {
  search: CatalogSearch;
  onChange: (next: CatalogFilterChange) => void;
}
const brands = [
  'Samsung',
  'Apple',
  'Google',
  'OnePlus',
  'Xiaomi',
  'Nothing',
  'Dell',
  'Lenovo',
  'ASUS',
  'HP',
  'Sony',
  'Bose',
  'JBL',
  'Microsoft',
];
const categories = ['Smartphones', 'Laptops', 'Headphones', 'Tablets'];
export function CatalogFilters({ search, onChange }: CatalogFiltersProps) {
  // Draft lives in react-hook-form. Committed filters live in the URL, not duplicated in React state.
  const { register, handleSubmit, reset } = useForm<FilterForm>({
    defaultValues: {
      q: search.q,
      min: String(search.min ?? ''),
      max: String(search.max ?? ''),
      brand: String(search.brand ?? ''),
      category: String(search.category ?? ''),
    },
  });
  const apply = handleSubmit((values) => {
    const min = parseOptionalNumber(values.min),
      max = parseOptionalNumber(values.max);
    if (min !== undefined && max !== undefined && max < min) return;
    onChange({
      q: values.q.trim(),
      min,
      max,
      brand: parseOptionalNumber(values.brand),
      category: parseOptionalNumber(values.category),
    });
  });
  const clear = () => {
    reset({ q: '', min: '', max: '', brand: '', category: '' });
    onChange({ q: '', min: undefined, max: undefined, brand: undefined, category: undefined });
  };
  return (
    <aside className="w-full shrink-0 lg:w-64" aria-labelledby="filters-title">
      <div className="flex items-center justify-between border-b border-line pb-4">
        <h2 id="filters-title" className="flex items-center gap-2 text-sm font-bold">
          <SlidersHorizontal size={16} /> Refine results
        </h2>
        <button
          type="button"
          onClick={clear}
          className="text-xs font-semibold text-moss hover:underline"
        >
          Clear all
        </button>
      </div>
      <form className="mt-6 space-y-6" onSubmit={(event) => void apply(event)}>
        <div>
          <label htmlFor="search-products" className="mb-2 block text-xs font-bold">
            Search products
          </label>
          <div className="relative">
            <Input
              id="search-products"
              placeholder="Laptop, headphones…"
              className="pr-11"
              {...register('q')}
            />
            <Search
              size={17}
              className="pointer-events-none absolute right-4 top-3 text-slate-400"
            />
          </div>
        </div>
        <fieldset>
          <legend className="mb-3 text-xs font-bold">Price range</legend>
          <div className="grid grid-cols-2 gap-2">
            <label>
              <span className="sr-only">Minimum price</span>
              <Input type="number" step="0.01" min="0" placeholder="Min" {...register('min')} />
            </label>
            <label>
              <span className="sr-only">Maximum price</span>
              <Input type="number" step="0.01" min="0" placeholder="Max" {...register('max')} />
            </label>
          </div>
        </fieldset>
        <div className="rounded-xl border border-line bg-white p-4">
          <label htmlFor="brand-id" className="mb-2 block text-xs font-semibold">
            Brand
          </label>
          <select
            id="brand-id"
            className="w-full rounded-xl border border-line bg-white px-3 py-3 text-sm"
            {...register('brand')}
          >
            <option value="">All brands</option>
            {brands.map((name, index) => (
              <option key={name} value={index + 1}>
                {name}
              </option>
            ))}
          </select>
          <label htmlFor="category-id" className="mt-4 mb-2 block text-xs font-semibold">
            Category
          </label>
          <select
            id="category-id"
            className="w-full rounded-xl border border-line bg-white px-3 py-3 text-sm"
            {...register('category')}
          >
            <option value="">All categories</option>
            {categories.map((name, index) => (
              <option key={name} value={index + 1}>
                {name}
              </option>
            ))}
          </select>
        </div>
        <Button className="w-full" type="submit">
          Apply filters
        </Button>
      </form>
    </aside>
  );
}
