# BWFC Configuration Reference

## Project Setup (scaffolded by L1)

**`_Imports.razor`:**
```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

The `@inherits` line gives every page `Page.Title`, `Page.MetaDescription`, `IsPostBack`, `Session`, `Server`, `Response`, `Request`, `Cache`, `ViewState`, `ClientScript`, `PostBack` event, `ResolveUrl()`, and `GetRouteUrl()` — so Web Forms code-behind compiles unchanged.

> **Note:** `@rendermode InteractiveServer` is a directive attribute for component instances, NOT a standalone line in `_Imports.razor`.

**`Program.cs`:**
```csharp
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();
app.UseConfigurationManagerShim();
```

**`App.razor`** — render mode and BWFC script:
```razor
<HeadOutlet @rendermode="InteractiveServer" />
<Routes @rendermode="InteractiveServer" />
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

**Layout** (`MainLayout.razor`):
```razor
@inherits LayoutComponentBase

<BlazorWebFormsComponents.Page />

<header><!-- ... --></header>
<main>@Body</main>
```

> **Important:** `WebFormsPageBase` provides the code-behind API. The `<BlazorWebFormsComponents.Page />` component renders `<PageTitle>` and `<meta>` tags. Both are required.

## Available Shims

| Shim | Web Forms API | Blazor Implementation | Setup |
|------|--------------|----------------------|-------|
| **ConfigurationManager** | `ConfigurationManager.AppSettings["key"]`, `.ConnectionStrings["name"]` | Reads from `IConfiguration` | `app.UseConfigurationManagerShim()` |
| **SessionShim** | `Session["key"]` indexer, `.Get<T>()`, `.Remove()`, `.Clear()`, `.ContainsKey()` | In-memory per-circuit + optional `ISession` sync | Auto-registered by `AddBlazorWebFormsComponents()` |
| **ServerShim** | `Server.MapPath()`, `Server.HtmlEncode()`, `Server.HtmlDecode()`, `Server.UrlEncode()`, `Server.UrlDecode()` | Wraps `IWebHostEnvironment` + `WebUtility` | Auto-registered |
| **CacheShim** | `Cache["key"]` indexer, `Cache.Insert()`, `Cache.Get<T>()`, `Cache.Remove()` | Wraps `IMemoryCache` with absolute/sliding expiration | Auto-registered |
| **ResponseShim** | `Response.Redirect()`, `Response.Cookies` | Wraps `NavigationManager` + `HttpContext`; auto-strips `~/` and `.aspx` | Via `WebFormsPageBase.Response` |
| **RequestShim** | `Request.QueryString`, `Request.Cookies`, `Request.Url`, `Request.Form` | Wraps `NavigationManager` + `HttpContext`; Form via `FormShim` | Via `WebFormsPageBase.Request` |
| **FormShim** | `Request.Form["key"]`, `.GetValues()`, `.AllKeys`, `.Count`, `.ContainsKey()` | Wraps `IFormCollection` (SSR) or JS interop data (interactive) | Via `RequestShim.Form` — populated by `<WebFormsForm>` |
| **ClientScriptShim** | `Page.ClientScript.RegisterStartupScript()`, `.RegisterClientScriptBlock()`, `.RegisterClientScriptInclude()`, `.GetPostBackEventReference()` | Queues scripts, flushes via `IJSRuntime` in `OnAfterRenderAsync` | Auto-registered |
| **ScriptManagerShim** | `ScriptManager.GetCurrent(page)`, `.RegisterStartupScript()`, etc. | Delegates to `ClientScriptShim` | Auto-registered |
| **ViewStateDictionary** | `ViewState["key"]` indexer | Per-component in-memory dictionary | Via `WebFormsPageBase.ViewState` |
| **BundleConfig/RouteConfig** | `BundleTable.Bundles.Add()`, `RouteTable.Routes.MapPageRoute()` | No-op stubs | Compile-only — no setup needed |

## WebFormsForm Component (Form POST Migration)

