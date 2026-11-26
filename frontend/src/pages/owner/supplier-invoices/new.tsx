import { createFileRoute } from '@tanstack/react-router';
import { OwnerSupplierInvoiceCreatePage } from '@/features/owner/views/supplier-invoice-create-view';

export const Route = createFileRoute('/owner/supplier-invoices/new')({
  component: OwnerSupplierInvoiceCreatePage,
});
