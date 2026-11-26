import { useRef, useState } from 'react';
import { Link } from '@tanstack/react-router';
import { Plus, ReceiptText, Trash2 } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { FormError } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { currency } from '@/shared/lib/format';
import { useCreateInStoreSaleMutation } from '@/shared/api/in-store-sales-api';

type SaleItem = { id: string; itemName: string; unitPrice: string; quantity: string };

const emptyItem = (): SaleItem => ({ id: crypto.randomUUID(), itemName: '', unitPrice: '', quantity: '1' });

export function InStoreSaleCreatePage() {
  const [items, setItems] = useState<SaleItem[]>([emptyItem()]);
  const [receipt, setReceipt] = useState<{ saleNumber?: string; totalAmount?: number; created?: boolean }>();
  const submissionId = useRef<string | null>(null);
  const [createSale, status] = useCreateInStoreSaleMutation();
  const validItems = items.map((item) => ({
    itemName: item.itemName.trim(),
    unitPrice: Number(item.unitPrice),
    quantity: Number(item.quantity),
  }));
  const isValid = validItems.length > 0 && validItems.every((item) =>
    item.itemName.length > 0 && Number.isFinite(item.unitPrice) && item.unitPrice >= 0 &&
    Number.isSafeInteger(item.quantity) && item.quantity > 0,
  );
  const total = validItems.reduce((sum, item) =>
    sum + (Number.isFinite(item.unitPrice) && Number.isFinite(item.quantity) ? item.unitPrice * item.quantity : 0), 0);

  const updateItem = (id: string, field: keyof Omit<SaleItem, 'id'>, value: string) => {
    setReceipt(undefined);
    submissionId.current = null;
    setItems((current) => current.map((item) => item.id === id ? { ...item, [field]: value } : item));
  };
  const addItem = () => {
    setReceipt(undefined);
    submissionId.current = null;
    setItems((current) => [...current, emptyItem()]);
  };
  const removeItem = (id: string) => {
    setReceipt(undefined);
    submissionId.current = null;
    setItems((current) => current.length === 1 ? current : current.filter((item) => item.id !== id));
  };
  const submit = async () => {
    if (!isValid || status.isLoading) return;
    submissionId.current ??= crypto.randomUUID();
    try {
      const result = await createSale({ submissionId: submissionId.current, items: validItems }).unwrap();
      setReceipt({
        saleNumber: result.sale?.saleNumber,
        totalAmount: result.sale?.totalAmount,
        created: result.created,
      });
    } catch {
      // The API error is rendered below the form.
    }
  };
  const startAnotherSale = () => {
    setItems([emptyItem()]);
    setReceipt(undefined);
    submissionId.current = null;
    status.reset();
  };

  return <div className="max-w-5xl space-y-7">
    <PageHeader eyebrow="POINT OF SALE" title="Record an in-store sale" description="Create a completed sale for a customer buying directly at the store." action={<Button asChild variant="outline"><Link to="/owner/in-store-sales/history">View sale history</Link></Button>} />
    {receipt ? <Card className="border-emerald-200 bg-emerald-50 p-6 sm:p-8">
      <h2 className="text-xl font-bold text-emerald-950">Sale recorded</h2>
      <p className="mt-2 text-sm text-emerald-900">{receipt.saleNumber ? `Sale ${receipt.saleNumber}` : 'The in-store sale'} {receipt.created === false ? 'was already recorded from this submission' : 'was created'} for {currency(receipt.totalAmount)}.</p>
      <Button className="mt-5" onClick={startAnotherSale}><Plus size={16} /> Record another sale</Button>
    </Card> : <Card className="p-6 sm:p-8">
      <div className="flex items-start gap-3"><ReceiptText className="mt-0.5 text-blue-600" size={23} /><div><h2 className="text-xl font-bold">Sale items</h2><p className="mt-1 text-sm text-slate-500">Enter the name, price, and quantity for every item sold.</p></div></div>
      <div className="mt-7 space-y-4">
        {items.map((item, index) => <div key={item.id} className="grid gap-3 rounded-2xl border border-slate-200 p-4 sm:grid-cols-[minmax(0,1fr)_140px_120px_auto] sm:items-end">
          <label className="block text-xs font-semibold text-slate-700">Item name<Input className="mt-1.5" value={item.itemName} onChange={(event) => updateItem(item.id, 'itemName', event.target.value)} placeholder="e.g. USB-C charger" /></label>
          <label className="block text-xs font-semibold text-slate-700">Unit price (JOD)<Input className="mt-1.5" type="number" min="0" step="0.001" value={item.unitPrice} onChange={(event) => updateItem(item.id, 'unitPrice', event.target.value)} placeholder="0.000" /></label>
          <label className="block text-xs font-semibold text-slate-700">Quantity<Input className="mt-1.5" type="number" min="1" step="1" value={item.quantity} onChange={(event) => updateItem(item.id, 'quantity', event.target.value)} /></label>
          <Button type="button" variant="outline" size="icon" aria-label={`Remove item ${index + 1}`} disabled={items.length === 1} onClick={() => removeItem(item.id)}><Trash2 /></Button>
        </div>)}
      </div>
      <Button type="button" variant="outline" className="mt-5" onClick={addItem}><Plus size={16} /> Add item</Button>
      <div className="mt-8 flex flex-col gap-4 border-t border-slate-200 pt-6 sm:flex-row sm:items-center sm:justify-between"><div><p className="text-sm text-slate-500">Sale total</p><p className="text-2xl font-bold">{currency(total)}</p></div><Button type="button" size="lg" disabled={!isValid || status.isLoading} onClick={() => void submit()}>{status.isLoading ? 'Recording sale…' : 'Record sale'} <ReceiptText size={16} /></Button></div>
      {!isValid && <p className="mt-4 text-xs text-slate-500">Add an item name, a non-negative unit price, and a quantity of at least one.</p>}
      <div className="mt-4"><FormError error={status.error} /></div>
    </Card>}
  </div>;
}
