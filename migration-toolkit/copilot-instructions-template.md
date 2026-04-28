# Migration Instructions for [Your Project Name]

<!--
  HOW TO USE THIS FILE:
  1. Copy this file to your project: .github/copilot-instructions.md
  2. Replace all [bracketed placeholders] with your project-specific information
  3. Fill in the "Your Application Context" section with details from your bwfc-scan.ps1 report
  4. Update the "Architecture Decisions Made" section as you make Layer 3 decisions
  5. Delete these instructions when you're done

  This file teaches GitHub Copilot how to migrate your Web Forms application to Blazor
  using BlazorWebFormsComponents (BWFC). When Copilot reads this file, it will apply
  the correct transformation rules automatically.

  Source: BlazorWebFormsComponents migration toolkit
  Full skill reference: https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/migration-toolkit/skills/bwfc-migration/SKILL.md
-->

## Your Application Context

<!--
  FILL IN: Paste relevant sections from your bwfc-scan.ps1 report here.
  This gives Copilot the specific context of YOUR application.
-->

- **Application name:** [Your App Name]
- **Page count:** [N] .aspx pages, [N] .ascx user controls, [N] .master pages
- **Primary controls used:** [e.g., GridView, ListView, FormView, Login controls]
- **Data access pattern:** [e.g., SqlDataSource, EntityDataSource, ObjectDataSource, inline DbContext]
- **Authentication:** [e.g., ASP.NET Membership, ASP.NET Identity, Forms Auth, Windows Auth]
- **Session state usage:** [Heavy / Moderate / Light / None]
- **Target framework:** .NET [8/9/10]

---

## BWFC Core Migration Rules

When migrating Web Forms markup to Blazor using BWFC, apply these rules in order:

### 1. Remove Web Forms Boilerplate

- Remove all `runat="server"` attributes
- Remove all `asp:` tag prefixes (`<asp:Button>` → `<Button>`)
- Remove `<%@ Page %>`, `<%@ Control %>`, `<%@ Master %>` directives
- Add `@page "/route"` directive at the top of each page
- Remove `<asp:Content>` wrappers — the page body IS the content
- Remove `<form runat="server">` wrappers entirely

### 2. Convert Expressions

| Web Forms | Blazor |
|---|---|
| `<%: expression %>` | `@(expression)` |
| `<%= expression %>` | `@(expression)` |
| `<%# Item.Property %>` | `@context.Property` (in templates) |
| `<%# Eval("Property") %>` | `@context.Property` |
| `<%# Bind("Property") %>` | `@bind-Value="context.Property"` |
| `<%-- comment --%>` | `@* comment *@` |

### 3. Convert Data Binding

