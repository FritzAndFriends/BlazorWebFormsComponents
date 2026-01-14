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
| Phase 2-5: Test File Migration | 🔄 In Progress | - |
| Phase 6: Copilot Instructions | ⏳ Pending | - |

### Overall Statistics (as of 2026-01-14)
- **Files Migrated**: 51 of 197 test files (26%)
- **Files Remaining**: 146 test files
- **Build Errors**: 320 remaining (down from 412 initial)
- **Commit**: `aaadbcd` - "Refactor tests for BlazorWebFormsComponents to use Bunit framework"

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

### Stream G Progress (Partial)
**ListView folder:**
- ✅ ListView/AlternatingTemplate.razor - migrated
- ⏳ ListView/DataBindingEvents.razor - pending
- ⏳ ListView/Grouping6x5.razor - pending
- ⏳ ListView/Grouping7x3.razor - pending
- ⏳ ListView/Grouping8x2.razor - pending
- ⏳ ListView/Layout.razor - pending
- ⏳ ListView/SelectMethod.razor - pending
- ⏳ ListView/SimpleList.razor - pending
- ⏳ ListView/WebFormsEvents.razor - pending

**GridView folder:**
- ✅ GridView/BindAttribute.razor - migrated
- ⏳ GridView/ButtonFields.razor - pending
- ⏳ GridView/ButtonFields_Image.razor - pending
- ⏳ GridView/ButtonFields_Link.razor - pending
- ⏳ GridView/DataBoundFields.razor - pending
- ⏳ GridView/DataTableSupport.razor - pending
- ⏳ GridView/EmptyDataText.razor - pending
- ⏳ GridView/HyperlinkFields.razor - pending
- ⏳ GridView/TemplateFields.razor - pending

**Stream G Status**: 🔄 **IN PROGRESS** - 2 of 18 files migrated

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

#### Stream E: DataList Flow (22 files) ⏳ PENDING
- [ ] FlowLayout tests (16 files)
- [ ] FlowLayout/RepeatColumns tests (6 files)

#### Stream F: DataList Table (26 files) ⏳ PENDING
- [ ] TableLayout tests (26 files)

#### Stream G: Grid Components (18 files) 🔄 IN PROGRESS
- [ ] ListView (1/9 files migrated)
- [ ] GridView (1/9 files migrated)

#### Stream H: Navigation (17 files) ⏳ PENDING
- [ ] TreeView root (1 file)
- [ ] TreeView/ImageSet (3 files)
- [ ] TreeView/SiteMapDataSource (1 file)
- [ ] TreeView/StaticNodes (10 files)
- [ ] TreeView/XmlDataSource (2 files)

#### Stream I: Form Validation (33 files) ⏳ PENDING
- [ ] RequiredFieldValidator (6 files)
- [ ] RegularExpressionValidator (3 files)
- [ ] CustomValidator (4 files)
- [ ] CompareValidator/IntegerDataType/Invalid (7 files)
- [ ] CompareValidator/IntegerDataType/Valid (7 files)
- [ ] RangeValidator (2 files)
- [ ] ValidationSummary (4 files)

#### Stream J: Authentication (27 files) ⏳ PENDING
- [ ] Login (4 files)
- [ ] LoginName (3 files)
- [ ] LoginStatus (12 files)
- [ ] LoginView (8 files)

#### Stream K: Data Binding (4 files) ⏳ PENDING
- [ ] DataBinder (4 files)

#### Uncategorized Files ⏳ PENDING
- [ ] DataList/NullData.razor (1 file)

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

- [ ] ~197 test files converted to BunitContext pattern (51 complete, 146 remaining)
- [ ] All tests discoverable via `dotnet test --list-tests`
- [ ] Test execution completes (pass/fail documented)
- [x] _Imports.razor includes AngleSharp.Dom and Bunit.TestDoubles
- [x] .csproj references bUnit 2.5.3
- [ ] copilot-instructions.md reflects new patterns

---

## Remaining Work Summary

| Stream | Files | Status |
|--------|-------|--------|
| Stream E (DataList FlowLayout) | 22 | ⏳ Pending |
| Stream F (DataList TableLayout) | 26 | ⏳ Pending |
| Stream G (ListView, GridView) | 16 | 🔄 In Progress (2 done) |
| Stream H (TreeView) | 17 | ⏳ Pending |
| Stream I (Validations) | 33 | ⏳ Pending |
| Stream J (LoginControls) | 27 | ⏳ Pending |
| Stream K (DataBinder) | 4 | ⏳ Pending |
| Uncategorized | 1 | ⏳ Pending |
| **Total Remaining** | **146** | - |

---

## Estimated Timeline

| Work Stream | Duration |
|-------------|----------|
| Completed streams (A-D + partial G) | ✅ Done |
| Remaining streams (E-K) | ~2 hours |
| Integration & verification | 30 minutes |
| Copilot instructions update | 15 minutes |
| **Total remaining work** | **~2.5 hours** |

---

## Rollback Command

```powershell
git checkout -- src/BlazorWebFormsComponents.Test/
```
