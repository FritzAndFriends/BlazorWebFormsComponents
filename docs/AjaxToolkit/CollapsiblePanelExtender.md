# CollapsiblePanelExtender

The **CollapsiblePanelExtender** adds collapse/expand functionality to a target panel with smooth CSS transitions. It supports separate trigger controls for collapse and expand, dynamic label text updates, automatic collapse/expand on mouse events, and configurable animation directions.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/CollapsiblePanelExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the panel to make collapsible
- `CollapseControlID` — ID of the element that triggers collapse
- `ExpandControlID` — ID of the element that triggers expand
- `Collapsed` — Whether the panel starts in collapsed state
- `CollapsedSize` — Height/width in pixels when collapsed (0 = fully hidden)
- `ExpandedSize` — Height/width in pixels when expanded (0 = auto)
- `CollapsedText` — Text to display when collapsed
- `ExpandedText` — Text to display when expanded
- `TextLabelID` — ID of the element whose text changes with state
- `ExpandDirection` — Direction of collapse/expand animation (Vertical or Horizontal)
- `AutoCollapse` — Automatically collapse when mouse leaves
- `AutoExpand` — Automatically expand when mouse enters
- `ScrollContents` — Enable scrollbars when panel content overflows
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## ExpandDirection Enum

Controls the direction of the collapse/expand animation:

```csharp
enum ExpandDirection
{
    Vertical = 0,    // Collapse top-to-bottom (default)
    Horizontal = 1   // Collapse left-to-right
}
```

## Web Forms Syntax

```html
<asp:Button ID="btnToggle" Text="Click to expand" runat="server" />

<asp:Panel ID="pnlContent" style="display: none;">
    <p>This content can be collapsed and expanded.</p>
</asp:Panel>

<ajaxToolkit:CollapsiblePanelExtender 
    ID="cpe"
    runat="server"
    TargetControlID="pnlContent"
    CollapseControlID="btnToggle"
    ExpandControlID="btnToggle"
    CollapsedText="▶ Show Details"
    ExpandedText="▼ Hide Details"
    TextLabelID="btnToggle"
    Collapsed="true" />
```

## Blazor Migration

```razor
<Button ID="btnToggle" Text="Click to expand" />

<div ID="pnlContent" style="display: none; overflow: hidden;">
    <p>This content can be collapsed and expanded.</p>
</div>

<CollapsiblePanelExtender 
    TargetControlID="pnlContent"
    CollapseControlID="btnToggle"
    ExpandControlID="btnToggle"
    CollapsedText="▶ Show Details"
    ExpandedText="▼ Hide Details"
    TextLabelID="btnToggle"
    Collapsed="true" />
```

**Migration is simple:** Just remove the `ajaxToolkit:` prefix and remove the `runat="server"` and `ID` attributes. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the panel/element to make collapsible |
| `CollapseControlID` | `string` | `""` | ID of the element that triggers collapse |
| `ExpandControlID` | `string` | `""` | ID of the element that triggers expand (can equal CollapseControlID for toggle) |
| `Collapsed` | `bool` | `false` | Whether the panel starts in collapsed state |
| `CollapsedSize` | `int` | `0` | Height/width in pixels when collapsed (0 = fully hidden) |
| `ExpandedSize` | `int` | `0` | Height/width in pixels when expanded (0 = auto-size based on content) |
| `CollapsedText` | `string` | `""` | Text to display in TextLabelID element when collapsed |
| `ExpandedText` | `string` | `""` | Text to display in TextLabelID element when expanded |
| `TextLabelID` | `string` | `""` | ID of the element whose text content changes with state |
| `ExpandDirection` | `ExpandDirection` | `Vertical` | Direction of animation: Vertical (top-to-bottom) or Horizontal (left-to-right) |
| `AutoCollapse` | `bool` | `false` | Automatically collapse when mouse leaves the panel |
| `AutoExpand` | `bool` | `false` | Automatically expand when mouse enters the panel |
| `ScrollContents` | `bool` | `false` | Enable scrollbars within the panel when content overflows |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Simple Collapse/Expand

