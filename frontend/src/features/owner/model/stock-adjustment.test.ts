import { describe, expect, it } from 'vitest';
import { parseStockQuantity } from './stock-adjustment';
describe('inventory adjustment rules', () => {
  it('accepts positive integer quantities', () => {
    expect(parseStockQuantity(3)).toBe(3);
  });
  it('rejects fractional, negative and unsafe integer values', () => {
    expect(parseStockQuantity(-1)).toBeNull();
    expect(parseStockQuantity(1.25)).toBeNull();
    expect(parseStockQuantity(Number.MAX_SAFE_INTEGER + 1)).toBeNull();
  });
});
