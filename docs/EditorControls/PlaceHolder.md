# PlaceHolder

The PlaceHolder component emulates the ASP.NET Web Forms `asp:PlaceHolder` control. It is a simple container that renders NO wrapper element — it only renders its child content. This is useful for conditionally showing/hiding blocks of content.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.placeholder?view=netframework-4.8

## Features Supported in Blazor

- `ChildContent` - the content to render inside the PlaceHolder
- `Visible` - controls whether the content is rendered

## Key Characteristic

Unlike Panel, PlaceHolder renders **only its children** with no wrapper element. This makes it ideal for:

- Conditional rendering of content blocks
- Grouping content without affecting HTML structure
- Dynamic content placeholders

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:PlaceHolder ID="phDetails" Visible="True" runat="server">
        <p>This content renders directly with no wrapper</p>
    </asp:PlaceHolder>
    ```

    Full declarative syntax:

    ```html
    <asp:PlaceHolder
        EnableTheming="True|False"
        EnableViewState="True|False"
        ID="string"
        OnDataBinding="DataBinding event handler"
        OnDisposed="Disposed event handler"
        OnInit="Init event handler"
        OnLoad="Load event handler"
        OnPreRender="PreRender event handler"
        OnUnload="Unload event handler"
        runat="server"
        SkinID="string"
        Visible="True|False"
    />
    ```

=== "Blazor (After)"

    ```razor
    <PlaceHolder>
        <p>This content renders directly with no wrapper</p>
    </PlaceHolder>
    ```

    Conditional visibility:

    ```razor
    <PlaceHolder Visible="@showContent">
        <div>Only shown when showContent is true</div>
    </PlaceHolder>

    @code {
        private bool showContent = true;
    }
    ```

## HTML Output

PlaceHolder renders no wrapper HTML element — only its children are output directly into the DOM.

**Blazor Input:**
```razor
<PlaceHolder>
    <p>This content renders directly with no wrapper</p>
</PlaceHolder>
```

**Rendered HTML:**
```html
<p>This content renders directly with no wrapper</p>
```

## Example Migration

=== "Web Forms (Before)"

    ```html
    <asp:PlaceHolder ID="phAddress" Visible="true" runat="server">
        <div class="address-block">
            <asp:Label ID="lblStreet" runat="server" Text="123 Main St" />
            <br />
            <asp:Label ID="lblCity" runat="server" Text="Springfield" />
        </div>
    </asp:PlaceHolder>
    ```

=== "Blazor (After)"

    ```razor
    <PlaceHolder Visible="@showAddress">
        <div class="address-block">
            <Label Text="123 Main St" />
            <br />
            <Label Text="Springfield" />
        </div>
    </PlaceHolder>

    @code {
        private bool showAddress = true;
    }
    ```

!!! tip "Migration Tips"
    1. Remove `asp:` prefix and `runat="server"`
    2. The `ID` property is not rendered (use Blazor's `@ref` for component references)
    3. PlaceHolder is ideal for conditional rendering — use the `Visible` property to show/hide blocks of content without adding wrapper elements to the DOM

!!! note "PlaceHolder vs Panel"
    Use `PlaceHolder` when you need a container that adds **no HTML markup**. Use `Panel` when you need a `<div>` or `<fieldset>` wrapper around your content.

## See Also

- [Panel](Panel.md) - Container that renders a `<div>` or `<fieldset>` wrapper
- [Literal](Literal.md) - Renders text with no wrapper element
