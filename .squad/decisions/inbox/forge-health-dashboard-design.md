# Component Health Dashboard — Scoring Specification

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-15
**Issue:** #48
**Status:** DESIGN — For implementation by Rogue

---

## 1. Purpose

The Component Health Dashboard is a public-facing MkDocs documentation page (`docs/component-health.md`) that shows developers migrating from Web Forms exactly how mature each BWFC component is. A PowerShell scanner generates the data; MkDocs renders it.

---

## 2. Scoring Dimensions & Weights

Each component receives a **Health Score** from 0–100%. The score is a weighted sum of seven dimensions:

| # | Dimension | Weight | What It Measures |
|---|-----------|--------|------------------|
| 1 | **Property Parity** | **30%** | `[Parameter]` properties implemented vs. expected Web Forms properties |
| 2 | **HTML Output Fidelity** | **15%** | Known divergences from DIVERGENCE-REGISTRY affecting this component |
| 3 | **bUnit Test Coverage** | **15%** | Number of bUnit test files exercising this component |
| 4 | **Integration Test Coverage** | **5%** | Playwright E2E tests covering this component (currently 0 — placeholder for growth) |
| 5 | **Style Support** | **15%** | Inherits BaseStyledComponent? Implements IStyle? Renders AccessKey/ToolTip? |
| 6 | **Sample Page Exists** | **10%** | Component has a sample page in AfterBlazorServerSide ControlSamples |
| 7 | **Event Support** | **10%** | Web Forms events implemented as EventCallback parameters |

**Total: 100%**

### Rationale for Weights

- **Property Parity (30%):** This is the #1 migration concern. If a developer's markup uses `<asp:GridView AllowPaging="true">` and we don't have `AllowPaging`, they're stuck. Highest weight.
- **HTML Output Fidelity (15%):** CSS and JS compatibility depend on matching output. Divergences tracked in D-01 through D-14 directly impact migration success.
- **bUnit Tests (15%):** Tests prove the component works. No tests = no confidence.
- **Integration Tests (5%):** Low weight because Playwright infrastructure doesn't exist yet. When it does, this weight can increase (steal from bUnit).
- **Style Support (15%):** Web Forms developers expect `CssClass`, `ForeColor`, `BackColor`, `Font-*`, `Width`, `Height` on every `WebControl`. Missing style support breaks every styled page.
- **Sample Page (10%):** Docs/samples ship with components (team decision). A missing sample means the component isn't "finished."
- **Event Support (10%):** Web Forms is event-driven. Missing `OnClick`, `OnSelectedIndexChanged`, etc. blocks behavioral migration.

---

## 3. Dimension Scoring Rules

### 3.1 Property Parity (30%)

**Formula:** `(implemented_parameter_count / expected_parameter_count) × 100`, capped at 100%.

**How to count "implemented":** Scan the component's `.razor.cs` file for `[Parameter]` attributes. Count each unique property name. Include properties inherited from the component's base class chain (BaseStyledComponent adds 9, BaseWebFormsComponent adds ~8 meaningful ones, ButtonBaseComponent adds 7, DataBoundComponent adds 5, BaseListControl adds 5).

**How to count "expected":** Use the reference data in Section 5 below. This is the denominator — the Web Forms properties that a migrating developer would expect.

**Inherited properties count:** Properties from base classes (ID, CssClass, Enabled, Visible, etc.) count as implemented for ALL components that inherit them. This prevents penalizing every component for base-class work.

**Exclusions:** Skip `[Obsolete]` parameters (`runat`, `EnableViewState`, `DataSourceID`, `DataKeys`, `ItemPlaceholderID`). Skip `AdditionalAttributes`, `ChildContent`, `ChildComponents` — these are Blazor infrastructure, not Web Forms properties.

### 3.2 HTML Output Fidelity (15%)

**Formula:** `max(0, 100 - (divergence_penalty))` where each applicable divergence entry deducts points.

**Divergence penalties:**
| Category | Penalty | Rationale |
|----------|---------|-----------|
| **Structural** (D-13: missing HTML elements) | **-25 each** | Missing DOM nodes break CSS and JS |
| **Style** (D-14: styles not applied) | **-15 each** | Visual regression but structure intact |
| **ID Generation** (D-11: GUID IDs) | **-10 each** | Fixable bug, workaround exists |
| **Attribute Format** (D-12: boolean attrs) | **-0** | Both valid HTML5, no impact |
| **Infrastructure** (D-01, D-02, D-03, D-04, D-09) | **-0** | Platform-level, intentional, no fix possible |
| **JS Interop** (D-07, D-08, D-10) | **-0** | Intentional architectural change |
| **Rendering Mode** (D-05, D-06) | **-5 each** | Intentional but CSS impact |

