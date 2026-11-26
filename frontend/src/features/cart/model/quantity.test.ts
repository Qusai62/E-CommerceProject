import { describe, expect, it } from 'vitest';
import { isValidCartQuantity, MAX_CART_QUANTITY, nextCartQuantity } from './quantity';

describe('cart quantity rules', () => {
  it('can increment and decrement an existing quantity', () => {
    expect(nextCartQuantity(2, 1)).toBe(3);
    expect(nextCartQuantity(2, -1)).toBe(1);
  });

  it('does not convert quantity one into a delete operation', () => {
    expect(nextCartQuantity(1, -1)).toBeNull();
  });

  it('rejects invalid numbers and int32 overflow', () => {
    expect(nextCartQuantity(MAX_CART_QUANTITY, 1)).toBeNull();
    expect(isValidCartQuantity(0)).toBe(false);
    expect(isValidCartQuantity(2.5)).toBe(false);
    expect(isValidCartQuantity(Number.NaN)).toBe(false);
  });
});
