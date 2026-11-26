import { baseApi } from './baseApi';

export type CreateSupplierInvoiceItemRequest = {
  productName: string;
  quantity: number;
  unitPrice: number;
};
export type CreateSupplierInvoiceRequest = {
  supplierId: number;
  invoiceNumber: string;
  invoiceDateUtc: string;
  items: CreateSupplierInvoiceItemRequest[];
};
export type SupplierInvoiceItemResponse = CreateSupplierInvoiceItemRequest & {
  supplierInvoiceItemId: number;
  lineTotal: number;
};
export type CreateSupplierInvoiceResponse = {
  supplierInvoiceId: number;
  supplierId: number;
  invoiceNumber: string;
  invoiceDateUtc: string;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  paymentStatus: string;
  items: SupplierInvoiceItemResponse[];
  createdAtUtc: string;
};
export type SupplierInvoiceDetailsResponse = CreateSupplierInvoiceResponse & {
  supplierName?: string | null;
};

export type ListSupplierInvoicesRequest = {
  SupplierId?: number;
  Search?: string;
  PaymentStatus?: string;
  PageNumber?: number;
  PageSize?: number;
  SortBy?: string;
  SortDescending?: boolean;
};

export type SupplierInvoiceListItemResponse = {
  supplierInvoiceId: number;
  supplierId: number;
  supplierName: string;
  invoiceNumber: string;
  invoiceDateUtc: string;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  paymentStatus: string;
};

export type PagedSupplierInvoicesResponse = {
  items: SupplierInvoiceListItemResponse[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type RecordSupplierPaymentRequest = {
  submissionId: string;
  amount: number;
  paidAtUtc: string;
};

export type RecordSupplierPaymentResponse = {
  supplierPaymentId: number;
  supplierInvoiceId: number;
  submissionId: string;
  paymentAmount: number;
  paidAtUtc: string;
  recordedAtUtc: string;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  paymentStatus: string;
};

export type SupplierPaymentHistoryItemResponse = {
  supplierPaymentId: number;
  submissionId: string;
  amount: number;
  paidAtUtc: string;
  recordedAtUtc: string;
};

export type SupplierPaymentHistoryResponse = {
  supplierInvoiceId: number;
  invoiceNumber: string;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  paymentStatus: string;
  payments: SupplierPaymentHistoryItemResponse[];
};

export const supplierInvoicesApi = baseApi.injectEndpoints({
  endpoints: (build) => ({
    createSupplierInvoice: build.mutation<
      CreateSupplierInvoiceResponse,
      CreateSupplierInvoiceRequest
    >({
      query: (body) => ({ url: '/api/supplier-invoices', method: 'POST', body }),
      invalidatesTags: ['SupplierInvoices'],
    }),
    getSupplierInvoiceById: build.query<SupplierInvoiceDetailsResponse, { id: number }>({
      query: ({ id }) => ({ url: `/api/supplier-invoices/${encodeURIComponent(String(id))}` }),
      providesTags: (_result, _error, { id }) => [{ type: 'SupplierInvoices', id }],
    }),
    getSupplierInvoices: build.query<PagedSupplierInvoicesResponse, ListSupplierInvoicesRequest>({
      query: (params) => ({ url: '/api/supplier-invoices', params }),
      providesTags: ['SupplierInvoices'],
    }),
    recordSupplierPayment: build.mutation<
      RecordSupplierPaymentResponse,
      { id: number; body: RecordSupplierPaymentRequest }
    >({
      query: ({ id, body }) => ({
        url: `/api/supplier-invoices/${encodeURIComponent(String(id))}/payments`,
        method: 'POST',
        body,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'SupplierInvoices', id },
        'SupplierInvoices',
      ],
    }),
    getSupplierPaymentHistory: build.query<SupplierPaymentHistoryResponse, { id: number }>({
      query: ({ id }) => ({
        url: `/api/supplier-invoices/${encodeURIComponent(String(id))}/payments`,
      }),
      providesTags: (_result, _error, { id }) => [{ type: 'SupplierInvoices', id }],
    }),
  }),
});
export const {
  useCreateSupplierInvoiceMutation,
  useGetSupplierInvoiceByIdQuery,
  useGetSupplierInvoicesQuery,
  useGetSupplierPaymentHistoryQuery,
  useRecordSupplierPaymentMutation,
} = supplierInvoicesApi;
