import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from '@tanstack/react-router';
import { FileText, Plus, Trash2 } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { EmptyState, FormError, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { useCreateSupplierInvoiceMutation } from '@/shared/api/supplier-invoices-api';
import { useGetSuppliersQuery } from '@/shared/api/suppliers-api';
import { currency } from '@/shared/lib/format';

type DraftItem = { id: string; productName: string; quantity: string; unitPrice: string };
const emptyItem = (): DraftItem => ({
  id: crypto.randomUUID(),
  productName: '',
  quantity: '1',
  unitPrice: '',
});
const today = () => new Date().toISOString().slice(0, 10);

export function OwnerSupplierInvoiceCreatePage() {
  const [supplierId, setSupplierId] = useState('');
  const [invoiceNumber, setInvoiceNumber] = useState('');
  const [invoiceDate, setInvoiceDate] = useState(today());
  const [invoiceId, setInvoiceId] = useState('');
  const [items, setItems] = useState<DraftItem[]>([emptyItem()]);
  const [created, setCreated] = useState<{
    invoiceNumber: string;
    totalAmount: number;
    supplierInvoiceId: number;
  }>();
  const suppliers = useGetSuppliersQuery({
    PageNumber: 1,
    PageSize: 100,
    SortBy: 'name',
    SortDescending: false,
  });
  const [createInvoice, status] = useCreateSupplierInvoiceMutation();
  const navigate = useNavigate();
  const lineItems = items.map(({ productName, quantity, unitPrice }) => ({
    productName: productName.trim(),
    quantity: Number(quantity),
    unitPrice: Number(unitPrice),
  }));
  const valid =
    Number(supplierId) > 0 &&
    invoiceNumber.trim().length > 0 &&
    invoiceDate.length > 0 &&
    lineItems.every(
      (item) =>
        item.productName &&
        Number.isSafeInteger(item.quantity) &&
        item.quantity > 0 &&
        Number.isFinite(item.unitPrice) &&
        item.unitPrice >= 0,
    );
  const total = lineItems.reduce(
    (sum, item) =>
      sum +
      (Number.isFinite(item.unitPrice) && Number.isFinite(item.quantity)
        ? item.quantity * item.unitPrice
        : 0),
    0,
  );
  const updateItem = (id: string, field: keyof Omit<DraftItem, 'id'>, value: string) =>
    setItems((current) =>
      current.map((item) => (item.id === id ? { ...item, [field]: value } : item)),
    );
  const submit = async () => {
    if (!valid || status.isLoading) return;
    try {
      const result = await createInvoice({
        supplierId: Number(supplierId),
        invoiceNumber: invoiceNumber.trim(),
        invoiceDateUtc: new Date(`${invoiceDate}T00:00:00.000Z`).toISOString(),
        items: lineItems,
      }).unwrap();
      setCreated(result);
    } catch {
      /* error rendered below */
    }
  };
  const startAnother = () => {
    setSupplierId('');
    setInvoiceNumber('');
    setInvoiceDate(today());
    setItems([emptyItem()]);
    setCreated(undefined);
    status.reset();
  };
  const openInvoice = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const id = Number(invoiceId);
    if (Number.isSafeInteger(id) && id > 0)
      void navigate({
        to: '/owner/supplier-invoices/$invoiceId',
        params: { invoiceId: String(id) },
      });
  };
  if (suppliers.isLoading) return <LoadingState message="Loading suppliers…" />;
  if (!suppliers.data?.items.length)
    return (
      <EmptyState
        icon={<FileText size={32} />}
        title="Create a supplier first"
        description="An invoice must be linked to a supplier."
      />
    );
  return (
    <div className="max-w-6xl space-y-7">
      <PageHeader
        eyebrow="PROCUREMENT"
        title="New supplier invoice"
        description="Record supplier purchases with precise item, quantity, and cost details."
      />
      <Card className="p-5 sm:p-6">
        <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <h2 className="font-bold">Open an existing invoice</h2>
            <p className="mt-1 text-sm text-slate-500">
              Enter an invoice ID to view or download its formal PDF.
            </p>
          </div>
          <form className="flex gap-2" onSubmit={(event) => void openInvoice(event)}>
            <Input
              className="w-36"
              value={invoiceId}
              onChange={(event) => setInvoiceId(event.target.value)}
              type="number"
              min="1"
              step="1"
              placeholder="Invoice ID"
              aria-label="Invoice ID"
            />
            <Button type="submit" variant="outline" disabled={!/^[1-9]\d*$/.test(invoiceId)}>
              Open
            </Button>
          </form>
        </div>
      </Card>
      {created ? (
        <Card className="border-emerald-200 bg-emerald-50 p-7">
          <h2 className="text-xl font-bold text-emerald-950">Invoice recorded</h2>
          <p className="mt-2 text-sm text-emerald-900">
            Invoice {created.invoiceNumber} (#{created.supplierInvoiceId}) was created for{' '}
            {currency(created.totalAmount)}.
          </p>
          <div className="mt-5 flex flex-wrap gap-3">
            <Button asChild>
              <Link
                to="/owner/supplier-invoices/$invoiceId"
                params={{ invoiceId: String(created.supplierInvoiceId) }}
              >
                View formal invoice
              </Link>
            </Button>
            <Button variant="outline" onClick={startAnother}>
              <Plus size={16} /> Create another invoice
            </Button>
          </div>
        </Card>
      ) : (
        <div className="grid gap-6 xl:grid-cols-[1fr_330px]">
          <Card className="p-6 sm:p-8">
            <div className="flex items-start gap-3">
              <FileText className="text-blue-600" size={24} />
              <div>
                <h2 className="text-xl font-bold">Invoice details</h2>
                <p className="mt-1 text-sm text-slate-500">
                  Choose the supplier and enter the supplier’s invoice reference.
                </p>
              </div>
            </div>
            <div className="mt-7 grid gap-5 sm:grid-cols-2">
              <label className="text-xs font-semibold text-slate-700">
                Supplier
                <select
                  value={supplierId}
                  onChange={(event) => setSupplierId(event.target.value)}
                  className="mt-1.5 h-11 w-full rounded-xl border border-line bg-white px-3 text-sm"
                >
                  <option value="">Select supplier</option>
                  {suppliers.data.items.map((supplier) => (
                    <option key={supplier.supplierId} value={supplier.supplierId}>
                      {supplier.name}
                    </option>
                  ))}
                </select>
              </label>
              <label className="text-xs font-semibold text-slate-700">
                Invoice date
                <Input
                  className="mt-1.5"
                  type="date"
                  value={invoiceDate}
                  onChange={(event) => setInvoiceDate(event.target.value)}
                />
              </label>
              <label className="sm:col-span-2 text-xs font-semibold text-slate-700">
                Supplier invoice number
                <Input
                  className="mt-1.5"
                  value={invoiceNumber}
                  onChange={(event) => setInvoiceNumber(event.target.value)}
                  placeholder="e.g. SUP-2026-0042"
                />
              </label>
            </div>
            <div className="mt-9 border-t border-slate-200 pt-7">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-xl font-bold">Invoice items</h2>
                  <p className="mt-1 text-sm text-slate-500">
                    Costs are recorded as supplied, before payment.
                  </p>
                </div>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={() => setItems((current) => [...current, emptyItem()])}
                >
                  <Plus size={15} /> Add item
                </Button>
              </div>
              <div className="mt-5 space-y-3">
                {items.map((item, index) => (
                  <div
                    key={item.id}
                    className="grid gap-3 rounded-2xl border border-slate-200 p-4 sm:grid-cols-[minmax(0,1fr)_100px_135px_auto] sm:items-end"
                  >
                    <label className="text-xs font-semibold text-slate-700">
                      Product name
                      <Input
                        className="mt-1.5"
                        value={item.productName}
                        onChange={(event) => updateItem(item.id, 'productName', event.target.value)}
                        placeholder="e.g. USB-C charger"
                      />
                    </label>
                    <label className="text-xs font-semibold text-slate-700">
                      Qty
                      <Input
                        className="mt-1.5"
                        type="number"
                        min="1"
                        step="1"
                        value={item.quantity}
                        onChange={(event) => updateItem(item.id, 'quantity', event.target.value)}
                      />
                    </label>
                    <label className="text-xs font-semibold text-slate-700">
                      Unit cost
                      <Input
                        className="mt-1.5"
                        type="number"
                        min="0"
                        step="0.001"
                        value={item.unitPrice}
                        onChange={(event) => updateItem(item.id, 'unitPrice', event.target.value)}
                        placeholder="0.000"
                      />
                    </label>
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      aria-label={`Remove item ${index + 1}`}
                      disabled={items.length === 1}
                      onClick={() =>
                        setItems((current) => current.filter((row) => row.id !== item.id))
                      }
                    >
                      <Trash2 />
                    </Button>
                  </div>
                ))}
              </div>
            </div>
            <FormError error={status.error} />
          </Card>
          <Card className="h-fit bg-[#172747] p-6 text-white sm:p-7">
            <p className="text-xs font-bold uppercase tracking-[.16em] text-sky-300">
              Invoice total
            </p>
            <p className="mt-3 text-4xl font-bold">{currency(total)}</p>
            <div className="mt-7 space-y-3 border-y border-white/15 py-5 text-sm text-slate-300">
              <div className="flex justify-between">
                <span>Line items</span>
                <strong className="text-white">{items.length}</strong>
              </div>
              <div className="flex justify-between">
                <span>Payment status</span>
                <strong className="text-white">Unpaid</strong>
              </div>
            </div>
            <Button
              className="mt-7 w-full bg-white text-slate-900 hover:bg-slate-100"
              size="lg"
              disabled={!valid || status.isLoading}
              onClick={() => void submit()}
            >
              {status.isLoading ? 'Recording invoice…' : 'Create invoice'}
            </Button>
            {!valid && (
              <p className="mt-4 text-xs leading-5 text-slate-300">
                Select a supplier and complete every item with a name, positive quantity, and unit
                cost.
              </p>
            )}
          </Card>
        </div>
      )}
    </div>
  );
}
