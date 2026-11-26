import { useState } from 'react';
import { Link } from '@tanstack/react-router';
import {
  CalendarDays,
  CheckCircle2,
  ChevronLeft,
  ChevronRight,
  CircleX,
  Plus,
  ReceiptText,
} from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { currency, date } from '@/shared/lib/format';
import {
  inStoreSaleStatusLabel,
  type InStoreSaleStatus,
  useGetInStoreSalesQuery,
} from '@/shared/api/in-store-sales-api';

const pageSize = 10;
const utcStart = (value: string) => (value ? `${value}T00:00:00.000Z` : undefined);
const utcEnd = (value: string) => (value ? `${value}T23:59:59.999Z` : undefined);

function SaleStatusBadge({ status }: { status: InStoreSaleStatus | undefined }) {
  const completed = status === 1;
  return (
    <Badge
      className={
        completed
          ? 'gap-1.5 border border-emerald-200 bg-emerald-50 text-emerald-700'
          : status === 2
            ? 'gap-1.5 border border-rose-200 bg-rose-50 text-rose-700'
            : 'gap-1.5 border border-slate-200 bg-slate-50 text-slate-600'
      }
    >
      {completed ? <CheckCircle2 size={13} /> : <CircleX size={13} />}
      {inStoreSaleStatusLabel(status)}
    </Badge>
  );
}

export function InStoreSalesHistoryPage() {
  const [page, setPage] = useState(1);
  const [from, setFrom] = useState('');
  const [to, setTo] = useState('');
  const [status, setStatus] = useState('');
  const statusValue = status === '' ? undefined : (Number(status) as InStoreSaleStatus);
  const sales = useGetInStoreSalesQuery({
    PageNumber: page,
    PageSize: pageSize,
    FromUtc: utcStart(from),
    ToUtc: utcEnd(to),
    Status: statusValue,
  });
  const updateFilter = (setter: (value: string) => void, value: string) => {
    setPage(1);
    setter(value);
  };
  const rows = sales.data?.items ?? [];
  const totalPages = Math.max(1, sales.data?.totalPages ?? 1);
  const currentPage = sales.data?.pageNumber ?? page;
  const totalCount = sales.data?.totalCount ?? 0;

  return (
    <div className="space-y-7">
      <PageHeader
        eyebrow="POINT OF SALE"
        title="In-store sale history"
        description="Review completed point-of-sale transactions recorded by StoreOwner accounts."
        action={
          <Button asChild>
            <Link to="/owner/in-store-sales">
              <Plus size={16} /> Record sale
            </Link>
          </Button>
        }
      />
      <Card className="overflow-hidden">
        <div className="flex items-center gap-3 border-b border-slate-100 bg-gradient-to-r from-blue-50 to-indigo-50/60 px-5 py-4 sm:px-6">
          <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-white shadow-md shadow-blue-200">
            <CalendarDays size={19} />
          </span>
          <div>
            <h2 className="font-bold text-slate-900">Filter sales</h2>
            <p className="text-xs text-slate-500">
              Narrow the history by date or transaction status.
            </p>
          </div>
        </div>
        <div className="p-5 sm:p-6">
          <div className="grid gap-4 sm:grid-cols-3">
            <label className="block text-xs font-semibold text-slate-700">
              From date (UTC)
              <Input
                className="mt-1.5"
                type="date"
                value={from}
                onChange={(event) => updateFilter(setFrom, event.target.value)}
              />
            </label>
            <label className="block text-xs font-semibold text-slate-700">
              To date (UTC)
              <Input
                className="mt-1.5"
                type="date"
                value={to}
                onChange={(event) => updateFilter(setTo, event.target.value)}
              />
            </label>
            <label className="block text-xs font-semibold text-slate-700">
              Status
              <select
                className="mt-1.5 flex h-11 w-full rounded-xl border border-line bg-white px-4 py-2 text-sm text-ink outline-none transition focus-visible:border-blue-500 focus-visible:ring-2 focus-visible:ring-blue-100"
                value={status}
                onChange={(event) => updateFilter(setStatus, event.target.value)}
              >
                <option value="">All statuses</option>
                <option value="1">Completed</option>
                <option value="2">Voided</option>
              </select>
            </label>
          </div>
          <p className="mt-3 text-xs text-slate-500">Dates are sent to the API as UTC.</p>
        </div>
      </Card>
      {sales.isLoading ? (
        <LoadingState message="Loading in-store sales…" />
      ) : sales.isError ? (
        <ErrorState error={sales.error} onRetry={() => void sales.refetch()} />
      ) : (
        <Card className="overflow-hidden">
          <div className="flex items-center justify-between gap-3 border-b border-slate-100 bg-gradient-to-r from-white to-blue-50/60 p-5 sm:px-6">
            <div>
              <h2 className="font-bold text-slate-900">Sales activity</h2>
              <p className="mt-1 text-sm text-slate-500">
                {totalCount} recorded sale{totalCount === 1 ? '' : 's'}
              </p>
            </div>
            <span className="grid size-10 place-items-center rounded-xl bg-blue-100 text-blue-700">
              <ReceiptText size={20} />
            </span>
          </div>
          {rows.length === 0 ? (
            <p className="p-8 text-center text-sm text-slate-500">
              No in-store sales match these filters.
            </p>
          ) : (
            <div className="overflow-x-auto">
              <table className="min-w-full text-left text-sm">
                <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
                  <tr>
                    <th className="px-6 py-4">Sale</th>
                    <th className="px-6 py-4">Sold at</th>
                    <th className="px-6 py-4">Status</th>
                    <th className="px-6 py-4 text-right">Total</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {rows.map((sale) => (
                    <tr
                      key={sale.inStoreSaleId ?? sale.saleNumber}
                      className="transition-colors hover:bg-blue-50/50"
                    >
                      <td className="px-6 py-4 font-semibold text-slate-900">
                        {sale.inStoreSaleId ? (
                          <Link
                            to="/owner/in-store-sales/$saleId"
                            params={{ saleId: String(sale.inStoreSaleId) }}
                            className="text-blue-700 hover:underline"
                          >
                            {sale.saleNumber ?? `Sale #${sale.inStoreSaleId}`}
                          </Link>
                        ) : (
                          (sale.saleNumber ?? '—')
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-600">{date(sale.soldAtUtc)}</td>
                      <td className="px-6 py-4">
                        <SaleStatusBadge status={sale.status} />
                      </td>
                      <td className="px-6 py-4 text-right font-bold text-slate-900">
                        {currency(sale.totalAmount)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          <div className="flex items-center justify-between border-t border-slate-200 p-4">
            <p className="text-xs text-slate-500">
              Page {currentPage} of {totalPages}
            </p>
            <div className="flex gap-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                disabled={currentPage <= 1 || sales.isFetching}
                onClick={() => setPage((current) => Math.max(1, current - 1))}
              >
                <ChevronLeft /> Previous
              </Button>
              <Button
                type="button"
                variant="outline"
                size="sm"
                disabled={currentPage >= totalPages || sales.isFetching}
                onClick={() => setPage((current) => Math.min(totalPages, current + 1))}
              >
                Next <ChevronRight />
              </Button>
            </div>
          </div>
        </Card>
      )}
    </div>
  );
}
