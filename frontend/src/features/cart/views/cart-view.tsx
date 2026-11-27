import { Link } from '@tanstack/react-router';
import { ArrowRight, LockKeyhole, ShoppingBag, Trash2, ShieldCheck } from 'lucide-react';
import { ConfirmAction } from '@/components/ui/alert-dialog';
import { FormError } from '@/components/ui/feedback';
import { CheckoutStepper } from '@/features/checkout/components/checkout-stepper';
import { useCartCommands } from '../hooks/use-cart-commands';
import { useGetCartQuery } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { currency } from '@/shared/lib/format';
import { CartItemRow } from '../components/cart-item-row';

export function CartPage() {
  const query = useGetCartQuery();
  const { clear, clearing } = useCartCommands();

  if (query.isLoading) {
    return (
      <div className="page-container py-16">
        <LoadingState />
      </div>
    );
  }

  if (query.isError) {
    if ('status' in query.error && query.error.status === 401) {
      return (
        <div className="page-container py-16">
          <EmptyState
            icon={<LockKeyhole size={30} />}
            title="Sign in to view your cart"
            description="Your shopping cart belongs to your customer account."
            action={
              <Button asChild>
                <Link to="/login" search={{ redirect: '/cart' }}>
                  Sign in <ArrowRight size={15} />
                </Link>
              </Button>
            }
          />
        </div>
      );
    }
    return (
      <div className="page-container py-16">
        <ErrorState
          error={query.error}
          onRetry={() => {
            void query.refetch();
          }}
        />
      </div>
    );
  }

  const items = query.data?.items ?? [];
  const totalQuantity = query.data?.totalQuantity ?? 0;
  const totalAmount = query.data?.totalAmount;

  return (
    <div className="page-container py-12 sm:py-16">
      <PageHeader
        eyebrow="YOUR CART"
        title="Your shopping cart"
        description="Review your electronics and adjust quantities before checkout."
      />
      <div className="mt-8 rounded-2xl border border-line bg-white p-5 sm:px-8">
        <CheckoutStepper current={1} />
      </div>
      {items.length === 0 ? (
        <div className="mt-12">
          <EmptyState
            icon={<ShoppingBag size={30} />}
            title="Your cart is waiting"
            description="Discover your next tech essential and add it to your cart."
            action={
              <Button asChild>
                <Link to="/shop">
                  Shop electronics <ArrowRight size={16} />
                </Link>
              </Button>
            }
          />
        </div>
      ) : (
        <div className="mt-10 grid gap-8 lg:grid-cols-[minmax(0,1fr)_350px]">
          <section aria-label="Cart items" className="space-y-4">
            <div className="flex flex-wrap items-center justify-between gap-3">
              <p className="text-sm font-medium text-slate-600">
                {totalQuantity} items in your bag
              </p>
              <ConfirmAction
                destructive
                title="Clear the whole cart?"
                description="Every item will be removed from your cart."
                trigger={
                  <Button variant="outline" size="sm" disabled={clearing.isLoading}>
                    <Trash2 size={15} /> {clearing.isLoading ? 'Clearing…' : 'Clear cart'}
                  </Button>
                }
                onConfirm={() => {
                  void clear();
                }}
              />
            </div>
            {clearing.error && <FormError error={clearing.error} />}
            {items.map((item) => (
              <CartItemRow key={item.cartItemId} item={item} />
            ))}
            <p className="rounded-xl bg-blue-50 p-4 text-xs leading-6 text-stone-600">
              Quantities and subtotals refresh after the API confirms each change. Stock is
              rechecked at checkout.
            </p>
          </section>
          <Card className="h-fit p-6 sm:p-7">
            <h2 className="font-display text-2xl">Order summary</h2>
            <div className="mt-6 space-y-4 text-sm">
              <div className="flex justify-between gap-4">
                <span className="text-stone-600">Items ({totalQuantity})</span>
                <span>{currency(totalAmount)}</span>
              </div>
              <div className="flex justify-between gap-4">
                <span className="text-stone-600">Shipping</span>
                <span>Not calculated</span>
              </div>
            </div>
            <div className="mt-6 flex justify-between gap-4 border-t border-line pt-5 font-semibold">
              <span>Subtotal</span>
              <span>{currency(totalAmount)}</span>
            </div>
            <Button asChild className="mt-7 w-full" size="lg">
              <Link to="/checkout">
                <ShieldCheck size={16} /> Secure checkout <ArrowRight size={16} />
              </Link>
            </Button>
            <p className="mt-3 text-center text-xs leading-5 text-stone-500">
              Shipping and the final payable total are verified at checkout.
            </p>
            <Button asChild variant="ghost" className="mt-2 w-full">
              <Link to="/shop">Continue shopping</Link>
            </Button>
          </Card>
        </div>
      )}
    </div>
  );
}
