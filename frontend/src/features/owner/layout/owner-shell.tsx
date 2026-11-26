import { useState, type ReactNode } from 'react';
import { Link, useRouterState } from '@tanstack/react-router';
import {
  BarChart3,
  Boxes,
  CircleDollarSign,
  Building2,
  Cpu,
  FolderTree,
  FileText,
  LayoutDashboard,
  LogOut,
  Menu,
  PackageOpen,
  ReceiptText,
  ShieldAlert,
  ShoppingBag,
  Tag,
  Warehouse,
  X,
} from 'lucide-react';
import { useGetCurrentUserQuery } from '@/shared/api/generated/api.generated';
import { Button } from '@/components/ui/button';
import { EmptyState, ErrorState, FormError, LoadingState } from '@/components/ui/feedback';
import { useSignOut } from '@/features/auth/hooks/use-sign-out';
import { hasRole } from '@/features/auth/model/roles';
import { cn } from '@/shared/lib/cn';

const ownerLinks = [
  { to: '/owner', label: 'Overview', icon: LayoutDashboard, exact: true },
  { to: '/owner/products', label: 'Products', icon: Boxes },
  { to: '/owner/categories', label: 'Categories', icon: FolderTree },
  { to: '/owner/brands', label: 'Brands', icon: Tag },
  { to: '/owner/inventory', label: 'Inventory', icon: Warehouse },
  { to: '/owner/suppliers', label: 'Suppliers', icon: Building2 },
  { to: '/owner/supplier-invoices', label: 'Supplier invoices', icon: FileText },
  { to: '/owner/orders', label: 'Orders', icon: ReceiptText },
  { to: '/owner/in-store-sales', label: 'In-store sales', icon: CircleDollarSign, exact: true },
  { to: '/owner/in-store-sales/history', label: 'Sale history', icon: ReceiptText },
  { to: '/owner/reports', label: 'Reports', icon: BarChart3 },
] as const;

function OwnerSidebar({ onNavigate }: { onNavigate?: () => void }) {
  const pathname = useRouterState({ select: (state) => state.location.pathname });
  return (
    <div className="flex h-full flex-col overflow-y-auto bg-[#111d35] px-4 py-6 text-white">
      <Link
        to="/owner"
        onClick={onNavigate}
        className="flex items-center gap-3 px-3"
        aria-label="Nexora Owner dashboard"
      >
        <span className="grid size-11 place-items-center rounded-2xl bg-blue-500">
          <Cpu size={22} />
        </span>
        <span>
          <strong className="block text-lg tracking-tight">Nexora</strong>
          <span className="text-[10px] font-semibold uppercase tracking-[.22em] text-slate-400">
            Merchant workspace
          </span>
        </span>
      </Link>
      <div className="mt-10 px-3 text-[11px] font-bold uppercase tracking-[.18em] text-slate-500">
        Management
      </div>
      <nav aria-label="Owner navigation" className="mt-3 space-y-1.5">
        {ownerLinks.map(({ to, label, icon: Icon, ...rest }) => {
          const active =
            'exact' in rest && rest.exact
              ? pathname === to || pathname === `${to}/`
              : pathname === to || pathname.startsWith(`${to}/`);
          return (
            <Link
              key={to}
              to={to}
              onClick={onNavigate}
              aria-current={active ? 'page' : undefined}
              className={cn(
                'flex items-center gap-3 rounded-xl px-4 py-3 text-sm font-medium transition-colors',
                active
                  ? 'bg-blue-600 text-white shadow-lg shadow-blue-950/30'
                  : 'text-slate-300 hover:bg-white/10 hover:text-white',
              )}
            >
              <Icon size={18} aria-hidden="true" /> {label}
            </Link>
          );
        })}
      </nav>
      <div className="mt-auto border-t border-white/10 pt-5">
        <Link
          onClick={onNavigate}
          to="/shop"
          className="flex items-center gap-3 rounded-xl px-4 py-3 text-sm text-slate-300 hover:bg-white/10 hover:text-white"
        >
          <ShoppingBag size={18} /> View storefront
        </Link>
        <div className="mx-3 mt-5 flex items-center gap-3 rounded-xl bg-white/5 p-3 text-xs leading-5 text-slate-300">
          <ShieldAlert size={25} className="shrink-0 text-blue-300" />
          <span>Actions are validated by your ASP.NET authorization policies.</span>
        </div>
      </div>
    </div>
  );
}

