# bUnit Test Framework Migration Plan

## Overview

This document outlines the migration plan for upgrading the BlazorWebFormsComponents test project from **bUnit 1.0.0-beta-10** (deprecated) to **bUnit 2.5.x** (current stable).

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

### Phase 1 Completion Notes
- ✅ Updated `BlazorWebFormsComponents.Test.csproj` - bUnit 1.0.0-beta-10 → 2.5.3
- ✅ Updated `_Imports.razor` - Added `AngleSharp.Dom` and `Bunit.TestDoubles`
- ✅ Package restore successful
- ⚠️ Build shows 412 errors (expected - test files need migration)

---

## Migration Scope

### Files Requiring Migration

| Category | Count | Complexity |
|----------|-------|------------|
| Test .razor files | **198** | Medium |
| _Imports.razor | 1 | Low |
| .csproj | 1 | Low |
| copilot-instructions.md | 1 | Low |
| **Total** | **201** | |

### Test Files by Component

| Component | File Count |
|-----------|------------|
| AdRotator | 6 |
| BaseWebFormsComponent | 4 |
| Button | 9 |
| DataBinder | 4 |
| DataList (FlowLayout) | 18 |
| DataList (TableLayout) | 23 |
| DataList (other) | 1 |
| FormView | 5 |
| GridView | 9 |
| HiddenField | 2 |
| HyperLink | 2 |
| Image | 4 |
| ImageButton | 6 |
| LinkButton | 3 |
| ListView | 9 |
| Literal | 3 |
| LoginControls | 24 |
| Repeater | 5 |
| TreeView | 15 |
| Validations | 33 |
| ViewState | 1 |

---

## Phase 1: Package and Configuration Updates

### 1.1 Update BlazorWebFormsComponents.Test.csproj

**Current:**
```xml
<PackageReference Include="bunit" Version="1.0.0-beta-10" />
```

**Updated:**
```xml
<PackageReference Include="bunit" Version="2.5.3" />
```

### 1.2 Update _Imports.razor

**Current:**
```razor
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.Extensions.DependencyInjection
@using Bunit
@using Shouldly
@using SharedSampleObjects.Models
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Validations
@using Xunit
@using static BlazorWebFormsComponents.Enums.ValidationSummaryDisplayMode
@using static BlazorWebFormsComponents.Enums.ValidationCompareOperator
@using static BlazorWebFormsComponents.Enums.ValidationDataType
```

**Updated:**
```razor
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.Extensions.DependencyInjection
@using AngleSharp.Dom
@using Bunit
@using Bunit.TestDoubles
@using Shouldly
@using SharedSampleObjects.Models
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Validations
@using Xunit
@using static BlazorWebFormsComponents.Enums.ValidationSummaryDisplayMode
@using static BlazorWebFormsComponents.Enums.ValidationCompareOperator
@using static BlazorWebFormsComponents.Enums.ValidationDataType
```

**Changes:**
- Add `@using AngleSharp.Dom` (for DOM element types)
- Add `@using Bunit.TestDoubles` (for fake services)

---

## Phase 2: Test File Migration Pattern

### Old Pattern (bUnit beta)

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

        // Assertions
        cut.Find("element").TextContent.ShouldBe("expected");
    }
}
```

### New Pattern (bUnit 2.x)

```razor
@inherits BunitContext

@code {
    [Fact]
    public void TestMethodName()
    {
        // Arrange & Act
        var cut = Render(@<MyComponent Parameter="value" />);

        // Assert
        cut.Find("element").TextContent.ShouldBe("expected");
    }
}
```

### Migration Checklist Per File

- [ ] Change `@inherits TestComponentBase` → `@inherits BunitContext`
- [ ] Remove `<Fixture Test="...">` wrapper
- [ ] Remove `<ComponentUnderTest>` wrapper
- [ ] Convert method signature: `void TestName(Fixture fixture)` → `[Fact] public void TestName()`
- [ ] Change `fixture.GetComponentUnderTest()` → `Render(@<Component />)`
- [ ] Move component markup from `<ComponentUnderTest>` into `Render(@<.../>)` call
- [ ] Move any `@code` state (fields, helper methods) to remain in `@code` block
- [ ] Verify test passes

---

## Phase 3: Migration Examples

### Example 1: Simple Component Test

**Before (Button/Click.razor):**
```razor
@inherits TestComponentBase

