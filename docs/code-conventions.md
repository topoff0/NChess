# Code Conventions

## General

- Keep commits small and focused.
- Do not mix backend, frontend, docs, and infrastructure changes without a clear reason.
- Prefer explicit names over abbreviations.
- Do not leave unused files, dead code, or commented-out code.
- Keep business logic out of controllers and React components.

## Backend

- API layer handles HTTP, auth claims, request/response mapping.
- Application layer contains use cases and MediatR handlers.
- Core layer contains entities, domain models, and repository interfaces.
- Infrastructure layer contains EF Core, repositories, external services, JWT, email, and database configuration.
- Controllers should not contain business rules.
- Use `Guid` for Account user ids.
- Do not use `Guid.Empty` as a fake user id.
- Use nullable values when absence is meaningful, for example `Guid? SecondPlayerId`.

## Frontend

- Use feature-based structure.
- Shared code goes to `shared` only if it is reused by multiple features.
- API calls live in `features/*/api`.
- Pages live in `features/*/pages`.
- Pure helper functions live in `features/*/lib`.
- Store JWT access token only through `shared/auth/tokenStorage`.
- Do not call `fetch` directly from pages. Use API modules.
- Prefer named exports.
- Use default exports only when a library/framework convention requires it.
- Use Tailwind classes for component styling.
- Keep `src/index.css` for global styles only.
- Avoid large component-specific CSS files unless Tailwind becomes unreadable.

## Imports

- Use aliases instead of long relative paths.
- Prefer:
  ```ts
  import { getAccessToken } from '@shared/auth/tokenStorage'
  ```
