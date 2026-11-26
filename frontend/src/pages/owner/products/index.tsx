import { createFileRoute } from '@tanstack/react-router';
import { OwnerProductsPage } from '@/features/owner/views/products-list-view';
export const Route = createFileRoute('/owner/products/')({ component: OwnerProductsPage });
