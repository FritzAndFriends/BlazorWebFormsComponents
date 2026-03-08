# BWFC Library Audit — 2026-03-06

**Auditor:** Forge (Lead / Web Forms Reviewer)
**Requested by:** Jeffrey T. Fritz
**Scope:** Full library audit of `src/BlazorWebFormsComponents/`

---

## Executive Summary

The BlazorWebFormsComponents library ships **153 Razor components** and **197 standalone C# classes** (enums, event args, base classes, interfaces, utilities). CONTROL-COVERAGE.md documented **58 primary controls** — this count is accurate for top-level Web Forms control equivalents, but the document omitted **95 supporting Razor components** (style sub-components, field columns, child components, infrastructure, helpers) and the entire utility/infrastructure C# layer.

Key findings:
- **ContentPlaceHolder** was listed as "Not Supported" in CONTROL-COVERAGE.md, but a working component EXISTS in the library
- **4 field column components** (BoundField, ButtonField, HyperLinkField, TemplateField) are shipped but undocumented as independent components
- **10+ infrastructure components** (MasterPage, Content, WebFormsPage, Page, NamingContainer, EmptyLayout, etc.) are shipped but undocumented
- **63 style sub-components** are shipped for declarative styling — completely undocumented
- **54 enums**, **16 interfaces**, **3 extensions**, **4 data binding classes**, **3 custom control shims**, and **3 theming classes** are shipped but not referenced in CONTROL-COVERAGE.md
- The AJAX Controls section listed 5 components but Web Forms actually has 6 (Substitution was miscategorized under Editor Controls — it IS an AJAX control in Web Forms, but functionally it fits Editor better in BWFC since it renders content)

**Verdict:** The library is far more complete than CONTROL-COVERAGE.md suggests. The document needs significant expansion to cover supporting components, infrastructure, and utilities.

---

## Phase 1: Full Component Catalog

### 1. Primary Web Forms Controls (58 components)

These are the top-level controls directly emulating ASP.NET Web Forms server controls.

#### Editor Controls (28)

| # | Component | Type | Notes |
|---|-----------|------|-------|
| 1 | AdRotator | .razor + .cs | Rotating ad display with Advertisment model |
| 2 | BulletedList | .razor + .cs | Ordered/unordered lists with DisplayMode enum |
| 3 | Button | .razor + .cs | Standard button, inherits ButtonBaseComponent |
| 4 | Calendar | .razor + .cs | Full calendar with 9 style sub-components |
| 5 | Chart | .razor + .cs | Chart.js-based; uses ChartJsInterop, ChartConfigBuilder |
| 6 | CheckBox | .razor + .cs | Two-way binding via @bind-Checked |
| 7 | CheckBoxList | .razor + .cs | List of checkboxes from Items collection |
| 8 | DropDownList | .razor + .cs | Select element with Items + SelectedValue binding |
| 9 | FileUpload | .razor + .cs | Uses Blazor InputFile internally |
| 10 | HiddenField | .razor + .cs | Hidden input with Value property |
| 11 | HyperLink | .razor + .cs | Anchor tag with NavigateUrl |
| 12 | Image | .razor + .cs | Img tag with ImageUrl |
| 13 | ImageButton | .razor + .cs | Clickable image |
| 14 | ImageMap | .razor + .cs | Image with clickable hotspot regions |
| 15 | Label | .razor + .cs | Span/label with AssociatedControlID |
| 16 | LinkButton | .razor + .cs | Anchor styled as button, CommandName/CommandArgument |
| 17 | ListBox | .razor + .cs | Multi-select list |
| 18 | Literal | .razor + .cs | Raw text/HTML output with LiteralMode |
| 19 | Localize | .cs only | Inherits Literal; markup compatibility shim |
| 20 | MultiView | .razor + .cs | Container for View components |
| 21 | Panel | .razor + .cs | Renders `<div>` |
| 22 | PlaceHolder | .razor + .cs | Structural container, renders no HTML |
| 23 | RadioButton | .razor + .cs | Radio input with GroupName |
| 24 | RadioButtonList | .razor + .cs | Group of radio buttons from Items |
| 25 | Substitution | .razor + .cs | Func<HttpContext, string> callback rendering |
| 26 | Table | .razor + .cs | HTML table with TableRow/TableCell children |
| 27 | TextBox | .razor + .cs | Input/textarea with TextMode enum |
| 28 | View | .razor + .cs | Child of MultiView |

