import { Link } from '@tanstack/react-router';
import { ArrowRight, LockKeyhole, MapPin, Package, UserRound } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { EmptyState } from '@/components/ui/feedback';

export function AccountNav() {
  return (
    <nav
      aria-label="Account sections"
      className="h-fit space-y-2 rounded-2xl border border-line bg-white p-3"
    >
      <Link
        to="/account"
        activeOptions={{ exact: true }}
        activeProps={{ className: 'bg-muted font-semibold' }}
        className="flex items-center gap-3 rounded-xl px-4 py-3 text-sm hover:bg-muted"
      >
        <UserRound size={17} /> Profile
      </Link>
      <Link
        to="/account/addresses"
        activeProps={{ className: 'bg-muted font-semibold' }}
        className="flex items-center gap-3 rounded-xl px-4 py-3 text-sm hover:bg-muted"
      >
        <MapPin size={17} /> Saved addresses
      </Link>
      <Link
        to="/account/orders"
        activeProps={{ className: 'bg-muted font-semibold' }}
        className="flex items-center gap-3 rounded-xl px-4 py-3 text-sm hover:bg-muted"
      >
        <Package size={17} /> My orders
      </Link>
      <Link
        to="/account/security"
        activeProps={{ className: 'bg-muted font-semibold' }}
        className="flex items-center gap-3 rounded-xl px-4 py-3 text-sm hover:bg-muted"
      >
        <LockKeyhole size={17} /> Security
      </Link>
    </nav>
  );
}
export function SignInRequired() {
  return (
    <div className="page-container py-20">
      <EmptyState
        icon={<LockKeyhole size={30} />}
        title="Your space starts here"
        description="Sign in to see your account and manage your personal details."
        action={
          <Button asChild>
            <Link to="/login">
              Sign in <ArrowRight size={16} />
            </Link>
          </Button>
        }
      />
    </div>
  );
}
