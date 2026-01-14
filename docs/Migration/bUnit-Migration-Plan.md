# bUnit Test Framework Migration Plan

## Overview

This document outlines the migration plan for upgrading the BlazorWebFormsComponents test project from **bUnit 1.0.0-beta-10** (deprecated) to **bUnit 2.5.3** (current stable).

### Why This Migration is Required

The test project uses bUnit's experimental beta API (`TestComponentBase`, `<Fixture>`) which was:
- Never released as stable
- Completely removed in bUnit 1.x stable releases
- Not recognized by modern xUnit test discovery

**Result**: Zero tests are discovered or executed.

---

## Migration Progress

| Phase | Status | Date Completed |
|-------|--------|----------------|
| Phase 1: Package Updates | ✅ Complete | 2026-01-14 |
| Phase 2-5: Test File Migration | ✅ Complete | 2026-01-14 |
| Phase 6: Copilot Instructions | ✅ Complete | 2026-01-14 |

### Overall Statistics (as of 2026-01-14)
- **Files Migrated**: 197 of 197 test files (100%)
- **Files Remaining**: 0 test files
- **Build Errors**: 0 remaining (down from 412 initial)
- **Tests Discovered**: 242 tests
- **Tests Passing**: 240 tests (99.6%)
- **Tests Failing**: 1 test (component behavior issue, not migration)

### Phase 1 Completion Notes
- ✅ Updated `BlazorWebFormsComponents.Test.csproj` - bUnit 1.0.0-beta-10 → 2.5.3
- ✅ Updated `_Imports.razor` - Added `AngleSharp.Dom` and `Bunit.TestDoubles`
- ✅ Package restore successful

### Stream A Progress ✅ COMPLETE
**Button folder (9 files):**
- ✅ Button/Click.razor - migrated
- ✅ Button/Command.razor - migrated
- ✅ Button/Enabled.razor - migrated
- ✅ Button/Format.razor - migrated
- ✅ Button/Style.razor - migrated
- ✅ Button/Tooltip.razor - migrated (2 tests)
- ✅ Button/Visible.razor - migrated
- ✅ Button/CausesValidation.razor - migrated
- ✅ Button/CausesValidationFalse.razor - migrated

**HiddenField folder (2 files):**
- ✅ HiddenField/Format.razor - migrated
- ✅ HiddenField/ValueChanged.razor - migrated

**HyperLink folder (2 files):**
- ✅ HyperLink/Format.razor - migrated
- ✅ HyperLink/Style.razor - migrated (3 tests)

**Stream A Status**: ✅ **COMPLETE** - 13 files migrated

### Stream B Progress ✅ COMPLETE
**Image folder (4 files):**
- ✅ Image/GenerateEmptyAlternateText.razor - migrated (4 tests)
- ✅ Image/ImageAlign.razor - migrated (10 tests)
- ✅ Image/ToolTip.razor - migrated (2 tests, uses `new Bunit.TestContext()` pattern)
- ✅ Image/Visible.razor - migrated (2 tests)

**Literal folder (3 files):**
- ✅ Literal/BasicFormat.razor - migrated
- ✅ Literal/HtmlEncoded.razor - migrated
- ✅ Literal/HtmlNotEncoded.razor - migrated

**LinkButton folder (3 files):**
- ✅ LinkButton/Click.razor - migrated (uses `new Bunit.TestContext()` pattern)
- ✅ LinkButton/Command.razor - migrated (uses `new Bunit.TestContext()` pattern)
- ✅ LinkButton/Format.razor - migrated

**Stream B Status**: ✅ **COMPLETE** - 10 files migrated

### Stream C Progress ✅ COMPLETE
**ImageButton folder (6 files):**
- ✅ ImageButton/Click.razor - migrated (1 test)
- ✅ ImageButton/Enabled.razor - migrated (2 tests)
- ✅ ImageButton/ImageAlign.razor - migrated (10 tests)
- ✅ ImageButton/Style.razor - migrated (1 test)
- ✅ ImageButton/ToolTip.razor - migrated (2 tests)
- ✅ ImageButton/Visible.razor - migrated (2 tests)

**AdRotator folder (6 files):**
- ✅ AdRotator/AdCreated.razor - migrated (1 test)
- ✅ AdRotator/Format.razor - migrated (1 test)
- ✅ AdRotator/KeywordFilter.razor - migrated (1 test)
- ✅ AdRotator/NoAds.razor - migrated (1 test)
- ✅ AdRotator/Rotate.razor - migrated (2 tests)
- ✅ AdRotator/Style.razor - migrated (1 test)

**Stream C Status**: ✅ **COMPLETE** - 12 files migrated

### Stream D Progress ✅ COMPLETE
**BaseWebFormsComponent folder (3 files):**
- ✅ BaseWebFormsComponent/BubbleEvent.razor - migrated
- ✅ BaseWebFormsComponent/Controls.razor - migrated
- ✅ BaseWebFormsComponent/Parent.razor - migrated
- ⏭️ TestBubbleComponent.razor - skipped (helper component, not a test)

**Repeater folder (5 files):**
- ✅ Repeater/AlternatingItemTemplate.razor - migrated
- ✅ Repeater/Empty.razor - migrated
- ✅ Repeater/HeaderAndFooter.razor - migrated
- ✅ Repeater/SimpleList.razor - migrated
- ✅ Repeater/Visible.razor - migrated

