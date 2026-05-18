# Themes and Skins

The **Themes and Skins** feature enables you to centrally define and apply consistent styling to all BlazorWebFormsComponents throughout your application—just as you would in ASP.NET Web Forms. This guide explains how to use themes, apply them at runtime, and migrate from Web Forms theme files.

!!! note "Feature Status"
    Themes and Skins are implemented in the `BlazorWebFormsComponents.Theming` namespace and fully available for use.

---

## Overview

### What Themes and Skins Provide

**Themes** and **Skins** solve a common problem: maintaining consistent, centralized styling across your entire application without duplicating style definitions on every control.

In Web Forms, you would create `.skin` files containing control definitions with appearance properties:

```xml
<!-- App_Themes/Professional/controls.skin -->
<asp:Button runat="server"
    BackColor="#507CD1"
    ForeColor="White"
    Font-Bold="true" />
```

Then apply the theme to your page—and every Button would automatically adopt that styling.

With BlazorWebFormsComponents, you achieve the same result using a **ThemeConfiguration** object and the **ThemeProvider** wrapper component.

### Key Concepts

- **ThemeConfiguration** — A builder object that defines styling rules for controls
- **ThemeProvider** — A wrapper component that applies a theme to all child controls
- **ControlSkin** — The styling definition for a single control type (e.g., all Buttons)
- **SubStyle** — Named style definitions for parts of complex controls (e.g., GridView headers, rows)
- **SkinID** — A named skin variant for a control (optional; enables multiple skins per control)
- **EnableTheming** — A control-level property to opt-out of theming
- **ThemeMode** — Either `StyleSheetTheme` (defaults) or `Theme` (overrides)

---

## Quick Start

### 1. Define Your Theme

Create a `ThemeConfiguration` object that describes how your components should look:

```csharp
// In App.razor, _Layout.razor, or a dedicated service
var myTheme = new ThemeConfiguration()
    .ForControl("Button", skin => skin
        .Set(s => s.BackColor, WebColor.FromHtml("#507CD1"))
        .Set(s => s.ForeColor, WebColor.FromHtml("#FFFFFF"))
        .Set(s => s.Font.Bold, true))
    .ForControl("Label", skin => skin
        .Set(s => s.ForeColor, WebColor.FromHtml("#333333"))
        .Set(s => s.Font.Names, "Arial, sans-serif"));
```

### 2. Apply the Theme

Wrap your content with `<ThemeProvider>` to apply the theme:

```html
<ThemeProvider Theme="myTheme">
    <Button Text="Themed Button" />
    <Label Text="Themed Label" />
</ThemeProvider>
```

### 3. Result

Every Button and Label inside the ThemeProvider automatically adopts the theme styling. No additional markup changes needed.

---

## Theme Modes

Themes support two modes that control how styling precedence works.

### Mode 1: StyleSheetTheme (Default)

**Behavior:** Theme acts as *defaults*. Explicit control properties **override** the theme.

**Use when:** You want components to respect their own property values when specified, but fall back to the theme for unspecified properties.

**Example:**

```csharp
var theme = new ThemeConfiguration()
    .ForControl("Button", skin => skin
        .Set(s => s.BackColor, WebColor.FromHtml("#507CD1"))
        .Set(s => s.Font.Bold, true));

// In your page
<ThemeProvider Theme="theme" ThemeMode="ThemeMode.StyleSheetTheme">
    <!-- Uses theme: BackColor=#507CD1, Bold=true -->
    <Button Text="Themed Button" />
    
    <!-- Overrides theme BackColor; keeps theme Bold -->
    <Button Text="Custom" BackColor="Red" />
</ThemeProvider>
```

### Mode 2: Theme

**Behavior:** Theme **overrides** all control properties. Explicit control settings are ignored.

**Use when:** You want to enforce consistent appearance across your entire application (no exceptions).

**Example:**

```csharp
<ThemeProvider Theme="theme" ThemeMode="ThemeMode.Theme">
    <!-- Uses theme: BackColor=#507CD1, Bold=true -->
    <Button Text="Themed Button" />
    
    <!-- Theme still overrides; BackColor stays #507CD1 -->
    <Button Text="Custom" BackColor="Red" />
</ThemeProvider>
```

