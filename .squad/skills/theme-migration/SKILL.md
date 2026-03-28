---
name: "Web Forms Theme Migration"
description: "How to migrate ASP.NET Web Forms themes (App_Themes) to Blazor using BWFC auto-discovery"
domain: "theme-migration"
confidence: "low"
source: "manual — Cyclops theme auto-discovery implementation"
tools:
  - name: "SkinFileParser"
    description: "Runtime parser that reads .skin files and converts them to ThemeConfiguration objects"
    when: "When auto-discovering themes, the framework calls this internally"
  - name: "ThemeProvider"
    description: "Layout component that injects discovered theme CSS automatically"
    when: "When layout needs to render theme CSS in <head>"
---

## Context

When migrating an ASP.NET Web Forms application to Blazor using BWFC, the application's themes (stored in `App_Themes/` folder) need to be migrated to the Blazor project. Web Forms themes consist of:
- `.skin` files: ASP.NET control skin definitions with default property values
- `.css` files: Theme CSS files
- `images/` folder: Theme-related images

BWFC provides **automatic theme discovery and registration**. When you copy the theme folder to the Blazor project, `AddBlazorWebFormsComponents()` auto-discovers `.skin` files, parses them via `SkinFileParser`, registers the theme in DI, and `ThemeProvider` automatically injects theme CSS into the layout. **No manual registration code is required.**

## Patterns

### 1. Identify Source Themes in Web Forms Project

Locate the `App_Themes/` folder in the source Web Forms project:
```
MyWebFormsApp/
  App_Themes/
    MyTheme/
      GridView.skin
      Button.skin
      MyTheme.css
      images/
        button-bg.png
    DarkTheme/
      GridView.skin
      Button.skin
      DarkTheme.css
```

Each subfolder under `App_Themes/` is a separate theme.

### 2. Copy Theme Folder to Blazor Project

Copy the entire `App_Themes/` folder from the Web Forms project to `wwwroot/App_Themes/` in the Blazor project:

```
Source:      MyWebFormsApp/App_Themes/
Target:      BlazorApp/wwwroot/App_Themes/
```

**Important:** Preserve the exact folder structure:
- Copy **all subfolders** (each theme name becomes a subfolder)
- Copy **all .skin files** (they define control property defaults)
- Copy **all .css files** (theme stylesheets)
- Copy **all image files** (referenced by CSS or components)

### 3. Auto-Discovery and Registration

When the Blazor application starts and `AddBlazorWebFormsComponents()` is called in `Program.cs`:

1. **Discovery**: The framework scans `wwwroot/App_Themes/` for theme folders
2. **Parsing**: For each theme, `SkinFileParser.ParseThemeFolder()` reads all `.skin` files and converts them to `ControlSkin` objects
3. **DI Registration**: `ThemeConfiguration` is registered in the dependency injection container
4. **Default Theme**: The first theme folder (alphabetically) is auto-registered as the default theme
5. **CSS Auto-Discovery**: All `.css` files in the theme folder are auto-discovered

### 4. CSS Injection in Layout

In your Blazor layout (typically `App.razor` or `MainLayout.razor`), use `ThemeProvider`:

```html
<ThemeProvider>
    @Body
</ThemeProvider>
```

`ThemeProvider` automatically:
- Reads the registered theme from DI
- Injects CSS files from the theme folder into the `<head>` via `HeadContent`
- Does nothing if no theme is configured (safe for non-themed projects)

### 5. Using Skins in Components

If a `.skin` file defines a named skin with `SkinID`, you must explicitly pass the `SkinID` parameter to the component:

```html
<!-- Web Forms: uses default Button skin automatically -->
<asp:Button runat="server" Text="Normal Button" />

<!-- Web Forms: uses DangerButton skin -->
<asp:Button runat="server" SkinID="DangerButton" Text="Danger Button" />

<!-- Blazor: default skin applied automatically -->
<Button Text="Normal Button" />

<!-- Blazor: must explicitly pass SkinID -->
<Button Text="Danger Button" SkinID="DangerButton" />
```

If the `.skin` file does not define a `SkinID` attribute, the component uses the default (unnamed) skin.

## Examples

### Example 1: Simple Theme Migration

**Web Forms:**
```
App_Themes/
  BlueTheme/
    Button.skin       # <asp:Button runat="server" ForeColor="White" BackColor="Navy" />
    GridView.skin     # <asp:GridView runat="server" CssClass="grid" HeaderStyle-BackColor="Navy" />
    BlueTheme.css     # .grid { border: 1px solid #ccc; }
```

**Migration steps:**
1. Copy `App_Themes/BlueTheme/` → `wwwroot/App_Themes/BlueTheme/`
2. Ensure `Program.cs` calls `AddBlazorWebFormsComponents()`
3. Use `<ThemeProvider>` in layout
4. Components automatically receive Button and GridView defaults from BlueTheme.skin

**Result:** All Button, GridView, and other components with skins now render with BlueTheme defaults. CSS is injected automatically.

### Example 2: Multiple Themes

If the Web Forms app has multiple themes:
```
App_Themes/
  BlueTheme/
    *.skin
    BlueTheme.css
  GreenTheme/
    *.skin
    GreenTheme.css
```

