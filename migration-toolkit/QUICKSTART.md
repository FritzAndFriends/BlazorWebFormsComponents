# Quickstart: Scan → Migrate → Verify

**Go from "I have a Web Forms app" to "I have a running Blazor app" in the shortest path.**

This guide walks you through the linear steps. It doesn't explain *why* each step exists — see [METHODOLOGY.md](METHODOLOGY.md) for the theory behind the pipeline.

---

## Before You Start

- [ ] .NET 8+ SDK installed (`dotnet --version`)
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

> 📄 Script reference: [`scripts/bwfc-scan.ps1`](../scripts/bwfc-scan.ps1)

---

## Step 3: Run Layer 1 — Automated Transforms

The migration script handles the mechanical work: stripping `asp:` prefixes, removing `runat="server"`, converting expressions, renaming files, and scaffolding the Blazor project:

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
.\scripts\bwfc-migrate.ps1 -Path "C:\src\MyWebFormsApp" -Output "C:\src\MyBlazorApp" -WhatIf
```

> 📄 Script reference: [`scripts/bwfc-migrate.ps1`](../scripts/bwfc-migrate.ps1)

---

## Step 4: Configure BWFC in the Blazor Project

After the migration script runs, verify these are in place (the script scaffolds them, but check):

**`_Imports.razor`** — add BWFC namespaces:
```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
```

**`Program.cs`** — register BWFC services:
```csharp
builder.Services.AddBlazorWebFormsComponents();
```

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

> 📄 Skill file: [`.github/skills/webforms-migration/SKILL.md`](../.github/skills/webforms-migration/SKILL.md)

---

## Step 6: Walk Through Layer 2 — Copilot-Assisted Transforms

Open each migrated `.razor` file and work through the structural transforms that the script couldn't handle. These are the patterns Copilot handles well with the migration skill:

| Transform | What To Do |
|---|---|
| `SelectMethod` → `Items` | Replace `SelectMethod="GetProducts"` with `Items="products"`, load data in `OnInitializedAsync` |
| `ItemType` → `TItem` | Already done by Layer 1, but verify generic type parameter is correct |
| Template context | Add `Context="Item"` to `<ItemTemplate>`, `<EditItemTemplate>`, etc. |
| Code-behind lifecycle | Convert `Page_Load` → `OnInitializedAsync`, remove `IsPostBack` checks |
| Event handlers | Convert `void Btn_Click(object sender, EventArgs e)` → `void Btn_Click()` |
| Navigation | Replace `Response.Redirect("~/path")` → `NavigationManager.NavigateTo("/path")` |
| Form wrappers | Remove `<form runat="server">`, use `<EditForm>` where validation is needed |
| Master Page → Layout | Convert to `@inherits LayoutComponentBase` with `@Body` |

Look for `<!-- TODO: BWFC-MIGRATE -->` comments left by the migration script — these mark items that need manual attention.

---

## Step 7: Address Layer 3 — Architecture Decisions

These are the decisions that need a human (or a human + the migration agent):

- **Data access:** Replace `SqlDataSource`/`ObjectDataSource` with injected services
- **Session state:** Convert `Session["key"]` to scoped services or `ProtectedSessionStorage`
- **Authentication:** Migrate ASP.NET Membership/Identity to ASP.NET Core Identity
- **EF6 → EF Core:** Update DbContext, register with DI, adjust LINQ queries
- **Global.asax → Program.cs:** Convert lifecycle hooks to middleware
- **Third-party integrations:** Port to `HttpClient` pattern

> 📄 For interactive guidance, use the [Migration Agent](../.github/agents/migration.agent.md)

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

Use the [per-page checklist](CHECKLIST.md) to track progress across your application. Migrate pages in priority order:

1. **Leaf pages first** — simple display pages with no dependencies
2. **Shared layouts** — Master Page → `MainLayout.razor`
3. **Data-bound pages** — pages with GridView, ListView, FormView
4. **Auth-dependent pages** — login, account management
5. **Integration pages** — checkout, payment, external APIs

---

## What Comes Next

| If you need... | Go to... |
|---|---|
| Understand the pipeline theory | [METHODOLOGY.md](METHODOLOGY.md) |
| Check if a specific control is supported | [CONTROL-COVERAGE.md](CONTROL-COVERAGE.md) |
| Track per-page migration progress | [CHECKLIST.md](CHECKLIST.md) |
| Set up Copilot instructions for your team | [copilot-instructions-template.md](copilot-instructions-template.md) |
