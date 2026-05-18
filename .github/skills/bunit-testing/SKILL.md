---
name: bunit-testing
description: "Write bUnit v2 component tests for BlazorWebFormsComponents. Covers BlazorWebFormsTestContext base class, Render() with inline Razor syntax, Shouldly assertions, testing data-bound components, validation components, event callbacks, JS interop mocking, service registration, authentication testing, and xUnit logger integration. Use when writing new component tests, testing complex scenarios, or understanding the BWFC test infrastructure."
---

# bUnit Test Authoring

This skill covers writing new bUnit v2 component tests for the BlazorWebFormsComponents library.

> **Not for migration.** If you need to migrate existing tests from bUnit beta to v2, use the `bunit-test-migration` skill instead.

## Test Infrastructure

### BlazorWebFormsTestContext

All BWFC component tests should inherit from `BlazorWebFormsTestContext`, which extends bUnit's `BunitContext` with pre-registered services:

```csharp
public abstract class BlazorWebFormsTestContext : BunitContext
{
    // Pre-configured:
    // - JSInterop.Mode = JSRuntimeMode.Loose
    // - LinkGenerator (mocked)
    // - IHttpContextAccessor (mocked)
    // - IWebHostEnvironment (mocked with temp paths)
    // - IMemoryCache
    // - CacheShim, ServerShim, ClientScriptShim
    // - ILogger<T> (with optional xUnit output)
}
```

This base class ensures components that depend on `WebFormsPageBase` shims can render without additional setup.

### Test File Location

Tests are `.razor` files organized by component name:

```
src/BlazorWebFormsComponents.Test/
├── Button/
│   ├── Click.razor
│   ├── Enabled.razor
│   └── Style.razor
├── GridView/
│   ├── Selection.razor
│   ├── Sorting.razor
│   └── Paging.razor
├── Validations/
│   └── SetFocusOnErrorTests.razor
└── ...
```

### Test Naming Convention

`ComponentName_Scenario_ExpectedBehavior`

Examples:
- `Button_Click_InvokesHandler`
- `GridView_WithDataSource_RendersRows`
- `RequiredFieldValidator_BlankInput_DisplaysError`

## Writing Tests

### Basic Pattern

```razor
@inherits BlazorWebFormsTestContext

@code {
    [Fact]
    public void Button_Click_InvokesHandler()
    {
        // Arrange
        var clicked = false;
        var cut = Render(@<Button OnClick="() => clicked = true">Submit</Button>);

        // Act
        cut.Find("input").Click();

        // Assert
        clicked.ShouldBeTrue();
    }
}
```

### Testing Component Properties

```razor
@code {
    [Fact]
    public void Label_Text_RendersInSpan()
    {
        var cut = Render(@<Label Text="Hello World" />);

        cut.Find("span").TextContent.ShouldBe("Hello World");
    }

    [Fact]
    public void TextBox_Visible_False_RendersNothing()
    {
        var cut = Render(@<TextBox Visible="false" />);

        cut.FindAll("input").Count.ShouldBe(0);
    }
}
```

### Testing Data-Bound Components

