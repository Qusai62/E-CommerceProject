// The request schema uses int32. Server-side stock rules remain authoritative.
export const MAX_CART_QUANTITY = 2_147_483_647;

export function isValidCartQuantity(quantity: number): boolean {
  return Number.isInteger(quantity) && quantity >= 1 && quantity <= MAX_CART_QUANTITY;
}

export function nextCartQuantity(current: number, delta: -1 | 1): number | null {
  if (!isValidCartQuantity(current)) return null;
  const next = current + delta;
  return isValidCartQuantity(next) ? next : null;
}
