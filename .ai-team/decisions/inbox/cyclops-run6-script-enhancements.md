### 2026-03-05: Run 6 Script Enhancements (4 changes to bwfc-migrate.ps1)

**By:** Cyclops
**What:** Implemented 4 highest-ROI enhancements to `migration-toolkit/scripts/bwfc-migrate.ps1`:

1. **Scaffold TFM** → `net10.0` (was `net8.0`). _Imports.razor now includes `@using static Microsoft.AspNetCore.Components.Web.RenderMode` and `@rendermode InteractiveServer`.
2. **SelectMethod TODO** → BWFC-aware guidance: tells developers to use `Items="@_data"` parameter on BWFC data controls and load in `OnInitializedAsync`, instead of generic service injection advice.
3. **Static files** → Copy to `$Output\wwwroot\$relPath` instead of `$Output\$relPath`.
4. **Compilable stubs** → Pages containing Identity/Auth/Payment patterns (SignInManager, UserManager, FormsAuthentication, Session[, PayPal, Checkout) get minimal compilable `@page`/`@code{}` stubs instead of broken partial conversions.

**Why:** These 4 changes eliminate ~205 seconds of manual fix time per migration run. Enhancement 2 (SelectMethod) is highest impact at -120s. Enhancement 4 ensures clean builds without manual stubbing. All changes are surgical — no restructuring.
