# Ajax Control Toolkit Components

The **Ajax Control Toolkit** in BWFC provides Blazor components that emulate Ajax Control Toolkit extender components, enabling seamless migration of Ajax-enriched Web Forms pages to Blazor with minimal markup changes.

## What are Extenders?

Extenders are special components that **attach JavaScript behavior to existing HTML controls** without rendering any HTML themselves. They enhance a target control's functionality through client-side JavaScript.

### Key Characteristics

- **Target-based** — Extenders identify their target control by `TargetControlID`
- **Non-rendering** — Extenders produce no HTML output; they only inject behavior
- **JavaScript-powered** — All functionality runs client-side via ES modules
- **Declarative** — Attach to controls with simple Blazor syntax
- **Familiar API** — Same properties and patterns as the original Ajax Control Toolkit

### The Migration Advantage

Migration from Web Forms is remarkably simple:

| Web Forms (Ajax Control Toolkit) | BWFC Blazor |
|---|---|
| `<ajaxToolkit:ConfirmButtonExtender>` | `<ConfirmButtonExtender>` |
| `TargetControlID="btnSubmit"` | `TargetControlID="btnSubmit"` |
| All same properties | All same properties |

**You literally just remove the `ajaxToolkit:` prefix** — everything else stays the same!

## Available Components

### ConfirmButtonExtender
Displays a browser confirmation dialog when a button is clicked. If the user cancels, the click is suppressed.

[View Documentation →](ConfirmButtonExtender.md)

### FilteredTextBoxExtender
Restricts input in a TextBox to specified character types. Filters keystrokes in real-time and strips invalid characters on paste.

[View Documentation →](FilteredTextBoxExtender.md)

### ModalPopupExtender
Displays a target element as a modal popup with an overlay backdrop. Supports OK/Cancel actions, focus trapping, drag support, and Escape key dismissal.

[View Documentation →](ModalPopupExtender.md)

### CollapsiblePanelExtender
Adds collapse/expand functionality to a panel with smooth CSS transitions. Supports separate collapse/expand triggers, dynamic label updates, auto-collapse/expand on hover, and vertical/horizontal animations.

[View Documentation →](CollapsiblePanelExtender.md)

## Requirements

### Render Mode
Extender components require **InteractiveServer** render mode because they depend on JavaScript interoperability.

```razor
@rendermode InteractiveServer
```

### JavaScript Support
Your application must have `IJSRuntime` available and be able to execute ES modules. Extenders gracefully degrade in:
- Static Site Generation (SSR) mode without interactivity
- Pre-rendering scenarios

When JavaScript isn't available, extenders silently skip initialization (no errors thrown).

## Usage Pattern

All extender components follow this pattern:

```razor
@* 1. Render the target control (Button, TextBox, etc.) *@
<Button ID="btnConfirm" Text="Delete" />

@* 2. Attach the extender to the target *@
<ConfirmButtonExtender 
    TargetControlID="btnConfirm"
    ConfirmText="Are you really sure?" />
```

## Common Properties

All extender components inherit these properties from `BaseExtenderComponent`:

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the control to enhance |
| `BehaviorID` | `string` | TargetControlID | Optional behavior identifier for JS lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Render Mode Considerations

### InteractiveServer Mode

In `InteractiveServer` render mode, extenders work normally:
- JavaScript initializes automatically after first render
- Parameters can be updated and changes propagate to JavaScript
- Full interoperability available

### Static/SSR Mode

When extenders are placed in Static (non-interactive) components:
- JavaScript interop is **not available**
- Extenders detect this and skip initialization
- No exceptions are thrown — graceful degradation
- Consider wrapping in an `InteractiveServer` child component

### Recommended Pattern

```razor
@rendermode InteractiveServer

<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(Program).Assembly">
        <!-- Your routes here -->
    </Router>
</CascadingAuthenticationState>
```

## Troubleshooting

### Extender not activating
1. Verify `TargetControlID` matches the target control's `ID` attribute
2. Ensure the component is in `InteractiveServer` render mode
3. Check browser console for JavaScript errors
4. Verify browser allows dynamic module imports

### Target control not found
- Extenders initialize after first render; ensure the target control renders before the extender
- JavaScript looks for `document.getElementById(TargetControlID)`
- If target is dynamically created, initialize the extender after the target

## See Also

- [ConfirmButtonExtender](ConfirmButtonExtender.md) — Browser confirmation dialogs
- [FilteredTextBoxExtender](FilteredTextBoxExtender.md) — Character filtering for text input
- [Button Component](../EditorControls/Button.md) — The Button control
- [TextBox Component](../EditorControls/TextBox.md) — The TextBox control
