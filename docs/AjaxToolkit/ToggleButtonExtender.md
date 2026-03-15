# ToggleButtonExtender

The **ToggleButtonExtender** replaces a target checkbox with a clickable image that toggles between checked and unchecked states. It supports separate images for checked, unchecked, hover, and disabled states, providing a visually rich alternative to standard checkboxes.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/ToggleButtonExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the checkbox to enhance with toggle image behavior
- `ImageWidth` — Width of the toggle image in pixels
- `ImageHeight` — Height of the toggle image in pixels
- `UncheckedImageUrl` — Image URL for the unchecked state
- `CheckedImageUrl` — Image URL for the checked state
- `UncheckedImageAlternateText` — Alt text for the unchecked image
- `CheckedImageAlternateText` — Alt text for the checked image
- `CheckedImageOverUrl` — Hover image URL when checked
- `UncheckedImageOverUrl` — Hover image URL when unchecked
- `DisabledUncheckedImageUrl` — Image URL when disabled and unchecked
- `DisabledCheckedImageUrl` — Image URL when disabled and checked
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## Web Forms Syntax

```html
<asp:CheckBox ID="chkSubscribe" runat="server" />

<ajaxToolkit:ToggleButtonExtender
    ID="tbe1"
    runat="server"
    TargetControlID="chkSubscribe"
    ImageWidth="24"
    ImageHeight="24"
    UncheckedImageUrl="~/images/toggle-off.png"
    CheckedImageUrl="~/images/toggle-on.png"
    UncheckedImageAlternateText="Not subscribed"
    CheckedImageAlternateText="Subscribed"
    CheckedImageOverUrl="~/images/toggle-on-hover.png"
    UncheckedImageOverUrl="~/images/toggle-off-hover.png" />
```

## Blazor Migration

```razor
<CheckBox ID="chkSubscribe" />

<ToggleButtonExtender
    TargetControlID="chkSubscribe"
    ImageWidth="24"
    ImageHeight="24"
    UncheckedImageUrl="/images/toggle-off.png"
    CheckedImageUrl="/images/toggle-on.png"
    UncheckedImageAlternateText="Not subscribed"
    CheckedImageAlternateText="Subscribed"
    CheckedImageOverUrl="/images/toggle-on-hover.png"
    UncheckedImageOverUrl="/images/toggle-off-hover.png" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Update image paths from `~/` to `/`. Everything else stays the same!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the checkbox to enhance with toggle image behavior |
| `ImageWidth` | `int` | `0` | Width of the toggle image in pixels |
| `ImageHeight` | `int` | `0` | Height of the toggle image in pixels |
| `UncheckedImageUrl` | `string` | `""` | URL of the image displayed when the checkbox is unchecked |
| `CheckedImageUrl` | `string` | `""` | URL of the image displayed when the checkbox is checked |
| `UncheckedImageAlternateText` | `string` | `""` | Alternate text for the unchecked image (accessibility) |
| `CheckedImageAlternateText` | `string` | `""` | Alternate text for the checked image (accessibility) |
| `CheckedImageOverUrl` | `string` | `""` | URL of the image displayed on hover when checked |
| `UncheckedImageOverUrl` | `string` | `""` | URL of the image displayed on hover when unchecked |
| `DisabledUncheckedImageUrl` | `string` | `""` | URL of the image displayed when disabled and unchecked |
| `DisabledCheckedImageUrl` | `string` | `""` | URL of the image displayed when disabled and checked |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic Toggle Button

```razor
@rendermode InteractiveServer

<CheckBox ID="chkNotifications" />

<ToggleButtonExtender
    TargetControlID="chkNotifications"
    ImageWidth="48"
    ImageHeight="24"
    UncheckedImageUrl="/images/switch-off.png"
    CheckedImageUrl="/images/switch-on.png"
    UncheckedImageAlternateText="Notifications off"
    CheckedImageAlternateText="Notifications on" />
```

### Toggle with Hover States

```razor
@rendermode InteractiveServer

<CheckBox ID="chkFavorite" />

