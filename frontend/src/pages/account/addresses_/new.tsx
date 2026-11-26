import { createFileRoute } from '@tanstack/react-router';
import { validReturnPath } from '@/features/checkout/model/route-search';
import { AddressCreateView } from '@/features/account/views/address-create-view';
import {withAuthentication} from '@/features/auth/components/with-authentication';
export const Route=createFileRoute('/account/addresses_/new')({
  validateSearch: (search: Record<string, unknown>) => ({ returnTo: validReturnPath(search.returnTo) }),
  component: withAuthentication(AddressCreateView),
});
