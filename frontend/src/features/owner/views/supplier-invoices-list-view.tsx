import { Link } from '@tanstack/react-router';
import { FileText, Plus, Search } from 'lucide-react';
import { useState, type FormEvent } from 'react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { useGetSupplierInvoicesQuery } from '@/shared/api/supplier-invoices-api';
import { currency, date } from '@/shared/lib/format';

export function OwnerSupplierInvoicesPage() {
  const [searchDraft, setSearchDraft] = useState('');
  const [search, setSearch] = useState('');
  const [paymentStatus, setPaymentStatus] = useState('');
  const [sortBy, setSortBy] = useState('invoiceDate');
  const [sortDescending, setSortDescending] = useState(true);
  const [page, setPage] = useState(1);
  const invoices = useGetSupplierInvoicesQuery({
    Search: search || undefined,
    PaymentStatus: paymentStatus || undefined,
    PageNumber: page,
    PageSize: 10,
    SortBy: sortBy,
    SortDescending: sortDescending,
  });

  const applySearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSearch(searchDraft.trim());
    setPage(1);
  };

  return (
    <div className="max-w-6xl space-y-7">
      <PageHeader
        eyebrow="PROCUREMENT"
        title="Supplier invoices"
        description="Search, filter, and open supplier purchase invoices."
        action={
          <Button asChild>
            <Link to="/owner/supplier-invoices/new">
              <Plus size={16} /> New invoice
            </Link>
          </Button>
        }
      />
      <Card className="p-6 sm:p-8">
        <form
          className="grid gap-3 lg:grid-cols-[minmax(0,1fr)_160px_180px_auto]"
          onSubmit={applySearch}
        >
          <div className="relative">
            <Input
              value={searchDraft}
              onChange={(event) => setSearchDraft(event.target.value)}
              placeholder="Search invoice number or supplier"
              className="pr-10"
            />
            <Search
              size={18}
              className="pointer-events-none absolute right-3 top-3 text-slate-400"
            />
          </div>
          <select
            aria-label="Payment status"
            value={paymentStatus}
            onChange={(event) => {
              setPaymentStatus(event.target.value);
              setPage(1);
            }}
            className="h-11 rounded-xl border border-line bg-white px-3 text-sm"
          >
            <option value="">All statuses</option>
            <option value="Unpaid">Unpaid</option>
            <option value="PartiallyPaid">Partially paid</option>
            <option value="Paid">Paid</option>
          </select>
          <select
            aria-label="Invoice sort order"
            value={`${sortBy}:${sortDescending}`}
            onChange={(event) => {
              const [field, descending] = event.target.value.split(':');
              setSortBy(field);
              setSortDescending(descending === 'true');
              setPage(1);
            }}
            className="h-11 rounded-xl border border-line bg-white px-3 text-sm"
          >
            <option value="invoiceDate:true">Newest first</option>
            <option value="invoiceDate:false">Oldest first</option>
            <option value="invoiceNumber:false">Invoice number A–Z</option>
            <option value="supplierName:false">Supplier A–Z</option>
            <option value="totalAmount:true">Highest amount</option>
          </select>
          <Button type="submit" variant="secondary">
            Search
          </Button>
        </form>
        {invoices.isLoading ? (
          <LoadingState message="Loading supplier invoices…" />
        ) : invoices.isError ? (
          <ErrorState error={invoices.error} onRetry={invoices.refetch} />
        ) : invoices.data?.items.length ? (
          <>
            <div className="mt-6 overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead className="border-b border-line text-xs uppercase tracking-wide text-stone-500">
                  <tr>
                    <th className="pb-3 pr-4">Invoice</th>
                    <th className="pb-3 pr-4">Supplier</th>
                    <th className="pb-3 pr-4">Date</th>
                    <th className="pb-3 pr-4 text-right">Total</th>
                    <th className="pb-3 pr-4 text-right">Remaining</th>
                    <th className="pb-3 pr-4">Status</th>
                    <th className="pb-3">
                      <span className="sr-only">Actions</span>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {invoices.data.items.map((invoice) => (
                    <tr
                      key={invoice.supplierInvoiceId}
                      className="border-b border-line last:border-0"
                    >
                      <td className="py-4 pr-4 font-semibold text-ink">{invoice.invoiceNumber}</td>
                      <td className="py-4 pr-4">
                        <p className="text-ink">{invoice.supplierName}</p>
                        <p className="mt-1 text-xs text-slate-400">#{invoice.supplierId}</p>
                      </td>
                      <td className="py-4 pr-4 text-slate-600">{date(invoice.invoiceDateUtc)}</td>
                      <td className="py-4 pr-4 text-right text-slate-600">
                        {currency(invoice.totalAmount)}
                      </td>
                      <td className="py-4 pr-4 text-right text-slate-600">
                        {currency(invoice.remainingAmount)}
                      </td>
                      <td className="py-4 pr-4 text-slate-600">{invoice.paymentStatus}</td>
                      <td className="py-4 text-right">
                        <Button asChild size="sm" variant="outline">
                          <Link
                            to="/owner/supplier-invoices/$invoiceId"
                            params={{ invoiceId: String(invoice.supplierInvoiceId) }}
                          >
                            View
                          </Link>
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="mt-6 flex items-center justify-between gap-3">
              <p className="text-sm text-slate-500">
                Page {invoices.data.pageNumber} of {Math.max(1, invoices.data.totalPages)} ·{' '}
                {invoices.data.totalCount} invoices
              </p>
              <div className="flex gap-2">
                <Button
                  size="sm"
                  variant="secondary"
                  disabled={page <= 1}
                  onClick={() => setPage((current) => current - 1)}
                >
                  Previous
                </Button>
                <Button
                  size="sm"
                  variant="secondary"
                  disabled={page >= invoices.data.totalPages}
                  onClick={() => setPage((current) => current + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          </>
        ) : (
          <EmptyState
            icon={<FileText size={30} />}
            title="No supplier invoices found"
            description={
              search || paymentStatus
                ? 'Try changing the filters.'
                : 'Record your first supplier invoice to see it here.'
            }
            action={
              <Button asChild>
                <Link to="/owner/supplier-invoices/new">
                  <Plus size={16} /> New invoice
                </Link>
              </Button>
            }
          />
        )}
      </Card>
    </div>
  );
}