**Divergence-to-component mapping (from DIVERGENCE-REGISTRY):**

| Component | Applicable Divergences | Penalty |
|-----------|----------------------|---------|
| Calendar | D-13 (-25), D-14 (-15) | -40 → **60%** |
| CheckBox | D-11 (-10) | -10 → **90%** |
| RadioButton | D-11 (-10) | -10 → **90%** |
| RadioButtonList | D-11 (-10) | -10 → **90%** |
| FileUpload | D-11 (-10) | -10 → **90%** |
| Menu | D-06 (-5) | -5 → **95%** |
| Chart | D-05 (-5) | -5 → **95%** |
| All others | None applicable | **100%** |

**Note:** D-01 through D-04, D-07 through D-10 are infrastructure/platform divergences — they affect ALL controls equally and are intentional. They score 0 penalty because they're not bugs or component-level gaps.

### 3.3 bUnit Test Coverage (15%)

**Formula:** `min(100, (test_file_count / expected_test_threshold) × 100)`

**Expected test thresholds by complexity tier:**

| Tier | Components | Threshold |
|------|-----------|-----------|
| **Complex** (data-bound, CRUD, templates) | GridView, ListView, FormView, DetailsView, DataGrid, DataList, Menu, TreeView, Calendar, Chart | **10 test files** |
| **Medium** (interactive, events, children) | Button, CheckBox, RadioButton, DropDownList, ListBox, CheckBoxList, RadioButtonList, TextBox, FileUpload, ImageMap, BulletedList, Repeater, DataPager, Table | **5 test files** |
| **Simple** (display, wrapper, infrastructure) | Label, Literal, Image, HyperLink, Panel, PlaceHolder, HiddenField, LinkButton, ImageButton, AdRotator, MultiView, Timer, SiteMapPath, UpdatePanel, UpdateProgress, ScriptManager | **3 test files** |
| **Login** (visual shells) | Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery | **3 test files** |
| **Validation** | RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator, ValidationSummary | **3 test files** |

### 3.4 Integration Test Coverage (5%)

**Formula:** `min(100, (playwright_test_count / 1) × 100)` — binary for now (0% or 100%).

**Current state:** No Playwright infrastructure exists. Every component scores **0%** on this dimension. When Playwright tests are added, the scanner should look for `*.spec.ts` files referencing the component name.

**Future adjustment:** When Playwright tests exist for ≥10 components, increase this weight to 10% and reduce bUnit to 10%.

### 3.5 Style Support (15%)

**Scoring rubric (additive, max 100%):**

| Criterion | Points | How to Detect |
|-----------|--------|---------------|
| Inherits `BaseStyledComponent` (or descendant like `DataBoundComponent`) | **50** | Check class hierarchy in `.razor.cs` |
| Implements `IStyle` interface | **20** | Check interface list (BaseStyledComponent does this automatically) |
| Renders `AccessKey` attribute | **10** | Check if component inherits `BaseWebFormsComponent` (which has `AccessKey` parameter) |
| Renders `ToolTip` as `title` attribute | **10** | Check if component inherits `BaseWebFormsComponent` (which has `ToolTip` parameter) |
| Has sub-style components (e.g., `GridViewHeaderStyle`) | **10** | Check for companion `*Style.razor` files in the project |

**Components that inherit BaseStyledComponent** get at least 90% automatically (50 + 20 + 10 + 10 from base class chain). Those with sub-styles get 100%.

**Components that inherit only BaseWebFormsComponent** (Literal, HiddenField, PlaceHolder, MultiView, ScriptManager) get **20%** (AccessKey 10 + ToolTip 10). This is intentional — these controls don't render styled HTML in Web Forms either (e.g., `<asp:Literal>` has no style properties).

### 3.6 Sample Page Exists (10%)

**Formula:** Binary — **100%** if a directory exists at `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/{ComponentName}/`, **0%** otherwise.

**Current sample coverage (from scan):**

✅ Has sample: AdRotator, BulletedList, Button, Calendar, Chart, CheckBox, CheckBoxList, DataGrid, DataList, DataPager, DetailsView, DropDownList, FileUpload, FormView, GridView, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, ListView, Literal, Localize, Menu, MultiView, Panel, PlaceHolder, RadioButtonList, Repeater, ScriptManager, SiteMapPath, Substitution, Table, Timer, TreeView, UpdatePanel, UpdateProgress

✅ Has sample (Login): ChangePassword, CreateUserWizard, LoginView, PasswordRecovery, LoginControls (umbrella)

✅ Has sample (Other): Validations, MasterPage, Theming, ViewState, DataBinder

❌ Missing sample: RadioButton (individual — covered under RadioButtonList?), LoginName (individual), LoginStatus (individual)

