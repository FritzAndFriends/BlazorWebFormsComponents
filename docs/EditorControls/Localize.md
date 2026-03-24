# Localize

The **Localize** component emulates the ASP.NET Web Forms `asp:Localize` control. In Web Forms, `Localize` inherits directly from `Literal` and is functionally identical to it — the only difference is semantic. `Localize` marks text as localizable for design-time tooling and resource expression support (`<%$ Resources:... %>`). In Blazor, there is no design-time localization distinction, so this component exists purely for markup compatibility during migration.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.localize?view=netframework-4.8

## Features Supported in Blazor

- `Text` - the text content to display (pass localized strings from `IStringLocalizer<T>`)
- `Mode` - specifies how the content is rendered (`Encode`, `PassThrough`, or `Transform`)
- `Visible` - controls whether the text is rendered

### Blazor Notes

- `Localize` inherits from `Literal` and behaves identically. All properties, rendering, and modes are inherited.
- No wrapper HTML element is rendered — the text is output directly into the DOM.
- When `Mode` is `Encode` (the default), text is HTML-encoded. When `Mode` is `PassThrough`, text is rendered as raw markup.

!!! warning "Localization in Blazor"
    In Web Forms, `Localize` enabled design-time localization via resource expressions like `<%$ Resources:MyResource, MyKey %>`. Blazor does not have resource expressions. Instead, inject `IStringLocalizer<T>` and pass localized strings to the `Text` property. See [Microsoft's Blazor globalization documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization) for details.

## Web Forms Features NOT Supported

- **Resource Expressions** (`<%$ Resources:... %>`) - Not supported in Blazor; use `IStringLocalizer<T>` instead
- **Design-time localization tooling** - No Blazor equivalent; localization is handled at runtime via .NET localization APIs
- **EnableTheming / SkinID** - Theming is not supported in Blazor

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:Localize
        EnableTheming="True|False"
        EnableViewState="True|False"
        ID="string"
        Mode="Transform|PassThrough|Encode"
        OnDataBinding="DataBinding event handler"
        OnDisposed="Disposed event handler"
        OnInit="Init event handler"
        OnLoad="Load event handler"
        OnPreRender="PreRender event handler"
        OnUnload="Unload event handler"
        runat="server"
        SkinID="string"
        Text="string"
        Visible="True|False"
    />
    ```

=== "Blazor (After)"

    ```razor
    <Localize Text="Hello, World!" />
    ```

### Basic Usage

```razor
<Localize Text="Hello, World!" />
```

### With Localized Text

```razor
@inject IStringLocalizer<MyPage> Localizer

<Localize Text="@Localizer["WelcomeMessage"]" />
```

### With Mode

```razor
<Localize Text="<strong>Bold text</strong>" Mode="LiteralMode.PassThrough" />
<Localize Text="<script>alert('safe')</script>" Mode="LiteralMode.Encode" />
```

## HTML Output

`Localize` renders no wrapper HTML element — text is output directly into the DOM, identical to `Literal`.

**Blazor Input:**
```razor
<Localize Text="Hello, World!" />
```

**Rendered HTML:**
```html
Hello, World!
```

**With Mode=PassThrough:**
```razor
<Localize Text="<em>emphasized</em>" Mode="LiteralMode.PassThrough" />
```

**Rendered HTML:**
```html
<em>emphasized</em>
```

**With Mode=Encode (default):**
```razor
<Localize Text="<em>encoded</em>" Mode="LiteralMode.Encode" />
```

**Rendered HTML:**
```html
&lt;em&gt;encoded&lt;/em&gt;
```

!!! tip "Migration Notes"
    1. **Remove `asp:` prefix** — Change `<asp:Localize>` to `<Localize>`
    2. **Remove `runat="server"`** — Not needed in Blazor
    3. **Replace resource expressions** — Replace `<%$ Resources:MyResource, MyKey %>` with `IStringLocalizer<T>` injection
    4. **Static text** — If the `Text` was hardcoded, no additional changes are needed

## Example Migration

=== "Web Forms (Before)"

    ```html
    <asp:Localize ID="lblWelcome"
                  Text="<%$ Resources:Messages, WelcomeText %>"
                  Mode="Encode"
                  runat="server" />
    ```

=== "Blazor (After)"

    ```razor
    @inject IStringLocalizer<MyPage> Localizer

    <Localize Text="@Localizer["WelcomeText"]" Mode="LiteralMode.Encode" />
    ```

!!! note "Key Difference"
    In Web Forms, `Localize` enabled design-time localization via resource expressions like `<%$ Resources:MyResource, MyKey %>`. In Blazor, inject `IStringLocalizer<T>` and pass localized strings to the `Text` property.

!!! tip "Consider Using Literal"
    Since `Localize` is identical to `Literal` in Blazor, you can use either component interchangeably. If you are writing new Blazor code (not migrating), `Literal` is the conventional choice. Use `Localize` when migrating existing markup that already uses `<asp:Localize>` to minimize changes.

## See Also

- [Literal](Literal.md) - Identical component; Localize inherits from Literal
- [Blazor Globalization and Localization](https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization) - Microsoft's guide to localization in Blazor