- Replace `ItemType="Namespace.Type"` with `TItem="Type"`
- **When original used `SelectMethod`:** Preserve as delegate reference: `SelectMethod="@service.GetItems"` (BWFC's `DataBoundComponent` auto-populates `Items`)
- **When original used `DataSource`/`DataBind()`:** Replace with `Items="propertyName"` (collections) or `DataItem="propertyName"` (single record)
- Load data in `OnInitializedAsync` (only when using `Items` binding):

```csharp
@inject IYourService YourService

private List<YourType> items = new();

protected override async Task OnInitializedAsync()
{
    items = await YourService.GetItemsAsync();
}
```

> ⚠️ Do NOT convert `SelectMethod` to `Items=` binding — BWFC natively supports `SelectMethod` as a delegate.

### 4. Convert Templates

Add `Context="Item"` to all data templates:

```razor
<ItemTemplate Context="Item">
    @Item.PropertyName
</ItemTemplate>
```

### 5. Convert Code-Behind

| Web Forms | Blazor |
|---|---|
| `Page_Load(sender, e)` | `OnInitializedAsync()` |
| `if (!IsPostBack)` | Works AS-IS via `WebFormsPageBase`; optionally simplify |
| `Page_PreRender` | `OnParametersSetAsync()` |
| `Response.Redirect("~/path")` | Works AS-IS via `ResponseShim` (auto-strips `~/` and `.aspx`); or use `NavigationManager.NavigateTo("/path")` |
| `Page.Title = "X"` | Works AS-IS via `WebFormsPageBase` |
| `Session["key"]` | Works AS-IS via `SessionShim`; or use scoped service / `ProtectedSessionStorage` |
| `Request.Form["key"]` | Works AS-IS via `FormShim` + `<WebFormsForm>` component |
| `Request.QueryString["key"]` | Works AS-IS via `RequestShim`; or use `[SupplyParameterFromQuery]` |
| `Server.MapPath("~/path")` | Works AS-IS via `ServerShim` |
| `Cache["key"]` | Works AS-IS via `CacheShim` |
| `Page.ClientScript.RegisterStartupScript(...)` | Works AS-IS via `ClientScriptShim` |
| `ViewState["key"]` | Works AS-IS via `ViewStateDictionary`; or use component field |

### 6. Convert Event Handlers

```csharp
// Web Forms
protected void Button_Click(object sender, EventArgs e) { }

// Blazor
private void Button_Click() { }
```

### 7. Convert URLs

- Replace `~/` with `/` in all URL references
- Replace `NavigateUrl="~/path"` with `NavigateUrl="/path"`

### 8. Normalize Master Page Shells

- Keep migrated master-page structure in `<MasterPage>` with `<ChildContent>`
- Preserve named `<ContentPlaceHolder ID="...">` regions and their default content
- Group child-page overrides under `<ChildComponents>` as `<Content ContentPlaceHolderID="...">`
- Refactor to native `@layout` + `@Body` later only when the shell becomes a true single-slot layout

---

## Control-Specific Notes

<!--
  FILL IN: Add notes for controls your app actually uses.
  Delete rows for controls you don't use.
-->

| Control | Your Usage | Migration Notes |
|---|---|---|
| GridView | [e.g., ProductGrid on Products.aspx] | `Items="products"` + `TItem="Product"` |
| ListView | [e.g., CategoryList on Default.aspx] | `Items="categories"` + `TItem="Category"` |
| FormView | [e.g., ProductDetail on Detail.aspx] | `DataItem="product"` + `TItem="Product"` |
| TextBox | [e.g., Login form fields] | Add `@bind-Text="model.Property"` |
| DropDownList | [e.g., Category filter] | Bind `Items` + `@bind-SelectedValue` |
| Validators | [e.g., Login/Register forms] | Nearly 1:1 — wrap form in `<EditForm>` |

---

## Architecture Decisions Made

<!--
  FILL IN: As you make Layer 3 decisions, document them here so Copilot
  applies them consistently across all pages.
-->

### Data Access
<!-- Example: "We use EF Core with repository pattern. All data access goes through
     services in the Services/ folder. Register with AddScoped<IXService, XService>()." -->

[Describe your data access approach]

### Authentication
<!-- Example: "Migrated from ASP.NET Membership to ASP.NET Core Identity.
     Use [Authorize] attribute and <AuthorizeView> component." -->

[Describe your auth approach]

### Session State
<!-- Example: "Converted Session['cart'] to a scoped CartService registered in Program.cs.
     No ProtectedSessionStorage needed — all state is in-memory per circuit." -->

[Describe your session state approach]

### Navigation / Routing
<!-- Example: "Preserve original URL structure. Products.aspx → @page '/Products'.
     Product detail uses route parameter: @page '/Products/{ProductId:int}'." -->

[Describe your routing strategy]

---

## Routing Table

<!--
  FILL IN: Map your old .aspx URLs to new @page directives.
  This helps Copilot set the correct routes on each page.
-->

| Original URL | Blazor Route | Page File |
|---|---|---|
| `/Default.aspx` | `@page "/"` | `Pages/Index.razor` |
| `/Products.aspx` | `@page "/Products"` | `Pages/Products.razor` |
| `/Products/Detail.aspx?id=5` | `@page "/Products/{ProductId:int}"` | `Pages/ProductDetail.razor` |
<!-- Add your pages here -->

---

## Attributes to Remove Silently

When Copilot encounters these attributes during migration, remove them without comment — they have no Blazor equivalent:

`runat="server"`, `AutoEventWireup`, `CodeBehind`, `CodeFile`, `Inherits` (usually), `EnableViewState`, `ViewStateMode`, `ValidateRequest`, `MaintainScrollPositionOnPostBack`, `ClientIDMode`, `EnableTheming`, `SkinID`

---

## Common Gotchas

1. **No ViewState** — Blazor components maintain state in fields/properties. `ViewStateDictionary` shim available for compile-compat via `WebFormsPageBase.ViewState`.
2. **IsPostBack works AS-IS** — `WebFormsPageBase` provides `IsPostBack` (SSR: checks HTTP method; Interactive: tracks render count). `if (!IsPostBack)` compiles and runs correctly.
3. **No DataSource controls** — `SqlDataSource`, `ObjectDataSource`, `EntityDataSource` must be replaced with injected services.
4. **Template Context** — In Web Forms, `Item` is implicitly available. In BWFC, add `Context="Item"` on template elements.
5. **ID Rendering** — Blazor doesn't generate client IDs like `ctl00_MainContent_Grid`. If CSS/JS targets these IDs, switch to class selectors.
6. **String formatting** — Replace `<%#: string.Format("{0:C}", Item.Price) %>` with `@Item.Price.ToString("C")`.
7. **TextBox TextMode** — The value is `Multiline` (not `MultiLine`) in BWFC.
8. **Visibility** — `Visible="false"` works in BWFC, but prefer `@if (condition)` for dynamic visibility.
9. **ClientScript migration** — `Page.ClientScript.RegisterStartupScript()` works via `ClientScriptShim`. For new code, prefer `IJSRuntime`.
10. **Form POST data** — Use `<WebFormsForm OnSubmit="SetRequestFormData">` to enable `Request.Form["key"]` in interactive mode.
11. **Server.Transfer has NO shim** — There is no BWFC shim for `Server.Transfer()`. Use `NavigationManager.NavigateTo()` instead. Server.Transfer does server-side URL rewriting which doesn't exist in Blazor.
12. **Server.GetLastError/ClearError have NO shim** — Use `ILogger` and middleware-based error handling (`app.UseExceptionHandler`).
13. **ThreadAbortException is dead code** — Web Forms throws `ThreadAbortException` on `Response.Redirect(url, true)`. Blazor does not. Any `catch (ThreadAbortException)` blocks are dead code after migration — review and remove.
14. **HttpContext.Current doesn't work in Blazor** — Static `HttpContext.Current.Session["key"]` must be replaced with `Session["key"]` (on pages) or constructor-injected `SessionShim` (in non-page classes). The CLI tool handles this replacement automatically.

---

## References

- [BWFC Migration Skill (full rules)](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/migration-toolkit/skills/bwfc-migration/SKILL.md)
- [BWFC Control Coverage](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/migration-toolkit/CONTROL-COVERAGE.md)
- [BWFC Migration Toolkit](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/migration-toolkit/README.md)