```razor
@code {
    [Fact]
    public void GridView_WithDataSource_RendersCorrectRowCount()
    {
        var data = new List<TestItem>
        {
            new() { Id = 1, Name = "Item 1" },
            new() { Id = 2, Name = "Item 2" },
            new() { Id = 3, Name = "Item 3" }
        };

        var cut = Render(
            @<GridView ItemType="TestItem" DataSource="data" AutoGenerateColumns="false">
                <Columns>
                    <BoundField ItemType="TestItem" DataField="Name" HeaderText="Name" />
                </Columns>
            </GridView>);

        cut.FindAll("tbody tr").Count.ShouldBe(3);
    }

    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

### Testing Event Callbacks

```razor
@code {
    private GridViewSelectEventArgs _receivedArgs;

    [Fact]
    public void GridView_SelectButton_FiresEventWithCorrectIndex()
    {
        var data = GetTestItems();

        var cut = Render(
            @<GridView ItemType="TestItem" DataSource="data" AutoGenerateColumns="false"
                AutoGenerateSelectButton="true"
                SelectedIndexChanging="@((GridViewSelectEventArgs e) => _receivedArgs = e)">
                <Columns>
                    <BoundField ItemType="TestItem" DataField="Name" HeaderText="Name" />
                </Columns>
            </GridView>);

        // Click Select on the third row
        cut.FindAll("tbody a")
            .Where(a => a.TextContent == "Select")
            .ToList()[2].Click();

        _receivedArgs.ShouldNotBeNull();
        _receivedArgs.NewSelectedIndex.ShouldBe(2);
    }

    [Fact]
    public void GridView_CancelEvent_PreventsAction()
    {
        var data = GetTestItems();

        var cut = Render(
            @<GridView ItemType="TestItem" DataSource="data" AutoGenerateColumns="false"
                AutoGenerateSelectButton="true"
                SelectedIndexChanging="@((GridViewSelectEventArgs e) => e.Cancel = true)">
                <Columns>
                    <BoundField ItemType="TestItem" DataField="Name" HeaderText="Name" />
                </Columns>
            </GridView>);

        cut.FindAll("tbody a").First(a => a.TextContent == "Select").Click();

        // No row should be highlighted since cancel was set
        var rows = cut.FindAll("tbody tr");
        foreach (var row in rows)
        {
            var style = row.GetAttribute("style");
            (style == null || !style.Contains("background-color")).ShouldBeTrue();
        }
    }
}
```

### Testing Validation Components

```razor
@using BlazorWebFormsComponents.Enums

@code {
    ForwardRef<InputBase<string>> NameRef = new ForwardRef<InputBase<string>>();

    [Fact]
    public void RequiredFieldValidator_EmptyInput_ShowsError()
    {
        var model = new TestModel();
        var cut = Render(
            @<EditForm Model="@model" OnValidSubmit="() => { }">
                <InputText @ref="NameRef.Current" @bind-Value="model.Name" />
                <RequiredFieldValidator Type="string"
                    ControlRef="@NameRef"
                    ErrorMessage="Name is required." />
            </EditForm>);

        cut.Find("input").Change("  ");
        cut.Find("form").Submit();

        cut.Find("span").TextContent.ShouldContain("Name is required");
    }

    public class TestModel { public string Name { get; set; } }
}
```

### Testing JS Interop

```razor
@code {
    [Fact]
    public void Component_WithJsCall_InvokesCorrectMethod()
    {
        // Set up specific JS mock
        JSInterop.SetupVoid("bwfc.Validation.SetFocus", _ => true);

        var cut = Render(@<MyComponent SetFocusOnError="true" />);

        // Trigger the action that calls JS
        cut.Find("form").Submit();

        // Verify JS was invoked
        JSInterop.VerifyInvoke("bwfc.Validation.SetFocus");
    }

    [Fact]
    public void Component_WithoutJsCall_DoesNotInvoke()
    {
        var cut = Render(@<MyComponent SetFocusOnError="false" />);

        cut.Find("form").Submit();

        JSInterop.VerifyNotInvoke("bwfc.Validation.SetFocus");
    }
}
```

### Testing with Service Registration

```razor
@code {
    [Fact]
    public void Component_WithCustomService_UsesIt()
    {
        Services.AddSingleton<IMyService>(new FakeMyService());

        var cut = Render(@<MyComponent />);

        cut.Find("div").TextContent.ShouldBe("fake-data");
    }
}
```

### Testing with Authentication

```razor
@code {
    [Fact]
    public void SecureComponent_AuthenticatedUser_ShowsContent()
    {
        var auth = this.AddTestAuthorization();
        auth.SetAuthorized("testuser");
        auth.SetRoles("Admin", "User");

        var cut = Render(@<LoginView>
            <LoggedInTemplate>Welcome!</LoggedInTemplate>
            <AnonymousTemplate>Please log in.</AnonymousTemplate>
        </LoginView>);

        cut.Markup.ShouldContain("Welcome!");
    }
}
```

### Accessing Component Instance

```razor
@code {
    [Fact]
    public void TreeView_Nodes_HasCorrectCount()
    {
        var cut = Render(@<TreeView>
            <Nodes>
                <TreeNode Text="Node 1" />
                <TreeNode Text="Node 2" />
            </Nodes>
        </TreeView>);

        cut.FindComponent<TreeView>().Instance.Nodes.Count.ShouldBe(2);
    }
}
```

### HTML Markup Assertions

```razor
@code {
    [Fact]
    public void Button_RendersCorrectHtml()
    {
        var cut = Render(@<Button Text="Submit" CssClass="btn" />);

        // Exact match (whitespace-insensitive)
        cut.MarkupMatches(@"<input type=""submit"" value=""Submit"" class=""btn"" />");
    }
}
```

## Using the xUnit Logger

For complex tests that need diagnostic output:

```razor
@using Microsoft.Extensions.Logging

