import { useState } from 'react';
import { ClipboardList } from 'lucide-react';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { type OrderFulfillmentStatus, useGetOrdersByStatusQuery } from '@/shared/api/reports-api';

const statusLabels: Record<OrderFulfillmentStatus, string> = { 1: 'Pending', 2: 'Processing', 3: 'Fulfilled', 4: 'Cancelled' };
const statusStyles: Record<OrderFulfillmentStatus, string> = { 1: 'bg-amber-100 text-amber-800', 2: 'bg-blue-100 text-blue-800', 3: 'bg-emerald-100 text-emerald-800', 4: 'bg-red-100 text-red-800' };
const isoDate = (dateValue: Date) => dateValue.toISOString().slice(0, 10);
const utcStart = (value: string) => `${value}T00:00:00.000Z`;
const utcEnd = (value: string) => `${value}T23:59:59.999Z`;

export function OrdersByStatusReport() {
  const today = isoDate(new Date());
  const monthAgo = isoDate(new Date(Date.now() - 29 * 86_400_000));
  const [from, setFrom] = useState(monthAgo);
  const [to, setTo] = useState(today);
  const rangeIsValid = from <= to;
  const report = useGetOrdersByStatusQuery({ fromUtc: utcStart(from), toUtc: utcEnd(to) }, { skip: !rangeIsValid });
  const statuses = report.data?.statuses ?? [];
  const maxCount = Math.max(1, ...statuses.map((item) => item.count ?? 0));

  return <section className="space-y-5"><Card className="p-6 sm:p-8"><div className="flex items-start gap-3"><ClipboardList className="mt-0.5 text-blue-600" size={23} /><div><h2 className="text-xl font-bold">Orders by fulfillment status</h2><p className="mt-1 text-sm text-slate-500">Track how customer orders move through store fulfillment.</p></div></div><div className="mt-6 grid gap-4 sm:grid-cols-2"><label className="block text-xs font-semibold text-slate-700">From date (UTC)<Input className="mt-1.5" type="date" value={from} max={to} onChange={(event) => setFrom(event.target.value)} /></label><label className="block text-xs font-semibold text-slate-700">To date (UTC)<Input className="mt-1.5" type="date" value={to} min={from} onChange={(event) => setTo(event.target.value)} /></label></div>{!rangeIsValid && <p className="mt-3 text-sm text-red-600">The end date must be on or after the start date.</p>}</Card>
    {!rangeIsValid ? null : report.isLoading ? <LoadingState message="Loading order-status report…" /> : report.isError ? <ErrorState error={report.error} onRetry={() => void report.refetch()} /> : <Card className="p-6 sm:p-8"><div className="flex items-baseline justify-between gap-4"><h3 className="text-lg font-bold">{report.data?.totalOrders ?? 0} total orders</h3><span className="text-xs text-slate-500">Selected UTC range</span></div>{statuses.length === 0 ? <p className="py-10 text-center text-sm text-slate-500">No orders were recorded for this period.</p> : <div className="mt-7 space-y-5">{statuses.map((item) => { const status = item.status; return <div key={status}><div className="mb-2 flex items-center justify-between gap-4 text-sm"><span className={`rounded-full px-2.5 py-1 text-xs font-semibold ${status ? statusStyles[status] : 'bg-slate-100 text-slate-700'}`}>{status ? statusLabels[status] : 'Unavailable'}</span><span className="font-bold">{item.count ?? 0}</span></div><div className="h-3 overflow-hidden rounded-full bg-slate-100"><div className={status === 4 ? 'h-full bg-red-500' : status === 3 ? 'h-full bg-emerald-500' : status === 2 ? 'h-full bg-blue-500' : 'h-full bg-amber-500'} style={{ width: `${Math.max(0, ((item.count ?? 0) / maxCount) * 100)}%` }} /></div></div>; })}</div>}</Card>}</section>;
}
