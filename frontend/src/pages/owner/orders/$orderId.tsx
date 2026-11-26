import { createFileRoute } from '@tanstack/react-router';
import { OwnerOrderDetailsPage } from '@/features/owner/views/order-details-view';

export const Route = createFileRoute('/owner/orders/$orderId')({
  component: OwnerOrderDetailsPage,
});