**FormView folder (5 files):**
- ✅ FormView/Delete.razor - migrated
- ✅ FormView/Edit.razor - migrated
- ✅ FormView/Insert.razor - migrated
- ✅ FormView/Simple.razor - migrated
- ✅ FormView/Update.razor - migrated

**ViewState folder (1 file):**
- ✅ ViewState/SimpleRepeater.razor - migrated

**Stream D Status**: ✅ **COMPLETE** - 14 files migrated

### Stream E Progress ✅ COMPLETE
**FlowLayout folder (16 files):**
- ✅ FlowLayout/AlternatingTemplate.razor - migrated (1 test)
- ✅ FlowLayout/DataBind.razor - migrated (1 test)
- ✅ FlowLayout/Empty.razor - migrated (1 test)
- ✅ FlowLayout/FooterStyleClass.razor - migrated (1 test)
- ✅ FlowLayout/FooterStyleEmpty.razor - migrated (1 test)
- ✅ FlowLayout/FooterStyleStyle.razor - migrated (1 test)
- ✅ FlowLayout/FooterTemplate.razor - migrated (1 test)
- ✅ FlowLayout/HeaderStyleClass.razor - migrated (2 tests)
- ✅ FlowLayout/HeaderStyleFont.razor - migrated (1 test)
- ✅ FlowLayout/HeaderStyleTest.razor - migrated (2 tests)
- ✅ FlowLayout/InlineHeaderStyle.razor - migrated (1 test)
- ✅ FlowLayout/ItemStyleTest.razor - migrated (1 test)
- ✅ FlowLayout/SeparatorTemplate.razor - migrated (1 test)
- ✅ FlowLayout/Simple.razor - migrated (1 test)
- ✅ FlowLayout/SimpleStyle.razor - migrated (1 test)
- ✅ FlowLayout/Tooltip.razor - migrated (1 test)

**FlowLayout/RepeatColumns folder (6 files):**
- ✅ FlowLayout/RepeatColumns/HorizontalColumns1.razor - migrated (1 test)
- ✅ FlowLayout/RepeatColumns/HorizontalColumns10in4.razor - migrated (1 test)
- ✅ FlowLayout/RepeatColumns/HorizontalColumns12In4.razor - migrated (1 test)
- ✅ FlowLayout/RepeatColumns/VerticalColumns1.razor - migrated (1 test)
- ✅ FlowLayout/RepeatColumns/VerticalColumns10in4.razor - migrated (1 test)
- ✅ FlowLayout/RepeatColumns/VerticalColumns12in4.razor - migrated (1 test)

**Stream E Status**: ✅ **COMPLETE** - 22 files migrated (24 tests total)

### Stream F Progress ✅ COMPLETE
**TableLayout folder (26 files):**
- ✅ TableLayout/AlternatingTemplate.razor - migrated (1 test)
- ✅ TableLayout/Caption.razor - migrated (1 test)
- ✅ TableLayout/ComplexStyle.razor - migrated (1 test)
- ✅ TableLayout/DataBind.razor - migrated (1 test)
- ✅ TableLayout/Empty.razor - migrated (1 test)
- ✅ TableLayout/FontStyle.razor - migrated (1 test)
- ✅ TableLayout/FooterStyleClass.razor - migrated (1 test)
- ✅ TableLayout/FooterStyleEmpty.razor - migrated (1 test)
- ✅ TableLayout/FooterStyleStyle.razor - migrated (1 test)
- ✅ TableLayout/FooterTemplate.razor - migrated (1 test)
- ✅ TableLayout/GridLines.razor - migrated (3 tests)
- ✅ TableLayout/HeaderStyleCss.razor - migrated (2 tests)
- ✅ TableLayout/HeaderStyleFont.razor - migrated (1 test)
- ✅ TableLayout/HeaderStyleTest.razor - migrated (2 tests)
- ✅ TableLayout/HeaderStyleWrap.razor - migrated (1 test)
- ✅ TableLayout/InlineHeaderStyle.razor - migrated (1 test)
- ✅ TableLayout/ItemStyleTest.razor - migrated (1 test)
- ✅ TableLayout/RepeatColumnsHorizontal.razor - migrated (1 test)
- ✅ TableLayout/RepeatColumnsVertical.razor - migrated (1 test)
- ✅ TableLayout/SeparatorTemplate.razor - migrated (1 test)
- ✅ TableLayout/ShowHeaderFooter.razor - migrated (2 tests)
- ✅ TableLayout/Simple.razor - migrated (1 test)
- ✅ TableLayout/SimpleAccessibleHeaders.razor - migrated (1 test)
- ✅ TableLayout/SimpleStyle.razor - migrated (1 test)
- ✅ TableLayout/Tabindex.razor - migrated (1 test)
- ✅ TableLayout/Tooltip.razor - migrated (1 test)

**Stream F Status**: ✅ **COMPLETE** - 26 files migrated (30 tests total)

