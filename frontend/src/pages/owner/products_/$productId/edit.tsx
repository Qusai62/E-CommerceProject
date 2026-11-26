import { createFileRoute } from '@tanstack/react-router';
import { ProductEditView } from '@/features/owner/views/product-edit-view';
export const Route = createFileRoute('/owner/products_/$productId/edit')({component: Page});
function Page() { const { productId } = Route.useParams(); return <ProductEditView productId={Number(productId)}/>; }
