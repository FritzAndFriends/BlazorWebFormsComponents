# Intentional Divergence Registry

**Created:** 2026-02-26
**Author:** Forge (Lead / Web Forms Reviewer)
**Status:** Updated through M18 — 14 entries (D-01 through D-14)
**Milestone:** M11-02

---

## Executive Summary

This document catalogs every **known intentional difference** between HTML rendered by ASP.NET Web Forms controls (.NET Framework 4.8, IIS) and the Blazor components in this library. These divergences exist by design — they are inherent to the architectural differences between Web Forms and Blazor, not bugs.

**Why this matters:** The HTML fidelity audit (M11–M13) will compare gold-standard Web Forms output against Blazor output. Without a pre-defined registry of intentional divergences, auditors would waste time investigating platform-level differences that can never (and should never) be replicated. This registry is the authoritative reference for classifying every audit finding as "intentional" or "bug."

**Scope:** All 51+ implemented Blazor components. Chart is documented here but excluded from the structural audit entirely (permanent divergence — different rendering technology).

---

## Divergence Summary Table

| # | Divergence | Category | Controls Affected | CSS Impact | JS Impact |
|---|-----------|----------|-------------------|------------|-----------|
| D-01 | ID Mangling | ID Mangling | ALL controls | None | Medium |
| D-02 | PostBack Link Mechanism | PostBack | GridView, Calendar, DetailsView, FormView, Menu, TreeView | None | High |
| D-03 | ViewState Hidden Fields | Infrastructure | Page-level (all pages) | None | None |
| D-04 | WebResource.axd URLs | Infrastructure | TreeView, Menu | None | Medium |
| D-05 | Chart Rendering Technology | Rendering Mode | Chart | Low | High |
| D-06 | Menu RenderingMode=Table | Rendering Mode | Menu | Medium | Low |
| D-07 | TreeView JavaScript | JS Interop | TreeView | None | High |
| D-08 | Calendar Day Selection | JS Interop | Calendar | None | High |
| D-09 | Login Control Infrastructure | Infrastructure | Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery | None | Low |
| D-10 | Validator Client-Side Scripts | JS Interop | RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator, ValidationSummary | None | High |
| D-11 | GUID-Based IDs | ID Generation | CheckBox, RadioButton, RadioButtonList, FileUpload | Medium | Medium |
| D-12 | Boolean Attribute Format | Attribute Format | DropDownList, ListBox, CheckBoxList, RadioButtonList | None | None |
| D-13 | Calendar Previous-Month Day Padding | Structural | Calendar | Low | None |
| D-14 | Calendar Style Property Pass-Through | Style | Calendar | Medium | None |

---

## Detailed Divergence Entries

### D-01: ID Mangling

| Field | Value |
|-------|-------|
| **Control** | ALL controls |
| **Divergence** | Web Forms generates mangled IDs using the naming container hierarchy (e.g., `ctl00_MainContent_ButtonName`, `ctl00$MainContent$GridView1`). Blazor uses developer-provided IDs directly (e.g., `ButtonName`). |
| **Category** | ID Mangling |
| **Reason** | Web Forms' `INamingContainer` interface generates unique IDs to prevent collisions across user controls, master pages, and repeating templates. Blazor's component model has no naming container concept — component isolation is handled by the framework differently. Replicating `ctl00_` prefixes would provide no value and would confuse developers. |
| **CSS Impact** | **None** — Well-written CSS uses class selectors, not ID selectors. Legacy CSS that targets `#ctl00_MainContent_ButtonName` will need updating, but this is a migration task, not a component bug. |
| **JS Impact** | **Medium** — JavaScript using `document.getElementById('ctl00_MainContent_...')` will break. However, this JavaScript was already tightly coupled to Web Forms infrastructure and must be rewritten for Blazor regardless. Developers should use `@ref` or standard DOM queries by class/data attribute. |

**Normalization rule:** The HTML normalization pipeline (M11-06) must strip `ctl00_`-style ID prefixes and `ctl00$`-style name prefixes before comparison. IDs should be compared by their developer-meaningful suffix only.

