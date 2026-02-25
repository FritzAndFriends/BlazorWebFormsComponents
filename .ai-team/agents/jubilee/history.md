# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
<!-- ⚠ Summarized 2026-02-23 by Scribe — original entries covered 2026-02-10 through 2026-02-12 -->

### Summary: Milestones 1–3 Sample Pages (2026-02-10 through 2026-02-12)

Sprint 1: Calendar (improved existing), FileUpload (@ref pattern), ImageMap (List<HotSpot> parameter), PageService. Fixed PostedFileWrapper.SaveAs path traversal vulnerability. Sprint 3: DetailsView (Items parameter, inline data), PasswordRecovery (3-step flow, Sender casting). Nav ordering is semi-alphabetical. LoginControls need explicit `@using`.

### Summary: Milestone 4 Chart + Utility Samples (2026-02-12)

Chart: 8 basic + 4 advanced sample pages (DataBinding, MultiSeries, Styling, ChartAreas). Child components via CascadingValue. `SeriesChartType.Point` for scatter. Axis is POCO, not component. DataBinder: 3 Eval() signatures with Repeater. ViewState: @ref Panel with counter demo. Fixed NavMenu ordering and ComponentList entries. WebColor: use static fields, not FromName().

**Key patterns:** Samples in `Components/Pages/ControlSamples/{Name}/Index.razor`. Nav updates: NavMenu.razor + ComponentList.razor. `@using BlazorWebFormsComponents.LoginControls` required for login controls. `#pragma warning disable CS0618` for Obsolete APIs in demos.

### Milestone 6 — Sample Page Updates for Base Class Features (WI-03, WI-06, WI-09, WI-12)

- **Button AccessKey + ToolTip (WI-03, WI-06):** Added `AccessKey="b"` and `ToolTip="Click to submit"` to the existing Button demo in `Components/Pages/ControlSamples/Button/Index.razor`. Button already had `ToolTip` as a declared parameter rendering `title=` attribute. `AccessKey` goes through `AdditionalAttributes` capture — rendering depends on the component template including `accesskey` in its HTML output.
- **GridView CssClass (WI-09):** Added `CssClass="table table-striped"` to the GridView default sample. GridView inherits style properties from `BaseStyledComponent` via `DataBoundComponent<T>` → `BaseDataBoundComponent` → `BaseStyledComponent`, and `GridView.razor` already renders `class="@CssClass"` on its `<table>` element — so this works immediately.
- **Validator Display (WI-12):** Added `Display="ValidatorDisplay.Dynamic"` to the second `RequiredFieldValidator` in the RequiredFieldValidator sample. The `ValidatorDisplay` enum exists in `Enums/ValidatorDisplay.cs` with values `None`, `Static`, `Dynamic`. The attribute compiles via `AdditionalAttributes` capture on `BaseValidator` → `BaseStyledComponent` → `BaseWebFormsComponent`. Actual Display behavior (collapsing vs hidden vs none) depends on Cyclops implementing the `Display` parameter in `BaseValidator.razor.cs` and using it in the template.
- **Minimal change pattern:** For feature demos on existing samples, just add the new property to one existing component instance plus a brief explanatory note — no need for new sections or pages.

### P2 Feature Samples — DataTextFormatString, Menu Orientation, Label AssociatedControlID

- **DropDownList DataTextFormatString:** Added two new sections to the existing `Components/Pages/ControlSamples/DropDownList/Index.razor` — one showing `{0:C}` currency formatting with a `PricedProduct` model, and one showing `"Item: {0}"` prefix formatting. Both use data-bound items to demonstrate the feature realistically.
- **Menu Orientation (Horizontal):** Added a horizontal menu demo to the existing `Pages/ControlSamples/Menu/Index.razor`. The `Orientation` parameter requires a local variable in `@code` because the parameter name matches the enum type name — `BlazorWebFormsComponents.Enums.Orientation horizontal = BlazorWebFormsComponents.Enums.Orientation.Horizontal;` then `Orientation="@horizontal"` in markup. Added `@using BlazorWebFormsComponents.Enums` to the page.
- **Label AssociatedControlID:** Created a new sample page at `Components/Pages/ControlSamples/Label/Index.razor`. Demos basic Label (renders as `<span>`), styled Label, and the key feature: `AssociatedControlID` which renders as `<label for="...">` for accessibility. Associated two Labels with TextBox inputs. Requires `@using BlazorWebFormsComponents.Enums` for `TextBoxMode`.
- **ComponentCatalog updated:** Added Label entry under Editor category in `ComponentCatalog.cs`. Menu was not in ComponentCatalog (old `Pages/` path) — left as-is to avoid scope creep.

 Team update (2026-02-23): Login controls outer style properties consolidated  Login/ChangePassword/CreateUserWizard now inherit BaseStyledComponent  decided by Rogue, Cyclops
 Team update (2026-02-23): Label AssociatedControlID switches rendered element (label vs span)  decided by Cyclops
 Team update (2026-02-23): Milestone 6 Work Plan ratified  54 WIs across P0/P1/P2 tiers  decided by Forge
 Team update (2026-02-23): UI overhaul requested  Jubilee is frontend lead (UI-1,3,4,5,6,7,10)  decided by Jeffrey T. Fritz

 Team update (2026-02-24): Menu auto-ID pattern  Menu now auto-generates IDs for JS interop  decided by Cyclops
 Team update (2026-02-24): M8 scope excludes version bump to 1.0 and release  decided by Jeffrey T. Fritz
 Team update (2026-02-24): PagerSettings shared sub-component created  samples may need PagerSettings demos  decided by Cyclops

