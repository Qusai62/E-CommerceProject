import { useState, type ReactNode } from 'react';
import { Link, useRouterState } from '@tanstack/react-router';
import { ArrowUpRight, Cpu, LayoutDashboard, Menu, Search, ShieldCheck, ShoppingCart, UserRound, X } from 'lucide-react';
import { useGetCartQuery, useGetCurrentUserQuery } from '@/shared/api/generated/api.generated';
import { hasRole } from '@/features/auth/model/roles';
import { Button } from '@/components/ui/button';
import { cn } from '@/shared/lib/cn';

function Brand() {
  return <Link to="/" className="inline-flex items-center gap-2.5" aria-label="Nexora home">
    <span className="grid size-9 place-items-center rounded-xl bg-moss text-white"><Cpu size={19}/></span>
    <span className="text-[22px] font-extrabold tracking-[-.065em]">NEXORA<span className="text-moss">.</span></span>
    <span className="hidden border-l border-line pl-3 text-[10px] font-bold tracking-[.14em] text-slate-400 lg:inline">TECH</span>
  </Link>;
}
function HeaderActions({ authenticated, isOwner }: { authenticated: boolean; isOwner: boolean }) {
  const { data: cart } = useGetCartQuery(undefined, { skip: !authenticated || isOwner });
  return <div className="flex items-center gap-1 sm:gap-2">
    {isOwner && <Link to="/owner" aria-label="Owner console" className="rounded-xl p-2.5 text-moss hover:bg-muted"><LayoutDashboard size={20}/></Link>}
    <Link to="/shop" aria-label="Search products" className="rounded-xl p-2.5 hover:bg-muted"><Search size={20}/></Link>
    <Link to={authenticated ? (isOwner ? '/owner' : '/account') : '/login'} aria-label={authenticated ? 'My account' : 'Sign in'} className="rounded-xl p-2.5 hover:bg-muted"><UserRound size={20}/></Link>
    {!isOwner && <Link to="/cart" aria-label="Shopping cart" className="relative rounded-xl p-2.5 hover:bg-muted"><ShoppingCart size={20}/>{Boolean(cart?.totalQuantity) && <span className="absolute -right-1 -top-1 grid min-h-5 min-w-5 place-items-center rounded-full bg-moss px-1 text-[10px] text-white">{cart?.totalQuantity}</span>}</Link>}
  </div>;
}
function Header() {
  const [open, setOpen] = useState(false);
  const path = useRouterState({ select: (state) => state.location.pathname });
  const currentUser = useGetCurrentUserQuery();
  const authenticated = Boolean(currentUser.data?.userId);
  const isOwner = hasRole(currentUser.data?.roles, 'StoreOwner');
  const navigation: Array<{ to: '/shop' | '/owner'; label: string }> = [{ to: '/shop', label: 'Shop electronics' }, ...(isOwner ? [{ to: '/owner' as const, label: 'Merchant console' }] : [])];
  return <header className="sticky top-0 z-40 border-b border-line bg-white/95 backdrop-blur-xl">
    <div className="page-container flex h-[74px] items-center justify-between gap-3">
      <div className="flex items-center gap-2"><button type="button" className="rounded-lg p-2 md:hidden" aria-label={open ? 'Close menu' : 'Open menu'} aria-expanded={open} onClick={() => setOpen(!open)}>{open ? <X size={20}/> : <Menu size={20}/>}</button><Brand/></div>
      <nav aria-label="Main navigation" className="hidden items-center gap-8 md:flex">{navigation.map((item) => <Link key={item.to} to={item.to} className={cn('text-sm font-medium transition-colors hover:text-moss', path.startsWith(item.to) ? 'text-moss' : 'text-slate-600')}>{item.label}</Link>)}</nav>
      <HeaderActions authenticated={authenticated} isOwner={isOwner}/>
    </div>
    {open && <nav aria-label="Mobile navigation" className="border-t border-line bg-white px-5 pb-4 md:hidden">{navigation.map((item) => <Link key={item.to} to={item.to} onClick={() => setOpen(false)} className="block rounded-lg px-3 py-3 text-sm hover:bg-muted">{item.label}</Link>)}</nav>}
  </header>;
}
function Footer() {
  return <footer className="mt-18 border-t border-line bg-white py-12"><div className="page-container grid gap-8 md:grid-cols-[2fr_1fr_1fr]">
    <div><Brand/><p className="mt-4 max-w-xs text-sm leading-7 text-slate-500">Discover everyday electronics, made simple. Your catalog, cart and account are powered by ASP.NET Core.</p><p className="mt-5 flex items-center gap-2 text-xs text-slate-500"><ShieldCheck size={15}/> Secure account access</p></div>
    <div><h3 className="mb-4 text-xs font-bold uppercase tracking-widest">Explore</h3><Link to="/shop" className="block py-2 text-sm text-slate-500 hover:text-moss">All electronics</Link><Link to="/cart" className="block py-2 text-sm text-slate-500 hover:text-moss">My cart</Link></div>
    <div><h3 className="mb-4 text-xs font-bold uppercase tracking-widest">My account</h3><Link to="/account" className="block py-2 text-sm text-slate-500 hover:text-moss">Profile</Link><Link to="/account/orders" className="block py-2 text-sm text-slate-500 hover:text-moss">My orders</Link></div>
  </div><div className="page-container mt-10 border-t border-line pt-5 text-xs text-slate-400">© 2026 Nexora · Electronics storefront</div></footer>;
}
export function SiteLayout({ children }: { children: ReactNode }) {
  return <div className="flex min-h-screen flex-col bg-cream text-ink">
    {import.meta.env.VITE_DEMO_MODE === 'true' && <div className="bg-blue-50 px-4 py-2 text-center text-[11px] font-semibold text-blue-800">DEMO MODE · Local sample data. Sign in as owner@aster.test or customer@aster.test.</div>}
    <div className="bg-ink px-3 py-2 text-center text-[11px] font-medium tracking-wide text-white">THE TECH YOU LOVE. THE PERFORMANCE YOU NEED.</div>
    <Header/><a href="#main-content" className="sr-only focus:not-sr-only focus:absolute focus:z-50 focus:bg-white focus:p-3">Skip to main content</a>
    <main id="main-content" className="flex-1">{children}</main><Footer/>
  </div>;
}
export function PageHeader({ eyebrow, title, description, action }: { eyebrow: string; title: string; description?: string; action?: ReactNode }) {
  return <div className="flex flex-wrap items-end justify-between gap-5"><div><p className="eyebrow">{eyebrow}</p><h1 className="font-display mt-3 text-3xl sm:text-4xl">{title}</h1>{description && <p className="mt-3 max-w-2xl text-sm leading-7 text-slate-500">{description}</p>}</div>{action}</div>;
}
export function BackLink({ to, children }: { to: '/shop'|'/owner'|'/owner/products'|'/account'; children: ReactNode }) { return <Link to={to} className="text-sm text-moss hover:underline">← {children}</Link>; }
export function TextLink({ to, children }: { to: '/shop'|'/account'|'/login'|'/register'|'/owner'; children: ReactNode }) { return <Button asChild variant="outline"><Link to={to}>{children}<ArrowUpRight size={15}/></Link></Button>; }
