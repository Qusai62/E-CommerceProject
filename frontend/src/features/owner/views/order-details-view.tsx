import { useState } from 'react';
import { getRouteApi, Link } from '@tanstack/react-router';
import {
  Banknote,
  CheckCircle2,
  Clock3,
  CreditCard,
  MapPin,
  Package,
  Receipt,
  UserRound,
  XCircle,
} from 'lucide-react';
import {
  useCancelOrderMutation,
  useGetOrderFulfillmentHistoryQuery,
  useGetStoreOrderByIdQuery,
  useUpdateOrderPaymentStatusMutation,
  useUpdateOrderFulfillmentStatusMutation,
  type OrderFulfillmentStatus,
} from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card } from '@/components/ui/card';
import { ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { Textarea } from '@/components/ui/textarea';
import { currency, date } from '@/shared/lib/format';

const route = getRouteApi('/owner/orders/$orderId');

const paymentMethodLabel = (value: number | undefined) =>
  value === 1 ? 'Online payment' : value === 2 ? 'Cash on delivery' : 'Unavailable';

const paymentStatusLabel = (value: number | undefined) =>
  value === 1 ? 'Unpaid' : value === 2 ? 'Paid' : 'Unavailable';

const fulfillmentStatusLabel = (value: number | undefined) =>
  value === 1
    ? 'Pending'
    : value === 2
      ? 'Processing'
      : value === 3
        ? 'Fulfilled'
        : value === 4
          ? 'Cancelled'
          : 'Unavailable';

function FulfillmentBadge({ value }: { value: number | undefined }) {
  const styles = {
    1: 'border border-sky-200 bg-sky-50 text-sky-700',
    2: 'border border-indigo-200 bg-indigo-50 text-indigo-700',
    3: 'border border-emerald-200 bg-emerald-50 text-emerald-700',
    4: 'border border-rose-200 bg-rose-50 text-rose-700',
  } as const;
  const Icon = value === 3 ? CheckCircle2 : value === 4 ? XCircle : Clock3;
  return (
    <Badge
      className={`gap-1.5 ${styles[value as keyof typeof styles] ?? 'border border-slate-200 bg-slate-50 text-slate-600'}`}
    >
      <Icon size={13} />
      {fulfillmentStatusLabel(value)}
    </Badge>
  );
}

function PaymentBadges({
  method,
  status,
}: {
  method: number | undefined;
  status: number | undefined;
}) {
  const paid = status === 2;
  return (
    <div className="flex flex-wrap gap-2">
      <Badge
        className={
          method === 1
            ? 'gap-1.5 border border-violet-200 bg-violet-50 text-violet-700'
            : 'gap-1.5 border border-amber-200 bg-amber-50 text-amber-800'
        }
      >
        {method === 1 ? <CreditCard size={13} /> : <Banknote size={13} />}
        {paymentMethodLabel(method)}
      </Badge>
      <Badge
        className={
          paid
            ? 'gap-1.5 border border-emerald-200 bg-emerald-50 text-emerald-700'
            : 'gap-1.5 border border-rose-200 bg-rose-50 text-rose-700'
        }
      >
        {paid ? <CheckCircle2 size={13} /> : <Clock3 size={13} />}
        {paymentStatusLabel(status)}
      </Badge>
    </div>
  );
}

function formatVariantAttributes(value: string | null | undefined) {
  if (!value) return undefined;
  try {
    const attributes: unknown = JSON.parse(value);
    if (!Array.isArray(attributes)) return undefined;
    const labels = attributes.flatMap((attribute) => {
      if (!attribute || typeof attribute !== 'object') return [];
      const { Name, Value } = attribute as { Name?: unknown; Value?: unknown };
      return typeof Name === 'string' && typeof Value === 'string' ? [`${Name}: ${Value}`] : [];
    });
    return labels.length ? labels.join(' · ') : undefined;
  } catch {
    return undefined;
  }
}

export function OwnerOrderDetailsPage() {
  const { orderId } = route.useParams();
  const id = Number(orderId);
  const validId = Number.isSafeInteger(id) && id > 0;
  const order = useGetStoreOrderByIdQuery({ orderId: id }, { skip: !validId });
  const fulfillmentHistory = useGetOrderFulfillmentHistoryQuery(
    { orderId: id },
    { skip: !validId },
  );
  const [updateFulfillmentStatus, updateStatus] = useUpdateOrderFulfillmentStatusMutation();
  const [updatePaymentStatus, paymentUpdateStatus] = useUpdateOrderPaymentStatusMutation();
  const [cancelOrder, cancelStatus] = useCancelOrderMutation();
  const [selectedFulfillmentStatus, setSelectedFulfillmentStatus] = useState<number | undefined>();
  const [cancellationReason, setCancellationReason] = useState('');
  const isCancelled = order.data?.fulfillmentStatus === 4;
  const canCancel = order.data?.fulfillmentStatus === 1 || order.data?.fulfillmentStatus === 2;
  if (!validId) return <ErrorState error={new Error('Invalid order ID')} />;
  if (order.isLoading) return <LoadingState message="Loading order details…" />;
  if (order.isError) return <ErrorState error={order.error} onRetry={() => void order.refetch()} />;
  const data = order.data;
  if (!data) return <ErrorState error={new Error('The order response is empty.')} />;
  const currentFulfillmentStatus = selectedFulfillmentStatus ?? data.fulfillmentStatus ?? 1;
  const saveFulfillmentStatus = async () => {
    await updateFulfillmentStatus({
      orderId: id,
      body: { newStatus: currentFulfillmentStatus as OrderFulfillmentStatus },
    }).unwrap();
    setSelectedFulfillmentStatus(undefined);
  };
  const markPaymentAsPaid = async () => {
    if (isCancelled) return;
    await updatePaymentStatus({ orderId: id }).unwrap();
  };
  const cancelThisOrder = async () => {
    if (!canCancel) return;
    await cancelOrder({ orderId: id, body: { reason: cancellationReason.trim() } }).unwrap();
    setCancellationReason('');
  };
  const delivery = data.delivery;
  const address = [delivery?.street, delivery?.buildingInfo, delivery?.neighborhood, delivery?.city]
    .filter(Boolean)
    .join(', ');
  return (
    <div className="py-2">
      <Button asChild variant="outline" size="sm">
        <Link to="/owner/orders">← Back to orders</Link>
      </Button>
      <div className="mt-7">
        <PageHeader
          eyebrow="STORE ORDER"
          title={data.orderNumber ?? `Order #${id}`}
          description={`Placed ${date(data.placedAtUtc ?? undefined)} · Fulfillment: ${fulfillmentStatusLabel(data.fulfillmentStatus)}`}
        />
      </div>
      <Card className="mt-6 overflow-hidden">
        <div className="flex flex-col justify-between gap-4 bg-gradient-to-r from-blue-50 via-white to-indigo-50 px-6 py-5 sm:flex-row sm:items-center sm:px-8">
          <div>
            <p className="text-xs font-bold uppercase tracking-[.16em] text-blue-700">
              Order overview
            </p>
            <p className="mt-1 text-sm text-slate-600">
              Customer #{data.customerId ?? '—'} · {data.items?.length ?? 0} item
              {(data.items?.length ?? 0) === 1 ? '' : 's'}
            </p>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <FulfillmentBadge value={data.fulfillmentStatus} />
            <PaymentBadges method={data.paymentMethod} status={data.paymentStatus} />
          </div>
        </div>
      </Card>
      <div className="mt-7 grid gap-6 lg:grid-cols-[1.4fr_1fr]">
        <div className="space-y-5">
          <Card className="border-blue-100 p-6 sm:p-8">
            <h2 className="flex items-center gap-3 text-xl font-bold">
              <span className="grid size-10 place-items-center rounded-xl bg-blue-100 text-blue-700">
                <Package size={20} />
              </span>{' '}
              Fulfillment
            </h2>
            <p className="mt-3 text-sm text-slate-600">
              Update the current fulfillment stage for this order.
            </p>
            <div className="mt-5 flex flex-col gap-3 sm:flex-row">
              <select
                aria-label="Fulfillment status"
                value={currentFulfillmentStatus}
                onChange={(event) => setSelectedFulfillmentStatus(Number(event.target.value))}
                disabled={updateStatus.isLoading || data.fulfillmentStatus === 4}
                className="h-11 flex-1 rounded-xl border border-line bg-white px-3 text-sm"
              >
                <option value={1}>Pending</option>
                <option value={2}>Processing</option>
                <option value={3}>Fulfilled</option>
                {data.fulfillmentStatus === 4 && <option value={4}>Cancelled</option>}
              </select>
              <Button
                type="button"
                onClick={() => void saveFulfillmentStatus()}
                disabled={
                  updateStatus.isLoading ||
                  data.fulfillmentStatus === 4 ||
                  selectedFulfillmentStatus === undefined
                }
              >
                {updateStatus.isLoading ? 'Updating…' : 'Update status'}
              </Button>
            </div>
            <div className="mt-3">
              <FormError error={updateStatus.error} />
            </div>
          </Card>
          {canCancel && (
            <Card className="border-red-200 p-6 sm:p-8">
              <h2 className="text-xl font-bold text-red-800">Cancel order</h2>
              <p className="mt-3 text-sm text-slate-600">
                This marks the order as cancelled. A reason is required and will be recorded in the
                status history.
              </p>
              <Textarea
                className="mt-5"
                aria-label="Cancellation reason"
                placeholder="Reason for cancellation"
                value={cancellationReason}
                onChange={(event) => setCancellationReason(event.target.value)}
                disabled={cancelStatus.isLoading}
              />
              <div className="mt-3 flex flex-wrap items-center gap-3">
                <Button
                  type="button"
                  variant="destructive"
                  onClick={() => void cancelThisOrder()}
                  disabled={cancelStatus.isLoading || !cancellationReason.trim()}
                >
                  {cancelStatus.isLoading ? 'Cancelling…' : 'Cancel order'}
                </Button>
                <FormError error={cancelStatus.error} />
              </div>
            </Card>
          )}
          <Card className="p-6 sm:p-8">
            <h2 className="flex items-center gap-3 text-xl font-bold">
              <span className="grid size-10 place-items-center rounded-xl bg-indigo-100 text-indigo-700">
                <Receipt size={20} />
              </span>{' '}
              Status history
            </h2>
            {fulfillmentHistory.isLoading ? (
              <p className="mt-4 text-sm text-slate-500">Loading status history…</p>
            ) : fulfillmentHistory.isError ? (
              <p className="mt-4 text-sm text-red-700">Unable to load status history.</p>
            ) : !(fulfillmentHistory.data?.history ?? []).length ? (
              <p className="mt-4 text-sm text-slate-500">
                No fulfillment status changes have been recorded.
              </p>
            ) : (
              <ol className="mt-5 space-y-5 border-l-2 border-slate-200 pl-5">
                {(fulfillmentHistory.data?.history ?? []).map((entry) => (
                  <li key={entry.orderFulfillmentStatusHistoryId} className="relative">
                    <span className="absolute -left-[1.7rem] top-1.5 h-3 w-3 rounded-full bg-blue-600 ring-4 ring-white" />
                    <p className="font-semibold text-slate-900">
                      {fulfillmentStatusLabel(entry.status)}
                    </p>
                    <p className="mt-1 text-sm text-slate-500">
                      {date(entry.changedAtUtc ?? undefined)}
                    </p>
                    {entry.reason && <p className="mt-2 text-sm text-slate-600">{entry.reason}</p>}
                  </li>
                ))}
              </ol>
            )}
          </Card>
          <Card className="p-6 sm:p-8">
            <h2 className="flex items-center gap-3 text-xl font-bold">
              <span className="grid size-10 place-items-center rounded-xl bg-blue-100 text-blue-700">
                <Package size={20} />
              </span>{' '}
              Ordered items
            </h2>
            <div className="mt-5 divide-y divide-slate-100">
              {(data.items ?? []).map((item, index) => (
                <div
                  key={item.orderItemId ?? index}
                  className="flex items-start justify-between gap-4 py-5"
                >
                  <div>
                    <p className="font-semibold">
                      {item.productName ?? `Variant #${item.productVariantId ?? '—'}`}
                    </p>
                    <p className="mt-1 text-xs text-slate-500">SKU: {item.sku ?? '—'}</p>
                    {formatVariantAttributes(item.variantAttributes) && (
                      <p className="mt-1 text-xs font-medium text-slate-600">
                        {formatVariantAttributes(item.variantAttributes)}
                      </p>
                    )}
                    <p className="mt-2 text-xs text-slate-500">Quantity: {item.quantity ?? '—'}</p>
                  </div>
                  <span className="shrink-0 font-semibold">
                    {currency(item.subtotal ?? (item.unitPrice ?? 0) * (item.quantity ?? 0))}
                  </span>
                </div>
              ))}
            </div>
          </Card>
          <Card className="p-6 sm:p-8">
            <h2 className="flex items-center gap-3 text-xl font-bold">
              <span className="grid size-10 place-items-center rounded-xl bg-emerald-100 text-emerald-700">
                <MapPin size={20} />
              </span>{' '}
              Delivery address
            </h2>
            <p className="mt-4 text-sm font-semibold">{delivery?.recipientName ?? '—'}</p>
            <p className="mt-2 text-sm leading-7 text-slate-600">{address || '—'}</p>
            <p className="text-sm text-slate-600">{delivery?.contactNumber ?? '—'}</p>
            {delivery?.additionalInfo && (
              <p className="mt-2 text-sm text-slate-500">{delivery.additionalInfo}</p>
            )}
          </Card>
        </div>
        <div className="space-y-5">
          <Card className="p-6 sm:p-8">
            <h2 className="flex items-center gap-3 text-xl font-bold">
              <span className="grid size-10 place-items-center rounded-xl bg-violet-100 text-violet-700">
                <UserRound size={20} />
              </span>{' '}
              Customer
            </h2>
            <p className="mt-5 font-semibold">Customer #{data.customerId ?? '—'}</p>
            <p className="mt-1 text-sm text-slate-600">
              Contact details are shown in the delivery address.
            </p>
          </Card>
          <Card className="p-6 sm:p-8">
            <h2 className="flex items-center gap-3 text-xl font-bold">
              <span className="grid size-10 place-items-center rounded-xl bg-amber-100 text-amber-700">
                <Receipt size={20} />
              </span>{' '}
              Payment summary
            </h2>
            <div className="mt-6 space-y-4 text-sm">
              <div className="flex justify-between">
                <span className="text-slate-500">Products</span>
                <span>
                  {currency(data.subtotal ?? (data.totalAmount ?? 0) - (data.shippingCharge ?? 0))}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-slate-500">Shipping</span>
                <span>{currency(data.shippingCharge)}</span>
              </div>
              <div className="flex justify-between border-t border-slate-200 pt-4 text-lg font-bold">
                <span>Total</span>
                <span>{currency(data.totalAmount)}</span>
              </div>
              <PaymentBadges method={data.paymentMethod} status={data.paymentStatus} />
              <p className="text-xs leading-6 text-slate-500">
                Payment method: {paymentMethodLabel(data.paymentMethod)} · Payment status:{' '}
                {paymentStatusLabel(data.paymentStatus)}.
              </p>
              {!isCancelled && data.paymentMethod === 2 && data.paymentStatus === 1 && (
                <div className="pt-2">
                  <Button
                    type="button"
                    size="sm"
                    onClick={() => void markPaymentAsPaid()}
                    disabled={paymentUpdateStatus.isLoading}
                  >
                    {paymentUpdateStatus.isLoading ? 'Updating…' : 'Mark as paid'}
                  </Button>
                  <div className="mt-3">
                    <FormError error={paymentUpdateStatus.error} />
                  </div>
                </div>
              )}
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}
