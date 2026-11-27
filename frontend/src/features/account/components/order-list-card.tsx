import { Link } from '@tanstack/react-router';
import { ArrowUpRight, CalendarDays, Package } from 'lucide-react';
import type { OrderSummary } from '../model/order-list';
import { fulfillmentStatusLabel } from '../model/fulfillment-status';
import { Card } from '@/components/ui/card';
import { currency, date } from '@/shared/lib/format';

export function OrderListCard({ order }: { order: OrderSummary }) {
  return (
    <Card className="flex flex-col justify-between gap-5 p-5 sm:flex-row sm:items-center sm:p-6">
      <div className="flex gap-4">
        <div className="grid size-12 shrink-0 place-items-center rounded-xl bg-blue-50 text-blue-600">
          <Package size={22} />
        </div>
        <div>
          <p className="text-base font-bold text-slate-900">{order.number}</p>
          <p className="mt-1 flex items-center gap-2 text-xs text-slate-500">
            <CalendarDays size={14} />
            {date(order.date)}
          </p>
          <p className="mt-2 text-xs text-slate-500">
            Status: {fulfillmentStatusLabel(order.status)}
          </p>
        </div>
      </div>
      <div className="flex items-center justify-between gap-5 border-t border-slate-100 pt-4 sm:border-none sm:pt-0">
        <span className="font-semibold">
          {order.total === null || order.total === undefined
            ? 'See details'
            : currency(order.total)}
        </span>
        <Link
          to="/account/orders/$orderId"
          params={{ orderId: String(order.id) }}
          className="inline-flex items-center gap-2 text-sm font-bold text-blue-600 hover:underline"
        >
          View details <ArrowUpRight size={15} />
        </Link>
      </div>
    </Card>
  );
}
