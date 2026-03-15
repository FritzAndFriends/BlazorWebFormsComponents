# ConfirmButtonExtender

The **ConfirmButtonExtender** displays a browser confirmation dialog when a user clicks a target button. If the user cancels the dialog, the button's click event is suppressed, preventing form submission or callback execution.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/ConfirmButtonExtender

## Features Supported in Blazor

- `ConfirmText` — The message displayed in the confirmation dialog
- `ConfirmOnFormSubmit` — Optionally confirm on form submission instead of button click
- `TargetControlID` — ID of the Button to enhance
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Features NOT Supported

- **DisplayModalPopupID** — Not implemented in v1; reserved for future use (custom modal instead of `confirm()`)

## Web Forms Syntax

```html
<asp:Button ID="btnDelete" Text="Delete" runat="server" />

<ajaxToolkit:ConfirmButtonExtender 
    ID="cbe"
    runat="server"
    TargetControlID="btnDelete"
    ConfirmText="Are you sure you want to delete this record?"
    OnClientCancel="return false;" />
```

## Blazor Migration

```razor
<Button ID="btnDelete" Text="Delete" OnClick="HandleDelete" />

<ConfirmButtonExtender 
    TargetControlID="btnDelete"
    ConfirmText="Are you sure you want to delete this record?" />

@code {
    void HandleDelete()
    {
        // This only runs if the user clicks OK in the confirm dialog
    }
}
```

**Migration is simple:** Just remove the `ajaxToolkit:` prefix and remove the `runat="server"` attribute. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the Button control to enhance |
| `ConfirmText` | `string` | `"Are you sure?"` | The message displayed in the confirmation dialog |
| `ConfirmOnFormSubmit` | `bool` | `false` | If `true`, show confirmation on form submission; if `false`, show on button click |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Confirmation on Click

```razor
@rendermode InteractiveServer

<Button ID="btnDelete" Text="Delete Record" OnClick="DeleteRecord" />

<ConfirmButtonExtender 
    TargetControlID="btnDelete"
    ConfirmText="This will permanently delete the record. Continue?" />

@code {
    void DeleteRecord()
    {
        // This only executes if user clicks OK
    }
}
```

### Confirmation on Form Submission

When `ConfirmOnFormSubmit` is `true`, the confirmation appears when the user submits the form, regardless of which button they click.

```razor
@rendermode InteractiveServer

<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <InputText @bind-Value="model.Name" placeholder="Name" />
    
    <Button ID="btnSubmit" Text="Submit" />
    
    <ConfirmButtonExtender 
        TargetControlID="btnSubmit"
        ConfirmText="Are you sure you want to submit this information?"
        ConfirmOnFormSubmit="true" />
</EditForm>

@code {
    private FormModel model = new FormModel();
    
    void HandleSubmit()
    {
        // This executes after user confirms
    }
}
```

### Multiple Buttons with Different Confirmations

```razor
@rendermode InteractiveServer

<Button ID="btnSave" Text="Save" OnClick="SaveRecord" />
<Button ID="btnDelete" Text="Delete" OnClick="DeleteRecord" />

<ConfirmButtonExtender 
    TargetControlID="btnSave"
    ConfirmText="Save changes?" />

<ConfirmButtonExtender 
    TargetControlID="btnDelete"
    ConfirmText="Permanently delete? This cannot be undone." />

@code {
    void SaveRecord() { /* Save */ }
    void DeleteRecord() { /* Delete */ }
}
```

### Dynamic Confirmation Messages

```razor
@rendermode InteractiveServer

<Button ID="btnDelete" Text="Delete" OnClick="Delete" />

<ConfirmButtonExtender 
    TargetControlID="btnDelete"
    ConfirmText="@GetConfirmMessage()" />

@code {
    private int recordCount = 0;
    
    string GetConfirmMessage()
    {
        return recordCount == 1
            ? "Delete 1 record?"
            : $"Delete {recordCount} records?";
    }
    
    void Delete()
    {
        // Delete...
    }
}
```

