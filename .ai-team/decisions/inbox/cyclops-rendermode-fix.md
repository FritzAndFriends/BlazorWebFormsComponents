### @rendermode InteractiveServer must NOT appear as standalone directive in _Imports.razor

**By:** Cyclops
**What:** Removed `@rendermode InteractiveServer` from the _Imports.razor scaffold in bwfc-migrate.ps1. Kept `@using static Microsoft.AspNetCore.Components.Web.RenderMode` (correct). App.razor already had the correct pattern with `@rendermode="InteractiveServer"` as a directive attribute on `<Routes>` and `<HeadOutlet>`.
**Why:** `@rendermode` is a directive attribute, not a standalone Razor directive. Placing it bare in _Imports.razor caused 8 build errors in Run 6 benchmarks. The correct pattern for global interactivity is to apply `@rendermode="InteractiveServer"` on component instances in App.razor. The `@using static` import enables the shorthand `InteractiveServer` without `RenderMode.` prefix.