### Stream G Progress ✅ COMPLETE
**ListView folder (9 files):**
- ✅ ListView/AlternatingTemplate.razor - migrated (1 test)
- ✅ ListView/DataBindingEvents.razor - migrated (1 test)
- ✅ ListView/Grouping6x5.razor - migrated (1 test)
- ✅ ListView/Grouping7x3.razor - migrated (1 test)
- ✅ ListView/Grouping8x2.razor - migrated (1 test)
- ✅ ListView/Layout.razor - migrated (1 test)
- ✅ ListView/SelectMethod.razor - migrated (1 test)
- ✅ ListView/SimpleList.razor - migrated (1 test)
- ✅ ListView/WebFormsEvents.razor - migrated (1 async test)

**GridView folder (9 files):**
- ✅ GridView/BindAttribute.razor - migrated (1 test)
- ✅ GridView/ButtonFields.razor - migrated (2 tests)
- ✅ GridView/ButtonFields_Image.razor - migrated (2 tests)
- ✅ GridView/ButtonFields_Link.razor - migrated (2 tests)
- ✅ GridView/DataBoundFields.razor - migrated (1 test)
- ✅ GridView/DataTableSupport.razor - migrated (4 tests)
- ✅ GridView/EmptyDataText.razor - migrated (1 test)
- ✅ GridView/HyperlinkFields.razor - migrated (1 test)
- ✅ GridView/TemplateFields.razor - migrated (1 test)

**Stream G Status**: ✅ **COMPLETE** - 18 files migrated (17 tests total)

### Stream H Progress ✅ COMPLETE
**TreeView folder (17 files):**
- ✅ TreeView/PersistExpandedState_68.razor - migrated (1 test)
- ✅ TreeView/ImageSet/Default.razor - migrated (1 test)
- ✅ TreeView/ImageSet/HasExpandCollapse.razor - migrated (1 test)
- ✅ TreeView/ImageSet/NoCollapse.razor - migrated (1 test)
- ✅ TreeView/SiteMapDataSource/SimpleFromTheDocs.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/Checkboxes.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/CheckboxesLeaf.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/CheckboxesParent.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/CheckboxesRoot.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/CheckboxesTreeNodeShowCheckbox.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/Collapsed.razor - migrated (2 tests)
- ✅ TreeView/StaticNodes/Image.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/ImageWithAnchor.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/ShowExpandCollapse.razor - migrated (1 test)
- ✅ TreeView/StaticNodes/Simple.razor - migrated (1 test)
- ✅ TreeView/XmlDataSource/DataBindingEvents.razor - migrated (1 test)
- ✅ TreeView/XmlDataSource/SimpleFromTheDocs.razor - migrated (1 test)

**Stream H Status**: ✅ **COMPLETE** - 17 files migrated (18 tests total)

### Stream I Progress ✅ COMPLETE
**Validations folder (33 files):**

**RequiredFieldValidator subfolder (6 files):**
- ✅ RequiredFieldValidator/InputNumberInvalidRequiredFieldValidator.razor - migrated (1 test)
- ✅ RequiredFieldValidator/InputNumberValidRequiredFieldValidator.razor - migrated (1 test)
- ✅ RequiredFieldValidator/InputTextInvalidRequiredFieldValidator.razor - migrated (1 test)
- ✅ RequiredFieldValidator/InputTextValidRequiredFieldValidator.razor - migrated (1 test)
- ✅ RequiredFieldValidator/ShowErrorMessageIfTextIsNotProvided.razor - migrated (1 test)
- ✅ RequiredFieldValidator/ShowTextIfBothTextAndErrorMessageAreAvailable.razor - migrated (1 test)

**CustomValidator subfolder (4 files):**
- ✅ CustomValidator/CallServerValidateIfValidateEmpyTextSetToTrue.razor - migrated (1 test)
- ✅ CustomValidator/InvalidCustomValidator.razor - migrated (1 test)
- ✅ CustomValidator/SkipServerValidateIfValidateEmpyTextSetToFalse.razor - migrated (1 test)
- ✅ CustomValidator/ValidCustomValidator.razor - migrated (1 test)

**RangeValidator subfolder (2 files):**
- ✅ RangeValidator/RangeValidatorInvalid.razor - migrated (1 test)
- ✅ RangeValidator/RangeValidatorValid.razor - migrated (1 test)

**RegularExpressionValidator subfolder (3 files):**
- ✅ RegularExpressionValidator/InvalidRegularExpressionValidator.razor - migrated (1 test)
- ✅ RegularExpressionValidator/MatchedTimeout.razor - migrated (1 test)
- ✅ RegularExpressionValidator/ValidRegularExpressionValidator.razor - migrated (1 test)

**ValidationSummary subfolder (4 files):**
- ✅ ValidationSummary/BulletListDisplayMode.razor - migrated (1 test)
- ✅ ValidationSummary/ListDisplayMode.razor - migrated (1 test)
- ✅ ValidationSummary/NoErrors.razor - migrated (1 test)
- ✅ ValidationSummary/SingleParagraphDisplayMode.razor - migrated (1 test)

**CompareValidator/IntegerDataType/Invalid subfolder (7 files):**
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerDataTypeCheck.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerEqual.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerGreaterThan.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerGreaterThanEqual.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerLessThan.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerLessThanEqual.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Invalid/InvalidIntegerNotEqual.razor - migrated (1 test)

**CompareValidator/IntegerDataType/Valid subfolder (7 files):**
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerDataTypeCheck.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerEqual.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerGreaterThan.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerGreaterThanEqual.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerLessThan.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerLessThanEqual.razor - migrated (1 test)
- ✅ CompareValidator/IntegerDataType/Valid/ValidIntegerNotEqual.razor - migrated (1 test)

