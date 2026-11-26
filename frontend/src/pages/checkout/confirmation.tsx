import { createFileRoute } from '@tanstack/react-router';
import { parseConfirmationSearch } from '@/features/checkout/model/route-search';
import { OrderConfirmationRouteView } from '@/features/checkout/views/order-confirmation-route-view';

export const Route = createFileRoute('/checkout/confirmation')({
  validateSearch: parseConfirmationSearch,
  component: OrderConfirmationRouteView,
});
