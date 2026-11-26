import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query';
import { demoBaseQuery } from './demoBaseQuery';
import { ensureCsrfRequestToken, readCsrfRequestToken } from './csrf';

// Cookies are sent automatically; authentication secrets never enter Redux or localStorage.
// ASP.NET antiforgery's paired cookie remains browser-managed (normally HttpOnly).
const rawBaseQuery = fetchBaseQuery({
  baseUrl: '/',
  credentials: 'include',
  prepareHeaders: (headers, { type }) => {
    const token = readCsrfRequestToken();
    if (token && type === 'mutation') {
      headers.set('X-CSRF-TOKEN', token);
    }
    return headers;
  },
});

function isUnsafeRequest(args: string | FetchArgs): boolean {
  const method = typeof args === 'string' ? 'GET' : (args.method ?? 'GET').toUpperCase();
  return !['GET', 'HEAD', 'OPTIONS'].includes(method);
}

const realBaseQuery: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> = async (
  args,
  api,
  extraOptions,
) => {
  if (isUnsafeRequest(args)) {
    try {
      await ensureCsrfRequestToken();
    } catch (reason) {
      return {
        error: {
          status: 'CUSTOM_ERROR',
          error: reason instanceof Error ? reason.message : 'Unable to obtain a CSRF token.',
        },
      };
    }
  }
  return rawBaseQuery(args, api, extraOptions);
};

export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: import.meta.env.VITE_DEMO_MODE === 'true' ? demoBaseQuery : realBaseQuery,
  tagTypes: [
    'Products',
    'Cart',
    'Inventory',
    'Customer',
    'Addresses',
    'Checkout',
    'Orders',
    'Auth',
    'Suppliers',
    'SupplierInvoices',
  ],
  refetchOnFocus: true,
  refetchOnReconnect: true,
  endpoints: () => ({}),
});