#### Data Controls (8)

| # | Component | Type | Notes |
|---|-----------|------|-------|
| 1 | DataGrid | .razor + .cs | Legacy grid with paging, sorting, column generator |
| 2 | DataList | .razor + .cs | Repeating data with style sub-components |
| 3 | DataPager | .razor + .cs | Pagination for pageable controls |
| 4 | DetailsView | .razor + .cs | Single-record display with 12 style sub-components |
| 5 | FormView | .razor + .cs | Templated single-record with RenderOuterTable |
| 6 | GridView | .razor + .cs | Full-featured grid with columns, paging, sorting |
| 7 | ListView | .razor + .cs | Most flexible data control, LayoutTemplate + GroupTemplate |
| 8 | Repeater | .razor + .cs | Simple templated repeater with SeparatorTemplate |

#### Validation Controls (7)

| # | Component | Type | Notes |
|---|-----------|------|-------|
| 1 | CompareValidator | .cs only | Inherits BaseCompareValidator → BaseValidator.razor |
| 2 | CustomValidator | .cs only | Server-side validation callback |
| 3 | ModelErrorMessage | .razor + .cs | Blazor EditContext integration, ModelStateKey |
| 4 | RangeValidator | .cs only | Min/Max with ValidationDataType |
| 5 | RegularExpressionValidator | .cs only | Pattern-based validation |
| 6 | RequiredFieldValidator | .cs only | Non-empty field check |
| 7 | AspNetValidationSummary | .razor + .cs | Maps to Web Forms `ValidationSummary` |

#### Navigation Controls (3)

| # | Component | Type | Notes |
|---|-----------|------|-------|
| 1 | Menu | .razor + .cs | Horizontal/vertical menu with MenuItem children |
| 2 | SiteMapPath | .razor + .cs | Breadcrumb navigation with SiteMapNode data |
| 3 | TreeView | .razor + .cs | Hierarchical tree with 7 style sub-components |

#### Login Controls (7)

| # | Component | Type | Notes |
|---|-----------|------|-------|
| 1 | ChangePassword | .razor + .cs | Password change form with Orientation/TextLayout |
| 2 | CreateUserWizard | .razor + .cs | Multi-step user creation wizard |
| 3 | Login | .razor + .cs | Authentication form with 10+ style sub-components |
| 4 | LoginName | .razor + .cs | Displays authenticated user name |
| 5 | LoginStatus | .razor + .cs | Login/logout toggle link |
| 6 | LoginView | .razor + .cs | Template switching based on auth state |
| 7 | PasswordRecovery | .razor + .cs | 3-step password recovery wizard |

#### AJAX Controls (5)

| # | Component | Type | Notes |
|---|-----------|------|-------|
| 1 | ScriptManager | .razor + .cs | Migration stub — renders nothing |
| 2 | ScriptManagerProxy | .razor + .cs | Migration stub — renders nothing |
| 3 | Timer | .razor + .cs | Interval-based tick events |
| 4 | UpdatePanel | .razor + .cs | Renders div/span — Blazor already does partial rendering |
| 5 | UpdateProgress | .razor + .cs | Loading indicator with explicit bool state |

---

### 2. Field Column Components (4 components) — UNDOCUMENTED

Used inside GridView, DetailsView, and DataGrid. These ARE Web Forms controls.

| Component | Type | Notes |
|-----------|------|-------|
| BoundField | .razor + .cs | Auto-generates columns from data properties with DataFormatString |
| ButtonField | .razor + .cs | Column with button (Button/Link/Image types) |
| HyperLinkField | .razor + .cs | Column with navigable hyperlinks |
| TemplateField | .razor + .cs | Fully templated column with HeaderTemplate/ItemTemplate/EditItemTemplate |

---

### 3. Child / Structural Components (9 components) — UNDOCUMENTED

| Component | Location | Notes |
|-----------|----------|-------|
| MenuItem | root | Child of Menu; defines menu items |
| TreeNode | root | Child of TreeView; defines tree nodes |
| DataGridRow | root | Internal row rendering for DataGrid |
| GridViewRow | root | Internal row rendering for GridView |
| GroupTemplate | root | Used with ListView for item grouping |
| Items | root | List item container template |
| Nodes | root | Tree node container template |
| DataBindings | root | Data binding template container |
| RoleGroup | LoginControls/ | Child of LoginView for role-based templates |

