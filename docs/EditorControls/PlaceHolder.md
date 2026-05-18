The PlaceHolder component is meant to emulate the asp:PlaceHolder control in markup and is defined in https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.placeholder?view=netframework-4.8

PlaceHolder is a simple container that renders NO wrapper element - it only renders its child content. This is useful for conditionally showing/hiding blocks of content.

## Blazor Features Supported

- `ChildContent` - the content to render inside the PlaceHolder
- `Visible` - controls whether the content is rendered

## Key Characteristic

Unlike Panel, PlaceHolder renders **only its children** with no wrapper element. This makes it ideal for:

- Conditional rendering of content blocks
- Grouping content without affecting HTML structure
- Dynamic content placeholders

## Usage Examples

### Basic Usage

```razor
<PlaceHolder>
    <p>This content renders directly with no wrapper</p>
</PlaceHolder>
```

Renders as:
```html
<p>This content renders directly with no wrapper</p>
```

### Conditional Visibility

```razor
<PlaceHolder Visible="@showContent">
    <div>Only shown when showContent is true</div>
</PlaceHolder>
```

## Syntax Comparison

=== "Web Forms"

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

=== "Blazor"

    ```razor
    <!-- Basic usage: renders children with no wrapper element -->
    <PlaceHolder>
        <h2>Welcome</h2>
        <p>This content renders directly into the page.</p>
    </PlaceHolder>

    <!-- Conditional visibility -->
    <PlaceHolder Visible="@isLoggedIn">
        <p>Hello, @username! You have @messageCount new messages.</p>
        <button @onclick="ViewMessages">View Messages</button>
    </PlaceHolder>

    @code {
        private bool isLoggedIn = true;
        private string username = "Alice";
        private int messageCount = 3;

        private void ViewMessages() { /* ... */ }
    }
    ```

## Migration Notes

## See Also

- [Panel](Panel.md) — Div-based container control
- [MultiView](MultiView.md) — Multi-view container
- [View](View.md) — Individual view within a MultiView
