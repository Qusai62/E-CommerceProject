import { useUpdateCartItemQuantityMutation } from '@/shared/api/generated/api.generated';
import { isValidCartQuantity } from '../model/quantity';

/** Application-facing command hook. UI never constructs HTTP requests. */
export function useUpdateCartQuantity(cartItemId: number | undefined) {
  const [update, mutation] = useUpdateCartItemQuantityMutation();

  async function changeQuantity(quantity: number): Promise<void> {
    if (cartItemId === undefined || !Number.isSafeInteger(cartItemId) || cartItemId <= 0 || !isValidCartQuantity(quantity)) {
      return;
    }

    try {
      await update({ cartItemId, body: { quantity } }).unwrap();
      // RTK Query invalidates the Cart tag, so GET /api/cart refreshes automatically.
    } catch {
      // RTK Query exposes the 400/401/404/409 error through mutation.error.
    }
  }

  return {
    changeQuantity,
    isUpdating: mutation.isLoading,
    error: mutation.error,
  };
}
