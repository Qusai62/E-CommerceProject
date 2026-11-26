# Role-based UX and API test plan

| Scenario               | Route / API                                | Expected                                               |
| ---------------------- | ------------------------------------------ | ------------------------------------------------------ |
| Guest storefront       | `/`, `/shop`                               | Browsing visible without auth                          |
| Public categories      | `GET /api/categories`                      | Category cards render and link to the filtered catalog |
| Guest owner            | `/owner` + `GET /api/auth/me`              | Sign-in required                                       |
| Customer login         | `POST /api/auth/login`, `GET /api/auth/me` | Role `Customer`, account destination                   |
| Customer owner URL     | `/owner`                                   | Access denied (server returns 403 for owner APIs)      |
| Owner login            | `POST /api/auth/login`, `GET /api/auth/me` | Role `StoreOwner`, owner dashboard                     |
| Owner catalog          | `/owner/products`                          | Valid SortBy `createdAt`, no 400                       |
| Owner create category  | `POST /api/store/categories`               | 201 and new category ID                                |
| Owner category list    | `GET /api/store/categories`                | 200 and category names, IDs, and parent IDs render     |
| Owner stock adjust     | POST increase/decrease                     | Server reflects correct stock                          |
| Owner reports          | `/owner/reports`                           | Backend totals accurate; sample clearly labeled        |
| Customer cart          | Add, change, remove, clear                 | Quantity and totals refresh from API                   |
| Customer order list    | `GET /api/orders`                          | Recognized items adapter or explicit contract warning  |
| Customer order details | `GET /api/orders/{orderId}`                | Items, address, total                                  |
| Checkout               | Summary, address, review, POST orders      | Server-reviewed totals; safe retry UUID                |
| Sign out               | `POST /api/auth/logout`                    | Session invalidated; client cache reset                |

Manual responsive checkpoints: 320, 375, 768, 1024, 1440px. Confirm menu accessibility, no clipped controls, keyboard tab order, focus, loading, failure and empty states.

Security: Verify server role authorization. Do not rely on frontend route guards. Verify antiforgery separately if required by backend; the current Swagger does not expose a CSRF initialization endpoint.

Run `npm run smoke:demo`, `npm run check`, and test real API responses in Chrome Network. Full npm-based checks were not run by the assistant due unavailable npm registry.
