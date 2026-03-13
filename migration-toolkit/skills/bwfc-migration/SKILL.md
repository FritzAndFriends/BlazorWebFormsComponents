---
name: bwfc-migration
description: "Migrate ASP.NET Web Forms .aspx/.ascx/.master markup to Blazor Server using BlazorWebFormsComponents (BWFC). Covers control translation, expression conversion, data binding, code-behind lifecycle, and Master Page to Layout conversion. WHEN: \"migrate aspx\", \"convert web forms markup\", \"master page to layout\", \"asp prefix removal\", \"data binding expressions\". FOR SINGLE OPERATIONS: use /bwfc-identity-migration for auth, /bwfc-data-migration for EF/architecture."
---

# Web Forms → Blazor Markup Migration with BWFC

This skill provides transformation rules for migrating ASP.NET Web Forms markup to Blazor Server using the **BlazorWebFormsComponents** (BWFC) NuGet package.

**Related skills:**
- `/bwfc-identity-migration` — ASP.NET Identity/Membership → Blazor Identity
- `/bwfc-data-migration` — EF6 → EF Core, DataSource → services, architecture decisions

## What Is BWFC?

BlazorWebFormsComponents is an open-source library that provides **drop-in Blazor replacements** for ASP.NET Web Forms server controls. It preserves the same component names, attribute names, and rendered HTML output — enabling migration with minimal markup changes.

- **NuGet Package:** <https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents>
- **GitHub Repository:** <https://github.com/FritzAndFriends/BlazorWebFormsComponents>
- **58 components** across 6 categories
- **Same HTML output** — existing CSS and JavaScript continue to work

> **Core Principle:** Strip `asp:` and `runat="server"`, keep everything else, and it just works.

---

## Installation

### Step 1: Create Blazor Server Project

```bash
dotnet new blazor -n MyBlazorApp --interactivity Server
cd MyBlazorApp
dotnet add package Fritz.BlazorWebFormsComponents
```

### Step 2: Configure `_Imports.razor`

```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

The `@inherits` line makes every page inherit from `WebFormsPageBase`, which provides `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`, and `IsPostBack` — so Web Forms code-behind patterns compile unchanged. No per-page `@inject IPageService` is needed for page-level usage. Individual pages can override with their own `@inherits` if needed.

> **Note:** The `@using static` import lets you write `InteractiveServer` as shorthand in `App.razor`. Do **not** add `@rendermode InteractiveServer` as a line in `_Imports.razor` — `@rendermode` is a directive attribute that belongs on component instances, not a standalone directive.

> **Note:** `@inject IPageService` is still valid for non-page components (e.g., a shared header component) that need access to page metadata. `WebFormsPageBase` only applies to routable pages.

### Step 2b: Configure Render Mode in `App.razor`

The `dotnet new blazor --interactivity Server` template generates `App.razor` with render mode already set. Verify it contains:

```razor
<HeadOutlet @rendermode="InteractiveServer" />
...
<Routes @rendermode="InteractiveServer" />
```

This enables global server interactivity for all pages. See [ASP.NET Core Blazor render modes](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes) for per-page alternatives.

### Step 3: Register BWFC Services and Add Page Component to Layout

In `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
```

In your layout file (`MainLayout.razor`), add the `<Page />` render component once. This subscribes to `IPageService` and emits `<PageTitle>` and `<meta>` tags:

```razor
@inherits LayoutComponentBase

<BlazorWebFormsComponents.Page />

<header>
    <!-- ... -->
</header>
<main>
    @Body
</main>
```

> **Important:** `WebFormsPageBase` provides the code-behind API (`Page.Title`, `IsPostBack`). The `<BlazorWebFormsComponents.Page />` component does the rendering (`<PageTitle>`, `<meta>` tags). Both are required.

### Step 4: Add BWFC JavaScript

In `App.razor` or the host page `<head>`:

```html
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

---

## Migration Pipeline — MANDATORY

