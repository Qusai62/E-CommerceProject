# Migrating from ASTER Tech 0.3.x

Use 0.4.0 as the complete project. You do not need to overlay older archives. Back up local changes first.

Critical backend contract changes:

- Removed `/api/customers/login`, `/api/customers/logout`, `/api/customers/csrf` and session-test from the latest Swagger.
- Added `/api/auth/login`, `/api/auth/logout`, `/api/auth/me` with roles.
- Added `GET /api/orders` (200 response lacks schema), `GET /api/orders/{orderId}`, and `POST /api/store/categories`.
- Preserved product/catalog, cart, account, checkout, inventory and owner editor workflows.

After extraction, run `npm install`, then `npm run check` and `npm run dev` from `ecommerce-frontend`. Use demo logins to inspect roles. Configure `.env` before real API integration.

Old versions expect an antiforgery JSON token from `/api/customers/csrf`; do **not** copy those hooks back. The new Swagger needs separate clarification for the password-change CSRF header.
