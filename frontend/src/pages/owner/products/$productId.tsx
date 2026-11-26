import { createFileRoute } from '@tanstack/react-router';
import { ProductManagePage as ProductWorkspaceView } from '@/features/owner/views/product-manage-view';

export const Route = createFileRoute('/owner/products/$productId')({ component: ProductWorkspaceView });