<Fixture Test="FirstTest">
    <ComponentUnderTest>
        <Button OnClick="OnClick">Click me!</Button>
    </ComponentUnderTest>
</Fixture>

@code {
    public string TheContent { get; set; } = "Not clicked yet!";

    void OnClick()
    {
        TheContent = "I've been clicked";
    }

    void FirstTest(Fixture fixture)
    {
        var cut = fixture.GetComponentUnderTest();
        TheContent.ShouldBe("Not clicked yet!");
        cut.Find("button").Click();
        TheContent.ShouldBe("I've been clicked");
    }
}
```

**After:**
```razor
@inherits BunitContext

@code {
    public string TheContent { get; set; } = "Not clicked yet!";

    void OnClick()
    {
        TheContent = "I've been clicked";
    }

    [Fact]
    public void Button_Click_UpdatesContent()
    {
        // Arrange & Act
        var cut = Render(@<Button OnClick="OnClick">Click me!</Button>);
        TheContent.ShouldBe("Not clicked yet!");

        // Act
        cut.Find("button").Click();

        // Assert
        TheContent.ShouldBe("I've been clicked");
    }
}
```

### Example 2: Data-Bound Component Test

**Before (DataList/FlowLayout/Simple.razor):**
```razor
@inherits TestComponentBase

<Fixture Test="FirstTest">
    <ComponentUnderTest>
        <DataList Items="Widget.SimpleWidgetList"
                  ItemType="Widget"
                  RepeatLayout="Flow"
                  Context="Item">
            <HeaderTemplate>My Widget List</HeaderTemplate>
            <ItemTemplate>@Item.Name</ItemTemplate>
        </DataList>
    </ComponentUnderTest>
</Fixture>

@code {
    void FirstTest(Fixture fixture)
    {
        var cut = fixture.GetComponentUnderTest();
        cut.FindAll("span").Count().ShouldBe(Widget.SimpleWidgetList.Length+2);
        cut.FindAll("span").Skip(1).First().TextContent.ShouldBe("My Widget List");
        cut.Find("span").HasAttribute("title").ShouldBeFalse();
    }
}
```

**After:**
```razor
@inherits BunitContext

@code {
    [Fact]
    public void DataList_FlowLayout_RendersSimpleList()
    {
        // Arrange & Act
        var cut = Render(
            @<DataList Items="Widget.SimpleWidgetList"
                       ItemType="Widget"
                       RepeatLayout="Flow"
                       Context="Item">
                <HeaderTemplate>My Widget List</HeaderTemplate>
                <ItemTemplate>@Item.Name</ItemTemplate>
            </DataList>
        );

        // Assert
        cut.FindAll("span").Count().ShouldBe(Widget.SimpleWidgetList.Length + 2);
        cut.FindAll("span").Skip(1).First().TextContent.ShouldBe("My Widget List");
        cut.Find("span").HasAttribute("title").ShouldBeFalse();
    }
}
```

### Example 3: Form Validation Test

**Before (Validations/RequiredFieldValidator/InputTextInvalidRequiredFieldValidator.razor):**
```razor
@inherits TestComponentBase

<Fixture Test="FirstTest">
    <ComponentUnderTest>
        <EditForm Model="@exampleModel" OnValidSubmit="@HandleValidSubmit" OnInvalidSubmit="@HandleInvalidSubmit">
            Write something
            <InputText @ref="Name.Current" @bind-Value="exampleModel.Name" />
            <RequiredFieldValidator Type="string"
                                    ControlToValidate="@Name"
                                    Text="Name is required." />
        </EditForm>
    </ComponentUnderTest>
</Fixture>

