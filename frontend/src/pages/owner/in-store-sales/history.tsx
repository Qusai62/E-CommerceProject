import { createFileRoute } from '@tanstack/react-router';
import { InStoreSalesHistoryPage } from '@/features/owner/views/in-store-sales-history-view';

export const Route = createFileRoute('/owner/in-store-sales/history')({
  component: InStoreSalesHistoryPage,
});
