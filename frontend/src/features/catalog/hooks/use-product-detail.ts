import {
  useAddCartItemMutation,
  useGetProductByIdQuery,
  useGetVariantAvailabilityQuery,
} from '@/shared/api/generated/api.generated';
import { toProductDetail } from '../model/product';

/** One product-detail use case: fetch, select an active SKU and check its stock. */
export function useProductDetail(
  productId: number,
  selectedVariantId: number | undefined,
  quantity: number,
) {
  const validId = Number.isSafeInteger(productId) && productId > 0;
  const productQuery = useGetProductByIdQuery({ id: productId }, { skip: !validId });
  // Pure derived data: the model adapter does not require its own React state or Effect.
  const product = toProductDetail(productQuery.data);
  const variant =
    product?.variants.find((item) => item.id === selectedVariantId) ??
    product?.variants.find((item) => item.isActive);
  const availability = useGetVariantAvailabilityQuery(
    { productVariantId: variant?.id ?? 0 },
    { skip: !variant },
  );
  const [addCartItem, addCartStatus] = useAddCartItemMutation();
  const availableQuantity = availability.data?.availableQuantity ?? 0;
  const canAddToCart =
    Boolean(variant?.isActive && availability.data?.isInStock) &&
    quantity > 0 &&
    quantity <= availableQuantity;

  return {
    validId,
    product,
    variant,
    productQuery,
    availability,
    addCartStatus,
    availableQuantity,
    canAddToCart,
    addToCart: () => {
      if (!variant || !canAddToCart) return;
      // Cart quantity and stock are revalidated by the server.
      void addCartItem({ body: { productVariantId: variant.id, quantity } });
    },
  };
}
