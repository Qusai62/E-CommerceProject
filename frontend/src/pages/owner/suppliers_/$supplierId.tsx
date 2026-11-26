import { createFileRoute } from '@tanstack/react-router';
import { OwnerSupplierDetailsPage } from '@/features/owner/views/supplier-details-view';

export const Route = createFileRoute('/owner/suppliers_/$supplierId')({
  component: OwnerSupplierDetailsPage,
});