---

### 4. Style Sub-Components (63 components) — UNDOCUMENTED

These provide declarative styling matching Web Forms' sub-element style pattern.

| Category | Count | Components |
|----------|-------|------------|
| Calendar styles | 9 | CalendarDayHeaderStyle, CalendarDayStyle, CalendarNextPrevStyle, CalendarOtherMonthDayStyle, CalendarSelectedDayStyle, CalendarSelectorStyle, CalendarTitleStyle, CalendarTodayDayStyle, CalendarWeekendDayStyle |
| DataGrid styles | 7 | DataGridAlternatingItemStyle, DataGridEditItemStyle, DataGridFooterStyle, DataGridHeaderStyle, DataGridItemStyle, DataGridPagerStyle, DataGridSelectedItemStyle |
| DetailsView styles | 10 | DetailsViewAlternatingRowStyle, DetailsViewCommandRowStyle, DetailsViewEditRowStyle, DetailsViewEmptyDataRowStyle, DetailsViewFieldHeaderStyle, DetailsViewFooterStyle, DetailsViewHeaderStyle, DetailsViewInsertRowStyle, DetailsViewPagerStyle, DetailsViewRowStyle |
| FormView styles | 7 | FormViewEditRowStyle, FormViewEmptyDataRowStyle, FormViewFooterStyle, FormViewHeaderStyle, FormViewInsertRowStyle, FormViewPagerStyle, FormViewRowStyle |
| GridView styles | 7 | GridViewAlternatingRowStyle, GridViewEditRowStyle, GridViewEmptyDataRowStyle, GridViewFooterStyle, GridViewHeaderStyle, GridViewPagerStyle, GridViewRowStyle, GridViewSelectedRowStyle |
| TreeView styles | 6 | TreeViewHoverNodeStyle, TreeViewLeafNodeStyle, TreeViewNodeStyle, TreeViewParentNodeStyle, TreeViewRootNodeStyle, TreeViewSelectedNodeStyle |
| Login styles | 10 | CheckBoxStyle, FailureTextStyle, HyperLinkStyle, InstructionTextStyle, LabelStyle, LoginButtonStyle, SuccessTextStyle, TextBoxStyle, TitleTextStyle, ValidatorTextStyle |
| Shared styles | 5 | AlternatingItemStyle, FooterStyle, HeaderStyle, ItemStyle, MenuItemStyle, SeparatorStyle |
| PagerSettings | 3 | DetailsViewPagerSettings, FormViewPagerSettings, GridViewPagerSettings |

---

### 5. Infrastructure Components (7 components) — UNDOCUMENTED

| Component | Type | Notes |
|-----------|------|-------|
| Content | .razor + .cs | Provides content to ContentPlaceHolder slots (MasterPage child) |
| ContentPlaceHolder | .razor + .cs | Defines replaceable content regions in MasterPage |
| MasterPage | .razor + .cs | Emulates Web Forms Master Pages using Blazor layouts |
| WebFormsPage | .razor + .cs | Wrapper component providing cascading values + head rendering |
| Page | .razor + .cs | Renders PageTitle/HeadContent from IPageService |
| NamingContainer | .razor + .cs | Establishes naming scope with ClientID prefixing |
| EmptyLayout | .razor | Minimal layout (just `@Body`) used by MasterPage |

---

### 6. Helper Components (3 components) — UNDOCUMENTED

| Component | Location | Notes |
|-----------|----------|-------|
| BlazorWebFormsHead | HelperComponents/ | Adds BWFC script tag to `<head>` via HeadContent |
| BlazorWebFormsScripts | HelperComponents/ | Auto-loads JS module via dynamic import, IAsyncDisposable |
| Eval | HelperComponents/ | DataBinder.Eval helper — reflection-based property access |

---

### 7. Theming (1 component + 3 classes) — UNDOCUMENTED

| Item | Type | Notes |
|------|------|-------|
| ThemeProvider | .razor | Cascades ThemeConfiguration to child components |
| ThemeConfiguration | .cs | Dictionary of ControlSkin entries keyed by control type + SkinID |
| ControlSkin | .cs | Holds visual properties (BackColor, CssClass, Font, etc.) |
| SkinBuilder | .cs | Fluent API for constructing ControlSkin instances |

