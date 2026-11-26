import { describe, expect, it } from 'vitest';
import { isReviewCurrent, resolveAddressId, isReadyToPlaceOrder } from './checkout';

describe('checkout model', () => {
  const address = { savedAddressId: 8, isDefault: true, recipientName: 'Alex' };
  const item = { cartItemId: 10, productVariantId: 200, quantity: 2, subtotal: 40 };
  const checkout = { shoppingCartId: 1, items: [item], productsTotal: 40 };
  const review = { shoppingCartId: 1, items: [item], deliveryAddress: { savedAddressId: 8 }, paymentMethod: 2 as const, productsTotal: 40, shippingCharge: 5, finalTotal: 45 };

  it('defaults to a saved default address without copied state', () => {
    expect(resolveAddressId([{ savedAddressId: 9 }, address])).toBe(8);
    expect(resolveAddressId([address], 8)).toBe(8);
    expect(resolveAddressId([], 8)).toBeUndefined();
  });
  it('requires server-reviewed totals and selected address', () => {
    expect(isReviewCurrent(review, checkout, 8, 2)).toBe(true);
    expect(isReadyToPlaceOrder(review)).toBe(true);
  });
  it('rejects a stale review if cart quantity or payment selection changes', () => {
    expect(isReviewCurrent(review, { ...checkout, items: [{ ...item, quantity: 3 }] }, 8, 2)).toBe(false);
    expect(isReviewCurrent(review, checkout, 8, 1)).toBe(false);
    expect(isReviewCurrent(review, checkout, 7, 2)).toBe(false);
  });
});
