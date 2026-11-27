import { ProductResourceLayout } from '../containers/product-resource-layout';
import { ProductInfoEditor } from '../components/product-info-editor';
export function ProductEditView({ productId }: { productId: number }) {
  return (
    <ProductResourceLayout productId={productId} title="Edit product details">
      {({ product, raw }) => (
        <ProductInfoEditor
          key={`${product.id}-${String(raw.updatedAtUtc ?? '')}`}
          product={{
            productId,
            name: product.name,
            description: product.description,
            brandId: typeof raw.brandId === 'number' ? raw.brandId : null,
            categoryId: typeof raw.categoryId === 'number' ? raw.categoryId : null,
          }}
        />
      )}
    </ProductResourceLayout>
  );
}