```razor
@rendermode InteractiveServer

<button ID="btnToggle" style="padding: 10px 20px; cursor: pointer; background: #007bff; color: white; border: none; border-radius: 4px; font-weight: bold;">
    Show Details
</button>

<div ID="detailsPanel" style="display: none; overflow: hidden; margin-top: 15px; padding: 20px; background: #f8f9fa; border: 1px solid #dee2e6; border-radius: 4px;">
    <h4>Details</h4>
    <p>This content expands and collapses smoothly with CSS transitions.</p>
    <ul>
        <li>Item 1</li>
        <li>Item 2</li>
        <li>Item 3</li>
    </ul>
</div>

<CollapsiblePanelExtender 
    TargetControlID="detailsPanel"
    CollapseControlID="btnToggle"
    ExpandControlID="btnToggle"
    CollapsedText="▶ Show Details"
    ExpandedText="▼ Hide Details"
    TextLabelID="btnToggle"
    Collapsed="true" />
```

### Separate Expand/Collapse Buttons

```razor
@rendermode InteractiveServer

<div style="display: flex; gap: 10px; margin-bottom: 15px;">
    <button ID="btnExpand" style="padding: 8px 16px; cursor: pointer; background: #28a745; color: white; border: none; border-radius: 4px;">
        ▲ Expand All
    </button>
    <button ID="btnCollapse" style="padding: 8px 16px; cursor: pointer; background: #dc3545; color: white; border: none; border-radius: 4px;">
        ▼ Collapse All
    </button>
</div>

<div ID="settingsPanel" style="display: none; overflow: hidden; padding: 20px; background: white; border: 1px solid #dee2e6; border-radius: 4px;">
    <h4>Advanced Settings</h4>
    <label style="display: block; margin: 10px 0;">
        <input type="checkbox" @bind-checked="@setting1" />
        Enable feature 1
    </label>
    <label style="display: block; margin: 10px 0;">
        <input type="checkbox" @bind-checked="@setting2" />
        Enable feature 2
    </label>
    <label style="display: block; margin: 10px 0;">
        <input type="checkbox" @bind-checked="@setting3" />
        Enable feature 3
    </label>
</div>

<CollapsiblePanelExtender 
    TargetControlID="settingsPanel"
    ExpandControlID="btnExpand"
    CollapseControlID="btnCollapse"
    Collapsed="true" />

@code {
    private bool setting1 = false;
    private bool setting2 = true;
    private bool setting3 = false;
}
```

### Auto-Collapse/Expand on Hover

```razor
@rendermode InteractiveServer

<div style="max-width: 300px;">
    <div ID="hoverPanel" style="display: none; overflow: hidden; padding: 20px; background: #fff3cd; border: 1px solid #ffc107; border-radius: 4px;">
        <p>Hover over this area to expand. Move your mouse away to collapse.</p>
        <p>This is useful for tooltips or quick previews.</p>
    </div>
</div>

<CollapsiblePanelExtender 
    TargetControlID="hoverPanel"
    ExpandControlID="hoverPanel"
    CollapseControlID="hoverPanel"
    AutoExpand="true"
    AutoCollapse="true"
    Collapsed="true" />

<style>
    #hoverPanel {
        transition: all 0.3s ease;
    }
</style>
```

### Horizontal Collapse (Sidebar-Style)