@code {
    bool _validSubmit = false;
    bool _invalidSubmit = false;
    ForwardRef<InputBase<string>> Name = new ForwardRef<InputBase<string>>();

    void FirstTest(Fixture fixture)
    {
        var cut = fixture.GetComponentUnderTest();
        cut.Find("input").Change("  ");
        cut.Find("form").Submit();
        _validSubmit.ShouldBeFalse();
        _invalidSubmit.ShouldBeTrue();
        cut.Find("span").FirstChild.TextContent.ShouldContain("Name is required.");
    }

    private ExampleModel exampleModel = new ExampleModel();
    private void HandleValidSubmit() => _validSubmit = true;
    private void HandleInvalidSubmit() => _invalidSubmit = true;

    public class ExampleModel
    {
        public string Name { get; set; }
    }
}
```

**After:**
```razor
@inherits BunitContext

@code {
    bool _validSubmit = false;
    bool _invalidSubmit = false;
    ForwardRef<InputBase<string>> Name = new ForwardRef<InputBase<string>>();
    private ExampleModel exampleModel = new ExampleModel();

    private void HandleValidSubmit() => _validSubmit = true;
    private void HandleInvalidSubmit() => _invalidSubmit = true;

    public class ExampleModel
    {
        public string Name { get; set; }
    }

    [Fact]
    public void RequiredFieldValidator_EmptyInput_ShowsErrorMessage()
    {
        // Arrange
        var cut = Render(
            @<EditForm Model="@exampleModel" OnValidSubmit="@HandleValidSubmit" OnInvalidSubmit="@HandleInvalidSubmit">
                Write something
                <InputText @ref="Name.Current" @bind-Value="exampleModel.Name" />
                <RequiredFieldValidator Type="string"
                                        ControlToValidate="@Name"
                                        Text="Name is required." />
            </EditForm>
        );

        // Act
        cut.Find("input").Change("  ");
        cut.Find("form").Submit();

        // Assert
        _validSubmit.ShouldBeFalse();
        _invalidSubmit.ShouldBeTrue();
        cut.Find("span").FirstChild.TextContent.ShouldContain("Name is required.");
    }
}
```

---

## Phase 4: Test Naming Conventions

### New Naming Standard

Follow the pattern: `ComponentName_Scenario_ExpectedBehavior`

**Examples:**
- `Button_Click_InvokesHandler`
- `DataList_EmptyItems_RendersEmptyTemplate`
- `RequiredFieldValidator_EmptyInput_ShowsErrorMessage`
- `GridView_WithDataSource_RendersAllRows`

### Rename During Migration

| Old Name | New Name |
|----------|----------|
| `FirstTest` | Descriptive name based on test purpose |
| `TestName` | `ComponentName_Scenario_ExpectedBehavior` |

---

## Phase 5: Execution Order

### Priority Order (by complexity and dependencies)

1. **Low complexity, foundational** (do first)
   - Button (9 files)
   - HiddenField (2 files)
   - HyperLink (2 files)
   - Image (4 files)
   - Literal (3 files)
   - LinkButton (3 files)

2. **Medium complexity**
   - ImageButton (6 files)
   - AdRotator (6 files)
   - BaseWebFormsComponent (4 files)
   - Repeater (5 files)
   - FormView (5 files)
   - ViewState (1 file)

3. **High complexity - Data Controls**
   - DataBinder (4 files)
   - DataList FlowLayout (18 files)
   - DataList TableLayout (23 files)
   - ListView (9 files)
   - GridView (9 files)

4. **High complexity - TreeView**
   - TreeView (15 files)

5. **High complexity - Validations**
   - RequiredFieldValidator (6 files)
   - RegularExpressionValidator (3 files)
   - CustomValidator (4 files)
   - CompareValidator (14 files)
   - RangeValidator (2 files)
   - ValidationSummary (4 files)

6. **High complexity - Login Controls**
   - LoginName (3 files)
   - LoginStatus (12 files)
   - LoginView (8 files)
   - Login (4 files)

---

## Phase 6: Copilot Instructions Update

Update `.github/copilot-instructions.md` testing section:

### Replace Testing Conventions Section

**Old:**
```markdown
### bUnit Test Pattern
```razor
@inherits TestComponentBase

<Fixture Test="TestName">
    <ComponentUnderTest>
        <Button OnClick="OnClick">Click me!</Button>
    </ComponentUnderTest>
</Fixture>

@code {
    void TestName(Fixture fixture)
    {
        // Given
        var cut = fixture.GetComponentUnderTest();

        // When
        cut.Find("button").Click();

        // Then
        TheContent.ShouldBe("expected value");
    }
}
```
```

**New:**
```markdown
### bUnit Test Pattern (v2.x)

