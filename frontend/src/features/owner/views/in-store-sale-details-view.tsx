import { useState } from 'react';
import { getRouteApi, Link } from '@tanstack/react-router';
import { ArrowLeft, Pencil, Plus, ReceiptText, ShoppingBag, Trash2 } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { currency, date } from '@/shared/lib/format';
import {
  inStoreSaleStatusLabel,
  useGetInStoreSaleByIdQuery,
  useUpdateInStoreSaleMutation,
} from '@/shared/api/in-store-sales-api';

const route = getRouteApi('/owner/in-store-sales/$saleId');

type EditableItem = { id: string; itemName: string; unitPrice: string; quantity: string };

const newItem = (): EditableItem => ({
  id: crypto.randomUUID(),
  itemName: '',
  unitPrice: '',
  quantity: '1',
});

export function InStoreSaleDetailsPage() {
  const { saleId } = route.useParams();
  const id = Number(saleId);
  const validId = Number.isSafeInteger(id) && id > 0;
  const sale = useGetInStoreSaleByIdQuery({ id }, { skip: !validId });
  const [editing, setEditing] = useState(false);
  const [items, setItems] = useState<EditableItem[]>([]);
  const [updateSale, updateStatus] = useUpdateInStoreSaleMutation();
  if (!validId) return <ErrorState error={new Error('Invalid in-store sale ID')} />;
  if (sale.isLoading) return <LoadingState message="Loading in-store sale…" />;
  if (sale.isError) return <ErrorState error={sale.error} onRetry={() => void sale.refetch()} />;
  const data = sale.data;
  if (!data) return <ErrorState error={new Error('The in-store sale response is empty.')} />;

  const editable = data.status === 1 && Boolean(data.rowVersion);
  const resetItems = () =>
    setItems(
      (data.items ?? []).map((item) => ({
        id: crypto.randomUUID(),
        itemName: item.itemName ?? '',
        unitPrice: item.unitPrice?.toString() ?? '',
        quantity: item.quantity?.toString() ?? '1',
      })),
    );
  const beginEditing = () => {
    resetItems();
    updateStatus.reset();
    setEditing(true);
  };
  const parsedItems = items.map((item) => ({
    itemName: item.itemName.trim(),
    unitPrice: Number(item.unitPrice),
    quantity: Number(item.quantity),
  }));
  const validItems =
    parsedItems.length > 0 &&
    parsedItems.every(
      (item) =>
        item.itemName.length > 0 &&
        Number.isFinite(item.unitPrice) &&
        item.unitPrice >= 0 &&
        Number.isSafeInteger(item.quantity) &&
        item.quantity > 0,
    );
  const editTotal = parsedItems.reduce(
    (total, item) =>
      total +
      (Number.isFinite(item.unitPrice) && Number.isFinite(item.quantity)
        ? item.unitPrice * item.quantity
        : 0),
    0,
  );
  const changeItem = (itemId: string, field: keyof Omit<EditableItem, 'id'>, value: string) =>
    setItems((current) =>
      current.map((item) => (item.id === itemId ? { ...item, [field]: value } : item)),
    );
  const save = async () => {
    if (!data.rowVersion || !validItems || updateStatus.isLoading) return;
    try {
      await updateSale({ id, body: { items: parsedItems, rowVersion: data.rowVersion } }).unwrap();
      setEditing(false);
      await sale.refetch();
    } catch {
      // The API error is rendered below the editor; a 409 means the sale changed elsewhere.
    }
  };

  return (
    <div className="max-w-5xl space-y-7">
      <Button asChild variant="outline" size="sm">
        <Link to="/owner/in-store-sales/history">
          <ArrowLeft /> Back to sale history
        </Link>
      </Button>
      <PageHeader
        eyebrow="POINT OF SALE"
        title={data.saleNumber ?? `Sale #${id}`}
        description={`Sold ${date(data.soldAtUtc)} · ${inStoreSaleStatusLabel(data.status)}`}
        action={
          editable && !editing ? (
            <Button onClick={beginEditing}>
              <Pencil size={16} /> Edit sale
            </Button>
          ) : undefined
        }
      />
      {editing && (
        <Card className="p-6 sm:p-8">
          <div className="flex items-start justify-between gap-4">
            <div>
              <h2 className="text-xl font-bold">Edit sale items</h2>
              <p className="mt-1 text-sm text-slate-500">
                Updating the sale replaces its item list.
              </p>
            </div>
            <Button
              variant="outline"
              onClick={() => {
                setEditing(false);
                updateStatus.reset();
              }}
            >
              Cancel
            </Button>
          </div>
          <div className="mt-6 space-y-4">
            {items.map((item, index) => (
              <div
                key={item.id}
                className="grid gap-3 rounded-2xl border border-slate-200 p-4 sm:grid-cols-[minmax(0,1fr)_140px_120px_auto] sm:items-end"
              >
                <label className="block text-xs font-semibold text-slate-700">
                  Item name
                  <Input
                    className="mt-1.5"
                    value={item.itemName}
                    onChange={(event) => changeItem(item.id, 'itemName', event.target.value)}
                  />
                </label>
                <label className="block text-xs font-semibold text-slate-700">
                  Unit price (JOD)
                  <Input
                    className="mt-1.5"
                    type="number"
                    min="0"
                    step="0.001"
                    value={item.unitPrice}
                    onChange={(event) => changeItem(item.id, 'unitPrice', event.target.value)}
                  />
                </label>
                <label className="block text-xs font-semibold text-slate-700">
                  Quantity
                  <Input
                    className="mt-1.5"
                    type="number"
                    min="1"
                    step="1"
                    value={item.quantity}
                    onChange={(event) => changeItem(item.id, 'quantity', event.target.value)}
                  />
                </label>
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  aria-label={`Remove item ${index + 1}`}
                  disabled={items.length === 1}
                  onClick={() =>
                    setItems((current) => current.filter((entry) => entry.id !== item.id))
                  }
                >
                  <Trash2 />
                </Button>
              </div>
            ))}
          </div>
          <Button
            type="button"
            variant="outline"
            className="mt-5"
            onClick={() => setItems((current) => [...current, newItem()])}
          >
            <Plus size={16} /> Add item
          </Button>
          <div className="mt-7 flex flex-col gap-4 border-t border-slate-200 pt-5 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <p className="text-sm text-slate-500">Updated total</p>
              <p className="text-2xl font-bold">{currency(editTotal)}</p>
            </div>
            <Button disabled={!validItems || updateStatus.isLoading} onClick={() => void save()}>
              {updateStatus.isLoading ? 'Saving…' : 'Save changes'}
            </Button>
          </div>
          {!validItems && (
            <p className="mt-4 text-xs text-slate-500">
              Add an item name, a non-negative unit price, and a quantity of at least one.
            </p>
          )}
          <div className="mt-4">
            <FormError error={updateStatus.error} />
          </div>
        </Card>
      )}
      <div className="grid gap-6 lg:grid-cols-[1.45fr_1fr]">
        <Card className="overflow-hidden">
          <div className="flex items-center gap-3 border-b border-slate-200 p-6">
            <ShoppingBag className="text-blue-600" size={22} />
            <div>
              <h2 className="text-xl font-bold">Items</h2>
              <p className="mt-1 text-sm text-slate-500">
                Products included in this in-store transaction.
              </p>
            </div>
          </div>
          {(data.items?.length ?? 0) === 0 ? (
            <p className="p-8 text-center text-sm text-slate-500">
              This sale has no recorded items.
            </p>
          ) : (
            <div className="overflow-x-auto">
              <table className="min-w-full text-left text-sm">
                <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
                  <tr>
                    <th className="px-6 py-4">Item</th>
                    <th className="px-6 py-4 text-right">Unit price</th>
                    <th className="px-6 py-4 text-right">Qty.</th>
                    <th className="px-6 py-4 text-right">Line total</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {data.items?.map((item) => (
                    <tr key={item.inStoreSaleItemId}>
                      <td className="px-6 py-4 font-semibold text-slate-900">
                        {item.itemName ?? '—'}
                      </td>
                      <td className="px-6 py-4 text-right text-slate-600">
                        {currency(item.unitPrice)}
                      </td>
                      <td className="px-6 py-4 text-right text-slate-600">
                        {item.quantity ?? '—'}
                      </td>
                      <td className="px-6 py-4 text-right font-semibold">
                        {currency(item.lineTotal)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </Card>
        <Card className="h-fit p-6">
          <div className="flex items-center gap-3">
            <ReceiptText className="text-blue-600" size={22} />
            <h2 className="text-xl font-bold">Sale summary</h2>
          </div>
          <dl className="mt-6 space-y-4 text-sm">
            <div className="flex justify-between gap-4">
              <dt className="text-slate-500">Sale number</dt>
              <dd className="font-semibold text-right">{data.saleNumber ?? '—'}</dd>
            </div>
            <div className="flex justify-between gap-4">
              <dt className="text-slate-500">Sold at</dt>
              <dd className="font-semibold text-right">{date(data.soldAtUtc)}</dd>
            </div>
            <div className="flex justify-between gap-4">
              <dt className="text-slate-500">Status</dt>
              <dd className="font-semibold">{inStoreSaleStatusLabel(data.status)}</dd>
            </div>
            {data.voidedAtUtc && (
              <div className="flex justify-between gap-4">
                <dt className="text-slate-500">Voided at</dt>
                <dd className="font-semibold text-right">{date(data.voidedAtUtc)}</dd>
              </div>
            )}
            <div className="flex justify-between gap-4 border-t border-slate-200 pt-4 text-lg">
              <dt className="font-bold">Total</dt>
              <dd className="font-bold">{currency(data.totalAmount)}</dd>
            </div>
          </dl>
        </Card>
      </div>
    </div>
  );
}
