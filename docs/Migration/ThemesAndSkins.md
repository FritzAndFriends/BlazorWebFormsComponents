# Themes and Skins Migration Strategy

ASP.NET Web Forms **Themes** and **Skins** provided a centralized way to control the visual appearance of server controls across an entire application. Blazor has no built-in equivalent, but several Blazor-native approaches can achieve the same goals. This document explains the original Web Forms features, evaluates Blazor alternatives, and recommends a migration path.

!!! note "Current Status"
    The project README states: _"We will NOT be converting... skins or themes."_ The `EnableTheming` and `SkinID` properties exist on `BaseWebFormsComponent` but are marked `[Obsolete]` with no functional behavior. This document is an exploratory strategy for how these features **could** be addressed.

---

## What Are Web Forms Themes and Skins?

### Themes

A **Theme** is a named collection of CSS files, skin files (`.skin`), and images stored in a well-known folder structure:

```
App_Themes/
└── Professional/
    ├── styles.css
    ├── controls.skin
    └── Images/
        └── logo.png
```

Themes are applied in three ways:

| Method | Scope | Example |
|--------|-------|---------|
| `web.config` | Application-wide | `<pages theme="Professional" />` |
| `@Page` directive | Single page | `<%@ Page Theme="Professional" %>` |
| Programmatic | Dynamic | `Page.Theme = "Professional"` in `Page_PreInit` |

Themes have two modes that control **precedence**:

- **`Theme`** property — the theme **overrides** control property settings (theme wins over markup)
- **`StyleSheetTheme`** property — the theme acts as **defaults** (markup wins over theme)

This override-vs-default distinction is critical for any migration approach.

### Skins (.skin Files)

A **Skin file** is an XML-like file containing ASP.NET server control declarations with appearance property values — but no `ID` attribute and no behavioral properties:

```html
<!-- Default skin — applies to ALL Button instances -->
<asp:Button runat="server"
    BackColor="#C04000"
    BorderColor="Maroon"
    Font-Names="Tahoma"
    Font-Size="10pt" />

<!-- Named skin — applies only when SkinID="action" -->
<asp:Button runat="server" SkinID="action"
    BackColor="Navy"
    ForeColor="White"
    Font-Bold="True" />
```

Key rules:

- **Default skins** (no `SkinID`) apply to **every** instance of that control type automatically
- **Named skins** (`SkinID="action"`) apply only to controls with a matching `SkinID` property
- Only **one** default skin per control type is allowed
- Only **appearance** properties belong in skins (colors, fonts, borders — not `Text`, `CommandName`, etc.)

### SkinID Property

Every Web Forms control inherits `SkinID` from `System.Web.UI.Control`:

```html
<!-- Uses the default Button skin -->
<asp:Button ID="btnCancel" runat="server" Text="Cancel" />

<!-- Uses the "action" named skin -->
<asp:Button ID="btnSave" runat="server" Text="Save" SkinID="action" />
```

!!! warning "Known Issue"
    In the current codebase, `BaseWebFormsComponent.SkinID` is typed as `bool` instead of `string`. If any approach below is implemented, this must be corrected to `string` to match the Web Forms API.

---

## Blazor CSS Capabilities

Before evaluating approaches, here are the Blazor features available as building blocks:

| Feature | Description |
|---------|-------------|
| **CSS Isolation** | `{Component}.razor.css` files with auto-scoped selectors via `b-{hash}` attributes |
| **`::deep`** | Pseudo-element for styling child component markup from a parent's scoped CSS |
| **CSS Custom Properties** | Native CSS variables (`var(--color-primary)`) for theming |
| **CascadingValue / CascadingParameter** | Pass values down the component tree without explicit parameters |
| **DI Services** | Inject configuration objects via dependency injection |
| **RCL Bundling** | Component library CSS bundled as `{PackageId}.bundle.scp.css` |

---

## Approach 1: CSS Custom Properties (Variables)

Define theme variables at the root or page level; components reference them with `var()`.

### How It Works

**Define the theme (CSS file or `<style>` block):**

```css
/* themes/professional.css */
:root {
    --button-bg: #C04000;
    --button-border: Maroon;
    --button-font: "Tahoma", sans-serif;
    --button-font-size: 10pt;

    --button-action-bg: Navy;
    --button-action-fg: White;
    --button-action-font-weight: bold;
}
```

