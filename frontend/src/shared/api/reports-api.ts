import { baseApi } from './baseApi';

export type GetSalesSummaryRequest = {
  fromUtc: string;
  toUtc: string;
};

export type GetSalesSummaryResponse = {
  fromUtc?: string;
  toUtc?: string;
  onlineOrdersCount?: number;
  paidOnlineOrdersCount?: number;
  inStoreSalesCount?: number;
  onlineRevenue?: number;
  inStoreRevenue?: number;
  totalRevenue?: number;
};

export type RevenueGrouping = 1 | 2;

export type RevenueTrendItemResponse = {
  periodStartUtc?: string;
  onlineRevenue?: number;
  inStoreRevenue?: number;
  totalRevenue?: number;
};

export type GetRevenueTrendsRequest = {
  fromUtc: string;
  toUtc: string;
  groupBy: RevenueGrouping;
};

export type GetRevenueTrendsResponse = {
  fromUtc?: string;
  toUtc?: string;
  groupBy?: RevenueGrouping;
  items?: RevenueTrendItemResponse[];
  totalOnlineRevenue?: number;
  totalInStoreRevenue?: number;
  totalRevenue?: number;
};

export type OrderFulfillmentStatus = 1 | 2 | 3 | 4;

export type OrderStatusCountResponse = {
  status?: OrderFulfillmentStatus;
  count?: number;
};

export type GetOrdersByStatusRequest = {
  fromUtc: string;
  toUtc: string;
};

export type GetOrdersByStatusResponse = {
  fromUtc?: string;
  toUtc?: string;
  totalOrders?: number;
  statuses?: OrderStatusCountResponse[];
};

export const reportsApi = baseApi.injectEndpoints({
  endpoints: (build) => ({
    getSalesSummary: build.query<GetSalesSummaryResponse, GetSalesSummaryRequest>({
      query: (params) => ({ url: '/api/reports/sales-summary', params }),
    }),
    getRevenueTrends: build.query<GetRevenueTrendsResponse, GetRevenueTrendsRequest>({
      query: (params) => ({ url: '/api/reports/revenue-trends', params }),
    }),
    getOrdersByStatus: build.query<GetOrdersByStatusResponse, GetOrdersByStatusRequest>({
      query: (params) => ({ url: '/api/reports/orders-by-status', params }),
    }),
  }),
});

export const { useGetSalesSummaryQuery, useGetRevenueTrendsQuery, useGetOrdersByStatusQuery } =
  reportsApi;
