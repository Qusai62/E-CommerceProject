import { createFileRoute } from '@tanstack/react-router';
import { validReturnPath } from '@/features/checkout/model/route-search';
import { LoginPage as LoginView } from '@/features/auth/views/login-view';
import type { SearchSchemaInput } from '@tanstack/react-router';
export const Route = createFileRoute('/login')({
  component: LoginView,
  validateSearch: (search: { redirect?: string } & SearchSchemaInput) => ({
    redirect: validReturnPath(search.redirect),
  }),
});