Tests use bUnit v2.x with the `BunitContext` base class pattern:

```razor
@inherits BunitContext

@code {
    [Fact]
    public void ComponentName_Scenario_ExpectedBehavior()
    {
        // Arrange & Act
        var cut = Render(@<Button OnClick="OnClick">Click me!</Button>);

        // Act
        cut.Find("button").Click();

        // Assert
        TheContent.ShouldBe("expected value");
    }
}
```

### Test Naming Convention

Follow the pattern: `ComponentName_Scenario_ExpectedBehavior`

Examples:
- `Button_Click_InvokesHandler`
- `DataList_EmptyItems_RendersEmptyTemplate`
- `RequiredFieldValidator_EmptyInput_ShowsErrorMessage`

### Key bUnit v2.x Patterns

- Inherit from `BunitContext` (not `TestComponentBase`)
- Use `Render(@<Component />)` inline Razor template syntax
- Add `[Fact]` attribute to all test methods
- Test methods must be `public`
- Use Shouldly for assertions (`value.ShouldBe(expected)`)

### Registering Services

```razor
@code {
    [Fact]
    public void MyTest()
    {
        // Register services before rendering
        Services.AddSingleton<IMyService>(new MockMyService());

        var cut = Render(@<MyComponent />);
        // ...
    }
}
```

### Testing with Authentication

```razor
@code {
    [Fact]
    public void MyAuthTest()
    {
        // Setup fake authentication
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");
        authContext.SetRoles("Admin");

        var cut = Render(@<MyComponent />);
        // ...
    }
}
```
```

---

## Verification Steps

After each batch of migrations:

1. **Build the test project:**
   ```bash
   dotnet build src/BlazorWebFormsComponents.Test
   ```

2. **Run test discovery:**
   ```bash
   dotnet test src/BlazorWebFormsComponents.Test --list-tests
   ```

3. **Execute tests:**
   ```bash
   dotnet test src/BlazorWebFormsComponents.Test
   ```

4. **Verify test count matches expectations**

---

## Rollback Plan

If migration causes issues:

1. Revert package changes in `.csproj`
2. Revert `_Imports.razor`
3. Restore original test files from git

```bash
git checkout -- src/BlazorWebFormsComponents.Test/
```

---

## Success Criteria

- [ ] All 198 test files migrated to bUnit v2.x pattern
- [ ] `dotnet test` discovers and runs all tests
- [ ] All tests pass (or failures are documented as known issues)
- [ ] `_Imports.razor` updated with new using statements
- [ ] `.csproj` references bUnit 2.5.3
- [ ] Copilot instructions updated with new test patterns
- [ ] Documentation updated

---

## Timeline Estimate

| Phase | Estimated Time |
|-------|----------------|
| Phase 1: Package updates | 15 minutes |
| Phase 2-5: Test file migration (198 files) | 4-6 hours |
| Phase 6: Copilot instructions | 30 minutes |
| Testing & verification | 1 hour |
| **Total** | **6-8 hours** |

---

## Notes

- Some tests may have helper components (like `TestBubbleComponent.razor`) that are not test files - these should be left as-is
- The `ElementExtensions.cs` and `MockNavigationManager.cs` files are helper classes, not tests
- Consider running tests in batches to catch issues early
