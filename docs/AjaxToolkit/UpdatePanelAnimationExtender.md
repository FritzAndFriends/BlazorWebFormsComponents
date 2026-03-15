# UpdatePanelAnimationExtender

The **UpdatePanelAnimationExtender** provides visual feedback animations when content is updating. It applies CSS classes and fade effects during loading and after updates complete, giving users clear indication that content is being refreshed. Useful for async operations and dynamic content loading.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/UpdatePanelAnimationExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the panel or container to animate
- `AlwaysFinishOnUpdatingAnimation` — Whether to complete the "updating" animation before starting "updated"
- `OnUpdatingCssClass` — CSS class to apply during update/loading state
- `OnUpdatedCssClass` — CSS class to apply after update completes
- `FadeInDuration` — Duration of the fade-in effect in seconds
- `FadeOutDuration` — Duration of the fade-out effect in seconds
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:Panel ID="pnlContent" runat="server">
    <p>Updating content...</p>
</asp:Panel>

<ajaxToolkit:UpdatePanelAnimationExtender
    ID="anim1"
    runat="server"
    TargetControlID="pnlContent"
    OnUpdatingCssClass="updating"
    OnUpdatedCssClass="updated"
    FadeOutDuration="0.3"
    FadeInDuration="0.3" />
```

## Blazor Migration

```razor
<Panel ID="pnlContent">
    <p>Updating content...</p>
</Panel>

<UpdatePanelAnimationExtender
    TargetControlID="pnlContent"
    OnUpdatingCssClass="updating"
    OnUpdatedCssClass="updated"
    FadeOutDuration="0.3"
    FadeInDuration="0.3" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the panel or container to animate |
| `AlwaysFinishOnUpdatingAnimation` | `bool` | `false` | Whether to complete the "updating" animation before starting the "updated" animation |
| `OnUpdatingCssClass` | `string` | `""` | CSS class to apply during update/loading state |
| `OnUpdatedCssClass` | `string` | `""` | CSS class to apply after update completes |
| `FadeInDuration` | `double` | `0.3` | Duration in seconds for the fade-in effect after update completes |
| `FadeOutDuration` | `double` | `0.3` | Duration in seconds for the fade-out effect before update starts |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Update Animation

```razor
@rendermode InteractiveServer

<Panel ID="pnlData" style="padding: 20px; background-color: white; border: 1px solid #ccc;">
    <p>Content loaded</p>
</Panel>

<UpdatePanelAnimationExtender
    TargetControlID="pnlData"
    OnUpdatingCssClass="loading"
    OnUpdatedCssClass="loaded"
    FadeOutDuration="0.2"
    FadeInDuration="0.3" />

<style>
    .loading {
        opacity: 0.5;
        background-color: #f5f5f5;
    }
    
    .loaded {
        background-color: #e8f5e9;
    }
</style>
```

### Loading Spinner Animation

```razor
@rendermode InteractiveServer

<Panel ID="pnlResults" style="padding: 20px;">
    <p>Results will appear here...</p>
</Panel>

<UpdatePanelAnimationExtender
    TargetControlID="pnlResults"
    OnUpdatingCssClass="updating-with-spinner"
    OnUpdatedCssClass="update-complete"
    FadeOutDuration="0.2"
    FadeInDuration="0.5" />

<style>
    .updating-with-spinner {
        position: relative;
        pointer-events: none;
    }
    
    .updating-with-spinner::before {
        content: '';
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 30px;
        height: 30px;
        border: 3px solid #f3f3f3;
        border-top: 3px solid #2196F3;
        border-radius: 50%;
        animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
        0% { transform: translate(-50%, -50%) rotate(0deg); }
        100% { transform: translate(-50%, -50%) rotate(360deg); }
    }
    
    .update-complete {
        background-color: #c8e6c9;
        border: 1px solid #4caf50;
    }
</style>
```

### Gradient Fade Effect

