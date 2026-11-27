import type { ReactNode } from 'react';
import { Card } from '@/components/ui/card';
export function MetricCard({
  icon,
  label,
  value,
  note,
}: {
  icon: ReactNode;
  label: string;
  value: string | number;
  note: string;
}) {
  return (
    <Card className="relative overflow-hidden border-slate-200 bg-white p-5 shadow-sm sm:p-6">
      <div className="flex items-start justify-between gap-2">
        <span className="text-sm font-medium text-slate-500">{label}</span>
        <span className="grid size-10 shrink-0 place-items-center rounded-2xl bg-blue-50 text-blue-600">
          {icon}
        </span>
      </div>
      <p className="mt-4 text-3xl font-bold tracking-tight tabular-nums text-slate-900 sm:text-4xl">
        {value}
      </p>
      <p className="mt-3 text-xs leading-5 text-slate-500">{note}</p>
    </Card>
  );
}
