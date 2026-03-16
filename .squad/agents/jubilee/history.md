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


