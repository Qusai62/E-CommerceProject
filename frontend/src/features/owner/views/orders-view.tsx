import { useState, type FormEvent } from 'react';
import { Link } from '@tanstack/react-router';
import {
  Banknote,
  CheckCircle2,
  ClipboardList,
  Clock3,
  CreditCard,
  PackageCheck,
  RotateCcw,
  XCircle,
} from 'lucide-react';
import {
  useGetStoreOrdersQuery,
  useGetStoreOrderByNumberQuery,
  type StoreOrderResponse,
} from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card } from '@/components/ui/card';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';

const money = (amount: number | undefined) =>
  typeof amount === 'number'
    ? new Intl.NumberFormat(undefined, { style: 'currency', currency: 'USD' }).format(amount)
    : '—';

const date = (value: string | null | undefined) =>
  value
    ? new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(
        new Date(value),
      )
    : '—';

const paymentMethodLabel = (value: number | undefined) =>
  value === 1 ? 'Online payment' : value === 2 ? 'Cash on delivery' : '—';

const paymentStatusLabel = (value: number | undefined) =>
  value === 1 ? 'Unpaid' : value === 2 ? 'Paid' : '—';

const fulfillmentStatusLabel = (value: number | undefined) =>
  value === 1
    ? 'Pending'
    : value === 2
      ? 'Processing'
      : value === 3
        ? 'Fulfilled'
        : value === 4
          ? 'Cancelled'
          : '—';

const fulfillmentFilter = (value: string) =>
  value === '1' ? 1 : value === '2' ? 2 : value === '3' ? 3 : value === '4' ? 4 : undefined;

const paymentFilter = (value: string) => (value === '1' ? 1 : value === '2' ? 2 : undefined);

const emptyFilters = { fulfillmentStatus: '', paymentStatus: '', startDate: '', endDate: '' };

function PaymentMethodBadge({ value }: { value: number | undefined }) {
  const online = value === 1;
  return (
    <Badge
      className={
        online
          ? 'gap-1.5 border border-violet-200 bg-violet-50 text-violet-700'
          : value === 2
            ? 'gap-1.5 border border-amber-200 bg-amber-50 text-amber-800'
            : 'gap-1.5 border border-slate-200 bg-slate-50 text-slate-600'
      }
    >
      {online ? <CreditCard size={13} /> : <Banknote size={13} />}
      {paymentMethodLabel(value)}
    </Badge>
  );
}

function PaymentStatusBadge({ value }: { value: number | undefined }) {
  const paid = value === 2;
  return (
    <Badge
      className={
        paid
          ? 'gap-1.5 border border-emerald-200 bg-emerald-50 text-emerald-700'
          : value === 1
            ? 'gap-1.5 border border-rose-200 bg-rose-50 text-rose-700'
            : 'gap-1.5 border border-slate-200 bg-slate-50 text-slate-600'
      }
    >
      {paid ? <CheckCircle2 size={13} /> : <Clock3 size={13} />}
      {paymentStatusLabel(value)}
    </Badge>
  );
}

function FulfillmentStatusBadge({ value }: { value: number | undefined }) {
  const styles = {
    1: 'border border-sky-200 bg-sky-50 text-sky-700',
    2: 'border border-indigo-200 bg-indigo-50 text-indigo-700',
    3: 'border border-emerald-200 bg-emerald-50 text-emerald-700',
    4: 'border border-rose-200 bg-rose-50 text-rose-700',
  } as const;
  const Icon =
    value === 3 ? PackageCheck : value === 4 ? XCircle : value === 2 ? RotateCcw : Clock3;
  return (
    <Badge
      className={`gap-1.5 ${styles[value as keyof typeof styles] ?? 'border border-slate-200 bg-slate-50 text-slate-600'}`}
    >
      <Icon size={13} />
      {fulfillmentStatusLabel(value)}
    </Badge>
  );
}

