import { useRef, useState } from 'react';
import { useNavigate } from '@tanstack/react-router';
import {
  useGetAddressesQuery,
  useGetCheckoutQuery,
  useGetShippingChargeQuery,
  useSelectCheckoutAddressQuery,
  useReviewCheckoutMutation,
  usePlaceOrderMutation,
  type PaymentMethod,
} from '@/shared/api/generated/api.generated';
import { isReadyToPlaceOrder, isReviewCurrent, resolveAddressId } from '../model/checkout';

/** Application orchestration. No UI elements, HTTP URLs or duplicated server totals live here. */
export function useCheckout() {
  const navigate = useNavigate();
  // Fail closed for real orders until the backend PaymentMethod numeric mapping is verified.
  const paymentMappingVerified = import.meta.env.VITE_DEMO_MODE === 'true' ||
    import.meta.env.VITE_PAYMENT_METHODS_VERIFIED === 'true';
  const cart = useGetCheckoutQuery();
  const addresses = useGetAddressesQuery();
  // Shipping endpoint has no address parameter; treat it as indicative until review.
  const shipping = useGetShippingChargeQuery();
  const [selectedOverride, setSelectedOverride] = useState<number>();
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>(2);
  const selectedAddressId = resolveAddressId(addresses.data ?? [], selectedOverride);
  const selectedAddress = useSelectCheckoutAddressQuery(
    { savedAddressId: selectedAddressId ?? 0 },
    { skip: selectedAddressId === undefined },
  );
  const [reviewCheckout, reviewStatus] = useReviewCheckoutMutation();
  const [placeOrder, orderStatus] = usePlaceOrderMutation();
  const currentReview = isReviewCurrent(reviewStatus.data, cart.data, selectedAddressId, paymentMethod)
    ? reviewStatus.data : undefined;
  // Stable across retries of the SAME submission; never create a new UUID on each click.
  const submissionId = useRef<string | null>(null);

  function chooseAddress(id: number) {
    setSelectedOverride(id);
    reviewStatus.reset();
  }
  function choosePayment(method: PaymentMethod) {
    setPaymentMethod(method);
    reviewStatus.reset();
  }
  async function review() {
    if (selectedAddressId === undefined || reviewStatus.isLoading) return;
    try {
      await reviewCheckout({ body: { savedAddressId: selectedAddressId, paymentMethod } }).unwrap();
    } catch {
      // 4xx / 503 stay in reviewStatus.error and are shown by the view.
    }
  }
  async function submitOrder() {
    if (!paymentMappingVerified || !isReadyToPlaceOrder(currentReview) || selectedAddressId === undefined || orderStatus.isLoading) return;
    submissionId.current ??= crypto.randomUUID();
    try {
      const result = await placeOrder({ body: {
        checkoutSubmissionId: submissionId.current,
        savedAddressId: selectedAddressId,
        paymentMethod,
      } }).unwrap();
      await navigate({
        to: '/checkout/confirmation',
        search: { orderNumber: result.orderNumber ?? '', orderId: result.orderId ?? 0 },
        replace: true,
      });
    } catch {
      // Retain submission ID for retry to preserve idempotency.
    }
  }
  return {
    cart, addresses, shipping, selectedAddress,
    selectedAddressId, paymentMethod, paymentMappingVerified, chooseAddress, choosePayment,
    reviewStatus, currentReview, orderStatus, review, submitOrder,
  };
}
