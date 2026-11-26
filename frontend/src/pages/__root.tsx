import { lazy, Suspense } from 'react';
import { createRootRoute, Outlet, useRouterState } from '@tanstack/react-router';
import { SiteLayout } from '@/components/layout/site-layout';
import { LoadingState } from '@/components/ui/feedback';
import { NotFoundView } from '@/features/home/views/not-found-view';

const OwnerShell = lazy(async () => ({
  default: (await import('@/features/owner/layout/owner-shell')).OwnerShell,
}));

function RootLayout() {
  const isOwner = useRouterState({ select: (state) => state.location.pathname.startsWith('/owner') });
  if (isOwner) return <Suspense fallback={<LoadingState message="Preparing the merchant workspace…"/>}><OwnerShell><Outlet/></OwnerShell></Suspense>;
  return <SiteLayout><Outlet/></SiteLayout>;
}

export const Route = createRootRoute({ component: RootLayout, notFoundComponent: NotFoundView });
