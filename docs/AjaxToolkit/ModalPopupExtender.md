# ModalPopupExtender

The **ModalPopupExtender** displays a target element as a modal popup dialog with an overlay backdrop. It supports OK/Cancel button actions, focus trapping, optional drag behavior, and Escape key dismissal, enabling modal interactions with minimal markup.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/ModalPopupExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the control that triggers the modal display
- `PopupControlID` — ID of the element to display as the modal popup
- `BackgroundCssClass` — CSS class applied to the overlay backdrop
- `OkControlID` — ID of the button that confirms and closes the modal
- `CancelControlID` — ID of the button that cancels and closes the modal
- `OnOkScript` — JavaScript code to execute when OK is clicked
- `OnCancelScript` — JavaScript code to execute when Cancel is clicked or Escape is pressed
- `DropShadow` — Whether to add a drop shadow effect to the popup
- `Drag` — Whether the popup can be dragged by the user
- `PopupDragHandleControlID` — ID of the element to use as a drag handle (optional)
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:Button ID="btnOpenModal" Text="Open Settings" runat="server" />

<div ID="modalSettings" style="display: none; width: 400px; background: white; padding: 20px;">
    <h2>Settings</h2>
    <p>This is the modal content.</p>
    
    <button ID="btnOK">OK</button>
    <button ID="btnCancel">Cancel</button>
</div>

<ajaxToolkit:ModalPopupExtender 
    ID="mpe"
    runat="server"
    TargetControlID="btnOpenModal"
    PopupControlID="modalSettings"
    BackgroundCssClass="modalBackground"
    OkControlID="btnOK"
    CancelControlID="btnCancel"
    DropShadow="true"
    Drag="true" />
```

## Blazor Migration

```razor
<Button ID="btnOpenModal" Text="Open Settings" />

<div ID="modalSettings" style="display: none; width: 400px; background: white; padding: 20px;">
    <h2>Settings</h2>
    <p>This is the modal content.</p>
    
    <button ID="btnOK">OK</button>
    <button ID="btnCancel">Cancel</button>
</div>

<ModalPopupExtender 
    TargetControlID="btnOpenModal"
    PopupControlID="modalSettings"
    BackgroundCssClass="modalBackground"
    OkControlID="btnOK"
    CancelControlID="btnCancel"
    DropShadow="true"
    Drag="true" />
```

**Migration is simple:** Just remove the `ajaxToolkit:` prefix and remove the `runat="server"` and `ID` attributes. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the control that opens the modal when clicked |
| `PopupControlID` | `string` | (required) | ID of the element to display as the modal popup |
| `BackgroundCssClass` | `string` | `""` | CSS class applied to the overlay backdrop |
| `OkControlID` | `string` | `""` | ID of the button that confirms and closes the modal |
| `CancelControlID` | `string` | `""` | ID of the button that cancels and closes the modal |
| `OnOkScript` | `string` | `""` | JavaScript code to execute when OK button is clicked |
| `OnCancelScript` | `string` | `""` | JavaScript code to execute when Cancel button is clicked or Escape key is pressed |
| `DropShadow` | `bool` | `false` | Whether to add a drop shadow effect to the popup |
| `Drag` | `bool` | `false` | Whether the popup can be dragged by the user |
| `PopupDragHandleControlID` | `string` | `""` | ID of the element to use as a drag handle; if empty, entire popup is draggable |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Modal Confirmation

```razor
@rendermode InteractiveServer

<Button ID="btnDelete" Text="Delete" />

<div ID="confirmDialog" style="display: none; width: 350px; background: white; padding: 20px; border-radius: 8px;">
    <h3>Confirm Action</h3>
    <p>Are you sure you want to delete this item?</p>
    <button ID="btnConfirm" style="padding: 8px 16px; margin-right: 10px;">Yes, Delete</button>
    <button ID="btnDismiss" style="padding: 8px 16px;">Cancel</button>
</div>

<ModalPopupExtender 
    TargetControlID="btnDelete"
    PopupControlID="confirmDialog"
    OkControlID="btnConfirm"
    CancelControlID="btnDismiss"
    BackgroundCssClass="backdrop" />

<style>
    .backdrop {
        background-color: rgba(0, 0, 0, 0.5);
    }
</style>
```

### Modal with Drop Shadow and Drag Support

```razor
@rendermode InteractiveServer

<Button ID="btnSettings" Text="Open Settings" />

