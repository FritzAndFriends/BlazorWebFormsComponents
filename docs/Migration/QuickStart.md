# Quickstart: Scan → Migrate → Verify

**Go from "I have a Web Forms app" to "I have a running Blazor app" in the shortest path.**

This guide walks you through the linear steps. It doesn't explain *why* each step exists — see [Methodology](Methodology.md) for the theory behind the pipeline.

---

## Before You Start

- [ ] .NET 10+ SDK installed (`dotnet --version`)
- [ ] PowerShell 7+ installed (`pwsh --version`)
- [ ] Your Web Forms project compiles and runs on .NET Framework
- [ ] Git initialized in your project (you'll want to track changes)

---

## Step 1: Install BWFC

Create your Blazor project and add the BWFC package:

```bash
dotnet new blazor -n MyBlazorApp --interactivity Server
cd MyBlazorApp
dotnet add package Fritz.BlazorWebFormsComponents
```

---

## Step 2: Scan Your Web Forms Project

Run the scanner against your existing Web Forms project to understand what you're working with:

```powershell
# From the BWFC repo root
.\scripts\bwfc-scan.ps1 -Path "C:\src\MyWebFormsApp" -OutputFormat Markdown -OutputFile scan-report.md
```

The scanner inventories every `.aspx`, `.ascx`, and `.master` file — extracting control usage, data binding patterns, and DataSource controls. Review the report to understand:

- **Total page count** and complexity distribution
- **Control coverage** — what percentage of your controls BWFC supports
- **DataSource controls** — these need manual replacement (no BWFC equivalent)
- **Migration readiness score** — your starting point

> 📄 Script reference: [scripts/bwfc-scan.ps1](https://github.com/FritzAndFriends/BlazorWebFormsComponents/tree/dev/migration-toolkit/scripts/bwfc-scan.ps1)

---

## Step 3: Run Layer 1 — Automated Transforms

Layer 1 applies mechanical transforms deterministically. Use either the CLI tool (recommended) or the PowerShell script:

**Option A: CLI tool** (37 compiled transforms, migration report):

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate -i "C:\src\MyWebFormsApp" -o "C:\src\MyBlazorApp"
```

**Option B: PowerShell script** (lightweight, no build required):

```powershell
.\scripts\bwfc-migrate.ps1 -Path "C:\src\MyWebFormsApp" -Output "C:\src\MyBlazorApp"
```

**What this does (in ~30 seconds for a typical app):**

| Transform | Example |
|---|---|
| Strip `asp:` prefixes | `<asp:Button>` → `<Button>` |
| Remove `runat="server"` | `runat="server"` → *(removed)* |
| Convert expressions | `<%: Item.Name %>` → `@(Item.Name)` |
| Convert URLs | `~/Products` → `/Products` |
| Rename files | `Default.aspx` → `Default.razor` |
| Convert `ItemType` | `ItemType="NS.Product"` → `TItem="Product"` |
| Remove content wrappers | `<asp:Content>` → *(unwrapped)* |
| Scaffold project | Generates `.csproj`, `Program.cs`, `_Imports.razor` |

**Dry-run first** to preview changes without writing files:

```powershell
# CLI: use the --dry-run flag
dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate -i "C:\src\MyWebFormsApp" -o "C:\src\MyBlazorApp" --dry-run

# PowerShell alternative:
.\scripts\bwfc-migrate.ps1 -Path "C:\src\MyWebFormsApp" -Output "C:\src\MyBlazorApp" -WhatIf
```

> 📄 Script reference: [scripts/bwfc-migrate.ps1](https://github.com/FritzAndFriends/BlazorWebFormsComponents/tree/dev/migration-toolkit/scripts/bwfc-migrate.ps1)

---

## Step 4: Configure BWFC in the Blazor Project

After the migration script runs, verify these are in place (the script scaffolds them, but check):

**`_Imports.razor`** — add BWFC namespaces and page base class:
```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

**This one line gives every page the Web Forms API:**
- `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`
- `IsPostBack` (false on first render, true on interactions)
- `Session["key"]` (scoped in-memory dictionary)
- `Response.Redirect("~/path")` (auto-strips `~/` and `.aspx`)
- `Request.Url`, `Request.QueryString["key"]`, `Request.Form["key"]`
- `Cache["key"]` (application-level cache)
- `Server.MapPath("~/path")` (virtual → physical path)
- `ClientScript.RegisterStartupScript(...)` (JS interop)

Your Web Forms code-behind compiles **unchanged**. No manual conversion needed.

**`Program.cs`** — register BWFC services:
```csharp
builder.Services.AddBlazorWebFormsComponents();
```

**What this does:**
- Registers `SessionShim` (scoped in-memory dictionary for `Session["key"]`)
- Registers `ResponseShim` (handles `Response.Redirect`, `Response.Write`)
- Registers `RequestShim` (provides `Request.QueryString`, `Request.Form`, `Request.Url`)
- Registers `CacheShim` (in-memory application cache)
- Registers `ServerShim` (provides `Server.MapPath`)
- Registers `ClientScriptShim` (JS interop for `ClientScript.RegisterStartupScript`)
- Registers `ViewStateShim` (compile-compatible dictionary)

After this single call, all Web Forms APIs work AS-IS in your migrated code — no manual conversion required.

**Layout (`MainLayout.razor`)** — add the Page render component:
```razor
<BlazorWebFormsComponents.Page />
```

This renders `<PageTitle>` and `<meta>` tags. `WebFormsPageBase` provides the code-behind API, `<Page />` does the rendering — both are required.

**`App.razor`** (or layout head) — add BWFC JavaScript:
```html
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

---

## Step 5: Set Up Copilot for Layer 2

Copy the Copilot instructions template into your project to give Copilot migration-specific context:

```bash
# From your Blazor project root
mkdir -p .github
cp path/to/bwfc-repo/migration-toolkit/copilot-instructions-template.md .github/copilot-instructions.md
```

Then open `.github/copilot-instructions.md` and fill in the `<!-- FILL IN -->` sections with your project-specific details.

Alternatively, point Copilot at the BWFC migration skill directly:

> 📄 Skill file: [Core Migration Skill](CopilotSkills/CoreMigration.md)

---

## Step 6: Walk Through Layer 2 — Copilot-Assisted Transforms

Open each migrated `.razor` file and work through the structural transforms that the script couldn't handle. These are the patterns Copilot handles well with the migration skill:

> 💡 **Many Web Forms API calls now compile unchanged thanks to BWFC shims.** `Response.Redirect`, `Session["key"]`, `IsPostBack`, `Page.Title`, `Request.QueryString`, `Cache`, and `Server.MapPath` all work AS-IS — no conversion needed. Focus Layer 2 effort on data binding, templates, and event handler signatures.

| Transform | What To Do |
|---|---|
| `SelectMethod` → `Items` | Replace `SelectMethod="GetProducts"` with `Items="products"`, load data in `OnInitializedAsync` |
| `ItemType` → `TItem` | Already done by Layer 1, but verify generic type parameter is correct |
| Template context | Add `Context="Item"` to `<ItemTemplate>`, `<EditItemTemplate>`, etc. |
| Code-behind lifecycle | Convert `Page_Load(object sender, EventArgs e)` signature → `OnInitializedAsync`; `IsPostBack` inside works AS-IS |
| Event handlers | Convert `void Btn_Click(object sender, EventArgs e)` → `void Btn_Click()` |
| Form wrappers | Remove `<form runat="server">`; use `<WebFormsForm>` if page uses `Request.Form`, or `<EditForm>` for validation |
| Master Page → Layout | Convert to `@inherits LayoutComponentBase` with `@Body` |

> **If your pages use `Request.Form`**, wrap the form content in `<WebFormsForm>` — this component captures form POST data and feeds the `FormShim` so `Request.Form["key"]` works in your code-behind.

The following are **no longer Layer 2 work** — they work AS-IS via shims:
- ~~`Response.Redirect("~/path")` → `NavigationManager.NavigateTo`~~ → works via ResponseShim
- ~~`Session["key"]` → mark for Layer 3~~ → works via SessionShim
- ~~`Page.Title` conversion~~ → works via WebFormsPageBase

### Using Shims (No Conversion Needed)

**The shims preserve Web Forms API calls AS-IS.** Here's what works unchanged:

```csharp
// Session access — works exactly like Web Forms
Session["CartId"] = cartId;
var cartId = Session["CartId"];

// Response.Redirect — auto-strips ~/ and .aspx
Response.Redirect("~/Products");
Response.Redirect("~/Product.aspx?id=5"); // becomes /Product/5 if routing configured

// Request.QueryString — reads URL parameters
var productId = Request.QueryString["id"];

// Request.Form — reads form POST data (requires <WebFormsForm> wrapper)
var username = Request.Form["username"];

// IsPostBack — false on first render, true on interactions
if (!IsPostBack)
{
    LoadInitialData();
}

// Page properties — auto-rendered by <Page /> component
Page.Title = "Product Details";
Page.MetaDescription = "View product details";

// Cache — application-level cache
Cache["RecentProducts"] = products;

// Server.MapPath — virtual to physical path
var filePath = Server.MapPath("~/App_Data/config.xml");
```

**Do NOT inject these services manually:**
- ❌ `IHttpContextAccessor` — use `Request` property instead
- ❌ `NavigationManager` (for redirects) — use `Response.Redirect()` instead
- ❌ `IMemoryCache` — use `Cache` property instead
- ❌ `IJSRuntime` (for startup scripts) — use `ClientScript.RegisterStartupScript()` instead

The shim properties are already available via `WebFormsPageBase`. Injecting these services and manually converting is extra work that provides no migration benefit.

Look for `<!-- TODO: BWFC-MIGRATE -->` comments left by the migration script — these mark items that need manual attention.

---

## Step 7: Address Layer 3 — Architecture Decisions

These are the decisions that need a human (or a human + the migration agent):

- **Data access:** Replace `SqlDataSource`/`ObjectDataSource` with injected services
- **Session state:** Convert `Session["key"]` to scoped services or `ProtectedSessionStorage` (if you need persistence or distributed sessions — basic usage works AS-IS via SessionShim)
- **Authentication:** Migrate ASP.NET Membership/Identity to ASP.NET Core Identity
- **EF6 → EF Core:** Update DbContext, register with DI, adjust LINQ queries
- **Global.asax → Program.cs:** Convert lifecycle hooks to middleware
- **Third-party integrations:** Port to `HttpClient` pattern
- **Shim replacement (OPTIONAL):** Replace `Response.Redirect()` with `NavigationManager.NavigateTo()`, `Session` with injected state services, etc. — this is a performance/modernization step, NOT a migration requirement

> 📄 For interactive guidance, use the [Data & Architecture Migration Skill](CopilotSkills/DataMigration.md)

### Common Mistakes to Avoid

**Anti-pattern #1: Manually converting shim-supported APIs**

❌ **Wrong:**
```csharp
@inject NavigationManager Nav
@code {
    void GoToProducts() => Nav.NavigateTo("/Products");
}
```

✅ **Correct (use the shim):**
```csharp
@code {
    void GoToProducts() => Response.Redirect("~/Products");
}
```

**Anti-pattern #2: Injecting services that shims already provide**

❌ **Wrong:**
```csharp
@inject IHttpContextAccessor HttpContext
@code {
    var id = HttpContext.HttpContext.Request.Query["id"];
}
```

✅ **Correct (use the shim):**
```csharp
@code {
    var id = Request.QueryString["id"];
}
```

**Anti-pattern #3: Treating shims as temporary scaffolding**

❌ **Wrong mindset:** "I'll use shims to get it compiling, then replace them with 'real' Blazor code."

✅ **Correct mindset:** "Shims are the migration strategy. They work correctly. Replacing them is an optional optimization I can do later if my team wants to reduce BWFC dependency."

The shims ARE the solution, not a workaround.

---

## Step 8: Build and Verify

```bash
dotnet build
```

Fix any compilation errors. Common issues at this stage:

- Missing `@using` statements for model namespaces
- Event handler signature mismatches (Web Forms `EventArgs` vs. Blazor parameterless)
- Unresolved `SelectMethod` references that should be `Items` bindings

Once it builds:

```bash
dotnet run
```

Open the app in a browser and compare against your original Web Forms application:

- [ ] Pages render without errors
- [ ] Visual layout matches the original
- [ ] Interactive features work (buttons, forms, navigation)
- [ ] No console errors in browser dev tools

---

## Step 9: Iterate

Use the [per-page checklist](ChecklistTemplate.md) to track progress across your application. Migrate pages in priority order:

1. **Leaf pages first** — simple display pages with no dependencies
2. **Shared layouts** — Master Page → `MainLayout.razor`
3. **Data-bound pages** — pages with GridView, ListView, FormView
4. **Auth-dependent pages** — login, account management
5. **Integration pages** — checkout, payment, external APIs

---

## What Comes Next

| If you need... | Go to... |
|---|---|
| Understand the pipeline theory | [Methodology](Methodology.md) |
| Check if a specific control is supported | [Control Coverage](ControlCoverage.md) |
| Track per-page migration progress | [Migration Checklist](ChecklistTemplate.md) |
| Set up Copilot instructions for your team | [Copilot Skills Overview](CopilotSkills/Overview.md) |