**Component CSS uses the variables:**

```css
/* Button.razor.css */
button {
    background-color: var(--button-bg, #ddd);
    border-color: var(--button-border, #ccc);
    font-family: var(--button-font, inherit);
    font-size: var(--button-font-size, inherit);
}
```

**SkinID support via CSS classes:**

```css
/* Named skin: action */
button.skin-action {
    background-color: var(--button-action-bg, Navy);
    color: var(--button-action-fg, White);
    font-weight: var(--button-action-font-weight, bold);
}
```

```razor
<!-- Component applies SkinID as a CSS class -->
<Button Text="Save" SkinID="action" />
<!-- Renders: <button class="skin-action">Save</button> -->
```

**Theme switching — swap the CSS file:**

```razor
<!-- In App.razor or a layout -->
<link href="themes/@currentTheme.css" rel="stylesheet" />
```

### Theme vs StyleSheetTheme Semantics

| Mode | CSS Approach |
|------|-------------|
| **Theme** (override) | Variables defined with higher specificity or `!important` |
| **StyleSheetTheme** (default) | Variables defined on `:root` with normal specificity; inline styles from markup win naturally |

CSS custom properties with `var(--prop, fallback)` already behave like **StyleSheetTheme** — an inline `style` attribute set in markup wins over the variable. **Theme** (override) semantics are harder; they require `!important` or higher-specificity selectors, which is fragile.

### Strengths

- ✅ Pure CSS — no C# runtime cost
- ✅ Theme switching is instant (swap a stylesheet)
- ✅ Browser DevTools support for live editing
- ✅ Works with CSS isolation and `::deep`
- ✅ Incrementally adoptable — add variables one component at a time

### Weaknesses

- ❌ Only works for CSS-expressible properties (colors, fonts, borders)
- ❌ Cannot set non-CSS properties (`Text`, `Width` as an HTML attribute, `Visible`)
- ❌ **Theme** override semantics require `!important` hacks
- ❌ SkinID → CSS class mapping must be built into each component
- ❌ No compile-time validation of variable names

---

## Approach 2: CascadingValue ThemeProvider

A wrapper component provides default property values via `CascadingParameter`.

### How It Works

**Define a theme model:**

```csharp
public class WebFormsTheme
{
    public string Name { get; set; } = "Default";

    // Per-control-type defaults (default skin)
    public Dictionary<Type, ControlSkinDefaults> DefaultSkins { get; } = new();

    // Named skins keyed by SkinID
    public Dictionary<(Type, string), ControlSkinDefaults> NamedSkins { get; } = new();
}

public class ControlSkinDefaults
{
    public WebColor? BackColor { get; set; }
    public WebColor? ForeColor { get; set; }
    public WebColor? BorderColor { get; set; }
    public string? FontFamily { get; set; }
    public string? FontSize { get; set; }
    public bool? FontBold { get; set; }
    public string? CssClass { get; set; }
    // ... other appearance properties
}
```

**Create a ThemeProvider component:**

```razor
<!-- ThemeProvider.razor -->
<CascadingValue Value="Theme" Name="WebFormsTheme">
    @ChildContent
</CascadingValue>

@code {
    [Parameter] public WebFormsTheme Theme { get; set; } = new();
    [Parameter] public RenderFragment ChildContent { get; set; }
}
```

**Wrap your app or page:**

```razor
<ThemeProvider Theme="@professionalTheme">
    <Router AppAssembly="@typeof(App).Assembly">
        <!-- ... -->
    </Router>
</ThemeProvider>

@code {
    private WebFormsTheme professionalTheme = ThemeLoader.Load("Professional");
}
```

**Components read from the cascaded theme:**

```csharp
// Inside BaseWebFormsComponent or BaseStyledComponent
[CascadingParameter(Name = "WebFormsTheme")]
protected WebFormsTheme? Theme { get; set; }

protected override void OnParametersSet()
{
    if (Theme != null)
    {
        ApplyThemeDefaults();
    }
}

private void ApplyThemeDefaults()
{
    var controlType = GetType();
    ControlSkinDefaults? skin = null;

    // Check for named skin first
    if (!string.IsNullOrEmpty(SkinID))
    {
        Theme.NamedSkins.TryGetValue((controlType, SkinID), out skin);
    }

    // Fall back to default skin
    skin ??= Theme.DefaultSkins.GetValueOrDefault(controlType);

    if (skin == null) return;

    // StyleSheetTheme: apply only if property not explicitly set
    if (BackColor == default && skin.BackColor.HasValue)
        BackColor = skin.BackColor.Value;

    if (ForeColor == default && skin.ForeColor.HasValue)
        ForeColor = skin.ForeColor.Value;

    // ... repeat for other appearance properties
}
```

