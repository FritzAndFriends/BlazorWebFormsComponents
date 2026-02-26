# NamingContainer

The `NamingContainer` component establishes a naming scope for child components, equivalent to [INamingContainer](https://learn.microsoft.com/dotnet/api/system.web.ui.inamingcontainer) in ASP.NET Web Forms. Child controls get IDs prefixed with this container's ID, separated by underscores — exactly as they did in Web Forms.

`NamingContainer` renders **no HTML of its own**. It is a purely structural component that exists solely to define the naming hierarchy for ID generation.

## Why It Exists

In Web Forms, any control implementing `INamingContainer` created a scope so that child controls received fully-qualified `ClientID` values. This ensured unique IDs across the page and enabled JavaScript and CSS selectors that relied on predictable ID patterns like `MainContent_txtUsername`.

Blazor has no built-in equivalent. `NamingContainer` fills that gap, letting your migrated JavaScript and CSS continue to target elements by their original Web Forms IDs.

## Features Supported in Blazor

- **Naming scope** — Prefixes child component IDs with the container's own ID using underscore separators
- **UseCtl00Prefix** — Optionally prepends `ctl00` to the naming hierarchy for full Web Forms ID compatibility
- **Nesting** — Multiple `NamingContainer` components can be nested; IDs accumulate through the hierarchy
- **Visible** — Controls whether child content renders (inherited from `BaseWebFormsComponent`)

### Features NOT Supported

- **ClientIDMode** — Web Forms offered `AutoID`, `Static`, `Predictable`, and `Inherit` modes. This library uses a simplified model equivalent to `AutoID`.
- **UniqueID** — The postback-oriented `UniqueID` (colon-separated) is not replicated; only `ClientID` (underscore-separated) is supported.

## Web Forms Syntax

```asp
<%-- INamingContainer was typically an interface on custom controls or built-in containers --%>
<asp:Panel ID="MainContent" runat="server">
    <asp:TextBox ID="txtSearch" runat="server" />
    <asp:Button ID="btnGo" runat="server" Text="Search" />
</asp:Panel>
```

Rendered IDs: `MainContent_txtSearch`, `MainContent_btnGo`

With `Page` as the root naming container and `ClientIDMode="AutoID"`:

```html
<input id="ctl00_MainContent_txtSearch" type="text" />
<input id="ctl00_MainContent_btnGo" type="submit" value="Search" />
```

## Blazor Syntax

```razor
<NamingContainer ID="MainContent">
    <TextBox ID="txtSearch" />
    <Button ID="btnGo" Text="Search" />
</NamingContainer>
```

With `UseCtl00Prefix`:

```razor
<NamingContainer ID="MainContent" UseCtl00Prefix="true">
    <TextBox ID="txtSearch" />
    <Button ID="btnGo" Text="Search" />
</NamingContainer>
```

## Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ID` | `string` | `null` | Sets the naming scope prefix for child component IDs |
| `UseCtl00Prefix` | `bool` | `false` | When true, prepends `ctl00` to the naming hierarchy |
| `Visible` | `bool` | `true` | Controls whether child content renders |
| `ChildContent` | `RenderFragment` | — | The child components to wrap |

## Examples

### Basic Usage

Wrap a group of controls to establish a naming scope:

```razor
<NamingContainer ID="SearchPanel">
    <Label ID="lblQuery" Text="Search:" />
    <TextBox ID="txtQuery" />
    <Button ID="btnSearch" Text="Go" />
</NamingContainer>
```

Rendered IDs:

| Component | Rendered `id` attribute |
|---|---|
| Label | `SearchPanel_lblQuery` |
| TextBox | `SearchPanel_txtQuery` |
| Button | `SearchPanel_btnSearch` |

Your existing JavaScript continues to work:

```javascript
var query = document.getElementById('SearchPanel_txtQuery').value;
```

### Nested Containers

`NamingContainer` components can be nested. Each level adds its ID as a prefix:

```razor
<NamingContainer ID="Page">
    <NamingContainer ID="Sidebar">
        <TextBox ID="txtFilter" />
    </NamingContainer>
    <NamingContainer ID="Content">
        <Button ID="btnSave" Text="Save" />
    </NamingContainer>
</NamingContainer>
```

Rendered IDs:

| Component | Rendered `id` attribute |
|---|---|
| TextBox | `Page_Sidebar_txtFilter` |
| Button | `Page_Content_btnSave` |

### UseCtl00Prefix for Full Web Forms Compatibility

In Web Forms, the page-level naming container prepended `ctl00` to all client IDs. Enable this behavior with `UseCtl00Prefix`:

```razor
<NamingContainer ID="MainContent" UseCtl00Prefix="true">
    <TextBox ID="txtName" />
    <Button ID="btnSubmit" Text="Submit" />
</NamingContainer>
```

Rendered IDs:

| Component | Rendered `id` attribute |
|---|---|
| TextBox | `ctl00_MainContent_txtName` |
| Button | `ctl00_MainContent_btnSubmit` |

This is essential when your JavaScript or CSS targets the full `ctl00_`-prefixed IDs that Web Forms generated.

### Migration Example

**Before (Web Forms):**
```asp
<form id="form1" runat="server">
    <asp:Panel ID="MainContent" runat="server">
        <asp:TextBox ID="txtEmail" runat="server" />
        <asp:Button ID="btnRegister" runat="server" Text="Register" />
    </asp:Panel>
</form>

<script>
    // JavaScript targeting Web Forms IDs
    var email = document.getElementById('ctl00_MainContent_txtEmail');
    var btn = document.getElementById('ctl00_MainContent_btnRegister');
</script>
```

**After (Blazor):**
```razor
<NamingContainer ID="MainContent" UseCtl00Prefix="true">
    <TextBox ID="txtEmail" />
    <Button ID="btnRegister" Text="Register" />
</NamingContainer>

<script>
    // JavaScript unchanged — same IDs as before
    var email = document.getElementById('ctl00_MainContent_txtEmail');
    var btn = document.getElementById('ctl00_MainContent_btnRegister');
</script>
```

## Relationship to WebFormsPage

[WebFormsPage](WebFormsPage.md) inherits from `NamingContainer` and adds theme/skin cascading. Use the table below to choose the right component:

| Component | Naming Scope | Theming | When to Use |
|---|---|---|---|
| `NamingContainer` | ✅ | ❌ | Nested naming scopes within a page, or when theming is not needed |
| `WebFormsPage` | ✅ | ✅ | Layout-level wrapper providing both naming scope and theme support |

If you only need ID prefixing — for example, to scope a subsection of a page — use `NamingContainer`. If you need the full Web Forms `Page`-level experience (naming + themes), use `WebFormsPage` in your layout.

## Moving On

As you complete your migration away from Web Forms patterns:

1. **Replace string-based IDs with `@ref`** — Blazor's component references are type-safe and don't depend on naming conventions
2. **Remove `ctl00` prefixes** — Update JavaScript to use simpler IDs, then set `UseCtl00Prefix="false"` or remove `NamingContainer` entirely
3. **Use CSS classes instead of ID selectors** — CSS class selectors are more resilient and don't depend on component hierarchy

## See Also

- [ID Rendering](IDRendering.md) — How component IDs work across the library
- [WebFormsPage](WebFormsPage.md) — Combined naming container and theme wrapper
- [ViewState](ViewState.md) — How ViewState is emulated in Blazor
