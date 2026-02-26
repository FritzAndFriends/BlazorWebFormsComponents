# WebFormsPage

The `WebFormsPage` component is a unified legacy support wrapper that combines naming container and theming support into a single component. It mirrors `System.Web.UI.Page` — the root class of every Web Forms page — which established the naming scope for all child controls and applied the page-level theme.

Place `WebFormsPage` in your layout to give all pages automatic ID mangling (naming container) and theme/skin support without per-page configuration.

## Background

In ASP.NET Web Forms, the `Page` class provided several structural services to all controls on the page:

1. **Naming Container** — The page was the root `INamingContainer`, generating fully-qualified client IDs like `ctl00_MainContent_MyButton`
2. **Theme Application** — The `<%@ Page Theme="..." %>` directive applied skin files to all controls
3. **ViewState** — The page serialized control state to a hidden field (not replicated — Blazor preserves state in component fields)

`WebFormsPage` combines the first two capabilities into a single Blazor component.

## Features Supported in Blazor

- **Naming Container** — Cascades naming scope to child components, prefixing IDs with the container's ID
- **UseCtl00Prefix** — Optionally prepends `ctl00` to the naming hierarchy for full Web Forms ID compatibility
- **Theme Cascading** — Passes a `ThemeConfiguration` to all child styled components via `CascadingValue`
- **Visible** — Controls whether child content renders (inherited from `BaseWebFormsComponent`)

### Blazor Notes

`WebFormsPage` inherits from `NamingContainer` and composes `ThemeProvider` behavior. Both standalone components remain available for use independently when you only need one capability.

## Usage

### Layout Placement (Recommended)

Place `WebFormsPage` in your `MainLayout.razor` wrapping `@Body`:

```razor
@inherits LayoutComponentBase

<WebFormsPage ID="MainContent" UseCtl00Prefix="true" Theme="@_theme">
    @Body
</WebFormsPage>

@code {
    private ThemeConfiguration _theme = new();
}
```

This mirrors how `<form runat="server">` wrapped all page content in Web Forms. Every page automatically gets naming scope and theming.

### Per-Page Usage

For area-specific configuration (e.g., different themes per section):

```razor
@page "/admin"

<WebFormsPage ID="AdminContent" Theme="@_adminTheme">
    <GridView DataSource="@data" AutoGenerateColumns="true" />
</WebFormsPage>

@code {
    private ThemeConfiguration _adminTheme = new();
}
```

### Naming Only (No Theme)

When you only need ID mangling, omit the `Theme` parameter:

```razor
<WebFormsPage ID="MainContent" UseCtl00Prefix="true">
    <Button ID="Submit" Text="Save" />
    @* Button gets id="ctl00_MainContent_Submit" *@
</WebFormsPage>
```

### With Theme Configuration

```razor
@using BlazorWebFormsComponents.Theming

<WebFormsPage ID="MainContent" Theme="@_theme">
    <Label Text="Themed label" />
    <Button ID="Save" Text="Save" />
</WebFormsPage>

@code {
    private ThemeConfiguration _theme;

    protected override void OnInitialized()
    {
        _theme = new ThemeConfiguration();
        _theme.AddSkin("Label", new ControlSkin { BackColor = WebColor.LightBlue });
        _theme.AddSkin("Button", new ControlSkin { BackColor = WebColor.Green, ForeColor = WebColor.White });
    }
}
```

## Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ID` | `string` | `null` | Sets the naming scope prefix for child component IDs |
| `UseCtl00Prefix` | `bool` | `false` | When true, prepends `ctl00` to the naming hierarchy |
| `Theme` | `ThemeConfiguration` | `null` | Theme configuration to cascade to child components |
| `Visible` | `bool` | `true` | Controls whether child content renders |
| `ChildContent` | `RenderFragment` | — | The page content |

## Relationship to Other Components

| Component | Purpose | When to Use |
|---|---|---|
| `WebFormsPage` | Combined naming + theming | Layout-level wrapper for full legacy support |
| `NamingContainer` | Naming scope only | When you need nested naming scopes within a page |
| `ThemeProvider` | Theme cascading only | When demonstrating theming in isolation |

## Moving On

As you refactor away from Web Forms patterns:

1. **ID Mangling** — Consider using Blazor's built-in `@ref` instead of string-based IDs for element references
2. **Theming** — Consider migrating to CSS custom properties or a CSS framework for theming
3. **ViewState** — Blazor preserves component state automatically in component fields; no hidden field serialization needed

## See Also

- [ID Rendering](IDRendering.md) — How component IDs work in this library
- [Themes and Skins](../Migration/ThemesAndSkins.md) — Migration guide for Web Forms themes
- [ViewState](ViewState.md) — How ViewState is emulated