### Theme vs StyleSheetTheme Semantics

```csharp
public enum ThemeMode { StyleSheetTheme, Theme }

// In BaseWebFormsComponent:
private void ApplySkin(ControlSkinDefaults skin, ThemeMode mode)
{
    if (mode == ThemeMode.StyleSheetTheme)
    {
        // Default mode: only apply if not explicitly set in markup
        if (BackColor == default) BackColor = skin.BackColor ?? default;
    }
    else // ThemeMode.Theme
    {
        // Override mode: theme always wins
        if (skin.BackColor.HasValue) BackColor = skin.BackColor.Value;
    }
}
```

This approach can faithfully model **both** `Theme` and `StyleSheetTheme` semantics because it operates at the property level in C#, not in CSS.

### SkinID Support

```razor
<!-- Default skin applies automatically -->
<Button Text="Cancel" />

<!-- Named skin "action" applies -->
<Button Text="Save" SkinID="action" />
```

Both work identically to Web Forms — the base class resolves the correct skin entry.

### Strengths

- ✅ Faithful to Web Forms semantics — supports both Theme and StyleSheetTheme modes
- ✅ SkinID works exactly like Web Forms
- ✅ Can set **any** property, not just CSS-related ones
- ✅ Compile-time type safety on the theme model
- ✅ Theme data could be loaded from `.skin`-like configuration files
- ✅ Single point of change in `BaseWebFormsComponent` benefits all 50+ components

### Weaknesses

- ❌ Requires changes to `BaseWebFormsComponent` (touches every component)
- ❌ Runtime cost — theme resolution on every `OnParametersSet`
- ❌ Not a standard Blazor pattern — new contributors must learn it
- ❌ Theme switching requires re-render of the entire component tree

---

## Approach 3: Generated CSS Isolation Files

Pre-generate `.razor.css` files for each component from skin definitions. Closest to Jeff's initial idea.

### How It Works

**Source: a skin definition file (JSON, YAML, or `.skin`):**

```json
{
    "theme": "Professional",
    "skins": {
        "Button": {
            "default": {
                "background-color": "#C04000",
                "border-color": "Maroon",
                "font-family": "Tahoma",
                "font-size": "10pt"
            },
            "action": {
                "background-color": "Navy",
                "color": "White",
                "font-weight": "bold"
            }
        },
        "TextBox": {
            "default": {
                "border": "1px solid #999",
                "padding": "4px"
            }
        }
    }
}
```

**Build-time tool generates scoped CSS:**

```css
/* Button.razor.css (generated) */
button {
    background-color: #C04000;
    border-color: Maroon;
    font-family: "Tahoma", sans-serif;
    font-size: 10pt;
}

button[data-skinid="action"] {
    background-color: Navy;
    color: White;
    font-weight: bold;
}
```

**SkinID rendered as a `data-` attribute:**

```razor
<!-- Button.razor (modified) -->
<button data-skinid="@SkinID" class="@CssClass" style="@Style">
    @Text
</button>
```

### Theme Switching

Generate separate CSS bundles per theme. Switch by loading a different bundle:

```razor
<link href="_content/BlazorWebFormsComponents/themes/professional.css" rel="stylesheet" />
```

### Theme vs StyleSheetTheme Semantics

| Mode | Generated CSS Approach |
|------|----------------------|
| **StyleSheetTheme** (default) | Generated CSS uses low specificity; inline `style` attributes from markup override |
| **Theme** (override) | Generated CSS uses `!important` or high-specificity selectors |

### Strengths

- ✅ Aligns with Jeff's initial idea
- ✅ Pure CSS output — no runtime cost
- ✅ Leverages Blazor's built-in CSS isolation
- ✅ Theme files can be authored by designers without C# knowledge
- ✅ Build-time validation of skin definitions

### Weaknesses