**Note for scanner:** Some Login controls share the `LoginControls/` sample directory. The scanner should accept either `{ComponentName}/` or presence in a shared sample page (grep for component tag usage).

### 3.7 Event Support (10%)

**Formula:** `(implemented_event_count / expected_event_count) × 100`, capped at 100%.

**How to count "implemented":** Scan for `EventCallback` parameters in the `.razor.cs` file. Count each unique event. Include inherited events from base classes (OnClick, OnCommand from ButtonBaseComponent; OnDataBinding/OnDataBound from base classes; lifecycle events OnInit/OnLoad/OnPreRender/OnUnload/OnDisposed from BaseWebFormsComponent).

**How to count "expected":** Use the event reference data in Section 5 below.

**Exclusions:** Lifecycle events (OnInit, OnLoad, OnPreRender, OnUnload, OnDisposed) are already implemented in BaseWebFormsComponent for ALL components. Don't count them in either numerator or denominator — they'd inflate every score without meaning.

---

## 4. Computing the Health Score

### Formula

```
HealthScore = Σ (dimension_score × dimension_weight)
```

Where each `dimension_score` is 0–100 and each `dimension_weight` is from Section 2.

### Example: GridView

| Dimension | Score | Weight | Contribution |
|-----------|-------|--------|--------------|
| Property Parity | 40% (est. 12/30 properties) | 0.30 | 12.0 |
| HTML Fidelity | 100% (no component-specific divergences) | 0.15 | 15.0 |
| bUnit Tests | 100% (18 files, threshold 10) | 0.15 | 15.0 |
| Integration Tests | 0% (no Playwright) | 0.05 | 0.0 |
| Style Support | 100% (DataBoundComponent → BaseStyledComponent + sub-styles) | 0.15 | 15.0 |
| Sample Page | 100% (GridView/ exists) | 0.10 | 10.0 |
| Event Support | 50% (est. 5/10 events) | 0.10 | 5.0 |
| **TOTAL** | | | **72.0%** 🟡 |

### Example: Label

| Dimension | Score | Weight | Contribution |
|-----------|-------|--------|--------------|
| Property Parity | 90% (est. 9/10 properties) | 0.30 | 27.0 |
| HTML Fidelity | 100% | 0.15 | 15.0 |
| bUnit Tests | 100% (3 files, threshold 3) | 0.15 | 15.0 |
| Integration Tests | 0% | 0.05 | 0.0 |
| Style Support | 100% (BaseStyledComponent) | 0.15 | 15.0 |
| Sample Page | 100% | 0.10 | 10.0 |
| Event Support | 100% (no component-specific events expected) | 0.10 | 10.0 |
| **TOTAL** | | | **92.0%** 🟢 |

---

## 5. Component Inventory & Reference Data

### Category Definitions

| Category | Components |
|----------|-----------|
| **Data** | GridView, ListView, DataGrid, DataList, Repeater, FormView, DetailsView, DataPager |
| **Editor** | TextBox, DropDownList, ListBox, CheckBox, CheckBoxList, RadioButton, RadioButtonList, FileUpload, HiddenField, Calendar |
| **Display** | Label, Literal, Image, HyperLink, BulletedList, AdRotator, Table, Panel, PlaceHolder, Substitution |
| **Button** | Button, LinkButton, ImageButton |
| **Navigation** | Menu, TreeView, SiteMapPath, ImageMap |
| **Login** | Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery |
| **Validation** | RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator, ValidationSummary (AspNetValidationSummary) |
| **Infrastructure** | MultiView, View, UpdatePanel, UpdateProgress, ScriptManager, Timer, MasterPage, ContentPlaceHolder, Content, Page, WebFormsPage, NamingContainer |
| **Charting** | Chart, ChartArea, ChartSeries, ChartTitle, ChartLegend |