### Comparison Table

| Feature | StyleSheetTheme (Default) | Theme |
|---------|--------------------------|-------|
| **Behavior** | Sets defaults; explicit values win | Overrides all values |
| **Web Forms equivalent** | `Page.StyleSheetTheme` | `Page.Theme` |
| **Use when** | You want components to override theme | You want consistent theming |
| **Priority** | Component property > Theme | Theme > Component property |
| **Flexibility** | High—exceptions per control | Low—uniform across app |

---

## Sub-Component Styles

Complex controls like **GridView**, **DetailsView**, and **FormView** have sub-components (HeaderStyle, RowStyle, etc.) that need their own styling rules.

Use the **SubStyle** API to define styles for these sub-components:

```csharp
var theme = new ThemeConfiguration()
    .ForControl("GridView", skin => skin
        .SubStyle("HeaderStyle", style => {
            style.BackColor = WebColor.FromHtml("#507CD1");
            style.ForeColor = WebColor.FromHtml("#FFFFFF");
            style.Font.Bold = true;
        })
        .SubStyle("RowStyle", style => {
            style.BackColor = WebColor.FromHtml("#EFF3FB");
        })
        .SubStyle("AlternatingRowStyle", style => {
            style.BackColor = WebColor.FromHtml("#F7F6F3");
        })
        .SubStyle("FooterStyle", style => {
            style.BackColor = WebColor.FromHtml("#507CD1");
            style.ForeColor = WebColor.FromHtml("#FFFFFF");
        }));
```

When you render the GridView, all HeaderStyle, RowStyle, AlternatingRowStyle, and FooterStyle elements automatically adopt the theme styling.

### Supported Sub-Styles by Control

**GridView** (8 sub-styles):
- HeaderStyle, FooterStyle, RowStyle, AlternatingRowStyle, SelectedRowStyle, EditRowStyle, PagerStyle, SortedHeaderStyle

**DetailsView** (10 sub-styles):
- HeaderStyle, FooterStyle, RowStyle, AlternatingRowStyle, SelectedRowStyle, EditRowStyle, InsertRowStyle, CommandRowStyle, PagerStyle, EmptyDataRowStyle

**FormView** (7 sub-styles):
- HeaderStyle, FooterStyle, RowStyle, AlternatingRowStyle, EditRowStyle, InsertRowStyle, PagerStyle

**DataGrid** (7 sub-styles):
- HeaderStyle, FooterStyle, ItemStyle, AlternatingItemStyle, SelectedItemStyle, EditItemStyle, PagerStyle

**DataList** (5 sub-styles):
- HeaderStyle, FooterStyle, ItemStyle, AlternatingItemStyle, SelectedItemStyle

---

## Migration Guide: Web Forms to Blazor

### Converting .skin Files to ThemeConfiguration

#### Web Forms (.skin File)

In Web Forms, you would create a `.skin` file in your `App_Themes` folder:

```xml
<!-- App_Themes/Professional/controls.skin -->
<asp:Button runat="server" 
    BackColor="#507CD1" 
    ForeColor="White" 
    Font-Bold="true" />

<asp:Label runat="server" 
    ForeColor="#333333" />

<asp:GridView runat="server">
    <HeaderStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
    <RowStyle BackColor="#EFF3FB" />
    <AlternatingRowStyle BackColor="#F7F6F3" />
</asp:GridView>
```

#### Blazor Equivalent

In Blazor with BlazorWebFormsComponents, create a `ThemeConfiguration`:

