import { getRouteApi } from '@tanstack/react-router';
import { OrderConfirmationView } from './order-confirmation-view';

const route = getRouteApi('/checkout/confirmation');
export function OrderConfirmationRouteView() {
  const { orderNumber, orderId } = route.useSearch();
  return <OrderConfirmationView orderNumber={orderNumber} orderId={orderId} />;
}
