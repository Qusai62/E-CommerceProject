import type { summarizeLoadedStock } from '../model/stock-report';

type StockSummary = ReturnType<typeof summarizeLoadedStock>;
const buckets = [
  { key: 'healthy', label: 'Healthy', color: 'bg-emerald-500' },
  { key: 'lowStock', label: 'Low (1–5)', color: 'bg-amber-400' },
  { key: 'outOfStock', label: 'Out', color: 'bg-rose-500' },
] as const;
export function StockHealthVisual({ summary }: { summary: StockSummary }) {
  return <div><div className="flex h-4 w-full overflow-hidden rounded-full bg-slate-100" role="img" aria-label={`Loaded sample: ${summary.healthy} healthy, ${summary.lowStock} low stock, ${summary.outOfStock} out of stock`}>
    {buckets.map((bucket) => <div key={bucket.key} className={bucket.color} style={{ width: `${summary.total ? summary[bucket.key] / summary.total * 100 : 0}%` }}/>)}</div>
    <div className="mt-5 grid grid-cols-3 gap-2">{buckets.map((bucket) => <div key={bucket.key} className="text-xs"><p className="flex items-center gap-2 text-slate-500"><span className={`size-2.5 shrink-0 rounded-full ${bucket.color}`}/>{bucket.label}</p><p className="mt-2 text-xl font-bold tabular-nums">{summary[bucket.key]}</p></div>)}</div>
  </div>;
}
