# ASTER Tech 0.4.0 — Electronics E-Commerce

**Two distinct interfaces from one React + TypeScript codebase:** a mobile-first customer electronics storefront and a dedicated StoreOwner operations console. Both consume the uploaded **ECommerce.Api OpenAPI 3.0.1** definition, preserved in `openapi/swagger.original.json`.

**Contract coverage:** 46 HTTP operations / 65 OpenAPI schemas, including role-aware `/api/auth/login`, `/api/auth/me`, `/api/auth/logout`, customer order history/details, owner inventory, and category creation.

## Start here (Windows PowerShell)

Extract the ZIP, open a terminal **inside `ecommerce-frontend`** (the directory containing `package.json`), then:

```powershell
npm install
npm run dev
```

Visit `http://localhost:5173`. `.env` defaults to demo mode; **this does not connect to your backend**.

**Demo credentials:** `customer@aster.test` or `owner@aster.test` (any nonempty password). The demo uses deterministic local data and role checks; it is not an authentication system.

To use your ASP.NET Core API:

```dotenv
VITE_DEMO_MODE=false
API_PROXY_TARGET=https://localhost:7162
VITE_CURRENCY=JOD
VITE_IMAGE_BASE_URL=
VITE_CSRF_ENDPOINT=/api/security/csrf
VITE_PAYMENT_METHODS_VERIFIED=false
```

Use the actual backend HTTPS port. Restart `npm run dev` after editing environment variables. The frontend calls `/api` on the same origin and Vite forwards it to ASP.NET in development. Production requires a secure reverse proxy, authorization, cookie settings, and suitable CSRF protection.

## Customer pages

- `/`: electronics homepage and featured products.
- `/shop`: search, filters, sorting, pagination, product card grid.
- `/shop/:productId`: product details, variants, availability, and cart action (the GET detail schema needs documenting; see limitations).
- `/login` and `/register`: account entry. Login uses the **new `/api/auth/login` route**, then roles from `/api/auth/me`.
- `/account`, `/account/security`, `/account/addresses`, address CRUD views.
- `/account/orders` and `/account/orders/:orderId`: order history and item/address/total details.
- `/cart`: quantity stepper, remove item, clear cart, server totals.
- `/checkout`: address, payment method, backend review and shipping calculation; idempotent submission UUID.
- `/checkout/confirmation`: order confirmation and links to order history.

## StoreOwner pages

A **separate visual shell**: slate/navy collapsible sidebar, owner-only navigation, dense data tables, role checking, dashboard and operational insights.

- `/owner`: inventory KPIs, catalog totals, stock-health indicators and quick actions.
- `/owner/products`: paginated catalog with **valid `SortBy=createdAt&SortDirection=desc` defaults**.
- `/owner/products/new` and `/owner/products/:productId/...`: dedicated product info, variant, image and technical specification editor screens.
- `/owner/inventory`: stock adjustment, availability and out-of-stock filter.
- `/owner/categories`: create categories and list existing categories with their IDs and parent IDs. Editing and deletion are not currently exposed.
- `/owner/reports`: real operational inventory totals, stock-health sample and CSV export of first 50 rows. Revenue/sales charts are intentionally not fabricated.

**Role security:** `/api/auth/me` supplies an array of roles. The frontend uses `StoreOwner` for owner shell access and `Customer` for a customer landing destination; **server-side ASP.NET `[Authorize(Roles = "StoreOwner")]` remains mandatory**. UI guards are UX features, not a security boundary.

## Architecture

```text
src/
  pages/                        # TanStack Router file routes ONLY
  features/
    auth/{components,hooks,model,views}/
    home/{components,hooks,views}/
    catalog/{components,hooks,model,views}/
    cart/{components,hooks,model,views}/
    checkout/{components,hooks,model,views}/
    account/{components,model,views}/
    owner/{layout,components,containers,hooks,model,views}/
  components/ui/                # shadcn-style, locally owned Radix/Tailwind primitives
  components/layout/            # customer storefront shell
  shared/api/                   # RTK Query, cookie handling, OpenAPI-generated hooks
    generated/api.generated.ts  # DO NOT EDIT
    demoBaseQuery.ts            # local demo only
  shared/lib/                   # formatting, errors, class merging
  app/store.ts
```

