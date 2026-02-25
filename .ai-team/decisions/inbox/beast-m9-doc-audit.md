# Beast M9 Documentation Audit Results

**Date:** 2026-02-25
**Author:** Beast (Technical Writer)
**Branch:** `milestone9/migration-fidelity`
**Requested by:** Jeffrey T. Fritz

---

## WI-09: Documentation Gap Audit

Audited `docs/` against features added in Milestones 6–8. Each control was checked for whether its M6-M8 features are documented.

### Summary Table

| Control | M6-M8 Feature | Doc Status | Notes |
|---------|---------------|------------|-------|
| GridView | Selection (SelectedIndex, SelectedRow, events) | ✅ Documented | Full coverage with examples |
| GridView | Styles (RowStyle, HeaderStyle, etc.) | ✅ Documented | 8 style sub-components listed |
| GridView | Display (ShowHeader, ShowFooter, Caption, EmptyDataTemplate, GridLines) | ✅ Documented | All properties covered |
| TreeView | Selection (SelectedNode, SelectedValue, event) | ✅ Documented | Dedicated reference table + example |
| TreeView | Expand/Collapse (ExpandDepth, ExpandAll, CollapseAll, FindNode) | ✅ Documented | Dedicated reference table + example |
| TreeView | Node-Level Styles (6 style sub-components) | ✅ Documented | Reference table + example |
| Menu | Selection (MenuItemClick, SelectedItem, SelectedValue) | ✅ Documented | Reference table + examples |
| Menu | Events (MenuItemClick, MenuItemDataBound) | ✅ Documented | Reference table |
| Menu | Style Sub-Components (6 styles) | ✅ Documented | Reference table |
| Menu | Base class upgrade | ✅ N/A | Implementation detail, not user-facing |
| FormView | ModeChanged event | ✅ Documented | Listed in features supported |
| FormView | ItemCommand event | ⚠️ **GAP** | Only in Web Forms syntax; not in Blazor features or Blazor syntax |
| FormView | Styles (RowStyle, HeaderStyle, etc.) | ⚠️ **GAP** | Only in Web Forms syntax; no Blazor documentation |
| FormView | PagerSettings | ⚠️ **GAP** | Only in Web Forms syntax; not documented for Blazor usage |
| DetailsView | Styles (HeaderStyle, RowStyle, etc.) | ⚠️ **GAP** | Listed as NOT supported — may be stale if M6-M8 added them |
| DetailsView | Caption property | ❌ **GAP** | Not mentioned in Blazor features or syntax |
| DetailsView | PagerSettings | ⚠️ Listed NOT supported | Doc says only numeric paging; verify if PagerSettings was added |
| DataGrid | Styles (7 sub-components) | ✅ Documented | Full coverage with migration example |
| DataGrid | Paging | ❌ Listed NOT supported | Still listed as unsupported; verify if M6-M8 added paging |
| Validators | ControlToValidate dual-path (string + ForwardRef) | ✅ Documented | Dedicated `ControlToValidate.md` page |
| PagerSettings | Shared component doc | ❌ **MISSING** | No dedicated documentation page exists |
| Login | Orientation | ✅ Documented | Reference table + examples + migration |
| Login | TextLayout | ✅ Documented | Reference table + examples + migration |
| ChangePassword | Orientation | ❌ **GAP** | Not mentioned anywhere in doc |
| ChangePassword | TextLayout | ❌ **GAP** | Not mentioned anywhere in doc |

### Detailed Findings

#### ✅ Fully Documented (No Action Needed)

1. **GridView** — All M6-M8 features (selection, styles, display properties) are thoroughly documented with reference tables, code examples, and migration notes.
2. **TreeView** — Selection, expand/collapse, ExpandDepth, and all 6 node-level style sub-components are documented with dedicated reference tables and examples.
3. **Menu** — Selection events, style sub-components, orientation, and navigation properties are all covered.
4. **Validators (ControlToValidate)** — Full dedicated page covering both string ID and ForwardRef patterns with examples and migration guidance.
5. **Login** — Orientation and TextLayout are documented with reference tables, code examples, and migration examples.

#### ⚠️ Gaps Requiring Doc Updates

6. **FormView** — Three gaps:
   - `ItemCommand` event is only shown in Web Forms syntax but not listed in Blazor features or Blazor syntax sections.
   - Style sub-components (EditRowStyle, EmptyDataRowStyle, FooterStyle, HeaderStyle, InsertRowStyle, PagerStyle, RowStyle) are only shown in Web Forms syntax. No Blazor usage documentation.
   - `PagerSettings` child component is only shown in Web Forms syntax. No Blazor syntax or usage guidance.

7. **DetailsView** — Three gaps:
   - `Caption` property is not mentioned in Blazor features or Blazor syntax.
   - Row styles (HeaderStyle, RowStyle, AlternatingRowStyle, etc.) are listed as "NOT Supported" — needs verification against current implementation. If M6-M8 added them, doc is stale.
   - `PagerSettings` listed as unsupported — needs verification.

8. **DataGrid** — One gap:
   - Paging (AllowPaging, PageSize, CurrentPageIndex) listed as NOT supported. Needs verification — if M6-M8 added it, doc is stale.

9. **ChangePassword** — Two gaps:
   - `Orientation` property is not documented (Login doc has it, ChangePassword does not).
   - `TextLayout` property is not documented (Login doc has it, ChangePassword does not).

10. **PagerSettings** — Missing entirely:
    - No dedicated documentation page exists for the shared `PagerSettings` sub-component, despite it being used by FormView, GridView, and DetailsView.

### Recommended Actions

| Priority | Action | Owner |
|----------|--------|-------|
| P1 | Add Orientation + TextLayout to ChangePassword.md (mirror Login.md pattern) | Beast |
| P1 | Create PagerSettings utility feature doc page | Beast |
| P2 | Update FormView.md: add ItemCommand to Blazor features/syntax, document styles + PagerSettings for Blazor | Beast |
| P2 | Update DetailsView.md: add Caption, verify and update styles/PagerSettings status | Beast + Cyclops |
| P3 | Verify DataGrid paging status and update doc if implemented | Beast + Cyclops |

---

## WI-10: Planning-Docs Historical Snapshot Headers

### Files Updated

Added `> ⚠️ **Historical Snapshot (Pre-Milestone 6):**` header to all 54 per-control audit files in `planning-docs/`, plus `SUMMARY.md` (dated 2026-02-12, pre-M6).

**Files marked as historical (54):**

AdRotator, BulletedList, Button, Calendar, ChangePassword, Chart, CheckBox, CheckBoxList, CompareValidator, CreateUserWizard, CustomValidator, DataGrid, DataList, DataPager, DetailsView, DropDownList, FileUpload, FormView, GridView, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, ListView, Literal, Localize, Login, LoginName, LoginStatus, LoginView, Menu, MultiView, Panel, PasswordRecovery, PlaceHolder, RadioButton, RadioButtonList, RangeValidator, RegularExpressionValidator, Repeater, RequiredFieldValidator, SiteMapPath, Substitution, Table, TextBox, TreeView, ValidationSummary, View, Xml, SUMMARY

**Files NOT marked (kept current):**

- `README.md` — Meta doc explaining the folder; still accurate
- `MILESTONE6-PLAN.md` — Forward-looking plan, not an audit
- `MILESTONE7-PLAN.md` — Forward-looking plan, not an audit
- `MILESTONE9-PLAN.md` — Current milestone plan; actively in use
