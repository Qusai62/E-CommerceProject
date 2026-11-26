import { createFileRoute } from '@tanstack/react-router';
import { InStoreSaleDetailsPage } from '@/features/owner/views/in-store-sale-details-view';

export const Route = createFileRoute('/owner/in-store-sales/$saleId')({ component: InStoreSaleDetailsPage });
