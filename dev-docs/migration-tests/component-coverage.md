# Component Documentation Coverage

A gap analysis of BWFC library components versus their documentation pages. Last updated: 2026-03-08.

## Summary

**Coverage: 52/52 main components documented (100%)**

All primary components in the BWFC library have corresponding documentation pages. Three controls are permanently deferred (Chart, Substitution, Xml) and are documented in the [Deferred Controls](../Migration/DeferredControls.md) guide — though Chart and Substitution also have their own component docs.

## Coverage by Category

### Editor Controls — 25 components ✅

| Component | Doc Page | Status |
|-----------|----------|--------|
| AdRotator | `EditorControls/AdRotator.md` | ✅ |
| BulletedList | `EditorControls/BulletedList.md` | ✅ |
| Button | `EditorControls/Button.md` | ✅ |
| Calendar | `EditorControls/Calendar.md` | ✅ |
| CheckBox | `EditorControls/CheckBox.md` | ✅ |
| CheckBoxList | `EditorControls/CheckBoxList.md` | ✅ |
| DropDownList | `EditorControls/DropDownList.md` | ✅ |
| FileUpload | `EditorControls/FileUpload.md` | ✅ |
| HiddenField | `EditorControls/HiddenField.md` | ✅ |
| Image | `EditorControls/Image.md` | ✅ |
| ImageButton | `EditorControls/ImageButton.md` | ✅ |
| Label | `EditorControls/Label.md` | ✅ |
| LinkButton | `EditorControls/LinkButton.md` | ✅ |
| ListBox | `EditorControls/ListBox.md` | ✅ |
| Literal | `EditorControls/Literal.md` | ✅ |
| Localize | `EditorControls/Localize.md` | ✅ |
| MultiView | `EditorControls/MultiView.md` | ✅ |
| Panel | `EditorControls/Panel.md` | ✅ |
| PlaceHolder | `EditorControls/PlaceHolder.md` | ✅ |
| RadioButton | `EditorControls/RadioButton.md` | ✅ |
| RadioButtonList | `EditorControls/RadioButtonList.md` | ✅ |
| Table | `EditorControls/Table.md` | ✅ |
| TextBox | `EditorControls/TextBox.md` | ✅ |
| View | (documented within MultiView.md) | ✅ |
| Localize | `EditorControls/Localize.md` | ✅ |

### Data Controls — 10 components ✅

| Component | Doc Page | Status |
|-----------|----------|--------|
| Chart | `DataControls/Chart.md` | ✅ |
| DataGrid | `DataControls/DataGrid.md` | ✅ |
| DataList | `DataControls/DataList.md` | ✅ |
| DataPager | `DataControls/DataPager.md` | ✅ |
| DetailsView | `DataControls/DetailsView.md` | ✅ |
| FormView | `DataControls/FormView.md` | ✅ |
| GridView | `DataControls/GridView.md` | ✅ |
| ListView | `DataControls/ListView.md` | ✅ |
| PagerSettings | `DataControls/PagerSettings.md` | ✅ |
| Repeater | `DataControls/Repeater.md` | ✅ |

### Validation Controls — 7 components ✅

| Component | Doc Page | Status |
|-----------|----------|--------|
| CompareValidator | `ValidationControls/CompareValidator.md` | ✅ |
| CustomValidator | `ValidationControls/CustomValidator.md` | ✅ |
| RangeValidator | `ValidationControls/RangeValidator.md` | ✅ |
| RegularExpressionValidator | `ValidationControls/RegularExpressionValidator.md` | ✅ |
| RequiredFieldValidator | `ValidationControls/RequiredFieldValidator.md` | ✅ |
| ValidationSummary | `ValidationControls/ValidationSummary.md` | ✅ |
| ModelErrorMessage | `ValidationControls/ModelErrorMessage.md` | ✅ |

### Navigation Controls — 5 components ✅

| Component | Doc Page | Status |
|-----------|----------|--------|
| HyperLink | `NavigationControls/HyperLink.md` | ✅ |
| ImageMap | `NavigationControls/ImageMap.md` | ✅ |
| Menu | `NavigationControls/Menu.md` | ✅ |
| SiteMapPath | `NavigationControls/SiteMapPath.md` | ✅ |
| TreeView | `NavigationControls/TreeView.md` | ✅ |

### Login Controls — 7 components ✅

| Component | Doc Page | Status |
|-----------|----------|--------|
| ChangePassword | `LoginControls/ChangePassword.md` | ✅ |
| CreateUserWizard | `LoginControls/CreateUserWizard.md` | ✅ |
| Login | `LoginControls/Login.md` | ✅ |
| LoginName | `LoginControls/LoginName.md` | ✅ |
| LoginStatus | `LoginControls/LoginStatus.md` | ✅ |
| LoginView | `LoginControls/LoginView.md` | ✅ |
| PasswordRecovery | `LoginControls/PasswordRecovery.md` | ✅ |

### AJAX Controls — 5 components ✅

| Component | Doc Page | Status |
|-----------|----------|--------|
| ScriptManager | `EditorControls/ScriptManager.md` | ✅ |
| ScriptManagerProxy | `EditorControls/ScriptManagerProxy.md` | ✅ |
| Substitution | `EditorControls/Substitution.md` | ✅ |
| Timer | `EditorControls/Timer.md` | ✅ |
| UpdatePanel | `EditorControls/UpdatePanel.md` | ✅ |
| UpdateProgress | `EditorControls/UpdateProgress.md` | ✅ |

### Utility Features — 7 docs ✅

| Feature | Doc Page | Status |
|---------|----------|--------|
| Databinder | `UtilityFeatures/Databinder.md` | ✅ |
| ID Rendering | `UtilityFeatures/IDRendering.md` | ✅ |
| JavaScript Setup | `UtilityFeatures/JavaScriptSetup.md` | ✅ |
| NamingContainer | `UtilityFeatures/NamingContainer.md` | ✅ |
| Page System | `UtilityFeatures/PageService.md` | ✅ |
| ViewState | `UtilityFeatures/ViewState.md` | ✅ |
| WebFormsPage | `UtilityFeatures/WebFormsPage.md` | ✅ |

## Sub-Components Without Standalone Docs

These are child/style sub-components documented within their parent component pages. They do not need standalone docs:

- **View** — documented in MultiView.md
- **RoleGroup** — documented in LoginView.md
- **MenuItem** — documented in Menu.md
- **TreeNode** — documented in TreeView.md
- **ChartArea, ChartSeries, ChartTitle, ChartLegend** — documented in Chart.md
- **BoundField, ButtonField, HyperLinkField, TemplateField** — documented in GridView.md
- **PagerSettings** — has its own doc; also referenced in GridView, FormView, DetailsView, ListView
- All `*Style.razor` components — documented within their parent's "Features Supported" sections

## Gaps Identified

No gaps found. All 52 primary components have corresponding documentation.

!!! note "Potential improvement"
    The `RoleGroup` and `View` components could benefit from their own short doc pages with cross-links to their parent components, making them more discoverable via search. This is a nice-to-have, not a gap.
