# ResizableControlExtender

The **ResizableControlExtender** allows users to resize an element by dragging its edges or a resize handle (typically positioned at the bottom-right corner). It supports optional minimum and maximum size constraints to prevent elements from becoming too small or too large.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/ResizableControlExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the element to make resizable
- `HandleCssClass` — CSS class for the resize handle element
- `ResizableCssClass` — CSS class applied while resizing
- `MinimumWidth` — Minimum width constraint in pixels
- `MinimumHeight` — Minimum height constraint in pixels
- `MaximumWidth` — Maximum width constraint in pixels (0 = no limit)
- `MaximumHeight` — Maximum height constraint in pixels (0 = no limit)
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:Panel ID="pnlResizable" runat="server" style="width: 300px; height: 200px; border: 1px solid black;">
    <p>Resize me from the bottom-right corner!</p>
    <div id="resizeHandle" class="resize-handle" style="position: absolute; bottom: 0; right: 0; width: 10px; height: 10px; background: #333; cursor: nwse-resize;"></div>
</asp:Panel>

<ajaxToolkit:ResizableControlExtender
    ID="resize1"
    runat="server"
    TargetControlID="pnlResizable"
    HandleCssClass="resize-handle"
    MinimumWidth="100"
    MinimumHeight="100"
    MaximumWidth="600"
    MaximumHeight="500" />
```

## Blazor Migration

```razor
<Panel ID="pnlResizable" style="width: 300px; height: 200px; border: 1px solid black;">
    <p>Resize me from the bottom-right corner!</p>
    <div id="resizeHandle" class="resize-handle" style="position: absolute; bottom: 0; right: 0; width: 10px; height: 10px; background: #333; cursor: nwse-resize;"></div>
</Panel>

<ResizableControlExtender
    TargetControlID="pnlResizable"
    HandleCssClass="resize-handle"
    MinimumWidth="100"
    MinimumHeight="100"
    MaximumWidth="600"
    MaximumHeight="500" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the element to make resizable |
| `HandleCssClass` | `string` | `""` | CSS class for the resize handle element (usually positioned at bottom-right) |
| `ResizableCssClass` | `string` | `""` | CSS class applied to the target while it is being resized |
| `MinimumWidth` | `int` | `0` | Minimum width constraint in pixels; 0 = no limit |
| `MinimumHeight` | `int` | `0` | Minimum height constraint in pixels; 0 = no limit |
| `MaximumWidth` | `int` | `0` | Maximum width constraint in pixels; 0 = no limit |
| `MaximumHeight` | `int` | `0` | Maximum height constraint in pixels; 0 = no limit |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Resizable Panel

```razor
@rendermode InteractiveServer

<Panel ID="pnlBox" style="width: 300px; height: 200px; border: 1px solid #ccc; padding: 10px; position: relative; overflow: auto;">
    <p>Drag the handle in the corner to resize this panel.</p>
    <div class="resize-handle"></div>
</Panel>

<ResizableControlExtender TargetControlID="pnlBox" HandleCssClass="resize-handle" />

<style>
    .resize-handle {
        position: absolute;
        bottom: 0;
        right: 0;
        width: 15px;
        height: 15px;
        background: #333;
        cursor: nwse-resize;
    }
</style>
```

### Resizable with Size Constraints

```razor
@rendermode InteractiveServer

<Panel ID="pnlWindow" style="width: 400px; height: 300px; border: 2px solid #2196F3; position: relative; background: white;">
    <div style="background: #2196F3; color: white; padding: 10px; font-weight: bold;">Resizable Window</div>
    <div style="padding: 20px; overflow: auto; height: calc(100% - 50px);">
        <p>Minimum size: 200x150px</p>
        <p>Maximum size: 800x600px</p>
    </div>
    <div class="resize-corner"></div>
</Panel>

<ResizableControlExtender
    TargetControlID="pnlWindow"
    HandleCssClass="resize-corner"
    MinimumWidth="200"
    MinimumHeight="150"
    MaximumWidth="800"
    MaximumHeight="600"
    ResizableCssClass="resizing" />

<style>
    .resize-corner {
        position: absolute;
        bottom: 0;
        right: 0;
        width: 20px;
        height: 20px;
        background: linear-gradient(135deg, transparent 50%, #2196F3 50%);
        cursor: nwse-resize;
    }
    
    .resizing {
        opacity: 0.8;
    }
</style>
```

