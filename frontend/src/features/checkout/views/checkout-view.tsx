import { Link } from '@tanstack/react-router';
import { ArrowLeft, ArrowRight, LockKeyhole, ShieldCheck, Truck } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, FormError, LoadingState, EmptyState } from '@/components/ui/feedback';
import { currency } from '@/shared/lib/format';
import { CheckoutStepper } from '../components/checkout-stepper';
import { AddressPicker } from '../components/address-picker';
import { PaymentMethodPicker } from '../components/payment-method-picker';
import { CheckoutOrderSummary } from '../components/checkout-order-summary';
import { useCheckout } from '../hooks/use-checkout';

export function CheckoutView() {
  const flow = useCheckout();
  const {
    cart,
    addresses,
    shipping,
    selectedAddress,
    selectedAddressId,
    paymentMethod,
    paymentMappingVerified,
    reviewStatus,
    currentReview,
    orderStatus,
  } = flow;
  const authError = [cart.error, addresses.error].some(
    (error) => error && 'status' in error && error.status === 401,
  );
  if (cart.isLoading || addresses.isLoading) {
    return (
      <div className="page-container py-16">
        <LoadingState message="Preparing secure checkout…" />
      </div>
    );
  }
  if (authError) {
    return (
      <div className="page-container py-16">
        <EmptyState
          icon={<LockKeyhole />}
          title="Sign in to checkout"
          description="Your saved addresses and shopping cart belong to your customer account."
          action={
            <Button asChild>
              <Link to="/login" search={{ redirect: '/checkout' }}>
                Sign in <ArrowRight />
              </Link>
            </Button>
          }
        />
      </div>
    );
  }
  if (cart.isError || addresses.isError) {
    return (
      <div className="page-container py-16">
        <ErrorState error={cart.error ?? addresses.error} />
      </div>
    );
  }
  const items = cart.data?.items ?? [];
  const hasItems = items.length > 0;
  const isReviewed = !!currentReview;
  const shippingPreview = shipping.isSuccess ? shipping.data?.shippingCharge : undefined;

  return (
    <div className="page-container pb-20 pt-9 sm:pt-14">
      <Link
        to="/cart"
        className="mb-6 inline-flex items-center gap-2 text-sm font-medium text-slate-500 hover:text-moss"
      >
        <ArrowLeft size={16} /> Back to cart
      </Link>
      <PageHeader
        eyebrow="SECURE CHECKOUT"
        title="Almost yours."
        description="Choose where to deliver your electronics, confirm payment, and review the total before placing your order."
      />
      <div className="mt-8 rounded-2xl border border-line bg-white px-5 py-5 sm:px-8">
        <CheckoutStepper current={isReviewed ? 3 : 2} />
      </div>
      {!hasItems ? (
        <div className="mt-12">
          <EmptyState
            title="Your cart is empty"
            description="Add a product before starting checkout."
            action={
              <Button asChild>
                <Link to="/shop">Explore products</Link>
              </Button>
            }
          />
        </div>
      ) : (
        <div className="mt-8 grid items-start gap-6 lg:grid-cols-[minmax(0,1fr)_370px] lg:gap-8">
          <div className="min-w-0 space-y-6">
            <Card className="p-5 sm:p-8">
              <div className="flex items-center gap-3">
                <span className="grid size-9 place-items-center rounded-full bg-blue-100 font-bold text-blue-700">
                  1
                </span>
                <div>
                  <h2 className="font-display text-xl">Delivery address</h2>
                  <p className="text-xs text-slate-500">Select a saved address, or add another</p>
                </div>
              </div>
              <div className="mt-6">
                <AddressPicker
                  addresses={addresses.data ?? []}
                  selectedId={selectedAddressId}
                  onSelect={flow.chooseAddress}
                />
              </div>
              {selectedAddress.isFetching && (
                <p className="mt-3 text-xs text-slate-500">Validating selected address…</p>
              )}
              {selectedAddress.isError && (
                <div className="mt-4">
                  <FormError error={selectedAddress.error} />
                </div>
              )}
            </Card>
            <Card className="p-5 sm:p-8">
              <div className="flex items-center gap-3">
                <span className="grid size-9 place-items-center rounded-full bg-blue-100 font-bold text-blue-700">
                  2
                </span>
                <div>
                  <h2 className="font-display text-xl">Payment method</h2>
                  <p className="text-xs text-slate-500">Select your preferred payment option</p>
                </div>
              </div>
              <div className="mt-6">
                <PaymentMethodPicker value={paymentMethod} onChange={flow.choosePayment} />
              </div>
            </Card>
            <Card className="p-5 sm:p-8">
              <div className="flex items-center gap-3">
                <span className="grid size-9 place-items-center rounded-full bg-blue-100 font-bold text-blue-700">
                  3
                </span>
                <h2 className="font-display text-xl">Order review</h2>
              </div>
              <p className="mt-4 text-sm leading-7 text-slate-600">
                We ask the server to verify prices, address, shipping, and availability before you
                submit. Nothing is charged at this step.
              </p>
              {currentReview && (
                <div
                  role="status"
                  className="mt-5 rounded-2xl border border-emerald-200 bg-emerald-50 p-5 text-sm text-emerald-900"
                >
                  <span className="font-bold">Ready to place your order</span>
                  <p className="mt-1">
                    Confirmed total: {currency(currentReview.finalTotal)} including{' '}
                    {currency(currentReview.shippingCharge)} shipping.
                  </p>
                </div>
              )}
              <div className="mt-5 space-y-3">
                <FormError error={reviewStatus.error} />
                <FormError error={orderStatus.error} />
              </div>
              {!paymentMappingVerified && (
                <p
                  role="alert"
                  className="mt-5 rounded-xl border border-amber-200 bg-amber-50 p-4 text-xs leading-6 text-amber-900"
                >
                  For real orders, verify the numeric values of your C# PaymentMethod enum and set{' '}
                  <code>VITE_PAYMENT_METHODS_VERIFIED=true</code> before enabling order submission.
                </p>
              )}
              {!isReviewed ? (
                <Button
                  className="mt-5 w-full sm:w-auto"
                  size="lg"
                  disabled={
                    !selectedAddressId ||
                    selectedAddress.isError ||
                    selectedAddress.isFetching ||
                    reviewStatus.isLoading
                  }
                  onClick={() => void flow.review()}
                >
                  {reviewStatus.isLoading ? 'Checking your order…' : 'Review order'}{' '}
                  <ArrowRight size={16} />
                </Button>
              ) : (
                <div className="mt-6 flex flex-col gap-3 sm:flex-row">
                  <Button
                    size="lg"
                    disabled={orderStatus.isLoading || !paymentMappingVerified}
                    onClick={() => void flow.submitOrder()}
                  >
                    {orderStatus.isLoading ? 'Placing order…' : 'Place order'}{' '}
                    <ArrowRight size={16} />
                  </Button>
                  <Button
                    variant="outline"
                    size="lg"
                    disabled={orderStatus.isLoading}
                    onClick={() => flow.choosePayment(paymentMethod)}
                  >
                    Edit selections
                  </Button>
                </div>
              )}
              <p className="mt-5 flex items-start gap-2 text-xs leading-5 text-slate-500">
                <LockKeyhole size={15} className="mt-0.5 shrink-0" />
                Your API is responsible for authentication, stock validation, order idempotency, and
                payment handling.
              </p>
            </Card>
          </div>
          <div className="space-y-4">
            <CheckoutOrderSummary
              items={items}
              productsTotal={cart.data?.productsTotal}
              review={currentReview}
              shippingPreview={shippingPreview}
            />
            <div className="grid gap-3 rounded-2xl border border-line bg-white p-5 text-xs text-slate-600">
              <p className="flex items-center gap-2">
                <ShieldCheck size={17} className="text-blue-600" /> Backend-validated totals
              </p>
              <p className="flex items-center gap-2">
                <Truck size={17} className="text-blue-600" /> Shipping calculated on review
              </p>
            </div>
            {shipping.isError && (
              <p role="status" className="text-xs leading-5 text-amber-800">
                Shipping estimate is temporarily unavailable. The checkout review will request the
                final charge.
              </p>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
