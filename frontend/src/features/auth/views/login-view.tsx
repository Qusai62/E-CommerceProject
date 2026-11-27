import { Link } from '@tanstack/react-router';
import { FormError } from '@/components/ui/feedback';
import { AuthShell } from '../components/auth-shell';
import { LoginForm } from '../components/login-form';
import { useSignIn } from '../hooks/use-auth-commands';
export function LoginPage() {
  const { submit, loginError, isLoading } = useSignIn();
  return (
    <AuthShell
      eyebrow="WELCOME BACK"
      title="Sign in to Nexora"
      description="Access your cart, saved delivery addresses and account preferences."
    >
      <LoginForm onSubmit={submit} pending={isLoading} error={<FormError error={loginError} />} />
      <p className="mt-7 text-center text-sm text-slate-500">
        New to NEXORA?{' '}
        <Link to="/register" className="font-semibold text-moss hover:underline">
          Create an account
        </Link>
      </p>
      {import.meta.env.VITE_DEMO_MODE === 'true' && (
        <p className="mt-4 rounded-xl bg-blue-50 p-3 text-center text-xs text-blue-700">
          Demo roles: use owner@aster.test for StoreOwner, or customer@aster.test for Customer. Any
          nonempty password.
        </p>
      )}
    </AuthShell>
  );
}
