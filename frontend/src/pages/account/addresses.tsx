import { createFileRoute } from '@tanstack/react-router';
import { AddressesPage as AddressesListView } from '@/features/account/views/addresses-view';

export const Route = createFileRoute('/account/addresses')({ component: AddressesListView });
