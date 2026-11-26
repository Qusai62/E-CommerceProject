# Responsive UX Checklist

Customer: touch-friendly product cards and quantity controls, keyboard-accessible forms, visible validation, server-derived totals, empty/error/loading states, checkout steps and order confirmation. StoreOwner: collapsible mobile sidebar, clear active navigation, separate CRUD views, actionable stock KPIs, horizontal table overflow, disabled/pending mutation buttons, and accurate reporting labels.

Manual viewport QA: 320px, 375px, 768px, 1024px, 1440px. Verify no clipped actions, horizontal document scrolling, focus visibility, tab order, screen-reader button names and reduced-motion preference.

Authentication QA: Guest `/owner` → sign-in. Customer `/owner` → access denied. StoreOwner `/owner` → dashboard. Customer account links show orders; StoreOwner navigation remains visually and functionally distinct.

Error QA: backend 400 validation arrays, 401 unauthenticated, 403 unauthorized, 404 missing resource, 409 stock conflict, 503 payment/shipping unavailable. Do not show server errors as successful mutations.

Financial integrity: never compute final checkout totals from unsynchronized local state, assume a payment was completed when an order was created, or display revenue based on guessed figures. Owner report totals must clarify full API counts vs 50-row samples.

Audit against WCAG 2.2 AA and validate real customer/owner flows with browser accessibility tools before production release.
