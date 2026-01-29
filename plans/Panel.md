# Panel Component Implementation Plan

## Overview
Implement the Panel component that emulates ASP.NET Web Forms `<asp:Panel>` control. Panel is a container control that renders as a `<div>` element.

## Web Forms Reference
- **Documentation**: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.panel?view=netframework-4.8
- **Namespace**: `System.Web.UI.WebControls`

## HTML Output

**Web Forms:**
```html
<asp:Panel ID="panel1" CssClass="my-panel" runat="server">
    <p>Content here</p>
</asp:Panel>
```

**Rendered HTML:**
```html
<div id="panel1" class="my-panel">
    <p>Content here</p>
</div>
```

## Key Properties to Implement

| Property | Type | Description |
|----------|------|-------------|
| `ChildContent` | `RenderFragment` | Child content (Blazor pattern) |
| `DefaultButton` | `string` | ID of button that is clicked when Enter is pressed |
| `Direction` | `ContentDirection` | Text direction (LeftToRight, RightToLeft, NotSet) |
| `GroupingText` | `string` | Caption for fieldset rendering |
| `HorizontalAlign` | `HorizontalAlign` | Horizontal alignment of content |
| `ScrollBars` | `ScrollBars` | Scrollbar visibility |
| `Wrap` | `bool` | Whether content wraps |
| All `IStyle` properties | various | BackColor, ForeColor, CssClass, etc. |

## Special Behaviors

1. **GroupingText**: When set, renders as `<fieldset>` with `<legend>` instead of `<div>`
2. **ScrollBars**: Maps to CSS `overflow` property
3. **HorizontalAlign**: Maps to CSS `text-align` property
4. **Direction**: Maps to `dir` HTML attribute

## Implementation Tasks

- [ ] Create `src/BlazorWebFormsComponents/Panel.razor`
- [ ] Create `src/BlazorWebFormsComponents/Panel.razor.cs`
- [ ] Add `ContentDirection` enum if not exists (in `Enums/` folder)
- [ ] Add `ScrollBars` enum if not exists
- [ ] Add `HorizontalAlign` enum if not exists
- [ ] Create unit tests in `src/BlazorWebFormsComponents.Test/Panel/`
  - [ ] Basic.razor - basic div rendering
  - [ ] Style.razor - style properties
  - [ ] Visible.razor - visibility toggle
  - [ ] GroupingText.razor - fieldset rendering
  - [ ] ScrollBars.razor - overflow styles
- [ ] Create documentation in `docs/EditorControls/Panel.md`
- [ ] Update `mkdocs.yml` nav section

## Code Structure

### Panel.razor
```razor
@inherits BaseStyledComponent

@if (Visible)
{
    @if (!string.IsNullOrEmpty(GroupingText))
    {
        <fieldset class="@CssClass" style="@ComputedStyle" dir="@DirectionAttr">
            <legend>@GroupingText</legend>
            @ChildContent
        </fieldset>
    }
    else
    {
        <div class="@CssClass" style="@ComputedStyle" dir="@DirectionAttr">
            @ChildContent
        </div>
    }
}
```

### Panel.razor.cs
```csharp
public partial class Panel : BaseStyledComponent
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public string DefaultButton { get; set; }

    [Parameter]
    public ContentDirection Direction { get; set; } = ContentDirection.NotSet;

    [Parameter]
    public string GroupingText { get; set; }

    [Parameter]
    public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.NotSet;

    [Parameter]
    public ScrollBars ScrollBars { get; set; } = ScrollBars.None;

    [Parameter]
    public bool Wrap { get; set; } = true;

    private string DirectionAttr => Direction switch
    {
        ContentDirection.LeftToRight => "ltr",
        ContentDirection.RightToLeft => "rtl",
        _ => null
    };

    private string ComputedStyle => BuildStyle();
    
    private string BuildStyle()
    {
        var baseStyle = this.ToStyle().Build();
        var styles = new List<string>();
        
        if (!string.IsNullOrEmpty(baseStyle))
            styles.Add(baseStyle);
            
        // Add HorizontalAlign
        if (HorizontalAlign != HorizontalAlign.NotSet)
            styles.Add($"text-align:{HorizontalAlign.ToString().ToLower()}");
            
        // Add ScrollBars
        styles.Add(ScrollBars switch
        {
            ScrollBars.Horizontal => "overflow-x:scroll;overflow-y:hidden",
            ScrollBars.Vertical => "overflow-x:hidden;overflow-y:scroll",
            ScrollBars.Both => "overflow:scroll",
            ScrollBars.Auto => "overflow:auto",
            _ => null
        });
        
        // Add Wrap
        if (!Wrap)
            styles.Add("white-space:nowrap");
            
        return string.Join(";", styles.Where(s => s != null)).NullIfEmpty();
    }
}
```

## Estimated Effort
- Complexity: **Low**
- Estimated time: **2-3 hours**

## Acceptance Criteria
1. Panel renders as `<div>` by default
2. Panel renders as `<fieldset>` with `<legend>` when GroupingText is set
3. All style properties work correctly
4. ScrollBars property maps to correct CSS overflow values
5. Direction property sets `dir` attribute
6. Visible="false" prevents rendering
7. All unit tests pass
