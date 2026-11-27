import { useNavigate } from '@tanstack/react-router';
import {
  useGetAddressesQuery,
  useUpdateAddressMutation,
} from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Card } from '@/components/ui/card';
import { ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { AddressForm } from '../components/address-form';
import { emptyAddress, type AddressFormValues } from '../model/address-schema';
export function AddressEditView({ addressId }: { addressId: number }) {
  const navigate = useNavigate();
  const valid = Number.isSafeInteger(addressId) && addressId > 0;
  const query = useGetAddressesQuery(undefined, { skip: !valid });
  const [update, status] = useUpdateAddressMutation();
  if (!valid) return <ErrorState error={new Error('Invalid address ID')} />;
  if (query.isLoading) return <LoadingState />;
  if (query.isError) return <ErrorState error={query.error} onRetry={() => void query.refetch()} />;
  // The contract only exposes GET /api/customer/addresses, so locate the record in the list.
  const address = query.data?.find((item) => item.savedAddressId === addressId);
  if (!address) return <ErrorState error={new Error('Address not found')} />;
  const initial = Object.fromEntries(
    Object.keys(emptyAddress).map((key) => [key, address[key as keyof AddressFormValues] ?? '']),
  ) as AddressFormValues;
  async function submit(values: AddressFormValues) {
    try {
      await update({ addressId, body: values }).unwrap();
      await navigate({ to: '/account/addresses' });
    } catch {
      /* Rendered in FormError */
    }
  }
  return (
    <div className="page-container max-w-4xl py-12">
      <PageHeader
        eyebrow="ACCOUNT / ADDRESSES"
        title="Edit address"
        description="Keep your saved delivery details accurate."
      />
      <Card className="mt-7 p-6 sm:p-9">
        <AddressForm
          key={addressId}
          initial={initial}
          onSave={submit}
          saving={status.isLoading}
          error={<FormError error={status.error} />}
        />
      </Card>
    </div>
  );
}
