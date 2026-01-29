# PlaceHolder Component Implementation Plan

## Overview
Implement the PlaceHolder component that emulates ASP.NET Web Forms `<asp:PlaceHolder>` control. PlaceHolder is a simple container that renders NO wrapper element - it only renders its child content.

## Web Forms Reference
- **Documentation**: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.placeholder?view=netframework-4.8
- **Namespace**: `System.Web.UI.WebControls`

## HTML Output

**Web Forms:**
```html
<asp:PlaceHolder ID="ph1" runat="server">
    <p>Dynamic content here</p>
</asp:PlaceHolder>
```

**Rendered HTML:**
```html
<p>Dynamic content here</p>
```

**Note:** PlaceHolder renders NO wrapper element - this is its key characteristic.

## Key Properties to Implement

| Property | Type | Description |
|----------|------|-------------|
| `ChildContent` | `RenderFragment` | Child content (Blazor pattern) |
| `Visible` | `bool` | Whether to render content |
| `ID` | `string` | Component ID (inherited, not rendered) |

## Special Behaviors

1. **No wrapper element**: Unlike Panel, PlaceHolder renders only its children
2. **Visibility control**: Primary use case is showing/hiding content dynamically
3. **No styling**: PlaceHolder has no style properties (it has no element to style)

## Implementation Tasks

- [ ] Create `src/BlazorWebFormsComponents/PlaceHolder.razor`
- [ ] Create `src/BlazorWebFormsComponents/PlaceHolder.razor.cs`
- [ ] Create unit tests in `src/BlazorWebFormsComponents.Test/PlaceHolder/`
  - [ ] Basic.razor - renders child content only
  - [ ] Visible.razor - visibility toggle
  - [ ] Empty.razor - empty placeholder renders nothing
- [ ] Create documentation in `docs/EditorControls/PlaceHolder.md`
- [ ] Update `mkdocs.yml` nav section

## Code Structure

### PlaceHolder.razor
```razor
@inherits BaseWebFormsComponent

@if (Visible)
{
    @ChildContent
}
```

### PlaceHolder.razor.cs
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
    public partial class PlaceHolder : BaseWebFormsComponent
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
```

## Test Examples

### Basic.razor
```razor
@code {
    [Fact]
    public void PlaceHolder_RendersChildContentOnly()
    {
        // Arrange & Act
        var cut = Render(
            @<PlaceHolder>
                <p>Test content</p>
            </PlaceHolder>
        );

        // Assert - no wrapper element, just the content
        cut.Markup.Trim().ShouldBe("<p>Test content</p>");
    }

    [Fact]
    public void PlaceHolder_WithMultipleChildren_RendersAll()
    {
        // Arrange & Act
        var cut = Render(
            @<PlaceHolder>
                <p>First</p>
                <p>Second</p>
            </PlaceHolder>
        );

        // Assert
        var paragraphs = cut.FindAll("p");
        paragraphs.Count.ShouldBe(2);
    }
}
```

### Visible.razor
```razor
@code {
    [Fact]
    public void PlaceHolder_WhenNotVisible_RendersNothing()
    {
        // Arrange & Act
        var cut = Render(
            @<PlaceHolder Visible="false">
                <p>Should not appear</p>
            </PlaceHolder>
        );

        // Assert
        cut.Markup.Trim().ShouldBeEmpty();
    }
}
```

## Estimated Effort
- Complexity: **Very Low**
- Estimated time: **1-2 hours**

## Acceptance Criteria
1. PlaceHolder renders NO wrapper element
2. Child content is rendered directly
3. Visible="false" prevents all content from rendering
4. Empty PlaceHolder renders nothing
5. All unit tests pass

## Migration Notes

When migrating from Web Forms:
1. Remove `asp:` prefix and `runat="server"`
2. The `ID` property is not rendered (Blazor uses `@ref` for references)
3. Use for conditional rendering of content blocks