---

### 8. Validation Infrastructure (8 classes) — UNDOCUMENTED

| Item | Type | Notes |
|------|------|-------|
| BaseValidator | .razor + .cs | Base Razor component for all validators |
| BaseCompareValidator | .cs | Base for CompareValidator and RangeValidator |
| ValidationGroupProvider | .razor | Provides validation group scoping |
| ValidationGroupCoordinator | .cs | Coordinates validation across groups |
| ComparerFactory | .cs | Creates type-specific comparers |
| ForwardRef | .cs | Element reference forwarding utility |
| TypeComparers/ (6 files) | .cs | CurrencyComparer, DateComparer, DoubleComparer, IComparer, IntegerComparer, StringComparer |

---

### 9. Enums (54 enums)

| Enum | Used By |
|------|---------|
| BorderStyle | BaseStyledComponent |
| BulletedListDisplayMode | BulletedList |
| BulletStyle | BulletedList |
| ButtonType | ButtonField |
| CalendarSelectionMode | Calendar |
| ChartDashStyle | Chart |
| ChartPalette | Chart |
| ClientIDMode | BaseWebFormsComponent |
| ContentDirection | Panel |
| DayNameFormat | Calendar |
| DetailsViewMode | DetailsView |
| Docking | ChartLegend |
| FontSize | Style/FontInfo |
| FormViewMode | FormView |
| GridLines | Table, GridView |
| HorizontalAlign | Table, Panel |
| HotSpotMode | ImageMap |
| HyperLinkTarget | HyperLink |
| ImageAlign | Image |
| InsertItemPosition | ListView |
| ListSelectionMode | ListBox |
| ListViewCancelMode | ListView |
| ListViewItemType | ListView |
| LiteralMode | Literal |
| LoginTextLayout | Login controls |
| LogoutAction | LoginStatus |
| MenuRenderingMode | Menu |
| Orientation | Login controls |
| PagerButtons | PagerSettings |
| PagerPosition | PagerSettings |
| PathDirection | SiteMapPath |
| RepeatDirection | CheckBoxList, RadioButtonList |
| RepeatLayout | CheckBoxList, RadioButtonList |
| ScriptMode | ScriptReference |
| ScrollBars | Panel |
| SeriesChartType | ChartSeries |
| SortDirection | GridView, DataGrid |
| TableCaptionAlign | Table |
| TableHeaderScope | TableHeaderCell |
| TableRowSection | TableRow |
| TextAlign | CheckBox, RadioButton |
| TextBoxMode | TextBox |
| TitleFormat | Calendar |
| TreeNodeSelectAction | TreeNode |
| TreeNodeTypes | TreeNodeBinding |
| TreeViewImageSet | TreeView |
| UnitType | Unit |
| UpdatePanelRenderMode | UpdatePanel |
| UpdatePanelUpdateMode | UpdatePanel |
| ValidationCompareOperator | CompareValidator |
| ValidationDataType | BaseCompareValidator |
| ValidationSummaryDisplayMode | AspNetValidationSummary |
| ValidatorDisplay | BaseValidator |
| VerticalAlign | TableCell |

---

### 10. Interfaces (16)

| Interface | Purpose |
|-----------|---------|
| IButtonComponent | Shared button properties |
| ICalendarStyleContainer | Calendar style slots |
| IColumn | Column definition |
| IColumnCollection | Column collection |
| IDataGridStyleContainer | DataGrid style slots |
| IDataListStyleContainer | DataList style slots |
| IDetailsViewStyleContainer | DetailsView style slots |
| IFormViewStyleContainer | FormView style slots |
| IGridViewStyleContainer | GridView style slots |
| IImageComponent | Shared image properties |
| IMenuStyleContainer | Menu style slots |
| IPagerSettingsContainer | Pager settings |
| IRow | Row definition |
| IRowCollection | Row collection |
| ITextComponent | Shared text properties |
| ITreeViewStyleContainer | TreeView style slots |

---

### 11. Style Infrastructure (6 interfaces + 1 class)

