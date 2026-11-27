import type { ComponentType } from 'react';
import { Link } from '@tanstack/react-router';
import { LockKeyhole } from 'lucide-react';
import { useGetCustomerProfileQuery } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';

/** Apply only to routes requiring an authenticated customer. This is a UX guard, not an authorization boundary. */
export function withAuthentication<Props extends object>(Screen: ComponentType<Props>) {
  function AuthenticatedScreen(props: Props) {
    const profile = useGetCustomerProfileQuery();
    if (profile.isLoading) return <LoadingState message="Checking your session…" />;
    if (profile.isError && 'status' in profile.error && profile.error.status === 401) {
      return (
        <div className="page-container py-16">
          <EmptyState
            icon={<LockKeyhole size={28} />}
            title="Sign in required"
            description="Sign in to manage your saved account details."
            action={
              <Button asChild>
                <Link to="/login">Sign in</Link>
              </Button>
            }
          />
        </div>
      );
    }
    if (profile.isError) {
      return <ErrorState error={profile.error} onRetry={() => void profile.refetch()} />;
    }
    return <Screen {...props} />;
  }
  AuthenticatedScreen.displayName = `withAuthentication(${Screen.displayName ?? Screen.name ?? 'Screen'})`;
  return AuthenticatedScreen;
}
