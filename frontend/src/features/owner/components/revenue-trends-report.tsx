import { useState } from 'react';
import { BarChart3 } from 'lucide-react';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { currency, date } from '@/shared/lib/format';
import { type RevenueGrouping, useGetRevenueTrendsQuery } from '@/shared/api/reports-api';

const isoDate = (dateValue: Date) => dateValue.toISOString().slice(0, 10);
const utcStart = (value: string) => `${value}T00:00:00.000Z`;
const utcEnd = (value: string) => `${value}T23:59:59.999Z`;

export function RevenueTrendsReport() {
  const today = isoDate(new Date());
  const monthAgo = isoDate(new Date(Date.now() - 29 * 86_400_000));
  const [from, setFrom] = useState(monthAgo);
  const [to, setTo] = useState(today);
  const [groupBy, setGroupBy] = useState<RevenueGrouping>(1);
  const rangeIsValid = from <= to;
  const trends = useGetRevenueTrendsQuery(
    { fromUtc: utcStart(from), toUtc: utcEnd(to), groupBy },
    { skip: !rangeIsValid },
  );
  const items = trends.data?.items ?? [];
  const maximum = Math.max(1, ...items.map((item) => item.totalRevenue ?? 0));

  return (
    <section className="space-y-5">
      <Card className="p-6 sm:p-8">
        <div className="flex items-start gap-3">
          <BarChart3 className="mt-0.5 text-blue-600" size={23} />
          <div>
            <h2 className="text-xl font-bold">Revenue trends</h2>
            <p className="mt-1 text-sm text-slate-500">
              Compare online and in-store revenue over time.
            </p>
          </div>
        </div>
        <div className="mt-6 grid gap-4 sm:grid-cols-3">
          <label className="block text-xs font-semibold text-slate-700">
            From date (UTC)
            <Input
              className="mt-1.5"
              type="date"
              value={from}
              max={to}
              onChange={(event) => setFrom(event.target.value)}
            />
          </label>
          <label className="block text-xs font-semibold text-slate-700">
            To date (UTC)
            <Input
              className="mt-1.5"
              type="date"
              value={to}
              min={from}
              onChange={(event) => setTo(event.target.value)}
            />
          </label>
          <label className="block text-xs font-semibold text-slate-700">
            Group by
            <select
              className="mt-1.5 flex h-11 w-full rounded-xl border border-line bg-white px-4 py-2 text-sm text-ink outline-none focus-visible:border-moss focus-visible:ring-2 focus-visible:ring-moss/15"
              value={groupBy}
              onChange={(event) => setGroupBy(Number(event.target.value) as RevenueGrouping)}
            >
              <option value={1}>Day</option>
              <option value={2}>Month</option>
            </select>
          </label>
        </div>
        {!rangeIsValid && (
          <p className="mt-3 text-sm text-red-600">
            The end date must be on or after the start date.
          </p>
        )}
      </Card>
      {!rangeIsValid ? null : trends.isLoading ? (
        <LoadingState message="Loading revenue trends…" />
      ) : trends.isError ? (
        <ErrorState error={trends.error} onRetry={() => void trends.refetch()} />
      ) : (
        <Card className="p-6 sm:p-8">
          <div className="flex flex-wrap items-center justify-between gap-4">
            <div>
              <h3 className="text-lg font-bold">Revenue by {groupBy === 1 ? 'day' : 'month'}</h3>
              <p className="mt-1 text-sm text-slate-500">
                Total: {currency(trends.data?.totalRevenue)}
              </p>
            </div>
            <div className="flex gap-4 text-xs">
              <span className="flex items-center gap-1.5">
                <i className="size-3 rounded-sm bg-blue-600" /> Online
              </span>
              <span className="flex items-center gap-1.5">
                <i className="size-3 rounded-sm bg-emerald-500" /> In-store
              </span>
            </div>
          </div>
          {items.length === 0 ? (
            <p className="py-12 text-center text-sm text-slate-500">
              No revenue was recorded for this period.
            </p>
          ) : (
            <div className="mt-8 space-y-5">
              {items.map((item) => (
                <div key={item.periodStartUtc}>
                  <div className="mb-2 flex justify-between gap-4 text-xs">
                    <span className="font-semibold text-slate-700">
                      {date(item.periodStartUtc)}
                    </span>
                    <span className="text-slate-500">{currency(item.totalRevenue)}</span>
                  </div>
                  <div className="flex h-5 overflow-hidden rounded-full bg-slate-100">
                    <div
                      className="bg-blue-600"
                      style={{
                        width: `${Math.max(0, ((item.onlineRevenue ?? 0) / maximum) * 100)}%`,
                      }}
                    />
                    <div
                      className="bg-emerald-500"
                      style={{
                        width: `${Math.max(0, ((item.inStoreRevenue ?? 0) / maximum) * 100)}%`,
                      }}
                    />
                  </div>
                </div>
              ))}
            </div>
          )}
        </Card>
      )}
    </section>
  );
}