Separation: **route → view → feature hook/application logic → RTK Query**. Presentational UI receives props and events. Derived totals remain derived, not duplicated in effects. Side effects are for external systems (form resets, requests); state is reserved for user interaction. Composition is preferred to HOCs; the existing authentication HOC is purpose-specific. TanStack Router Vite plugin has route-based code splitting enabled.

### UI/design system

Shadcn-style button, card, input, label, alert dialog, tabs and table primitives are locally owned under `src/components/ui`, built with Radix UI and Tailwind 4. They are **not asserted to be exact CLI-generated output**; use the official shadcn CLI locally if you need to re-sync with current registry components: https://ui.shadcn.com/docs/installation/vite. Iconography: Lucide. Mobile headers, accessible controls, keyboard states, loading, error, and empty states are included.

### Tooling

| Script                          | Function                                                  |
| ------------------------------- | --------------------------------------------------------- |
| `npm run api:generate`          | Offline deterministic OpenAPI → typed RTK Query client    |
| `npm run api:generate:official` | Optional official RTK code generator                      |
| `npm run api:check`             | Checks operation and schema coverage                      |
| `npm run check:architecture`    | Asserts routes remain thin and imports resolve            |
| `npm run routes:generate`       | TanStack Router CLI route tree                            |
| `npm run typecheck`             | Strict TypeScript check                                   |
| `npm run lint`                  | ESLint 9 React/TS rules, Airbnb-inspired conventions      |
| `npm run format`                | Apply Prettier                                            |
| `npm test`                      | Vitest model tests                                        |
| `npm run smoke:demo`            | Node smoke test of demo auth, role gating and order flows |
| `npm run build`                 | Production bundle + TypeScript project build              |
| `npm run check`                 | Full quality suite                                        |

Git hooks: `git init` then `npm run prepare`. Husky + lint-staged run on commit, Commitlint enforces Conventional Commits, and pre-push runs checks. Example: `feat(owner): add inventory reports`.

## Contract limitations that matter

1. **There is no StoreOwner sales/revenue reporting endpoint.** The report view presents actual inventory counts and labels sample-only aggregates accurately. It cannot truthfully show revenue, profits, orders across all customers, or best sellers.
2. `GET /api/orders` returns HTTP 200 with **no response schema**. The frontend parses a recognizable `items`-based or array response at runtime, and shows a contract warning if the shape differs. `GET /api/orders/{orderId}` is documented and typed.
3. `GET /api/products/{id}` and `POST /api/products` do **not** document success response payloads. Existing runtime validation is defensive. Publish those DTOs for a fully type-safe product-detail/editor flow.
4. `PaymentMethod` is represented by numeric **1/2** without enum labels in Swagger. The UI's mapping must be confirmed against your backend before enabling real order placement (`VITE_PAYMENT_METHODS_VERIFIED=true`). Online payments need gateway/payment-attempt support.
5. **CSRF protection:** before every unsafe RTK Query operation, the frontend calls `VITE_CSRF_ENDPOINT` (default `/api/security/csrf`), retains `requestToken` only in memory, and sends it as `X-CSRF-TOKEN`. The paired antiforgery cookie remains HttpOnly and browser-managed.
6. `GET /api/categories` supplies public category browsing, while `GET` and `POST /api/store/categories` support owner listing and creation. Brand lookup, category update/delete, owner order management, and financial reporting are not in this Swagger.
7. Product images expose storage keys rather than verified public URLs; configure an appropriate CDN URL. Currency is not specified in the API.

See `docs/API_GAPS.md`, `docs/INTEGRATION_GUIDE.md`, and `docs/ARCHITECTURE.md`.

## Verification

Passed in this workspace: OpenAPI generation (46 operations, 65 schemas), architecture/import checks, TypeScript **syntax parsing** of all TS/TSX files, and demo smoke test covering customer/owner role gates, category creation, cart operations, and order history/details. **Full npm dependency installation, TypeScript semantic checking, ESLint, Vitest, production build, and live backend testing are NOT verified** because npm registry access was unavailable here. Run `npm install` and `npm run check` on your machine.

## Official references

- https://tanstack.com/router/latest/docs/routing/file-based-routing
- https://redux.js.org/toolkit/rtk-query/usage/code-generation
- https://ui.shadcn.com/docs/installation/vite
- https://react.dev/learn/you-might-not-need-an-effect
- https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles
