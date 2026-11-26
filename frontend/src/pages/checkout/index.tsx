import { createFileRoute } from '@tanstack/react-router';
import { CheckoutView } from '@/features/checkout/views/checkout-view';

export const Route = createFileRoute('/checkout/')({ component: CheckoutView });
