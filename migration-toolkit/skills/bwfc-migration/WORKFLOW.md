# Migration Workflow

## Phase 1: L1 Automated Transforms (CLI)

> ⚠️ **CRITICAL: Always run L1 via the CLI tool. Do NOT apply L1 transforms manually.** The tool produces deterministic, testable output. Manual L1 transforms corrupt measurement and miss edge cases.

### Full Project Migration
```bash
webforms-to-blazor migrate -i ./MyWebFormsApp -o ./MyBlazorApp --report migration-report.json --verbose
```

| Option | Description |
|--------|-------------|
| `-i, --input <path>` | Source Web Forms project root (required) |
| `-o, --output <path>` | Output Blazor project directory (required) |
| `--report <path>` | Write JSON migration report to file |
| `--report-format <fmt>` | `json` (default) or `markdown` |
| `--skip-scaffold` | Skip `.csproj`, `Program.cs`, `_Imports.razor` generation |
| `--dry-run` | Show transforms without writing files |
| `-v, --verbose` | Detailed per-file transform log |
| `--overwrite` | Overwrite existing files in output directory |

### Single File Conversion

```bash
webforms-to-blazor convert -i ./Pages/Products.aspx -o ./Pages/ --overwrite
```

| Option | Description |
|--------|-------------|
| `-i, --input <file>` | `.aspx`, `.ascx`, or `.master` file (required) |
| `-o, --output <path>` | Output directory (default: same directory) |
| `--overwrite` | Overwrite existing `.razor` file |

### What L1 Handles (27 Transforms)

**Markup Transforms (16):**

| # | Transform | Description |
|---|-----------|-------------|
| 1 | PageDirective | `<%@ Page %>` → `@page "/route"` with title extraction |
| 2 | MasterDirective | Remove `<%@ Master %>`, add `@inherits LayoutComponentBase` |
| 3 | ControlDirective | Remove `<%@ Control %>` directives |
| 4 | ImportDirective | `<%@ Import Namespace="X" %>` → `@using X` |
| 5 | RegisterDirective | Remove `<%@ Register %>` tag registrations |
| 6 | ContentWrapper | Strip `<asp:Content>` wrappers, convert HeadContent |
| 7 | FormWrapper | `<form runat="server">` → `<div>` (preserves `id` for CSS) |
| 8 | GetRouteUrl | `Page.GetRouteUrl()` → `GetRouteUrlHelper.GetRouteUrl()` |
| 9 | Expression | `<%: %>` → `@()`, `<%# Item.X %>` → `@context.X`, Eval/Bind conversion |
| 10 | LoginView | Strip attributes, flag RoleGroups for review |
| 11 | SelectMethod | Preserve attribute, add TODO for delegate conversion |
| 12 | AjaxToolkitPrefix | `ajaxToolkit:X` → `X` (runs before asp: prefix) |
| 13 | AspPrefix | `asp:X` → `X` for all server controls |
| 14 | AttributeStrip | Remove `runat="server"`, normalize `ID` → `id` |
| 15 | EventWiring | `OnClick="Handler"` → `OnClick="@Handler"` |
| 16 | UrlReference | `~/path` → `/path` in href, NavigateUrl, ImageUrl |

**Code-Behind Transforms (11):**

| # | Transform | Description |
|---|-----------|-------------|
| 1 | UsingStrip | Remove `System.Web.*`, `Microsoft.AspNet.*` usings |
| 2 | BaseClassStrip | Remove `: Page`, `: System.Web.UI.Page` base classes |
| 3 | ResponseRedirect | ⚠️ **DEPRECATED** — L1 converts `Response.Redirect()` → `NavigationManager.NavigateTo()`, but this is WRONG. L2 should revert to `Response.Redirect()` and use ResponseShim. |
| 4 | SessionDetect | Detect `Session["key"]` patterns, inject `// TODO(bwfc-session-state)` guidance |
| 5 | ViewStateDetect | Detect `ViewState["key"]` patterns, inject `// TODO(bwfc-viewstate)` guidance |
| 6 | IsPostBack | Unwrap simple `if (!IsPostBack)` guards; TODO complex guards with `else` |
| 7 | PageLifecycle | `Page_Load` → `OnInitializedAsync`, `Page_Init` → `OnInitialized`, `Page_PreRender` → `OnAfterRenderAsync` |
| 8 | EventHandlerSignature | Strip `(object sender, EventArgs e)` from standard handlers |
| 9 | DataBind | Cross-file: `ctrl.DataSource = x` → field assignment, inject `Items=` in markup |
| 10 | UrlCleanup | `"~/Products.aspx?id=5"` → `"/Products?id=5"` in string literals |
| 11 | AttributeNormalize | Boolean, enum, and unit value normalization |

