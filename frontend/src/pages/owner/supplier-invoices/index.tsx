import { createFileRoute } from '@tanstack/react-router';
import { OwnerSupplierInvoicesPage } from '@/features/owner/views/supplier-invoices-list-view';

export const Route = createFileRoute('/owner/supplier-invoices/')({
  component: OwnerSupplierInvoicesPage,
});
