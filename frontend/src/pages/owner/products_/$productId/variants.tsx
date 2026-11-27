import { createFileRoute } from '@tanstack/react-router';
import { ProductVariantsView } from '@/features/owner/views/product-variants-view';
export const Route = createFileRoute('/owner/products_/$productId/variants')({ component: Page });
function Page() {
  const { productId } = Route.useParams();
  return <ProductVariantsView productId={Number(productId)} />;
}
