# ImageButton

The **ImageButton** component displays a clickable image that functions as a submit button. It emulates the `asp:ImageButton` control, allowing you to use an image as a button with full support for click events, command events, and client-side scripting.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagebutton?view=netframework-4.8

## Features Supported in Blazor

- `AlternateText` - Alternate text displayed when the image is unavailable (renders as `alt` attribute)
- `DescriptionUrl` - The location to a detailed description for the image (renders as `longdesc` attribute)
- `ImageAlign` - The alignment of the image in relation to other elements on the page
- `ImageUrl` - The URL of the image to display (renders as `src` attribute)
- `OnClick` - Event handler triggered when the image is clicked
- `OnClientClick` - JavaScript to execute on the client before the click event
- `OnCommand` - Event handler with command name and argument for event bubbling
- `PostBackUrl` - The URL of the page to post to from the current page

## Web Forms Features NOT Supported

- **Style properties** (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Width`, `Height`, `Font`) - Use standard HTML/CSS styling
- **CausesValidation / ValidationGroup** - Blazor uses EditForm and DataAnnotations for validation
- **AccessKey / TabIndex** - Use HTML attributes directly if needed
- **EnableTheming / SkinID** - ASP.NET theming not available in Blazor
- **EnableViewState** - Not needed; Blazor manages state differently
- **Lifecycle events** (`OnDataBinding`, `OnInit`, `OnLoad`, etc.) - Use Blazor lifecycle methods instead

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:ImageButton
        AccessKey="string"
        AlternateText="string"
        BackColor="color name|#dddddd"
        BorderColor="color name|#dddddd"
        BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
            Inset|Outset"
        BorderWidth="size"
        CausesValidation="True|False"
        CommandArgument="string"
        CommandName="string"
        CssClass="string"
        DescriptionUrl="uri"
        Enabled="True|False"
        EnableTheming="True|False"
        EnableViewState="True|False"
        ForeColor="color name|#dddddd"
        Height="size"
        ID="string"
        ImageAlign="NotSet|Left|Right|Baseline|Top|Middle|Bottom|
            AbsBottom|AbsMiddle|TextTop"
        ImageUrl="uri"
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
        ToolTip="string"
        ValidationGroup="string"
        Visible="True|False"
        Width="size"
    />
    ```

=== "Blazor (After)"

    ```razor
    <ImageButton ImageUrl="/images/submit.png"
                 AlternateText="Submit"
                 OnClick="HandleClick" />

    @code {
        private void HandleClick(ImageClickEventArgs e)
        {
            // Handle click - e.X and e.Y contain click coordinates
        }
    }
    ```

!!! note "Key Difference"
    In Web Forms, `ImageButton` performs a form postback. In Blazor, it uses Blazor's event system — no full page reload occurs. The `OnClick` handler receives `ImageClickEventArgs` with the X and Y coordinates of the click.

## Blazor Examples

### Basic Click Handler

```razor
<ImageButton ImageUrl="/images/go-button.png"
             AlternateText="Go"
             OnClick="HandleClick" />

@code {
    private void HandleClick(ImageClickEventArgs e)
    {
        Console.WriteLine($"Clicked at ({e.X}, {e.Y})");
    }
}
```

### With Command Event

```razor
<ImageButton ImageUrl="/images/add-item.png"
             AlternateText="Add Item"
             CommandName="Add"
             CommandArgument="42"
             OnCommand="HandleCommand" />

@code {
    private void HandleCommand(CommandEventArgs e)
    {
        var action = e.CommandName;    // "Add"
        var itemId = e.CommandArgument; // "42"
    }
}
```

### With Alignment

```razor
@using BlazorWebFormsComponents.Enums

<ImageButton ImageUrl="/images/submit.png"
             AlternateText="Submit"
             ImageAlign="ImageAlign.Right"
             OnClick="HandleSubmit" />

@code {
    private void HandleSubmit(ImageClickEventArgs e)
    {
        // Process form submission
    }
}
```

### With Description URL

```razor
<ImageButton ImageUrl="/images/complex-action.png"
             AlternateText="Perform Action"
             DescriptionUrl="/descriptions/action-details.html"
             OnClick="HandleAction" />
```

## HTML Output

**Blazor Input:**
```razor
<ImageButton ImageUrl="/images/submit.png"
             AlternateText="Submit Form" />
```

**Rendered HTML:**
```html
<input type="image" src="/images/submit.png" alt="Submit Form" />
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** - Change `<asp:ImageButton>` to `<ImageButton>`
2. **Remove `runat="server"`** - Not needed in Blazor
3. **Update image paths** - Replace `~/images/` with `/images/` and place files in `wwwroot`
4. **Update event handlers** - `OnClick` receives `ImageClickEventArgs` (not `ImageClickEventHandler`)
5. **Replace `OnCommand`** - Signature changes from `(object sender, CommandEventArgs e)` to `(CommandEventArgs e)`
6. **Remove validation attributes** - Use Blazor's `EditForm` and `DataAnnotations` instead

### Before / After

=== "Web Forms (Before)"

    ```html
    <asp:ImageButton ID="btnSubmit"
        ImageUrl="~/images/submit.png"
        AlternateText="Submit"
        OnClick="btnSubmit_Click"
        CausesValidation="True"
        CssClass="submit-btn"
        runat="server" />
    ```

=== "Blazor (After)"

    ```razor
    <ImageButton ImageUrl="/images/submit.png"
                 AlternateText="Submit"
                 OnClick="HandleSubmit" />
    ```

!!! tip "Migration Tip"
    The `CausesValidation` and `ValidationGroup` properties are not supported. In Blazor, use `EditForm` with `DataAnnotationsValidator` to handle form validation. The ImageButton can be placed inside an EditForm to trigger validation on click.

## See Also

- [Image](Image.md) - Display a non-clickable image
- [Button](Button.md) - Standard button control
- [ImageMap](ImageMap.md) - Image with clickable regions
