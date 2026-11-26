import { createFileRoute } from '@tanstack/react-router';
import { OwnerSupplierInvoiceDetailsPage } from '@/features/owner/views/supplier-invoice-details-view';
export const Route = createFileRoute('/owner/supplier-invoices_/$invoiceId')({
  component: OwnerSupplierInvoiceDetailsPage,
});
