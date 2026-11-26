import { useDispatch } from 'react-redux';
import { useNavigate } from '@tanstack/react-router';
import { baseApi } from '@/shared/api/baseApi';
import { clearCsrfRequestToken } from '@/shared/api/csrf';
import { useLogoutMutation } from '@/shared/api/generated/api.generated';

export function useSignOut() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [logout, status] = useLogoutMutation();
  const signOut = async () => {
    try {
      await logout().unwrap();
      clearCsrfRequestToken();
      dispatch(baseApi.util.resetApiState());
      await navigate({ to: '/login' });
    } catch {
      // Do not pretend the server session was revoked if logout failed.
    }
  };
  return { signOut, pending: status.isLoading, error: status.error };
}
