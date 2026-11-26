import { Link } from '@tanstack/react-router';
import { ArrowRight, BarChart3, Boxes, Layers3, PackageX, Warehouse } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { PageHeader } from '@/components/layout/site-layout';
import { MetricCard } from '../components/metric-card';
import { StockHealthVisual } from '../components/stock-health-visual';
import { useOwnerReports } from '../hooks/use-owner-reports';

export function OwnerDashboard() {
  const report = useOwnerReports();
  const loading = report.products.isLoading || report.inventory.isLoading || report.missingVariants.isLoading;
  const error = report.products.error || report.inventory.error || report.missingVariants.error;
  return <div className="space-y-8"><PageHeader eyebrow="MERCHANT OVERVIEW" title="Your store at a glance." description="Manage the electronics catalog, track available stock, and handle the daily essentials." action={<Button asChild><Link to="/owner/products/new">Add product <ArrowRight size={16}/></Link></Button>}/>
    {loading ? <LoadingState/> : error ? <ErrorState error={error}/> : <>
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      <MetricCard icon={<Boxes size={20}/>} label="Catalog products" value={report.products.data?.totalCount ?? 0} note="Server-reported listing total"/>
      <MetricCard icon={<Layers3 size={20}/>} label="Stock-keeping units" value={report.inventory.data?.totalCount ?? 0} note="Server-reported inventory total"/>
      <MetricCard icon={<PackageX size={20}/>} label="Out-of-stock variants" value={report.missingVariants.data?.totalCount ?? 0} note="Requires replenishment"/>
      <MetricCard icon={<Warehouse size={20}/>} label="Available units" value={report.sampleSummary.availableUnits} note={`First ${report.sampleSummary.total} loaded inventory rows only`}/>
    </div>
    <div className="grid gap-5 xl:grid-cols-[1.35fr_1fr]">
      <Card className="p-6 sm:p-8"><div className="flex flex-wrap items-start justify-between gap-4"><div><h2 className="text-xl font-bold">Inventory health</h2><p className="mt-1 text-sm text-slate-500">Breakdown of {report.sample.length} loaded SKUs, not all stock.</p></div><Button asChild variant="outline" size="sm"><Link to="/owner/reports">Reports <ArrowRight size={14}/></Link></Button></div><div className="mt-9"><StockHealthVisual summary={report.sampleSummary}/></div></Card>
      <Card className="flex flex-col justify-between bg-[#172747] p-6 text-white sm:p-8"><div><BarChart3 className="text-sky-300" size={30}/><h2 className="mt-6 text-2xl font-bold tracking-tight">Operations, in control.</h2><p className="mt-3 text-sm leading-7 text-slate-300">Review stock levels, quickly replenish popular devices, and organize your electronics catalog.</p></div><div className="mt-8 flex flex-wrap gap-3"><Button asChild className="bg-white text-slate-900 hover:bg-slate-100"><Link to="/owner/inventory">Manage inventory</Link></Button><Button asChild variant="outline" className="border-white/30 text-white hover:bg-white/10"><Link to="/owner/categories">Categories</Link></Button></div></Card>
    </div>
    <p className="text-xs text-slate-500">Financial sales and order-wide revenue reports require dedicated StoreOwner reporting endpoints not included in the provided Swagger.</p>
    </>}
  </div>;
}