```razor
@rendermode InteractiveServer

<Panel ID="pnlInfo" style="padding: 20px; background-color: white; border-radius: 5px;">
    <p>Information updated</p>
</Panel>

<UpdatePanelAnimationExtender
    TargetControlID="pnlInfo"
    OnUpdatingCssClass="fade-out"
    OnUpdatedCssClass="fade-in"
    FadeOutDuration="0.3"
    FadeInDuration="0.5"
    AlwaysFinishOnUpdatingAnimation="true" />

<style>
    @@keyframes fadeOut {
        from { opacity: 1; }
        to { opacity: 0.3; }
    }
    
    @@keyframes fadeIn {
        from { opacity: 0.3; }
        to { opacity: 1; }
    }
    
    .fade-out {
        animation: fadeOut 0.3s ease-out forwards;
    }
    
    .fade-in {
        animation: fadeIn 0.5s ease-in forwards;
    }
</style>
```

### Color Pulse Animation

```razor
@rendermode InteractiveServer

<Panel ID="pnlNotification" style="padding: 20px; background-color: white; border-left: 4px solid #2196F3;">
    <p>Status updated</p>
</Panel>

<UpdatePanelAnimationExtender
    TargetControlID="pnlNotification"
    OnUpdatingCssClass="pulse-updating"
    OnUpdatedCssClass="pulse-updated"
    FadeOutDuration="0.2"
    FadeInDuration="0.4" />

<style>
    @@keyframes pulse {
        0% { background-color: white; }
        50% { background-color: #E3F2FD; }
        100% { background-color: white; }
    }
    
    .pulse-updating {
        animation: pulse 0.6s ease-in-out;
        opacity: 0.7;
    }
    
    .pulse-updated {
        animation: pulse 0.8s ease-in-out 2;
    }
</style>
```

## HTML Output

The UpdatePanelAnimationExtender produces no HTML itself — it attaches JavaScript behavior to the target panel, applying CSS classes based on update state.

## JavaScript Interop

The UpdatePanelAnimationExtender loads `update-panel-animation-extender.js` as an ES module. JavaScript handles:

- Detecting content update events
- Applying "updating" CSS class when update begins
- Removing "updating" class when complete
- Applying "updated" CSS class after update
- Fade effects using opacity transitions
- Sequencing animations if `AlwaysFinishOnUpdatingAnimation` is enabled
- Managing animation timing and duration

## Render Mode Requirements

The UpdatePanelAnimationExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. Panel updates occur without animation.
- **JavaScript disabled:** Same as SSR — No animations are applied.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:UpdatePanelAnimationExtender
   + <UpdatePanelAnimationExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **CSS classes and animation durations stay the same**
   ```diff
   - FadeOutDuration="0.3"
   + FadeOutDuration="0.3"
   ```

### Before (Web Forms)

```html
<asp:Panel ID="pnlData" runat="server">
    <p>Content</p>
</asp:Panel>

<ajaxToolkit:UpdatePanelAnimationExtender
    ID="anim1"
    TargetControlID="pnlData"
    OnUpdatingCssClass="loading"
    OnUpdatedCssClass="loaded"
    FadeOutDuration="0.3"
    FadeInDuration="0.3"
    runat="server" />
```

### After (Blazor)

```razor
<Panel ID="pnlData">
    <p>Content</p>
</Panel>

<UpdatePanelAnimationExtender
    TargetControlID="pnlData"
    OnUpdatingCssClass="loading"
    OnUpdatedCssClass="loaded"
    FadeOutDuration="0.3"
    FadeInDuration="0.3" />
```

## Best Practices

1. **Keep animations short** — Use 0.2-0.5 seconds for snappy feel; longer durations can feel sluggish
2. **Use meaningful CSS classes** — Make updating/updated states visually distinct
3. **Avoid blocking interactions** — Use `pointer-events: none` during updates if appropriate
4. **Test with slow networks** — Ensure animations work well even with delayed updates
5. **Combine with loading indicators** — Show spinners or progress during the updating state
6. **Use `AlwaysFinishOnUpdatingAnimation` carefully** — Only enable if you need sequential animations

## Troubleshooting

| Issue | Solution |
|---|---|
| Animations not appearing | Verify `TargetControlID` matches the panel's ID. Ensure `@rendermode InteractiveServer` is set. Define CSS classes. |
| Animation timing seems off | Adjust `FadeOutDuration` and `FadeInDuration` values. Try 0.2-0.5 seconds for best results. |
| CSS classes not applied | Verify the CSS class names in `OnUpdatingCssClass` and `OnUpdatedCssClass` are defined in your stylesheets. |
| Content blocked during update | If content should remain clickable, avoid `pointer-events: none` or set it only on specific elements. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [Panel Component](../LayoutControls/Panel.md) — The Panel control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