**Note for scanner:** Infrastructure and Charting components are included in the inventory but scored separately. Infrastructure controls are intentionally minimal (they're plumbing, not UI). Chart has a permanent architectural divergence (D-05).

### 5.1 Expected Properties & Events per Component

Below are the **top commonly-used properties** from ASP.NET Web Forms (.NET Framework 4.8) for each primary component. This is the denominator for Property Parity and Event Support scoring.

Properties inherited from WebControl base (`ID`, `CssClass`, `Enabled`, `Visible`, `TabIndex`, `AccessKey`, `ToolTip`, `BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Font`, `Height`, `Width`) are **not listed** below — they're tracked via Style Support dimension instead. Only component-specific properties appear.

---

#### DATA CONTROLS

**GridView**
- Properties: `AllowPaging`, `AllowSorting`, `AutoGenerateColumns`, `AutoGenerateDeleteButton`, `AutoGenerateEditButton`, `AutoGenerateSelectButton`, `Caption`, `CaptionAlign`, `CellPadding`, `CellSpacing`, `DataKeyNames`, `EditIndex`, `EmptyDataText`, `GridLines`, `HorizontalAlign`, `PageIndex`, `PageSize`, `SelectedIndex`, `ShowFooter`, `ShowHeader`, `ShowHeaderWhenEmpty`, `SortDirection`, `SortExpression`, `UseAccessibleHeader`
- Events: `OnPageIndexChanging`, `OnPageIndexChanged`, `OnRowCancelingEdit`, `OnRowCommand`, `OnRowCreated`, `OnRowDataBound`, `OnRowDeleting`, `OnRowEditing`, `OnRowUpdating`, `OnSelectedIndexChanged`, `OnSelectedIndexChanging`, `OnSorting`, `OnSorted`

**ListView**
- Properties: `DataKeyNames`, `EditIndex`, `InsertItemPosition`, `ItemPlaceholderID`, `LayoutTemplate`, `ItemTemplate`, `EditItemTemplate`, `InsertItemTemplate`, `EmptyDataTemplate`, `EmptyItemTemplate`, `SelectedIndex`, `GroupItemCount`, `GroupPlaceholderID`
- Events: `OnItemCanceling`, `OnItemCommand`, `OnItemCreated`, `OnItemDataBound`, `OnItemDeleting`, `OnItemEditing`, `OnItemInserted`, `OnItemInserting`, `OnItemUpdated`, `OnItemUpdating`, `OnLayoutCreated`, `OnPagePropertiesChanging`, `OnSelectedIndexChanging`, `OnSorting`

**DataGrid**
- Properties: `AllowPaging`, `AllowSorting`, `AllowCustomPaging`, `AutoGenerateColumns`, `CellPadding`, `CellSpacing`, `CurrentPageIndex`, `DataKeyField`, `DataKeys`, `EditItemIndex`, `GridLines`, `HorizontalAlign`, `PageCount`, `PageSize`, `SelectedIndex`, `ShowFooter`, `ShowHeader`, `VirtualItemCount`
- Events: `OnCancelCommand`, `OnDeleteCommand`, `OnEditCommand`, `OnItemCommand`, `OnItemCreated`, `OnItemDataBound`, `OnPageIndexChanged`, `OnSelectedIndexChanged`, `OnSortCommand`, `OnUpdateCommand`

**FormView**
- Properties: `AllowPaging`, `CellPadding`, `CellSpacing`, `DataKeyNames`, `DefaultMode`, `EditRowStyle`, `EmptyDataText`, `FooterText`, `GridLines`, `HeaderText`, `HorizontalAlign`, `InsertRowStyle`, `PageIndex`, `PagerSettings`
- Events: `OnItemCommand`, `OnItemCreated`, `OnItemDeleted`, `OnItemDeleting`, `OnItemInserted`, `OnItemInserting`, `OnItemUpdated`, `OnItemUpdating`, `OnModeChanged`, `OnModeChanging`, `OnPageIndexChanging`

**DetailsView**
- Properties: `AllowPaging`, `AutoGenerateDeleteButton`, `AutoGenerateEditButton`, `AutoGenerateInsertButton`, `AutoGenerateRows`, `CellPadding`, `CellSpacing`, `Caption`, `CaptionAlign`, `DataKeyNames`, `DefaultMode`, `EmptyDataText`, `GridLines`, `HeaderText`, `HorizontalAlign`, `FooterText`, `PageIndex`
- Events: `OnItemCommand`, `OnItemCreated`, `OnItemDeleted`, `OnItemDeleting`, `OnItemInserted`, `OnItemInserting`, `OnItemUpdated`, `OnItemUpdating`, `OnModeChanged`, `OnModeChanging`, `OnPageIndexChanging`

**DataList**
- Properties: `CellPadding`, `CellSpacing`, `DataKeyField`, `EditItemIndex`, `GridLines`, `HorizontalAlign`, `RepeatColumns`, `RepeatDirection`, `RepeatLayout`, `SelectedIndex`, `ShowFooter`, `ShowHeader`
- Events: `OnCancelCommand`, `OnDeleteCommand`, `OnEditCommand`, `OnItemCommand`, `OnItemCreated`, `OnItemDataBound`, `OnSelectedIndexChanged`, `OnUpdateCommand`

**Repeater**
- Properties: `DataMember`, `HeaderTemplate`, `ItemTemplate`, `AlternatingItemTemplate`, `SeparatorTemplate`, `FooterTemplate`
- Events: `OnItemCommand`, `OnItemCreated`, `OnItemDataBound`

**DataPager**
- Properties: `PageSize`, `PagedControlID`, `TotalRowCount`, `StartRowIndex`, `MaximumRows`, `QueryStringField`
- Events: (none specific — paging is field-driven)

---

#### EDITOR CONTROLS

**TextBox**
- Properties: `AutoCompleteType`, `AutoPostBack`, `Columns`, `MaxLength`, `Placeholder`, `ReadOnly`, `Rows`, `Text`, `TextMode`, `Wrap`, `CausesValidation`, `ValidationGroup`
- Events: `OnTextChanged`

**DropDownList**
- Properties: `AppendDataBoundItems`, `AutoPostBack`, `DataTextField`, `DataTextFormatString`, `DataValueField`, `SelectedIndex`, `SelectedItem`, `SelectedValue`, `CausesValidation`, `ValidationGroup`
- Events: `OnSelectedIndexChanged`

**ListBox**
- Properties: `AppendDataBoundItems`, `AutoPostBack`, `DataTextField`, `DataTextFormatString`, `DataValueField`, `Rows`, `SelectionMode`, `SelectedIndex`, `SelectedItem`, `SelectedValue`, `CausesValidation`, `ValidationGroup`
- Events: `OnSelectedIndexChanged`

**CheckBox**
- Properties: `AutoPostBack`, `Checked`, `Text`, `TextAlign`, `CausesValidation`, `ValidationGroup`
- Events: `OnCheckedChanged`

**CheckBoxList**
- Properties: `AppendDataBoundItems`, `AutoPostBack`, `CellPadding`, `CellSpacing`, `DataTextField`, `DataTextFormatString`, `DataValueField`, `RepeatColumns`, `RepeatDirection`, `RepeatLayout`, `TextAlign`
- Events: `OnSelectedIndexChanged`

**RadioButton**
- Properties: `AutoPostBack`, `Checked`, `GroupName`, `Text`, `TextAlign`, `CausesValidation`, `ValidationGroup`
- Events: `OnCheckedChanged`

**RadioButtonList**
- Properties: `AppendDataBoundItems`, `AutoPostBack`, `CellPadding`, `CellSpacing`, `DataTextField`, `DataTextFormatString`, `DataValueField`, `RepeatColumns`, `RepeatDirection`, `RepeatLayout`, `SelectedIndex`, `SelectedValue`, `TextAlign`
- Events: `OnSelectedIndexChanged`

**FileUpload**
- Properties: `AllowMultiple`, `HasFile`, `FileName`, `FileBytes`, `FileContent`, `PostedFile`
- Events: (none — Web Forms FileUpload has no server events; file access is via properties)

**HiddenField**
- Properties: `Value`
- Events: `OnValueChanged`

**Calendar**
- Properties: `Caption`, `CaptionAlign`, `CellPadding`, `CellSpacing`, `DayNameFormat`, `FirstDayOfWeek`, `NextMonthText`, `NextPrevFormat`, `OtherMonthDayStyle`, `PrevMonthText`, `SelectedDate`, `SelectedDates`, `SelectionMode`, `SelectMonthText`, `SelectWeekText`, `ShowDayHeader`, `ShowGridLines`, `ShowNextPrevMonth`, `ShowTitle`, `TitleFormat`, `TodaysDate`, `UseAccessibleHeader`, `VisibleDate`
- Events: `OnDayRender`, `OnSelectionChanged`, `OnVisibleMonthChanged`

---

#### DISPLAY CONTROLS

**Label**
- Properties: `AssociatedControlID`, `Text`
- Events: (none)

**Literal**
- Properties: `Mode`, `Text`
- Events: (none)

**Image**
- Properties: `AlternateText`, `DescriptionUrl`, `GenerateEmptyAlternateText`, `ImageAlign`, `ImageUrl`
- Events: (none)

**HyperLink**
- Properties: `ImageUrl`, `NavigateUrl`, `Target`, `Text`
- Events: (none)

**BulletedList**
- Properties: `BulletImageUrl`, `BulletStyle`, `DisplayMode`, `FirstBulletNumber`, `Target`
- Events: `OnClick`

**AdRotator**
- Properties: `AdvertisementFile`, `AlternateTextField`, `ImageUrlField`, `KeywordFilter`, `NavigateUrlField`, `Target`
- Events: `OnAdCreated`

**Table**
- Properties: `Caption`, `CaptionAlign`, `CellPadding`, `CellSpacing`, `GridLines`, `HorizontalAlign`
- Events: (none)

**Panel**
- Properties: `DefaultButton`, `Direction`, `GroupingText`, `HorizontalAlign`, `ScrollBars`, `Wrap`
- Events: (none)

**PlaceHolder**
- Properties: (none beyond base — it's a container)
- Events: (none)

**Substitution**
- Properties: `MethodName`
- Events: (none)

---

#### BUTTON CONTROLS

**Button**
- Properties: `CausesValidation`, `CommandArgument`, `CommandName`, `OnClientClick`, `PostBackUrl`, `Text`, `UseSubmitBehavior`, `ValidationGroup`
- Events: `OnClick`, `OnCommand`

**LinkButton**
- Properties: `CausesValidation`, `CommandArgument`, `CommandName`, `OnClientClick`, `PostBackUrl`, `Text`, `ValidationGroup`
- Events: `OnClick`, `OnCommand`

**ImageButton**
- Properties: `AlternateText`, `CausesValidation`, `CommandArgument`, `CommandName`, `DescriptionUrl`, `ImageAlign`, `ImageUrl`, `OnClientClick`, `PostBackUrl`, `ValidationGroup`
- Events: `OnClick`, `OnCommand`

---

#### NAVIGATION CONTROLS

**Menu**
- Properties: `DataSourceID`, `DisappearAfter`, `DynamicBottomSeparatorImageUrl`, `DynamicEnableDefaultPopOutImage`, `DynamicHorizontalOffset`, `DynamicPopOutImageUrl`, `DynamicTopSeparatorImageUrl`, `DynamicVerticalOffset`, `IncludeStyleBlock`, `MaximumDynamicDisplayLevels`, `Orientation`, `RenderingMode`, `ScrollDownImageUrl`, `ScrollDownText`, `ScrollUpImageUrl`, `ScrollUpText`, `SkipLinkText`, `StaticBottomSeparatorImageUrl`, `StaticDisplayLevels`, `StaticEnableDefaultPopOutImage`, `StaticPopOutImageUrl`, `StaticSubMenuIndent`, `StaticTopSeparatorImageUrl`, `Target`
- Events: `OnMenuItemClick`, `OnMenuItemDataBound`

**TreeView**
- Properties: `AutoGenerateDataBindings`, `CollapseImageUrl`, `CollapseImageToolTip`, `EnableClientScript`, `ExpandDepth`, `ExpandImageUrl`, `ExpandImageToolTip`, `ImageSet`, `LineImagesFolder`, `MaxDataBindDepth`, `NoExpandImageUrl`, `NodeIndent`, `NodeWrap`, `PathSeparator`, `PopulateNodesFromClient`, `ShowCheckBoxes`, `ShowExpandCollapse`, `ShowLines`, `SkipLinkText`, `Target`
- Events: `OnSelectedNodeChanged`, `OnTreeNodeCheckChanged`, `OnTreeNodeCollapsed`, `OnTreeNodeExpanded`, `OnTreeNodePopulate`

**SiteMapPath**
- Properties: `CurrentNodeStyle`, `NodeStyle`, `PathDirection`, `PathSeparator`, `PathSeparatorStyle`, `PathSeparatorTemplate`, `RenderCurrentNodeAsLink`, `RootNodeStyle`, `ShowToolTips`, `SiteMapProvider`, `SkipLinkText`
- Events: (none)

**ImageMap**
- Properties: `AlternateText`, `DescriptionUrl`, `HotSpotMode`, `ImageAlign`, `ImageUrl`, `Target`
- Events: `OnClick`

---

#### LOGIN CONTROLS

**Login**
- Properties: `CreateUserText`, `CreateUserUrl`, `DestinationPageUrl`, `DisplayRememberMe`, `FailureAction`, `FailureText`, `HelpPageIconUrl`, `HelpPageText`, `HelpPageUrl`, `InstructionText`, `LoginButtonText`, `LoginButtonType`, `Orientation`, `PasswordLabelText`, `PasswordRecoveryText`, `PasswordRecoveryUrl`, `PasswordRequiredErrorMessage`, `RememberMeSet`, `RememberMeText`, `TextLayout`, `TitleText`, `UserNameLabelText`, `UserNameRequiredErrorMessage`, `VisibleWhenLoggedIn`
- Events: `OnAuthenticate`, `OnLoggedIn`, `OnLoggingIn`, `OnLoginError`

**LoginName**
- Properties: `FormatString`
- Events: (none)

**LoginStatus**
- Properties: `LoggedInImageUrl`, `LoggedInText`, `LoggedOutImageUrl`, `LoggedOutText`, `LoginImageUrl`, `LoginText`, `LogoutAction`, `LogoutImageUrl`, `LogoutPageUrl`, `LogoutText`
- Events: `OnLoggedOut`, `OnLoggingOut`

**LoginView**
- Properties: `AnonymousTemplate`, `LoggedInTemplate`, `RoleGroups`
- Events: `OnViewChanged`, `OnViewChanging`

**ChangePassword**
- Properties: `CancelButtonText`, `CancelDestinationPageUrl`, `ChangePasswordButtonText`, `ChangePasswordFailureText`, `ChangePasswordTitleText`, `ConfirmNewPasswordLabelText`, `ConfirmPasswordCompareErrorMessage`, `ConfirmPasswordRequiredErrorMessage`, `ContinueButtonText`, `ContinueDestinationPageUrl`, `CreateUserText`, `CreateUserUrl`, `DisplayUserName`, `EditProfileText`, `EditProfileUrl`, `HelpPageIconUrl`, `HelpPageText`, `HelpPageUrl`, `InstructionText`, `NewPasswordLabelText`, `NewPasswordRegularExpression`, `NewPasswordRequiredErrorMessage`, `PasswordHintText`, `PasswordLabelText`, `PasswordRecoveryText`, `PasswordRecoveryUrl`, `PasswordRequiredErrorMessage`, `SuccessText`, `SuccessTitleText`, `UserNameLabelText`, `UserNameRequiredErrorMessage`
- Events: `OnCancelButtonClick`, `OnChangedPassword`, `OnChangePasswordError`, `OnChangingPassword`, `OnContinueButtonClick`

**CreateUserWizard**
- Properties: `CompleteSuccessText`, `ConfirmPasswordLabelText`, `ConfirmPasswordRequiredErrorMessage`, `ContinueDestinationPageUrl`, `CreateUserButtonText`, `DisableCreatedUser`, `DuplicateEmailErrorMessage`, `DuplicateUserNameErrorMessage`, `EditProfileText`, `EditProfileUrl`, `EmailLabelText`, `EmailRegularExpression`, `EmailRegularExpressionErrorMessage`, `EmailRequiredErrorMessage`, `HelpPageIconUrl`, `HelpPageText`, `HelpPageUrl`, `InstructionText`, `InvalidEmailErrorMessage`, `InvalidPasswordErrorMessage`, `InvalidQuestionErrorMessage`, `LoginCreatedUser`, `PasswordHintText`, `PasswordLabelText`, `PasswordRegularExpression`, `PasswordRegularExpressionErrorMessage`, `PasswordRequiredErrorMessage`, `QuestionLabelText`, `RequireEmail`, `UnknownErrorMessage`, `UserNameLabelText`, `UserNameRequiredErrorMessage`
- Events: `OnContinueButtonClick`, `OnCreatedUser`, `OnCreateUserError`, `OnCreatingUser`

**PasswordRecovery**
- Properties: `GeneralFailureText`, `HelpPageIconUrl`, `HelpPageText`, `HelpPageUrl`, `InstructionText`, `QuestionFailureText`, `QuestionInstructionText`, `QuestionLabelText`, `QuestionTitleText`, `SubmitButtonText`, `SuccessText`, `SuccessPageUrl`, `UserNameFailureText`, `UserNameInstructionText`, `UserNameLabelText`, `UserNameRequiredErrorMessage`, `UserNameTitleText`
- Events: `OnAnswerLookupError`, `OnSendingMail`, `OnSendMailError`, `OnUserLookupError`, `OnVerifyingAnswer`, `OnVerifyingUser`

---

#### VALIDATION CONTROLS

**RequiredFieldValidator**
- Properties: `ControlToValidate`, `Display`, `EnableClientScript`, `ErrorMessage`, `ForeColor`, `InitialValue`, `IsValid`, `SetFocusOnError`, `Text`, `ValidationGroup`
- Events: (none)

**CompareValidator**
- Properties: `ControlToCompare`, `ControlToValidate`, `Display`, `EnableClientScript`, `ErrorMessage`, `Operator`, `SetFocusOnError`, `Text`, `Type`, `ValidationGroup`, `ValueToCompare`
- Events: (none)

**RangeValidator**
- Properties: `ControlToValidate`, `Display`, `EnableClientScript`, `ErrorMessage`, `MaximumValue`, `MinimumValue`, `SetFocusOnError`, `Text`, `Type`, `ValidationGroup`
- Events: (none)

**RegularExpressionValidator**
- Properties: `ControlToValidate`, `Display`, `EnableClientScript`, `ErrorMessage`, `SetFocusOnError`, `Text`, `ValidationExpression`, `ValidationGroup`
- Events: (none)

**CustomValidator**
- Properties: `ClientValidationFunction`, `ControlToValidate`, `Display`, `EnableClientScript`, `ErrorMessage`, `SetFocusOnError`, `Text`, `ValidateEmptyText`, `ValidationGroup`
- Events: `OnServerValidate`

**ValidationSummary**
- Properties: `DisplayMode`, `EnableClientScript`, `ForeColor`, `HeaderText`, `ShowMessageBox`, `ShowSummary`, `ValidationGroup`
- Events: (none)

---

#### INFRASTRUCTURE CONTROLS

Infrastructure controls are scored but **displayed separately** in the dashboard. They intentionally have fewer properties because they're plumbing, not UI.

**MultiView** — Properties: `ActiveViewIndex`. Events: `OnActiveViewChanged`
**UpdatePanel** — Properties: `ContentTemplate`, `RenderMode`, `UpdateMode`, `ChildrenAsTriggers`. Events: (none)
**UpdateProgress** — Properties: `AssociatedUpdatePanelID`, `DisplayAfter`, `DynamicLayout`. Events: (none)
**Timer** — Properties: `Enabled`, `Interval`. Events: `OnTick`
**ScriptManager** — Properties: `EnablePartialRendering`, `EnablePageMethods`, `AsyncPostBackTimeout`. Events: (none)

---

## 6. Display Format — MkDocs Page Layout

### Page: `docs/component-health.md`

```markdown
# Component Health Dashboard

> Auto-generated by `scripts/Invoke-ComponentHealthScan.ps1`
> Last updated: {TIMESTAMP}

## Summary

| Metric | Value |
|--------|-------|
| Total Components | {COUNT} |
| Average Health | {AVG}% |
| 🟢 Healthy (>80%) | {GREEN_COUNT} |
| 🟡 Needs Work (50-80%) | {YELLOW_COUNT} |
| 🔴 Critical (<50%) | {RED_COUNT} |

## Components by Category

### Data Controls

| Component | Health | Props | Tests | Events | Sample | Status |
|-----------|--------|-------|-------|--------|--------|--------|
| GridView | 72% | 12/30 | 18 | 5/13 | ✅ | 🟡 |
| ListView | 68% | 10/14 | 17 | 8/14 | ✅ | 🟡 |
| ... | | | | | | |

### Editor Controls
(same table format)

### Display Controls
(same table format)

### Button Controls
(same table format)

### Navigation Controls
(same table format)

### Login Controls
(same table format)

### Validation Controls
(same table format)

### Infrastructure Controls
(same table format, note: intentionally minimal)

### Charting Controls
(same table format, note: permanent architectural divergence D-05)

## Scoring Methodology

(Brief explanation of the 7 dimensions and weights, linking to this spec)

## Known Divergences

(Summary table from DIVERGENCE-REGISTRY with D-XX entries affecting component scores)
```

### Visual Indicators

| Health Range | Indicator | Meaning |
|-------------|-----------|---------|
| **>80%** | 🟢 | Healthy — ready for production migration |
| **50–80%** | 🟡 | Needs Work — usable with known gaps |
| **<50%** | 🔴 | Critical — significant gaps, migration risk |

### Column Definitions

| Column | Content |
|--------|---------|
| **Component** | Name (linked to component docs page if it exists) |
| **Health** | Weighted score as percentage |
| **Props** | `{implemented}/{expected}` count |
| **Tests** | Total bUnit test file count |
| **Events** | `{implemented}/{expected}` count |
| **Sample** | ✅ or ❌ |
| **Status** | 🟢 🟡 🔴 indicator |

---

## 7. Scanner Implementation Notes (for Rogue)

### Script: `scripts/Invoke-ComponentHealthScan.ps1`

**Inputs:**
1. Component `.razor.cs` files in `src/BlazorWebFormsComponents/`
2. This spec's reference data (embed as hashtable or companion JSON)
3. Test directories in `src/BlazorWebFormsComponents.Test/`
4. Sample directories in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/`
5. DIVERGENCE-REGISTRY.md (parse divergence-to-component mapping)

**Outputs:**
1. `docs/component-health.md` — the MkDocs page
2. (Optional) `docs/component-health.json` — machine-readable scores for CI/CD gates

**Key implementation decisions:**
- Use regex to scan for `[Parameter]` attributes in `.razor.cs` files. Count unique property names.
- Walk base class chain by checking `class X : Y` declarations. Map known base classes to their inherited [Parameter] counts.
- Test count = count of `.razor` files in the test directory matching the component name.
- Divergence mapping is static (hardcoded from this spec's Section 3.2 table).
- Reference property/event counts are static (hardcoded from this spec's Section 5).

**Update frequency:** Run the scanner as part of the docs build pipeline. The page should regenerate on every PR merge to `dev`.

---

## 8. Future Enhancements

1. **Trend tracking** — Store historical scores in a JSON file and show ↑↓ arrows for score changes.
2. **Per-property detail pages** — Drill down from the summary table to see exactly which properties are implemented/missing.
3. **Playwright integration** — When E2E tests exist, increase integration test weight from 5% → 10%.
4. **CI gate** — Fail the build if any component drops below a configurable threshold (e.g., 40%).
5. **Web Forms comparison screenshots** — Side-by-side rendered output comparisons embedded in the dashboard.

---

*Forge out. This methodology gives developers an honest, data-driven view of where each component stands. No hand-waving, no vanity metrics — just the numbers that matter for migration.*
