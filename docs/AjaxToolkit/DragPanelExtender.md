# DragPanelExtender

The **DragPanelExtender** makes a panel or container element draggable. Users can click and drag the panel to reposition it freely on the page. Optionally, a specific handle or title bar can be designated as the drag grip, restricting dragging to that element.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/DragPanelExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the panel or container to make draggable
- `DragHandleID` — Optional ID of the element to use as the drag handle
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:Panel ID="pnlDialog" runat="server" style="border: 1px solid black; width: 300px;">
    <div id="pnlHeader" style="background-color: #333; padding: 5px;">
        <h3>My Dialog</h3>
    </div>
    <div style="padding: 10px;">
        <p>Drag me by the header!</p>
    </div>
</asp:Panel>

<ajaxToolkit:DragPanelExtender
    ID="drag1"
    runat="server"
    TargetControlID="pnlDialog"
    DragHandleID="pnlHeader" />
```

## Blazor Migration

```razor
<Panel ID="pnlDialog" style="border: 1px solid black; width: 300px;">
    <div id="pnlHeader" style="background-color: #333; padding: 5px;">
        <h3>My Dialog</h3>
    </div>
    <div style="padding: 10px;">
        <p>Drag me by the header!</p>
    </div>
</Panel>

<DragPanelExtender
    TargetControlID="pnlDialog"
    DragHandleID="pnlHeader" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the panel or container to make draggable |
| `DragHandleID` | `string` | `""` | Optional ID of an element to use as the drag handle; if not set, entire panel is draggable |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Draggable Entire Panel

```razor
@rendermode InteractiveServer

<Panel ID="pnlBox" style="width: 300px; border: 1px solid #ccc; padding: 10px; background: white; cursor: move;">
    <p>Drag this panel anywhere!</p>
</Panel>

<DragPanelExtender TargetControlID="pnlBox" />
```

### Draggable with Handle

```razor
@rendermode InteractiveServer

<Panel ID="pnlWindow" style="width: 400px; border: 1px solid black;">
    <div id="titleBar" style="background-color: #4CAF50; color: white; padding: 10px; cursor: move;">
        Window Title
    </div>
    <div style="padding: 20px; background-color: white;">
        <p>This can only be dragged by the title bar.</p>
    </div>
</Panel>

<DragPanelExtender
    TargetControlID="pnlWindow"
    DragHandleID="titleBar" />
```

### Floating Dialog Box

```razor
@rendermode InteractiveServer

<Panel ID="pnlDialog" style="position: absolute; top: 100px; left: 100px; width: 350px; border: 1px solid #999; box-shadow: 0 2px 5px rgba(0,0,0,0.2); background: white;">
    <div id="dialogHeader" style="background: linear-gradient(to right, #2196F3, #1976D2); color: white; padding: 10px; font-weight: bold; cursor: move;">
        Settings
    </div>
    <div style="padding: 20px;">
        <form>
            <input type="checkbox" /> Enable notifications
            <br /><br />
            <button type="button" class="btn btn-primary">Save</button>
        </form>
    </div>
</Panel>

<DragPanelExtender
    TargetControlID="pnlDialog"
    DragHandleID="dialogHeader" />
```

## HTML Output

The DragPanelExtender produces no HTML itself — it attaches JavaScript behavior to the target panel. The panel retains its original HTML structure and styling.

## JavaScript Interop

The DragPanelExtender loads `drag-panel-extender.js` as an ES module. JavaScript handles:

- Detecting mouse down on the panel or handle
- Tracking mouse movement while dragging
- Updating the panel's position in real-time
- Handling mouse up to stop dragging
- Managing z-index to keep dragged panel on top
- Preventing text selection during drag

## Render Mode Requirements

The DragPanelExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The Panel displays statically.
- **JavaScript disabled:** Same as SSR — Panel is not draggable.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:DragPanelExtender
   + <DragPanelExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Panel element stays the same**
   ```diff
   - <asp:Panel ID="pnlDialog" runat="server">
   + <Panel ID="pnlDialog">
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlDialog" runat="server" style="width: 300px; border: 1px solid black;">
    <div id="header" style="background: #333; color: white; padding: 5px; cursor: move;">Title</div>
    <div style="padding: 10px;">Content</div>
</asp:Panel>

<ajaxToolkit:DragPanelExtender
    ID="drag1"
    TargetControlID="pnlDialog"
    DragHandleID="header"
    runat="server" />
```

### After (Blazor)

```razor
<Panel ID="pnlDialog" style="width: 300px; border: 1px solid black;">
    <div id="header" style="background: #333; color: white; padding: 5px; cursor: move;">Title</div>
    <div style="padding: 10px;">Content</div>
</Panel>

<DragPanelExtender
    TargetControlID="pnlDialog"
    DragHandleID="header" />
```

## Best Practices

1. **Set position: absolute** — Use CSS to position the panel before dragging starts
2. **Use a handle** — Provide a `DragHandleID` to prevent accidental dragging of content
3. **Apply cursor: move** — Use CSS to indicate the element is draggable
4. **Preserve boundaries** — Monitor panel position to keep it within viewport
5. **Apply z-index** — Ensure dragged panels appear above other page content

## Troubleshooting

| Issue | Solution |
|---|---|
| Panel not draggable | Verify `TargetControlID` matches the Panel's `ID`. Ensure `@rendermode InteractiveServer` is set. |
| Dragging causes text selection | Add `user-select: none` CSS or use `DragHandleID` to restrict dragging to a specific element. |
| Handle not working as expected | Ensure `DragHandleID` references a valid element within the panel with the correct ID. |
| Panel disappears during drag | Check z-index values and ensure the panel's position is compatible with absolute positioning. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [Panel Component](../LayoutControls/Panel.md) — The Panel control
- [ResizableControlExtender](ResizableControlExtender.md) — Make panels resizable
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