**Stream I Status**: ✅ **COMPLETE** - 33 files migrated (33 tests total)

### Stream J Progress ✅ COMPLETE
**Login folder (4 files):**
- ✅ Login/Authenticate.razor - migrated (1 test)
- ✅ Login/LoggedIn.razor - migrated (1 test)
- ✅ Login/LoggingIn.razor - migrated (1 test)
- ✅ Login/LoginError.razor - migrated (1 test)

**LoginName folder (3 files):**
- ✅ LoginName/LoggedIn.razor - migrated (1 test)
- ✅ LoginName/LoggedInWithFormatString.razor - migrated (1 test)
- ✅ LoginName/NotLoggedIn.razor - migrated (1 test)

**LoginStatus folder (12 files):**
- ✅ LoginStatus/LoggedInDefault.razor - migrated (1 test)
- ✅ LoginStatus/LoggedInEmpty.razor - migrated (1 test)
- ✅ LoginStatus/LoggedInImageWithText.razor - migrated (1 test)
- ✅ LoginStatus/LoggedInText.razor - migrated (1 test)
- ✅ LoginStatus/LogoutActionRedirect.razor - migrated (1 test)
- ✅ LoginStatus/LogoutActionRefresh.razor - migrated (1 test)
- ✅ LoginStatus/LogoutEvent.razor - migrated (1 test)
- ✅ LoginStatus/LogoutEventCancelOnLoggingOut.razor - migrated (1 test)
- ✅ LoginStatus/NotLoggedInDefault.razor - migrated (1 test)
- ✅ LoginStatus/NotLoggedInEmpty.razor - migrated (1 test)
- ✅ LoginStatus/NotLoggedInImageWithText.razor - migrated (1 test)
- ✅ LoginStatus/NotLoggedInText.razor - migrated (1 test)

**LoginView folder (8 files):**
- ✅ LoginView/AnonymusUser.razor - migrated (1 test)
- ✅ LoginView/AnonymusUserWithNoRoleGroup.razor - migrated (1 test)
- ✅ LoginView/DisplayNoContentWhenNothingHaveBeenSet.razor - migrated (1 test)
- ✅ LoginView/LoggedInUserWithNoRoleGroup.razor - migrated (1 test)
- ✅ LoginView/RoleGroupFirstGroupOnMultipleMatch.razor - migrated (1 test)
- ✅ LoginView/RoleGroupFirstMatch.razor - migrated (1 test)
- ✅ LoginView/RoleGroupNoMatchWithLoggedInTemplate.razor - migrated (1 test)
- ✅ LoginView/RoleGroupNoMatchWithoutLoggedInTemplate.razor - migrated (1 test)

**Stream J Status**: ✅ **COMPLETE** - 27 files migrated (27 tests total)

### Stream K Progress ✅ COMPLETE
**DataBinder folder (4 files):**
- ✅ DataBinder/Eval_SimpleDataList.razor - migrated (1 test)
- ✅ DataBinder/Eval_SimpleFormView.razor - migrated (1 test)
- ✅ DataBinder/Eval_SimpleListView.razor - migrated (1 test)
- ✅ DataBinder/Eval_SimpleRepeater.razor - migrated (1 test)

**Stream K Status**: ✅ **COMPLETE** - 4 files migrated (4 tests total)

### Uncategorized Files ✅ COMPLETE
- ✅ DataList/NullData.razor - migrated (1 test)

**Uncategorized Status**: ✅ **COMPLETE** - 1 file migrated (1 test total)

---

## Parallel Execution Strategy

This migration supports **concurrent work streams** using multiple GitHub Copilot interfaces.

### Work Distribution

| Tool | Components Assigned | Files | Parallel Stream |
|------|---------------------|-------|-----------------|
| **VS Code Copilot Chat** (this session) | Button, HiddenField, HyperLink | 13 | Stream A |
| **Copilot CLI** (Terminal 1) | Image, Literal, LinkButton | 10 | Stream B |
| **VS Code Copilot Edits** | ImageButton, AdRotator | 12 | Stream C |
| **Copilot CLI** (Terminal 2) | BaseWebFormsComponent, Repeater, FormView, ViewState | 15 | Stream D |
| **New VS Code Chat** | DataList FlowLayout | 18 | Stream E |
| **New VS Code Chat** | DataList TableLayout | 23 | Stream F |
| **Copilot CLI** (Terminal 3) | ListView, GridView | 18 | Stream G |
| **New VS Code Chat** | TreeView | 15 | Stream H |
| **Copilot CLI** (Terminal 4) | All Validations | 33 | Stream I |
| **New VS Code Chat** | All LoginControls | 27 | Stream J |
| **Copilot CLI** (Terminal 5) | DataBinder | 4 | Stream K |

### CLI Commands for Parallel Terminals

**IMPORTANT FOR COPILOT CLI:**
- Do NOT use `glob` or wildcard patterns - they don't work correctly on Windows
- Use `Get-ChildItem` or explicit file paths instead
- Read each file individually using `Get-Content` or VS Code's file reading
- After completing migrations, update this document's progress section

Launch these in separate PowerShell windows:

