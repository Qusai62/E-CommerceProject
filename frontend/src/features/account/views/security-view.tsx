import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { ShieldCheck } from 'lucide-react';
import { useGetCustomerProfileQuery, useChangeCustomerPasswordMutation } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Field, FormError, LoadingState } from '@/components/ui/feedback';
import { PageHeader } from '@/components/layout/site-layout';
import { AccountNav, SignInRequired } from '../components/account-nav';

const passwordSchema=z.object({currentPassword:z.string().min(1,'Current password required'),newPassword:z.string().min(8,'Use 8+ characters').regex(/[A-Z]/).regex(/[a-z]/).regex(/\d/).regex(/[^a-zA-Z0-9]/),confirmPassword:z.string()}).refine(v=>v.newPassword===v.confirmPassword,{path:['confirmPassword'],message:'Passwords do not match'});
export function SecurityPage(){
  const query=useGetCustomerProfileQuery();const [change,{error,isLoading,isSuccess}]=useChangeCustomerPasswordMutation();
  const {register,handleSubmit,reset,formState:{errors,isSubmitting}}=useForm<z.infer<typeof passwordSchema>>({resolver:zodResolver(passwordSchema)});
  if(query.isLoading)return <LoadingState/>;if(query.isError&&'status' in query.error&&query.error.status===401)return <SignInRequired/>;
  return <div className="page-container py-14"><PageHeader eyebrow="YOUR ACCOUNT" title="Security settings." description="Update your credentials whenever you need to."/>
    <div className="mt-10 grid gap-8 lg:grid-cols-[260px_1fr]"><AccountNav/><Card className="max-w-2xl p-8"><div className="mb-6 flex items-center gap-3"><ShieldCheck className="text-moss"/><h2 className="font-display text-2xl">Change password</h2></div><form className="space-y-5" noValidate onSubmit={e=>void handleSubmit(async v=>{try{await change({body:{currentPassword:v.currentPassword,newPassword:v.newPassword}}).unwrap();reset();}catch{/* Server error displayed in mutation state */}})(e)}><Field label="Current password" error={errors.currentPassword?.message}><Input type="password" autoComplete="current-password" {...register('currentPassword')}/></Field><Field label="New password" error={errors.newPassword?.message} hint="8+ characters, uppercase, lowercase, number and symbol."><Input type="password" autoComplete="new-password" {...register('newPassword')}/></Field><Field label="Confirm new password" error={errors.confirmPassword?.message}><Input type="password" autoComplete="new-password" {...register('confirmPassword')}/></Field><FormError error={error}/><p className="text-xs leading-5 text-amber-700">The latest Swagger declares an X-CSRF-TOKEN header for password changes but does not provide a token-issuance endpoint. Confirm your backend antiforgery contract before using this operation in production.</p>{isSuccess&&<p role="status" className="text-sm text-moss">Password updated successfully.</p>}<Button type="submit" disabled={isLoading||isSubmitting}>{isLoading||isSubmitting?'Updating…':'Update password'}</Button></form></Card></div>
  </div>;
}
