import { describe, it, expect } from 'vitest';
import { summarizeLoadedStock, stockCsv } from './stock-report';
describe('owner operational report', () => {
  const stock = [
    {
      productVariantId: 1,
      productName: 'Laptop',
      sku: 'SKU1',
      isInStock: true,
      availableQuantity: 18,
      reservedQuantity: 2,
    },
    {
      productVariantId: 2,
      productName: 'Speaker',
      sku: 'SKU2',
      isInStock: true,
      availableQuantity: 3,
      reservedQuantity: 0,
    },
    {
      productVariantId: 3,
      productName: 'Mouse',
      sku: 'SKU3',
      isInStock: false,
      availableQuantity: 0,
      reservedQuantity: 0,
    },
  ];
  it('calculates counts from ONLY supplied records', () =>
    expect(summarizeLoadedStock(stock)).toEqual({
      total: 3,
      outOfStock: 1,
      lowStock: 1,
      healthy: 1,
      availableUnits: 21,
      reservedUnits: 2,
    }));
  it('escapes csv values to avoid broken columns', () =>
    expect(stockCsv([{ ...stock[0], productName: 'Laptop, "Pro"' }])).toContain(
      '"Laptop, ""Pro"""',
    ));
});