### Destructive Action Pattern

```razor
@rendermode InteractiveServer

<div style="border: 2px solid red; padding: 1rem; margin-bottom: 1rem;">
    <h3>Danger Zone</h3>
    <p>This action cannot be undone.</p>
    
    <Button ID="btnPermanentDelete" Text="Permanently Delete Account" 
            CssClass="btn-danger" OnClick="DeleteAccount" />
    
    <ConfirmButtonExtender 
        TargetControlID="btnPermanentDelete"
        ConfirmText="I understand this is permanent. Delete my account?" />
</div>

@code {
    void DeleteAccount()
    {
        // Account deletion logic
    }
}
```

## HTML Output

The ConfirmButtonExtender produces no HTML — it only attaches JavaScript behavior.

**Before interaction:**
```html
<button type="submit" id="btnDelete">Delete Record</button>
```

**On click:** Browser shows system confirmation dialog:
```
Are you sure you want to delete this record?
[Cancel] [OK]
```

**If user clicks OK:** Button's `OnClick` handler executes normally.
**If user clicks Cancel:** Button's `OnClick` handler is suppressed.

## Browser Behavior

The confirmation dialog is the browser's native `confirm()` function:
- Shows in the system's native style
- Blocks further JavaScript execution until dismissed
- Cannot be styled with CSS
- Works consistently across all browsers and devices

## Render Mode Requirements

The ConfirmButtonExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer

<Button ID="btnDelete" Text="Delete" />
<ConfirmButtonExtender TargetControlID="btnDelete" ConfirmText="Sure?" />
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization (no error thrown). The button works normally without confirmation.
- **JavaScript disabled:** Same as SSR — button functions without confirmation.
- **Module import fails:** Any JavaScript import errors are logged to browser console; button continues to function.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:ConfirmButtonExtender
   + <ConfirmButtonExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**
   ```diff
   - runat="server"
   - ID="cbe"
   ```

3. **Keep all properties the same**
   ```razor
   TargetControlID="btnDelete"
   ConfirmText="Are you sure?"
   ```

### Before (Web Forms)

```html
<asp:Button ID="btnDelete" Text="Delete" OnClick="btnDelete_Click" runat="server" />

<ajaxToolkit:ConfirmButtonExtender 
    ID="cbe"
    TargetControlID="btnDelete"
    ConfirmText="This action cannot be undone. Continue?"
    runat="server" />
```

### After (Blazor)

```razor
<Button ID="btnDelete" Text="Delete" OnClick="HandleDelete" />

<ConfirmButtonExtender 
    TargetControlID="btnDelete"
    ConfirmText="This action cannot be undone. Continue?" />

@code {
    void HandleDelete()
    {
        // Handle deletion
    }
}
```

## Best Practices

1. **Use for destructive actions** — Deletion, account closure, data purge
2. **Keep messages clear and short** — Users skim confirmation text
3. **Include action consequence** — Help users make informed decisions
4. **Test in your target browsers** — Native dialogs vary by browser
5. **Provide alternative paths** — Allow users to cancel/undo if possible

## Troubleshooting

| Issue | Solution |
|---|---|
| Confirmation not appearing | Verify `TargetControlID` matches button's `ID`. Ensure `@rendermode InteractiveServer` is set. Check browser console for errors. |
| Button click always fires | Check that `TargetControlID` is spelled correctly and matches exactly (case-sensitive). |
| Multiple confirmations for one click | Ensure only one extender targets each button. |
| Confirmation appears but ignored | Verify `ConfirmText` is not empty. Check for JavaScript errors in browser console. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [FilteredTextBoxExtender](FilteredTextBoxExtender.md) — Text input character filtering
- [Button Component](../EditorControls/Button.md) — BWFC Button control documentation
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