```csharp
public static class Themes
{
    public static ThemeConfiguration CreateProfessionalTheme()
    {
        return new ThemeConfiguration()
            .ForControl("Button", skin => skin
                .Set(s => s.BackColor, WebColor.FromHtml("#507CD1"))
                .Set(s => s.ForeColor, WebColor.FromHtml("#FFFFFF"))
                .Set(s => s.Font.Bold, true))
            .ForControl("Label", skin => skin
                .Set(s => s.ForeColor, WebColor.FromHtml("#333333")))
            .ForControl("GridView", skin => skin
                .SubStyle("HeaderStyle", s => {
                    s.BackColor = WebColor.FromHtml("#507CD1");
                    s.ForeColor = WebColor.FromHtml("#FFFFFF");
                    s.Font.Bold = true;
                })
                .SubStyle("RowStyle", s => {
                    s.BackColor = WebColor.FromHtml("#EFF3FB");
                })
                .SubStyle("AlternatingRowStyle", s => {
                    s.BackColor = WebColor.FromHtml("#F7F6F3");
                }));
    }
}
```

Then use it in your pages:

```html
@page "/"
@inject NavigationManager nav

<ThemeProvider Theme="@theme">
    <Button Text="Themed Button" />
    <Label Text="Themed Label" />
    <GridView ItemsSource="@data">
        <!-- GridView header, rows, etc. all inherit theme -->
    </GridView>
</ThemeProvider>

@code {
    private ThemeConfiguration theme = null!;

    protected override void OnInitialized()
    {
        theme = Themes.CreateProfessionalTheme();
    }
}
```

---

## EnableTheming and Opt-Out

By default, all controls participate in theming when wrapped in a `<ThemeProvider>`. You can opt-out at the control level using the `EnableTheming` parameter.

### Control-Level Opt-Out

```html
<ThemeProvider Theme="myTheme">
    <!-- This Button uses the theme -->
    <Button Text="Themed" />
    
    <!-- This Button does NOT use the theme -->
    <Button Text="Custom" EnableTheming="false" />
</ThemeProvider>
```

### Container Propagation

When you set `EnableTheming="false"` on a container (like a Panel), it disables theming for all child controls:

```html
<ThemeProvider Theme="myTheme">
    <Button Text="Themed" />
    
    <Panel EnableTheming="false">
        <!-- All children are NOT themed -->
        <Button Text="Not Themed" />
        <Label Text="Not Themed" />
    </Panel>
</ThemeProvider>
```

---

## Named Skins with SkinID

The `SkinID` parameter allows you to define multiple theme variants for the same control type.

### Defining Multiple Skins

```csharp
var theme = new ThemeConfiguration()
    // Default skin for all Buttons (no SkinID required)
    .ForControl("Button", skin => skin
        .Set(s => s.BackColor, WebColor.FromHtml("#507CD1"))
        .Set(s => s.Font.Bold, true))
    // Named skin: "Danger" for destructive actions
    .ForControl("Button", "Danger", skin => skin
        .Set(s => s.BackColor, WebColor.FromHtml("#DC3545"))
        .Set(s => s.ForeColor, WebColor.FromHtml("#FFFFFF")))
    // Named skin: "Success" for positive actions
    .ForControl("Button", "Success", skin => skin
        .Set(s => s.BackColor, WebColor.FromHtml("#28A745"))
        .Set(s => s.ForeColor, WebColor.FromHtml("#FFFFFF")));
```

### Using Named Skins

```html
<ThemeProvider Theme="theme">
    <!-- Uses default Button skin -->
    <Button Text="Save" />
    
    <!-- Uses "Danger" skin -->
    <Button Text="Delete" SkinID="Danger" />
    
    <!-- Uses "Success" skin -->
    <Button Text="Approve" SkinID="Success" />
</ThemeProvider>
```

---

## Runtime Theme Switching

You can switch themes at runtime by assigning a new `ThemeConfiguration` instance to the `ThemeProvider`.

### Example: Theme Switcher

```html
@page "/"

<div>
    <button @onclick="() => SwitchTheme(Themes.CreateProfessionalTheme())">
        Professional Theme
    </button>
    <button @onclick="() => SwitchTheme(Themes.CreateModernTheme())">
        Modern Theme
    </button>
</div>

<ThemeProvider Theme="@currentTheme">
    <Button Text="Click me" />
    <Label Text="Current theme applied" />
</ThemeProvider>

@code {
    private ThemeConfiguration currentTheme = null!;

    protected override void OnInitialized()
    {
        currentTheme = Themes.CreateProfessionalTheme();
    }

    private void SwitchTheme(ThemeConfiguration newTheme)
    {
        currentTheme = newTheme;
    }
}
```

