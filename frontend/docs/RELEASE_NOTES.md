# ASTER Tech v0.4.0

- Migrated authentication from `/api/customers/*` to `/api/auth/*` for login/logout/current user and removed the obsolete CSRF initialization call.
- Added explicit `Customer` and `StoreOwner` role handling with a separate mobile-responsive dark merchant console.
- Added customer order history and order details UI. Order listing has defensive runtime parsing because the Swagger 200 response body is undocumented.
- Added owner category creation from `POST /api/store/categories`.
- Added owner category listing from `GET /api/store/categories`.
- Added public category browsing from `GET /api/categories`.
- Added owner inventory reports with documented API totals, 50-row sample visual and sanitized CSV export. No fake revenue charts.
- Improved default product sorting to fix backend `InvalidSortBy` response and improved array validation error messages.
- Updated demo role behavior and a self-contained smoke check for login/roles, category creation, cart, orders.
- Retained existing catalog, account, cart, checkout, product media/spec/variant management and responsive UI primitives.
- 46 API operations, 65 schemas.

**Not verified:** Dependency-based TypeScript check, lint, Vitest, Vite build, and live backend integration. npm registry was unreachable in this environment. Run `npm run check` locally.
