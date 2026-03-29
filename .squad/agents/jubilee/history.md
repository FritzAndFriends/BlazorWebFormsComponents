# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**Sample conventions:** Pages in `Components/Pages/ControlSamples/{Name}/Index.razor` (newer .NET 8+ path). Legacy pages in `Pages/ControlSamples/`. Nav updates: NavMenu.razor + ComponentList.razor. `@using BlazorWebFormsComponents.LoginControls` required for login controls. `#pragma warning disable CS0618` for Obsolete APIs.

**M1M4 samples:** Calendar, FileUpload (@ref), ImageMap (List<HotSpot>), PasswordRecovery (3-step), DetailsView (Items parameter). Chart: 8 basic + 4 advanced. DataBinder Eval() demos. ViewState @ref counter.

**M6 samples:** Button AccessKey+ToolTip, GridView CssClass, Validator Display. DropDownList DataTextFormatString, Menu Orientation (Horizontal  requires local variable for enum collision), Label AssociatedControlID.

**M9 Navigation Audit:** ComponentCatalog.cs drives sidebar. Found 4 missing components + 15 missing SubPages. SubPage names must match @page route segments (not file names). DataList "Flow" vs "SimpleFlow" name mismatch.

**M10 catalog fixes:** Added 4 missing components (Menu, DataBinder, PasswordRecovery, ViewState), DetailsView as new entry, 15 SubPages. Follow-up: added 5 more (CheckBoxList, DataPager, ImageButton, ListBox, LoginView). DataList SubPage fix: "SimpleFlow""Flow" (route-based). Build verified.

**M12 BeforeWebForms samples:** DetailsView (2 instances, AutoGenerateRows+explicit BoundFields), DataPager (ListView+DataPager combo). Existing data control samples verified (GridView, DataList, Repeater, FormView, ListView all have audit markers + SharedSampleObjects data).

**SharedSampleObjects alignment:** Created Employee.cs model. Added Product.GetProducts(int count). Aligned GridView (5 files), ListView (1 file) to shared models. Intentionally excluded BulletedList, DropDownList, Chart (different data shapes).