### Resizable Text Area

```razor
@rendermode InteractiveServer

<Panel ID="pnlTextArea" style="width: 500px; height: 300px; position: relative; border: 1px solid #999;">
    <TextArea ID="txtContent" style="width: 100%; height: 100%; border: none; padding: 10px; resize: none;"></TextArea>
    <div class="resize-handle"></div>
</Panel>

<ResizableControlExtender
    TargetControlID="pnlTextArea"
    HandleCssClass="resize-handle"
    MinimumWidth="250"
    MinimumHeight="150"
    MaximumWidth="1000"
    MaximumHeight="800" />

<style>
    .resize-handle {
        position: absolute;
        bottom: 2px;
        right: 2px;
        width: 18px;
        height: 18px;
        background-image: repeating-linear-gradient(45deg, #ccc, #ccc 2px, transparent 2px, transparent 5px);
        cursor: nwse-resize;
    }
</style>
```

## HTML Output

The ResizableControlExtender produces no HTML itself — it attaches JavaScript behavior to the target element. You must provide the resize handle as part of your panel's HTML.

## JavaScript Interop

The ResizableControlExtender loads `resizable-control-extender.js` as an ES module. JavaScript handles:

- Detecting mouse down on the resize handle
- Tracking mouse movement while resizing
- Updating the element's width and height in real-time
- Enforcing minimum and maximum size constraints
- Applying/removing resize CSS classes
- Handling mouse up to stop resizing
- Preventing text selection during resize

## Render Mode Requirements

The ResizableControlExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The Panel displays at its initial size.
- **JavaScript disabled:** Same as SSR — Panel is not resizable.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:ResizableControlExtender
   + <ResizableControlExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Size constraints use integers, not strings**
   ```diff
   - MinimumWidth="200"
   + MinimumWidth="200"
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlBox" runat="server" style="width: 300px; height: 200px; border: 1px solid black; position: relative;">
    <p>Content</p>
    <div class="handle"></div>
</asp:Panel>

<ajaxToolkit:ResizableControlExtender
    ID="resize1"
    TargetControlID="pnlBox"
    HandleCssClass="handle"
    MinimumWidth="150"
    MaximumWidth="800"
    runat="server" />
```

### After (Blazor)

```razor
<Panel ID="pnlBox" style="width: 300px; height: 200px; border: 1px solid black; position: relative;">
    <p>Content</p>
    <div class="handle"></div>
</Panel>

<ResizableControlExtender
    TargetControlID="pnlBox"
    HandleCssClass="handle"
    MinimumWidth="150"
    MaximumWidth="800" />
```

## Best Practices

1. **Use position: relative or absolute** — Ensure the target has proper positioning for the handle
2. **Style the handle clearly** — Make the resize handle visually obvious with contrasting colors
3. **Set overflow: auto** — Use `overflow: auto` on the panel so content scrolls when resized small
4. **Provide constraints** — Set reasonable min/max sizes to prevent broken layouts
5. **Test edge cases** — Verify behavior when resizing to minimum/maximum sizes

## Troubleshooting

| Issue | Solution |
|---|---|
| Handle not visible | Verify `HandleCssClass` references a styled element within the panel. Check CSS positioning. |
| Resizing not working | Ensure `@rendermode InteractiveServer` is set and `TargetControlID` matches the panel's ID. |
| Size constraints ignored | Verify min/max values are integers and make sense (min < max). A value of 0 means no limit. |
| Content overlaps handle | Adjust panel padding or use `overflow: auto` to prevent content overlap. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- Panel Component — The Panel control (documentation coming soon)
- [DragPanelExtender](DragPanelExtender.md) — Make panels draggable
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
