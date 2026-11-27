import { Link, getRouteApi } from '@tanstack/react-router';
import { useState, type FormEvent } from 'react';
import { ArrowLeft, FileDown } from 'lucide-react';
import { PDFDownloadLink } from '@react-pdf/renderer';
import { Button } from '@/components/ui/button';
import { ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import {
  useGetSupplierInvoiceByIdQuery,
  useGetSupplierPaymentHistoryQuery,
  useRecordSupplierPaymentMutation,
} from '@/shared/api/supplier-invoices-api';
import { currency, date } from '@/shared/lib/format';
import { SupplierInvoicePdf } from '../components/supplier-invoice-pdf';

const route = getRouteApi('/owner/supplier-invoices_/$invoiceId');
export function OwnerSupplierInvoiceDetailsPage() {
  const { invoiceId } = route.useParams();
  const id = Number(invoiceId);
  const query = useGetSupplierInvoiceByIdQuery(
    { id },
    { skip: !Number.isSafeInteger(id) || id < 1 },
  );
  const [signature, setSignature] = useState('');
  const [paymentAmount, setPaymentAmount] = useState('');
  const [paidAt, setPaidAt] = useState(() => new Date().toISOString().slice(0, 16));
  const [recordPayment, payment] = useRecordSupplierPaymentMutation();
  const paymentHistory = useGetSupplierPaymentHistoryQuery(
    { id },
    { skip: !Number.isSafeInteger(id) || id < 1 },
  );
  if (!Number.isSafeInteger(id) || id < 1) {
    return <ErrorState error={{ data: { detail: 'Invoice ID must be a positive number.' } }} />;
  }
  if (query.isLoading) return <LoadingState message="Loading invoice…" />;
  if (query.isError) return <ErrorState error={query.error} onRetry={query.refetch} />;
  if (!query.data) return null;
  const invoice = query.data;
  const submitPayment = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const amount = Number(paymentAmount);
    if (
      !Number.isFinite(amount) ||
      amount <= 0 ||
      amount > invoice.remainingAmount ||
      payment.isLoading
    ) {
      return;
    }
    try {
      await recordPayment({
        id,
        body: {
          submissionId: crypto.randomUUID(),
          amount,
          paidAtUtc: new Date(paidAt).toISOString(),
        },
      }).unwrap();
      setPaymentAmount('');
    } catch {
      // The API error is shown below the form.
    }
  };
  return (
    <div className="max-w-5xl space-y-6">
      <div className="print:hidden">
        <Button asChild variant="ghost" size="sm">
          <Link to="/owner/supplier-invoices">
            <ArrowLeft size={16} /> Back to invoices
          </Link>
        </Button>
      </div>
      <section className="supplier-invoice-document rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200 sm:p-10">
        <div className="supplier-invoice-heading flex flex-col justify-between gap-6 border-b-2 border-slate-900 pb-7 sm:flex-row">
          <div>
            <p className="text-xs font-bold uppercase tracking-[.2em] text-blue-700">
              Nexora · Procurement
            </p>
            <h1 className="mt-3 text-4xl font-bold tracking-tight text-slate-950">
              SUPPLIER INVOICE
            </h1>
            <p className="mt-2 text-sm text-slate-500">
              Invoice #{invoice.supplierInvoiceId} · Created {date(invoice.createdAtUtc)}
            </p>
          </div>
          <div className="text-left sm:text-right">
            <p className="text-sm font-semibold text-slate-500">Invoice reference</p>
            <p className="mt-1 text-xl font-bold text-slate-900">{invoice.invoiceNumber}</p>
            <p className="mt-2 text-sm text-slate-600">
              Invoice date: {date(invoice.invoiceDateUtc)}
            </p>
          </div>
        </div>
        <div className="grid gap-6 py-7 sm:grid-cols-2">
          <div>
            <p className="text-xs font-bold uppercase tracking-wide text-slate-500">Supplier</p>
            <p className="mt-2 text-lg font-bold text-slate-900">
              {invoice.supplierName || `Supplier #${invoice.supplierId}`}
            </p>
          </div>
          <div className="sm:text-right">
            <p className="text-xs font-bold uppercase tracking-wide text-slate-500">
              Payment status
            </p>
            <p className="mt-2 text-lg font-bold text-slate-900">{invoice.paymentStatus}</p>
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="supplier-invoice-table w-full text-left text-sm">
            <thead className="border-y border-slate-300 text-xs uppercase tracking-wide text-slate-500">
              <tr>
                <th className="px-3 py-3">Item</th>
                <th className="px-3 py-3 text-right">Qty</th>
                <th className="px-3 py-3 text-right">Unit cost</th>
                <th className="px-3 py-3 text-right">Line total</th>
              </tr>
            </thead>
            <tbody>
              {invoice.items.map((item) => (
                <tr key={item.supplierInvoiceItemId} className="border-b border-slate-100">
                  <td className="px-3 py-4 font-medium">{item.productName}</td>
                  <td className="px-3 py-4 text-right">{item.quantity}</td>
                  <td className="px-3 py-4 text-right">{currency(item.unitPrice)}</td>
                  <td className="px-3 py-4 text-right font-semibold">{currency(item.lineTotal)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="supplier-invoice-totals ml-auto mt-7 max-w-xs space-y-3 border-t-2 border-slate-900 pt-4 text-sm">
          <div className="flex justify-between">
            <span>Total</span>
            <strong>{currency(invoice.totalAmount)}</strong>
          </div>
          <div className="flex justify-between">
            <span>Paid</span>
            <strong>{currency(invoice.paidAmount)}</strong>
          </div>
          <div className="flex justify-between text-lg">
            <span>Outstanding</span>
            <strong>{currency(invoice.remainingAmount)}</strong>
          </div>
        </div>
        {invoice.remainingAmount > 0 && (
          <form
            className="mt-8 rounded-2xl border border-slate-200 bg-slate-50 p-5 print:hidden"
            onSubmit={(event) => void submitPayment(event)}
          >
            <h2 className="font-bold text-slate-900">Record payment</h2>
            <p className="mt-1 text-sm text-slate-500">
              Outstanding balance: {currency(invoice.remainingAmount)}
            </p>
            <div className="mt-4 grid gap-4 sm:grid-cols-2">
              <label className="text-xs font-semibold text-slate-700">
                Amount
                <Input
                  className="mt-1.5"
                  type="number"
                  min="0.001"
                  max={invoice.remainingAmount}
                  step="0.001"
                  value={paymentAmount}
                  onChange={(event) => setPaymentAmount(event.target.value)}
                  placeholder="0.000"
                />
              </label>
              <label className="text-xs font-semibold text-slate-700">
                Paid at
                <Input
                  className="mt-1.5"
                  type="datetime-local"
                  value={paidAt}
                  onChange={(event) => setPaidAt(event.target.value)}
                />
              </label>
            </div>
            <div className="mt-4 flex flex-wrap items-center gap-3">
              <Button
                type="submit"
                disabled={
                  payment.isLoading ||
                  !paymentAmount ||
                  Number(paymentAmount) > invoice.remainingAmount
                }
              >
                {payment.isLoading ? 'Recording payment…' : 'Record payment'}
              </Button>
              <FormError error={payment.error} />
            </div>
          </form>
        )}
        <section className="mt-8 print:hidden">
          <h2 className="font-bold text-slate-900">Payment history</h2>
          {paymentHistory.isLoading ? (
            <p className="mt-3 text-sm text-slate-500">Loading payments…</p>
          ) : paymentHistory.isError ? (
            <div className="mt-3">
              <ErrorState error={paymentHistory.error} onRetry={paymentHistory.refetch} />
            </div>
          ) : paymentHistory.data?.payments.length ? (
            <div className="mt-3 overflow-x-auto rounded-xl border border-slate-200">
              <table className="w-full text-left text-sm">
                <thead className="border-b border-slate-200 bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
                  <tr>
                    <th className="px-4 py-3">Payment</th>
                    <th className="px-4 py-3">Paid at</th>
                    <th className="px-4 py-3">Recorded at</th>
                    <th className="px-4 py-3 text-right">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  {paymentHistory.data.payments.map((historyPayment) => (
                    <tr
                      key={historyPayment.supplierPaymentId}
                      className="border-b border-slate-100 last:border-0"
                    >
                      <td className="px-4 py-3 font-medium">#{historyPayment.supplierPaymentId}</td>
                      <td className="px-4 py-3 text-slate-600">{date(historyPayment.paidAtUtc)}</td>
                      <td className="px-4 py-3 text-slate-600">
                        {date(historyPayment.recordedAtUtc)}
                      </td>
                      <td className="px-4 py-3 text-right font-semibold">
                        {currency(historyPayment.amount)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="mt-3 text-sm text-slate-500">No payments have been recorded.</p>
          )}
        </section>
        <div className="mt-12 grid gap-7 border-t border-slate-200 pt-7 sm:grid-cols-2">
          <div>
            <p className="text-xs font-bold uppercase tracking-wide text-slate-500">
              Authorized signature
            </p>
            <Input
              className="mt-3 print:border-0 print:px-0"
              value={signature}
              onChange={(event) => setSignature(event.target.value)}
              placeholder="Type full name"
            />
            <div className="mt-8 border-t border-slate-400 pt-2 text-xs text-slate-500">
              Signature / name
            </div>
          </div>
          <div className="text-sm text-slate-500 sm:text-right">
            This document is generated from the supplier invoice record.
          </div>
        </div>
      </section>
      <div className="print:hidden">
        <PDFDownloadLink
          document={<SupplierInvoicePdf invoice={invoice} signature={signature} />}
          fileName={`supplier-invoice-${invoice.invoiceNumber}.pdf`}
        >
          {({ loading }) => (
            <Button disabled={loading}>
              <FileDown size={16} /> {loading ? 'Preparing PDF…' : 'Download PDF'}
            </Button>
          )}
        </PDFDownloadLink>
        <p className="mt-2 text-xs text-slate-500">
          Choose “Save as PDF” in the print dialog. The signer name will be included in the
          document.
        </p>
      </div>
    </div>
  );
}
