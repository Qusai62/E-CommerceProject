import { Link } from '@tanstack/react-router';
import { CheckCircle2, ArrowRight, ClipboardList } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';

export function OrderConfirmationView({ orderNumber, orderId }: { orderNumber: string; orderId: number }) {
  if (!orderNumber && !orderId) return <div className="page-container grid min-h-[60vh] place-items-center py-16"><Card className="w-full max-w-lg p-9 text-center"><h1 className="font-display text-2xl">No order confirmation found</h1><p className="mt-3 text-sm text-slate-500">Finish checkout to receive a reference number.</p><Button asChild className="mt-6"><Link to="/cart">View cart</Link></Button></Card></div>;
  return <div className="page-container grid min-h-[70vh] place-items-center py-16">
    <Card className="w-full max-w-xl p-7 text-center sm:p-12">
      <div className="mx-auto grid size-20 place-items-center rounded-full bg-emerald-50 text-emerald-700"><CheckCircle2 size={40} strokeWidth={1.6} /></div>
      <p className="eyebrow mt-7">ORDER SUBMITTED</p>
      <h1 className="font-display mt-3 text-3xl sm:text-4xl">Thank you for your order.</h1>
      <p className="mt-4 text-sm leading-7 text-slate-600">Your order was created by the E-Commerce API. Payment completion and delivery tracking depend on your backend payment and order-status features.</p>
      <div className="mt-8 rounded-2xl border border-line bg-slate-50 px-5 py-6"><ClipboardList className="mx-auto text-blue-600" /><p className="mt-3 text-xs font-semibold uppercase tracking-widest text-slate-500">Order reference</p><p className="mt-2 break-all text-xl font-extrabold tabular-nums">{orderNumber || (orderId ? `#${orderId}` : 'Unavailable')}</p></div>
      <p className="mt-4 text-xs leading-5 text-slate-500">Save your reference number. You can review your order history and order details in your account.</p>
      <div className="mt-8 flex flex-col gap-3 sm:flex-row"><Button asChild className="flex-1"><Link to="/account/orders">My orders <ArrowRight size={16}/></Link></Button><Button asChild variant="outline" className="flex-1"><Link to="/shop">Keep shopping</Link></Button></div>
    </Card>
  </div>;
}
