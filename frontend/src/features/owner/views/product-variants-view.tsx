import { ProductResourceLayout } from '../containers/product-resource-layout';
import { VariantManager } from '../components/variant-manager';
export function ProductVariantsView({ productId }: { productId: number }) {
  return (
    <ProductResourceLayout productId={productId} title="Variants & pricing">
      {({ product }) => <VariantManager productId={productId} variants={product.variants} />}
    </ProductResourceLayout>
  );
}