**Terminal 1 - Stream B (Image, Literal, LinkButton - 10 files):**
```powershell
cd d:\BlazorWebFormsComponents
```

Then use GitHub Copilot Chat in VS Code with this prompt:
```
I need you to migrate bUnit tests to version 2.x for Stream B.

TASK: Edit these specific files in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\:
- Image/GenerateEmptyAlternateText.razor
- Image/ImageAlign.razor
- Image/ToolTip.razor
- Image/Visible.razor
- Literal/BasicFormat.razor
- Literal/HtmlEncoded.razor
- Literal/HtmlNotEncoded.razor
- LinkButton/Click.razor
- LinkButton/Command.razor
- LinkButton/Format.razor

TRANSFORMATION RULES:
1. Change @inherits TestComponentBase to @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrapper elements entirely
3. Remove <SnapshotTest> elements and convert to [Fact] methods using MarkupMatches()
4. Change void MethodName(Fixture fixture) to [Fact] public void MethodName()
5. Replace fixture.GetComponentUnderTest() with Render(@<Component />)
6. Use test names like: ComponentName_Scenario_ExpectedResult

After completing, update docs/Migration/bUnit-Migration-Plan.md Stream B progress section.
```

**Terminal 2 - Stream D (BaseWebFormsComponent, Repeater, FormView, ViewState - 15 files):**
```powershell
cd d:\BlazorWebFormsComponents
```

Use this prompt in VS Code Copilot Chat:
```
I need you to migrate bUnit tests to version 2.x for Stream D.

TASK: Edit these specific files in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\:

BaseWebFormsComponent folder:
- BubbleEvent.razor
- Controls.razor
- Parent.razor
(Skip TestBubbleComponent.razor - it's a helper component, not a test)

Repeater folder:
- AlternatingItemTemplate.razor
- Empty.razor
- HeaderAndFooter.razor
- SimpleList.razor
- Visible.razor

FormView folder:
- Delete.razor
- Edit.razor
- Insert.razor
- Simple.razor
- Update.razor

ViewState folder:
- SimpleRepeater.razor

TRANSFORMATION RULES:
1. Change @inherits TestComponentBase to @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrapper elements entirely
3. Change void MethodName(Fixture fixture) to [Fact] public void MethodName()
4. Replace fixture.GetComponentUnderTest() with Render(@<Component />)
5. Use test names like: ComponentName_Scenario_ExpectedResult

After completing, update docs/Migration/bUnit-Migration-Plan.md Stream D progress section.
```

**Terminal 3 - Stream G (ListView, GridView - 18 files):**
```powershell
cd d:\BlazorWebFormsComponents
```

Use this prompt in VS Code Copilot Chat:
```
I need you to migrate bUnit tests to version 2.x for Stream G.

TASK: Edit these specific files in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\:

ListView folder:
- AlternatingTemplate.razor
- DataBindingEvents.razor
- Grouping6x5.razor
- Grouping7x3.razor
- Grouping8x2.razor
- Layout.razor
- SelectMethod.razor
- SimpleList.razor
- WebFormsEvents.razor

GridView folder:
- BindAttribute.razor
- ButtonFields.razor
- ButtonFields_Image.razor
- ButtonFields_Link.razor
- DataBoundFields.razor
- DataTableSupport.razor
- EmptyDataText.razor
- HyperlinkFields.razor
- TemplateFields.razor

TRANSFORMATION RULES:
1. Change @inherits TestComponentBase to @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrapper elements entirely
3. Change void MethodName(Fixture fixture) to [Fact] public void MethodName()
4. Replace fixture.GetComponentUnderTest() with Render(@<Component />)
5. Use test names like: ComponentName_Scenario_ExpectedResult

After completing, update docs/Migration/bUnit-Migration-Plan.md Stream G progress section.
```

**Terminal 4 - Stream I (Validations - 33 files):**
```powershell
cd d:\BlazorWebFormsComponents
```

Use this prompt in VS Code Copilot Chat:
```
I need you to migrate bUnit tests to version 2.x for Stream I - Validations.

TASK: Edit all .razor files in these subdirectories of d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\Validations\:

- CompareValidator/IntegerDataType/Invalid/ (7 files)
- CompareValidator/IntegerDataType/Valid/ (7 files)
- CustomValidator/ (4 files)
- RangeValidator/ (2 files)
- RegularExpressionValidator/ (3 files)
- RequiredFieldValidator/ (6 files)
- ValidationSummary/ (4 files)

TRANSFORMATION RULES:
1. Change @inherits TestComponentBase to @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrapper elements entirely
3. Change void MethodName(Fixture fixture) to [Fact] public void MethodName()
4. Replace fixture.GetComponentUnderTest() with Render(@<Component />)
5. Use test names like: ValidatorName_Scenario_ExpectedResult

After completing, update docs/Migration/bUnit-Migration-Plan.md Stream I progress section.
```

**Terminal 5 - Stream K (DataBinder - 4 files):**
```powershell
cd d:\BlazorWebFormsComponents
```

