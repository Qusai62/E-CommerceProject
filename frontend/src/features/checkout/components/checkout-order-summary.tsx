import type { CheckoutItemResponse, ReviewCheckoutResponse } from '@/shared/api/generated/api.generated';
import { Card } from '@/components/ui/card';
import { currency } from '@/shared/lib/format';

export function CheckoutOrderSummary({ items, productsTotal, review, shippingPreview }: {
  items: CheckoutItemResponse[];
  productsTotal: number | undefined;
  review?: ReviewCheckoutResponse;
  shippingPreview?: number;
}) {
  const lineItems = review?.items ?? items;
  return (
    <Card className="overflow-hidden p-5 sm:p-7 lg:sticky lg:top-28">
      <h2 className="text-lg font-bold">Order summary</h2>
      <p className="mt-1 text-xs text-slate-500">Pricing is calculated by your server.</p>
      <ul className="mt-5 max-h-72 space-y-4 overflow-auto pr-1">
        {lineItems.map((item, index) => (
          <li key={item.cartItemId ?? index} className="flex items-start justify-between gap-4 text-sm">
            <div className="min-w-0"><p className="line-clamp-2 font-medium">{item.productName || 'Electronics product'}</p><p className="mt-1 text-xs text-slate-500">Qty {item.quantity ?? 0} · {item.sku || 'No SKU'}</p></div>
            <span className="shrink-0 font-semibold tabular-nums">{currency(item.subtotal)}</span>
          </li>
        ))}
      </ul>
      <div className="mt-6 space-y-3 border-t border-line pt-5 text-sm">
        <div className="flex justify-between gap-3"><span className="text-slate-500">Products</span><span className="font-medium tabular-nums">{currency(review?.productsTotal ?? productsTotal)}</span></div>
        <div className="flex justify-between gap-3"><span className="text-slate-500">Shipping</span><span className="font-medium tabular-nums">{review ? currency(review.shippingCharge) : shippingPreview === undefined ? 'At review' : `${currency(shippingPreview)} estimate`}</span></div>
      </div>
      <div className="mt-5 flex justify-between gap-4 border-t border-line pt-5 text-lg font-extrabold"><span>{review ? 'Final total' : 'Products subtotal'}</span><span className="tabular-nums">{currency(review?.finalTotal ?? productsTotal)}</span></div>
      {!review && <p className="mt-3 text-xs leading-5 text-slate-500">The final amount, including shipping, appears after checkout review.</p>}
    </Card>
  );
}
