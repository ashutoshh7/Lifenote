# Consistent Page Headers & Goals Scroll Fix

Ensure page header consistency across all pages (Dashboard, Goals, Habits, Pomodoro Timer, Settings) by creating a reusable, standalone `<app-page-header>` component. Address the scroll issue where navigating to the Goals edit page incorrectly scrolls the layout container down on load.

## User Review Required
> [!IMPORTANT]
> The reusable `<app-page-header>` component uses content projection (`<ng-content>`) to allow actions (like "New Goal" buttons) to be passed in from parent pages.
> The Notes workspace has a custom two-column design where the header is inline within the left sidebar rather than a full-width page header. This matches the design specifications.

## Proposed Changes

---

### Reusable Page Header Component

#### [NEW] [page-header.component.ts](file:///d:/Lifenote/frontend/src/app/shared/components/page-header/page-header.component.ts)
- Create a standalone component that accepts `title` (required string) and `subtitle` (optional string) inputs.
- Define a content projection slot for actions.

#### [NEW] [page-header.component.html](file:///d:/Lifenote/frontend/src/app/shared/components/page-header/page-header.component.html)
- Define structural markup using HTML5 semantic `<header>` with `.page-header` class.
- Display `title` and conditionally render `subtitle` if present.
- Define container for projected actions.

#### [NEW] [page-header.component.scss](file:///d:/Lifenote/frontend/src/app/shared/components/page-header/page-header.component.scss)
- Style the page header using existing SCSS variables (`_variables.scss`).
- Apply responsive flex layout to align title on the left and actions on the right, stacking on mobile viewports.

---

### Dashboard Feature

#### [MODIFY] [dashboard-page.component.html](file:///d:/Lifenote/frontend/src/app/features/home/pages/dashboard-page/dashboard-page.component.html)
- Replace bespoke `.welcome-section` header with `<app-page-header>`.

#### [MODIFY] [dashboard-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/home/pages/dashboard-page/dashboard-page.component.ts)
- Import `PageHeaderComponent` in the standalone metadata imports.

#### [MODIFY] [dashboard-page.component.scss](file:///d:/Lifenote/frontend/src/app/features/home/pages/dashboard-page/dashboard-page.component.scss)
- Remove custom styling for `.welcome-section`, `.greeting`, and `.subtext` to rely on the shared header's style rules.

---

### Goals Feature

#### [MODIFY] [goals-page.component.html](file:///d:/Lifenote/frontend/src/app/features/goals/pages/goals-page/goals-page.component.html)
- Replace bespoke `<header class="goals-header">` with `<app-page-header>`.
- Project the "New Goal" button inside `<app-page-header>`.

#### [MODIFY] [goals-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/goals/pages/goals-page/goals-page.component.ts)
- Import `PageHeaderComponent` in imports.

#### [MODIFY] [goals-page.component.scss](file:///d:/Lifenote/frontend/src/app/features/goals/pages/goals-page/goals-page.component.scss)
- Remove custom header layout styles (`.goals-header`, `.header-text`, etc.). Keep page button styles (`.add-goal-btn`).

---



### Pomodoro Timer Feature

#### [MODIFY] [pomodoro-page.component.html](file:///d:/Lifenote/frontend/src/app/features/pomodoro/pages/pomodoro-page/pomodoro-page.component.html)
- Prepend the `<app-page-header>` component with title "Timer" and subtitle "Time-boxed deep focus sessions" to add consistency.

#### [MODIFY] [pomodoro-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/pomodoro/pages/pomodoro-page/pomodoro-page.component.ts)
- Import `PageHeaderComponent` in imports.

---

### Settings Feature

#### [MODIFY] [settings-page.component.html](file:///d:/Lifenote/frontend/src/app/features/settings/pages/settings-page/settings-page.component.html)
- Replace custom `<header class="settings-header">` with `<app-page-header>`.

#### [MODIFY] [settings-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/settings/pages/settings-page/settings-page.component.ts)
- Import `PageHeaderComponent` in imports.

#### [MODIFY] [settings-page.component.scss](file:///d:/Lifenote/frontend/src/app/features/settings/pages/settings-page/settings-page.component.scss)
- Clean up custom `.settings-header` style definitions.

---

### Global Scroll Issue

#### [MODIFY] [app.component.ts](file:///d:/Lifenote/frontend/src/app/app.component.ts)
- Update the `router.events` NavigationEnd subscriber to perform scroll restoration in a `setTimeout(() => ..., 0)`.
- This ensures that scrolling to top is executed after the new route component is fully rendered in the DOM, preventing browser/routing page height jumps.

## Verification Plan

### Manual Verification
- Run the Angular development server (`npm run dev`) and test page navigation across all features.
- Check headers on: Dashboard, Goals, Pomodoro Timer, Settings. Ensure they display consistently in size, font, and padding.
- Verify responsiveness of headers: Title and action buttons should stack cleanly on mobile.
- Verify Scroll Restoration: Scroll down on the Goals list page, click to edit a goal. Verify the edit page loads scrolled to the very top.