---

### D-02: PostBack Link Mechanism

| Field | Value |
|-------|-------|
| **Control** | GridView (paging, sorting), Calendar (day/week/month selection), DetailsView (paging), FormView (paging), Menu (item clicks), TreeView (node expand/collapse) |
| **Divergence** | Web Forms generates `<a href="javascript:__doPostBack('ctl00$MainContent$GridView1','Page$2')">` for interactive links. Blazor uses `@onclick` event handlers, rendering either `<a href="#" @onclick="...">` or `<button @onclick="...">` depending on the control. |
| **Category** | PostBack |
| **Reason** | `__doPostBack` is the core of the Web Forms event model — it submits the form back to the server with event target and argument. Blazor's SignalR-based (Server) or WASM-based (WebAssembly) event model replaces this entirely with `@onclick` handlers that invoke C# methods directly. There is no `<form>` postback in Blazor. |
| **CSS Impact** | **None** — The `<a>` tags remain `<a>` tags in most cases. Any CSS targeting `a` elements within the control still applies. |
| **JS Impact** | **High** — Any JavaScript that calls `__doPostBack()` directly, intercepts postback events, or parses `__doPostBack` arguments will not function. This is expected — the entire event model is replaced. |

**Normalization rule:** The normalization pipeline must replace `href="javascript:__doPostBack(...)"` with a placeholder (e.g., `href="[postback]"`) and compare only the structural HTML, not the event mechanism.

---

### D-03: ViewState Hidden Fields

| Field | Value |
|-------|-------|
| **Control** | Page-level infrastructure — affects all pages, not individual controls |
| **Divergence** | Web Forms injects `<input type="hidden" name="__VIEWSTATE" value="...">` and `<input type="hidden" name="__EVENTVALIDATION" value="...">` hidden fields into every page with a server-side `<form>`. These fields are large base64-encoded strings. Blazor has no equivalent. |
| **Category** | Infrastructure |
| **Reason** | ViewState is Web Forms' mechanism for persisting control state across postbacks. Blazor maintains state in memory (Server) or in the browser (WebAssembly) — there is no round-tripping of serialized state through hidden fields. Event validation (CSRF protection) is handled by Blazor's anti-forgery infrastructure differently. |
| **CSS Impact** | **None** — Hidden fields are not visible and do not affect layout. |
| **JS Impact** | **None** — While JavaScript can technically access `__VIEWSTATE`, no well-designed application relies on reading or modifying it. |

**Normalization rule:** The normalization pipeline must strip all `<input type="hidden">` elements with names `__VIEWSTATE`, `__VIEWSTATEGENERATOR`, `__EVENTVALIDATION`, and `__EVENTTARGET` / `__EVENTARGUMENT`.

---

### D-04: WebResource.axd URLs

| Field | Value |
|-------|-------|
| **Control** | TreeView, Menu (script and stylesheet includes) |
| **Divergence** | Web Forms uses dynamically-generated `WebResource.axd?d=...&t=...` URLs to serve embedded resources (JavaScript files, CSS files, images) from assemblies. Blazor uses static files served from `wwwroot/` or referenced via standard `<link>` / `<script>` tags. |
| **Category** | Infrastructure |
| **Reason** | `WebResource.axd` is an HTTP handler that extracts resources embedded in .NET assemblies at runtime. Blazor has no equivalent handler — static assets are served directly from the file system. Any JavaScript/CSS that Web Forms controls inject via `WebResource.axd` must be replaced with explicit static file references or component-level `<link>` / `<script>` injection in Blazor. |
| **CSS Impact** | **None** — The CSS content is the same; only the delivery mechanism differs. However, if the Web Forms control relies on specific CSS classes defined in embedded stylesheets, the Blazor component must either ship equivalent CSS or document the dependency. |
| **JS Impact** | **Medium** — JavaScript functionality delivered via `WebResource.axd` (e.g., TreeView expand/collapse, Menu popup behavior) is completely replaced by Blazor's interop model. This overlaps with D-07 (TreeView) and D-06 (Menu). |

