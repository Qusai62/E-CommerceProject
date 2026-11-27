import {
  useGetOwnerInventoryQuery,
  useGetOutOfStockVariantsQuery,
  useIncreaseStockMutation,
  useDecreaseStockMutation,
} from '@/shared/api/generated/api.generated';
import { parseStockQuantity, type AdjustmentMode } from '../model/stock-adjustment';
export function useInventoryList(page: number, pageSize: number, onlyOut: boolean) {
  const full = useGetOwnerInventoryQuery({ pageNumber: page, pageSize }, { skip: onlyOut });
  const out = useGetOutOfStockVariantsQuery({ pageNumber: page, pageSize }, { skip: !onlyOut });
  return onlyOut ? out : full;
}
export function useAdjustInventory() {
  const [increase, increaseState] = useIncreaseStockMutation();
  const [decrease, decreaseState] = useDecreaseStockMutation();
  async function save(variantId: number, mode: AdjustmentMode, quantity: number) {
    const valid = parseStockQuantity(quantity);
    if (!valid || !Number.isSafeInteger(variantId) || variantId <= 0) {
      throw new Error('Invalid inventory adjustment');
    }
    if (mode === 'increase') {
      await increase({ productVariantId: variantId, body: { quantity: valid } }).unwrap();
    } else {
      await decrease({ productVariantId: variantId, body: { quantity: valid } }).unwrap();
    }
  }
  return {
    save,
    pending: increaseState.isLoading || decreaseState.isLoading,
    error: increaseState.error || decreaseState.error,
    reset: () => {
      increaseState.reset();
      decreaseState.reset();
    },
  };
}
