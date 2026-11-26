import { createFileRoute } from '@tanstack/react-router';
import { OwnerInventoryPage } from '@/features/owner/views/inventory-view';
export const Route = createFileRoute('/owner/inventory')({ component: OwnerInventoryPage });
