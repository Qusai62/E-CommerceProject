import { X } from 'lucide-react';
import type { ProductListItemResponse } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { EmptyState } from '@/components/ui/feedback';
import { ProductCard } from './product-card';

interface CatalogResultsProps {
  products: ProductListItemResponse[];
  onReset: () => void;
}
export function CatalogResults({ products, onReset }: CatalogResultsProps) {
  if (!products.length) {
    return (
      <EmptyState
        icon={<X size={27} />}
        title="No matching electronics"
        description="Try changing the price range or removing a filter."
        action={
          <Button variant="secondary" onClick={onReset}>
            Reset filters
          </Button>
        }
      />
    );
  }
  return (
    <div className="grid gap-x-5 gap-y-10 sm:grid-cols-2 xl:grid-cols-3">
      {products.map((p) => (
        <ProductCard key={p.productId} product={p} />
      ))}
    </div>
  );
}
