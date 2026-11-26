import { createFileRoute } from '@tanstack/react-router';
import { ProductCreateView } from '@/features/owner/views/product-create-view';
export const Route = createFileRoute('/owner/products/new')({ component: ProductCreateView });
