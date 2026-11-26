import { getRouteApi, Link } from '@tanstack/react-router';
import { Package, MapPin, Receipt } from 'lucide-react';
import { useGetOrderByIdQuery } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { currency, date } from '@/shared/lib/format';
import { fulfillmentStatusLabel } from '../model/fulfillment-status';

const route = getRouteApi('/account/orders/$orderId');
export function CustomerOrderDetailsPage() {
  const { orderId } = route.useParams();
  const id = Number(orderId);
  const validId = Number.isSafeInteger(id) && id > 0;
  const order = useGetOrderByIdQuery({ orderId: id }, { skip: !validId });
  if (!validId) return <ErrorState error={new Error('Invalid order ID')}/>;
  if (order.isLoading) return <LoadingState message="Loading order details…"/>;
  if (order.isError) return <div className="page-container py-12"><ErrorState error={order.error} onRetry={() => void order.refetch()}/></div>;
  const data = order.data;
  if (!data) return <ErrorState error={new Error('The order response is empty.')}/>;
  return <div className="page-container py-12"><Button asChild variant="outline" size="sm"><Link to="/account/orders">← Back to orders</Link></Button>
    <div className="mt-8"><PageHeader eyebrow="ORDER DETAILS" title={data.orderNumber || `Order #${id}`} description={`Placed ${date(data.placedAtUtc)} · Fulfillment: ${fulfillmentStatusLabel(data.fulfillmentStatus)}`}/></div>
    <div className="mt-8 grid gap-6 lg:grid-cols-[1.4fr_1fr]"><div className="space-y-5"><Card className="p-6 sm:p-8"><h2 className="flex items-center gap-3 text-xl font-bold"><Package size={22} className="text-blue-600"/> Ordered items</h2><div className="mt-5 divide-y divide-slate-100">{(data.items ?? []).map((item, index) => <div key={item.orderItemId ?? index} className="flex items-start justify-between gap-4 py-5"><div><p className="font-semibold">{item.productName || `Variant #${item.productVariantId}`}</p><p className="mt-1 text-xs text-slate-500">{item.sku} {item.variantAttributes ? `· ${item.variantAttributes}` : ''}</p><p className="mt-2 text-xs text-slate-500">Quantity: {item.quantity}</p></div><span className="shrink-0 font-semibold">{currency(item.subtotal ?? (item.unitPrice ?? 0) * (item.quantity ?? 0))}</span></div>)}</div></Card>
      <Card className="p-6 sm:p-8"><h2 className="flex items-center gap-3 text-xl font-bold"><MapPin size={22} className="text-blue-600"/> Delivery address</h2><p className="mt-4 text-sm font-semibold">{data.deliveryRecipientName}</p><p className="mt-2 text-sm leading-7 text-slate-600">{[data.deliveryStreet,data.deliveryBuildingInfo,data.deliveryNeighborhood,data.deliveryCity].filter(Boolean).join(', ')}</p><p className="text-sm text-slate-600">{data.deliveryContactNumber}</p></Card></div>
      <Card className="h-fit p-6 sm:p-8"><h2 className="flex items-center gap-3 text-xl font-bold"><Receipt size={22} className="text-blue-600"/> Payment summary</h2><div className="mt-6 space-y-4 text-sm"><div className="flex justify-between"><span className="text-slate-500">Products</span><span>{currency(data.subtotal ?? (data.totalAmount ?? 0) - (data.shippingCharge ?? 0))}</span></div><div className="flex justify-between"><span className="text-slate-500">Shipping</span><span>{currency(data.shippingCharge)}</span></div><div className="flex justify-between border-t border-slate-200 pt-4 text-lg font-bold"><span>Total</span><span>{currency(data.totalAmount)}</span></div><p className="text-xs leading-6 text-slate-500">Payment method: code {data.paymentMethod ?? '—'} · Payment status: code {data.paymentStatus ?? '—'}. Backend enum labels are not included in Swagger.</p></div></Card>
    </div></div>;
}
