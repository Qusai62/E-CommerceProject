import { useClearCartMutation, useRemoveCartItemMutation } from '@/shared/api/generated/api.generated';

/** Only orchestration: components never know URLs, request verbs, or cache tags. */
export function useCartCommands() {
  const [removeItem, removing] = useRemoveCartItemMutation();
  const [clearCart, clearing] = useClearCartMutation();

  async function remove(cartItemId?: number): Promise<void> {
    if (!Number.isSafeInteger(cartItemId) || Number(cartItemId) <= 0) return;
    try { await removeItem({ cartItemId: cartItemId! }).unwrap(); }
    catch { /* Error is exposed by RTK Query status. */ }
  }
  async function clear(): Promise<void> {
    try { await clearCart().unwrap(); }
    catch { /* Error is exposed by RTK Query status. */ }
  }
  return { remove, clear, removing, clearing };
}