- ❌ Requires a build-time code generation tool (MSBuild task or Source Generator)
- ❌ Only works for CSS-expressible properties
- ❌ Cannot set non-CSS properties (`Visible`, `Width` as attribute, etc.)
- ❌ Generated CSS fights with CSS isolation scoping (`b-{hash}` attributes)
- ❌ Theme override semantics are fragile (`!important`)
- ❌ Significant tooling investment for a migration-only feature

---

## Approach 4: Dictionary-Based Configuration via DI

A `ThemeConfiguration` service registered in DI holds property defaults per control type, keyed by optional SkinID.

### How It Works

**Define the configuration service:**

```csharp
public class ThemeConfiguration
{
    public string ThemeName { get; set; } = "Default";
    public ThemeMode Mode { get; set; } = ThemeMode.StyleSheetTheme;

    private readonly Dictionary<string, ControlSkinDefaults> _skins = new();

    /// <summary>
    /// Register a default skin for a control type.
    /// </summary>
    public void AddDefaultSkin<TControl>(ControlSkinDefaults defaults)
        where TControl : BaseWebFormsComponent
    {
        _skins[typeof(TControl).Name] = defaults;
    }

    /// <summary>
    /// Register a named skin for a control type.
    /// </summary>
    public void AddNamedSkin<TControl>(string skinId, ControlSkinDefaults defaults)
        where TControl : BaseWebFormsComponent
    {
        _skins[$"{typeof(TControl).Name}:{skinId}"] = defaults;
    }

    public ControlSkinDefaults? GetSkin(Type controlType, string? skinId = null)
    {
        if (!string.IsNullOrEmpty(skinId) &&
            _skins.TryGetValue($"{controlType.Name}:{skinId}", out var named))
            return named;

        _skins.TryGetValue(controlType.Name, out var def);
        return def;
    }
}
```

**Register in DI:**

```csharp
// Program.cs
builder.Services.AddSingleton<ThemeConfiguration>(sp =>
{
    var theme = new ThemeConfiguration { ThemeName = "Professional" };

    theme.AddDefaultSkin<Button>(new ControlSkinDefaults
    {
        BackColor = WebColor.FromName("Maroon"),
        FontFamily = "Tahoma",
        FontSize = "10pt"
    });

    theme.AddNamedSkin<Button>("action", new ControlSkinDefaults
    {
        BackColor = WebColor.FromName("Navy"),
        ForeColor = WebColor.FromName("White"),
        FontBold = true
    });

    return theme;
});
```

**Components inject the service:**

```csharp
// BaseWebFormsComponent.cs
[Inject]
protected ThemeConfiguration? ThemeConfig { get; set; }

protected override void OnParametersSet()
{
    if (ThemeConfig != null)
    {
        var skin = ThemeConfig.GetSkin(GetType(), SkinID);
        if (skin != null) ApplySkin(skin, ThemeConfig.Mode);
    }
}
```

### Theme Switching

Swap the DI registration or use a scoped service:

```csharp
builder.Services.AddScoped<ThemeConfiguration>(sp =>
    ThemeLoader.LoadFromJson("wwwroot/themes/professional.json"));
```

### Strengths

- ✅ Standard DI pattern — familiar to Blazor developers
- ✅ Supports both Theme and StyleSheetTheme semantics
- ✅ SkinID works like Web Forms
- ✅ Can set any property (not limited to CSS)
- ✅ Theme data can be loaded from JSON/XML files at startup
- ✅ No component tree re-render on theme switch (if scoped per-circuit)

### Weaknesses

- ❌ Service injection in base component may surprise library consumers
- ❌ Configuration is imperative (code), not declarative (markup)
- ❌ Requires changes to `BaseWebFormsComponent`
- ❌ No cascading scope — the theme applies globally per DI scope
- ❌ Cannot easily scope a theme to a subtree of components (unlike CascadingValue)

---

## Approach 5: Hybrid — CSS Variables + CascadingParameter

Use CSS custom properties for visual styles (colors, fonts, borders) and `CascadingParameter` for non-CSS properties.

### How It Works

**CSS variables handle the visual layer:**

```css
/* themes/professional.css */
:root {
    --bwfc-button-bg: #C04000;
    --bwfc-button-border: Maroon;
    --bwfc-button-font: "Tahoma", sans-serif;
}
```

