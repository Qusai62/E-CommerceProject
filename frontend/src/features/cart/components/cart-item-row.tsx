import { Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { ConfirmAction } from '@/components/ui/alert-dialog';
import { useCartCommands } from '../hooks/use-cart-commands';
import { ProductVisual } from '@/features/catalog/components/product-card';
import { FormError } from '@/components/ui/feedback';
import type { ShoppingCartItemResponse } from '@/shared/api/generated/api.generated';
import { currency } from '@/shared/lib/format';
import { CartQuantityControl } from './cart-quantity-control';
import { useUpdateCartQuantity } from '../hooks/use-update-cart-quantity';
import { isValidCartQuantity } from '../model/quantity';

type CartItemRowProps = { item: ShoppingCartItemResponse };

/** Feature container: fetch/update orchestration is delegated to a command hook. */
export function CartItemRow({ item }: CartItemRowProps) {
  const name = item.productName || 'Electronics item';
  const quantity = item.quantity ?? 0;
  const validItem = typeof item.cartItemId === 'number' && Number.isSafeInteger(item.cartItemId) && item.cartItemId > 0;
  const { changeQuantity, isUpdating, error } = useUpdateCartQuantity(item.cartItemId);
  const { remove, removing } = useCartCommands();

  return (
    <article className="rounded-2xl border border-line bg-white p-4 sm:p-5">
      <div className="grid grid-cols-[80px_minmax(0,1fr)] gap-4 sm:grid-cols-[100px_minmax(0,1fr)_auto] sm:gap-5">
        <div className="w-20 sm:w-24">
          <ProductVisual name={name} />
        </div>
        <div className="min-w-0">
          <h2 className="text-sm font-semibold text-ink">{name}</h2>
          <p className="mt-1 text-xs text-stone-500">SKU: {item.sku || '—'}</p>
          <p className="mt-2 text-xs text-stone-600">Unit price: {currency(item.unitPrice)}</p>
          <div className="mt-4">
            <CartQuantityControl
              quantity={quantity}
              itemName={name}
              isUpdating={isUpdating}
              disabled={!validItem || !isValidCartQuantity(quantity)}
              onQuantityChange={(next) => { void changeQuantity(next); }}
            />
          </div>
          <div aria-live="polite" aria-atomic="true" className="mt-2 min-h-4 text-xs text-stone-500">
            {isUpdating ? 'Updating quantity…' : ''}
          </div>
          <ConfirmAction
            destructive
            title="Remove this item?"
            description={`Remove ${name} from your shopping cart?`}
            trigger={<Button variant="ghost" size="sm" className="mt-2 -ml-3 text-red-600" disabled={!validItem || removing.isLoading}><Trash2 size={15} /> Remove</Button>}
            onConfirm={() => { void remove(item.cartItemId); }}
          />
        </div>
        <div className="col-span-2 text-right text-sm font-semibold tabular-nums sm:col-span-1 sm:self-center">
          {currency(item.subtotal)}
          <span className="mt-1 block text-xs font-normal text-stone-500">Item subtotal</span>
        </div>
      </div>
      {(error || removing.error) && <div className="mt-4"><FormError error={error || removing.error} /></div>}
    </article>
  );
}
