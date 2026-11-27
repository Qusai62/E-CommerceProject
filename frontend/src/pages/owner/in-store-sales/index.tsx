import { createFileRoute } from '@tanstack/react-router';
import { InStoreSaleCreatePage } from '@/features/owner/views/in-store-sale-create-view';

export const Route = createFileRoute('/owner/in-store-sales/')({
  component: InStoreSaleCreatePage,
});
