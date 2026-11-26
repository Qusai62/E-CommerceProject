import { createFileRoute } from '@tanstack/react-router';
import { OwnerOrdersPage } from '@/features/owner/views/orders-view';

export const Route = createFileRoute('/owner/orders/')({ component: OwnerOrdersPage });
