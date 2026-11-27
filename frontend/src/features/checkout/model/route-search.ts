/** Only allow known internal routes; arbitrary URLs must never become navigation targets. */
export type CheckoutReturnPath = '/checkout' | '/cart' | '/account' | '/owner';
export function validReturnPath(value: unknown): CheckoutReturnPath | undefined {
  return value === '/checkout' || value === '/cart' || value === '/account' || value === '/owner'
    ? value
    : undefined;
}
export function parseConfirmationSearch(search: Record<string, unknown>) {
  return {
    orderId: typeof search.orderId === 'number' ? search.orderId : Number(search.orderId) || 0,
    orderNumber: typeof search.orderNumber === 'string' ? search.orderNumber : '',
  };
}
