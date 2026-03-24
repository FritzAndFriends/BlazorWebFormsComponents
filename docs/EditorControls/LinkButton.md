# LinkButton

The **LinkButton** component renders a hyperlink-styled button that triggers server-side events when clicked. It emulates the `asp:LinkButton` control, providing click and command event handling while rendering as an anchor (`<a>`) element styled as a link.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.linkbutton?view=netframework-4.8

## Features Supported in Blazor

- `Text` - The text caption displayed for the link button
- `OnClick` - Event handler triggered when the link is clicked
- `OnClientClick` - JavaScript to execute on the client before the server-side click event
- `OnCommand` - Event handler with command name and argument for event bubbling
- `CommandName` - The command name associated with the LinkButton (used with OnCommand)
- `CommandArgument` - The argument passed with the command event
- `PostBackUrl` - The URL of the page to post to from the current page
- `CssClass` - CSS class applied to the rendered anchor element
- `Enabled` - Controls whether the link button is clickable

## Web Forms Features NOT Supported

- **Style properties** (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Width`, `Height`, `Font-*`) - Use CSS classes instead
- **CausesValidation / ValidationGroup** - Blazor uses EditForm and DataAnnotations for validation
- **AccessKey / TabIndex** - Use HTML attributes directly if needed
- **EnableTheming / SkinID** - ASP.NET theming not available in Blazor
- **EnableViewState** - Not needed; Blazor manages state differently
- **Lifecycle events** (`OnDataBinding`, `OnInit`, `OnLoad`, etc.) - Use Blazor lifecycle methods instead

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:LinkButton
        AccessKey="string"
        BackColor="color name|#dddddd"
        BorderColor="color name|#dddddd"
        BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
            Inset|Outset"
        BorderWidth="size"
        CausesValidation="True|False"
        CommandArgument="string"
        CommandName="string"
        CssClass="string"
        Enabled="True|False"
        EnableTheming="True|False"
        EnableViewState="True|False"
        Font-Bold="True|False"
        Font-Italic="True|False"
        Font-Names="string"
        Font-Overline="True|False"
        Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|
            Large|X-Large|XX-Large"
        Font-Strikeout="True|False"
        Font-Underline="True|False"
        ForeColor="color name|#dddddd"
        Height="size"
        ID="string"
        OnClick="Click event handler"
        OnClientClick="string"
        OnCommand="Command event handler"
        OnDataBinding="DataBinding event handler"
        OnDisposed="Disposed event handler"
        OnInit="Init event handler"
        OnLoad="Load event handler"
        OnPreRender="PreRender event handler"
        OnUnload="Unload event handler"
        PostBackUrl="uri"
        runat="server"
        SkinID="string"
        Style="string"
        TabIndex="integer"
        Text="string"
        ToolTip="string"
        ValidationGroup="string"
        Visible="True|False"
        Width="size"
    />
    ```

=== "Blazor (After)"

    ```razor
    <LinkButton Text="Click Me" OnClick="HandleClick" />

    @code {
        private void HandleClick(EventArgs e)
        {
            // Handle the click event
        }
    }
    ```

!!! note "Key Difference"
    In Web Forms, `LinkButton` triggers a full-page postback via JavaScript. In Blazor, it uses Blazor's event system for a seamless, no-reload interaction. The rendered HTML is an `<a>` element rather than a submit button.

## Blazor Examples

### Basic Click Handler

```razor
<LinkButton Text="Save Changes" OnClick="HandleSave" />

@code {
    private void HandleSave(EventArgs e)
    {
        // Save logic here
    }
}
```

### With Command Event

```razor
<LinkButton Text="Delete"
            CommandName="Delete"
            CommandArgument="@itemId"
            OnCommand="HandleCommand" />

@code {
    private string itemId = "42";

    private void HandleCommand(CommandEventArgs e)
    {
        var action = e.CommandName;    // "Delete"
        var id = e.CommandArgument;     // "42"
    }
}
```

### Styled Link Button

```razor
<LinkButton Text="Learn More"
            CssClass="btn btn-link"
            OnClick="HandleLearnMore" />

@code {
    private void HandleLearnMore(EventArgs e)
    {
        // Navigate or display additional information
    }
}
```

### Disabled Link Button

```razor
<LinkButton Text="Submit"
            Enabled="false"
            OnClick="HandleSubmit" />
```

## HTML Output

**Blazor Input:**
```razor
<LinkButton Text="Click Me" CssClass="action-link" />
```

**Rendered HTML:**
```html
<a class="action-link" href="javascript:void(0);">Click Me</a>
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** - Change `<asp:LinkButton>` to `<LinkButton>`
2. **Remove `runat="server"`** - Not needed in Blazor
3. **Update event handlers** - Change `OnClick="btnSave_Click"` to `OnClick="HandleSave"` with the simplified Blazor signature
4. **Replace style properties** - Use `CssClass` with CSS instead of individual `Font-*`, `ForeColor`, etc. properties
5. **Remove validation attributes** - Use Blazor's `EditForm` and `DataAnnotations` instead of `CausesValidation`

### Before / After

=== "Web Forms (Before)"

    ```html
    <asp:LinkButton ID="lnkSubmit" Text="Submit Order"
        OnClick="lnkSubmit_Click" CssClass="submit-link"
        CausesValidation="True" Font-Bold="True"
        runat="server" />
    ```

=== "Blazor (After)"

    ```razor
    <LinkButton Text="Submit Order"
                OnClick="HandleSubmit"
                CssClass="submit-link fw-bold" />
    ```

!!! tip "Migration Tip"
    Font properties like `Font-Bold`, `Font-Italic`, and `Font-Size` are not supported as component parameters. Use CSS classes instead — for example, replace `Font-Bold="True"` with a CSS class like `fw-bold` (Bootstrap) or a custom class.

## See Also

- [Button](Button.md) - Standard button control
- [HyperLink](../NavigationControls/HyperLink.md) - For navigation-only links without click events
- [ImageButton](ImageButton.md) - Image-based button control
