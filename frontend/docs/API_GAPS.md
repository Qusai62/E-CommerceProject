# API gaps — 13 November 2026 Swagger

ECommerce.Api OpenAPI 3.0.1: **46 operations, 36 paths, 65 schemas**.

## Owner reporting

- `GET /api/owner/inventory` and `/api/owner/inventory/out-of-stock/{products|variants}` support operational counts. The owner dashboard shows their `totalCount` values.
- The first 50 inventory rows are used for per-row quantities, CSV export and stock health. The UI explicitly labels these **sampled** numbers; it does not extrapolate them to full inventory.
- **Not available:** authorized owner-wide orders, sales revenue by date, conversion, profits, best-selling products, refunds or payment success analytics. Proposed endpoints: `GET /api/owner/reports/sales?from&to`, `/inventory-summary`, `/orders`. Define response schemas and apply role authorization.

## Category/catalog contracts

- `GET /api/categories` supports public category browsing and powers the storefront’s category links.
- `GET` and `POST /api/store/categories` support listing and creation. Category update/delete endpoints are still needed for a complete category manager.
- `GET /api/products/{id}` 200 and `POST /api/products` 201 omit response models. Publish real DTOs with image/variant/spec IDs, active status and appropriate permissions.
- Product list omits active/draft status, while owner needs draft listings; define an owner catalog endpoint or documented filtering.
- Product images use `storageKey`; publish a public media URL or signed URL contract. Prices lack a currency declaration.
- `SortBy` should accept name, price, createdAt; owner queries supply `createdAt` by default to avoid the observed InvalidSortBy 400.

## Auth

- Current auth paths are `POST /api/auth/login`, `POST /api/auth/logout`, and `GET /api/auth/me`. `LoginResponse`/`CurrentUserResponse` include `roles` (strings).
- Frontend role routing is not an authorization boundary. ASP.NET must protect owner endpoints and per-customer resources.
- Previous `/api/customers/csrf` was removed. Swagger still lists an `X-CSRF-TOKEN` header on `POST /api/customers/me/change-password` without a token provider. Clarify whether antiforgery is required and publish a token-bootstrap contract if necessary; do not invent one.
- Document authentication cookie behavior, SameSite/Secure configuration and whether BFF cookies use an antiforgery token on all unsafe methods.

## Orders/checkout

- `GET /api/orders` declares 200 with **no schema**. Strongly recommend a documented `CustomerOrdersPagedResponse` with `items: CustomerOrderSummaryResponse[]`, pagination and enum fields. The provided frontend uses runtime validation instead of pretending the contract exists.
- `GET /api/orders/{orderId}` is documented and powers customer order details.
- Numeric `PaymentMethod` 1/2 values have no named mapping. Live placement remains blocked until values are verified; card payments need a gateway integration.
- `GET /api/checkout/shipping` has no address ID request parameter, so it is only a preview. `POST /api/checkout/review` supplies the final authoritative amount.
- `POST /api/checkout/payment-attempts/{id}/stock-reservation` exists but there is no gateway creation endpoint to obtain a valid payment attempt ID.
