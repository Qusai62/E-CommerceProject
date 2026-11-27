import type { ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ArrowRight } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Field } from '@/components/ui/feedback';
import { registrationSchema, type RegistrationFormValues } from '../model/auth-schemas';
export function RegisterForm({
  onSubmit,
  pending,
  error,
}: {
  onSubmit: (values: RegistrationFormValues) => Promise<void>;
  pending: boolean;
  error?: ReactNode;
}) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegistrationFormValues>({ resolver: zodResolver(registrationSchema) });
  return (
    <form
      noValidate
      onSubmit={(event) => void handleSubmit(onSubmit)(event)}
      className="mt-8 space-y-5"
    >
      <div className="grid grid-cols-2 gap-3">
        <Field label="First name" error={errors.firstName?.message}>
          <Input autoComplete="given-name" {...register('firstName')} />
        </Field>
        <Field label="Last name" error={errors.lastName?.message}>
          <Input autoComplete="family-name" {...register('lastName')} />
        </Field>
      </div>
      <Field label="Email address" error={errors.email?.message}>
        <Input autoComplete="email" type="email" {...register('email')} />
      </Field>
      <Field label="Phone number" error={errors.phoneNumber?.message}>
        <Input autoComplete="tel" type="tel" {...register('phoneNumber')} />
      </Field>
      <Field
        label="Password"
        error={errors.password?.message}
        hint="Use at least 8 characters, uppercase, lowercase, a number and a symbol."
      >
        <Input type="password" autoComplete="new-password" {...register('password')} />
      </Field>
      <Field label="Confirm password" error={errors.confirmPassword?.message}>
        <Input type="password" autoComplete="new-password" {...register('confirmPassword')} />
      </Field>
      {error}
      <Button type="submit" className="w-full" disabled={pending || isSubmitting}>
        {pending || isSubmitting ? 'Creating account…' : 'Create account'}
        <ArrowRight size={16} />
      </Button>
    </form>
  );
}