### M9 Navigation Audit (WI-12)

- Sidebar navigation is driven entirely by `ComponentCatalog.cs` — `NavMenu.razor` iterates over it with SubPages support.
- Found **4 components** completely missing from ComponentCatalog (Menu, DataBinder, PasswordRecovery, ViewState) — invisible in sidebar.
- Found **15 SubPage entries** missing across GridView (5), TreeView (2), FormView (3), DetailsView (2), ListView (1), DataGrid (1), Panel (1).
- All 10 M7/M8 feature pages exist on disk with valid `@page` directives but none appear in sidebar navigation.
- Some pages are partially reachable via in-page `Nav.razor` components, but TreeView Selection/ExpandCollapse and DetailsView Styles/Caption have no nav links at all.
- DataList has a SubPage name mismatch: catalog says "Flow" but file is `SimpleFlow.razor`.
- Report written to `.ai-team/decisions/inbox/jubilee-m9-nav-audit.md`.

� Team update (2026-02-25): ToolTip moved to BaseStyledComponent (28+ controls)  decided by Cyclops
 Team update (2026-02-25): M9 plan ratified  12 WIs, migration fidelity  decided by Forge
 Team update (2026-02-25): Nav audit merged  4 missing components + 15 missing SubPages in ComponentCatalog.cs  decided by Jubilee

 Team update (2026-02-25): Consolidated audit reports now use `planning-docs/AUDIT-REPORT-M{N}.md` pattern for all milestone audits  decided by Beast


 Team update (2026-02-25): M12 introduces Migration Analysis Tool PoC (`bwfc-migrate` CLI, regex-based ASPX parsing, 3-phase roadmap)  decided by Forge

### M10 — Fix 19 Unreachable Sample Pages in ComponentCatalog.cs (#350)

- **4 missing components added:** Menu (Navigation, route to Selection since no Index), DataBinder (Utility), PasswordRecovery (Login), ViewState (Utility).
- **DetailsView added as new component** (Data category) with SubPages: Caption, Styles — was completely absent from catalog despite having 3 pages on disk.
- **15 missing SubPages added to existing components:** GridView (+5: DisplayProperties, InlineEditing, Paging, Selection, Sorting), TreeView (+2: ExpandCollapse, Selection), FormView (+3: Edit, Events, Styles), ListView (+1: CrudOperations), DataGrid (+1: Styles), Panel (+1: BackImageUrl).
- **DataList SubPage name fix:** "Flow" → "SimpleFlow" to match actual file `SimpleFlow.razor`.
- **Pattern confirmed:** SubPages are alphabetically ordered in catalog arrays; components without an Index.razor use their specific page route (e.g., Menu → `/ControlSamples/Menu/Selection`).
- Build verified: `dotnet build samples/AfterBlazorServerSide/AfterBlazorServerSide.csproj --no-restore --verbosity quiet` passes.



 Team update (2026-02-25): Future milestone work should include a doc review pass to catch stale 'NOT Supported' entries  decided by Beast

 Team update (2026-02-25): Shared sub-components of sufficient complexity get their own doc page (e.g., PagerSettings)  decided by Beast

 Team update (2026-02-25): All login controls (Login, LoginView, ChangePassword, PasswordRecovery, CreateUserWizard) now inherit from BaseStyledComponent  decided by Cyclops

 Team update (2026-02-25): ListView now has full CRUD event parity (7 new events)  samples may need updating  decided by Cyclops
 Team update (2026-02-25): Menu styles use MenuItemStyle with IMenuStyleContainer  samples may need updating  decided by Cyclops
