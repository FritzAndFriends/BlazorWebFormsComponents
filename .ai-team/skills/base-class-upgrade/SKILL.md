# Skill: Upgrading a Control to BaseStyledComponent

## When to Use
When a component currently inherits `BaseWebFormsComponent` and needs IStyle properties (BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width) on its outer rendered element.

## Steps

### 1. Change the base class in `.razor.cs`
```csharp
// Before
public partial class MyControl : BaseWebFormsComponent
// After
public partial class MyControl : BaseStyledComponent
```

### 2. Change `@inherits` in `.razor`
```razor
@* Before *@
@inherits BaseWebFormsComponent
@* After *@
@inherits BaseStyledComponent
```

### 3. Apply styles to the outer HTML element
```html
<!-- Use class="@GetCssClassOrNull()" and style="...@Style" on the outermost rendered element -->
<table class="@GetCssClassOrNull()" style="border-collapse:collapse;@Style">
```

### 4. Add the `GetCssClassOrNull()` helper
Either in `@code {}` block or in `.razor.cs`:
```csharp
private string GetCssClassOrNull()
{
    return !string.IsNullOrEmpty(CssClass) ? CssClass : null;
}
```
Returning null when empty omits the `class` attribute from rendered HTML entirely.

### 5. Add Font attribute parsing in `HandleUnknownAttributes()`
```csharp
protected override void HandleUnknownAttributes()
{
    // existing sub-style attribute parsing...
    this.SetFontsFromAttributes(AdditionalAttributes);
    base.HandleUnknownAttributes();
}
```

### 6. Remove duplicate IStyle properties
If the component previously declared its own `CssClass`, `BackColor`, etc., remove them — they now come from `BaseStyledComponent`.

## Key Facts
- `BaseStyledComponent` extends `BaseWebFormsComponent` — no functionality is lost.
- `Style` property on `BaseStyledComponent` is `protected string` (computed getter), not a `[Parameter]`.
- `[Parameter]` outer styles do NOT conflict with `[CascadingParameter]` sub-styles.
- `this.ToStyle()` extension (from `HasStyleExtensions`) builds inline CSS from all IStyle properties.
- The `GetCssClassOrNull()` pattern ensures clean HTML — no empty `class=""` attributes.

## Reference Implementations
- **Simple control:** `Label.razor` / `Label.razor.cs` (WI-17)
- **Login controls with sub-styles:** `Login.razor.cs`, `ChangePassword.razor.cs`, `CreateUserWizard.razor.cs` (WI-52)
- **Data controls:** `DataList.razor.cs` (WI-07 via BaseDataBoundComponent)
