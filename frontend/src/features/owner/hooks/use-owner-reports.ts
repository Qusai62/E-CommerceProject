import { useGetOutOfStockProductsQuery, useGetOutOfStockVariantsQuery, useGetOwnerInventoryQuery, useGetProductsQuery } from '@/shared/api/generated/api.generated';
import { summarizeLoadedStock } from '../model/stock-report';

/** Each API count is server-wide; detail summaries are for the first 50 rows only. */
export function useOwnerReports() {
  const products = useGetProductsQuery({ PageNumber: 1, PageSize: 1, SortBy: 'createdAt', SortDirection: 'desc' });
  const inventory = useGetOwnerInventoryQuery({ pageNumber: 1, pageSize: 50 });
  const missingVariants = useGetOutOfStockVariantsQuery({ pageNumber: 1, pageSize: 1 });
  const missingProducts = useGetOutOfStockProductsQuery({ pageNumber: 1, pageSize: 1 });
  const sample = inventory.data?.items ?? [];
  const sampleSummary = summarizeLoadedStock(sample);
  return { products, inventory, missingVariants, missingProducts, sample, sampleSummary };
}
