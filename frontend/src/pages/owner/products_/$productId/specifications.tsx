import { createFileRoute } from '@tanstack/react-router';
import { ProductSpecificationsView } from '@/features/owner/views/product-specifications-view';
export const Route = createFileRoute('/owner/products_/$productId/specifications')({
  component: Page,
});
function Page() {
  const { productId } = Route.useParams();
  return <ProductSpecificationsView productId={Number(productId)} />;
}