function OrderRow({ order }: { order: StoreOrderResponse }) {
  return (
    <tr className="border-t border-slate-100 text-sm transition-colors hover:bg-blue-50/40">
      <td className="px-4 py-4 font-semibold text-slate-900">
        {order.orderNumber ?? (order.orderId ? `#${order.orderId}` : '—')}
      </td>
      <td className="px-4 py-4">
        <p className="font-medium text-slate-800">Customer #{order.customerId ?? '—'}</p>
      </td>
      <td className="px-4 py-4 text-slate-600">{date(order.placedAtUtc)}</td>
      <td className="px-4 py-4 font-semibold text-slate-900">{money(order.totalAmount)}</td>
      <td className="px-4 py-4">
        <PaymentMethodBadge value={order.paymentMethod} />
      </td>
      <td className="px-4 py-4">
        <PaymentStatusBadge value={order.paymentStatus} />
      </td>
      <td className="px-4 py-4">
        <FulfillmentStatusBadge value={order.fulfillmentStatus} />
      </td>
      <td className="px-4 py-4">
        <Link
          to="/owner/orders/$orderId"
          params={{ orderId: String(order.orderId) }}
          className="text-sm font-semibold text-blue-600 hover:underline"
        >
          View details
        </Link>
      </td>
    </tr>
  );
}