```razor
@rendermode InteractiveServer

<div style="display: flex; gap: 20px;">
    <button ID="btnToggleSidebar" style="padding: 8px 16px; cursor: pointer; background: #6c757d; color: white; border: none; border-radius: 4px;">
        ☰ Menu
    </button>
    
    <div ID="sidebarPanel" style="display: none; overflow: hidden; min-width: 200px; background: #f8f9fa; padding: 20px; border-right: 1px solid #dee2e6;">
        <ul style="list-style: none; padding: 0;">
            <li><a href="#" style="display: block; padding: 10px 0; text-decoration: none; color: #007bff;">Dashboard</a></li>
            <li><a href="#" style="display: block; padding: 10px 0; text-decoration: none; color: #007bff;">Users</a></li>
            <li><a href="#" style="display: block; padding: 10px 0; text-decoration: none; color: #007bff;">Reports</a></li>
            <li><a href="#" style="display: block; padding: 10px 0; text-decoration: none; color: #007bff;">Settings</a></li>
        </ul>
    </div>
    
    <div style="flex: 1;">
        <h2>Main Content</h2>
        <p>Content area that grows when sidebar is hidden.</p>
    </div>
</div>

<CollapsiblePanelExtender 
    TargetControlID="sidebarPanel"
    CollapseControlID="btnToggleSidebar"
    ExpandControlID="btnToggleSidebar"
    ExpandDirection="ExpandDirection.Horizontal"
    Collapsed="false" />
```

### FAQ Accordion Pattern

```razor
@rendermode InteractiveServer

<div style="max-width: 600px;">
    @foreach (var item in faqItems)
    {
        <div style="margin-bottom: 10px; border: 1px solid #dee2e6; border-radius: 4px;">
            <button ID="@($"faqBtn{item.Id}")" 
                    style="width: 100%; padding: 15px; text-align: left; background: #f8f9fa; border: none; cursor: pointer; font-weight: bold; display: flex; justify-content: space-between; align-items: center;">
                @item.Question
                <span style="font-size: 12px;">►</span>
            </button>
            
            <div ID="@($"faqContent{item.Id}")" style="display: none; overflow: hidden; padding: 20px; background: white; border-top: 1px solid #dee2e6;">
                <p>@item.Answer</p>
            </div>
        </div>
        
        <CollapsiblePanelExtender 
            TargetControlID="@($"faqContent{item.Id}")"
            CollapseControlID="@($"faqBtn{item.Id}")"
            ExpandControlID="@($"faqBtn{item.Id}")"
            CollapsedText="▶"
            ExpandedText="▼"
            TextLabelID="@($"faqBtn{item.Id}")"
            Collapsed="true" />
    }
</div>

@code {
    private class FaqItem
    {
        public int Id { get; set; }
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
    }
    
    private List<FaqItem> faqItems = new()
    {
        new FaqItem { Id = 1, Question = "What is Blazor?", Answer = "Blazor is a framework for building interactive web applications with C# instead of JavaScript." },
        new FaqItem { Id = 2, Question = "How do I migrate from Web Forms?", Answer = "Use BlazorWebFormsComponents (BWFC) to emulate familiar Web Forms controls with minimal markup changes." },
        new FaqItem { Id = 3, Question = "Is BWFC production-ready?", Answer = "Yes, BWFC is production-ready. It has been tested extensively and is used in production applications." }
    };
}
```

### Panel with Scrollable Content

```razor
@rendermode InteractiveServer

<button ID="btnToggleLog" style="padding: 10px 20px; cursor: pointer; background: #007bff; color: white; border: none; border-radius: 4px;">
    Show Log
</button>

<div ID="logPanel" style="display: none; overflow: hidden; margin-top: 15px; max-height: 300px; border: 1px solid #dee2e6; border-radius: 4px; background: #f8f9fa;">
    <div style="padding: 15px; font-family: monospace; font-size: 12px;">
        <div>[2026-03-15 10:25:00] Application started</div>
        <div>[2026-03-15 10:25:01] Database connected</div>
        <div>[2026-03-15 10:25:02] Loading configuration</div>
        <div>[2026-03-15 10:25:03] Initializing services</div>
        <div>[2026-03-15 10:25:04] Ready</div>
        <div>[2026-03-15 10:26:01] User login attempt</div>
        <div>[2026-03-15 10:26:02] Authentication successful</div>
        <div>[2026-03-15 10:26:15] Page load: /dashboard</div>
    </div>
</div>

<CollapsiblePanelExtender 
    TargetControlID="logPanel"
    CollapseControlID="btnToggleLog"
    ExpandControlID="btnToggleLog"
    CollapsedText="▶ Show Log"
    ExpandedText="▼ Hide Log"
    TextLabelID="btnToggleLog"
    ScrollContents="true"
    Collapsed="true" />
```

