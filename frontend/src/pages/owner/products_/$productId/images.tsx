import { createFileRoute } from '@tanstack/react-router';
import { ProductImagesView } from '@/features/owner/views/product-images-view';
export const Route = createFileRoute('/owner/products_/$productId/images')({ component: Page });
function Page() {
  const { productId } = Route.useParams();
  return <ProductImagesView productId={Number(productId)} />;
}