**Scaffolding:**
- `.csproj` with BWFC NuGet reference
- `Program.cs` with `AddBlazorWebFormsComponents()` — **registers ALL shims automatically** (SessionShim, ResponseShim, RequestShim, ServerShim, CacheShim, ClientScriptShim, FormShim)
- `_Imports.razor` with BWFC usings and `@inherits WebFormsPageBase` — **gives EVERY page access to Session, Response, Request, Server, Cache, ClientScript, ViewState, IsPostBack properties**
- `App.razor` with `InteractiveServer` render mode, detected CSS/JS references
- `Routes.razor`, `GlobalUsings.cs`, `launchSettings.json`
- `appsettings.json` from `web.config` connection strings and app settings
- `WebFormsShims.cs`, `IdentityShims.cs` when applicable
- Copies `App_Start/BundleConfig.cs` and `RouteConfig.cs` as no-op shims

**🔑 Key Point:** The CLI scaffolding sets up the shim infrastructure automatically. You do NOT need to:
- ❌ Manually register shim services in DI
- ❌ Add `[Inject]` attributes for Session, Response, Request, etc.
- ❌ Create custom services for patterns the shims already handle

### Reading the Migration Report

The `--report` flag generates a JSON file that drives L2 decisions:

```json
{
  "summary": {
    "filesProcessed": 24,
    "transformsApplied": 187,
    "todosGenerated": 12,
    "scaffoldFilesCreated": 8
  },
  "todos": [
    {
      "category": "bwfc-session-state",
      "file": "Cart.razor.cs",
      "line": 15,
      "message": "Session[\"CartId\"] detected — convert to scoped service",
      "severity": "warning"
    }
  ]
}
```

**TODO categories** map to L2 sections:
- `bwfc-session-state` → Session shim wiring
- `bwfc-identity-migration` → Auth conversion (delegate to `/bwfc-identity-migration`)
- `bwfc-data-migration` → DataSource → service conversion (delegate to `/bwfc-data-migration`)
- `bwfc-viewstate` → ViewState replacement
- `bwfc-page-lifecycle` → Complex lifecycle patterns L1 couldn't auto-convert
- `bwfc-manual` → Items requiring developer decision

---

## Phase 2: L2 Contextual Transforms (Copilot-Assisted)

After L1 completes, read the migration report (`migration-report.json`). For each TODO category, apply the corresponding transforms below.

> ⚠️ **MANDATORY — READ BEFORE STARTING L2:** Open and read **all three** child documents:
> - **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)** — Lifecycle mapping, event handlers, data binding, Master Page → Shell
> - **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** — 58 BWFC component translation tables
> - **[AJAX-TOOLKIT.md](AJAX-TOOLKIT.md)** — Ajax Control Toolkit extender migration (14 components)

### 🔧 First Step: Revert L1's Response.Redirect Transform

**CRITICAL:** L1's ResponseRedirect transform is WRONG. It converts `Response.Redirect()` to `NavigationManager.NavigateTo()`, which breaks the shim pattern.

**L2 must revert this transform:**

```csharp
// L1 output (WRONG):
[Inject] NavigationManager NavigationManager { get; set; }
NavigationManager.NavigateTo("/Products");

// L2 fix (CORRECT — remove [Inject], restore original):
Response.Redirect("~/Products.aspx");  // ✅ Shim handles this
```

**Search pattern:** Look for `[Inject] NavigationManager` and `NavigationManager.NavigateTo()` calls that originated from Web Forms `Response.Redirect()`.

### TODO(bwfc-session-state)

**✅ The Original Pattern Works AS-IS — no code changes needed:**

```csharp
// Original Web Forms code AND migrated Blazor code (IDENTICAL):
Session["CartId"] = cartId;
var id = Session["CartId"]?.ToString();
```

**Why this works:** `_Imports.razor` has `@inherits WebFormsPageBase` → `Session` property backed by `SessionShim` → auto-registered by `AddBlazorWebFormsComponents()`.

**DO NOT:**
- ❌ Inject `IHttpContextAccessor` to access `HttpContext.Session`
- ❌ Create a custom session service when `SessionShim` exists
- ❌ Change `Session["key"]` to `await SessionStorage.GetAsync("key")`

**For non-page components**, inject `SessionShim` directly:
```razor
@inject SessionShim Session
```

### TODO(bwfc-identity-migration)