**CascadingParameter handles non-CSS properties:**

```csharp
public class ThemeDefaults
{
    public ThemeMode Mode { get; set; } = ThemeMode.StyleSheetTheme;
    public Dictionary<(Type, string?), NonCssDefaults> Defaults { get; } = new();
}

public class NonCssDefaults
{
    public string? Width { get; set; }   // HTML attribute, not CSS
    public bool? Visible { get; set; }
    public string? ToolTip { get; set; }
}
```

**Components use both:**

```css
/* Button.razor.css */
button {
    background-color: var(--bwfc-button-bg);
    border-color: var(--bwfc-button-border);
    font-family: var(--bwfc-button-font);
}
```

```csharp
// Button.razor.cs
[CascadingParameter(Name = "ThemeDefaults")]
private ThemeDefaults? ThemeDefaults { get; set; }

protected override void OnParametersSet()
{
    // CSS properties handled by CSS variables — no C# needed
    // Only non-CSS properties resolved here
    if (ThemeDefaults?.Defaults.TryGetValue(
            (GetType(), SkinID), out var defaults) == true)
    {
        if (Width == null && defaults.Width != null)
            Width = defaults.Width;
    }
}
```

**SkinID maps to CSS class + cascading lookup:**

```css
button.skin-action {
    background-color: var(--bwfc-button-action-bg, Navy);
    color: var(--bwfc-button-action-fg, White);
}
```

### Strengths

- ✅ Best of both worlds — CSS for visual, C# for structural
- ✅ Theme switching is fast (CSS swap for visuals, no full re-render)
- ✅ Clean separation of concerns
- ✅ Incrementally adoptable

### Weaknesses

- ❌ Two systems to learn and maintain
- ❌ Splitting properties between CSS and C# creates confusion
- ❌ SkinID must be handled in both CSS and C# code
- ❌ Most complex implementation of all approaches

---

## Comparison Matrix

| Criteria | CSS Variables | CascadingValue | Generated CSS | DI Service | Hybrid |
|----------|:---:|:---:|:---:|:---:|:---:|
| **Web Forms SkinID fidelity** | ⚠️ Partial | ✅ Full | ⚠️ Partial | ✅ Full | ✅ Full |
| **Theme mode (override)** | ❌ Fragile | ✅ Yes | ❌ Fragile | ✅ Yes | ⚠️ Partial |
| **StyleSheetTheme mode (default)** | ✅ Natural | ✅ Yes | ✅ Natural | ✅ Yes | ✅ Yes |
| **Non-CSS properties** | ❌ No | ✅ Yes | ❌ No | ✅ Yes | ✅ Yes |
| **Runtime performance** | ✅ Zero | ⚠️ Per-render | ✅ Zero | ⚠️ Per-render | ⚠️ Mixed |
| **Incremental adoption** | ✅ Easy | ⚠️ Medium | ❌ Hard | ⚠️ Medium | ⚠️ Medium |
| **Tooling investment** | ✅ None | ✅ Low | ❌ High | ✅ Low | ⚠️ Medium |
| **Scoped to subtree** | ✅ CSS scope | ✅ Cascading | ✅ CSS scope | ❌ Global DI | ✅ Mixed |
| **Blazor-idiomatic** | ✅ Yes | ✅ Yes | ⚠️ Somewhat | ✅ Yes | ⚠️ Complex |

---

## Recommended Approach: CascadingValue ThemeProvider (Approach 2)

**The CascadingValue ThemeProvider is the recommended approach** for the following reasons:

### Rationale

1. **Highest Web Forms fidelity.** It is the only approach that can faithfully model both `Theme` (override) and `StyleSheetTheme` (default) semantics, plus `SkinID` selection, using the same mental model as the original.

2. **Works for all property types.** Unlike CSS-only approaches, it can set `BackColor`, `ForeColor`, `Width`, `ToolTip`, `CssClass`, and any other appearance property — matching what `.skin` files actually do.

3. **Single point of change.** The theme resolution logic lives in `BaseWebFormsComponent.OnParametersSet`. Once implemented there, all 50+ components in the library inherit the behavior automatically. No per-component work required.

4. **Familiar Blazor pattern.** `CascadingValue`/`CascadingParameter` is a well-documented, first-class Blazor feature. Developers already encounter it with `EditContext`, `CascadingAuthenticationState`, and the library's own `TableItemStyle` cascading.

