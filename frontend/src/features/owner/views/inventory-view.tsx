import { useState } from 'react';
import { Boxes, TriangleAlert } from 'lucide-react';
import type { StoreOwnerStockResponse } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';
import { InventoryTable } from '../components/inventory-table';
import { StockAdjustmentDialog } from '../components/stock-adjustment-dialog';
import { useAdjustInventory, useInventoryList } from '../hooks/use-inventory';

export function OwnerInventoryPage() {
  const [page, setPage] = useState(1);
  const [onlyOut, setOnlyOut] = useState(false);
  const [selected, setSelected] = useState<StoreOwnerStockResponse | null>(null);
  const query = useInventoryList(page, 10, onlyOut);
  const adjust = useAdjustInventory();
  const rows = query.data?.items ?? [];
  const chooseMode = (out: boolean) => {
    setOnlyOut(out);
    setPage(1);
  };
  const open = (item: StoreOwnerStockResponse) => {
    adjust.reset();
    setSelected(item);
  };
  const close = () => {
    adjust.reset();
    setSelected(null);
  };
  return (
    <div className="py-2">
      <PageHeader
        eyebrow="STORE MANAGEMENT"
        title="Inventory"
        description="Track availability after reservations and COD allocations. Increase or decrease on-hand stock safely."
      />

      <div className="mt-7 flex flex-wrap items-center justify-between gap-3">
        <div className="flex gap-2">
          <Button
            size="sm"
            variant={!onlyOut ? 'default' : 'secondary'}
            onClick={() => chooseMode(false)}
          >
            All variants
          </Button>
          <Button
            size="sm"
            variant={onlyOut ? 'default' : 'secondary'}
            onClick={() => chooseMode(true)}
          >
            <TriangleAlert size={14} /> Out of stock
          </Button>
        </div>
        <span className="text-xs text-slate-500">
          {query.data?.totalCount ?? '—'} inventory records
        </span>
      </div>
      {query.isLoading ? (
        <LoadingState />
      ) : query.isError ? (
        <div className="mt-6">
          <ErrorState error={query.error} onRetry={() => void query.refetch()} />
        </div>
      ) : !rows.length ? (
        <EmptyState
          icon={<Boxes size={28} />}
          title="No matching inventory"
          description="There are no records for the current selection."
        />
      ) : (
        <InventoryTable rows={rows} onAdjust={open} />
      )}
      <div className="mt-6 flex items-center justify-between">
        <p className="text-sm text-slate-500">
          Page {page} of {Math.max(1, query.data?.totalPages ?? 1)}
        </p>
        <div className="flex gap-2">
          <Button
            size="sm"
            variant="outline"
            disabled={!query.data?.hasPreviousPage}
            onClick={() => setPage((value) => value - 1)}
          >
            Previous
          </Button>
          <Button
            size="sm"
            variant="outline"
            disabled={!query.data?.hasNextPage}
            onClick={() => setPage((value) => value + 1)}
          >
            Next
          </Button>
        </div>
      </div>
      {selected && (
        <StockAdjustmentDialog
          key={selected.productVariantId}
          stock={selected}
          pending={adjust.pending}
          error={adjust.error}
          onClose={close}
          onSave={async (mode, quantity) => {
            try {
              await adjust.save(selected.productVariantId ?? 0, mode, quantity);
              close();
            } catch {
              /* Server errors are rendered by the dialog. */
            }
          }}
        />
      )}
    </div>
  );
}