function OwnerWorkspace({ children, email }: { children: ReactNode; email?: string | null }) {
  const [menuOpen, setMenuOpen] = useState(false);
  const signOut = useSignOut();
  return (
    <div className="min-h-screen bg-[#f3f6fb] text-ink">
      <aside className="fixed inset-y-0 left-0 z-40 hidden w-[258px] lg:block">
        <OwnerSidebar />
      </aside>
      {menuOpen && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <button
            type="button"
            className="absolute inset-0 bg-slate-950/65"
            aria-label="Close navigation"
            onClick={() => setMenuOpen(false)}
          />
          <aside className="relative h-full w-[min(85vw,280px)] shadow-2xl">
            <OwnerSidebar onNavigate={() => setMenuOpen(false)} />
          </aside>
        </div>
      )}
      <div className="min-h-screen lg:pl-[258px]">
        <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/95 px-4 backdrop-blur-lg sm:px-7">
          <div className="mx-auto flex h-[76px] max-w-[1480px] items-center justify-between gap-3">
            <div className="flex items-center gap-3">
              <button
                type="button"
                onClick={() => setMenuOpen(!menuOpen)}
                aria-label="Open owner navigation"
                aria-expanded={menuOpen}
                className="rounded-xl border border-slate-200 p-2.5 lg:hidden"
              >
                {menuOpen ? <X size={20} /> : <Menu size={20} />}
              </button>
              <div>
                <p className="text-[10px] font-bold uppercase tracking-[.2em] text-blue-600">
                  StoreOwner
                </p>
                <p className="text-sm font-semibold text-slate-900 sm:text-base">
                  Store operations
                </p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <div className="hidden text-right sm:block">
                <p className="text-xs font-semibold">Merchant account</p>
                <p className="max-w-44 truncate text-xs text-slate-500">{email ?? 'Store owner'}</p>
              </div>
              <Button
                variant="outline"
                size="sm"
                disabled={signOut.pending}
                onClick={() => void signOut.signOut()}
              >
                <LogOut size={15} /> <span className="hidden sm:inline">Sign out</span>
              </Button>
            </div>
          </div>
          <FormError error={signOut.error} />
        </header>
        <main
          id="main-content"
          className="owner-main mx-auto max-w-[1480px] px-4 py-7 sm:px-7 sm:py-9 lg:px-9"
        >
          {children}
        </main>
        <footer className="border-t border-slate-200 px-6 py-5 text-center text-xs text-slate-500">
          Nexora Merchant Console · Operational reporting from documented API data
        </footer>
      </div>
    </div>
  );
}

/** Owner routes are UX gated from /api/auth/me. ASP.NET Core must enforce StoreOwner authorization. */
export function OwnerShell({ children }: { children: ReactNode }) {
  const session = useGetCurrentUserQuery();
  if (session.isLoading || session.isUninitialized)
    return (
      <div className="grid min-h-screen place-items-center bg-slate-100">
        <LoadingState message="Checking StoreOwner access…" />
      </div>
    );
  if (session.isError && 'status' in session.error && session.error.status === 401)
    return (
      <div className="grid min-h-screen place-items-center bg-slate-100 p-5">
        <div className="w-full max-w-lg rounded-3xl bg-white px-8 py-14 text-center shadow-xl">
          <ShieldAlert size={38} className="mx-auto text-blue-600" />
          <h1 className="mt-6 text-3xl font-bold">Merchant sign-in required</h1>
          <p className="mt-3 text-sm text-slate-500">
            Sign in with your StoreOwner account to access inventory and management tools.
          </p>
          <Button asChild className="mt-7">
            <Link to="/login" search={{ redirect: '/owner' }}>
              Sign in to console
            </Link>
          </Button>
        </div>
      </div>
    );
  if (session.isError)
    return (
      <div className="grid min-h-screen place-items-center p-8">
        <ErrorState error={session.error} onRetry={() => void session.refetch()} />
      </div>
    );
  if (!hasRole(session.data?.roles, 'StoreOwner'))
    return (
      <div className="grid min-h-screen place-items-center bg-slate-100 p-5">
        <EmptyState
          icon={<PackageOpen size={34} />}
          title="Owner access required"
          description="This account does not have the StoreOwner role. Ask an administrator to grant access if appropriate."
          action={
            <Button asChild>
              <Link to="/shop">Return to store</Link>
            </Button>
          }
        />
      </div>
    );
  return <OwnerWorkspace email={session.data?.email}>{children}</OwnerWorkspace>;
}
