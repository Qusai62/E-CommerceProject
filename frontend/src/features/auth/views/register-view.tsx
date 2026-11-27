import { Link } from '@tanstack/react-router';
import { FormError } from '@/components/ui/feedback';
import { AuthShell } from '../components/auth-shell';
import { RegisterForm } from '../components/register-form';
import { useRegister } from '../hooks/use-auth-commands';
export function RegisterPage() {
  const { submit, isLoading, error } = useRegister();
  return (
    <AuthShell
      eyebrow="CREATE ACCOUNT"
      title="Join the NEXORA community"
      description="Create an account to manage electronics, addresses and your shopping cart."
    >
      <RegisterForm onSubmit={submit} pending={isLoading} error={<FormError error={error} />} />
      <p className="mt-7 text-center text-sm text-slate-500">
        Already registered?{' '}
        <Link to="/login" className="font-semibold text-moss hover:underline">
          Sign in
        </Link>
      </p>
    </AuthShell>
  );
}