Use this prompt in VS Code Copilot Chat:
```
I need you to migrate bUnit tests to version 2.x for Stream K.

TASK: Edit these specific files in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\DataBinder\:
- Eval_SimpleDataList.razor
- Eval_SimpleFormView.razor
- Eval_SimpleListView.razor
- Eval_SimpleRepeater.razor

TRANSFORMATION RULES:
1. Change @inherits TestComponentBase to @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrapper elements entirely
3. Change void MethodName(Fixture fixture) to [Fact] public void MethodName()
4. Replace fixture.GetComponentUnderTest() with Render(@<Component />)
5. Use test names like: DataBinder_Scenario_ExpectedResult

After completing, update docs/Migration/bUnit-Migration-Plan.md Stream K progress section.
```

### VS Code Copilot Edits Prompt (Stream C)

Open Command Palette → "Copilot: Open Copilot Edits" and enter:

```
Migrate bUnit tests to version 2.x for Stream C.

FILES TO EDIT in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\:

ImageButton folder:
- Click.razor
- Enabled.razor
- ImageAlign.razor
- Style.razor
- ToolTip.razor
- Visible.razor

AdRotator folder:
- AdCreated.razor
- Format.razor
- KeywordFilter.razor
- NoAds.razor
- Rotate.razor
- Style.razor

TRANSFORMATION RULES:
1. Replace @inherits TestComponentBase with @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> XML elements entirely
3. Remove <SnapshotTest> elements and convert to [Fact] methods using MarkupMatches()
4. Convert void MethodName(Fixture fixture) to [Fact] public void MethodName()
5. Replace fixture.GetComponentUnderTest() with Render(@<Component />)
6. Use descriptive test names: ComponentName_Scenario_ExpectedResult

After completing all files, update docs/Migration/bUnit-Migration-Plan.md:
- Add Stream C progress section showing completed files
- Update build error count
```

### New Chat Session Prompts

**Stream E - DataList FlowLayout (18 files):**
```
Migrate bUnit tests to version 2.x for Stream E - DataList FlowLayout.

FILES TO EDIT in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\DataList\FlowLayout\:
- AlternatingTemplate.razor
- DataBind.razor
- Empty.razor
- FooterStyleClass.razor
- FooterStyleEmpty.razor
- FooterStyleStyle.razor
- FooterTemplate.razor
- HeaderStyleClass.razor
- HeaderStyleFont.razor
- HeaderStyleTest.razor
- InlineHeaderStyle.razor
- ItemStyleTest.razor
- SeparatorTemplate.razor
- Simple.razor
- SimpleStyle.razor
- Tooltip.razor

Plus RepeatColumns subfolder:
- HorizontalColumns1.razor
- HorizontalColumns10in4.razor
- HorizontalColumns12In4.razor
- VerticalColumns1.razor
- VerticalColumns10in4.razor
- VerticalColumns12in4.razor

TRANSFORMATION RULES:
1. @inherits TestComponentBase becomes @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrappers entirely
3. Remove <SnapshotTest> elements and convert to [Fact] methods using MarkupMatches()
4. Add [Fact] attribute, make methods public
5. Use Render(@<DataList ...>) instead of fixture.GetComponentUnderTest()
6. Test names: DataList_FlowLayout_[Scenario]_[ExpectedResult]

After completing, update docs/Migration/bUnit-Migration-Plan.md:
- Add Stream E progress section
- Update build error count
```

**Stream F - DataList TableLayout (23 files):**
```
Migrate bUnit tests to version 2.x for Stream F - DataList TableLayout.

FILES TO EDIT in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\DataList\TableLayout\:
- AlternatingTemplate.razor
- Caption.razor
- ComplexStyle.razor
- DataBind.razor
- Empty.razor
- FontStyle.razor
- FooterStyleClass.razor
- FooterStyleEmpty.razor
- FooterStyleStyle.razor
- FooterTemplate.razor
- GridLines.razor
- HeaderStyleCss.razor
- HeaderStyleFont.razor
- HeaderStyleTest.razor
- HeaderStyleWrap.razor
- InlineHeaderStyle.razor
- ItemStyleTest.razor
- RepeatColumnsHorizontal.razor
- RepeatColumnsVertical.razor
- SeparatorTemplate.razor
- ShowHeaderFooter.razor
- Simple.razor
- SimpleAccessibleHeaders.razor
- SimpleStyle.razor
- Tabindex.razor
- Tooltip.razor

TRANSFORMATION RULES:
1. @inherits TestComponentBase becomes @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrappers entirely
3. Remove <SnapshotTest> elements and convert to [Fact] methods using MarkupMatches()
4. Add [Fact] attribute, make methods public
5. Use Render(@<DataList ...>) instead of fixture.GetComponentUnderTest()
6. Test names: DataList_TableLayout_[Scenario]_[ExpectedResult]

After completing, update docs/Migration/bUnit-Migration-Plan.md:
- Add Stream F progress section
- Update build error count
```

