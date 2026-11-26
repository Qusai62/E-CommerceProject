import { useNavigate, getRouteApi } from '@tanstack/react-router';
import { useDispatch } from 'react-redux';
import { useLoginMutation, useRegisterCustomerMutation } from '@/shared/api/generated/api.generated';
import { baseApi } from '@/shared/api/baseApi';
import { hasRole, preferredDestination } from '@/features/auth/model/roles';
import { registerRequest, type LoginFormValues, type RegistrationFormValues } from '../model/auth-schemas';

const loginRoute = getRouteApi('/login');

/** The server resolves identity and roles. Never store auth cookies or claims in localStorage. */
export function useSignIn() {
  const navigate = useNavigate();
  const { redirect } = loginRoute.useSearch();
  const dispatch = useDispatch();
  const [login, status] = useLoginMutation();

  async function submit(values: LoginFormValues) {
    status.reset();
    try {
      const session = await login({ body: values }).unwrap();
      // Remove guest-scoped queries; /api/auth/me will reload with the new cookie.
      dispatch(baseApi.util.resetApiState());
      const destination = redirect === '/owner' && !hasRole(session.roles, 'StoreOwner')
        ? preferredDestination(session.roles) : redirect ?? preferredDestination(session.roles);
      await navigate({ to: destination });
    } catch {
      // The mutation exposes backend errors to the view.
    }
  }

  return { submit, loginError: status.error, isLoading: status.isLoading };
}

export function useRegister() {
  const navigate = useNavigate();
  const [register, status] = useRegisterCustomerMutation();
  async function submit(values: RegistrationFormValues) {
    try {
      await register({ body: registerRequest(values) }).unwrap();
      await navigate({ to: '/login' });
    } catch {
      // Server error displayed by view.
    }
  }
  return { submit, isLoading: status.isLoading, error: status.error };
}
