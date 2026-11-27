import { createFileRoute } from '@tanstack/react-router';
import { CustomerOrderDetailsPage } from '@/features/account/views/order-details-view';
export const Route = createFileRoute('/account/orders/$orderId')({
  component: CustomerOrderDetailsPage,
});
