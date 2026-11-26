/**
 * GET /api/orders currently returns HTTP 200 without a response schema.
 * Keep unsafe unknown data at this boundary; never assume a row shape in UI.
 */
export interface OrderSummary {
  id: number;
  number: string;
  date?: string;
  total?: number;
  status?: number;
}
export interface OrderListPage {
  items: OrderSummary[];
  pageNumber: number;
  totalPages?: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
const record = (value: unknown): value is Record<string, unknown> => value !== null && typeof value === 'object' && !Array.isArray(value);
const number = (value: unknown): number | undefined => typeof value === 'number' && Number.isFinite(value) ? value : undefined;
const string = (value: unknown): string | undefined => typeof value === 'string' ? value : undefined;

export function parseOrderList(value: unknown, requestedPage: number): OrderListPage | undefined {
  const raw = Array.isArray(value) ? value : record(value) && Array.isArray(value.items) ? value.items : undefined;
  if (!raw) return;
  const items: OrderSummary[] = [];
  for (const item of raw) {
    if (!record(item)) return;
    const id = number(item.orderId);
    if (id === undefined || !Number.isSafeInteger(id) || id <= 0) return;
    items.push({ id, number: string(item.orderNumber) || `#${id}`,
      date: string(item.placedAtUtc), total: number(item.totalAmount), status: number(item.fulfillmentStatus) });
  }
  const metadata = record(value) ? value : {};
  const pageNumber = number(metadata.pageNumber) ?? requestedPage;
  const totalPages = number(metadata.totalPages);
  return { items, pageNumber, totalPages,
    hasPreviousPage: typeof metadata.hasPreviousPage === 'boolean' ? metadata.hasPreviousPage : pageNumber > 1,
    hasNextPage: typeof metadata.hasNextPage === 'boolean' ? metadata.hasNextPage : false };
}
