# Live ASP.NET Core Integration — Role-Aware Frontend

1. Start your API and confirm its HTTPS address. Set `VITE_DEMO_MODE=false` and `API_PROXY_TARGET=https://localhost:<your-port>` in `.env`; restart Vite.
2. `POST /api/auth/login` should set the browser session cookie. `GET /api/auth/me` must return current user `roles`, for example `['Customer']` or `['StoreOwner']`. The frontend navigates based on roles, but all owner APIs must independently return 403 for non-owners.
3. `POST /api/auth/logout` should revoke the server session. The frontend then clears the RTK Query cache and returns to login.
4. Test store listing at `GET /api/products?PageNumber=1&PageSize=10&SortBy=createdAt&SortDirection=desc`. If it returns 400, inspect the backend validation body; frontend now displays error arrays.
5. Customer: browse products → add SKU → adjust cart → saved address → checkout review → submit order → `/account/orders` → order details. If orders page reports an unknown shape, document the success DTO for `GET /api/orders` in Swagger.
6. Owner: sign in with StoreOwner → `/owner` overview → create/edit products → adjust inventory → create category → `/owner/reports`. Reports only use documented inventory endpoints.
7. Confirm actual `PaymentMethod` numeric mapping and set `VITE_PAYMENT_METHODS_VERIFIED=true` only when correct. Online payment is not available without a gateway endpoint.
8. The frontend obtains antiforgery tokens from `GET /api/security/csrf` by default (override with `VITE_CSRF_ENDPOINT`). The response must include `requestToken` and set the paired antiforgery cookie; unsafe requests send the token in `X-CSRF-TOKEN`.
9. Images need `VITE_IMAGE_BASE_URL` or documented API-resolved URLs. Avoid untrusted image URLs.
10. Verify cookie scope and Secure/SameSite behavior under the actual frontend origin and HTTPS deployment. Never store session cookies, authentication secrets, or CSRF tokens in localStorage.

Recommended verification commands:

```powershell
npm install
npm run api:check
npm run check:architecture
npm run smoke:demo
npm run typecheck
npm run lint
npm test
npm run build
```

If `npm install` fails, stop and correct the first dependency error before running Vite.
