import { useGetProductsQuery } from '@/shared/api/generated/api.generated';

/** Data-loading adapter. FeaturedProducts stays a reusable presentation component. */
export function useFeaturedProducts() {
  return useGetProductsQuery({
    PageNumber: 1,
    PageSize: 4,
    SortBy: 'createdAt',
    SortDirection: 'desc',
  });
}