<div ID="settingsDialog" style="display: none; width: 400px; background: white; padding: 0; border-radius: 8px; overflow: hidden;">
    <div ID="dragHandle" style="background: #333; color: white; padding: 15px; cursor: move;">
        Settings
    </div>
    
    <div style="padding: 20px;">
        <label>
            Option 1:
            <input type="checkbox" @bind-checked="@option1" />
        </label>
        <br />
        <label>
            Option 2:
            <input type="checkbox" @bind-checked="@option2" />
        </label>
    </div>
    
    <div style="padding: 20px; border-top: 1px solid #eee; display: flex; gap: 10px; justify-content: flex-end;">
        <button ID="btnSave" style="padding: 8px 16px; background: #007bff; color: white; border: none; cursor: pointer; border-radius: 4px;">Save</button>
        <button ID="btnCancel" style="padding: 8px 16px; background: #6c757d; color: white; border: none; cursor: pointer; border-radius: 4px;">Cancel</button>
    </div>
</div>

<ModalPopupExtender 
    TargetControlID="btnSettings"
    PopupControlID="settingsDialog"
    OkControlID="btnSave"
    CancelControlID="btnCancel"
    BackgroundCssClass="modal-backdrop"
    DropShadow="true"
    Drag="true"
    PopupDragHandleControlID="dragHandle" />

<style>
    .modal-backdrop {
        background-color: rgba(0, 0, 0, 0.6);
    }
</style>

@code {
    private bool option1 = false;
    private bool option2 = false;
}
```

### Modal with JavaScript Callbacks

```razor
@rendermode InteractiveServer

<Button ID="btnAction" Text="Perform Action" />

<div ID="actionModal" style="display: none; width: 400px; background: white; padding: 20px; border-radius: 8px;">
    <h3>Perform Action</h3>
    <p>This action will process the selected items.</p>
    
    <button ID="btnProceed" style="padding: 8px 16px; margin-right: 10px;">Proceed</button>
    <button ID="btnCancel" style="padding: 8px 16px;">Cancel</button>
</div>

<ModalPopupExtender 
    TargetControlID="btnAction"
    PopupControlID="actionModal"
    OkControlID="btnProceed"
    CancelControlID="btnCancel"
    BackgroundCssClass="backdrop"
    OnOkScript="onModalOK()"
    OnCancelScript="onModalCancel()" />

<script>
    function onModalOK() {
        console.log('User confirmed action');
    }
    
    function onModalCancel() {
        console.log('User cancelled action');
    }
</script>

<style>
    .backdrop {
        background-color: rgba(0, 0, 0, 0.5);
    }
</style>
```

### Modal Form Dialog

```razor
@rendermode InteractiveServer

<Button ID="btnAddItem" Text="Add New Item" />

<div ID="formDialog" style="display: none; width: 450px; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);">
    <h3>Add New Item</h3>
    
    <div style="margin: 15px 0;">
        <label for="itemName">Item Name:</label>
        <input type="text" ID="itemName" @bind-value="@itemName" style="width: 100%; padding: 8px; margin-top: 5px; border: 1px solid #ddd; border-radius: 4px;" />
    </div>
    
    <div style="margin: 15px 0;">
        <label for="itemCategory">Category:</label>
        <select ID="itemCategory" @bind="@itemCategory" style="width: 100%; padding: 8px; margin-top: 5px; border: 1px solid #ddd; border-radius: 4px;">
            <option value="">Select...</option>
            <option value="electronics">Electronics</option>
            <option value="clothing">Clothing</option>
            <option value="other">Other</option>
        </select>
    </div>
    
    <div style="margin-top: 25px; display: flex; gap: 10px; justify-content: flex-end;">
        <button ID="btnAdd" style="padding: 10px 20px; background: #28a745; color: white; border: none; cursor: pointer; border-radius: 4px;">Add</button>
        <button ID="btnClose" style="padding: 10px 20px; background: #6c757d; color: white; border: none; cursor: pointer; border-radius: 4px;">Close</button>
    </div>
</div>

<ModalPopupExtender 
    TargetControlID="btnAddItem"
    PopupControlID="formDialog"
    OkControlID="btnAdd"
    CancelControlID="btnClose"
    BackgroundCssClass="form-modal-backdrop"
    DropShadow="true" />

<style>
    .form-modal-backdrop {
        background-color: rgba(0, 0, 0, 0.6);
        z-index: 999;
    }
</style>

@code {
    private string itemName = "";
    private string itemCategory = "";
}
```

## HTML Output

The ModalPopupExtender produces no HTML itself — it only attaches JavaScript behavior to the popup element.

**Before (initial state):**
```html
<button id="btnDelete">Delete</button>

<div id="confirmDialog" style="display: none; width: 350px;">
    <!-- Dialog content -->