@code {
    private ILogger<MyTest> _logger;

    public MyTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public void Component_ComplexScenario_LogsDiagnostics()
    {
        _logger = Services.GetService<ILogger<MyTest>>();
        _logger?.LogInformation("Starting complex test");

        var cut = Render(@<MyComponent />);

        _logger?.LogDebug("Rendered successfully, checking output");
        // assertions...
    }
}
```

Only add logging where diagnostic output helps debugging. Most simple tests don't need it.

## C# Code-Behind Tests

For tests that need programmatic component construction (no Razor syntax):

```csharp
public class IsPostBackTests : IDisposable
{
    private readonly BunitContext _ctx;

    public IsPostBackTests()
    {
        _ctx = new BunitContext();
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
        _ctx.Services.AddSingleton<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);
        // ... register all required services
    }

    [Fact]
    public void Component_SSR_GetRequest_IsPostBackFalse()
    {
        RegisterHttpContextWithMethod("GET");
        var cut = _ctx.Render<TestComponent>();
        cut.Instance.IsPostBackValue.ShouldBeFalse();
    }

    public void Dispose() => _ctx.Dispose();
}
```

**Prefer `.razor` test files** for component rendering tests — they're more readable and support inline Razor syntax. Use `.cs` files only when you need programmatic control (e.g., testing internal APIs, building render trees manually).

## Assertions Reference

| Assertion | Usage |
|-----------|-------|
| `value.ShouldBe(expected)` | Exact equality (Shouldly) |
| `value.ShouldBeTrue()` | Boolean true |
| `value.ShouldNotBeNull()` | Non-null check |
| `value.ShouldContain("text")` | String contains |
| `cut.MarkupMatches("<html>")` | HTML comparison (bUnit) |
| `cut.Find("selector")` | CSS selector (throws if not found) |
| `cut.FindAll("selector")` | CSS selector (returns list) |
| `cut.FindComponent<T>()` | Find child component instance |

## Running Tests

```bash
# Run all component tests
dotnet test src/BlazorWebFormsComponents.Test --nologo

# Run tests for a specific component
dotnet test src/BlazorWebFormsComponents.Test --filter "Button"

# Run with verbose output
dotnet test src/BlazorWebFormsComponents.Test --nologo -v normal
```

## Checklist

- [ ] Test file is in `src/BlazorWebFormsComponents.Test/{ComponentName}/`
- [ ] `.razor` test inherits `BlazorWebFormsTestContext` (not raw `BunitContext`)
- [ ] Test method name follows `ComponentName_Scenario_ExpectedBehavior`
- [ ] Uses Shouldly assertions (`ShouldBe`, `ShouldBeTrue`, etc.)
- [ ] Uses `Render(@<Component />)` inline Razor syntax
- [ ] Data-bound tests provide realistic test data
- [ ] Event tests verify both success and cancel paths
- [ ] `dotnet test src/BlazorWebFormsComponents.Test` passes across all target frameworks
