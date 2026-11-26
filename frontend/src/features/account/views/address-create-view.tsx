import { getRouteApi, useNavigate } from '@tanstack/react-router';
// The route's public URL is `/account/addresses/new`, while its TanStack
// route ID includes the trailing-underscore layout segment.
const addressCreateRoute = getRouteApi('/account/addresses_/new');
import { useAddAddressMutation } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Card } from '@/components/ui/card';
import { FormError } from '@/components/ui/feedback';
import { AddressForm } from '../components/address-form';
import type { AddressFormValues } from '../model/address-schema';
export function AddressCreateView(){
  const navigate=useNavigate();
  const { returnTo } = addressCreateRoute.useSearch();
  const [create,status]=useAddAddressMutation();
  async function submit(values:AddressFormValues){try{await create({body:values}).unwrap();await navigate({to:returnTo ?? '/account/addresses'});}catch{/* Rendered in FormError */}}
  return <div className="page-container max-w-4xl py-12"><PageHeader eyebrow="ACCOUNT / ADDRESSES" title="New address" description="Add the information your store needs for future deliveries."/><Card className="mt-7 p-6 sm:p-9"><AddressForm onSave={submit} saving={status.isLoading} error={<FormError error={status.error}/>}/></Card></div>;
}
