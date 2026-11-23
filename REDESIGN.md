# LifeDeck interface redesign

All changes are confined to this local copy. No Git commands, remote repository operations, deployment, dependency installation, or application-data migration were performed.

## Files changed

- `Layout/MainLayout.razor`: responsive shell, route breadcrumb, desktop sidebar collapse, mobile navigation, Escape dismissal, skip link and theme switch.
- `Layout/NavMenu.razor`: grouped navigation with local SVG icons and accessible names that remain available when collapsed.
- `Components/DeckIcon.razor` (new): reusable local SVG icon component.
- `_Imports.razor`: icon component namespace.
- `Pages/Home.razor`: real stored counts linked to modules, date, task/note/calendar shortcuts and backup access.
- All ten existing `Pages/*.razor.css` files: semantic surface, accent and feedback colors; shared action rules consolidated into the global stylesheet.
- `wwwroot/css/lifedeck.css`: light/dark tokens, shell, typography, controls, cards, focus states, dashboard and responsive layouts.
- `wwwroot/css/app.css`: theme-aware startup, validation and runtime-error styling.
- `wwwroot/js/theme.js` (new): system color preference and persisted explicit theme selection.
- `wwwroot/index.html`: local theme script loaded before the app starts.

## Behavior

The initial color theme follows the system unless an explicit choice has been saved. The theme button switches between light and dark. Only the new `lifedeck.ui.theme` preference is written by this work. Existing record keys, models, services, routes and CRUD handlers are unchanged.

Desktop navigation can collapse to icons. At 900px and below, a labelled navigation button opens the sidebar; selecting a route, pressing Escape or clicking the backdrop closes it. Dashboard grids use three columns on larger screens, two on mobile and one on narrow screens. Existing module-specific responsive forms, lists and calendars are retained. Focus indicators, readable semantic feedback colors and reduced-motion support apply throughout.

## Validation

- `dotnet build --no-restore`: succeeded with zero warnings and zero errors. The installed SDK prints a preview-SDK informational notice.
- Local development server: all eleven application routes return HTTP 200, as do the design stylesheet, theme script, manifest and service worker. This checks route fallback and asset delivery; it does not execute Blazor interactions.
- No models, services, calculator engine, backup JavaScript, manifest or service-worker source changed.

## Manual browser checks still required

- Inspect each module in light and dark at desktop, tablet and phone sizes, including expanded and collapsed navigation and long user content.
- Tab through navigation, forms, calendar and calculator; check focus visibility, skip link and mobile Escape dismissal.
- Using disposable test records, verify create/edit/delete, completion, search and filtering; reload to verify persistence.
- Verify calendar appointment editing, calculator keyboard input and financial totals.
- Verify weather location lookup and currency conversion against their external APIs.
- Export JSON, TXT and XLSX. Validate restore with a disposable browser profile so existing records are preserved.
- Verify installation and offline behavior in a browser supporting the existing PWA.
- Check system theme detection, saved preference after reload and reduced-motion behavior.

No browser interaction or screenshot-based visual verification was performed in this session.