**Normalization rule:** The normalization pipeline must strip all `<script src="WebResource.axd...">` and `<link href="WebResource.axd...">` elements, plus any inline `<script>` blocks that reference `WebResource`-delivered functions.

---

### D-05: Chart Rendering Technology

| Field | Value |
|-------|-------|
| **Control** | Chart |
| **Divergence** | Web Forms Chart renders as `<img src="ChartImg.axd?...">` — the chart is rendered server-side as a bitmap image by `System.Web.UI.DataVisualization.Charting` and served via an HTTP handler. Blazor Chart renders as `<canvas>` with Chart.js client-side rendering via JavaScript interop. |
| **Category** | Rendering Mode |
| **Reason** | This is a **permanent, architectural divergence**. Web Forms' server-side image generation (`ChartImg.axd`) has no Blazor equivalent. The team selected Chart.js (client-side canvas rendering) as the Blazor implementation strategy (decided M3). This provides better interactivity, responsive sizing, and eliminates server-side image generation overhead. The trade-off is that the HTML output is fundamentally different (`<canvas>` vs `<img>`). |
| **CSS Impact** | **Low** — CSS targeting `<img>` elements within the chart container will not match `<canvas>`. However, Chart.js handles its own sizing and the `<canvas>` element responds to container dimensions similarly. |
| **JS Impact** | **High** — Any JavaScript that reads the chart `<img>` `src` attribute or manipulates the image will not work. Chart.js has its own API for programmatic interaction. |

**Audit status:** ⛔ **Excluded from structural HTML audit.** The rendering technology difference is too fundamental for meaningful HTML comparison. Chart is documented here for completeness but will not appear in the Tier 1–3 audit reports. Any Chart-specific audit would compare Chart.js configuration fidelity (data binding, series types, axes) rather than HTML structure.

---

### D-06: Menu RenderingMode

| Field | Value |
|-------|-------|
| **Control** | Menu |
| **Divergence** | Web Forms Menu has two rendering modes controlled by the `RenderingMode` property: `RenderingMode="List"` (default in .NET 4.0+) renders `<ul>/<li>` structure; `RenderingMode="Table"` (legacy, default pre-4.0) renders `<table>/<tr>/<td>` structure. Blazor Menu renders `<ul>/<li>` structure only. |
| **Category** | Rendering Mode |
| **Reason** | The `<table>`-based rendering was the legacy default for accessibility and layout in older browsers. Microsoft switched the default to `<ul>/<li>` in .NET 4.0 as the web standards-compliant approach. Since Blazor targets modern browsers exclusively, only the standards-compliant `<ul>/<li>` mode is implemented. Applications migrating from `RenderingMode="Table"` will see a structural HTML change. |
| **CSS Impact** | **Medium** — CSS targeting `<table>`, `<tr>`, or `<td>` elements within the Menu will not match the `<ul>/<li>` output. Applications using the Table rendering mode will need CSS updates. Applications already using List mode (the default since .NET 4.0) will see no CSS impact. |
| **JS Impact** | **Low** — JavaScript traversing the Menu DOM via table-specific selectors (`querySelector('td')`, etc.) will break for Table-mode applications. List-mode applications are unaffected. |

