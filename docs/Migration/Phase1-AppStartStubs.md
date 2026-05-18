# App_Start Compilation Stubs

The `BundleConfig` and `RouteConfig` stubs allow your migrated `App_Start/` directory to compile without modification. These are **no-op shims** that do nothing at runtime — they exist only to make Web Forms configuration files compile as-is during Phase 1 migrations.

## Overview

**What they are:**
- No-op (no-operation) stub classes in `System.Web.Optimization` and `System.Web.Routing` namespaces
- Emulate the Web Forms bundle and routing APIs just enough to compile
- Located in the BlazorWebFormsComponents library and auto-imported via global usings

**Why they matter:**
When you migrate a Web Forms application, your `App_Start/BundleConfig.cs` and `App_Start/RouteConfig.cs` files contain code like:

```csharp
BundleTable.Bundles.Add(new ScriptBundle("~/bundles/jquery")...);
RouteTable.Routes.MapPageRoute(...);
```

Without stubs, this code fails to compile. The stubs allow your `App_Start/` directory to compile unchanged in Phase 1, giving you time to plan the Blazor-native alternatives in Phase 2+.

## Before and After

=== "Web Forms (Original App_Start)"
    ```csharp
    // App_Start/BundleConfig.cs
    using System.Web.Optimization;
    
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js"));
            
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/site.css"));
        }
    }
    
    // App_Start/RouteConfig.cs
    using System.Web.Routing;
    
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapPageRoute("", "{controller}/{action}/{id}", "~/Pages/{controller}/{action}.aspx");
        }
    }
    
    // Global.asax
    protected void Application_Start()
    {
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
    }
    ```

=== "Blazor with BWFC Stubs (No Changes Needed)"
    ```csharp
    // App_Start/BundleConfig.cs — compiles unchanged!
    using System.Web.Optimization;
    
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js"));
            
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/site.css"));
        }
    }
    
    // App_Start/RouteConfig.cs — compiles unchanged!
    using System.Web.Routing;
    
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapPageRoute("", "{controller}/{action}/{id}", 
                "~/Pages/{controller}/{action}.aspx");
        }
    }
    
    // Program.cs (Blazor)
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddBlazorWebFormsComponents();
    
    var app = builder.Build();
    app.UseStaticFiles();
    app.UseRouting();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    app.Run();
    ```

**Key difference:** The `App_Start/` classes compile but do nothing. Blazor handles bundling and routing completely differently (see below).

## The Stubs

The BWFC library provides minimal implementations of these classes:

```csharp
namespace System.Web.Optimization
{
    public class Bundle
    {
        public Bundle(string virtualPath) { }
        public Bundle Include(params string[] virtualPaths) => this;
    }
    
    public class ScriptBundle : Bundle { }
    public class StyleBundle : Bundle { }
    
    public static class BundleTable
    {
        public static BundleCollection Bundles { get; } = new();
    }
    
    public class BundleCollection
    {
        public void Add(Bundle bundle) { }
    }
}

namespace System.Web.Routing
{
    public class RouteCollection
    {
        public void MapPageRoute(string routeName, string routeUrl, 
            string physicalFile) { }
        public void Ignore(string url) { }
        public void Ignore(string url, object constraints) { }
    }
    
    public static class RouteTable
    {
        public static RouteCollection Routes { get; } = new();
    }
}
```

These stub classes do nothing — they exist only to satisfy the compiler.

## ⚠️ Important: These Stubs Are No-Ops

The stubs allow your code to **compile**, but they have **zero runtime effect**:

| Web Forms Feature | What Happens | Blazor Alternative |
|------------------|-------------|---|
| `BundleConfig.RegisterBundles()` | Called but does nothing | CSS/JS Isolation (see below) |
| Bundle minification | Disabled — stubs ignore it | Build tool or Vite/esbuild |
| `RouteTable.Routes.MapPageRoute()` | Called but does nothing | `@page` directives in Blazor |
| Route ignores (`{resource}.axd`) | Ignored | Not needed in Blazor |

In other words, **if your Web Forms app relied on bundling or routing, you must implement Blazor alternatives.** The stubs just prevent compilation errors.

