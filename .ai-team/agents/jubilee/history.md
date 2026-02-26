# Project Context

- **Owner:** Jeffrey T. Fritz
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

 Team update (2026-02-25): All new work MUST use feature branches pushed to origin with PR to upstream/dev. Never commit directly to dev.  decided by Jeffrey T. Fritz


 Team update (2026-02-25): Theme core types (#364) use nullable properties for StyleSheetTheme semantics, case-insensitive keys, empty-string default skin key. ThemeProvider is infrastructure, not a WebForms control. GetSkin returns null for missing entries.  decided by Cyclops


 Team update (2026-02-25): SkinID defaults to empty string, EnableTheming defaults to true. [Obsolete] removed  these are now functional [Parameter] properties.  decided by Cyclops


 Team update (2026-02-25): ThemeConfiguration CascadingParameter wired into BaseStyledComponent (not BaseWebFormsComponent). ApplySkin runs in OnParametersSet with StyleSheetTheme semantics. Font properties checked individually.  decided by Cyclops

### M10 — Theming Migration Guide & Calendar BeforeWebForms Sample

- **Theming migration guide (#367):** Added "Migration Guide — Before & After" section to `Components/Pages/ControlSamples/Theming/Index.razor`. Shows three panels: Before (Web Forms `.skin` file + `web.config` + ASPX markup), Migration Steps (4-step numbered list), After (Blazor `ThemeProvider` + simplified markup). Uses `<pre><code>` blocks with proper HTML entity encoding (`&lt;`, `@@`). Placed after the existing live demo so the page flows from "see it work" to "how to migrate".
- **Calendar BeforeWebForms sample:** Created `samples/BeforeWebForms/ControlSamples/Calendar/default.aspx`, `default.aspx.cs`, and `default.aspx.designer.cs`. Demonstrates basic Calendar, 3 SelectionMode variants (Day, DayWeek, DayWeekMonth), styled Calendar with TitleStyle/DayHeaderStyle/SelectedDayStyle/TodayDayStyle/OtherMonthDayStyle/WeekendDayStyle/NextPrevStyle/SelectorStyle, custom navigation text, and event handlers (SelectionChanged, VisibleMonthChanged). Follows established BeforeWebForms patterns (MasterPageFile, namespace convention, designer file with auto-generated control declarations).


 Team update (2026-02-25): ThemesAndSkins.md documentation updated to match PoC implementation  class names, API, roadmap status, PoC decisions table added  decided by Beast

 Team update (2026-02-25): Calendar selection behavior review found 7 issues (1 P0: external SelectedDate sync, 4 P1: SelectWeekText default, SelectedDates sorting/mutability, style layering, 2 P2: test gaps, allocation)  decided by Forge


 Team update (2026-02-25): HTML audit strategy approved  decided by Forge

 Team update (2026-02-25): HTML audit milestones M11-M13 defined, existing M12M14, Skins/ThemesM15+  decided by Forge per Jeff's directive

### M12-01 — Tier 2 BeforeWebForms Data Control Samples

- **DetailsView sample created:** `samples/BeforeWebForms/ControlSamples/DetailsView/Default.aspx` + `.aspx.cs` + `.aspx.designer.cs`. Two DetailsView instances: one with AutoGenerateRows=true and NumericFirstLast paging, one with explicit BoundFields (Name, Price formatted as {0:C}, Category, InStock) and NextPreviousFirstLast paging. Both use inline `List<Product>` with 10 items. Includes PageIndexChanging handlers for paging support. Styled with HeaderStyle, AlternatingRowStyle, PagerStyle, FieldHeaderStyle. Wrapped with `data-audit-control="DetailsView"`.
- **DataPager sample created:** `samples/BeforeWebForms/ControlSamples/DataPager/Default.aspx` + `.aspx.cs` + `.aspx.designer.cs`. ListView+DataPager combination showing paged product data. DataPager uses PageSize=3, combines two NextPreviousPagerField instances (first/prev on left, next/last on right) with a NumericPagerField (ButtonCount=5) in the middle. Uses inline `List<Product>` with 10 items. PreRender rebinding for postback paging. Wrapped with `data-audit-control="DataPager"`.
- **Existing samples verified:** GridView (3 pages), DataList (4 pages), Repeater (1 page), FormView (1 page), ListView (3 pages) — all already have `data-audit-control` markers and use inline data via `SharedSampleObjects.Models.Widget`. No database dependencies found; no fixes needed.

 Team update (2026-02-26): NamingContainer inherits BaseWebFormsComponent, UseCtl00Prefix handled in ComponentIdGenerator  decided by Cyclops

 Team update (2026-02-26): Menu RenderingMode=Table uses inline Razor to avoid whitespace; AngleSharp foster-parenting workaround  decided by Cyclops

 Team update (2026-02-26): Login+Identity strategy: handler delegates in core, separate Identity NuGet package, redirect-based cookie flows  decided by Forge

 Team update (2026-02-26): Data control divergence: 4 sample rewrites needed for data controls before re-capture  decided by Forge

 Team update (2026-02-26): Post-fix capture confirms sample data alignment is P0 blocker  20+ divergences could become exact matches  decided by Rogue

### SharedSampleObjects Data Alignment Sweep

- **Audited all Blazor sample pages** in `Components/Pages/ControlSamples/` for inline data that should use SharedSampleObjects.
- **Priority directories already aligned:** FormView (4 pages), DataList (6 pages), Repeater (1 page) — all already use `Widget.SimpleWidgetList` or `Widget.Widgets(n)`. DetailsView (2 pages) already uses `Product.GetProducts()`.
- **New shared model created:** `SharedSampleObjects/Models/Employee.cs` with `Id`, `Name`, `Department` properties and static `GetEmployees()` method (4 employees).
- **New Product overload added:** `Product.GetProducts(int count)` generates n products with deterministic data for paging/sorting demos (consistent categories: Tools/Electronics/Hardware).
- **Files aligned to SharedSampleObjects:**
  - `GridView/InlineEditing.razor` — removed local `Product` class, now uses `Product.GetProducts()`
  - `GridView/Paging.razor` — removed local `Product` class + inline Enumerable.Range, now uses `Product.GetProducts(50)`
  - `GridView/Sorting.razor` — removed local `Product` class + 25-item GetProductName() helper, now uses `Product.GetProducts(25)`
  - `GridView/Selection.razor` — removed local `Product` class + 5 inline items, now uses `Product.GetProducts().Take(5).ToList()`
  - `GridView/DisplayProperties.razor` — removed local `Employee` class + 4 inline items, now uses `Employee.GetEmployees()`
  - `ListView/CrudOperations.razor` — replaced 3 inline Widget objects with `Widget.SimpleWidgetList.Take(3)` copy
- **Files intentionally NOT changed:** BulletedList (has `Product` with `Name`/`Url` — different shape), DropDownList/RadioButtonList (have `Product` with `string Id` — list control binding pattern), Chart pages (chart-specific records like `SalesData`/`TrafficData`), Validation pages (form-specific `ExampleModel` classes).
- Build verified: `dotnet build samples\AfterBlazorServerSide\ -c Release` succeeds with 0 errors.

 Team update (2026-02-26): WebFormsPage unified wrapper  inherits NamingContainer, adds Theme cascading, replaces separate wrappers  decided by Jeffrey T. Fritz, Forge
 Team update (2026-02-26): SharedSampleObjects is the single source for sample data parity between Blazor and WebForms  decided by Jeffrey T. Fritz
 Team update (2026-02-26): Login+Identity controls deferred to future milestone  do not schedule samples  decided by Jeffrey T. Fritz

### M15-01 — Sample Data Alignment for HTML Audit Matching (#381)

- **14 Blazor sample pages aligned** to use identical text, values, URLs, and attributes as their WebForms counterparts. All changes are data-only (no component source code changes).
- **Label:** "Hello World" (no comma/exclamation), "Styled Label" with `text-primary`/Blue/Bold, HTML content `<em>Emphasized</em>` variant.
- **Literal:** PassThrough mode text `"This is <b>literal</b> content."`, Encode mode text `"This is <b>encoded</b> content."`, simple text unchanged.
- **HiddenField:** Value changed from `"initial-secret-value"` to `"secret-value-123"`.
- **PlaceHolder:** Content paragraphs changed to `"This content was added programmatically."` / `"PlaceHolder renders no HTML of its own."`.
- **Panel:** Restructured all 3 audited variants — Panel-1 is now GroupingText="User Info" with Label+TextBox, Panel-2 is ScrollBars.Auto Height=100px with 4 paragraphs, Panel-3 is DefaultButton with TextBox+Button.
- **HyperLink:** All 4 variants aligned — styled (Blue/White), tooltip ("Navigate to Bing!"), Visible=false, basic — all using `https://bing.com` and text "Blue Button".
- **Image:** Changed to `/Content/Images/banner.png` src, "Banner image" and "Sized image" alt text, added Width/Height on Image-2.
- **Button:** Changed from "Click me!" to "Blue Button" with BackColor=Blue ForeColor=White.
- **CheckBox:** Labels changed to "Accept Terms", "Subscribe", "Enable Feature" (with AutoPostBack).
- **DropDownList:** DDL-3 changed to data-bound First/Second/Third Item, DDL-4 disabled "Cannot change", DDL-5 styled "Styled" with form-select, DDL-6 colored "Colored dropdown" with Navy/LightYellow/200px.
- **BulletedList:** BL-1 Disc with Apple/Banana/Cherry/Date, BL-2 Numbered with First/Second/Third, BL-3 Square HyperLink with Example Site/Example Org.
- **LinkButton:** LB-1 "Click Me" with btn-primary, LB-2 "Submit Form", LB-3 "Disabled Link" with Enabled=false.
- **ImageMap:** Changed to banner.png, "Navigate" alt, two rect hotspots (0,0,100,50 Bing + 100,0,200,50 GitHub).
- **AdRotator:** Updated Ads.xml from CSharp/VB images to banner.png with Visit Bing/Visit GitHub ads matching WebForms ads.xml.
- **Infrastructure:** Created `wwwroot/Content/Images/banner.png` for image path parity with WebForms `~/Content/Images/banner.png`.
- **Key pattern:** The `data-audit-control` markers were preserved on all audited sections. Non-audited demo sections below were kept but updated to reference valid types/data.
- **Limitation found:** Label-3 HTML content (`<em>Emphasized</em>`) — Blazor Label uses `@Text` which HTML-encodes, so the rendered output will differ from WebForms which renders raw HTML. This is a component-level issue requiring a component fix, not a sample data fix.

### M15-08 — Add Audit Markers to Blazor Samples (#384)

- **10 existing Blazor sample pages updated** with `data-audit-control` wrapper divs matching WebForms counterparts:
  - ChangePassword (`ChangePassword-1`), Chart (`Chart`), CreateUserWizard (`CreateUserWizard-1`)
  - Login (`Login-1`), LoginName (`LoginName-1`), LoginStatus (`LoginStatus-1`, `LoginStatus-2`)
  - MultiView (`MultiView-1`), PasswordRecovery (`PasswordRecovery-1`, `PasswordRecovery-2`), Table (`Table-3`)
- **2 new Blazor sample pages created** for controls that had WebForms samples but no Blazor equivalents:
  - `DataPager/Index.razor` with `data-audit-control="DataPager"` — uses inline product data with paging demo
  - `LoginView/Index.razor` with `data-audit-control="LoginView-1"` — shows AnonymousTemplate and LoggedInTemplate
- **Audit coverage:** All WebForms controls with `data-audit-control` markers now have corresponding Blazor markers. Validator samples only have their first variant marked (matching the single-demo pattern already established).
- **Key learning:** Blazor samples split across two paths — `Components/Pages/ControlSamples/` (current .NET 8 pattern) and legacy `Pages/ControlSamples/` (RadioButton, TextBox). New pages always go in `Components/Pages/`.
- **Build verified:** `dotnet build samples/AfterBlazorServerSide/AfterBlazorServerSide.csproj -c Release` — 0 errors, 0 warnings.

### Fix Unreachable Sample Pages in ComponentCatalog.cs (#350) — Follow-up

- **5 new component entries added** to ComponentCatalog.cs for pages that existed on disk but had no catalog entry:
  - **CheckBoxList** (Editor, `/ControlSamples/CheckBoxList`) — list of checkboxes for multiple selections
  - **DataPager** (Data, `/ControlSamples/DataPager`) — paging control for data-bound list controls
  - **ImageButton** (Editor, `/ControlSamples/ImageButton`) — clickable image that functions as a button
  - **ListBox** (Editor, `/ControlSamples/ListBox`) — scrollable list for single or multiple selection
  - **LoginView** (Login, `/ControlSamples/LoginView`) — template-based display for authenticated/anonymous users
- **DataList SubPage fix:** Changed "SimpleFlow" → "Flow" in SubPages array. The NavMenu generates SubPage URLs as `{baseRoute}/{SubPageName}`, so the SubPage name must match the @page route segment (`/ControlSamples/DataList/Flow`), not the file name (`SimpleFlow.razor`).
- **2 structural edge cases noted but not added:** `LoginControls/Orientation` (route `/ControlSamples/LoginControls/Orientation`) and `LoginStatusNotAuthenticated` (route `/ControlSamples/LoginStatusNotAuthenticated`) don't fit the SubPage URL pattern of any existing catalog entry. These pages are reachable via in-page links but not from sidebar navigation.
- **ComponentCatalog.cs pattern for future reference:** Each entry is a `ComponentInfo` record with (Name, Category, Route, Description, SubPages?, Keywords?). SubPages are string arrays where each name is appended to the base Route to form nav links (`{Route}/{SubPage}`). Entries are grouped by category comments and alphabetically ordered within each category. Components without an Index.razor use their specific sub-page route (e.g., Menu → `/ControlSamples/Menu/Selection`).
- **Build verified:** `dotnet build samples\AfterBlazorServerSide\AfterBlazorServerSide.csproj -c Release --verbosity quiet` — exit code 0.
