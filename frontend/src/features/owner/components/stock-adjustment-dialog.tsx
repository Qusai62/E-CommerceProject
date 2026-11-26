import { useState, type FormEvent } from 'react';
import * as Dialog from '@radix-ui/react-dialog';
import type { StoreOwnerStockResponse } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Field, FormError } from '@/components/ui/feedback';
import { parseStockQuantity, type AdjustmentMode } from '../model/stock-adjustment';

interface Props{
 stock:StoreOwnerStockResponse;
 pending:boolean;
 error:unknown;
 onClose:()=>void;
 onSave:(mode:AdjustmentMode,quantity:number)=>Promise<void>;
}
export function StockAdjustmentDialog({stock,pending,error,onClose,onSave}:Props){
  const [mode,setMode]=useState<AdjustmentMode>('increase');
  const [quantity,setQuantity]=useState('1');
  const parsed=parseStockQuantity(Number(quantity)); // Derived validation: don't store another boolean in state.
  async function submit(event:FormEvent<HTMLFormElement>){event.preventDefault();if(parsed){await onSave(mode,parsed);}}
  return <Dialog.Root open onOpenChange={open=>{if(!open)onClose();}}><Dialog.Portal>
    <Dialog.Overlay className="fixed inset-0 z-50 bg-slate-950/60 backdrop-blur-sm"/>
    <Dialog.Content className="fixed left-1/2 top-1/2 z-50 w-[min(92vw,480px)] -translate-x-1/2 -translate-y-1/2 rounded-2xl bg-white p-7 shadow-2xl">
      <Dialog.Title className="text-2xl font-bold">Adjust stock</Dialog.Title>
      <Dialog.Description className="mt-2 text-sm text-slate-500">{stock.productName} · {stock.sku}</Dialog.Description>
      <p className="mt-3 rounded-xl bg-blue-50 p-3 text-xs text-blue-900">On hand: {stock.onHandQuantity} · Reserved: {stock.reservedQuantity} · Available: {stock.availableQuantity}</p>
      <form className="mt-6 space-y-5" onSubmit={event=>void submit(event)}><Field label="Action"><select value={mode} onChange={event=>setMode(event.target.value as AdjustmentMode)} className="h-11 w-full rounded-xl border border-line bg-white px-4 text-sm"><option value="increase">Increase stock</option><option value="decrease">Decrease stock</option></select></Field>
      <Field label="Quantity"><Input autoFocus type="number" min="1" step="1" required value={quantity} onChange={event=>setQuantity(event.target.value)}/></Field>
      <FormError error={error}/><div className="flex justify-end gap-2"><Button type="button" variant="secondary" onClick={onClose}>Cancel</Button><Button type="submit" disabled={!parsed||pending}>{pending?'Applying…':'Apply adjustment'}</Button></div></form>
    </Dialog.Content>
  </Dialog.Portal></Dialog.Root>;
}
