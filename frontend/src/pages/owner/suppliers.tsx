import { createFileRoute } from '@tanstack/react-router';
import { OwnerSuppliersPage } from '@/features/owner/views/suppliers-view';

export const Route = createFileRoute('/owner/suppliers')({ component: OwnerSuppliersPage });
