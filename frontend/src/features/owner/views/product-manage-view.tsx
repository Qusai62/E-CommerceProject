import { Link } from '@tanstack/react-router';
import { getRouteApi } from '@tanstack/react-router';
import { ProductResourceLayout } from '../containers/product-resource-layout';
import { ProductVisual } from '@/features/catalog/components/product-card';
import { currency } from '@/shared/lib/format';
import { Button } from '@/components/ui/button';
const route = getRouteApi('/owner/products/$productId');
export function ProductManagePage() {
  const { productId } = route.useParams();
  const id = Number(productId);
  return (
    <ProductResourceLayout productId={id} title="Product workspace">
      {({ product }) => (
        <div className="grid items-center gap-8 sm:grid-cols-[220px_1fr]">
          <ProductVisual name={product.name} imageKey={product.imageKey} />
          <div>
            <h2 className="text-2xl font-bold">{product.name}</h2>
            <p className="mt-3 text-sm text-slate-500">
              {product.description || 'No description available.'}
            </p>
            <p className="mt-4 text-xl font-semibold">{currency(product.price)}</p>
            <p className="mt-2 text-xs text-slate-500">
              {product.variants.length} variants · {product.images.length} images ·{' '}
              {product.specs.length} specifications
            </p>
            <Button asChild className="mt-6">
              <Link to="/owner/products/$productId/edit" params={{ productId: String(id) }}>
                Edit product details
              </Link>
            </Button>
          </div>
        </div>
      )}
    </ProductResourceLayout>
  );
}
