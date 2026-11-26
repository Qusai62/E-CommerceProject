import type { ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { addressSchema, emptyAddress, type AddressFormValues } from '../model/address-schema';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Field } from '@/components/ui/feedback';

interface AddressFormProps {
  initial?:AddressFormValues;
  saving:boolean;
  onSave:(value:AddressFormValues)=>Promise<void>;
  error?:ReactNode;
}
export function AddressForm({initial=emptyAddress,saving,onSave,error}:AddressFormProps){
  const {register,handleSubmit,formState:{errors}}=useForm<AddressFormValues>({resolver:zodResolver(addressSchema),defaultValues:initial});
  return <form noValidate onSubmit={(event)=>void handleSubmit(onSave)(event)} className="grid gap-5 sm:grid-cols-2">
    <Field label="Recipient name" error={errors.recipientName?.message}><Input autoComplete="name" {...register('recipientName')}/></Field>
    <Field label="Contact number" error={errors.contactNumber?.message}><Input type="tel" autoComplete="tel" {...register('contactNumber')}/></Field>
    <Field label="City" error={errors.city?.message}><Input autoComplete="address-level2" {...register('city')}/></Field>
    <Field label="Neighborhood" error={errors.neighborhood?.message}><Input {...register('neighborhood')}/></Field>
    <Field label="Street" error={errors.street?.message}><Input autoComplete="street-address" {...register('street')}/></Field>
    <Field label="Building details" error={errors.buildingInfo?.message}><Input {...register('buildingInfo')}/></Field>
    <div className="sm:col-span-2"><Field label="Additional directions" error={errors.additionalAddressInfo?.message}><Textarea rows={3} {...register('additionalAddressInfo')}/></Field></div>
    <div className="sm:col-span-2">{error}<Button type="submit" disabled={saving}>{saving?'Saving address…':'Save address'}</Button></div>
  </form>;
}
