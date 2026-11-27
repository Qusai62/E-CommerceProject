import { useState } from 'react';
import { getRouteApi, Link } from '@tanstack/react-router';
import { CheckCircle2, Minus, Plus, ShieldCheck, ShoppingBag, Truck } from 'lucide-react';
import { ProductVisual } from '../components/product-card';
import { useProductDetail } from '../hooks/use-product-detail';
import { currency } from '@/shared/lib/format';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { BackLink } from '@/components/layout/site-layout';
const productRoute = getRouteApi('/shop/$productId');

export function ProductDetailPage() {
  const { productId } = productRoute.useParams();
  const id = Number(productId);
  const [selectedVariant, setSelectedVariant] = useState<number | undefined>();
  const [qty, setQty] = useState(1);
  const {
    productQuery: query,
    product,
    variant,
    availability: stock,
    canAddToCart: canAdd,
    addCartStatus,
    addToCart,
  } = useProductDetail(id, selectedVariant, qty);
  const { isLoading: isAdding, error: addError, isSuccess } = addCartStatus;
  if (!Number.isSafeInteger(id) || id < 1) {
    return (
      <div className="page-container py-16">
        <ErrorState error={new Error('Invalid product identifier')} />
      </div>
    );
  }
  if (query.isLoading) return <LoadingState />;
  if (query.isError) {
    return (
      <div className="page-container py-16">
        <ErrorState error={query.error} onRetry={() => void query.refetch()} />
      </div>
    );
  }
  if (!product) {
    return (
      <div className="page-container py-16">
        <ErrorState
          error={
            new Error(
              'The API returned no recognizable product details. Please publish a response schema for GET /api/products/{id}.',
            )
          }
        />
      </div>
    );
  }
  return (
    <div className="page-container py-10 sm:py-16">
      <BackLink to="/shop">Back to electronics</BackLink>
      <div className="mt-8 grid gap-10 lg:grid-cols-[1.1fr_.9fr] lg:gap-16">
        <ProductVisual name={product.name} imageKey={product.imageKey} variant="detail" />
        <section className="flex flex-col justify-center">
          <p className="eyebrow">NEXORA / PRODUCT DETAILS</p>
          <h1 className="font-display mt-4 text-4xl leading-tight sm:text-5xl">{product.name}</h1>
          <div className="mt-5 flex items-center gap-4">
            <span className="text-2xl font-medium">
              {currency(variant?.price ?? product.price)}
            </span>
            <Badge className="bg-blue-50 text-blue-700">Electronics</Badge>
          </div>
          <p className="mt-8 max-w-md text-sm leading-8 text-stone-600">
            {product.description ||
              'See product details and available options before adding to your cart.'}
          </p>
          <div className="mt-8 border-t border-line pt-7">
            <p className="mb-3 text-xs font-semibold uppercase tracking-wide">Choose variant</p>
            {product.variants.length ? (
              <div className="flex flex-wrap gap-2">
                {product.variants.map((v) => (
                  <button
                    key={v.id}
                    type="button"
                    disabled={!v.isActive}
                    onClick={() => {
                      setSelectedVariant(v.id);
                      setQty(1);
                    }}
                    aria-pressed={variant?.id === v.id}
                    className={`rounded-xl border px-4 py-3 text-left text-xs transition-colors disabled:opacity-40 ${variant?.id === v.id ? 'border-moss bg-[#e8efe7]' : 'border-line bg-white hover:border-moss'}`}
                  >
                    {v.sku || `Variant #${v.id}`}
                    <span className="ml-3 font-semibold">{currency(v.price)}</span>
                  </button>
                ))}
              </div>
            ) : (
              <p className="text-sm text-stone-500">
                Variant selection is unavailable because product details don't include variant IDs.
              </p>
            )}
            {variant && (
              <p className="mt-3 text-xs text-stone-500">
                {stock.isFetching
                  ? 'Checking availability…'
                  : stock.data?.isInStock
                    ? `${stock.data.availableQuantity} available`
                    : 'Currently unavailable'}
              </p>
            )}
          </div>
          <div className="mt-6 flex gap-3">
            <div className="flex h-12 items-center rounded-full border border-line bg-white">
              <button
                type="button"
                aria-label="Decrease quantity"
                className="p-4"
                disabled={qty <= 1}
                onClick={() => setQty(Math.max(1, qty - 1))}
              >
                <Minus size={15} />
              </button>
              <span className="w-6 text-center text-sm">{qty}</span>
              <button
                type="button"
                aria-label="Increase quantity"
                className="p-4"
                disabled={qty >= Math.min(99, Number(stock.data?.availableQuantity ?? 1))}
                onClick={() => setQty(Math.min(99, qty + 1))}
              >
                <Plus size={15} />
              </button>
            </div>
            <Button className="h-12 flex-1" disabled={!canAdd || isAdding} onClick={addToCart}>
              <ShoppingBag size={18} />
              {isAdding ? 'Adding…' : 'Add to cart'}
            </Button>
          </div>
          {addError && (
            <div className="mt-4">
              <FormError error={addError} />
              <p className="mt-2 text-xs text-stone-500">
                If you are not signed in,{' '}
                <Link to="/login" className="underline">
                  log in
                </Link>{' '}
                first.
              </p>
            </div>
          )}
          {isSuccess && (
            <p
              role="status"
              className="mt-4 flex items-center gap-2 text-sm font-semibold text-moss"
            >
              <CheckCircle2 size={17} /> Added to your cart.{' '}
              <Link to="/cart" className="underline">
                View cart
              </Link>
            </p>
          )}
          <div className="mt-8 grid gap-4 border-t border-line pt-7 text-xs text-stone-600 sm:grid-cols-2">
            <span className="flex items-center gap-2">
              <ShieldCheck size={18} className="text-moss" /> Availability confirmed by API
            </span>
            <span className="flex items-center gap-2">
              <Truck size={18} className="text-moss" /> Shipping calculated at checkout (not yet
              available)
            </span>
          </div>
        </section>
      </div>
      {product.specs.length > 0 && (
        <section className="mt-16 max-w-2xl">
          <h2 className="font-display mb-5 text-3xl">Technical specifications</h2>
          <dl>
            {product.specs.map((s, i) => (
              <div
                key={`${s.name}-${i}`}
                className="flex justify-between gap-4 border-b border-line py-4 text-sm"
              >
                <dt className="font-medium">{s.name}</dt>
                <dd className="text-stone-600">{s.value}</dd>
              </div>
            ))}
          </dl>
        </section>
      )}
    </div>
  );
}
