import type { SearchSchemaInput } from '@tanstack/react-router';
import { createFileRoute } from '@tanstack/react-router';
import { CatalogPage } from '@/features/catalog/views/catalog-view';
import type { CatalogSearch } from '@/features/catalog/model/catalog-filters';

const number = (value: unknown) => {
  if (typeof value === 'number' && Number.isFinite(value)) return value;
  if (typeof value === 'string' && value.trim() && Number.isFinite(Number(value))) {
    return Number(value);
  }
  return undefined;
};

export const Route = createFileRoute('/shop/')({
  validateSearch: (search: Partial<CatalogSearch> & SearchSchemaInput): CatalogSearch => ({
    q: typeof search.q === 'string' ? search.q : '',
    page: Math.max(1, Math.floor(number(search.page) ?? 1)),
    sort: typeof search.sort === 'string' ? search.sort : 'created-desc',
    min: number(search.min),
    max: number(search.max),
    brand: number(search.brand),
    category: number(search.category),
  }),
  component: CatalogPage,
});
