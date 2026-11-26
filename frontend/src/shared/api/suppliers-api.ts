import { baseApi } from './baseApi';

export type CreateSupplierRequest = {
  name: string;
  phoneNumber?: string | null;
  email?: string | null;
};
export type UpdateSupplierRequest = CreateSupplierRequest;

export type CreateSupplierResponse = {
  supplierId: number;
  name: string;
  phoneNumber?: string | null;
  email?: string | null;
  createdAtUtc: string;
};

export type SupplierResponse = CreateSupplierResponse & {
  updatedAtUtc: string;
};

export type ListSuppliersRequest = {
  Search?: string;
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
};

export type PagedSuppliersResponse = {
  items: SupplierResponse[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

/** Supplier endpoints are maintained separately until they are included in the generated OpenAPI contract. */
export const suppliersApi = baseApi.injectEndpoints({
  endpoints: (build) => ({
    createSupplier: build.mutation<CreateSupplierResponse, CreateSupplierRequest>({
      query: (body) => ({ url: '/api/suppliers', method: 'POST', body }),
      invalidatesTags: ['Suppliers'],
    }),
    getSupplierById: build.query<SupplierResponse, { id: number }>({
      query: ({ id }) => ({ url: `/api/suppliers/${encodeURIComponent(String(id))}` }),
      providesTags: (_result, _error, { id }) => [{ type: 'Suppliers', id }],
    }),
    getSuppliers: build.query<PagedSuppliersResponse, ListSuppliersRequest>({
      query: (params) => ({ url: '/api/suppliers', params }),
      providesTags: ['Suppliers'],
    }),
    updateSupplier: build.mutation<SupplierResponse, { id: number; body: UpdateSupplierRequest }>({
      query: ({ id, body }) => ({
        url: `/api/suppliers/${encodeURIComponent(String(id))}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { id }) => [{ type: 'Suppliers', id }, 'Suppliers'],
    }),
  }),
});

export const {
  useCreateSupplierMutation,
  useGetSupplierByIdQuery,
  useGetSuppliersQuery,
  useUpdateSupplierMutation,
} = suppliersApi;
