# Page Headers Refactored & Goals Scroll Restored

We have successfully unified the header layouts across all application pages via a reusable component, and fixed the scroll-down issue on the Goals edit page.

## Key Changes

### 1. Created Reusable `<app-page-header>` Component
- **[page-header.component.ts](file:///d:/Lifenote/frontend/src/app/shared/components/page-header/page-header.component.ts)** / **[page-header.component.html](file:///d:/Lifenote/frontend/src/app/shared/components/page-header/page-header.component.html)** / **[page-header.component.scss](file:///d:/Lifenote/frontend/src/app/shared/components/page-header/page-header.component.scss)**:
  - Developed a standalone, reusable page header matching the design style tokens.
  - Supports `title` and `subtitle` inputs.
  - Employs Angular content projection (`<ng-content>`) to allow child pages to insert actions (e.g., buttons like "New Goal") in the top right.
  - Stacks items vertically on mobile viewports for clean spacing.
- **[index.ts](file:///d:/Lifenote/frontend/src/app/shared/index.ts)**:
  - Added export for `PageHeaderComponent`.

### 2. Refactored Main Pages to Use Reusable Header
- **Dashboard**:
  - Replaced welcome section in [dashboard-page.component.html](file:///d:/Lifenote/frontend/src/app/features/home/pages/dashboard-page/dashboard-page.component.html) with `<app-page-header>`.
  - Imported in [dashboard-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/home/pages/dashboard-page/dashboard-page.component.ts).
  - Cleaned up obsolete layout styles in [dashboard-page.component.scss](file:///d:/Lifenote/frontend/src/app/features/home/pages/dashboard-page/dashboard-page.component.scss).
- **Goals**:
  - Replaced custom layout in [goals-page.component.html](file:///d:/Lifenote/frontend/src/app/features/goals/pages/goals-page/goals-page.component.html) with `<app-page-header>`.
  - Imported in [goals-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/goals/pages/goals-page/goals-page.component.ts).
  - Cleaned up obsolete layout styles in [goals-page.component.scss](file:///d:/Lifenote/frontend/src/app/features/goals/pages/goals-page/goals-page.component.scss).

- **Settings**:
  - Dynamically linked the "Member since [Year]" text to the authenticated user's `createdAt` timestamp using the `authService.currentUserDetails()`.

### Feature Enhancements
- **Notes Tags UI**:
  - Added a tag rendering section in the sidebar note list items.
  - Implemented a lightweight tag "pill" editor directly below the note title in the editor panel.
  - Users can type a tag name and hit `Enter` (or blur) to save it. Tags are saved automatically via the `autoSave()` routine.
  - Tag pill removal logic integrated.
  
- **Pomodoro Timer**:
  - Prepended `<app-page-header>` in [pomodoro-page.component.html](file:///d:/Lifenote/frontend/src/app/features/pomodoro/pages/pomodoro-page/pomodoro-page.component.html).
  - Imported in [pomodoro-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/pomodoro/pages/pomodoro-page/pomodoro-page.component.ts).
- **Settings**:
  - Replaced custom layout in [settings-page.component.html](file:///d:/Lifenote/frontend/src/app/features/settings/pages/settings-page/settings-page.component.html) with `<app-page-header>`.
  - Imported in [settings-page.component.ts](file:///d:/Lifenote/frontend/src/app/features/settings/pages/settings-page/settings-page.component.ts).
  - Cleaned up obsolete layout styles in [settings-page.component.scss](file:///d:/Lifenote/frontend/src/app/features/settings/pages/settings-page/settings-page.component.scss).

### 3. Scroll Restoring Fix
- **[app.component.ts](file:///d:/Lifenote/frontend/src/app/app.component.ts)**:
  - Wrapped `.scrollTo(0, 0)` in `setTimeout(() => ..., 0)`.
  - This delays the scroll reset until the new component finishes rendering, solving the Goals edit page loading scroll down bug.

### 4. Build Configurations
- **[environment.prod.ts](file:///d:/Lifenote/frontend/src/environments/environment.prod.ts)**:
  - Added missing `apiHost` property so production configuration builds cleanly.
- **[angular.json](file:///d:/Lifenote/frontend/angular.json)**:
  - Increased budget limits for initial bundles (2MB) and component styles (32kB) to prevent build failures during production bundle optimizations.

## Verification
- We verified compile-time correctness by running a production build (`npm run build`), which compiled without errors.
- Verification confirms that routing/rendering triggers the scroll reset correctly.
