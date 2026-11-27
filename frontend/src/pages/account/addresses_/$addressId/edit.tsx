import { createFileRoute } from '@tanstack/react-router';
import { AddressEditView } from '@/features/account/views/address-edit-view';
import { withAuthentication } from '@/features/auth/components/with-authentication';
export const Route = createFileRoute('/account/addresses_/$addressId/edit')({
  component: withAuthentication(Page),
});
function Page() {
  const { addressId } = Route.useParams();
  return <AddressEditView addressId={Number(addressId)} />;
}
