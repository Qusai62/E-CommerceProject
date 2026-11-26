import { createFileRoute } from '@tanstack/react-router';
import { ProductDetailPage as ProductDetailView } from '@/features/catalog/views/product-detail-view';

export const Route = createFileRoute('/shop/$productId')({ component: ProductDetailView });
