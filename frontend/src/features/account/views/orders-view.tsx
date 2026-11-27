import { useState } from 'react';
import { Link } from '@tanstack/react-router';
import { ClipboardList } from 'lucide-react';
import { useGetOrdersQuery } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';
import { AccountNav, SignInRequired } from '../components/account-nav';
import { OrderListCard } from '../components/order-list-card';
import { parseOrderList } from '../model/order-list';

export function CustomerOrdersPage() {
  const [page, setPage] = useState(1);
  const orders = useGetOrdersQuery({ pageNumber: page, pageSize: 10 });
  const data = parseOrderList(orders.data, page);
  if (orders.isLoading) return <LoadingState message="Loading your orders…" />;
  if (orders.isError && 'status' in orders.error && orders.error.status === 401) {
    return <SignInRequired />;
  }
  return (
    <div className="page-container py-14">
      <PageHeader
        eyebrow="MY PURCHASES"
        title="Order history"
        description="Review past orders and open each purchase for delivery and item details."
      />
      <div className="mt-9 grid gap-7 lg:grid-cols-[260px_1fr]">
        <AccountNav />
        <div className="space-y-4">
          {orders.isError ? (
            <ErrorState error={orders.error} onRetry={() => void orders.refetch()} />
          ) : !data ? (
            <Card className="p-6 sm:p-8">
              <h2 className="text-lg font-bold">Order list contract is missing</h2>
              <p className="mt-3 text-sm leading-7 text-slate-600">
                Swagger declares GET /api/orders but does not describe its 200 response. This
                frontend safely accepts a paginated object with an items array containing orderId,
                or a direct order array. Document the actual response in Swagger for full type-safe
                rendering.
              </p>
            </Card>
          ) : data.items.length === 0 ? (
            <EmptyState
              icon={<ClipboardList size={30} />}
              title="No orders yet"
              description="Your purchases will appear here after checkout."
              action={
                <Button asChild>
                  <Link to="/shop">Shop electronics</Link>
                </Button>
              }
            />
          ) : (
            <>
              {data.items.map((order) => (
                <OrderListCard key={order.id} order={order} />
              ))}
              <div className="flex items-center justify-between gap-3 py-4">
                <p className="text-xs text-slate-500">
                  Page {data.pageNumber}
                  {data.totalPages ? ` of ${data.totalPages}` : ''}
                </p>
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    disabled={!data.hasPreviousPage}
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                  >
                    Previous
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    disabled={!data.hasNextPage}
                    onClick={() => setPage((p) => p + 1)}
                  >
                    Next
                  </Button>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