### Starting Expanded with Partial Visibility

```razor
@rendermode InteractiveServer

<button ID="btnTogglePreview" style="padding: 10px 20px; cursor: pointer; background: #007bff; color: white; border: none; border-radius: 4px;">
    ▼ Hide Preview
</button>

<div ID="previewPanel" style="overflow: hidden; margin-top: 15px; padding: 20px; background: #e7f3ff; border: 1px solid #0066cc; border-radius: 4px;">
    <p>Preview shows the first few lines of content. Click the button to show all details.</p>
    <p>Line 2 of preview content</p>
    <p>Line 3 of preview content</p>
    <p>Line 4 of preview content</p>
    <p>Line 5 of preview content</p>
</div>

<CollapsiblePanelExtender 
    TargetControlID="previewPanel"
    CollapseControlID="btnTogglePreview"
    ExpandControlID="btnTogglePreview"
    CollapsedSize="120"
    CollapsedText="▶ Show Full Content"
    ExpandedText="▼ Hide Preview"
    TextLabelID="btnTogglePreview"
    Collapsed="false" />
```

## HTML Output

The CollapsiblePanelExtender produces no HTML itself — it only attaches JavaScript behavior to the target panel.

**Before (initial state, collapsed):**
```html
<button id="btnToggle" style="...">▶ Show Details</button>

<div id="detailsPanel" style="display: none; overflow: hidden; ...">
    Content here
</div>
```

**After expand (via JavaScript):**
```html
<button id="btnToggle" style="...">▼ Hide Details</button>

<div id="detailsPanel" style="display: block; overflow: hidden; height: 200px; transition: height 0.3s ease; ...">
    Content here
</div>
```

## Collapsible Panel Behavior

### Animation

- Collapse/expand uses CSS `height` transitions for smooth animation
- Animation duration is typically 0.3s (controlled by CSS)
- During animation, `overflow: hidden` prevents content from showing outside bounds

### Initial State

- If `Collapsed="true"`, panel starts hidden (or at `CollapsedSize`)
- If `Collapsed="false"`, panel starts visible at full height
- `TextLabelID` text updates based on initial state

### Text Updates

- When panel state changes, the element with ID `TextLabelID` updates its text
- Use `CollapsedText` for text when panel is collapsed
- Use `ExpandedText` for text when panel is expanded
- Useful for toggle buttons showing the next action: "▶ Show" vs "▼ Hide"

### Expand Direction

**Vertical (default):**
- Panel collapses from bottom up
- Height transitions from full to `CollapsedSize`
- Best for stacked content

**Horizontal:**
- Panel collapses from right to left
- Width transitions instead of height
- Best for sidebars and menu toggles

### Auto-Expand/Auto-Collapse

When `AutoExpand="true"`:
- Moving mouse into panel automatically expands it
- Useful for hover tooltips or preview panes

When `AutoCollapse="true"`:
- Moving mouse out of panel automatically collapses it
- Combines with `AutoExpand` for full hover support

### Scrollable Content

When `ScrollContents="true"`:
- Overflow content gets scrollbars
- Set `ExpandedSize` to limit the expanded height
- Useful for log viewers or long content lists

## Render Mode Requirements

The CollapsiblePanelExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer

<button ID="btnToggle">Expand</button>
<div ID="panel">Content</div>
<CollapsiblePanelExtender TargetControlID="panel" ExpandControlID="btnToggle" />
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The panel remains in its initial display state and doesn't respond to clicks.
- **JavaScript disabled:** Same as SSR — collapse/expand functionality doesn't work.
- **Module import fails:** Any JavaScript errors are logged to browser console; panel remains in initial state.

## CSS Considerations

For smooth animations, include CSS transitions:

```css
#myPanel {
    transition: height 0.3s ease;
    overflow: hidden;
}
```

