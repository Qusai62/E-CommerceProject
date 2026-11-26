import { useState, type FormEvent } from 'react';
import { Pencil, Plus, Trash2 } from 'lucide-react';
import { useSpecificationCommands } from '../hooks/use-product-editor-commands';
import type { ProductSpecVM } from '@/features/catalog/model/product';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { ConfirmAction } from '@/components/ui/alert-dialog';
import { Field, FormError } from '@/components/ui/feedback';

export function SpecificationManager({ productId, specs }: { productId: number; specs: ProductSpecVM[] }) {
  const commands = useSpecificationCommands(productId);
  const [name, setName] = useState('');
  const [value, setValue] = useState('');
  const [sortOrder, setSortOrder] = useState('0');
  const [editingId, setEditingId] = useState<number | null>(null);
  const clear = () => { setName(''); setValue(''); setSortOrder('0'); setEditingId(null); };
  const edit = (spec: ProductSpecVM) => {
    if (spec.id === undefined) return;
    setEditingId(spec.id); setName(spec.name); setValue(spec.value); setSortOrder(String(spec.sortOrder));
  };
  const save = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!name.trim() || !value.trim()) return;
    const body = { name: name.trim(), value: value.trim(), sortOrder: Number(sortOrder) };
    try {
      if (editingId === null) await commands.add(body);
      else await commands.update(editingId, body);
      clear();
    } catch { /* Error shown in the form. */ }
  };

  return <section>
    <h2 className="font-display text-3xl">Technical specifications</h2>
    <p className="mt-2 text-sm text-stone-500">Help shoppers compare materials, measurements and product features.</p>
    <div className="mt-6 space-y-3">{specs.map((spec, index) => <div key={spec.id ?? index} className="flex flex-wrap items-center justify-between gap-4 rounded-xl bg-muted px-4 py-3 text-sm">
      <div><b>{spec.name}</b><p className="mt-1 text-stone-600">{spec.value}</p></div>
      {spec.id !== undefined ? <div className="flex gap-1"><Button variant="ghost" size="sm" onClick={() => edit(spec)}><Pencil size={13} /> Edit</Button><ConfirmAction destructive title="Remove specification?" description="This specification will no longer appear on the product." trigger={<Button size="sm" variant="ghost"><Trash2 size={13} /> Delete</Button>} onConfirm={async () => { try { await commands.remove(spec.id!); } catch { /* Error rendered inline. */ } }} /></div> : <span className="text-xs text-stone-500">Read only (no ID)</span>}
    </div>)}{!specs.length && <p className="rounded-xl bg-muted p-4 text-sm text-stone-500">No specifications have been returned.</p>}</div>
    <form className="mt-8 space-y-5 border-t border-line pt-7" onSubmit={(e) => void save(e)}>
      <h3 className="font-semibold">{editingId === null ? 'Add specification' : 'Edit specification'}</h3>
      <div className="grid gap-4 sm:grid-cols-2"><Field label="Specification name"><Input required value={name} onChange={(e) => setName(e.target.value)} /></Field><Field label="Value"><Input required value={value} onChange={(e) => setValue(e.target.value)} /></Field></div>
      <Field label="Sort order"><Input type="number" min="0" step="1" value={sortOrder} onChange={(e) => setSortOrder(e.target.value)} /></Field>
      <FormError error={commands.error} />
      {(commands.isSuccess) && <p role="status" className="text-sm text-moss">Specification changes saved.</p>}
      <div className="flex gap-2"><Button type="submit" disabled={!name.trim() || !value.trim() || commands.isLoading}><Plus size={16} /> {editingId === null ? 'Add specification' : 'Save specification'}</Button>{editingId !== null && <Button variant="secondary" type="button" onClick={clear}>Cancel edit</Button>}</div>
    </form>
  </section>;
}
