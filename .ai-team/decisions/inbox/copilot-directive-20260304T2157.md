### 2026-03-04: User directive
**By:** Jeffrey T. Fritz (via Copilot)
**What:** `@rendermode InteractiveServer` belongs in App.razor (to enable global server interactivity), NOT in _Imports.razor. Consult Microsoft Learn documentation for details.
**Why:** User request — captured for team memory. This corrects a script bug found in Run 6 where the scaffold placed @rendermode in _Imports.razor causing 8 build errors.
