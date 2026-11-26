import { createFileRoute } from '@tanstack/react-router';
import { validReturnPath } from '@/features/checkout/model/route-search';
import { LoginPage as LoginView } from '@/features/auth/views/login-view';

export const Route = createFileRoute('/login')({
  validateSearch: (search: Record<string, unknown>) => ({ redirect: validReturnPath(search.redirect) }),
  component: LoginView,
});
