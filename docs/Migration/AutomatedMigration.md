# Automated Migration Guide

This guide explains how to use the BlazorWebFormsComponents migration tooling to convert ASP.NET Web Forms applications to Blazor Server with minimal manual effort.

## Overview

The BWFC migration system uses a three-layer pipeline:

| Layer | Tool | Automation | What It Handles |
|-------|------|-----------|-----------------|
| **1. Scanner** | `bwfc-scan.ps1` | Inventory | Analyzes your Web Forms project and reports migration readiness |
| **2. Script** | `bwfc-migrate.ps1` | ~40% | Mechanical regex transforms (strip `asp:`, fix expressions, rename files) |
| **3. Copilot Skill** | `webforms-migration` | ~45% | Structural transforms (code-behind, data binding, lifecycle methods) |
| **4. Agent** | `migration.agent.md` | ~15% | Semantic decisions (Session→DI, Identity, EF Core, architecture) |

!!! tip "Core Principle"
    Strip `asp:` and `runat="server"`, keep everything else, and it just works. BWFC components match Web Forms control names, property names, and rendered HTML.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- PowerShell 7+ (for migration scripts)
- GitHub Copilot (for skill and agent layers)
- Your Web Forms application source code

## Step 1: Scan Your Project

Run the scanner to inventory your Web Forms project:

```powershell
pwsh scripts/bwfc-scan.ps1 -Path ./MyWebFormsApp
```

The scanner reports:

- All `.aspx`, `.ascx`, and `.master` files found
- Every `<asp:Control>` used and how many times
- Which controls have BWFC equivalents (52 supported controls)
- Which controls need manual migration (DataSource controls, Wizard, etc.)
- An overall migration readiness percentage

### Reading the Scan Report

```
=== BWFC Migration Scan Report ===

Files: 12 .aspx, 3 .ascx, 1 .master
Controls: 47 total across 15 unique types

✅ Supported (96.6%):  Button(8), TextBox(12), Label(6), GridView(2)...
❌ Not Supported (3.4%): SqlDataSource(2)

Migration Readiness: 96.6%
```

