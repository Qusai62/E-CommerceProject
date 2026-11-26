import { baseApi } from './baseApi';

export type InStoreSaleStatus = 1 | 2;

export const inStoreSaleStatusLabel = (status: number | undefined) =>
  status === 1 ? 'Completed' : status === 2 ? 'Voided' : 'Unavailable';

export type CreateInStoreSaleItemRequest = {
  itemName: string;
  unitPrice: number;
  quantity: number;
};

export type CreateInStoreSaleRequest = {
  submissionId: string;
  items: CreateInStoreSaleItemRequest[];
};

export type CreateInStoreSaleResponse = {
  inStoreSaleId?: number;
  saleNumber?: string;
  totalAmount?: number;
  status?: InStoreSaleStatus;
  soldAtUtc?: string;
};

/** A repeated SubmissionId returns the original sale with created set to false. */
export type CreateInStoreSaleResult = {
  sale?: CreateInStoreSaleResponse;
  created?: boolean;
};

export type InStoreSaleListItemResponse = {
  inStoreSaleId?: number;
  saleNumber?: string;
  totalAmount?: number;
  status?: InStoreSaleStatus;
  soldAtUtc?: string;
};

export type GetInStoreSalesRequest = {
  PageNumber?: number;
  PageSize?: number;
  FromUtc?: string;
  ToUtc?: string;
  Status?: InStoreSaleStatus;
};

export type GetInStoreSalesResponse = {
  items?: InStoreSaleListItemResponse[];
  pageNumber?: number;
  pageSize?: number;
  totalCount?: number;
  totalPages?: number;
};

export type InStoreSaleItemDetailsResponse = {
  inStoreSaleItemId?: number;
  itemName?: string;
  unitPrice?: number;
  quantity?: number;
  lineTotal?: number;
};

export type GetInStoreSaleDetailsResponse = {
  inStoreSaleId?: number;
  saleNumber?: string;
  totalAmount?: number;
  status?: InStoreSaleStatus;
  soldAtUtc?: string;
  voidedAtUtc?: string | null;
  /** Optimistic-concurrency token required when changing this sale. */
  rowVersion?: string;
  items?: InStoreSaleItemDetailsResponse[];
};

export type UpdateInStoreSaleItemRequest = {
  itemName: string;
  unitPrice: number;
  quantity: number;
};

export type UpdateInStoreSaleRequest = {
  items: UpdateInStoreSaleItemRequest[];
  rowVersion: string;
};

export type UpdateInStoreSaleResponse = {
  inStoreSaleId?: number;
  saleNumber?: string;
  totalAmount?: number;
  status?: InStoreSaleStatus;
  soldAtUtc?: string;
  rowVersion?: string;
};

export const inStoreSalesApi = baseApi.injectEndpoints({
  endpoints: (build) => ({
    createInStoreSale: build.mutation<CreateInStoreSaleResult, CreateInStoreSaleRequest>({
      query: (body) => ({ url: '/api/in-store-sales', method: 'POST', body }),
    }),
    getInStoreSales: build.query<GetInStoreSalesResponse, GetInStoreSalesRequest>({
      query: (params) => ({ url: '/api/in-store-sales', params }),
    }),
    getInStoreSaleById: build.query<GetInStoreSaleDetailsResponse, { id: number }>({
      query: ({ id }) => ({ url: `/api/in-store-sales/${encodeURIComponent(String(id))}` }),
    }),
    updateInStoreSale: build.mutation<
      UpdateInStoreSaleResponse,
      { id: number; body: UpdateInStoreSaleRequest }
    >({
      query: ({ id, body }) => ({
        url: `/api/in-store-sales/${encodeURIComponent(String(id))}`,
        method: 'PUT',
        body,
      }),
    }),
  }),
});

export const {
  useCreateInStoreSaleMutation,
  useGetInStoreSalesQuery,
  useGetInStoreSaleByIdQuery,
  useUpdateInStoreSaleMutation,
} = inStoreSalesApi;
