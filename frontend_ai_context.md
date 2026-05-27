# Momentum - Frontend AI Context Document

This document is designed to provide context, architectural patterns, and strict guidelines for any AI coding assistant interacting with the Momentum (formerly Lifenote) Frontend. Read this entirely before proposing or making changes.

## 1. Tech Stack
- **Framework:** Angular (Standalone Components)
- **Language:** TypeScript
- **Styling:** SCSS (Vanilla CSS principles, highly polished UI, no Tailwind)
- **State Management:** Angular Signals (`signal`, `computed`, `effect`) and RxJS (for API calls)

## 2. Directory Structure
The frontend strictly follows a Feature-based architecture located at `src/app/`:

- `/core`: Singleton services, guards, interceptors, base models.
  - *Rule:* Only import from `/core` if the service is truly global (e.g., `ToastService`, `ThemeService`, `AuthService`).
- `/features`: Domain-specific modules (e.g., `goals`, `notes`, `home`, `pomodoro`, `settings`).
  - *Rule:* Features should be isolated. A feature can import from `/shared` or `/core`, but should rarely import from another feature to prevent circular dependencies.
  - *Structure per feature:* `/components` (dumb), `/pages` (smart/routed), `/services` (API logic), `/models` (DTOs/Interfaces).
- `/shared`: Reusable, generic UI components (`app-goal-card`, `app-page-header`, `app-search-bar`), directives, and pipes.
- `/layout`: App shell components (Sidebar, Topbar).

## 3. Data & Typing Rules (CRITICAL)
- **Identifiers:** ALL identifiers (Primary Keys, Foreign Keys) are `string` (UUIDs). **Do NOT use `number` for IDs.** This matches the PostgreSQL backend which was refactored to GUIDs.
- **Strict Typing:** Always use interfaces for API responses and component inputs. Do not use `any`.
- **Parsing IDs:** When grabbing an ID from the Angular router, do not cast it to a number. Example: `const id = this.route.snapshot.paramMap.get('id');`

## 4. State Management & Reactivity
- **Signals First:** Use Angular Signals (`signal<T>`, `computed()`, `update()`, `set()`) for managing component-level state.
- **RxJS for Async:** Use RxJS `Observable<T>` primarily for HTTP requests in services. Components should `subscribe` to the service, then immediately set the result into a Signal. Do not mix complex RxJS pipelines into component templates.
- **Guard Clauses:** When implementing state-modifying actions (like timer starts/stops), add strict guard clauses to prevent redundant API calls or invalid state transitions (e.g., `if (this.isRunning()) return;`).

## 5. UI / UX Standards
- **Silent Failures are Banned:** Any API error or successful background action must use `ToastService.show(message, type)`. Do not just use `console.error`.
- **Premium Aesthetics:** Momentum demands a rich, dynamic aesthetic. Utilize CSS variables for theming, include smooth micro-animations/transitions, and use glassmorphism or sleek hover states. Do not output plain, generic UI.
- **Disabled States:** Interactive elements (buttons, inputs) must visually reflect when an action is unavailable or when a request is in-flight using `[disabled]`.

## 6. Common Pitfalls to Avoid
- **Rebranding:** The app was renamed from "Lifenote" to "Momentum". Do not introduce the word "Lifenote" into the UI.
- **Component Complexity:** Do not bloat Smart Components (Pages). If a page has complex visual elements, extract them into Dumb Components in the feature's `/components` directory.
- **Magic Strings:** Use constants or enumerations for repeated strings (e.g., `TaskStatus.COMPLETED`, `ThemeModes.DARK`).