| Item | Purpose |
|------|---------|
| IStyle | Core style properties interface |
| IFontStyle | Font-related properties |
| IHasLayoutStyle | Layout container styling |
| IHasLayoutTableItemStyle | Layout table item styling |
| IHasTableItemStyle | Table item styling |
| FontInfo | Font property bag (Style/Fonts/) |

---

### 12. Extensions (3)

| Extension | Purpose |
|-----------|---------|
| ColorHelpers | WebColor ↔ CSS string conversion |
| GetRouteUrlHelper | Web Forms GetRouteUrl() emulation |
| HasStyleExtensions | Style string builder for IStyle implementors |

---

### 13. Data Binding Infrastructure (4 classes)

| Class | Purpose |
|-------|---------|
| BaseDataBoundComponent | Base for all data-bound components |
| BaseListControl | Base for list-type controls (DropDownList, ListBox, etc.) |
| DataBoundComponent | Generic data-bound component with Items support |
| SelectHandler | Handler for selection events in data-bound controls |

---

### 14. Custom Control Shims (3 classes)

| Class | Purpose |
|-------|---------|
| WebControl | Minimal Web Forms WebControl compatibility shim |
| HtmlTextWriter | Minimal HtmlTextWriter compatibility shim |
| CompositeControl | Minimal CompositeControl compatibility shim |

---

### 15. Base Classes (6)

| Class | Purpose |
|-------|---------|
| BaseWebFormsComponent | Root base class — ID, ClientID, Visible, ViewState, lifecycle events, FindControl, theming |
| BaseStyledComponent | Adds CssClass, BackColor, ForeColor, BorderStyle, Font, Width, Height |
| WebFormsPageBase | Page base class — Title, MetaDescription, MetaKeywords, IsPostBack, `Page => this` |
| ButtonBaseComponent | Shared button logic for Button, LinkButton, ImageButton |
| BaseColumn | Base for column definitions |
| BaseRow | Base for row definitions |

---

### 16. Service / DI Classes (3)

| Class | Purpose |
|-------|---------|
| IPageService | Interface for page metadata (Title, MetaDescription, MetaKeywords) |
| PageService | Scoped implementation of IPageService |
| ServiceCollectionExtensions | `AddBlazorWebFormsComponents()` — registers JsInterop + PageService |

---

### 17. JS Interop (3)

| Class | Purpose |
|-------|---------|
| BlazorWebFormsJsInterop | Main JS interop service, auto-loads module |
| ChartJsInterop | Chart.js-specific JS interop |
| JsScripts | Constants for JS function names |

---

### 18. Event Args Classes (38)

| Category | Classes |
|----------|---------|
| General | AdCreatedEventArgs, BulletedListEventArgs, CommandEventArgs, ImageMapEventArgs, MenuEventArgs, PageChangedEventArgs |
| DataGrid | DataGridCommandEventArgs, DataGridItemEventArgs, DataGridPageChangedEventArgs, DataGridSortCommandEventArgs |
| DataList | DataListItemEventArgs |
| DetailsView | DetailsViewEventArgs |
| FormView | FormViewCommandEventArgs, FormViewDeletedEventArgs, FormViewDeleteEventArgs, FormViewInsertEventArgs, FormViewModeEventArgs, FormViewUpdatedEventArgs, FormViewUpdateEventArgs |
| GridView | GridViewCancelEditEventArgs, GridViewDeleteEventArgs, GridViewEditEventArgs, GridViewSelectEventArgs, GridViewSortEventArgs, GridViewUpdateEventArgs |
| ListView | ListViewCancelEventArgs, ListViewCommandEventArgs, ListViewDeletedEventArgs, ListViewDeleteEventArgs, ListViewEditEventArgs, ListViewInsertedEventArgs, ListViewInsertEventArgs, ListViewItemEventArgs, ListViewPagePropertiesChangingEventArgs, ListViewSelectEventArgs, ListViewSortEventArgs, ListViewUpdatedEventArgs, ListViewUpdateEventArgs |
| Login | AuthenticateEventArgs, CreateUserErrorEventArgs, LoginCancelEventArgs, MailMessageEventArgs, SendMailErrorEventArgs |

---

### 19. Model / Utility Classes (22)

