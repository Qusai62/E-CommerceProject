import { Link } from '@tanstack/react-router';
import { ArrowRight } from 'lucide-react';
import type { ProductListItemResponse } from '@/shared/api/generated/api.generated';
import { ProductCard } from '@/features/catalog/components/product-card';
import { Button } from '@/components/ui/button';
import { ErrorState } from '@/components/ui/feedback';
import { Skeleton } from '@/components/ui/skeleton';

interface FeaturedProductsProps {
  items: ProductListItemResponse[];
  isLoading: boolean;
  error?: unknown;
  onRetry: () => void;
}
export function FeaturedProducts({ items, isLoading, error, onRetry }: FeaturedProductsProps) {
  return (
    <section className="page-container py-16 sm:py-20" aria-labelledby="featured-title">
      <div className="mb-8 flex flex-wrap items-end justify-between gap-5">
        <div>
          <p className="eyebrow">EXPLORE THE RANGE</p>
          <h2 id="featured-title" className="font-display mt-2 text-3xl sm:text-4xl">
            Popular tech picks
          </h2>
          <p className="mt-2 text-sm text-slate-500">
            Find your next upgrade, from everyday essentials to powerful hardware.
          </p>
        </div>
        <Button asChild variant="outline">
          <Link to="/shop">
            View all products <ArrowRight size={15} />
          </Link>
        </Button>
      </div>
      {isLoading ? (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {Array.from({ length: 4 }, (_, index) => (
            <Skeleton key={index} className="h-80" />
          ))}
        </div>
      ) : error ? (
        <ErrorState error={error} onRetry={onRetry} />
      ) : items.length ? (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {items.map((product) => (
            <ProductCard key={product.productId} product={product} />
          ))}
        </div>
      ) : (
        <p className="rounded-xl border border-line bg-white p-10 text-center text-sm text-slate-500">
          No products are published yet.
        </p>
      )}
    </section>
  );
}
