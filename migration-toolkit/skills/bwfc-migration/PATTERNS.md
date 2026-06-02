# Common Migration Patterns & Troubleshooting

## Expression Conversion

| Web Forms Expression | Blazor Equivalent | Notes |
|---------------------|-------------------|-------|
| `<%: expression %>` | `@(expression)` | HTML-encoded output |
| `<%= expression %>` | `@(expression)` | Blazor always encodes |
| `<%# Item.Property %>` | `@context.Property` | Inside data-bound templates |
| `<%#: Item.Property %>` | `@context.Property` | Same — Blazor always encodes |
| `<%# Eval("Property") %>` | `@context.Property` | Direct property access |
| `<%# Bind("Property") %>` | `@bind-Value="context.Property"` | Two-way binding |
| `<%$ RouteValue:id %>` | `@Id` (with `[Parameter]`) | Route parameters |
| `<%-- comment --%>` | `@* comment *@` | Razor comments |
| `<% if (cond) { %>` | `@if (cond) {` | Control flow |
| `<% foreach (var x in items) { %>` | `@foreach (var x in items) {` | Loops |

## File Conversion

| Web Forms | Blazor |
|-----------|--------|
| `MyPage.aspx` + `.aspx.cs` | `MyPage.razor` + `.razor.cs` |
| `MyControl.ascx` + `.ascx.cs` | `MyControl.razor` + `.razor.cs` |
| `Site.Master` + `.Master.cs` | `MainLayout.razor` + `.razor.cs` |

## Directive Conversion

| Web Forms Directive | Blazor Equivalent |
|--------------------|-------------------|
| `<%@ Page Title="X" ... %>` | `@page "/route"` |
| `<%@ Master ... %>` | (remove — layouts don't need directives) |
| `<%@ Control ... %>` | (remove — components don't need directives) |
| `<%@ Register TagPrefix="uc" Src="~/X.ascx" %>` | `@using MyApp.Components` |
| `<%@ Import Namespace="X" %>` | `@using X` |

**Drop entirely:** `AutoEventWireup`, `CodeBehind`, `Inherits`, `EnableViewState`, `MasterPageFile`, `ValidateRequest`, `ClientIDMode`, `EnableTheming`, `SkinID`

## Content/Layout Conversion

| Web Forms | Blazor |
|-----------|--------|
| `<asp:Content ContentPlaceHolderID="MainContent">` | `<Content ContentPlaceHolderID="MainContent">` inside `<ChildComponents>` |
| `<asp:Content ContentPlaceHolderID="HeadContent">` | Prefer page-level `<HeadContent>` or shell `<Head>` |
| `<asp:ContentPlaceHolder ID="MainContent" />` | `<ContentPlaceHolder ID="MainContent" />` inside `<ChildContent>` |

## Route URL Conversion

| Web Forms | Blazor |
|-----------|--------|
| `href="~/Products"` | `href="/Products"` |
| `NavigateUrl="~/Products/<%: Item.ID %>"` | `NavigateUrl="@($"/Products/{context.ID}")"` |
| `GetRouteUrl("Route", new { id = Item.ID })` | `@($"/Products/{context.ID}")` or `GetRouteUrlHelper` |
| `Response.Redirect("~/Products")` | `Response.Redirect("~/Products")` — use ResponseShim |

## Master Page → BWFC Shell

```razor
<MasterPage>
    <Head>
        <title>@(Page.Title)</title>
    </Head>
    <ChildContent>
        <header><nav><Menu ... /></nav></header>
        <main><ContentPlaceHolder ID="MainContent" /></main>
        <footer>© @DateTime.Now.Year</footer>
        @ChildContent
    </ChildContent>
</MasterPage>

@code {
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
```

**Key changes:**
- `<form runat="server">` → removed from shell
- `<asp:ContentPlaceHolder>` → `<ContentPlaceHolder>`
- `<asp:ScriptManager>` → `<ScriptManager />` (no-op stub)
- CSS/meta/title from master `<head>` → shell `<Head>`

> **Tip:** Collapse to native `@layout` + `@Body` only after the migrated shell truly behaves like a single-slot layout.

---

## Common Gotchas

### No ViewState
Replace `ViewState["key"]` with component fields. `ViewStateDictionary` shim available for compile-compat.

### PostBack Compatibility
`WebFormsPageBase.IsPostBack` works: `false` for SSR GET / interactive first render, `true` for SSR POST / interactive subsequent renders. L1 auto-unwraps simple `if (!IsPostBack)` guards. Complex guards (with `else`) get TODO comments. For `__doPostBack()` patterns, subscribe to the `PostBack` event.

### No DataSource Controls
`SqlDataSource`, `ObjectDataSource`, `EntityDataSource` → injected services. See `/bwfc-data-migration`.

### ID Rendering
Blazor doesn't render component IDs. Use `CssClass` or explicit `id` attributes for CSS/JS targeting.

### Template Context Variable
Add `Context="Item"` on template elements:
```razor
<ItemTemplate Context="Item">
    @Item.PropertyName
</ItemTemplate>
```

### Event Handler Signatures
```csharp
// Web Forms: protected void Btn_Click(object sender, EventArgs e) { }
// Blazor:    private void Btn_Click() { }
```
L1 auto-strips standard `EventArgs`. Specialized types (`CommandEventArgs`, etc.) are preserved.

### `TextMode="MultiLine"` Casing
BWFC uses `Multiline` (lowercase 'l'), not `MultiLine`. Silent failure if wrong.

### ScriptManager/ScriptManagerProxy
Razor components are no-op stubs (render nothing). For code-behind, use `ScriptManagerShim.GetCurrent(this)` which delegates to `ClientScriptShim`.

### `runat="server"` on HTML Elements
L1 removes these. Use `@ref` if programmatic access is needed.

---

## Troubleshooting

### L1 Tool Issues

| Problem | Solution |
|---------|----------|
| `webforms-to-blazor` not found | Run `dotnet tool install -g Fritz.WebFormsToBlazor` |
| Tool version mismatch | Run `dotnet tool update -g Fritz.WebFormsToBlazor` |
| Output directory not empty | Use `--overwrite` flag |
| Need to preview changes first | Use `--dry-run` flag |
| Missing scaffolding files | Don't use `--skip-scaffold` unless you have an existing Blazor project |

### L2 Common Issues

| Problem | Solution |
|---------|----------|
| `SelectMethod` not firing | Ensure it's a delegate reference (`@service.Method`), not a string |
| `Items` always empty | Check that `SelectMethod` signature matches `SelectHandler<T>` delegate |
| Template binding errors | Add `Context="Item"` to `<ItemTemplate>` elements |
| Session data lost on refresh | `SessionShim` is per-circuit; use persistent storage for critical data |
| Infinite render loop | Guard `OnAfterRenderAsync` with `if (firstRender)`, call `StateHasChanged()` only when needed |