When you assign a new theme, all child controls automatically re-render with the new styling.

---

## API Reference

### ThemeConfiguration

The builder class for defining themes.

| Method | Description |
|--------|-------------|
| `ForControl(string name, Action<ControlSkin> skin)` | Define the default skin for a control type |
| `ForControl(string name, string skinId, Action<ControlSkin> skin)` | Define a named skin variant for a control type |

### ControlSkin

Defines styling for a single control.

| Method | Description |
|--------|-------------|
| `Set<T>(Expression<Func<Style, T>> property, T value)` | Set a style property |
| `SubStyle(string name, Action<Style> style)` | Define a sub-component style (for complex controls) |

### ThemeProvider (Component)

Applies a theme to child controls.

| Parameter | Type | Description |
|-----------|------|-------------|
| `Theme` | `ThemeConfiguration` | The theme to apply |
| `ThemeMode` | `ThemeMode` | Either `StyleSheetTheme` (default) or `Theme` |
| `ChildContent` | `RenderFragment` | Child controls to theme |

### Style Properties (Common)

All style objects support these properties:

| Property | Type | Description |
|----------|------|-------------|
| `BackColor` | `WebColor?` | Background color |
| `ForeColor` | `WebColor?` | Text color |
| `BorderColor` | `WebColor?` | Border color |
| `BorderStyle` | `BorderStyle?` | Border style (Solid, Dotted, etc.) |
| `BorderWidth` | `Unit?` | Border width |
| `Width` | `Unit?` | Element width |
| `Height` | `Unit?` | Element height |
| `CssClass` | `string?` | CSS class names |
| `Font` | `FontInfo` | Font properties (Bold, Italic, Names, Size, etc.) |
| `HorizontalAlign` | `HorizontalAlign?` | Horizontal alignment (Left, Center, Right) |
| `VerticalAlign` | `VerticalAlign?` | Vertical alignment (Top, Middle, Bottom) |
| `Wrap` | `bool?` | Text wrapping (default: true) |

### ThemeMode (Enum)

| Value | Description |
|-------|-------------|
| `StyleSheetTheme` | Theme acts as defaults; component properties override |
| `Theme` | Theme overrides all component properties |

---

## Best Practices

1. **Centralize Theme Definitions**: Create a dedicated `Themes.cs` static class containing all theme configurations
2. **Name Your Skins Meaningfully**: Use semantic names like "Danger", "Success", "Warning" rather than "Skin1", "Skin2"
3. **Test Theme Application**: Verify themes render correctly across all affected control types
4. **Document Theme Intent**: Add comments explaining the purpose of each theme variant
5. **Use StyleSheetTheme for Flexibility**: Default to `StyleSheetTheme` mode unless you need strict consistency
6. **Avoid Deep Nesting**: Don't nest multiple `ThemeProvider` components—apply a single theme at the page or layout level

---

## Troubleshooting

**Q: Theme not applying to a control**  
A: Verify the control is wrapped in `<ThemeProvider>` and the control type name matches exactly (e.g., "Button", not "btn").

**Q: Control property overriding the theme when I don't want it to**  
A: Switch to `ThemeMode.Theme` to make the theme override all control properties.

**Q: Sub-style not applying to GridView rows**  
A: Ensure the sub-style name matches exactly (e.g., "RowStyle", not "Row" or "Rows"). Verify the GridView is inside a `<ThemeProvider>`.

**Q: EnableTheming="false" not working on a child control**  
A: Ensure the container (Panel, etc.) where you set `EnableTheming="false"` is inside a `<ThemeProvider>`.

---

## See Also

- [Themes and Skins Migration Strategy](Migration/ThemesAndSkins.md) — Detailed migration guide from Web Forms theme files
- [Styling Components](UtilityFeatures/StylingComponents.md) — Style sub-components reference
- [GridView](DataControls/GridView.md) — GridView component documentation