Delegate to `/bwfc-identity-migration` skill for full auth migration. Quick pattern:
```csharp
// Before: FormsAuthentication.SignOut();
// After:  await SignInManager.SignOutAsync();
```

### TODO(bwfc-data-migration)

Delegate to `/bwfc-data-migration` skill. Quick pattern:
```csharp
// Before: <asp:SqlDataSource SelectCommand="SELECT * FROM Products" />
// After:  @inject ProductService ProductService
//         <GridView ItemType="Product" SelectMethod="@ProductService.GetProducts" />
```

### TODO(bwfc-viewstate)

```csharp
// Simple value → component field:
// Before: ViewState["SortColumn"] = "Name";
// After:  private string _sortColumn = "Name";

// Complex ViewState → ViewStateDictionary shim (compile-compat, works via WebFormsPageBase)
```

### TODO(bwfc-page-lifecycle)

L1 auto-converts simple lifecycle methods but flags complex patterns:

- **`IsPostBack` with `else`:** Move `if` body to `OnInitializedAsync`, `else` body to event handlers
- **Async `Page_Load`:** Make methods `async Task`, use `await`
- **`Page_PreRender`:** Guard with `if (firstRender)` in `OnAfterRenderAsync`, call `StateHasChanged()`

### TODO(bwfc-manual)

Document in `MIGRATION-NOTES.md`: custom HttpModule/HttpHandler, dynamic control creation, third-party controls, WebParts.

### Data Binding Transforms

> ⚠️ **SelectMethod MUST be preserved as a delegate.** Do NOT convert to `Items=` binding.

**Full L2 checklist for each file:**
- Convert `SelectMethod` string → `SelectHandler<ItemType>` delegate reference
- Preserve `ItemType` attribute (strip namespace prefix only)
- Add `Context="Item"` to `<ItemTemplate>` elements
- Ensure null-safe collection access: `Items="@(_products ?? new())"`
- When `SelectMethod` is set, `Items` is auto-populated — do NOT also set `Items`
- Add `@inject` directives for required services

---

## Phase 3: Build & Verify

```bash
cd MyBlazorApp
dotnet build
```

**Common build errors and fixes:**

| Error | Cause | Fix |
|-------|-------|-----|
| `CS0246: 'Page' could not be found` | Missing `@inherits WebFormsPageBase` | Verify `_Imports.razor` has `@inherits BlazorWebFormsComponents.WebFormsPageBase` |
| `CS0103: 'Session' does not exist` | Non-page component using Session | Add `@inject SessionShim Session` |
| `CS1061: 'X' ... 'DataBind'` | Explicit `.DataBind()` calls remaining | Remove — BWFC auto-binds via `SelectMethod` or `Items` |
| `CS0234: 'Web' does not exist` | Remaining `System.Web.*` using | Remove unless it's a BWFC shim namespace |
| `RZ9986: Complex content` | Expression in attribute without `@()` | Wrap: `Value="@(expr)"` |

---

## Phase 4: L3 Developer Tasks

Require human judgment:
- Custom controls → manual Blazor component creation
- Business logic review → verify async behavior
- Authentication flows → `/bwfc-identity-migration`
- Data architecture → `/bwfc-data-migration`
- Performance tuning → `StateHasChanged()` optimization, virtualization
- Integration testing → verify forms, navigation, data ops end-to-end

---

## Per-Page Migration Checklist

```markdown
## Page: [PageName.aspx] → [PageName.razor]

### L1 — CLI Tool (automated)
- [ ] `webforms-to-blazor migrate` or `convert` executed
- [ ] Migration report reviewed
- [ ] File renamed (.aspx → .razor)
- [ ] Directives, asp: prefixes, runat="server" converted
- [ ] Expressions and URLs converted
- [ ] IsPostBack guards unwrapped/TODO'd

### L2 — Copilot Transforms (per TODO category)
- [ ] TODO(bwfc-session-state) items resolved
- [ ] TODO(bwfc-viewstate) items resolved
- [ ] TODO(bwfc-page-lifecycle) items resolved
- [ ] TODO(bwfc-data-migration) items resolved or delegated
- [ ] TODO(bwfc-identity-migration) items resolved or delegated
- [ ] TODO(bwfc-manual) items documented
- [ ] SelectMethod string → SelectHandler delegate
- [ ] Template Context="Item" verified
- [ ] @inject directives added

### Verification
- [ ] `dotnet build` succeeds
- [ ] Page renders correctly
- [ ] Interactive features work
- [ ] No browser console errors
```
