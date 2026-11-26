# ASTER Tech v0.4.0 Architecture

## Presentation boundaries

- `src/pages`: **thin TanStack Router declarations only**. Route-based automatic code splitting is enabled in `vite.config.ts`.
- `src/features/*/views`: compose pages, data loading, errors, empty states and route navigation.
- `src/features/*/components`: small presentational controls receiving narrow values/callbacks; no direct HTTP routes or caching logic.
- `src/features/*/hooks`: orchestrate RTK Query and use-case state, not markup.
- `src/features/*/model`: pure validation, role predicates, order-list decoding, stock reporting and request adapters.
- `src/shared/api/generated/api.generated.ts`: generated contracts only. Run `npm run api:generate`; never hand edit.
- `src/components/ui`: locally owned shadcn-style Radix/Tailwind UI primitives.

## Separate role-based shells

- Customer routes mount `SiteLayout`: sticky storefront navigation, search/account/cart actions, featured electronics, responsive checkout.
- `/owner/*` routes lazily mount `OwnerShell`: dark collapsible sidebar, owner header, inventory and operational reporting screens.
- `OwnerShell` queries `GET /api/auth/me` and requires a `StoreOwner` role. A Customer sees a polite access-denied screen. Guest sees a login link with safe redirect.
- Post-login navigation derives from `LoginResponse.roles`, not guessing from email. A demo-only email selector simulates the backend.
- Neither shell provides authorization; the backend must secure endpoints and bind customer data by authenticated identity.

## SOLID and composition

- Single Responsibility: category form validates UI, category view orchestrates, generated API handles transport; report model computes metrics from passed rows.
- Open/Closed: new reports add a view/model and route rather than modifying generic Card, Button or router internals.
- Liskov: UI controls forward native HTML props, disabled and keyboard behavior.
- Interface Segregation: presentational components accept focused props, not the entire app state.
- Dependency Inversion: feature logic depends on generated hooks, with one reusable RTK Query base API; demo/live transport is switched at the base boundary.
- Composition by default: product editor render-prop container, small components and layout shells. Purpose-specific authentication HOC remains for customer-only screens.

## React guidance

- Keep server state in RTK Query; mutations invalidate cache tags. Do not copy cart totals, response objects or computed availability into `useState`.
- `useState` for genuine UI concerns: menu visibility, form input, selected address, page, confirmation dialogs.
- `useRef` for checkout submission UUID across retries; server still enforces idempotency.
- `useEffect` only to synchronize with external systems, e.g. react-hook-form resets after async profile response; never to compute derived totals.

## Report integrity

`useOwnerReports` returns backend-wide `totalCount` plus the first 50 inventory rows. `summarizeLoadedStock` and `stockCsv` are pure functions. The UI labels sampled numbers clearly and does **not** substitute simulated revenue. CSV values are escaped, including spreadsheet-formula prefixes.

## Tooling

ESLint flat config, AirBnB-inspired rules (not full upstream preset), Prettier, Husky, lint-staged, Commitlint, Vitest, strict TypeScript, route and API contract checks. Run `npm run check` after installing dependencies. The official shadcn CLI was not run here; current components are compatible locally owned Radix/Tailwind primitives.