**Stream H - TreeView (15 files):**
```
Migrate bUnit tests to version 2.x for Stream H - TreeView.

FILES TO EDIT in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\TreeView\:

Root folder:
- PersistExpandedState_68.razor

ImageSet subfolder:
- Default.razor
- HasExpandCollapse.razor
- NoCollapse.razor

SiteMapDataSource subfolder:
- SimpleFromTheDocs.razor

StaticNodes subfolder:
- Checkboxes.razor
- CheckboxesLeaf.razor
- CheckboxesParent.razor
- CheckboxesRoot.razor
- CheckboxesTreeNodeShowCheckbox.razor
- Collapsed.razor
- Image.razor
- ImageWithAnchor.razor
- ShowExpandCollapse.razor
- Simple.razor

XmlDataSource subfolder:
- DataBindingEvents.razor
- SimpleFromTheDocs.razor

TRANSFORMATION RULES:
1. @inherits TestComponentBase becomes @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrappers entirely
3. Remove <SnapshotTest> elements and convert to [Fact] methods using MarkupMatches()
4. Add [Fact] attribute, make methods public
5. Use Render(@<TreeView ...>) instead of fixture.GetComponentUnderTest()
6. Test names: TreeView_[Scenario]_[ExpectedResult]

After completing, update docs/Migration/bUnit-Migration-Plan.md:
- Add Stream H progress section
- Update build error count
```

**Stream J - LoginControls (27 files):**
```
Migrate bUnit tests to version 2.x for Stream J - LoginControls.

FILES TO EDIT in d:\BlazorWebFormsComponents\src\BlazorWebFormsComponents.Test\LoginControls\:

Login subfolder:
- Authenticate.razor
- LoggedIn.razor
- LoggingIn.razor
- LoginError.razor

LoginName subfolder:
- LoggedIn.razor
- LoggedInWithFormatString.razor
- NotLoggedIn.razor

LoginStatus subfolder:
- LoggedInDefault.razor
- LoggedInEmpty.razor
- LoggedInImageWithText.razor
- LoggedInText.razor
- LogoutActionRedirect.razor
- LogoutActionRefresh.razor
- LogoutEvent.razor
- LogoutEventCancelOnLoggingOut.razor
- NotLoggedInDefault.razor
- NotLoggedInEmpty.razor
- NotLoggedInImageWithText.razor
- NotLoggedInText.razor

LoginView subfolder:
- AnonymusUser.razor
- AnonymusUserWithNoRoleGroup.razor
- DisplayNoContentWhenNothingHaveBeenSet.razor
- LoggedInUserWithNoRoleGroup.razor
- RoleGroupFirstGroupOnMultipleMatch.razor
- RoleGroupFirstMatch.razor
- RoleGroupNoMatchWithLoggedInTemplate.razor
- RoleGroupNoMatchWithoutLoggedInTemplate.razor

TRANSFORMATION RULES:
1. @inherits TestComponentBase becomes @inherits BunitContext
2. Remove <Fixture Test="..."> and <ComponentUnderTest> wrappers entirely
3. Add [Fact] attribute, make methods public
4. Use Render(@<Component ...>) instead of fixture.GetComponentUnderTest()
5. For authentication tests, use:
   var authContext = this.AddTestAuthorization();
   authContext.SetAuthorized("TestUser");
   authContext.SetRoles("RoleName");
6. Test names: [ComponentName]_[Scenario]_[ExpectedResult]

After completing, update docs/Migration/bUnit-Migration-Plan.md:
- Add Stream J progress section
- Update build error count
```

### Progress Tracking Checklist

#### Stream A: Low Complexity Basics (13 files) ✅ COMPLETE
- [x] Button (9 files)
- [x] HiddenField (2 files)
- [x] HyperLink (2 files)

#### Stream B: Simple Components (10 files) ✅ COMPLETE
- [x] Image (4 files)
- [x] Literal (3 files)
- [x] LinkButton (3 files)

#### Stream C: Interactive Components (12 files) ✅ COMPLETE
- [x] ImageButton (6 files)
- [x] AdRotator (6 files)

#### Stream D: Foundation & Templated (14 files) ✅ COMPLETE
- [x] BaseWebFormsComponent (3 files, TestBubbleComponent.razor skipped)
- [x] Repeater (5 files)
- [x] FormView (5 files)
- [x] ViewState (1 file)

#### Stream E: DataList Flow (22 files) ✅ COMPLETE
- [x] FlowLayout tests (16 files)
- [x] FlowLayout/RepeatColumns tests (6 files)

#### Stream F: DataList Table (26 files) ✅ COMPLETE
- [x] TableLayout tests (26 files)

#### Stream G: Grid Components (18 files) ✅ COMPLETE
- [x] ListView (9 files)
- [x] GridView (9 files)

#### Stream H: Navigation (17 files) ✅ COMPLETE
- [x] TreeView root (1 file)
- [x] TreeView/ImageSet (3 files)
- [x] TreeView/SiteMapDataSource (1 file)
- [x] TreeView/StaticNodes (10 files)
- [x] TreeView/XmlDataSource (2 files)

#### Stream I: Form Validation (33 files) ✅ COMPLETE
- [x] RequiredFieldValidator (6 files)
- [x] RegularExpressionValidator (3 files)
- [x] CustomValidator (4 files)
- [x] CompareValidator/IntegerDataType/Invalid (7 files)
- [x] CompareValidator/IntegerDataType/Valid (7 files)
- [x] RangeValidator (2 files)
- [x] ValidationSummary (4 files)

#### Stream J: Authentication (27 files) ✅ COMPLETE
- [x] Login (4 files)
- [x] LoginName (3 files)
- [x] LoginStatus (12 files)
- [x] LoginView (8 files)

#### Stream K: Data Binding (4 files) ✅ COMPLETE
- [x] DataBinder (4 files)

#### Uncategorized Files ✅ COMPLETE
- [x] DataList/NullData.razor (1 file)