Controls marked ❌ need manual replacement with injected services. See [Data Binding Migration](#replacing-datasource-controls) below.

### Export Options

```powershell
# JSON for tooling
pwsh scripts/bwfc-scan.ps1 -Path ./MyWebFormsApp -OutputFormat Json -OutputFile scan.json

# Markdown for documentation
pwsh scripts/bwfc-scan.ps1 -Path ./MyWebFormsApp -OutputFormat Markdown -OutputFile scan.md
```

## Step 2: Run Mechanical Transforms

The migration script handles safe, mechanical transforms:

```powershell
pwsh scripts/bwfc-migrate.ps1 -Path ./MyWebFormsApp -Output ./MyBlazorApp
```

### What the Script Does

| Transform | Before | After |
|-----------|--------|-------|
| File rename | `Products.aspx` | `Products.razor` |
| Strip `asp:` prefix | `<asp:Button>` | `<Button>` |
| Remove `runat` | `runat="server"` | (removed) |
| Fix expressions | `<%: value %>` | `@(value)` |
| Fix data binding | `<%#: Item.Name %>` | `@context.Name` |
| Fix comments | `<%-- text --%>` | `@* text *@` |
| Fix URLs | `href="~/path"` | `href="/path"` |
| Strip directives | `<%@ Page ... %>` | `@page "/route"` |
| Remove wrappers | `<asp:Content ...>` | (removed) |
| Remove `<form runat>` | `<form runat="server">` | (removed) |
| Fix type params | `ItemType="NS.Class"` | `TItem="Class"` |
| Remove dead attrs | `EnableViewState`, `AutoEventWireup` | (removed) |

### Preview Mode

Use `-WhatIf` to see what would change without writing files:

```powershell
pwsh scripts/bwfc-migrate.ps1 -Path ./MyWebFormsApp -Output ./MyBlazorApp -WhatIf
```

### Output Structure

The script creates a Blazor Server project with:

```
MyBlazorApp/
├── MyBlazorApp.csproj          # With Fritz.BlazorWebFormsComponents reference
├── Program.cs                   # With AddBlazorWebFormsComponents()
├── _Imports.razor               # With BWFC usings
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor     # From Site.Master
│   └── Pages/
│       ├── Index.razor          # From Default.aspx
│       ├── Products.razor       # From Products.aspx
│       └── ...
├── Models/                      # Copied from source
└── Services/                    # Stub services for DataSource replacements
```

## Step 3: Apply Copilot Skill

Open the migrated project in VS Code with GitHub Copilot. The `webforms-migration` skill provides transformation rules for structural changes the script couldn't handle.

### Using the Skill

Ask Copilot to apply the skill to each file:

> "Using the webforms-migration skill, migrate the code-behind for Products.razor"

The skill handles:

- **Code-behind migration**: `Page_Load` → `OnInitializedAsync`, event handlers → `EventCallback`
- **Data binding**: `SelectMethod` → service injection + `Items` property
- **Navigation**: `Response.Redirect` → `NavigationManager.NavigateTo`
- **Visibility**: `Visible="false"` → `@if` conditional blocks
- **Route parameters**: `[QueryString]` → `[SupplyParameterFromQuery]`
- **Template context**: Adding `Context="Item"` to data-bound templates

### Example: Code-Behind Migration

**Before** (`Products.aspx.cs`):
```csharp
public partial class Products : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGrid();
        }
    }

    public IQueryable<Product> GetProducts()
    {
        var db = new ProductContext();
        return db.Products.OrderBy(p => p.Name);
    }

    protected void DeleteBtn_Click(object sender, EventArgs e)
    {
        // delete logic
        Response.Redirect("~/Products");
    }
}
```

**After** (`Products.razor.cs`):
```csharp
public partial class Products : ComponentBase
{
    [Inject] private IProductService ProductService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private List<Product> products = new();

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetProductsAsync();
    }

    private async Task DeleteBtn_Click()
    {
        // delete logic
        NavigationManager.NavigateTo("/Products");
    }
}
```

## Step 4: Resolve Semantic Decisions

For the remaining ~15% of migration work, use the migration agent for guidance on architectural decisions:

> "Using the migration agent, help me migrate the session state and authentication from my Web Forms app"

The agent guides decisions like:

- **Session state** → Scoped services or `ProtectedSessionStorage`
- **ASP.NET Identity** → Blazor Identity with `AuthenticationStateProvider`
- **Entity Framework 6** → EF Core with async patterns
- **Global.asax** → `Program.cs` middleware pipeline
- **Web.config** → `appsettings.json`
- **HTTP Handlers/Modules** → ASP.NET Core middleware

## Step 5: Build and Verify

```bash
cd MyBlazorApp
dotnet build
dotnet run
```

!!! warning "Common Build Errors"
    - **Missing service registrations**: Add `builder.Services.AddScoped<IMyService, MyService>()` in `Program.cs`
    - **Namespace mismatches**: Update `@using` directives in `_Imports.razor`
    - **Async mismatch**: Add `async`/`await` to data access methods

## Replacing DataSource Controls

Web Forms DataSource controls (`SqlDataSource`, `ObjectDataSource`, etc.) have no BWFC equivalent. Replace them with injected services:

**Before:**
```xml
<asp:SqlDataSource ID="ProductsDS" runat="server"
    ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
    SelectCommand="SELECT * FROM Products" />
<asp:GridView DataSourceID="ProductsDS" runat="server" />
```

**After:**
```razor
@inject IProductService ProductService

<GridView Items="products" TItem="Product" AutoGenerateColumns="true" />

@code {
    private List<Product> products = new();

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetProductsAsync();
    }
}
```

## Control Coverage

BWFC provides **52 Blazor components** covering the most commonly used Web Forms controls:

| Category | Controls |
|----------|----------|
| Editor | AdRotator, BulletedList, Button, Calendar, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Table, TextBox, View |
| Data | DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater |
| Validation | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| Navigation | Menu, SiteMapPath, TreeView |
| Login | ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery |
| AJAX | ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress |

## See Also

- [Migration Getting Started](readme.md)
- [Master Page Migration](MasterPages.md)
- [Migration Readiness Checklist](migration_readiness.md)
- [Custom Control Migration](Custom-Controls.md)
- [Deferred Controls](DeferredControls.md)