The `<WebFormsForm>` component enables `Request.Form["key"]` access in interactive Blazor Server mode where `HttpContext` and `IFormCollection` are unavailable. It captures form data via JS interop and feeds it to `RequestShim.Form`.

**Before (Web Forms):**
```html
<form runat="server">
    <asp:TextBox ID="txtName" runat="server" />
    <asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />
</form>

// Code-behind:
protected void Submit_Click(object sender, EventArgs e)
{
    var name = Request.Form["txtName"];
}
```

**After (Blazor with BWFC):**
```razor
<WebFormsForm OnSubmit="SetRequestFormData">
    <TextBox @bind-Text="name" />
    <Button Text="Submit" OnClick="Submit_Click" />
</WebFormsForm>

@code {
    private string name;

    private void Submit_Click()
    {
        var formName = Request.Form["txtName"];  // Works via FormShim
    }
}
```

**Key points:**
- `<WebFormsForm>` renders a standard `<form>` element
- In interactive mode, `OnSubmit` captures form data via JS interop and populates `Request.Form`
- Bind `OnSubmit="SetRequestFormData"` to auto-wire form data into `WebFormsPageBase.Request.Form`
- Supports `Method` (Get/Post) and `Action` parameters
- SSR mode uses native `IFormCollection` — no JS interop needed

**When to use `<WebFormsForm>` vs native Blazor forms:**
- Use `<WebFormsForm>` when migrated code-behind accesses `Request.Form["key"]` directly
- Use `<EditForm>` for new Blazor forms with model binding
- Use `<form method="post" action="/endpoint">` for auth operations (see identity migration skill)

## ClientScript Migration (Shim-Based)

`ClientScriptShim` provides a compile-compatible bridge for `Page.ClientScript` patterns. It queues scripts during the component lifecycle and flushes them via `IJSRuntime` after render.

**Before (Web Forms):**
```csharp
Page.ClientScript.RegisterStartupScript(GetType(), "init",
    "alert('Page loaded!');", addScriptTags: true);
```

**After (Blazor — via `WebFormsPageBase.ClientScript`):**
```csharp
// Code-behind compiles unchanged
ClientScript.RegisterStartupScript(GetType(), "init",
    "alert('Page loaded!');", addScriptTags: true);
```

**ScriptManager code-behind also works:**
```csharp
var sm = ScriptManagerShim.GetCurrent(this);
sm.RegisterStartupScript(this, GetType(), "key", "doWork();", true);
```

## PostBack Event Handling

`WebFormsPageBase` provides PostBack compatibility via JS interop. The `__doPostBack()` JavaScript function is auto-bootstrapped and routes events back to the Blazor component.

**Before (Web Forms):**
```csharp
public void RaisePostBackEvent(string eventArgument)
{
    ProcessAction(eventArgument);
}
Page.ClientScript.GetPostBackEventReference(this, "delete:42");
```

**After (Blazor with BWFC):**
```csharp
@inherits WebFormsPageBase

@code {
    protected override void OnInitialized()
    {
        PostBack += OnPostBack;
    }

    private void OnPostBack(object sender, PostBackEventArgs e)
    {
        ProcessAction(e.EventArgument);
    }
}
```

**PostBack API surface on `WebFormsPageBase`:**
- `event EventHandler<PostBackEventArgs> PostBack` — raised when `__doPostBack()` fires
- `ClientScript.GetPostBackEventReference(control, argument)` — returns JS expression string
- `ClientScript.GetPostBackClientHyperlink(control, argument)` — returns `javascript:__doPostBack(...)` URL
- `ClientScript.GetCallbackEventReference(...)` — returns `__bwfc_callback(...)` expression
- `HandlePostBackFromJs(eventTarget, eventArgument)` — `[JSInvokable]` bridge method

**appsettings.json mapping** (from `web.config`):
```json
{
  "AppSettings": {
    "SiteName": "My Store",
    "ItemsPerPage": "20"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;"
  }
}
```

## Component Reference

See **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** for the full translation table of 58 BWFC components across 6 categories.
