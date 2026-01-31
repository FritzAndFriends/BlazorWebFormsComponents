---
name: bunit-test-migration
description: Migrate bUnit test files from deprecated beta API (1.0.0-beta-10) to bUnit 2.x stable API. Use this when working on .razor test files in BlazorWebFormsComponents.Test that contain old patterns like TestComponentBase, Fixture, or SnapshotTest.
---

# bUnit Test Migration Skill (Beta → 2.x)

This skill provides guidance for migrating test files from the deprecated bUnit 1.0.0-beta-10 API to bUnit 2.5.3 stable API. Use this when you encounter test files using the old `TestComponentBase`, `<Fixture>`, or `<SnapshotTest>` patterns.

## When to Apply

Apply this skill when a `.razor` test file contains any of these patterns:
- `@inherits TestComponentBase`
- `<Fixture Test="...">`
- `<ComponentUnderTest>`
- `<SnapshotTest>`
- `void MethodName(Fixture fixture)`

## Transformation Rules

### 1. Change Inheritance

```diff
- @inherits TestComponentBase
+ @inherits BunitContext
```

### 2. Remove Wrapper Elements

Remove these XML elements entirely (keep only the component inside):

```diff
- <Fixture Test="TestName">
-     <ComponentUnderTest>
          <MyComponent Parameter="value" />
-     </ComponentUnderTest>
- </Fixture>
```

### 3. Convert Test Methods

```diff
- void TestMethodName(Fixture fixture)
+ [Fact]
+ public void ComponentName_Scenario_ExpectedResult()
```

### 4. Replace Component Access

```diff
- var cut = fixture.GetComponentUnderTest();
+ var cut = Render(@<MyComponent Parameter="value" />);
```

### 5. Convert Snapshot Tests

```diff
- <SnapshotTest Description="renders correctly">
-     <TestInput>
-         <MyComponent />
-     </TestInput>
-     <ExpectedOutput>
-         <div>expected html</div>
-     </ExpectedOutput>
- </SnapshotTest>
+ [Fact]
+ public void MyComponent_Default_RendersCorrectly()
+ {
+     var cut = Render(@<MyComponent />);
+     cut.MarkupMatches(@<div>expected html</div>);
+ }
```

## Complete Example

### Before

```razor
@inherits TestComponentBase

<Fixture Test="ShouldClickButton">
    <ComponentUnderTest>
        <Button OnClick="OnClick">Click me</Button>
    </ComponentUnderTest>
</Fixture>

@code {
    int ClickCount = 0;

    void ShouldClickButton(Fixture fixture)
    {
        var cut = fixture.GetComponentUnderTest();

        cut.Find("button").Click();

        ClickCount.ShouldBe(1);
    }

    void OnClick() => ClickCount++;
}
```

### After

```razor
@inherits BunitContext

@code {
    int ClickCount = 0;

    [Fact]
    public void Button_Click_IncrementsCounter()
    {
        var cut = Render(@<Button OnClick="OnClick">Click me</Button>);

        cut.Find("button").Click();

        ClickCount.ShouldBe(1);
    }

    void OnClick() => ClickCount++;
}
```

## Test Naming Convention

Pattern: `ComponentName_Scenario_ExpectedResult`

| Component | Scenario | Result | Test Name |
|-----------|----------|--------|-----------|
| Button | Click | InvokesHandler | `Button_Click_InvokesHandler` |
| DataList | EmptySource | ShowsEmptyTemplate | `DataList_EmptySource_ShowsEmptyTemplate` |
| GridView | WithData | RendersRows | `GridView_WithData_RendersRows` |

## Special Patterns

### Multiple Tests in One File

Each `<Fixture>` block becomes a separate `[Fact]` method:

```razor
@inherits BunitContext

@code {
    [Fact]
    public void Component_FirstScenario_ExpectedResult() { ... }

    [Fact]
    public void Component_SecondScenario_ExpectedResult() { ... }
}
```

### Tests with Services

```razor
@code {
    [Fact]
    public void Component_WithService_Works()
    {
        Services.AddSingleton<IMyService>(new FakeService());

        var cut = Render(@<MyComponent />);
    }
}
```

### Authentication Tests

```razor
@code {
    [Fact]
    public void SecureComponent_AuthenticatedUser_ShowsContent()
    {
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");
        authContext.SetRoles("Admin");

        var cut = Render(@<SecureComponent />);
    }
}
```

### Tests Requiring New TestContext

For tests that need isolated context (e.g., multiple renders):

```razor
@code {
    [Fact]
    public void Component_MultipleRenders_WorksCorrectly()
    {
        using var ctx = new Bunit.TestContext();

        var cut1 = ctx.Render(@<MyComponent Value="1" />);
        var cut2 = ctx.Render(@<MyComponent Value="2" />);

        cut1.Find("span").TextContent.ShouldBe("1");
        cut2.Find("span").TextContent.ShouldBe("2");
    }
}
```

### Tests with xUnit Logger (Optional)

For debugging complex tests, you can optionally enable xUnit logging:

```razor
@using Microsoft.Extensions.Logging

@code {
    private ILogger<MyTest> _logger;

    public MyTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void Component_ComplexScenario_WorksAsExpected()
    {
        _logger = Services.GetService<ILogger<MyTest>>();
        _logger?.LogInformation("Starting test");

        var cut = Render(@<MyComponent />);

        _logger?.LogDebug("Component rendered");

        // Test assertions...
    }
}
```

**Note:** Only add logging when diagnostic output is helpful. Most tests should remain simple without logging.

## Quick Reference Table

| Old Pattern | New Pattern |
|-------------|-------------|
| `@inherits TestComponentBase` | `@inherits BunitContext` |
| `<Fixture Test="Name">` | Remove |
| `<ComponentUnderTest>` | Remove |
| `<SnapshotTest>` | `[Fact]` method with `MarkupMatches()` |
| `void Name(Fixture fixture)` | `[Fact] public void Name()` |
| `fixture.GetComponentUnderTest()` | `Render(@<Component />)` |
| `fixture.GetComponentUnderTest<T>()` | `Render<T>(@<Component />)` |

## Verification

After migrating a file, verify with:

```powershell
# Build check
dotnet build src/BlazorWebFormsComponents.Test --no-restore

# List discovered tests
dotnet test src/BlazorWebFormsComponents.Test --list-tests --filter "FullyQualifiedName~ComponentName"

# Run tests
dotnet test src/BlazorWebFormsComponents.Test --filter "FullyQualifiedName~ComponentName"
```

## Common Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `CS0246: TestComponentBase not found` | Old inheritance | Change to `@inherits BunitContext` |
| `CS0103: Fixture does not exist` | Old wrapper element | Remove `<Fixture>` tags |
| `No tests discovered` | Missing `[Fact]` attribute | Add `[Fact]` to test methods |
| `Method must be public` | Private test method | Add `public` modifier |
