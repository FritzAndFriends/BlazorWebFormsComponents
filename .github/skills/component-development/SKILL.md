---
name: component-development
description: Guidance for creating Blazor components that emulate ASP.NET Web Forms controls. Use this when implementing new components or extending existing ones in the BlazorWebFormsComponents library.
---

# Component Development Skill

This skill covers creating new Blazor components that emulate ASP.NET Web Forms controls.

## Related Guides

- [HTML Output Matching Guide](HTML_OUTPUT_MATCHING.md) - How to ensure Blazor components render identical HTML to Web Forms controls

## Quick Reference

### Creating a New Component

1. **Identify the Web Forms control** from `System.Web.UI.WebControls`
2. **Research the HTML output** using the [HTML Output Matching Guide](HTML_OUTPUT_MATCHING.md)
3. **Create component files:**
   - `src/BlazorWebFormsComponents/{ComponentName}.razor`
   - `src/BlazorWebFormsComponents/{ComponentName}.razor.cs`
4. **Inherit from appropriate base class:**
   - `BaseWebFormsComponent` - Basic components
   - `BaseStyledComponent` - Components with styling
   - `DataBoundComponent<T>` - Data-bound components
5. **Add unit tests** in `src/BlazorWebFormsComponents.Test/{ComponentName}/`
6. **Add sample page** in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/{ComponentName}/`
7. **Add integration tests** using Playwright in `samples/AfterBlazorServerSide.Tests/`
8. **Create documentation** in `docs/{Category}/{ComponentName}.md`
9. **Update navigation:**
   - Add to `samples/AfterBlazorServerSide/Components/Layout/NavMenu.razor` (TreeView)
   - Add to `samples/AfterBlazorServerSide/Components/Pages/ComponentList.razor` (home page catalog)
   - Update `mkdocs.yml` and `README.md`

### Base Class Selection

| Base Class | Use When |
|------------|----------|
| `BaseWebFormsComponent` | Simple components without styling (Literal, PlaceHolder) |
| `BaseStyledComponent` | Components with visual styling (Label, Panel, Button) |
| `ButtonBaseComponent` | Button-like components (Button, LinkButton, ImageButton) |
| `DataBoundComponent<T>` | Components binding to collections (Repeater, GridView) |
| `BaseValidator` | Validation controls |

### Property Naming Convention

Match Web Forms property names exactly:
- `Text` not `Label` or `Content`
- `CssClass` not `Class` or `ClassName`
- `NavigateUrl` not `Href` or `Url`
- `ImageUrl` not `Src` or `Source`

### Event Naming Convention

Prefix with `On`:
- `OnClick` for click events
- `OnCommand` for command events
- `OnSelectedIndexChanged` for selection changes
- `OnDataBinding` for data binding events

### Integration Testing Requirements

Every component must have integration tests in `samples/AfterBlazorServerSide.Tests/` using Playwright:

1. **Page load test** in `ControlSampleTests.cs`:
   - Add route to the appropriate `[Theory]` test (EditorControl, DataControl, etc.)
   - Verifies page loads without console errors or page errors

2. **Interactive test** in `InteractiveComponentTests.cs` (for interactive components):
   - Test user interactions (clicks, input, selection changes)
   - Verify component responds correctly to user actions
   - Assert no console errors during interaction

Example page load test entry:
```csharp
[Theory]
[InlineData("/ControlSamples/YourComponent")]
public async Task EditorControl_Loads_WithoutErrors(string path)
```

Example interactive test:
```csharp
[Fact]
public async Task YourComponent_Interaction_Works()
{
    var page = await _fixture.NewPageAsync();
    try
    {
        await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/YourComponent");
        // Test interactions...
        // Assert expected behavior...
    }
    finally
    {
        await page.CloseAsync();
    }
}
```

Run integration tests with:
```bash
dotnet test samples/AfterBlazorServerSide.Tests
```
