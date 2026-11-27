import { useState, type ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ArrowRight, Eye, EyeOff } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Field } from '@/components/ui/feedback';
import { loginSchema, type LoginFormValues } from '../model/auth-schemas';
export function LoginForm({
  onSubmit,
  pending,
  error,
}: {
  onSubmit: (values: LoginFormValues) => Promise<void>;
  pending: boolean;
  error?: ReactNode;
}) {
  const [showPassword, setShowPassword] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });
  return (
    <form
      className="mt-8 space-y-5"
      noValidate
      onSubmit={(event) => void handleSubmit(onSubmit)(event)}
    >
      <Field label="Email address" error={errors.email?.message}>
        <Input
          type="email"
          autoComplete="email"
          placeholder="name@example.com"
          {...register('email')}
        />
      </Field>
      <Field label="Password" error={errors.password?.message}>
        <div className="relative">
          <Input
            type={showPassword ? 'text' : 'password'}
            autoComplete="current-password"
            className="pr-12"
            {...register('password')}
          />
          <button
            type="button"
            aria-label={showPassword ? 'Hide password' : 'Show password'}
            onClick={() => setShowPassword((show) => !show)}
            className="absolute right-3 top-3 text-slate-500"
          >
            {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
          </button>
        </div>
      </Field>
      {error}
      <Button type="submit" className="w-full" disabled={pending || isSubmitting}>
        {pending || isSubmitting ? 'Signing in…' : 'Sign in'}
        <ArrowRight size={16} />
      </Button>
    </form>
  );
}