**Migration:**
1. Copy both folders to `wwwroot/App_Themes/`
2. The framework auto-discovers both
3. By convention, the first theme (alphabetically: "BlueTheme") is the default

**To use a non-default theme**, use a custom options builder (see Manual Override below).

### Example 3: Named Skins (SkinID)

**Web Forms .skin file:**
```xml
<asp:Button runat="server" CssClass="btn" />
<asp:Button runat="server" SkinID="DangerButton" BackColor="Red" CssClass="btn btn-danger" />
<asp:Button runat="server" SkinID="SuccessButton" BackColor="Green" CssClass="btn btn-success" />
```

**Blazor usage:**
```html
<!-- Default skin -->
<Button Text="Click Me" />

<!-- DangerButton skin -->
<Button Text="Delete" SkinID="DangerButton" />

<!-- SuccessButton skin -->
<Button Text="Save" SkinID="SuccessButton" />
```

## Edge Cases

### No App_Themes Folder

If the source Web Forms application does not have an `App_Themes/` folder, there are no skins to migrate. Components render without theme defaults. This is normal for Web Forms apps that don't use themes.

### Theme Folder with Only CSS (No .skin files)

If `wwwroot/App_Themes/MyTheme/` contains only `.css` files and no `.skin` files:
- The CSS is still auto-discovered and injected by `ThemeProvider`
- Components render without skin defaults (only CSS styling applies)
- This is a valid configuration for CSS-only themes

### Multiple CSS Files in One Theme

If a theme folder has multiple `.css` files:
```
wwwroot/App_Themes/MyTheme/
  theme.css
  components.css
  utilities.css
```

**All CSS files are auto-discovered and injected** in the `<head>` by `ThemeProvider`. Load order is alphabetical.

### Custom ThemesPath

If you need to store themes in a location other than `wwwroot/App_Themes/`, configure `AddBlazorWebFormsComponents()`:

```csharp
builder.Services.AddBlazorWebFormsComponents(options =>
{
    options.ThemesPath = "my-custom-themes"; // relative to wwwroot
});
```

### StyleSheetTheme vs. Theme Mode

By default, BWFC uses **StyleSheetTheme mode**: theme defaults are applied **only if the component property is not explicitly set**. Explicit component properties always win.

```html
<!-- Uses theme default BackColor -->
<Button Text="Button 1" />

<!-- Ignores theme BackColor, uses explicit Red -->
<Button Text="Button 2" BackColor="Red" />
```

To change to **Theme mode** (theme wins over explicit properties), configure:

```csharp
builder.Services.AddBlazorWebFormsComponents(options =>
{
    options.ThemeMode = ThemeMode.Theme; // theme wins over explicit
});
```

> Theme mode is rarely used and not recommended unless you have a specific reason to override explicit properties.

## Anti-Patterns

### ❌ Manual Skin Registration

Do NOT manually register skins in `Program.cs`. Let auto-discovery do it:

```csharp
// ❌ WRONG: Manual registration is unnecessary
var config = new ThemeConfiguration();
config.Skins["Button"].Add("DangerButton", new ControlSkin { ... });
builder.Services.AddSingleton(config);
```

**Instead:** Ensure `.skin` files are in `wwwroot/App_Themes/` and let `AddBlazorWebFormsComponents()` auto-discover.

### ❌ Modifying .skin Files Manually

Do NOT hand-edit `.skin` files after copying them. They are XML-based and parsing is strict:

```xml
<!-- ❌ WRONG: Unbalanced tags, comments, invalid nesting -->
<asp:Button runat="server" CssClass="btn"
<asp:GridView runat="server" <!-- comment in the middle -->
```

**Instead:** If theme adjustments are needed, modify the component's `SkinID` parameter or adjust CSS in `.css` files.

### ❌ Forgetting ThemeProvider in Layout

If you copy themes but don't use `<ThemeProvider>` in the layout, CSS is never injected:

```html
<!-- ❌ WRONG: Themes copied but never injected -->
<body>
    @Body
</body>
```

**Instead:** Wrap content with `<ThemeProvider>`:

```html
<!-- ✅ RIGHT: CSS automatically injected -->
<ThemeProvider>
    @Body
</ThemeProvider>
```

### ❌ Assuming Theme is Applied Globally Without ThemeProvider

Themes only apply inside `<ThemeProvider>`. Components outside the provider do not receive theme defaults:

```html
<!-- ✅ RIGHT: Components inside ThemeProvider get theme defaults -->
<ThemeProvider>
    <Button Text="Has theme" /> <!-- Theme applied -->
</ThemeProvider>

<!-- Outside ThemeProvider -->
<Button Text="No theme" /> <!-- No theme applied -->
```

### ❌ Forgetting to Copy Images

Do NOT skip the `images/` subfolder when copying themes. If CSS or components reference theme images, they will break:

```
wwwroot/App_Themes/MyTheme/
  images/
    button-bg.png      <!-- ✅ MUST copy this -->
    icon.svg           <!-- ✅ MUST copy this -->
```

---

**Last Updated:** 2025-01-27 (Cyclops theme auto-discovery implementation)
