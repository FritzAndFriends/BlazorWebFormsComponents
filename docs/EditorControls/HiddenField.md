# HiddenField

The **HiddenField** component stores a non-displayed value that can be posted back to the server. It emulates the `asp:HiddenField` control, providing a way to store page-specific information that should not be visible to users but needs to be available during processing.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.hiddenfield?view=netframework-4.8

## Features Supported in Blazor

- **Value** - The value stored in the hidden field
- **OnValueChanged** - Event that fires when the value changes
- **ID** - Renders as the HTML `id` attribute

### Blazor Notes

In Blazor, the HiddenField is useful for:

- Storing values that need to be accessible via JavaScript interop
- Maintaining compatibility with migrated Web Forms markup
- Storing temporary data that shouldn't be visible to users

Unlike Web Forms, Blazor maintains component state automatically, so you may not need HiddenField as frequently. Consider using component fields or cascading parameters for state management in pure Blazor applications.

## Web Forms Features NOT Supported

- **EnableTheming** - Blazor uses CSS for styling
- **EnableViewState** - Blazor handles state differently; use component state instead
- **SkinID** - Themes/skins not supported; use CSS classes
- **OnDataBinding, OnDisposed, OnInit, OnLoad, OnPreRender, OnUnload** - Blazor component lifecycle is different; use Blazor lifecycle methods (`OnInitialized`, `OnParametersSet`, etc.)
- **Visible** - Not applicable; use conditional rendering with `@if` instead

## Web Forms Declarative Syntax

```html
<asp:HiddenField
    EnableTheming="True|False"
    EnableViewState="True|False"
    ID="string"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    OnValueChanged="ValueChanged event handler"
    runat="server"
    SkinID="string"
    Value="string"
    Visible="True|False"
/>
```

## Blazor Razor Syntax

### Basic Usage

```razor
<HiddenField ID="myHiddenField" Value="@secretValue" />

@code {
    private string secretValue = "stored-data-123";
}
```

### With Value Changed Event

```razor
<HiddenField ID="trackingField" 
             Value="@trackingId" 
             OnValueChanged="HandleValueChanged" />

@code {
    private string trackingId = "initial-value";

    private void HandleValueChanged(EventArgs e)
    {
        // Handle the value change
        Console.WriteLine("Hidden field value changed");
    }
}
```

## HTML Output

**Blazor Input:**
```razor
<HiddenField ID="myHidden" Value="secret123" />
```

**Rendered HTML:**
```html
<input id="myHidden" type="hidden" value="secret123" />
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `runat="server"`** - Not needed in Blazor
2. **Remove `EnableViewState` and `EnableTheming`** - Not applicable in Blazor
3. **Replace lifecycle events** - Use Blazor lifecycle methods instead
4. **Consider alternatives** - For pure Blazor apps, you may not need HiddenField; use component state or `@bind` instead
5. **JavaScript interop** - HiddenField is still useful when you need to exchange values with JavaScript

!!! tip "Best Practice"
    In new Blazor development, prefer using component fields and parameters over hidden fields. Only use HiddenField when migrating existing markup or when JavaScript interop requires a DOM element to read/write values.

## See Also

- [TextBox](TextBox.md) - For visible text input
- [ViewState](../UtilityFeatures/ViewState.md) - State management options
- [JavaScript Setup](../UtilityFeatures/JavaScriptSetup.md) - JavaScript interop guidance