<ToggleButtonExtender
    TargetControlID="chkFavorite"
    ImageWidth="32"
    ImageHeight="32"
    UncheckedImageUrl="/images/star-empty.png"
    CheckedImageUrl="/images/star-filled.png"
    UncheckedImageOverUrl="/images/star-empty-hover.png"
    CheckedImageOverUrl="/images/star-filled-hover.png"
    UncheckedImageAlternateText="Add to favorites"
    CheckedImageAlternateText="Remove from favorites" />
```

### Toggle with Disabled States

```razor
@rendermode InteractiveServer

<CheckBox ID="chkFeature" Enabled="false" />

<ToggleButtonExtender
    TargetControlID="chkFeature"
    ImageWidth="48"
    ImageHeight="24"
    UncheckedImageUrl="/images/toggle-off.png"
    CheckedImageUrl="/images/toggle-on.png"
    DisabledUncheckedImageUrl="/images/toggle-off-disabled.png"
    DisabledCheckedImageUrl="/images/toggle-on-disabled.png" />
```

## HTML Output

The ToggleButtonExtender produces no HTML itself — it attaches JavaScript behavior that replaces the checkbox's visual appearance with an image element while preserving the checkbox's checked/unchecked state.

## JavaScript Interop

The ToggleButtonExtender loads `toggle-button-extender.js` as an ES module. JavaScript handles:

- Hiding the original checkbox and displaying an image in its place
- Swapping images on click (checked ↔ unchecked)
- Swapping images on hover (over states)
- Displaying disabled images when the checkbox is disabled
- Keeping the underlying checkbox state in sync

## Render Mode Requirements

The ToggleButtonExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. The standard checkbox is displayed.
- **JavaScript disabled:** Same as SSR — standard checkbox functions normally.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:ToggleButtonExtender
   + <ToggleButtonExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Update image paths** from `~/` syntax to `/` paths
   ```diff
   - UncheckedImageUrl="~/images/toggle-off.png"
   + UncheckedImageUrl="/images/toggle-off.png"
   ```

### Before (Web Forms)

```html
<asp:CheckBox ID="chkAgree" runat="server" />

<ajaxToolkit:ToggleButtonExtender
    ID="tbe1"
    TargetControlID="chkAgree"
    ImageWidth="24"
    ImageHeight="24"
    UncheckedImageUrl="~/images/unchecked.png"
    CheckedImageUrl="~/images/checked.png"
    runat="server" />
```

### After (Blazor)

```razor
<CheckBox ID="chkAgree" />

<ToggleButtonExtender
    TargetControlID="chkAgree"
    ImageWidth="24"
    ImageHeight="24"
    UncheckedImageUrl="/images/unchecked.png"
    CheckedImageUrl="/images/checked.png" />
```

## Best Practices

1. **Always provide alt text** — Set `UncheckedImageAlternateText` and `CheckedImageAlternateText` for accessibility
2. **Use consistent image sizes** — Set `ImageWidth` and `ImageHeight` to prevent layout shifts
3. **Provide hover states** — `CheckedImageOverUrl` and `UncheckedImageOverUrl` improve visual feedback
4. **Include disabled states** — If your checkbox can be disabled, provide `DisabledCheckedImageUrl` and `DisabledUncheckedImageUrl`
5. **Optimize images** — Use small, optimized PNG or SVG images for fast loading

## Troubleshooting

| Issue | Solution |
|---|---|
| Image not appearing | Verify `CheckedImageUrl` and `UncheckedImageUrl` point to valid image files. Ensure `@rendermode InteractiveServer` is set. |
| Standard checkbox still visible | Check that `TargetControlID` matches the checkbox's `ID` exactly. |
| Hover state not working | Verify `CheckedImageOverUrl` and `UncheckedImageOverUrl` paths are correct. |
| Toggle state not syncing | Ensure the target is a checkbox element. Check browser console for JavaScript errors. |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- [ConfirmButtonExtender](ConfirmButtonExtender.md) — Button confirmation dialogs
- [CheckBox Component](../EditorControls/CheckBox.md) — The CheckBox control
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit
