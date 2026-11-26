import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useGetCustomerProfileQuery, useUpdateCustomerProfileMutation } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { ErrorState, Field, FormError, LoadingState } from '@/components/ui/feedback';
import { PageHeader } from '@/components/layout/site-layout';
import { date } from '@/shared/lib/format';
import { useSignOut } from '@/features/auth/hooks/use-sign-out';
import { AccountNav, SignInRequired } from '../components/account-nav';

const profileSchema=z.object({firstName:z.string().min(1,'Required'),lastName:z.string().min(1,'Required'),email:z.email(),phoneNumber:z.string(),currentPassword:z.string().optional()});
export function AccountPage(){
  const signOut = useSignOut();
  const query=useGetCustomerProfileQuery();const [update,{isLoading,isSuccess,error}]=useUpdateCustomerProfileMutation();
  const {register,handleSubmit,reset,formState:{errors}}=useForm<z.infer<typeof profileSchema>>({resolver:zodResolver(profileSchema)});
  useEffect(()=>{if(query.data)reset({firstName:query.data.firstName||'',lastName:query.data.lastName||'',email:query.data.email||'',phoneNumber:query.data.phoneNumber||'',currentPassword:''})},[query.data,reset]);
  if(query.isLoading)return <LoadingState/>;
  if(query.isError&&'status' in query.error&&query.error.status===401)return <SignInRequired/>;
  if(query.isError)return <div className="page-container py-16"><ErrorState error={query.error}/></div>;
  return <div className="page-container py-14"><PageHeader eyebrow="YOUR ACCOUNT" title={`Hello, ${query.data?.firstName||'there'}.`} description={`Member since ${date(query.data?.createdAtUtc)}. Your details, made easy.`}/>
    <div className="mt-10 grid gap-8 lg:grid-cols-[260px_1fr]"><AccountNav/><Card className="p-6 sm:p-9"><h2 className="font-display text-3xl">Personal information</h2><p className="mt-2 text-sm text-stone-500">Keep your account details up to date.</p><form className="mt-8 grid gap-5 sm:grid-cols-2" onSubmit={e=>void handleSubmit(async v=>{try{await update({body:v}).unwrap();reset({...v,currentPassword:''})}catch{/* shown inline */}})(e)} noValidate>
      <Field label="First name" error={errors.firstName?.message}><Input autoComplete="given-name" {...register('firstName')}/></Field><Field label="Last name" error={errors.lastName?.message}><Input autoComplete="family-name" {...register('lastName')}/></Field><Field label="Email" error={errors.email?.message}><Input type="email" autoComplete="email" {...register('email')}/></Field><Field label="Phone number" error={errors.phoneNumber?.message}><Input type="tel" autoComplete="tel" {...register('phoneNumber')}/></Field><div className="sm:col-span-2"><Field label="Current password (may be required to change email)"><Input type="password" autoComplete="current-password" {...register('currentPassword')}/></Field></div><div className="sm:col-span-2"><FormError error={error}/>{isSuccess&&<p role="status" className="mb-3 text-sm text-moss">Profile saved.</p>}<Button disabled={isLoading} type="submit">{isLoading?'Saving…':'Save changes'}</Button></div>
      </form><div className="mt-9 border-t border-line pt-6"><Button disabled={signOut.pending} variant="outline" onClick={() => void signOut.signOut()}>Sign out</Button><FormError error={signOut.error}/></div></Card></div></div>;
}