For horizontal collapse:
```css
#mySidebar {
    transition: width 0.3s ease;
    overflow: hidden;
}
```

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:CollapsiblePanelExtender
   + <CollapsiblePanelExtender
   ```

2. **Convert ExpandDirection to enum**
   ```diff
   - ExpandDirection="Vertical"
   + ExpandDirection="ExpandDirection.Vertical"
   ```

3. **Remove `runat="server"` and `ID` attributes**
   ```diff
   - runat="server"
   - ID="cpe"
   ```

### Before (Web Forms)

```html
<asp:Button ID="btnToggle" Text="Show Details" runat="server" />

<asp:Panel ID="detailsPanel" style="display: none;">
    <p>Details content</p>
</asp:Panel>

<ajaxToolkit:CollapsiblePanelExtender 
    ID="cpe"
    TargetControlID="detailsPanel"
    CollapseControlID="btnToggle"
    ExpandControlID="btnToggle"
    CollapsedText="▶ Show"
    ExpandedText="▼ Hide"
    TextLabelID="btnToggle"
    Collapsed="true"
    runat="server" />
```

### After (Blazor)

```razor
<Button ID="btnToggle" Text="Show Details" />

<div ID="detailsPanel" style="display: none;">
    <p>Details content</p>
</div>

<CollapsiblePanelExtender 
    TargetControlID="detailsPanel"
    CollapseControlID="btnToggle"
    ExpandControlID="btnToggle"
    CollapsedText="▶ Show"
    ExpandedText="▼ Hide"
    TextLabelID="btnToggle"
    Collapsed="true" />
```

## Best Practices

1. **Use same button for collapse and expand** — Set both `CollapseControlID` and `ExpandControlID` to same button ID for toggle behavior
2. **Update button text dynamically** — Use `TextLabelID` and `CollapsedText`/`ExpandedText` to show user the next action
3. **Test with keyboard** — Ensure collapse/expand triggers are keyboard-accessible (clickable buttons, not divs)
4. **Smooth transitions** — Add CSS `transition: all 0.3s ease;` to panel for smooth animation
5. **Avoid nested collapsibles** — Nesting multiple CollapsiblePanelExtenders can cause unexpected behavior
6. **Use appropriate sizes** — `CollapsedSize` of 0 means fully hidden; use positive values (30-50px) to show a header
7. **Set ExpandedSize for auto-sizing** — Leave `ExpandedSize="0"` for auto-height; set explicit value to limit expanded height
8. **Accessibility:** Use `<button>` elements for triggers (not `<div>`), ensure sufficient color contrast, test with screen readers

## Troubleshooting

| Issue | Solution |
|---|---|
| Collapse/expand not working | Verify `TargetControlID` matches panel's `ID`. Ensure `@rendermode InteractiveServer` is set. Check browser console for JavaScript errors. |
| Animation is jerky | Verify target panel has `overflow: hidden` in CSS. Add `transition` CSS property to smooth animation. |
| Text doesn't update | Verify `TextLabelID` matches the button/element ID. Check that `CollapsedText` and `ExpandedText` are set. |
| Wrong direction | Verify `ExpandDirection` is set correctly. Use `ExpandDirection.Vertical` for height, `ExpandDirection.Horizontal` for width. |
| Auto-collapse/expand not triggering | Verify `AutoCollapse="true"` and/or `AutoExpand="true"` are set. Ensure mouse events can reach the panel (check CSS `pointer-events`). |
| Panel expands but doesn't collapse | Check that `CollapseControlID` is set and matches an element ID. Verify trigger element is clickable. |
| Content runs off-screen | Set `ExpandedSize` to limit expanded height or width. Enable `ScrollContents="true"` for scrollbars. |
| Multiple panels collapse together | Each CollapsiblePanelExtender must have a unique `TargetControlID`. Avoid reusing panel IDs. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [ConfirmButtonExtender](ConfirmButtonExtender.md) — Button confirmation dialogs
- [ModalPopupExtender](ModalPopupExtender.md) — Modal popup dialogs
- [Panel Component](../EditorControls/Panel.md) — BWFC Panel control documentation
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
