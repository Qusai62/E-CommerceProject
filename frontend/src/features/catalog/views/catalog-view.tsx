import { getRouteApi, useNavigate } from '@tanstack/react-router';
import { useGetProductsQuery } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { CatalogFilters } from '../components/catalog-filters';
import { CatalogResults } from '../components/catalog-results';
import { toProductQuery, type CatalogFilterChange } from '../model/catalog-filters';

const route = getRouteApi('/shop/');
export type { CatalogSearch } from '../model/catalog-filters';
export function CatalogPage() {
  const search = route.useSearch();
  const navigate = useNavigate({ from: '/shop/' });
  const update = (patch: CatalogFilterChange) =>
    void navigate({ search: (previous) => ({ ...previous, ...patch, page: patch.page ?? 1 }) });
  // The URL is the single source of truth for committed filters and pagination.
  const products = useGetProductsQuery(toProductQuery(search));
  const clear = () =>
    update({ q: '', min: undefined, max: undefined, brand: undefined, category: undefined });
  return (
    <div className="page-container py-12">
      <PageHeader
        eyebrow="ELECTRONICS CATALOG"
        title="Discover your next upgrade"
        description="Compare everyday tech essentials in one place. Filter, sort and explore real-time availability."
      />
      <div className="mt-10 flex flex-col gap-8 lg:flex-row">
        <CatalogFilters
          key={JSON.stringify([search.q, search.min, search.max, search.brand, search.category])}
          search={search}
          onChange={update}
        />
        <section className="min-w-0 flex-1" aria-label="Product results">
          <div className="mb-6 flex flex-wrap items-center justify-between gap-3 border-b border-line pb-5">
            <p className="text-sm text-slate-500">
              {products.data ? `${products.data.totalCount ?? 0} products` : 'Browse electronics'}
            </p>
            <label className="flex items-center gap-2 text-xs font-semibold">
              <span className="text-slate-500">Sort by</span>
              <select
                aria-label="Sort products"
                className="rounded-xl border border-line bg-white px-3 py-2"
                value={search.sort}
                onChange={(event) => update({ sort: event.target.value })}
              >
                <option value="created-desc">Newest</option>
                <option value="price-asc">Price: low to high</option>
                <option value="price-desc">Price: high to low</option>
                <option value="name-asc">Name: A-Z</option>
              </select>
            </label>
          </div>
          {products.isLoading ? (
            <LoadingState message="Finding electronics…" />
          ) : products.isError ? (
            <ErrorState error={products.error} onRetry={() => void products.refetch()} />
          ) : (
            <CatalogResults products={products.data?.items ?? []} onReset={clear} />
          )}
          {products.data && products.data.totalCount !== 0 && (
            <div className="mt-12 flex items-center justify-between border-t border-line pt-6">
              <p className="text-sm text-slate-500">
                Page {products.data.pageNumber ?? search.page} of{' '}
                {Math.max(1, products.data.totalPages ?? 1)}
              </p>
              <div className="flex gap-2">
                <Button
                  variant="secondary"
                  size="sm"
                  disabled={!products.data.hasPreviousPage}
                  onClick={() => update({ page: search.page - 1 })}
                >
                  Previous
                </Button>
                <Button
                  variant="secondary"
                  size="sm"
                  disabled={!products.data.hasNextPage}
                  onClick={() => update({ page: search.page + 1 })}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </section>
      </div>
    </div>
  );
}
