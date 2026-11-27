import type {
  CustomerAddressResponse,
  PaymentMethod,
  ReviewCheckoutResponse,
  CheckoutSummaryResponse,
} from '@/shared/api/generated/api.generated';

/** Numeric meanings must match the ASP.NET PaymentMethod enum; see API_GAPS.md. */
export const paymentOptions: ReadonlyArray<{
  value: PaymentMethod;
  label: string;
  description: string;
}> = [{ value: 2, label: 'Cash on delivery', description: 'Pay when your order is delivered.' }];

export const hasValidAddressId = (
  address: CustomerAddressResponse,
): address is CustomerAddressResponse & { savedAddressId: number } =>
  Number.isSafeInteger(address.savedAddressId) && Number(address.savedAddressId) > 0;

export function resolveAddressId(
  addresses: readonly CustomerAddressResponse[],
  override?: number,
): number | undefined {
  if (override !== undefined && addresses.some((a) => a.savedAddressId === override)) {
    return override;
  }
  return (
    addresses.find((a) => a.isDefault && hasValidAddressId(a))?.savedAddressId ??
    addresses.find(hasValidAddressId)?.savedAddressId
  );
}

export function isReadyToPlaceOrder(review?: ReviewCheckoutResponse): boolean {
  return (
    !!review &&
    Number.isFinite(review.finalTotal) &&
    Number(review.finalTotal) >= 0 &&
    Number.isSafeInteger(review.deliveryAddress?.savedAddressId) &&
    Number(review.deliveryAddress?.savedAddressId) > 0
  );
}

/** Derived freshness check: invalidate a displayed review when the cart or selection changed. */
export function isReviewCurrent(
  review: ReviewCheckoutResponse | undefined,
  checkout: CheckoutSummaryResponse | undefined,
  addressId: number | undefined,
  paymentMethod: PaymentMethod,
): boolean {
  if (!review || !checkout || !addressId) return false;
  if (
    review.paymentMethod !== paymentMethod ||
    review.deliveryAddress?.savedAddressId !== addressId
  ) {
    return false;
  }
  if (!Number.isFinite(review.productsTotal) || !Number.isFinite(checkout.productsTotal)) {
    return false;
  }
  if (Math.abs(Number(review.productsTotal) - Number(checkout.productsTotal)) > 0.001) return false;
  const actual = checkout.items ?? [];
  const reviewed = review.items ?? [];
  if (actual.length !== reviewed.length) return false;
  return actual.every(
    (item) =>
      Number.isSafeInteger(item.cartItemId) &&
      Number(item.cartItemId) > 0 &&
      Number.isSafeInteger(item.quantity) &&
      Number(item.quantity) > 0 &&
      reviewed.some(
        (other) =>
          other.cartItemId === item.cartItemId &&
          other.quantity === item.quantity &&
          other.productVariantId === item.productVariantId,
      ),
  );
}
