import { createFileRoute } from '@tanstack/react-router';
import { CartPage as CartView } from '@/features/cart/views/cart-view';

export const Route = createFileRoute('/cart')({ component: CartView });
