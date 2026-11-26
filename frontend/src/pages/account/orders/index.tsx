import { createFileRoute } from '@tanstack/react-router';
import { CustomerOrdersPage } from '@/features/account/views/orders-view';
export const Route = createFileRoute('/account/orders/')({ component: CustomerOrdersPage });