| Class | Purpose |
|-------|---------|
| Advertisment | Ad rotation data model |
| Axis | Chart axis configuration |
| ChartConfigBuilder | Builds Chart.js configuration |
| CircleHotSpot | Circle region for ImageMap |
| ColorTranslator | Named color → hex conversion |
| ComponentIdGenerator | ClientID generation logic |
| DataGridColumnGenerator | Auto-generates DataGrid columns |
| DataPoint | Chart data point |
| FontUnit | Font size with unit |
| GridViewColumnGenerator | Auto-generates GridView columns |
| Helpers | Miscellaneous utility methods |
| HotSpot | Base class for ImageMap hotspots |
| ListItem | Item in a list control |
| ListItemCollection | Collection of ListItem |
| ListViewDataItem | Data item wrapper for ListView |
| ListViewItem | Item wrapper for ListView |
| MenuItemBinding | Data binding for Menu items |
| MenuLevelStyle | Per-level Menu styling |
| PagerSettings | Pager configuration (non-Razor) |
| PolygonHotSpot | Polygon region for ImageMap |
| RectangleHotSpot | Rectangle region for ImageMap |
| RoleGroupCollection | Collection of RoleGroup |
| ScriptReference | Script reference for ScriptManager |
| ServiceReference | Service reference for ScriptManager |
| SiteMapNode | Navigation node for SiteMapPath |
| Style | Base style class |
| TableItemStyle | Table cell/row style |
| TreeNodeBinding | Data binding for TreeNode |
| TreeNodeCollection | Collection of TreeNode |
| TreeNodeStyle | Style for TreeNode |
| UiPagerSettings | UI-specific pager settings |
| UiStyle | UI-specific style container |
| UiTableItemStyle | UI-specific table item style |
| UiTreeNodeStyle | UI-specific tree node style |
| Unit | CSS measurement (px, em, %, etc.) |
| WebColor | Color with named color support |
| DataBinder | [Obsolete] Web Forms DataBinder.Eval() emulation |

---

## Phase 2: Comparison Against CONTROL-COVERAGE.md

### Components in Code but NOT in CONTROL-COVERAGE.md

#### Critical Omissions (developer-facing components):

1. **ContentPlaceHolder** — Listed as "Not Supported" but EXISTS as a working component
2. **Content** — MasterPage content provider, completely undocumented
3. **MasterPage** — Master Page emulation, completely undocumented
4. **BoundField** — Referenced in GridView description but not listed as its own component
5. **TemplateField** — Referenced in GridView description but not listed as its own component
6. **ButtonField** — Grid column component, undocumented
7. **HyperLinkField** — Grid column component, undocumented
8. **WebFormsPage** — Page wrapper component, undocumented
9. **Page** — Page metadata renderer, undocumented
10. **NamingContainer** — ID naming scope, undocumented
11. **ThemeProvider** — Theming infrastructure, undocumented
12. **ValidationGroupProvider** — Validation group scoping, undocumented
13. **EmptyLayout** — Minimal layout, undocumented

#### Child Components (used in developer markup but not independently documented):

14. **MenuItem** — Child of Menu
15. **TreeNode** — Child of TreeView
16. **RoleGroup** — Child of LoginView
17. **DataGridRow**, **GridViewRow** — Row renderers
18. **GroupTemplate**, **Items**, **Nodes**, **DataBindings** — Template containers

#### Infrastructure (63 style sub-components + 3 PagerSettings):

All 63 style sub-components and 3 PagerSettings components are shipped but undocumented. Developers use these in markup (e.g., `<GridViewHeaderStyle CssClass="header" />`).

#### Helper Components:

- **BlazorWebFormsHead**, **BlazorWebFormsScripts**, **Eval** — undocumented

### Components in CONTROL-COVERAGE.md but NOT in Code

**None.** All 58 listed components exist in the codebase. ✅

### Inaccuracies in CONTROL-COVERAGE.md

1. **ContentPlaceHolder listed as "Not Supported"** — This is WRONG. ContentPlaceHolder.razor exists and works with MasterPage.razor + Content.razor. The "Not Supported" section says "Maps directly to Blazor's @Body — this is a framework concept, not a component gap." This is misleading — we provide a real component.

2. **Component count "58"** — Accurate for primary controls, but misleading. The library ships 153 Razor components total. The document should clarify "58 primary Web Forms control equivalents" and note supporting components.

