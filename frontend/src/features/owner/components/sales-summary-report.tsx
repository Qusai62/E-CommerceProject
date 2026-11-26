import { useState } from 'react';
import { ChartNoAxesCombined, Store, Truck } from 'lucide-react';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { currency } from '@/shared/lib/format';
import { useGetSalesSummaryQuery } from '@/shared/api/reports-api';
import { MetricCard } from './metric-card';

const isoDate = (date: Date) => date.toISOString().slice(0, 10);
const utcStart = (value: string) => `${value}T00:00:00.000Z`;
const utcEnd = (value: string) => `${value}T23:59:59.999Z`;

export function SalesSummaryReport() {
  const today = isoDate(new Date());
  const monthAgo = isoDate(new Date(Date.now() - 29 * 86_400_000));
  const [from, setFrom] = useState(monthAgo);
  const [to, setTo] = useState(today);
  const rangeIsValid = from <= to;
  const summary = useGetSalesSummaryQuery(
    { fromUtc: utcStart(from), toUtc: utcEnd(to) },
    { skip: !rangeIsValid },
  );

  return <section className="space-y-5">
    <Card className="p-6 sm:p-8"><div className="flex items-start gap-3"><ChartNoAxesCombined className="mt-0.5 text-blue-600" size={23} /><div><h2 className="text-xl font-bold">Sales summary</h2><p className="mt-1 text-sm text-slate-500">Revenue and transaction totals for online orders and in-store sales.</p></div></div><div className="mt-6 grid gap-4 sm:grid-cols-2"><label className="block text-xs font-semibold text-slate-700">From date (UTC)<Input className="mt-1.5" type="date" value={from} max={to} onChange={(event) => setFrom(event.target.value)} /></label><label className="block text-xs font-semibold text-slate-700">To date (UTC)<Input className="mt-1.5" type="date" value={to} min={from} onChange={(event) => setTo(event.target.value)} /></label></div>{!rangeIsValid && <p className="mt-3 text-sm text-red-600">The end date must be on or after the start date.</p>}</Card>
    {!rangeIsValid ? null : summary.isLoading ? <LoadingState message="Loading sales summary…" /> : summary.isError ? <ErrorState error={summary.error} onRetry={() => void summary.refetch()} /> : <><div className="grid gap-4 sm:grid-cols-3"><MetricCard icon={<ChartNoAxesCombined size={18}/>} label="Total revenue" value={currency(summary.data?.totalRevenue)} note="Online and in-store combined"/><MetricCard icon={<Truck size={18}/>} label="Online revenue" value={currency(summary.data?.onlineRevenue)} note={`${summary.data?.onlineOrdersCount ?? 0} online orders · ${summary.data?.paidOnlineOrdersCount ?? 0} paid`}/><MetricCard icon={<Store size={18}/>} label="In-store revenue" value={currency(summary.data?.inStoreRevenue)} note={`${summary.data?.inStoreSalesCount ?? 0} in-store sales`}/></div><Card className="p-6"><h3 className="font-bold">Report range</h3><p className="mt-2 text-sm text-slate-600">The API summarized sales from <strong>{from}</strong> through <strong>{to}</strong> in UTC.</p></Card></>}
  </section>;
}