export function OwnerOrdersPage() {
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [activeSearch, setActiveSearch] = useState('');
  const [draftFilters, setDraftFilters] = useState(emptyFilters);
  const [filters, setFilters] = useState(emptyFilters);
  const orders = useGetStoreOrdersQuery({
    PageNumber: page,
    PageSize: 10,
    FulfillmentStatus: fulfillmentFilter(filters.fulfillmentStatus),
    PaymentStatus: paymentFilter(filters.paymentStatus),
    StartDate: filters.startDate || undefined,
    EndDate: filters.endDate || undefined,
  });
  const search = useGetStoreOrderByNumberQuery(
    { orderNumber: activeSearch },
    { skip: !activeSearch },
  );
  const isSearching = Boolean(activeSearch);
  const result = isSearching ? search : orders;
  const rows = isSearching ? (search.data ? [search.data] : []) : (orders.data?.items ?? []);
  const submitSearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setActiveSearch(searchTerm.trim());
  };
  const clearSearch = () => {
    setSearchTerm('');
    setActiveSearch('');
  };
  const applyFilters = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setPage(1);
    setFilters(draftFilters);
  };
  const clearFilters = () => {
    setPage(1);
    setDraftFilters(emptyFilters);
    setFilters(emptyFilters);
  };
  return (
    <div className="py-2">
      <PageHeader
        eyebrow="STORE MANAGEMENT"
        title="Orders"
        description="Review customer orders placed across your store."
      />
      <form className="mt-7 flex flex-col gap-2 sm:flex-row" onSubmit={submitSearch}>
        <Input
          value={searchTerm}
          onChange={(event) => setSearchTerm(event.target.value)}
          placeholder="Search by order number, e.g. ORD-..."
          aria-label="Order number"
        />
        <Button type="submit" disabled={!searchTerm.trim() || search.isLoading}>
          Search
        </Button>
        {isSearching && (
          <Button type="button" variant="outline" onClick={clearSearch}>
            Clear
          </Button>
        )}
      </form>
      <Card className="mt-4 p-4">
        <form className="grid gap-3 md:grid-cols-2 xl:grid-cols-5" onSubmit={applyFilters}>
          <label className="text-xs font-semibold text-slate-600">
            Fulfillment status
            <select
              value={draftFilters.fulfillmentStatus}
              onChange={(event) =>
                setDraftFilters((value) => ({ ...value, fulfillmentStatus: event.target.value }))
              }
              className="mt-1 h-11 w-full rounded-xl border border-line bg-white px-3 text-sm"
            >
              <option value="">All fulfillment statuses</option>
              <option value="1">Pending</option>
              <option value="2">Processing</option>
              <option value="3">Fulfilled</option>
              <option value="4">Cancelled</option>
            </select>
          </label>
          <label className="text-xs font-semibold text-slate-600">
            Payment status
            <select
              value={draftFilters.paymentStatus}
              onChange={(event) =>
                setDraftFilters((value) => ({ ...value, paymentStatus: event.target.value }))
              }
              className="mt-1 h-11 w-full rounded-xl border border-line bg-white px-3 text-sm"
            >
              <option value="">All payment statuses</option>
              <option value="1">Unpaid</option>
              <option value="2">Paid</option>
            </select>
          </label>
          <label className="text-xs font-semibold text-slate-600">
            From date
            <Input
              className="mt-1"
              type="date"
              value={draftFilters.startDate}
              onChange={(event) =>
                setDraftFilters((value) => ({ ...value, startDate: event.target.value }))
              }
            />
          </label>
          <label className="text-xs font-semibold text-slate-600">
            To date
            <Input
              className="mt-1"
              type="date"
              value={draftFilters.endDate}
              onChange={(event) =>
                setDraftFilters((value) => ({ ...value, endDate: event.target.value }))
              }
            />
          </label>
          <div className="flex items-end gap-2">
            <Button type="submit">Apply filters</Button>
            <Button type="button" variant="outline" onClick={clearFilters}>
              Reset
            </Button>
          </div>
        </form>
      </Card>
      {result.isLoading ? (
        <LoadingState message="Loading store orders…" />
      ) : result.isError ? (
        <div className="mt-7">
          <ErrorState error={result.error} onRetry={() => void result.refetch()} />
        </div>
      ) : !rows.length ? (
        <EmptyState
          icon={<ClipboardList size={28} />}
          title={isSearching ? 'Order not found' : 'No orders yet'}
          description={
            isSearching
              ? 'Try a different order number or clear the search.'
              : 'Customer orders will appear here when they are placed.'
          }
        />
      ) : (
        <>
          <Card className="mt-7 overflow-hidden">
            <div className="flex items-center justify-between border-b border-slate-100 bg-gradient-to-r from-slate-50 to-blue-50/70 px-4 py-3">
              <div>
                <h2 className="font-semibold text-slate-900">Order activity</h2>
                <p className="mt-0.5 text-xs text-slate-500">
                  Payment and fulfillment progress at a glance.
                </p>
              </div>
              <span className="rounded-full bg-blue-100 px-2.5 py-1 text-xs font-bold text-blue-700">
                {rows.length} shown
              </span>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full min-w-[820px] text-left">
                <thead className="bg-slate-50 text-xs font-bold uppercase tracking-wide text-slate-500">
                  <tr>
                    <th className="px-4 py-3">Order</th>
                    <th className="px-4 py-3">Customer</th>
                    <th className="px-4 py-3">Placed</th>
                    <th className="px-4 py-3">Total</th>
                    <th className="px-4 py-3">Payment method</th>
                    <th className="px-4 py-3">Payment status</th>
                    <th className="px-4 py-3">Fulfillment</th>
                    <th className="px-4 py-3">
                      <span className="sr-only">Actions</span>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map((order) => (
                    <OrderRow key={order.orderId ?? order.orderNumber} order={order} />
                  ))}
                </tbody>
              </table>
            </div>
          </Card>
          {!isSearching && (
            <div className="mt-6 flex items-center justify-between gap-3">
              <p className="text-sm text-slate-500">
                Page {orders.data?.pageNumber ?? page} of{' '}
                {Math.max(1, orders.data?.totalPages ?? 1)} ·{' '}
                {orders.data?.totalCount ?? rows.length} orders
              </p>
              <div className="flex gap-2">
                <Button
                  size="sm"
                  variant="outline"
                  disabled={!orders.data?.hasPreviousPage}
                  onClick={() => setPage((value) => value - 1)}
                >
                  Previous
                </Button>
                <Button
                  size="sm"
                  variant="outline"
                  disabled={!orders.data?.hasNextPage}
                  onClick={() => setPage((value) => value + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
}