3. **Missing categories** — CONTROL-COVERAGE.md lists 6 categories (Editor, Data, Validation, Navigation, Login, AJAX). Missing categories:
   - **Infrastructure** (MasterPage, Content, ContentPlaceHolder, WebFormsPage, Page, NamingContainer, EmptyLayout)
   - **Field Columns** (BoundField, ButtonField, HyperLinkField, TemplateField)
   - **Theming** (ThemeProvider, ThemeConfiguration, ControlSkin, SkinBuilder)
   - **Helpers** (BlazorWebFormsHead, BlazorWebFormsScripts, Eval, DataBinder)
   - **Base Classes & Services** (WebFormsPageBase, ServiceCollectionExtensions, etc.)

4. **AJAX count says 5 but section title says "AJAX Controls (5 components)"** — Correct count (5), but Substitution in Web Forms is technically `System.Web.UI.WebControls.Substitution` (under AJAX framework). BWFC categorizes it under Editor, which is fine for migration purposes.

### Utilities and Infrastructure NOT Documented

The following are critical for migration developers but completely absent from CONTROL-COVERAGE.md:

- **WebFormsPageBase** — `@inherits WebFormsPageBase` gives converted pages `Page.Title`, `Page.MetaDescription`, `IsPostBack`
- **ServiceCollectionExtensions** — `AddBlazorWebFormsComponents()` required in Program.cs
- **DataBinder** — Transitional `DataBinder.Eval()` support (marked obsolete)
- **BaseWebFormsComponent** — Provides ID, ClientID, ViewState, Visible, lifecycle events
- **BaseStyledComponent** — Provides CssClass, BackColor, ForeColor, Font, Width, Height
- **54 Enums** — All Web Forms enum types faithfully reproduced
- **CustomControls/** — WebControl, HtmlTextWriter, CompositeControl shims for code-behind compilation

---

## Phase 3: Changes Made to CONTROL-COVERAGE.md

1. **Fixed ContentPlaceHolder** — Removed from "Not Supported" section, added to new Infrastructure section
2. **Updated component count** — Changed from "58" to "58 primary + 95 supporting" (153 total Razor components)
3. **Added "Infrastructure Controls" section** — Content, ContentPlaceHolder, MasterPage, WebFormsPage, Page, NamingContainer, EmptyLayout (7 components)
4. **Added "Field Column Components" section** — BoundField, ButtonField, HyperLinkField, TemplateField (4 components)
5. **Added "Style Sub-Components" section** — Documented the 63 style components + 3 PagerSettings
6. **Added "Utilities & Infrastructure" section** — WebFormsPageBase, ServiceCollectionExtensions, DataBinder, ThemeProvider, base classes, enums, custom control shims, helper components
7. **Updated visual summary** to reflect actual totals

---

## Phase 4: Recommendations

### High Priority

1. **Document setup requirements** — `AddBlazorWebFormsComponents()` and `@inherits WebFormsPageBase` should be prominently documented. Many developers will miss these.

2. **Document MasterPage → Layout migration** — Content/ContentPlaceHolder/MasterPage is a complete Master Page emulation system. This is a major feature that's invisible in the docs.

3. **Document field columns** — BoundField/TemplateField are heavily used in GridView migration and deserve their own entries.

### Medium Priority

4. **Document style sub-components** — At minimum, document the pattern: `<GridView><GridViewHeaderStyle CssClass="header" /></GridView>`

5. **Document ThemeProvider** — The theming system (ThemeProvider + ThemeConfiguration + ControlSkin) is a significant feature for apps using Web Forms Themes.

6. **Document helper components** — Eval, BlazorWebFormsHead, BlazorWebFormsScripts have specific use cases.

### Low Priority

7. **Consider splitting CONTROL-COVERAGE.md** — At 200+ lines it's getting long. Consider a separate INFRASTRUCTURE.md for non-control components.

8. **Enum documentation** — The 54 enums are critical for code-behind migration. A reference table would help.

---

## Appendix: Full File Counts

| Category | Count |
|----------|-------|
| Razor components (.razor) | 153 |
| Code-behind files (.razor.cs) | 134 |
| Standalone C# classes (.cs) | 197 |
| Enums | 54 |
| Interfaces | 16 + 6 (Style/) = 22 |
| Extensions | 3 |
| Event args classes | 38 |
| Base classes | 6 |
| **Total source files** | **~350** |
