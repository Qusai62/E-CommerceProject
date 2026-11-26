# Contributing to ASTER Tech

## Conventions

- Prefer TypeScript types generated from the OpenAPI document. Never edit generated files directly.
- Place **only routing configuration** under `src/pages`; implement screen markup under `src/features/<feature>/views`.
- Place presentational widgets under `features/*/components` or global `components/ui`. They should not import HTTP/RTK Query hooks directly.
- Place API orchestration under feature `hooks`; put pure transformations in `model`.
- Prefer composition and render props. Use HOCs only for cross-cutting behavior such as customer-session gating.
- Avoid effects for values that can be calculated while rendering. Do not mirror API data in `useState`; keep state for editable drafts and interactions.
- Keep accessibility visible in code: labels, alt text, keyboard interactions, focus indicators and meaningful empty/error/success states.
- `eslint.config.mjs` has an **Airbnb-inspired** rule subset with React Hooks and accessibility checks, not the full official preset.

## Local workflow

```bash
npm install
npm run routes:generate
npm run format
npm run lint:fix
npm run typecheck
npm test
npm run build
```

If using Git from the distributed ZIP, initialize it and activate hooks:

```bash
git init
npm run prepare
```

Use conventional commit messages; Commitlint rejects nonconforming messages:

```text
feat(catalog): add query-string sorting
fix(auth): block login without csrf token
refactor(owner): isolate image upload command
chore(tooling): update lint rules
```

Husky runs lint-staged on pre-commit; commitlint on commit-msg; typecheck/tests on pre-push. Keep feature changes small and include a focused test when adding a pure model function. Treat authorization and stock integrity as **server-side** responsibilities.

## References

- [React effects](https://react.dev/learn/you-might-not-need-an-effect)
- [Airbnb JS style](https://github.com/airbnb/javascript)
- [RTK Query code generation](https://redux.js.org/toolkit/rtk-query/usage/code-generation)
- [Husky hooks](https://typicode.github.io/husky/get-started.html)