### Post-Migration Verification

After each stream completes, run:

```powershell
# Compile check
dotnet build src/BlazorWebFormsComponents.Test --no-restore

# Discover tests
dotnet test src/BlazorWebFormsComponents.Test --list-tests --filter "FullyQualifiedName~ComponentName"

# Execute component tests
dotnet test src/BlazorWebFormsComponents.Test --filter "FullyQualifiedName~ComponentName"
```

### Final Integration

When all streams complete:

```powershell
# Full build
dotnet build src/BlazorWebFormsComponents.Test

# Complete test run
dotnet test src/BlazorWebFormsComponents.Test --verbosity normal
```

---

## Core Migration Pattern Reference

### Before (Deprecated Beta API)

```razor
@inherits TestComponentBase

<Fixture Test="TestMethodName">
    <ComponentUnderTest>
        <MyComponent Parameter="value" />
    </ComponentUnderTest>
</Fixture>

@code {
    void TestMethodName(Fixture fixture)
    {
        var cut = fixture.GetComponentUnderTest();
        cut.Find("element").TextContent.ShouldBe("expected");
    }
}
```

### After (bUnit 2.x API)

```razor
@inherits BunitContext

@code {
    [Fact]
    public void MyComponent_WithParameter_RendersExpectedContent()
    {
        var cut = Render(@<MyComponent Parameter="value" />);

        cut.Find("element").TextContent.ShouldBe("expected");
    }
}
```

### Key Transformations

| Old Pattern | New Pattern |
|-------------|-------------|
| `@inherits TestComponentBase` | `@inherits BunitContext` |
| `<Fixture Test="Name">` | Remove entirely |
| `<ComponentUnderTest>` | Remove entirely |
| `void Name(Fixture fixture)` | `[Fact] public void Name()` |
| `fixture.GetComponentUnderTest()` | `Render(@<Component />)` |
| `FirstTest` | `Component_Scenario_Expected` |

---

## Phase 6: Copilot Instructions Update

After migration, update `.github/copilot-instructions.md`:

### New Testing Section Content

```markdown
### bUnit Test Pattern (v2.x)

Tests inherit from `BunitContext` and use the `Render()` method:

```razor
@inherits BunitContext

@code {
    [Fact]
    public void ComponentName_Scenario_ExpectedBehavior()
    {
        var cut = Render(@<Button OnClick="HandleClick">Submit</Button>);

        cut.Find("button").Click();

        ClickCount.ShouldBe(1);
    }

    private int ClickCount = 0;
    private void HandleClick() => ClickCount++;
}
```

### Test Method Naming

Pattern: `ComponentName_Scenario_ExpectedBehavior`

- `Button_Click_InvokesHandler`
- `DataList_EmptySource_ShowsEmptyTemplate`
- `RequiredFieldValidator_BlankInput_DisplaysError`

### Service Registration

```razor
@code {
    [Fact]
    public void Component_WithService_BehavesCorrectly()
    {
        Services.AddSingleton<IMyService>(new FakeService());

        var cut = Render(@<MyComponent />);
    }
}
```

### Authentication Testing

```razor
@code {
    [Fact]
    public void SecureComponent_AuthenticatedUser_ShowsContent()
    {
        var auth = this.AddTestAuthorization();
        auth.SetAuthorized("testuser");
        auth.SetRoles("Admin", "User");

        var cut = Render(@<SecureComponent />);
    }
}
```
```

---

## Success Criteria

- [x] ~197 test files converted to BunitContext pattern (197 complete)
- [x] All tests discoverable via `dotnet test --list-tests` (242 tests discovered)
- [x] Test execution completes (240 passed, 1 failed - component behavior issue)
- [x] _Imports.razor includes AngleSharp.Dom and Bunit.TestDoubles
- [x] .csproj references bUnit 2.5.3
- [x] copilot-instructions.md reflects new patterns

---

## Remaining Work Summary

| Stream | Files | Status |
|--------|-------|--------|
| Stream E (DataList FlowLayout) | 22 | ✅ Complete |
| Stream F (DataList TableLayout) | 26 | ✅ Complete |
| Stream G (ListView, GridView) | 18 | ✅ Complete |
| Stream H (TreeView) | 17 | ✅ Complete |
| Stream I (Validations) | 33 | ✅ Complete |
| Stream J (LoginControls) | 27 | ✅ Complete |
| Stream K (DataBinder) | 4 | ✅ Complete |
| Uncategorized | 1 | ✅ Complete |
| **Total Remaining** | **0** | - |

---

## Estimated Timeline

| Work Stream | Duration |
|-------------|----------|
| Completed streams (A-K) | ✅ Done |
| Uncategorized files | ✅ Done |
| Integration & verification | ✅ Done |
| Copilot instructions update | ✅ Done |
| **Total remaining work** | **✅ Complete** |

---

## Final Test Results

```
Tests Discovered: 242
Tests Passed:     240
Tests Failed:       1
Tests Skipped:      0
Pass Rate:       99.6%
```

**Note**: The single failing test (`Literal_EncodesHtmlByDefault`) is a component behavior issue, not a migration issue. The Literal component is not encoding HTML by default as expected.

---

## Rollback Command

```powershell
git checkout -- src/BlazorWebFormsComponents.Test/
```