</div>
```

**When button is clicked:**
1. Overlay backdrop is created and inserted
2. Modal popup is shown (display changes from none to block)
3. Modal receives focus
4. Backdrop covers the page behind the modal

**On OK or Cancel:**
- Modal and backdrop are hidden or removed
- Focus returns to the trigger button
- Associated callback script executes (if provided)

## Modal Behavior

### Opening the Modal

- Clicking the target control (e.g., button) opens the modal
- An overlay backdrop appears behind the modal
- The modal receives keyboard focus
- Escape key closes the modal (executes OnCancelScript)

### Modal Positioning

- Modal is centered on the page
- Backdrop covers the entire viewport
- User cannot interact with page elements behind the modal

### Closing the Modal

- **OK button:** Executes `OnOkScript`, then closes modal
- **Cancel button:** Executes `OnCancelScript`, then closes modal
- **Escape key:** Executes `OnCancelScript`, then closes modal

### Drag Support

When `Drag="true"`:
- Entire modal can be dragged by the user by default
- If `PopupDragHandleControlID` is set, only that element serves as the drag handle
- Dragging moves the modal within the viewport

### Styling Notes

- The backdrop is created as a `<div>` with `position: fixed; top: 0; left: 0`
- Apply your CSS class to the `BackgroundCssClass` property to customize overlay appearance
- Use `opacity`, `background-color`, or `backdrop-filter` CSS properties for visual effects
- The modal itself retains the styling you define in your markup

## Render Mode Requirements

The ModalPopupExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer

<Button ID="btnOpen" Text="Open" />
<div ID="popup">Modal content</div>
<ModalPopupExtender TargetControlID="btnOpen" PopupControlID="popup" />
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The modal element remains hidden and the target button doesn't open it.
- **JavaScript disabled:** Same as SSR — modal doesn't function (element stays hidden).
- **Module import fails:** Any JavaScript errors are logged to browser console; button continues to function but modal doesn't open.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:ModalPopupExtender
   + <ModalPopupExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**
   ```diff
   - runat="server"
   - ID="mpe"
   ```

3. **Keep all properties the same**
   ```razor
   TargetControlID="btnOpen"
   PopupControlID="myModal"
   OkControlID="btnOK"
   CancelControlID="btnCancel"
   ```

### Before (Web Forms)

```html
<asp:Button ID="btnOpen" Text="Open" runat="server" />

<div ID="myModal" style="display: none; background: white; padding: 20px;">
    <p>Modal content</p>
    <button ID="btnOK">OK</button>
    <button ID="btnCancel">Cancel</button>
</div>

<ajaxToolkit:ModalPopupExtender 
    ID="mpe"
    TargetControlID="btnOpen"
    PopupControlID="myModal"
    OkControlID="btnOK"
    CancelControlID="btnCancel"
    BackgroundCssClass="modalBackground"
    runat="server" />
```

### After (Blazor)

```razor
<Button ID="btnOpen" Text="Open" />

<div ID="myModal" style="display: none; background: white; padding: 20px;">
    <p>Modal content</p>
    <button ID="btnOK">OK</button>
    <button ID="btnCancel">Cancel</button>
</div>

<ModalPopupExtender 
    TargetControlID="btnOpen"
    PopupControlID="myModal"
    OkControlID="btnOK"
    CancelControlID="btnCancel"
    BackgroundCssClass="modalBackground" />
```

## Best Practices

1. **Use semantic HTML inside modals** — Structure with `<h3>`, `<p>`, proper labels for inputs
2. **Provide visual feedback** — Use distinct colors for OK/Cancel buttons (green/blue vs. gray)
3. **Center content** — Use flexbox or grid on the modal container for readable layouts
4. **Handle Escape key** — Always implement `OnCancelScript` to match browser expectations
5. **Set a reasonable width** — 350–500px is typical; avoid full-screen modals
6. **Test drag behavior** — If dragging is enabled, ensure header is clearly draggable
7. **Minimize content** — Users prefer quick decisions; keep modal content focused
8. **Accessibility:** Use proper `<label>` elements for form inputs inside modals

## Troubleshooting

| Issue | Solution |
|---|---|
| Modal not opening | Verify `TargetControlID` matches button's `ID`. Ensure `@rendermode InteractiveServer` is set. Check browser console for JavaScript errors. |
| Modal always visible | Verify popup element has `style="display: none;"` or `display: none;` CSS. Modal should be hidden initially. |
| Can't close modal | Verify `OkControlID` and `CancelControlID` button IDs match their `ID` attributes. Ensure buttons are inside the modal container. |
| Backdrop not styled | Verify `BackgroundCssClass` is defined in your CSS. Check CSS class name spelling (case-sensitive). |
| Drag not working | Verify `Drag="true"` is set. If using `PopupDragHandleControlID`, ensure the ID matches exactly. Try without drag handle first to test. |
| Multiple modals open at once | Avoid triggering multiple ModalPopupExtenders simultaneously. Use a queue or state management to control which modal is visible. |
| Modal behind other elements | Check CSS `z-index` values. Backdrop and modal should have high z-index values (999+). |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [ConfirmButtonExtender](ConfirmButtonExtender.md) — Button confirmation dialogs
- [Button Component](../EditorControls/Button.md) — BWFC Button control documentation
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
