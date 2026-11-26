# Shopping cart quantity — confirmed contract

`PUT /api/cart/items/{cartItemId}` accepts the now documented `UpdateCartItemQuantityRequest` containing an `int32` `quantity`; the response contains `cartItemId`, `productVariantId`, and `quantity`. This schema is included in the latest uploaded `swagger.json` (no provisional model remains).

The UI uses `features/cart/hooks/use-update-cart-quantity.ts` to call the generated RTK Query mutation; the quantity control is reusable and receives a number, pending flag, and callback. Successful mutations invalidate `Cart` and `Checkout`, causing server data to refresh. We deliberately do not copy totals into React local state. Invalid quantities, stock conflicts and authentication failures use inline error messages.

The updated API also supports **`DELETE /api/cart/items/{cartItemId}`** to remove a single cart entry and **`DELETE /api/cart/items`** to clear the cart. Both have shadcn-style/Radix confirmation dialogs and separate command hooks.

See `docs/INTEGRATION_GUIDE.md` for the complete purchase flow and real-backend checklist.
