export type AdjustmentMode = 'increase' | 'decrease';
export function parseStockQuantity(value: number) {
  return Number.isSafeInteger(value) && value > 0 ? value : null;
}
