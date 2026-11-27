import { Download, PackageX, ScrollText, TrendingUp } from 'lucide-react';
import { Link } from '@tanstack/react-router';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { StockHealthVisual } from '../components/stock-health-visual';
import { MetricCard } from '../components/metric-card';
import { SalesSummaryReport } from '../components/sales-summary-report';
import { RevenueTrendsReport } from '../components/revenue-trends-report';
import { OrdersByStatusReport } from '../components/orders-by-status-report';
import { useOwnerReports } from '../hooks/use-owner-reports';
import { stockCsv } from '../model/stock-report';

/** Data exports are snapshots of the requested page only, not enterprise-wide analytics. */
export function OwnerReportsPage() {
  const data = useOwnerReports();
  const exportReport = () => {
    const file = new Blob([stockCsv(data.sample)], { type: 'text/csv;charset=utf-8' });
    const url = URL.createObjectURL(file);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'nexora-inventory-sample.csv';
    link.click();
    URL.revokeObjectURL(url);
  };
  return (
    <div className="space-y-7">
      <PageHeader
        eyebrow="OPERATIONAL REPORTS"
        title="Sales and inventory insights"
        description="Review server-calculated sales performance and actionable inventory visibility."
        action={
          <Button variant="outline" disabled={!data.sample.length} onClick={exportReport}>
            <Download size={16} /> Export loaded rows
          </Button>
        }
      />
      <SalesSummaryReport />
      <RevenueTrendsReport />
      <OrdersByStatusReport />
      {data.inventory.isLoading ||
      data.missingProducts.isLoading ||
      data.missingVariants.isLoading ? (
        <LoadingState />
      ) : data.inventory.isError || data.missingProducts.isError || data.missingVariants.isError ? (
        <ErrorState
          error={data.inventory.error || data.missingProducts.error || data.missingVariants.error}
        />
      ) : (
        <>
          <div className="grid gap-4 sm:grid-cols-3">
            <MetricCard
              icon={<PackageX size={18} />}
              label="Out-of-stock products"
              value={data.missingProducts.data?.totalCount ?? 0}
              note="Server-reported total"
            />
            <MetricCard
              icon={<ScrollText size={18} />}
              label="Out-of-stock variants"
              value={data.missingVariants.data?.totalCount ?? 0}
              note="Server-reported total"
            />
            <MetricCard
              icon={<TrendingUp size={18} />}
              label="Reserved units"
              value={data.sampleSummary.reservedUnits}
              note={`Across ${data.sample.length} loaded rows`}
            />
          </div>
          <div className="grid gap-5 xl:grid-cols-[1.4fr_1fr]">
            <Card className="p-6 sm:p-8">
              <h2 className="text-xl font-bold">Stock health breakdown</h2>
              <p className="mt-2 text-sm text-slate-500">
                First {data.sample.length} variants loaded (up to 50) — not a whole-inventory
                percentage.
              </p>
              <div className="mt-9">
                <StockHealthVisual summary={data.sampleSummary} />
              </div>
            </Card>
            <Card className="p-6 sm:p-8">
              <h2 className="text-xl font-bold">Next actions</h2>
              <div className="mt-6 space-y-4 text-sm text-slate-600">
                <p>Review products without available stock before promoting them to customers.</p>
                <p>Check reserved units against on-hand quantity and secured COD allocations.</p>
                <p>Replenish low-stock variants with the inventory adjustment workflow.</p>
              </div>
              <Button asChild className="mt-8">
                <Link to="/owner/inventory">Open inventory</Link>
              </Button>
            </Card>
          </div>
        </>
      )}
    </div>
  );
}