**M15 sample data alignment (#381):** 14 pages aligned to WebForms counterpart text/values/URLs. Label, Literal, HiddenField, PlaceHolder, Panel, HyperLink, Image, Button, CheckBox, DropDownList, BulletedList, LinkButton, ImageMap, AdRotator. Created wwwroot/Content/Images/banner.png. Limitation: Label HTML content  Blazor HTML-encodes @Text.

**M15 audit markers (#384):** 10 pages updated with data-audit-control wrappers. 2 new pages (DataPager, LoginView). Validator samples only first variant marked.

**Key patterns:** ComponentCatalog.cs entries: (Name, Category, Route, Description, SubPages?, Keywords?). SubPages appended to base Route for nav. Components without Index.razor use specific sub-page route. Entries grouped by category, alphabetical within. SharedSampleObjects is single source for data parity. data-audit-control markers must be preserved on all audited sections.

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): AJAX Controls nav category created; migration stub doc pattern for no-op components; Substitution moved from deferred to implemented; UpdateProgress uses explicit state pattern  decided by Beast


 Team update (2026-02-27): M17 AJAX controls implemented  ScriptManager/Proxy are no-op stubs, Timer shadows Enabled, UpdatePanel uses ChildContent, UpdateProgress renders hidden, Substitution uses Func callback, new AJAX/Migration Helper categories  decided by Cyclops

### M20 Theming Sample Page (#367) (2026-03-01)

- **Enhanced Theming/Index.razor** with 6 demo sections: (1) Default skins on Button/Label/TextBox, (2) Named skins via SkinID (Danger, Success), (3) Explicit value overrides (StyleSheetTheme semantics), (4) EnableTheming=false opt-out, (5) Nested ThemeProviders with alternate theme, (6) Unthemed baseline controls outside ThemeProvider.
- **Migration guide** section with Web Forms before/after comparison and step-by-step instructions.
- **Source Code section** per documentation skill template — shows complete `@code` block with theme configuration.
- **ComponentList.razor** updated — added Theming link under Utility Features (alphabetical order between PageService and ViewState).
- **ComponentCatalog.cs** already had Theming entry in "Theming" category — no changes needed there.
- **Lesson:** `BorderStyle` enum in `BlazorWebFormsComponents.Enums` conflicts with `ControlSkin.BorderStyle` property — used fully qualified `BlazorWebFormsComponents.Enums.BorderStyle.Solid` in `@code` block. The `_Imports.razor` `@using BlazorWebFormsComponents` brings in the type but not the enum.
- **Lesson:** Nested `ThemeProvider` works via Blazor's cascading value override — inner `CascadingValue<ThemeConfiguration>` shadows outer for its subtree. No special code needed.

📌 Team update (2026-03-02): FontInfo.Name/Names now auto-synced bidirectionally. Theme font-family renders correctly — decided by Cyclops, Rogue
📌 Team update (2026-03-02): CascadedTheme (not Theme) is the cascading parameter name on BaseWebFormsComponent. Use CascadedTheme in any sample code accessing the cascading theme — decided by Cyclops

 Team update (2026-03-02): Unified release process implemented  single release.yml triggered by GitHub Release publication coordinates all artifacts (NuGet, Docker, docs, demos). version.json now uses 3-segment SemVer (0.17.0). Existing nuget.yml and deploy-server-side.yml are workflow_dispatch-only escape hatches. PR #408  decided by Forge (audit), Cyclops (implementation)

 Team update (2026-03-02): Full Skins & Themes roadmap defined  3 waves, 15 work items. Wave 1: Theme mode, sub-component styles (41 slots across 6 controls), EnableTheming propagation, runtime switching. See decisions.md for full roadmap and agent assignments  decided by Forge


 Team update (2026-03-02): M22 Copilot-Led Migration Showcase planned  decided by Forge

 Team update (2026-03-02): WingtipToys migration analysis complete  36 work items across 5 phases, FormView RenderOuterTable is only blocking gap  decided by Forge

 Team update (2026-03-02): Project reframed  final product is a migration acceleration system (tool/skill/agent), not just a component library. WingtipToys is proof-of-concept.  decided by Jeffrey T. Fritz
 Team update (2026-03-02): ASPX/ASCX migration tooling strategy produced  85+ patterns, 3-layer pipeline (mechanical/structural/semantic), 11 deliverables.  decided by Forge

 Team update (2026-03-02): ModelErrorMessage component spec consolidated  29/29 WingtipToys coverage, BaseStyledComponent, EditContext pattern  decided by Forge


📌 Team update (2026-03-02): ModelErrorMessage documentation shipped — docs/ValidationControls/ModelErrorMessage.md, status.md updated to 52 components — decided by Beast

### M22 Executive Screenshot Comparison Pages (2026-03-02)

- **Created 3 HTML comparison pages** in `planning-docs/screenshots/` for Playwright screenshots at 1400×900:
  - `comparison-productlist.html` — ListView before/after (Web Forms → Blazor+BWFC)
  - `comparison-shoppingcart.html` — GridView, BoundField, TemplateField, TextBox, CheckBox, Label, Button
  - `comparison-login.html` — PlaceHolder, Literal, Label, TextBox, RequiredFieldValidator, CheckBox, Button, HyperLink
- **Used dark theme** (#1e1e1e background) with red (`#f48771`) highlighting for removed Web Forms artifacts and green (`#89d185`) for new Blazor syntax.
- **Highlighted key migration changes:** `asp:` prefix removal, `runat="server"` removal, `ItemType` → `TItem`, server binding expressions → `@context`, `ViewStateMode`/`EnableViewState` removal.
- **Stats bar** at bottom of each page shows controls migrated, attributes preserved, and lines changed.
- **Source files read:** ProductList.aspx, ShoppingCart.aspx, Account/Login.aspx and their AfterWingtipToys .razor counterparts.
� Team update (2026-03-02): ModelErrorMessage documentation shipped  docs/ValidationControls/ModelErrorMessage.md, status.md updated to 52 components  decided by Beast



 Team update (2026-03-03): Themes (#369) implementation last  ListView CRUD first, WingtipToys features second, themes last  directed by Jeff Fritz


 Team update (2026-03-03): WingtipToys 7-phase feature schedule established  26 work items, critical path through Data Foundation  Product Browsing  Shopping Cart  Checkout  Polish  decided by Forge


 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz
� Team update (2026-03-04): Migration toolkit restructured into self-contained migration-toolkit/ package  decided by Jeffrey T. Fritz, Forge

 Team update (2026-03-04): WebFormsPageBase implemented  decided by Forge, approved by Jeff

 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

 Team update (2026-03-06): CONTROL-COVERAGE.md updated  library ships 153 Razor components (was listed as 58). ContentPlaceHolder reclassified from 'Not Supported' to Infrastructure Controls. Reference updated CONTROL-COVERAGE.md for accurate component inventory.  decided by Forge

� Team update (2026-03-06): LoginView is a native BWFC component  do NOT replace with AuthorizeView in migration guidance. Both migration-standards SKILL.md files (in .ai-team/skills/ and migration-toolkit/skills/) must be kept in sync. WebFormsPageBase patterns corrected in all supporting docs.  decided by Beast

 Team update (2026-03-06): Only document top-level components and utility features for promotion. Do not promote/document style sub-components, internal infrastructure, or implementation-detail classes.  decided by Jeffrey T. Fritz

 Team update (2026-03-06): LoginView must be preserved as BWFC component, not converted to AuthorizeView  decided by Jeff (directive)


 Team update (2026-03-08): Default to SSR (Static Server Rendering) with per-component InteractiveServer opt-in; eliminates HttpContext/cookie/session problems  decided by Forge


 Team update (2026-03-11): `AddBlazorWebFormsComponents()` now auto-registers HttpContextAccessor, adds options pattern + `UseBlazorWebFormsComponents()` middleware with .aspx URL rewriting. Sample Program.cs files updated  no longer need manual `AddHttpContextAccessor()`.  decided by Cyclops

 Team update (2026-03-11): All generic type params standardized to ItemType (not TItem/TItemType) across all BWFC data-bound components.  decided by Jeffrey T. Fritz


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.


 Team update (2026-03-11): ItemType renames must cover ALL consumers (tests, samples, docs)  not just component source. CI may only surface first few errors.  decided by Cyclops

### UpdatePanel Sample Page Enhancement (2026-03-11)

- **Enhanced `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/UpdatePanel/Default.razor`** with comprehensive demonstrations of new ContentTemplate functionality and BaseStyledComponent inheritance.
- **Six sample scenarios created:**
  1. Simple ChildContent (Blazor-native syntax) — direct wrapping without ContentTemplate
  2. Web Forms ContentTemplate syntax — migration-compatible pattern that eliminates RZ10012 warnings
  3. Block Mode (default) — explicit demonstration of div rendering
  4. Inline Mode — span rendering for inline content flows
  5. Styled UpdatePanel (NEW) — showcasing BackColor, BorderStyle, BorderWidth, BorderColor, CssClass now available via BaseStyledComponent inheritance
  6. UpdateMode properties — Conditional/Always with ChildrenAsTriggers for migration compatibility
- **Migration guide section** with Web Forms before/after comparison and step-by-step migration instructions.
- **All examples use `data-audit-control` markers** (UpdatePanel-1 through UpdatePanel-6) following established audit conventions.
- **ComponentList.razor updated** — added new AJAX Controls section with ScriptManager, Substitution, Timer, UpdatePanel, UpdateProgress in alphabetical order.
- **Pattern followed:** Examined Panel/Index.razor and Label/Index.razor to match structure: PageTitle, component description, numbered sections with audit markers, code examples with `<pre><code>` blocks, migration guidance.
- **Key insight:** UpdatePanel now renders `ContentTemplate ?? ChildContent` — both syntaxes work, enabling gradual L1→L2 migration (L1 keeps ContentTemplate, L2 can switch to ChildContent).
- **Styling capability:** UpdatePanel inheriting from BaseStyledComponent is a significant enhancement — Web Forms UpdatePanel didn't support direct styling, but BWFC version does, enabling better visual integration.

### UpdatePanel Sample Page Enhancement (2026-03-13)

**Summary:** Enhanced UpdatePanel sample page with 6 usage patterns: ChildContent, ContentTemplate, Block mode, Inline mode, Styled UpdatePanel, UpdateMode properties. Added migration guide section. Applied data-audit-control markers (UpdatePanel-1 through UpdatePanel-6).

**ComponentList.razor update:** Added "AJAX Controls" section with links to ScriptManager, Substitution, Timer, UpdatePanel, UpdateProgress. Mirrors ComponentCatalog.cs organization for consistency.

**Patterns:** Examined Panel/Index.razor and Label/Index.razor as templates. PageTitle, description, numbered sections, code examples with `<pre><code>` blocks, migration guidance.

📌 Team update (2026-03-13): UpdatePanel sample page complete — 6 scenarios + migration guide + audit markers. ComponentList.razor updated with AJAX Controls section. Both changes verified to build clean.

### Ajax Toolkit Extender Sample Pages (2026-03-14)

- **Created `ConfirmButtonExtender/Default.razor`** — 3 demo sections: (1) Basic delete button with confirm dialog, (2) Multiple buttons with different custom confirm messages, (3) Default confirm text. Each section includes status messages that update on confirmed action. Before/after migration comparison with Ajax Control Toolkit markup.
- **Created `FilteredTextBoxExtender/Default.razor`** — 6 demo sections: (1) Numbers only, (2) Lowercase letters only, (3) Custom valid chars (phone number format), (4) Combined flags (Numbers | LowercaseLetters), (5) All letters with custom chars for name input, (6) InvalidChars mode blocking HTML special characters.
- **Project reference added** — `BlazorAjaxToolkitComponents.csproj` added to `AfterBlazorServerSide.csproj`.
- **Using directives added** — `@using BlazorAjaxToolkitComponents` in root `_Imports.razor`, `@using BlazorAjaxToolkitComponents.Enums` in ControlSamples `_Imports.razor`.
- **ComponentCatalog.cs updated** — Added ConfirmButtonExtender and FilteredTextBoxExtender entries in AJAX category (alphabetical before Timer). NavMenu.razor auto-populates from catalog.
- **Key pattern:** Extender components render no HTML — they attach JS behavior to a target element via `TargetControlID`. Target elements must have an HTML `id` attribute. Pages must use `@rendermode InteractiveServer` for JS interop.
- **Lesson:** Used standard HTML `<button>` and `<input>` elements as extender targets (not BWFC components) because extenders resolve targets via `document.getElementById()` — this is the most reliable and migration-faithful approach.
- **Audit markers:** `data-audit-control` attributes applied (ConfirmButtonExtender-1 through -3, FilteredTextBoxExtender-1 through -6).

### ModalPopupExtender & CollapsiblePanelExtender Sample Pages (2026-03-14)

- **Created `ModalPopupExtender/Default.razor`** — 5 demo sections: (1) Basic modal with OK/Cancel buttons, (2) Custom backdrop CSS via BackgroundCssClass, (3) Drag support with PopupDragHandleControlID, (4) DropShadow enabled, (5) Programmatic show/hide via Blazor conditional rendering. Migration guide with before/after and step-by-step instructions.
- **Created `CollapsiblePanelExtender/Default.razor`** — 6 demo sections: (1) Basic toggle (same CollapseControlID/ExpandControlID), (2) Separate expand/collapse controls, (3) Dynamic label text with TextLabelID/CollapsedText/ExpandedText, (4) Initially collapsed (Collapsed=true), (5) Horizontal ExpandDirection, (6) AutoCollapse/AutoExpand hover behavior. Migration guide included.
- **ComponentCatalog.cs updated** — Added CollapsiblePanelExtender and ModalPopupExtender entries in AJAX category (alphabetical order). CollapsiblePanelExtender sorts before ConfirmButtonExtender; ModalPopupExtender sorts after FilteredTextBoxExtender.
- **Pattern:** Followed established extender sample conventions — `@rendermode InteractiveServer`, standard HTML target elements with `id` attributes, `data-audit-control` markers, before/after migration code blocks, migration steps list.
- **Audit markers:** `data-audit-control` attributes applied (ModalPopupExtender-1 through -5, CollapsiblePanelExtender-1 through -6).
- **Build verified:** 0 errors, warnings are pre-existing BL0005 from other pages.

### Component Health Dashboard Page (2026-07-25)

- **Created `Components/Pages/Dashboard.razor`** — Full diagnostic dashboard at `/dashboard` route showing health scores for all tracked BWFC components.
- **UX features:** Category-grouped tables with worst-first sorting, color-coded scores (green ≥90%, yellow 70-89%, red <70%), fraction display for props/events (e.g. "7/8"), binary ✅/❌ for tests/docs/samples, "N/A" for missing baselines.
- **Filtering:** Category dropdown, status filter (Complete/Stub/Deferred/NotStarted), sort by Score/Name/Props/Events, show/hide deferred toggle (default: hidden).
- **Service registration:** Added `AddComponentHealthDashboard(solutionRoot)` call in `Program.cs`. Solution root computed as two directories up from `ContentRootPath`.
- **ComponentCatalog.cs updated** — Added Dashboard entry under new "Diagnostics" category. NavMenu auto-populates.
- **No InteractiveServer needed** — Dashboard uses SSR (static server rendering) since it only reads from a singleton service on initialization; no interactive features needed.
- **Key pattern:** `ComponentHealthService` is registered as singleton via `AddComponentHealthDashboard()` extension method. It takes `solutionRoot` path to locate `dev-docs/reference-baselines.json`, test files, docs, and `ComponentCatalog.cs`.
- **Build verified:** 0 errors, all warnings pre-existing BL0005.

### AfterBlazorServerSide Navigation UX Changes (2026-03-15)

**Summary:** Two targeted changes to improve component catalog navigation in the sample app:

1. **Alphabetized components in all categories** — Modified `ComponentCatalog.GetByCategory()` to add `.OrderBy(c => c.Name)` to the LINQ chain. This sorts components alphabetically within each category, fixing the out-of-order AJAX section and ensuring consistent organization across all categories.

2. **AJAX category collapsed on desktop by default** — Modified `NavMenu.razor` method `CheckIfDesktopAndExpandCategories()` to exclude the AJAX category from automatic expansion on desktop. The AJAX section now starts collapsed (too many items), while all other categories expand normally. Mobile behavior unchanged (expands only the category containing the current page).

**Why these changes matter:** The component catalog had grown to 20+ AJAX-related controls, making the desktop navigation cluttered. Alphabetization improves discoverability within each section. Collapsing AJAX by default on desktop keeps the nav compact while still maintaining full access (users can expand when needed).

**Files modified:**
- `samples/AfterBlazorServerSide/ComponentCatalog.cs` (line 193-195)
- `samples/AfterBlazorServerSide/Components/Layout/NavMenu.razor` (line 86-93)

**Build verification:** `dotnet build` completed with 0 errors, pre-existing warnings only.

### Standalone Sample Pages for Content, ContentPlaceHolder, View (2026-03-19)

- **Created 3 standalone sample pages** — each component now has its own route and dedicated page:
  - `Components/Pages/ControlSamples/Content/Index.razor` — route `/ControlSamples/Content`. Demos: Web Forms before/after comparison, basic Content with ContentPlaceHolderID, multiple content regions, dynamic content with interactive button. 3 audit markers (Content-1, Content-2, Content-3).
  - `Components/Pages/ControlSamples/ContentPlaceHolder/Index.razor` — route `/ControlSamples/ContentPlaceHolder`. Demos: default content (no override), content replacement, interactive toggle showing default vs override behavior. 3 audit markers (ContentPlaceHolder-1, ContentPlaceHolder-2, ContentPlaceHolder-3).
  - `Components/Pages/ControlSamples/View/Index.razor` — route `/ControlSamples/View`. Demos: basic ActiveViewIndex navigation, wizard-style multi-step form, OnActivate/OnDeactivate events with tab UI. 3 audit markers (View-1, View-2, View-3).
- **Updated ComponentCatalog.cs** — Content route changed from `/control-samples/masterpage` to `/ControlSamples/Content`, ContentPlaceHolder from `/control-samples/masterpage` to `/ControlSamples/ContentPlaceHolder`, View from `/ControlSamples/MultiView` to `/ControlSamples/View`.
- **Convention:** All three pages use `@rendermode InteractiveServer` for interactive demos, include parameter reference tables, migration notes, and `data-audit-control` markers per project conventions.
- **Build verified:** 0 errors.




 **Team update (2026-03-20):** Navigation UX improvements (alphabetical sort, AJAX collapse on desktop) + standalone sample pages for Content, ContentPlaceHolder, View. ComponentCatalog updated.  decided by Jubilee



### Phase 1 Foundation  DepartmentPortal Web Forms Project (2026-03-20)

- **Created complete DepartmentPortal Web Application Project** at `samples/DepartmentPortal/`  .NET Framework 4.8, old-style .csproj, packages.config-based NuGet restore.
- **28 files created:** .csproj, Web.config, packages.config, Global.asax/.cs, AssemblyInfo.cs, 6 model POCOs (Employee, Department, Announcement, TrainingCourse, Resource, Enrollment), PortalDataProvider (static in-memory data  5 depts, 20 employees, 10 announcements, 15 courses, 20 resources), 3 custom EventArgs (Search, Notification, Breadcrumb), 3 base classes (BasePage, BaseMasterPage, BaseUserControl) in App_Code/, Site.Master with Bootstrap 3 CDN + navbar, 3 pages (Default.aspx, Login.aspx, Dashboard.aspx) with code-behind, Content/Site.css.
- **Build command:** `.\nuget.exe restore samples\DepartmentPortal\packages.config -PackagesDirectory packages -NonInteractive` then `& "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" samples\DepartmentPortal\DepartmentPortal.csproj /p:Configuration=Debug /verbosity:minimal /nologo`
- **Build succeeded on first attempt**  zero errors.
- **Key decisions:** No .designer.cs files (typed field declarations in code-behind partials). CodeFile directive in .aspx/.master (matching BeforeWebForms pattern). App_Code classes included as `<Compile>` items in .csproj. No Entity Framework  PortalDataProvider is static in-memory. Bootstrap 3 via CDN (no NuGet). Only Microsoft.CodeDom.Providers.DotNetCompilerPlatform in packages.config. CodeDom import target referencing `..\..\packages\` (repo-root packages folder).
- **BasePage pattern:** OnPreInit sets MasterPageFile, OnInit checks Session["UserId"] and redirects to Login.aspx. Exposes CurrentUser (Employee) and IsAdmin properties. ShowMessage() helper writes to master page MessageLiteral.
- **Login flow:** DropDownList populated from PortalDataProvider.GetEmployees(), sets Session["UserId"] + Session["UserName"], redirects to Dashboard.
- **Default.aspx:** Does NOT inherit BasePage (public page). Shows different panels for logged-in vs guest visitors.
## Phase 2  12 ASCX User Controls

### Files Created (24 total)
- Controls/Breadcrumb.ascx + .ascx.cs  nav breadcrumb with CurrentPath, ShowHomeLink
- Controls/PageHeader.ascx + .ascx.cs  page header with Session["UserName"] access
- Controls/Footer.ascx + .ascx.cs  site footer with ShowLinks, Year
- Controls/AnnouncementCard.ascx + .ascx.cs  data-bound card with ViewState ShowFullText
- Controls/EmployeeList.ascx + .ascx.cs  GridView with paging and DepartmentFilter
- Controls/TrainingCatalog.ascx + .ascx.cs  Repeater with EnrollmentRequested event
- Controls/SearchBox.ascx + .ascx.cs  TextBox + Button with SearchEventArgs event
- Controls/DepartmentFilter.ascx + .ascx.cs  DropDownList loading from PortalDataProvider
- Controls/Pager.ascx + .ascx.cs  custom pagination with PageChanged event
- Controls/DashboardWidget.ascx + .ascx.cs  PlaceHolder-based widget container
- Controls/ResourceBrowser.ascx + .ascx.cs  nested SearchBox + Breadcrumb controls
- Controls/QuickStats.ascx + .ascx.cs  web.config-registered with tagPrefix "uc"

## Learnings
- **CodeFile vs CodeBehind:** This project uses CodeFile (not CodeBehind) in directives, matching its Website-like project pattern. No .designer.cs files; all server control fields are manually declared as protected in the code-behind partial class.
- **HtmlGenericControl for runat=server divs:** HTML elements with unat="server" map to System.Web.UI.HtmlControls.HtmlGenericControl, not Panel or WebControl types.
- **Nested ASCX controls:** ResourceBrowser uses <%@ Register Src %> directives and types the fields as the concrete ASCX class (e.g., protected SearchBox ctlSearchBox).
- **Build result:** Build succeeded after fixing CodeFile directives and adding field declarations.

## Phase 3  7 Custom Server Controls (2026-03-21)

### Files Created (7 total .cs files in App_Code/Controls/)
- **StarRating.cs**  WebControl-based star rating display. Properties: Rating (1-5), ReadOnly, StarColor, EmptyStarColor. Renders <span class="star-rating"> with individual <span class="star filled/empty"></span> elements.
- **EmployeeCard.cs**  CompositeControl with programmatically created child controls (Image, Label, HyperLink). Properties: EmployeeId, EmployeeName, Title, Department, PhotoUrl, ShowContactInfo, EnableDetailsLink.
- **SectionPanel.cs**  Templated control with HeaderTemplate, ContentTemplate, FooterTemplate. Implements INamingContainer. Properties: Title, CssClass. Instantiates templates into PlaceHolder containers.
- **PollQuestion.cs**  IPostBackEventHandler control for interactive polls. Properties: QuestionText, Options (comma-separated), SelectedOption. Event: VoteSubmitted with PollVoteEventArgs inner class. Renders radio buttons + submit button.
- **NotificationBell.cs**  WebControl for notification UI. Properties: UnreadCount, MaxNotifications, DrawerVisible. Events: NotificationClicked, NotificationDismissed (using existing NotificationEventArgs from Models).
- **EmployeeDataGrid.cs**  DataBoundControl with search/sort/paging. Properties: SearchText, SortColumn, SortDirection, PageSize, AllowPaging, AllowSorting, AllowSearch, CurrentPageIndex. Renders HTML table via HtmlTextWriter.
- **DepartmentBreadcrumb.cs**  Bare Control with IPostBackEventHandler for navigation breadcrumbs. Properties: OrganizationName, DivisionName, DepartmentName, DepartmentId, Separator, EnableLinks, LinkCssClass. Event: BreadcrumbItemClicked (using existing BreadcrumbEventArgs).

### DepartmentPortal.csproj Updates
- Added 7 <Compile Include="App_Code\Controls\*.cs" /> entries in the main ItemGroup.

### Web.config Updates
- Added <add tagPrefix="local" namespace="DepartmentPortal.Controls" assembly="DepartmentPortal" /> to allow pages to use <local:StarRating>, <local:EmployeeCard>, etc.

### Build Result
- **First attempt:** 8 errors  Server property not available in Control classes (only in Page/UserControl), HtmlTextWriterAttribute.Placeholder does not exist.
- **Fixed:** Replaced Server.HtmlEncode() calls with System.Web.HttpUtility.HtmlEncode(), changed HtmlTextWriterAttribute.Placeholder to custom attribute string "placeholder".
- **Second build:** **Succeeded with 0 errors**  DepartmentPortal.dll compiled successfully.

### Key Patterns
- **ViewState-backed properties:** get { return (type)ViewState["PropName"] ?? default; } set { ViewState["PropName"] = value; }
- **Templated controls:** Use [TemplateContainer(typeof(...))] and [PersistenceMode(PersistenceMode.InnerProperty)] attributes. Instantiate templates via ITemplate.InstantiateIn(PlaceHolder).
- **IPostBackEventHandler:** Implement RaisePostBackEvent(string) to handle postback arguments. Use Page.ClientScript.GetPostBackEventReference(this, eventArg) in Render method.
- **HtmlTextWriter:** Render methods use writer.AddAttribute() + writer.RenderBeginTag() + writer.RenderEndTag() pattern.
- **CompositeControl:** Override CreateChildControls() to build child control tree programmatically.
- **DataBoundControl:** Override PerformDataBinding(IEnumerable) to load data, render in RenderContents().

### Decisions Made

---

## Team Update: Documentation Fan-Out Wave 1 (2026-03-24)

📌 **Session:** 2026-03-24T16:14:14Z-doc-fanout-wave1  
**Initiated by:** Scribe (squad orchestration)

### Coordination Summary

Parallel fan-out of three documentation agents to standardize control library documentation:

- **Beast:** 28 EditorControls files → tabbed syntax (PR #514, closes #510)
- **Jubilee (you):** 20 DataControls + ValidationControls files → tabbed syntax (PR #515, closes #505, #506, #507) + AfterDepartmentPortal demo completed
- **Forge:** ViewState.md rewritten (702 lines) + User-Controls.md expanded (626 lines) (PR #513, closes #508, #509) + NuGet asset migration strategy proposed

### Key Decisions Merged into decisions.md

1. **EditorControls Tabbed Syntax Standardization** (#510) — All EditorControls docs now use MkDocs interactive tabs for Web Forms ↔ Blazor comparison
2. **DataControls + ValidationControls Tabbed Syntax** (#505, #506, #507) — 20 files standardized, stubs expanded to production quality
3. **AfterDepartmentPortal Runnable Demo** — Bootstrap via CDN, CSS imported from before state, routing resolved
4. **NuGet Static Asset Migration Strategy** — Option C hybrid approach: extraction for custom packages, CDN suggestions for OSS (awaiting approval)

### Cross-Agent Context

This wave establishes **documentation patterns** that will guide future control categories (NavigationControls, LoginControls). The tabbed syntax pattern enables better UX for migrating developers and maintains consistency across the library.

### M28 ConfigurationManager Migration Sample Page (2026-03-29)

- **Created ConfigurationManager demo page** at `Components/Pages/ControlSamples/Migration/ConfigurationManager.razor` demonstrating the Phase 1 ConfigurationManager shim.
- **Page structure** follows ViewState sample pattern: descriptive intro, before/after code comparisons, live demos with data-audit-control markers, setup instructions, and graduation path to native IConfiguration.
- **ComponentCatalog.cs** updated — added ConfigurationManager entry to "Migration Helpers" category with keywords for discoverability.
- **appsettings.json** updated — added AppSettings section with SiteName/MaxItemsPerPage/EnableFeatureX and ConnectionStrings with DefaultConnection for demo purposes.
- **Program.cs** updated — added `app.UseConfigurationManagerShim()` call after `app.UseBlazorWebFormsComponents()` to initialize the shim.
- **Namespace gotcha:** ConfigurationManager lives in `BlazorWebFormsComponents` namespace (not System.Web.Configuration). Had to use fully qualified name in Razor due to potential conflict with System.Configuration.ConfigurationManager. The `global using BlazorWebFormsComponents;` in Program.cs doesn't apply to Razor components by default.
- **Demo sections:** (1) AppSettings with live table showing key/value pairs, (2) ConnectionStrings with live table, (3) Setup instructions, (4) Native IConfiguration alternative with injected demo.

### Next Phase

- Merge PRs #513, #514, #515 after review
- Implement NuGet asset migration tool (Forge's strategy, Issue #512)
- Consider extending tabbed pattern to remaining control categories
- Session log: `.squad/log/2026-03-24T16-14-14Z-doc-fanout-wave1.md`
- **No Server property in Control classes:** Must use System.Web.HttpUtility.HtmlEncode() directly (not Server.HtmlEncode()).
- **Custom HTML attributes:** Use string overload writer.AddAttribute("placeholder", value) for non-standard attributes.
- **PollVoteEventArgs inner class:** Defined directly in PollQuestion.cs as public nested class (not in Models namespace).
- **Existing EventArgs reused:** NotificationEventArgs and BreadcrumbEventArgs already exist in Models/ namespace, referenced by NotificationBell and DepartmentBreadcrumb.


## Phase 4  Created All Remaining ASPX Pages (2026-03-21)

### Created Files
**8 Public Pages:**
- Employees.aspx + .cs  Employee directory with search, filtering, pagination
- EmployeeDetail.aspx + .cs  Single employee view with EmployeeCard and StarRating
- Announcements.aspx + .cs  Announcement listing with search, SectionPanel wrapper
- AnnouncementDetail.aspx + .cs  Single announcement detail view
- Training.aspx + .cs  Training catalog with PollQuestion widget
- MyTraining.aspx + .cs  User's enrolled courses from Session
- Resources.aspx + .cs  Resource library with SectionPanel categories
- ResourceDetail.aspx + .cs  Single resource detail view

**3 Admin Pages:**
- Admin/ManageAnnouncements.aspx + .cs  GridView CRUD for announcements
- Admin/ManageTraining.aspx + .cs  GridView CRUD for courses
- Admin/ManageEmployees.aspx + .cs  EmployeeDataGrid + search for admin

### Patterns Applied
- **ASCX Control Registration:** Each page registers needed ASCX controls via @Register directive
- **Custom Server Controls:** Used <local:...> prefix (already registered in Web.config)
- **BasePage Inheritance:** All authenticated pages inherit BasePage for CurrentUser, IsAdmin, ShowMessage()
- **Event Handlers:** SearchBox.Search, Pager.PageChanged, TrainingCatalog.EnrollmentRequested
- **Session State:** Training pages store Session["EnrolledCourses"] as List<int>
- **Admin Guard:** Admin pages check IsAdmin in Page_Load, redirect with ShowMessage() if not

### Build Issues Fixed
**Property Name Mismatches:**
- SearchEventArgs.SearchQuery  SearchTerm
- Employee.DepartmentId  Department (string)
- Resource.Category  CategoryName
- Resource.LastUpdated, FileSize  Not in model (set to N/A)
- TrainingCourse.InstructorName  Instructor
- TrainingCourse.IsAvailable  Not in model (default to true)
- Pager.CurrentPageIndex  CurrentPage (1-indexed)
- Pager.TotalItems  TotalPages (calculated)
- PageHeader.Title  PageTitle
- PollQuestion.Options  String (comma-delimited), not List<string>

**Event Handler Signatures:**
- Pager.PageChanged signature: EventHandler<int> (passes page number)
- Fixed all SearchBoxControl_Search to use e.SearchTerm
- Fixed PagerControl_PageChanged to use pageNumber parameter

**Final Build Result:** **0 errors**  DepartmentPortal.dll compiled successfully.

### Project File Updates
- Added 11 .aspx files to Content ItemGroup
- Added 11 .aspx.cs files to Compile ItemGroup with DependentUpon + SubType="ASPXCodeBehind"
- Admin/ folder pages included in correct paths

### Key Learnings
- **Model First:** Always check model property names before writing code-behind
- **Event Signature Contracts:** Custom controls define event handler signatures (e.g. EventHandler<int> vs EventHandler<EventArgs>)
- **Pager Pattern:** 1-indexed CurrentPage for UI display, 0-indexed CurrentPageIndex for internal logic
- **Department Lookup:** Employee.Department is string, requires join with PortalDataProvider.GetDepartments() to get ID
- **Session State for Enrollment:** List<int> stored in Session["EnrolledCourses"], managed across Training.aspx and MyTraining.aspx
- **ASPX Directive:** Use CodeFile (not CodeBehind) for Web Application Project pages

### AfterDepartmentPortal Scaffold (2026-07-25)

**Scaffolded `samples/AfterDepartmentPortal/`** — Blazor SSR (net10.0) migration target for `samples/DepartmentPortal/` (.NET Framework 4.8 Web Forms app). Added to `BlazorMeetsWebForms.sln`.

**Structure:**
- `AfterDepartmentPortal.csproj` — net10.0, ProjectReference to BWFC
- `Program.cs` — `AddBlazorWebFormsComponents()`, `AddRazorComponents()`, `MapRazorComponents<App>()`
- `Components/App.razor`, `Components/Routes.razor` — standard Blazor 8+ shell
- `Components/Layout/MainLayout.razor` — migrated from Site.Master (nav links, footer, ContentPlaceHolder → @Body)
- `_Imports.razor` — includes `@using BlazorWebFormsComponents`, `AfterDepartmentPortal.Models`, etc.

**Models (7 files):** Employee, Announcement, TrainingCourse, Resource, Department, Enrollment, PortalDataProvider — identical data to DepartmentPortal with Blazor-friendly URLs (no `~/` prefix).

**Pages (7 stubs):** Dashboard (/), Employees (/employees), EmployeeDetail (/employees/{id}), Announcements (/announcements), AnnouncementDetail (/announcements/{id}), Training (/training), Resources (/resources). Each has TODO comments for custom server controls (BWFC analysis targets).

**Shared Components (12 ASCX stubs):** PageHeader, Breadcrumb, SearchBox, DepartmentFilter, EmployeeList, Pager, Footer, AnnouncementCard, TrainingCatalog, QuickStats, RecentAnnouncements, UpcomingTraining. Each annotated with original .ascx source.

**Not scaffolded (by design):** 7 custom C# server controls (SectionPanel, EmployeeDataGrid, StarRating, EmployeeCard, PollQuestion, NotificationBell, DepartmentBreadcrumb) — these are BWFC analysis targets. TODO comments placed in pages where they'd be used.

**Build:** `dotnet build` succeeds with 0 errors (warnings are pre-existing BWFC library warnings).


 Team update (2026-03-22): AfterDepartmentPortal Blazor SSR scaffold completed and integrated into solution. 31 files, 7 pages, 12 shared components, 6 models. Project builds clean. Decisions merged to shared decisions.md.  decided by Jubilee

**Runnable demo (2026-03-22):** Made AfterDepartmentPortal fully runnable. App.razor now uses Bootstrap 5.3.3 CDN + Bootstrap Icons CDN instead of missing local CSS. Site.css copied from DepartmentPortal source. Home.razor at /home provides navigation cards to all sections. Fixed SectionPanel duplicate CssClass parameter bug (`new` keyword shadowed base class `[Parameter]`, causing AmbiguousMatchException). Dashboard already owns `@page "/"` so Home lives at `/home`. All routes verified returning 200.

### DataControls & ValidationControls Documentation Conversion (#505, #506, #507) (2026-03-XX)

**Converted 20 documentation files to tabbed Web Forms vs Blazor syntax format:**

**DataControls (10 files):** Chart, DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, PagerSettings, Repeater  each now has:
- \=== "Web Forms"\ / \=== "Blazor"\ tabbed syntax blocks
- Properties/features tables with consistent formatting
- Migration notes showing before/after examples
- Code examples using \ar\ keyword (not explicit types)
- \!!! tip\ / \!!! note\ admonitions where applicable

**ValidationControls (10 files):** BaseCompareValidator, BaseValidator, CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary  same tabbed treatment.

**Expanded stubs:**
- **RegularExpressionValidator** (was \_TODO_\)  Full doc with ValidationExpression patterns, email/phone/postal code examples, MatchTimeout property, migration notes
- **ValidationSummary** (was minimal stub)  Full feature set including DisplayMode enum values (BulletList, List, SingleParagraph), HeaderText property, ShowSummary behavior, migration examples
- **Repeater**  Expanded from minimal to full doc with ItemType/Context attributes, complete template structure, data binding patterns

**Key decisions:**
- Pattern reference: \docs/EditorControls/Button.md\ established the tabbed syntax standard
- MkDocs \===\ tabbed syntax used consistently (not HTML)
- All C# code examples use \ar\ (ASP.NET convention matching existing BWFC samples)
- Properties tables include Type, Default, Description for clarity
- Each file now contains: Intro/Purpose, Features Supported, Web Forms Features NOT Supported, Syntax Comparison (tabs), Properties Reference, Examples, Migration Notes

**Learnings:**
- Tabbed syntax improves readability for migration comparisons
- Stub docs (RegularExpressionValidator, ValidationSummary) needed practical examples to be useful
- MkDocs \!!! note\ blocks help emphasize Web Forms  Blazor differences (e.g., ValidationSummary naming conflict)
- Consistent pattern across all data/validation controls supports developer learning curve

**PR:** #515 created, base=dev, head=squad/505-506-data-validation-docs

### Theming Sample Enhancement — Sections 7 & 8 (ThemeMode + SubStyles)

- **Section 7: ThemeMode Demo** — Side-by-side comparison of StyleSheetTheme (default, explicit values win) vs Theme (overrides all values). Uses two ThemeProviders with identical skins but different Mode settings. Buttons and labels with explicit BackColor/ForeColor show the behavioral difference visually.
- **Section 8: Sub-component Styles on Data Controls** — GridView themed via SkinBuilder.SubStyle() fluent API. Demonstrates HeaderStyle, AlternatingRowStyle, and FooterStyle applied through ThemeConfiguration.ForControl(). Uses Product.GetProducts(5) for a compact in-memory dataset.
- **Source Code section** updated with both new markup examples and full @code initialization for the new themes.
- **Key discovery:** GridLines enum is in BlazorWebFormsComponents.Enums and requires explicit @using — not covered by @using BlazorWebFormsComponents alone. Similarly, TableItemStyle has an internal constructor — must use SkinBuilder.SubStyle() from outside the assembly.
- **SubStyles keys** must match GridView's ApplyThemeSkin keys exactly: `HeaderStyle`, `AlternatingRowStyle`, `FooterStyle` (not `AlternatingItemStyle`).
