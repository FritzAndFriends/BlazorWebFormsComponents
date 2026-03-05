### LoginView AuthorizeView Redesign — Implementation Notes

**By:** Cyclops
**Date:** 2026-03-06

**What was implemented:**

Per Forge's proposal (`forge-loginview-authorizeview-redesign.md`), LoginView now delegates to `<AuthorizeView>` internally. Key decisions made during implementation:

1. **RoleGroup parameter alias pattern:** `ContentTemplate` and `ChildContent` are independent `[Parameter]` auto-properties per the project's alias convention. Coalescing happens in `LoginView.GetAuthenticatedView()`: `roleGroup.ContentTemplate ?? roleGroup.ChildContent`. This means `ContentTemplate` takes priority when both are set.

2. **Two-phase render confirmed working:** The Razor template renders `@RoleGroups` inside a `<CascadingValue>` first (Phase 1 — RoleGroup children self-register), then `<AuthorizeView>` renders (Phase 2 — uses the populated `RoleGroupCollection`). Blazor's top-to-bottom rendering order makes this work without explicit synchronization.

3. **No wrapper element:** LoginView now renders zero wrapper HTML, matching the original Web Forms `Control`-based `LoginView` that renders only the active template's content. CSS/JS targeting a `#LoginView1` wrapper div will break — this is intentional and correct.

4. **Breaking changes:**
   - `ChildContent` parameter removed from `LoginView` (was being misused as RoleGroup container)
   - `RoleGroups` parameter type changed from `RoleGroupCollection` to `RenderFragment`
   - Base class changed from `BaseStyledComponent` to `BaseWebFormsComponent` (no more `CssClass`, `Style`, `ToolTip` on LoginView)
   - Manual `AuthenticationStateProvider` injection removed

**Who needs to know:**
- **Rogue (Tests):** Existing bUnit tests use bare `ChildContent` on `RoleGroup` — these still work. Tests that checked for wrapper `<div>` markup will need updating. The `ShouldBeEmpty()` test may need adjustment since AuthorizeView might render differently than raw null RenderFragment.
- **Beast (Docs):** LoginView docs should be updated to show `<ContentTemplate>` syntax and note the AuthorizeView dependency.
- **Jubilee (Samples):** The sample page at `ControlSamples/LoginView/Index.razor` should still work but may need `CascadingAuthenticationState` in the app's auth setup.

**Build status:** Clean build, 0 errors.