5. **Scoped application.** Unlike DI (which is global), a `CascadingValue` can be scoped to a page or section — matching how Web Forms allows `StyleSheetTheme` on a per-page basis via the `@Page` directive.

6. **Incrementally adoptable.** The `ThemeProvider` component and base class changes can ship without requiring any existing consumer to change their code. Themes are opt-in: if no `ThemeProvider` wraps the component tree, behavior is unchanged.

### Implementation Roadmap

If this strategy is approved, implementation would proceed in phases:

**Phase 1 — Foundation (Low effort)**

- Fix `SkinID` type from `bool` to `string` on `BaseWebFormsComponent`
- Remove `[Obsolete]` from `SkinID` and `EnableTheming`
- Define `ControlSkinDefaults`, `WebFormsTheme`, `ThemeMode`
- Create `ThemeProvider` cascading wrapper component
- Add theme resolution to `BaseWebFormsComponent.OnParametersSet`

**Phase 2 — Theme Loading (Medium effort)**

- Build a JSON/YAML theme file loader (`ThemeLoader`)
- Optionally support parsing legacy `.skin` files
- Ship one sample theme matching a common Web Forms starter template

**Phase 3 — Documentation & Samples (Low effort)**

- Sample page demonstrating theme switching
- Migration guide addendum showing before/after for themed Web Forms apps

### Complementary CSS Variables

While the CascadingValue approach is primary, nothing prevents developers from **also** using CSS custom properties for pure-CSS theming on top. The two are complementary:

```razor
<!-- CascadingValue for property defaults -->
<ThemeProvider Theme="@myTheme">
    <!-- CSS variables for visual layer -->
    <div style="--bwfc-primary: Navy; --bwfc-accent: Gold;">
        <Button Text="Themed" SkinID="action" />
    </div>
</ThemeProvider>
```

---

## Migration Example: Before and After

### Before — Web Forms with Theme

**`App_Themes/Corporate/buttons.skin`:**
```html
<asp:Button runat="server"
    BackColor="#336699"
    ForeColor="White"
    Font-Names="Segoe UI"
    Font-Size="9pt"
    BorderStyle="None" />

<asp:Button runat="server" SkinID="danger"
    BackColor="#CC3333"
    ForeColor="White"
    Font-Bold="True" />
```

**`web.config`:**
```xml
<pages theme="Corporate" />
```

**`Products.aspx`:**
```html
<%@ Page Title="Products" %>
<asp:Button ID="btnSave" runat="server" Text="Save" />
<asp:Button ID="btnDelete" runat="server" Text="Delete" SkinID="danger" />
```

### After — Blazor with ThemeProvider

**`CorporateTheme.cs`:**
```csharp
public static class CorporateTheme
{
    public static WebFormsTheme Create()
    {
        var theme = new WebFormsTheme { Name = "Corporate" };

        theme.DefaultSkins[typeof(Button)] = new ControlSkinDefaults
        {
            BackColor = WebColor.FromHtml("#336699"),
            ForeColor = WebColor.FromName("White"),
            FontFamily = "Segoe UI",
            FontSize = "9pt",
            BorderStyle = BorderStyle.None
        };

        theme.NamedSkins[(typeof(Button), "danger")] = new ControlSkinDefaults
        {
            BackColor = WebColor.FromHtml("#CC3333"),
            ForeColor = WebColor.FromName("White"),
            FontBold = true
        };

        return theme;
    }
}
```

**`App.razor` (or layout):**
```razor
<ThemeProvider Theme="@CorporateTheme.Create()">
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
    </Router>
</ThemeProvider>
```

**`Products.razor`:**
```razor
@page "/products"

<Button ID="btnSave" Text="Save" />
<Button ID="btnDelete" Text="Delete" SkinID="danger" />
```

The markup is nearly identical. The theme is applied automatically, just like Web Forms.

---

## Additional Resources

- [ASP.NET Web Forms Themes and Skins](https://learn.microsoft.com/en-us/previous-versions/aspnet/ystx0esc(v=vs.100))
- [Blazor CSS Isolation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/css-isolation)
- [Blazor Cascading Values](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/cascading-values-and-parameters)
- [Custom Controls Migration](Custom-Controls.md)