> ⚠️ **CRITICAL: The migration pipeline is a two-layer automated sequence. Both layers MUST run. Do NOT make any manual code fixes between Layer 1 and Layer 2.**
> The migration pipeline measures script quality. Manual fixes between layers corrupt the measurement. If Layer 1 output has issues, those issues should be fixed in the script, not patched by hand.

The migration pipeline has **two mandatory layers** that run in strict sequence:

| Step | Layer | Executor | Description |
|------|-------|----------|-------------|
| 1 | **Layer 1: Mechanical** | **Automated script** (`bwfc-migrate.ps1`) | Tag transforms, expression conversion, file renaming, scaffolding |
| 2 | **Layer 2: Structural** | **Copilot-assisted** (this skill) | Data binding, lifecycle, templates, layouts |
| 3 | Build & verify | Copilot | `dotnet build`, fix any remaining compile errors |
| 4 | Report | Copilot | Document results |

### Layer 1 — Run the Migration Script

**You MUST run Layer 1 as a PowerShell script. Do NOT apply Layer 1 transforms manually.**

```powershell
.\migration-toolkit\scripts\bwfc-migrate.ps1 -Path "<source-webforms-project>" -Output "<blazor-output-dir>"
```

- `-Path` — path to the source Web Forms project directory (containing `.aspx`, `.ascx`, `.master` files)
- `-Output` — path to the target Blazor project directory (will be created if it doesn't exist)
- Layer 1 typically completes in 1–2 seconds and processes 30+ files
- The script performs ALL mechanical transforms: `asp:` prefix removal, `runat="server"` removal, expression conversion, URL rewriting, file renaming, scaffold generation (`.csproj`, `Program.cs`, `_Imports.razor`, `App.razor`, etc.)

**What Layer 1 handles:**

- Remove all `asp:` tag prefixes
- Remove all `runat="server"` attributes
- Convert expressions: `<%: expr %>` → `@(expr)`, `<%# Item.X %>` → `@context.X`
- Convert URLs: `~/path` → `/path`
- Rename files: `.aspx` → `.razor`, `.ascx` → `.razor`, `.master` → `.razor`
- Remove `<asp:Content>` wrappers
- Convert `<%@ Page %>` directives to `@page "/route"`
- Remove `<form runat="server">` wrapper
- LoginView preservation (keeps BWFC LoginView, does NOT rewrite as AuthorizeView)
- Master page → MainLayout.razor conversion
- Scaffold generation (csproj, Program.cs, _Imports.razor, App.razor)

### Layer 2 — Copilot Transforms

**After Layer 1 completes, immediately proceed to Layer 2. Do NOT fix, edit, or clean up any Layer 1 output first.**

Layer 2 is where Copilot applies structural transforms to every generated `.razor` and `.razor.cs` file. Work through each file and apply ALL of the following:

> **⚠️ MANDATORY: SelectMethod MUST be preserved as a delegate.** When the original Web Forms markup has `SelectMethod="MethodName"`, the migrated Blazor markup MUST have `SelectMethod="@service.MethodName"` (or explicit lambda). Do NOT convert to `Items=` binding — this is the #1 recurring migration error.

- Preserve `SelectMethod` — convert string method name to `SelectHandler<ItemType>` delegate (e.g., `SelectMethod="@productService.GetProducts"` if signature matches, or `SelectMethod="@((maxRows, startRow, sort, out total) => service.GetProducts(maxRows, startRow, sort, out total))"` for explicit wiring). BWFC's `DataBoundComponent.OnAfterRenderAsync` automatically calls the delegate to populate `Items`.
- Preserve `ItemType` attribute — BWFC data controls use `ItemType` (matches Web Forms `DataBoundControl.ItemType`). Do NOT change to `TItem` or any other name.
- Add `Context="Item"` to `<ItemTemplate>` elements
- Migrate code-behind: `Page_Load` → `OnInitializedAsync`
- Convert `Response.Redirect` → `NavigationManager.NavigateTo`
- Wire `EditForm` where form validation is needed
- Convert Master Page → Blazor Layout
- Ensure null-safe collection access when using `Items` (for `DataSource`-originating data only): `Items="@(_products ?? new())"`
- When `SelectMethod` is set, `Items` is auto-populated by the BWFC framework — do NOT also set `Items`
- **Database provider:** Verify the L1-detected provider from the `[DatabaseProvider]` review item. Use the detected EF Core package and connection string. Do NOT substitute providers (e.g., do not use SQLite when the original used SQL Server).
- Add `@inject` directives for required services (NavigationManager, DbContext, etc.)
- Convert `Session["key"]` → scoped DI service patterns

### Pipeline Rules

1. **Run Layer 1 first** — always via the script, never manually
2. **Run Layer 2 immediately after** — no fixes between layers
3. **Build** — run `dotnet build` and fix compile errors
4. **Report** — document what was migrated and any issues

## Migration Workflow

This skill covers **Layers 1 and 2** of the three-layer pipeline. Use the related skills for Layer 3.

| Layer | What It Handles | Skill |
|-------|----------------|-------|
| **Layer 1: Mechanical** | Tag prefixes, `runat`, expressions, URLs, file renaming | ✅ This skill (automated via `bwfc-migrate.ps1`) |
| **Layer 2: Structural** | Data binding, code-behind lifecycle, templates, layouts | ✅ This skill (Copilot-assisted) |
| **Layer 3: Architecture** | State management, data access, auth, middleware | `/bwfc-data-migration`, `/bwfc-identity-migration` |

---

## Page Migration Rules

### File Conversion

| Web Forms | Blazor |
|-----------|--------|
| `MyPage.aspx` | `MyPage.razor` |
| `MyPage.aspx.cs` | `MyPage.razor.cs` (partial class) or `@code { }` block |
| `MyControl.ascx` | `MyControl.razor` |
| `MyControl.ascx.cs` | `MyControl.razor.cs` |
| `Site.Master` | `MainLayout.razor` |
| `Site.Master.cs` | `MainLayout.razor.cs` |

### Directive Conversion

| Web Forms Directive | Blazor Equivalent |
|--------------------|-------------------|
| `<%@ Page Title="X" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Y.aspx.cs" Inherits="NS.Y" %>` | `@page "/route"` |
| `<%@ Master Language="C#" ... %>` | (remove — layouts don't need directives) |
| `<%@ Control Language="C#" ... %>` | (remove — components don't need directives) |
| `<%@ Register TagPrefix="uc" TagName="X" Src="~/Controls/X.ascx" %>` | `@using MyApp.Components` (if needed) |
| `<%@ Import Namespace="X" %>` | `@using X` |

**Drop entirely** (no Blazor equivalent): `AutoEventWireup`, `CodeBehind`/`CodeFile`, `Inherits`, `EnableViewState`/`ViewStateMode`, `MasterPageFile`, `ValidateRequest`, `MaintainScrollPositionOnPostBack`, `ClientIDMode`, `EnableTheming`, `SkinID`

### Expression Conversion

| Web Forms Expression | Blazor Equivalent | Notes |
|---------------------|-------------------|-------|
| `<%: expression %>` | `@(expression)` | HTML-encoded output |
| `<%= expression %>` | `@(expression)` | Blazor always HTML-encodes |
| `<%# Item.Property %>` | `@context.Property` | Inside data-bound templates |
| `<%#: Item.Property %>` | `@context.Property` | Same — Blazor always encodes |
| `<%# Eval("Property") %>` | `@context.Property` | Direct property access |
| `<%# Bind("Property") %>` | `@bind-Value="context.Property"` | Two-way binding |
| `<%# string.Format("{0:C}", Item.Price) %>` | `@context.Price.ToString("C")` | Format in code |
| `<%$ RouteValue:id %>` | `@Id` (with `[Parameter]`) | Route parameters |
| `<%-- comment --%>` | `@* comment *@` | Razor comments |
| `<% if (condition) { %>` | `@if (condition) {` | Control flow |
| `<% foreach (var x in items) { %>` | `@foreach (var x in items) {` | Loops |

### Route URL Conversion

| Web Forms | Blazor |
|-----------|--------|
| `href="~/Products"` | `href="/Products"` |
| `NavigateUrl="~/Products/<%: Item.ID %>"` | `NavigateUrl="@($"/Products/{context.ID}")"` |
| `<%: GetRouteUrl("ProductRoute", new { id = Item.ID }) %>` | `@($"/Products/{context.ID}")` or use BWFC's `GetRouteUrlHelper` extension (see below) |
| `Response.Redirect("~/Products")` | `NavigationManager.NavigateTo("/Products")` |

> **BWFC GetRouteUrlHelper:** BWFC provides a `GetRouteUrlHelper` extension method on `BaseWebFormsComponent` that wraps ASP.NET Core's `LinkGenerator`. Inside any BWFC component, you can call `this.GetRouteUrl("RouteName", new { id = item.ID })` directly — no manual URL construction needed. Register routes via ASP.NET Core's routing system and the helper maps them automatically.

### Content/Layout Conversion

| Web Forms | Blazor |
|-----------|--------|
| `<asp:Content ContentPlaceHolderID="MainContent" runat="server">` | (remove — page body IS the content) |
| `<asp:Content ContentPlaceHolderID="HeadContent" runat="server">` | `<HeadContent>` ... `</HeadContent>` |
| `<asp:ContentPlaceHolder ID="MainContent" runat="server" />` | `@Body` (in layout) |

### Form Wrapper

- **Remove the `<form runat="server">` wrapper** entirely
- For forms that need validation, use `<EditForm Model="@model">` instead


## Reference Documents

Detailed control mappings and code transformation patterns are in child documents:

- **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)**  Control translation tables (Simple, Form, Validation, Data, Navigation, AJAX controls), component coverage summary (58 components), structural components, theming, and custom control base classes.
- **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)**  Code-behind lifecycle mapping, event handler conversion, navigation patterns, data binding migration (SelectMethod, template binding), and Master Page to Layout conversion.

---

## Common Gotchas

### No ViewState
Replace `ViewState["key"]` with component fields.

### No PostBack
`if (!IsPostBack)` → works AS-IS with `WebFormsPageBase` (always enters the block). `if (IsPostBack)` (without `!`) → **dead code** in Blazor; flag for manual review and move logic to event handlers.

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

### `runat="server"` on HTML Elements
Remove `runat="server"` from plain HTML elements. Use `@ref` if programmatic access needed.

### Event Handler Signatures
```csharp
// Web Forms: protected void Btn_Click(object sender, EventArgs e) { }
// Blazor:    private void Btn_Click() { }
```

### `TextMode="MultiLine"` Casing
BWFC uses `Multiline` (lowercase 'l'), not `MultiLine`. Silent failure if wrong.

### ScriptManager/ScriptManagerProxy Are No-Ops
Include during migration to prevent errors, remove when stable.

---

## Per-Page Migration Checklist

```markdown
## Page: [PageName.aspx] → [PageName.razor]

### Layer 1 — Mechanical
- [ ] File renamed (.aspx → .razor)
- [ ] <%@ Page %> → @page "/route"
- [ ] asp: prefixes removed
- [ ] runat="server" removed
- [ ] Expressions converted
- [ ] URLs converted (~/ → /)
- [ ] <asp:Content> wrappers removed
- [ ] <form runat="server"> removed

### Layer 2 — Structural
- [ ] SelectMethod string → SelectHandler delegate
- [ ] ItemType preserved (strip namespace prefix only)
- [ ] Data loading in OnInitializedAsync
- [ ] Event handlers converted
- [ ] Template Context="Item" added
- [ ] Navigation calls converted

### Verification
- [ ] Builds without errors
- [ ] Renders correctly
- [ ] Interactive features work
- [ ] No browser console errors
```

---