**Audit approach:** The audit will capture Web Forms Menu in **both** rendering modes for documentation purposes. The Blazor comparison will be against the List-mode output only (since that's what Blazor implements). Any structural differences in List-mode output are potential bugs.

---

### D-07: TreeView JavaScript

| Field | Value |
|-------|-------|
| **Control** | TreeView |
| **Divergence** | Web Forms TreeView injects a `TreeView_ToggleNode` JavaScript function and a data array (`TreeView1_Data`) at page level. This JavaScript handles client-side expand/collapse without a postback. The tree nodes contain `onclick="TreeView_ToggleNode(...)"` attributes. Blazor TreeView uses `@onclick` event handlers for expand/collapse, with no page-level JavaScript injection. |
| **Category** | JS Interop |
| **Reason** | Web Forms' approach embeds JavaScript inline because there's no component-level script isolation — `TreeView_ToggleNode` is a global function injected once per page. Blazor's component model naturally isolates behavior within the component using C# event handlers, eliminating the need for global JavaScript. The structural HTML (the `<div>/<table>/<a>` node hierarchy) should be comparable, but the interactivity mechanism is completely different. |
| **CSS Impact** | **None** — The tree node HTML structure (`<div>` containers, `<a>` links, `<img>` expand/collapse icons) should remain consistent. CSS targeting the tree's visual structure is unaffected. |
| **JS Impact** | **High** — Any JavaScript that calls `TreeView_ToggleNode`, reads the `TreeView1_Data` array, or hooks into the TreeView's client-side events will not function. The entire client-side behavior model is replaced by Blazor's server-side event handling. |

**Normalization rule:** The normalization pipeline must strip all `<script>` blocks containing `TreeView_ToggleNode` or `TreeView*_Data` definitions. It must also normalize `onclick="TreeView_ToggleNode(...)"` attributes to a placeholder. Structural HTML comparison focuses on the DOM tree shape, not the event wiring.

---

### D-08: Calendar Day Selection

| Field | Value |
|-------|-------|
| **Control** | Calendar |
| **Divergence** | Web Forms Calendar renders day cells as `<a href="javascript:__doPostBack('Calendar1','selectDay3')">` for selectable days. Month/week navigation uses similar `__doPostBack` links. Blazor Calendar uses `@onclick` event handlers on the same structural elements. |
| **Category** | JS Interop |
| **Reason** | This is a specific instance of D-02 (PostBack Link Mechanism) applied to Calendar. The calendar's `<table>` structure — `<table>/<thead>/<tbody>/<tr>/<td>` with day numbers — should match between Web Forms and Blazor. Only the link/event mechanism for interacting with days differs. |
| **CSS Impact** | **None** — The `<table>` structure, CSS classes on day cells (`<td>`), and the overall calendar layout should be identical. Existing CSS targeting calendar structure is preserved. |
| **JS Impact** | **High** — JavaScript that intercepts calendar `__doPostBack` calls to customize day selection behavior will not work. Developers should use Blazor's `SelectionChanged` event callback instead. |

**Normalization rule:** Same as D-02 — replace `href="javascript:__doPostBack(...)"` with `href="[postback]"` before comparing. The focus is on the `<table>` structure, CSS classes, and cell content matching.

---

### D-09: Login Control Infrastructure

| Field | Value |
|-------|-------|
| **Control** | Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery |
| **Divergence** | Web Forms login controls integrate deeply with ASP.NET Membership/Identity providers. `LoginStatus` generates redirect URLs to the login page configured in `web.config`. `Login` calls `Membership.ValidateUser()` and issues `FormsAuthentication` tickets. `CreateUserWizard` calls `Membership.CreateUser()`. Blazor login controls are **visual shells only** — they render the form UI but do not include any built-in authentication provider integration. |
| **Category** | Infrastructure |
| **Reason** | ASP.NET Membership and FormsAuthentication are tightly coupled to the Web Forms pipeline (`HttpModule`, `HttpContext.User`, cookies). Blazor uses a completely different auth model (`AuthenticationStateProvider`, `CascadingAuthenticationState`). The login controls render the same form fields, labels, and layout, but the backend authentication plumbing is the developer's responsibility in Blazor. |
| **CSS Impact** | **None** — The rendered HTML (form fields, labels, validation messages, layout tables/divs) should be structurally identical. CSS targeting the login form visual appearance is preserved. |
| **JS Impact** | **Low** — Web Forms login controls don't typically inject significant client-side JavaScript. The impact is limited to cases where JavaScript reads authentication cookies or redirect URLs generated by `FormsAuthentication`. |

**Audit approach:** Compare the rendered HTML structure only. Authentication behavior (redirects, cookie issuance, membership provider calls) is out of scope for HTML audit — it's a behavioral concern, not a structural one.

---

### D-10: Validator Client-Side Scripts

| Field | Value |
|-------|-------|
| **Control** | RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator, ValidationSummary |
| **Divergence** | Web Forms validators inject `WebUIValidation.js` (via `WebResource.axd`) for client-side validation. This script provides functions like `ValidatorOnChange`, `ValidatorValidate`, `Page_ClientValidate`. Each validator `<span>` element has attributes like `evaluationfunction="RequiredFieldValidatorEvaluateIsValid"` and `controltovalidate="TextBox1"`. Blazor validators use component-based validation (Blazor's `EditContext`/`DataAnnotationsValidator` model or custom component logic) with no injected script library. |
| **Category** | JS Interop |
| **Reason** | Web Forms' client-side validation is a standalone JavaScript framework (`WebUIValidation.js`) that Web Forms controls opt into via special HTML attributes. Blazor's validation model is fundamentally different — validation state flows through `EditContext`, and validation UI is rendered by components reacting to state changes rather than DOM-level JavaScript. The validator `<span>` elements should render similarly (error messages, CSS classes for visibility), but the validation trigger mechanism is entirely different. |
| **CSS Impact** | **None** — Validator `<span>` elements with error messages and visibility CSS classes should match. `CssClass`, `ForeColor`, `Display` properties should produce the same visual output. |
| **JS Impact** | **High** — JavaScript that calls `Page_ClientValidate()`, hooks into `ValidatorOnChange`, or reads `evaluationfunction` attributes will not function. Blazor's validation is component-driven, not script-driven. |

**Normalization rule:** Strip `<script>` blocks containing `WebUIValidation` references. Strip validator-specific attributes that are Web Forms validation framework hooks (`evaluationfunction`, `controltovalidate`, `initialvalue`, etc.) since these are implementation details of the Web Forms validation JS, not structural HTML. Compare the `<span>` output (text, classes, visibility) only.

---

### D-11: GUID-Based IDs

| Field | Value |
|-------|-------|
| **Control** | CheckBox, RadioButton, RadioButtonList, FileUpload |
| **Divergence** | Web Forms generates predictable IDs from the control naming hierarchy (e.g., `MainContent_CheckBox1`, `MainContent_RadioButtonList1_0`, `MainContent_RadioButtonList1_1`). Blazor generates GUID-based IDs for some controls (e.g., `5f017a2765e3...`), making the HTML non-deterministic across renders. |
| **Category** | ID Generation |
| **Reason** | This is a **separate concern from D-01** (which covers `ctl00_` prefix mangling). D-01 addresses the naming-container prefix — a structural difference that is always intentional. D-11 addresses the fact that certain Blazor controls use `Guid.NewGuid()` to generate internal IDs for linking `<input>` and `<label>` elements, rather than deriving IDs from a developer-provided `ID` parameter. Web Forms uses the control's `ClientID` with `_0`, `_1` suffixes for list items. |
| **CSS Impact** | **Medium** — CSS rules targeting specific IDs (e.g., `#MainContent_RadioButtonList1_0`) will not match GUID-based IDs. More importantly, GUIDs change on every render, so even dynamic CSS/JS that reads the ID at runtime cannot cache it reliably. |
| **JS Impact** | **Medium** — JavaScript using `document.getElementById()` with predictable IDs will break. The non-deterministic nature of GUIDs makes them untargetable by any ID-based selector strategy. |

**Status:** 🔧 **Fix recommended.** GUIDs are a bug, not an intentional divergence. Controls should use the developer-provided `ID` parameter and append `_0`, `_1` per Web Forms convention. Registered here for tracking until the fix lands.

**Normalization rule:** The normalization pipeline should replace GUID-pattern IDs (32-character hex strings) with a placeholder `[GUID]` before comparison. This prevents false divergences in the diff report while the fix is pending.

---

### D-12: Boolean Attribute Format

| Field | Value |
|-------|-------|
| **Control** | DropDownList, ListBox, CheckBoxList, RadioButtonList (any control rendering `<option>` or `<input>` with boolean attributes) |
| **Divergence** | Web Forms renders boolean HTML attributes in XHTML style: `selected="selected"`, `checked="checked"`, `disabled="disabled"`. Blazor renders them in HTML5 style: `selected=""`, `checked`, `disabled`. Both are valid per the HTML specification. |
| **Category** | Attribute Format |
| **Reason** | **Intentional — platform-level behavior.** Web Forms was designed in the XHTML era and renders XHTML-compliant boolean attributes. Blazor's Razor engine follows HTML5 conventions where boolean attributes are present-or-absent. The W3C HTML5 specification explicitly allows both forms: `selected=""`, `selected="selected"`, and the bare `selected` attribute are all equivalent. There is no functional difference. |
| **CSS Impact** | **None** — CSS attribute selectors like `[selected]` match all three forms identically. |
| **JS Impact** | **None** — `element.selected`, `element.checked`, and `element.disabled` DOM properties work identically regardless of the HTML attribute format. `getAttribute('selected')` returns `""` vs `"selected"` — but well-written code uses the boolean DOM property, not the attribute string. |

**Status:** ✅ **Intentional — no fix needed.** Both formats are valid HTML. This is a platform-level rendering difference.

**Normalization rule:** The normalization pipeline should normalize boolean attributes to a canonical form (e.g., always `selected="selected"` or always bare `selected`) before comparison. Recommended: normalize to bare attribute form (HTML5 canonical).

---

### D-13: Calendar Previous-Month Day Padding

| Field | Value |
|-------|-------|
| **Control** | Calendar |
| **Divergence** | Web Forms Calendar pads the first week of the displayed month with day cells from the previous month. For example, if February starts on a Wednesday, the preceding Sunday–Tuesday cells show January 29, 30, 31 with `OtherMonthDayStyle` applied. Blazor Calendar may start the grid at the 1st of the displayed month, leaving those cells empty or absent. |
| **Category** | Structural |
| **Reason** | Web Forms' `System.Web.UI.WebControls.Calendar` always renders a full 6-row × 7-column grid (42 cells), filling leading and trailing cells with adjacent-month days. This ensures a consistent rectangular table layout regardless of which day the month starts on. The Blazor implementation may not fill leading/trailing cells with previous/next month dates, producing a ragged or shorter grid. |
| **CSS Impact** | **Low** — CSS targeting the calendar `<table>` structure is minimally affected. However, CSS that styles `OtherMonthDayStyle` cells (typically grayed-out previous/next month days) will have no targets if those cells are absent. |
| **JS Impact** | **None** — JavaScript does not typically interact with the padding cells. |

**Status:** 🔧 **Fix recommended.** The previous-month day padding is visible structural content, not an infrastructure concern. The Blazor Calendar should render the same 42-cell grid with adjacent-month day numbers and `OtherMonthDayStyle` applied. This directly impacts the Calendar's 73% similarity score.

**Normalization rule:** None — this is a structural difference that should show up in the audit diff. Once fixed, the diff should shrink.

---

### D-14: Calendar Style Property Pass-Through

| Field | Value |
|-------|-------|
| **Control** | Calendar |
| **Divergence** | Web Forms Calendar applies inline styles from style sub-properties (`TitleStyle`, `DayStyle`, `OtherMonthDayStyle`, `WeekendDayStyle`, `TodayDayStyle`, `SelectedDayStyle`, `NextPrevStyle`, `SelectorStyle`, `DayHeaderStyle`) as inline `style` attributes on the corresponding `<td>`, `<th>`, and `<tr>` elements. Blazor Calendar does not fully pass through these style properties to the rendered HTML elements. |
| **Category** | Style |
| **Reason** | Web Forms' `Calendar` control has a rich style sub-property system where each visual region (title bar, day cells, weekend cells, today cell, etc.) supports `ForeColor`, `BackColor`, `Font`, `CssClass`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Height`, `Width`, `HorizontalAlign`, `VerticalAlign`, `Wrap`. These are rendered as inline CSS on the target elements. The Blazor Calendar implementation may not map all of these style sub-properties to the corresponding rendered elements, resulting in missing inline styles. |
| **CSS Impact** | **Medium** — Migrating applications that set `DayStyle-BackColor`, `TodayDayStyle-ForeColor`, or similar sub-properties will see visual differences because the styles are not applied to the HTML output. Applications using `CssClass` on these sub-properties may be partially affected depending on which properties are actually rendered. |
| **JS Impact** | **None** — JavaScript does not typically read Calendar cell styles. |

**Status:** 🔧 **Fix progressively.** Calendar style application is incomplete — the `<table>` and cell-level styles from the various style sub-properties are not being fully applied. This is a significant fidelity gap for Calendar specifically. Prioritize `TitleStyle`, `DayStyle`, and `TodayDayStyle` first as the most commonly used.

**Normalization rule:** None — style differences should show up in the audit diff. As individual style sub-properties are implemented, the diff will shrink progressively.

---

## How to Use This Registry

This section explains how auditors (Colossus for capture, Forge for classification) should use this registry during the HTML fidelity audit.

### Step 1: Capture Both Outputs

For each control, capture the rendered HTML from:
- **Web Forms:** IIS Express serving the BeforeWebForms sample app
- **Blazor:** The AfterBlazorServerSide sample app

### Step 2: Run the Normalization Pipeline

The normalization pipeline (M11-06) applies the normalization rules documented in each divergence entry above:
- Strip `ctl00_` ID prefixes (D-01)
- Replace `__doPostBack(...)` with `[postback]` placeholder (D-02, D-08)
- Remove `__VIEWSTATE` / `__EVENTVALIDATION` hidden fields (D-03)
- Remove `WebResource.axd` script/link includes (D-04, D-10)
- Strip TreeView page-level JavaScript (D-07)
- Strip validator evaluation attributes (D-10)

### Step 3: Diff Normalized HTML

Compare the normalized Web Forms HTML against the Blazor HTML. Any remaining differences fall into two categories:

### Step 4: Classify Each Difference

| If the difference is... | Classification | Action |
|------------------------|----------------|--------|
| Listed in this registry | **Intentional divergence** | Document in the audit report as "intentional." Reference the D-XX entry number. No fix needed. |
| NOT listed in this registry AND is structural (tag names, nesting, classes) | **Potential bug** | File a GitHub Issue with expected vs. actual HTML. Route to Cyclops for investigation. |
| NOT listed in this registry AND is attribute-level (style values, minor formatting) | **Review needed** | Forge reviews to determine if it's a new intentional divergence (add to registry) or a bug (file issue). |

### Step 5: Update the Registry

As auditing progresses through M11–M13, new intentional divergences may be discovered. When Forge classifies a difference as intentional:
1. Add a new D-XX entry to this registry
2. Update the summary table
3. Add the normalization rule to the pipeline (Cyclops)
4. Reference the new entry in the audit report

---

## Appendix: Category Definitions

| Category | Definition |
|----------|-----------|
| **ID Mangling** | Differences in `id` and `name` attribute values caused by Web Forms' `INamingContainer` hierarchy |
| **ID Generation** | Differences in how controls generate internal IDs (e.g., GUIDs vs. deterministic naming) |
| **PostBack** | Differences caused by Web Forms' `__doPostBack` event mechanism vs. Blazor's `@onclick` handlers |
| **JS Interop** | Differences caused by Web Forms injecting page-level JavaScript vs. Blazor's C# event handling |
| **Infrastructure** | Platform-level differences (ViewState, WebResource.axd, authentication providers) that exist outside individual controls |
| **Structural** | Differences in the HTML tag structure itself (e.g., `<table>` vs. `<ul>`, missing cells) |
| **Rendering Mode** | Differences caused by Web Forms supporting multiple rendering modes where Blazor implements only one |
| **Attribute Format** | Differences in how HTML attributes are formatted (e.g., boolean attribute syntax) |
| **Style** | Differences in inline style application from control style sub-properties |

---

## Revision History

| Date | Author | Change |
|------|--------|--------|
| 2026-02-26 | Forge | Initial creation — 10 divergence entries (D-01 through D-10) |
| 2026-02-28 | Forge | Added D-11 through D-14 from M15–M18 audit findings (GUID IDs, boolean attrs, Calendar padding, Calendar styles). Added 3 new categories (ID Generation, Attribute Format, Style). |

— Forge
