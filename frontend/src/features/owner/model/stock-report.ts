import type { StoreOwnerStockResponse } from '@/shared/api/generated/api.generated';

/** These totals describe ONLY the page loaded from the inventory endpoint. */
export function summarizeLoadedStock(rows: readonly StoreOwnerStockResponse[]) {
  const total = rows.length;
  const outOfStock = rows.filter((item) => !item.isInStock).length;
  const lowStock = rows.filter((item) => (item.availableQuantity ?? 0) > 0 && (item.availableQuantity ?? 0) <= 5).length;
  const healthy = Math.max(0, total - outOfStock - lowStock);
  const availableUnits = rows.reduce((sum, item) => sum + Math.max(0, item.availableQuantity ?? 0), 0);
  const reservedUnits = rows.reduce((sum, item) => sum + Math.max(0, item.reservedQuantity ?? 0), 0);
  return { total, outOfStock, lowStock, healthy, availableUnits, reservedUnits };
}

export function stockCsv(rows: readonly StoreOwnerStockResponse[]): string {
  const columns = ['productVariantId', 'productName', 'sku', 'onHandQuantity', 'reservedQuantity', 'securedCODQuantity', 'availableQuantity'] as const;
  // Prevent spreadsheet formula injection when exporting untrusted SKU/name data.
  const quote = (value: unknown) => {
    const raw = String(value ?? '');
    const safe = /^[=+@\-\t\r]/.test(raw) ? `\'${raw}` : raw;
    return `"${safe.replaceAll('"', '""')}"`;
  };
  return [columns.join(','), ...rows.map((row) => columns.map((key) => quote(row[key])).join(','))].join('\r\n');
}