## Blazor Alternatives

### CSS and JavaScript Bundling

Instead of `BundleConfig`, Blazor uses **CSS/JS Isolation**:

=== "Web Forms (BundleConfig)"
    ```csharp
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css", "~/Content/site.css"));
        }
    }
    
    // In .aspx page:
    // <link href="~/Content/css" rel="stylesheet" />
    ```

=== "Blazor (CSS Isolation)"
    ```razor
    <!-- Components/Layout/MainLayout.razor -->
    @inherits LayoutComponentBase
    
    <link href="bootstrap.css" rel="stylesheet" />
    <link href="site.css" rel="stylesheet" />
    
    @Body
    ```

Or, for component-scoped styles:

```razor
<!-- MyComponent.razor -->
<button class="btn">Click me</button>

<!-- MyComponent.razor.css (automatically scoped to component) -->
button.btn {
    background-color: blue;
}
```

For **JavaScript bundling**, use standard npm/webpack tooling:

```bash
npm install webpack webpack-cli --save-dev
npm run build  # Bundles and minifies JS
```

### Routing

Instead of `RouteConfig.MapPageRoute()`, Blazor uses **`@page` directives**:

=== "Web Forms (RouteConfig)"
    ```csharp
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("product_detail", 
                "products/{id}", 
                "~/Pages/ProductDetail.aspx");
        }
    }
    
    <!-- ProductDetail.aspx accessed via /products/123 -->
    ```

=== "Blazor (@page Directives)"
    ```razor
    <!-- Pages/ProductDetail.razor -->
    @page "/products/{id}"
    
    @code {
        [Parameter]
        public string Id { get; set; }
        
        protected override void OnInitialized()
        {
            // Load product with ID
        }
    }
    ```

Blazor's routing is declarative (`@page`) rather than centralized. This is simpler and more composable.

## Phase 1 → Phase 2+ Migration

Here's a suggested timeline:

**Phase 1 (Now):** 
- Keep `App_Start/BundleConfig.cs` and `RouteConfig.cs` unchanged
- They compile with BWFC stubs
- Focus on UI migration

**Phase 2 (After initial migration):**
1. Delete `App_Start/BundleConfig.cs` (no longer used)
2. Add CSS/JS imports to `Shared/MainLayout.razor`
3. Add `@page` directives to your Blazor components
4. Delete `App_Start/RouteConfig.cs`

Example transition:

```csharp
// Phase 1: Stub-based (works but ignored)
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(new StyleBundle("~/Content/css")
            .Include("~/Content/bootstrap.css", "~/Content/site.css"));
    }
}

// Phase 2: Migrate to Blazor alternatives
// Delete BundleConfig.cs, add to MainLayout.razor:
<link href="bootstrap.css" rel="stylesheet" />
<link href="site.css" rel="stylesheet" />
```

## Troubleshooting

### "BundleConfig not found" during compilation

Ensure the `BlazorWebFormsComponents` NuGet package is installed and `AddBlazorWebFormsComponents()` is called in `Program.cs`:

```bash
dotnet add package Fritz.BlazorWebFormsComponents
```

```csharp
// Program.cs
builder.Services.AddBlazorWebFormsComponents();
```

Global usings automatically import the stubs into every file.

### Bundling or routing not working at runtime

This is expected — **the stubs do nothing**. You must implement Blazor alternatives:

- For CSS/JS: Use Blazor CSS/JS Isolation or standard build tools
- For routing: Use `@page` directives instead of `RouteConfig`

## Summary

- ✅ `BundleConfig` and `RouteConfig` files compile with BWFC stubs
- ✅ No code changes needed in Phase 1
- ❌ Stubs do nothing at runtime — bundling and routing are disabled
- 🔄 Plan Phase 2 migration to Blazor alternatives (CSS Isolation, `@page` directives)

See the following for implementation details:

- [CSS Isolation in Blazor](https://learn.microsoft.com/aspnet/core/blazor/components/css-isolation)
- [Blazor Routing](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/routing)
- [JavaScript Interoperability](../UtilityFeatures/JavaScriptSetup.md)
