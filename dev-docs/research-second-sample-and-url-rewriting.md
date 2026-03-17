# Research: Second Sample Project & ASPX URL Rewriting

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-09
**Requested by:** Jeffrey T. Fritz

---

## Part 1: Second Web Forms Sample Project Candidates

### Problem Statement

WingtipToys is our only migration test bed. It's an e-commerce sample that heavily exercises:
- **Data display:** GridView, ListView, FormView, DetailsView
- **Input:** TextBox, Button, CheckBox, DropDownList, FileUpload, ImageButton
- **Display:** Label, HyperLink, LinkButton, Literal, PlaceHolder
- **Validation:** RequiredFieldValidator, CompareValidator, RegularExpressionValidator, ValidationSummary
- **Login:** Login, Register, ManagePassword (via Identity templates)
- **Model binding:** `SelectMethod`, `ItemType`, bound expressions

**Controls NOT exercised by WingtipToys:**
- TreeView, Menu, SiteMapPath, SiteMapDataSource
- Wizard (standalone — CreateUserWizard is in login, but not `<asp:Wizard>`)
- MultiView/View (standalone)
- Calendar (standalone page usage)
- DataList, DataGrid (legacy controls)
- Repeater (standalone)
- AdRotator
- Panel (structural usage with GroupingText)
- BulletedList (data-bound usage)
- RadioButtonList, CheckBoxList (data-bound list usage)

We need a second sample that stress-tests these gaps.

### Evaluation Criteria

1. **Must be a real, publicly available ASP.NET Web Forms application** (open source, sample, or tutorial app)
2. **Should exercise DIFFERENT controls than WingtipToys** — specifically navigation controls (TreeView, Menu, SiteMapPath), wizard flows, calendar, and list controls (DataList, Repeater, RadioButtonList, CheckBoxList)
3. **Should be small-to-medium complexity** — comparable to WingtipToys (~15 pages)
4. **Must target .NET Framework 4.x** (the migration toolkit targets 4.x Web Forms)
5. **Bonus: Has a database or data layer** (tests real data binding scenarios)

### Market Reality

After extensive searching across GitHub, Microsoft samples, CodeProject, and community repositories, **the landscape for comprehensive open-source Web Forms sample applications is extremely thin.** Key findings:

- Microsoft's only maintained Web Forms tutorial sample is **WingtipToys itself**
- The old Visual Studio "Starter Kits" (Personal Website, Club Web Site, Small Business) are no longer publicly available
- Contoso University, Music Store, and most other well-known Microsoft samples are **MVC or ASP.NET Core only** — no Web Forms versions exist
- Community repos on GitHub (under the `asp-net-web-forms` topic) are mostly small demos, homework projects, or single-control samples — not integrated applications
- Third-party samples (Telerik, DevExpress, Syncfusion) use vendor-specific controls, not standard ASP.NET Web Forms controls

### Candidates

#### 1. PallaviKatari/ASP.NET-WEBFORMS-CONCEPTS (GitHub)

- **Name and source URL:** ASP.NET-WEBFORMS-CONCEPTS — https://github.com/PallaviKatari/ASP.NET-WEBFORMS-CONCEPTS
- **Framework version:** .NET Framework 4.x (Visual Studio project)
- **Controls used:** MultiView, Calendar, TreeView, DataList, Repeater, RadioButtonList, CheckBoxList, BulletedList, AdRotator, Panel, Wizard (individual .aspx demo pages for each control)
- **Size:** ~30+ individual .aspx pages, each demonstrating one or two controls. Not an integrated application — more of a control showcase.
- **Data layer:** Inline data / XML files. No database.
- **Why it's a good fit:** Directly covers nearly every control WingtipToys misses. Each page is a self-contained demo with markup and code-behind. Could be adapted into a cohesive sample application.
- **Risks:**
  - Not an integrated app — it's a collection of isolated demos, not a real migration scenario
  - No navigation flow between pages (no master page, no site map)
  - No database layer — doesn't test real data binding
  - License unclear (no LICENSE file in repo)
  - Would require significant rework to become a migration test bed

#### 2. Purpose-Built "BWFC Control Gallery" (New Project)

- **Name and source URL:** Would be created from scratch under `samples/ControlGallery/` in this repo
- **Framework version:** .NET Framework 4.8 (matching our target)
- **Controls used:** TreeView + SiteMapDataSource (navigation), Menu + SiteMapPath (breadcrumbs), Wizard (multi-step form), Calendar (event scheduling), DataList (card layout), Repeater (custom templates), RadioButtonList + CheckBoxList (survey/filter), BulletedList (data-bound lists), AdRotator (banner rotation), Panel with GroupingText (fieldsets), MultiView/View (tabbed content)
- **Size:** Target ~12-15 pages with a shared master page and Web.sitemap
- **Data layer:** SQL Server LocalDB with Entity Framework — a simple domain like "Event Management" or "Employee Directory" that naturally uses Calendar, TreeView (department hierarchy), Wizard (event registration), DataList (event cards)
- **Why it's a good fit:**
  - Purpose-built to exercise exactly the controls WingtipToys doesn't cover
  - We control the complexity and can ensure every BWFC component gap is tested
  - Uses a real database for authentic data-binding scenarios
  - Follows the same project structure patterns as WingtipToys for consistency
  - Lives in our repo — no license or dependency concerns
- **Risks:**
  - Requires development effort (estimated 2-3 days for Cyclops + Jubilee)
  - Needs design spec and review before building
  - Not a "real world" app that someone would actually migrate — it's a test harness

#### 3. Microsoft AspNetDocs Embedded Samples (Assembled)

- **Name and source URL:** Assembled from https://github.com/dotnet/AspNetDocs/tree/main/aspnet/web-forms/
- **Framework version:** .NET Framework 4.5+
- **Controls used:** Various — the AspNetDocs repo contains code snippets and partial samples for navigation controls, data controls, and security scenarios across the tutorial series
- **Size:** Hundreds of code snippets across dozens of markdown files. Would need extraction and assembly.
- **Data layer:** Various — some samples use SQL Server, some use inline data
- **Why it's a good fit:** Official Microsoft code, MIT-licensed, covers breadth of Web Forms features
- **Risks:**
  - Not a runnable application — scattered code fragments embedded in documentation markdown
  - Assembly effort would be very high (parsing markdown, extracting code blocks, creating project structure)
  - Many snippets are incomplete or context-dependent
  - Essentially building from scratch with reference material

#### 4. Tour Management Application (GitHub Community)

- **Name and source URL:** Tour Management ASP.NET — found under `asp-net-web-forms` GitHub topic (various forks, e.g., jaygajera17/Tour_Management_Asp.Net)
- **Framework version:** .NET Framework 4.x
- **Controls used:** GridView, TextBox, Button, Label, DropDownList, Image — predominantly data entry/display controls
- **Size:** Small (~8-10 pages), tour booking CRUD
- **Data layer:** SQL Server
- **Why it's a good fit:** Real application with database, authentication, and CRUD operations
- **Risks:**
  - **Overlaps heavily with WingtipToys** — same control set (GridView, TextBox, Button, Label)
  - Doesn't exercise the navigation/wizard/calendar controls we need
  - Community project with unknown code quality
  - Doesn't fill the control coverage gaps

#### 5. Custom "Northwind Explorer" (Adapted from Northwind DB)

- **Name and source URL:** Would be created using the classic Northwind sample database schema
- **Framework version:** .NET Framework 4.8
- **Controls used:** TreeView (product category hierarchy), Menu + SiteMapPath (navigation), DataList (product cards), Repeater (order line items), Calendar (order date picker), DetailsView (single record view), RadioButtonList (filters), CheckBoxList (multi-select filters), BulletedList (related items), Panel with GroupingText
- **Size:** ~10-12 pages
- **Data layer:** SQL Server LocalDB with Northwind schema (well-known, freely available)
- **Why it's a good fit:**
  - Northwind is the most recognized .NET sample database — instantly familiar to Web Forms developers
  - The domain (products, categories, orders, employees) naturally maps to hierarchical controls (TreeView for categories), calendar controls (order dates), and list controls
  - Database schema is pre-built and well-documented
  - Signals to the migration community that BWFC handles real enterprise data
- **Risks:**
  - Still requires building the Web Forms frontend from scratch
  - Northwind schema is large — need to scope to a subset
  - Same "test harness" concern as Candidate 2

### Recommendation

**Top Pick: Candidate 2 — Purpose-Built "BWFC Control Gallery" with an Event Management domain**

**Runner-up: Candidate 5 — Northwind Explorer** (if we want name recognition over domain fit)

**Reasoning:**

1. **The market has spoken** — there is no existing open-source Web Forms sample app that exercises the controls we need. Every candidate either overlaps with WingtipToys or requires building from scratch anyway. Accepting this reality means we should build exactly what we need rather than force-fitting an existing project.

2. **Event Management domain is optimal** because:
   - **Calendar** is a natural fit (event dates, scheduling)
   - **TreeView** maps to event categories or venue hierarchy
   - **Menu + SiteMapPath** for site navigation (Events, Venues, Attendees, Reports)
   - **Wizard** for multi-step event registration (personal info → event selection → payment → confirmation)
   - **DataList** for event card layout (image + title + date + description)
   - **Repeater** for attendee lists with custom templates
   - **RadioButtonList/CheckBoxList** for event type filters and preference selection
   - **BulletedList** for event features/amenities
   - **AdRotator** for sponsor banners
   - **Panel with GroupingText** for form sections

3. **Controlled complexity** — we design it to be ~12-15 pages, matching WingtipToys scale, with a LocalDB database and Entity Framework Code First.

4. **Zero license risk** — it's our code, in our repo, MIT-licensed like everything else.

5. **Migration pipeline validation** — building the Web Forms version first, then running our migration toolkit against it, validates the entire BWFC pipeline for the control families that WingtipToys doesn't cover.

**Suggested name:** `EventManager` (parallel to `WingtipToys`)
**Suggested location:** `samples/EventManager/` (Web Forms original) + `samples/AfterEventManager/` (Blazor migrated)

**Next step:** Forge to write a design spec for Cyclops (build) + Jubilee (samples) + Beast (docs) + Rogue (tests).

---

## Part 2: ASPX URL Rewriting for Migration

### Problem Statement

When migrating a Web Forms application to Blazor, URLs change:

| Web Forms URL | Blazor URL |
|---|---|
| `/Products.aspx` | `/Products` |
| `/Account/Login.aspx` | `/Account/Login` |
| `/Admin/AdminPage.aspx?id=5` | `/Admin/AdminPage?id=5` |
| `/Catalog/Products.aspx?cat=shoes&page=2` | `/Catalog/Products?cat=shoes&page=2` |

Existing bookmarks, search engine indexes, external links, and hardcoded references all point to the `.aspx` URLs. We need a strategy to handle these gracefully.

### Approaches Investigated

#### Approach A: ASP.NET Core URL Rewriting Middleware (`Microsoft.AspNetCore.Rewrite`)

The built-in `Microsoft.AspNetCore.Rewrite` package provides regex-based URL rewriting and redirection.

**Transparent Rewrite (URL stays as `.aspx` in browser):**
```csharp
using Microsoft.AspNetCore.Rewrite;

var rewriteOptions = new RewriteOptions()
    .AddRewrite(@"^(.+)\.aspx$", "$1", skipRemainingRules: true);

app.UseRewriter(rewriteOptions);
// Must be placed BEFORE app.UseRouting()
```

**301 Redirect (browser URL changes to clean URL):**
```csharp
var rewriteOptions = new RewriteOptions()
    .AddRedirect(@"^(.+)\.aspx$", "$1", statusCode: 301);

app.UseRewriter(rewriteOptions);
```

**Pros:**
- Built into ASP.NET Core — no additional packages needed (ships with the framework)
- Regex-based — handles any `.aspx` URL pattern in a single rule
- Query strings are preserved automatically (they're not part of the path the regex matches)
- Well-documented by Microsoft
- Supports both rewrite (transparent) and redirect (SEO) modes

**Cons:**
- Regex operates on URL path only — works for our use case but limited for complex transformations
- Rewrite mode doesn't update the browser's URL bar — could confuse users if they copy the URL
- Must be placed before `UseRouting()` in the middleware pipeline

#### Approach B: Custom Middleware (`app.Use()`)

A lightweight inline middleware that catches `.aspx` requests:

```csharp
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path != null && path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
    {
        var newPath = path[..^5]; // Strip ".aspx"
        var queryString = context.Request.QueryString.Value;

        context.Response.StatusCode = 301;
        context.Response.Headers.Location = newPath + queryString;
        return; // Short-circuit — don't call next()
    }
    await next();
});
```

**Pros:**
- Zero dependencies — pure middleware, no NuGet package needed
- Full control over redirect logic (can add logging, conditional behavior, URL mapping tables)
- Easy to extend for complex cases (e.g., `/Default.aspx` → `/`, page-specific rewrites)
- Query string preservation is explicit and visible

**Cons:**
- More code to maintain than a single regex rule
- No built-in rewrite (transparent) mode — would need to manually rewrite `Request.Path`
- Less discoverable than the standard `RewriteOptions` API

#### Approach C: Custom `IRule` Implementation

A reusable rule class for the RewriteOptions pipeline:

```csharp
public class AspxRewriteRule : IRule
{
    private readonly int _statusCode;
    private readonly bool _redirect;

    public AspxRewriteRule(bool redirect = true, int statusCode = 301)
    {
        _redirect = redirect;
        _statusCode = statusCode;
    }

    public void ApplyRule(RewriteContext context)
    {
        var request = context.HttpContext.Request;
        var path = request.Path.Value;

        if (path == null || !path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = RuleResult.ContinueRules;
            return;
        }

        var newPath = path[..^5]; // Strip ".aspx"
        if (string.IsNullOrEmpty(newPath)) newPath = "/";

        if (_redirect)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = _statusCode;
            response.Headers.Location = newPath + request.QueryString.Value;
            context.Result = RuleResult.EndResponse;
        }
        else
        {
            request.Path = newPath;
            context.Result = RuleResult.SkipRemainingRules;
        }
    }
}

// Usage:
var rewriteOptions = new RewriteOptions()
    .Add(new AspxRewriteRule(redirect: true, statusCode: 301));
app.UseRewriter(rewriteOptions);
```

**Pros:**
- Plugs into the standard `RewriteOptions` pipeline — composable with other rules
- Supports both redirect and transparent rewrite modes
- Testable as a standalone class
- Can be shipped as part of a NuGet package

**Cons:**
- More code than Approach A's one-liner regex
- Requires understanding the `IRule` interface

#### Approach D: Blazor `@page` Directive with `.aspx` Patterns

Blazor components can declare multiple `@page` directives, including ones with `.aspx`:

```razor
@page "/Products"
@page "/Products.aspx"
```

**Pros:**
- No middleware needed — works purely at the Blazor routing level
- Each page explicitly declares its legacy URL
- Route parameters work: `@page "/ProductDetails.aspx/{Id:int}"`

**Cons:**
- **Does not scale** — every migrated page needs a duplicate `@page` directive
- Does not handle query strings differently (they work, but the `.aspx` stays in the URL)
- No 301 redirect — search engines see two URLs for the same content (duplicate content penalty)
- Violates DRY — migration toolkit would need to auto-generate these
- Not removable in a single place when legacy support is no longer needed

#### Approach E: Catch-All Fallback Route

A single Blazor component that catches all `.aspx` requests:

```razor
@page "/{*path}"
@inject NavigationManager Nav

@code {
    [Parameter] public string? Path { get; set; }

    protected override void OnInitialized()
    {
        if (Path != null && Path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            var newPath = Path[..^5];
            Nav.NavigateTo("/" + newPath + Nav.ToAbsoluteUri(Nav.Uri).Query, replace: true);
        }
    }
}
```

**Cons:**
- Catch-all routes in Blazor are greedy — can interfere with other routing
- Client-side redirect, not server-side 301 — no SEO benefit
- Extra round-trip (page loads, then redirects)
- Not recommended

### 301 Redirect vs. Transparent Rewrite — SEO Analysis

| Factor | 301 Redirect | Transparent Rewrite |
|---|---|---|
| **Browser URL** | Changes to clean URL | Stays as `.aspx` |
| **SEO** | ✅ Search engines update index to new URL | ⚠️ Two URLs may exist for same content |
| **Bookmarks** | Update on next visit | Stay as `.aspx` forever |
| **Performance** | Extra HTTP round-trip | No extra round-trip |
| **Migration phase** | Best for: after migration is complete | Best for: during migration (testing) |
| **Recommendation** | ✅ **Use this for production** | Use temporarily during development |

**Verdict:** Ship with **301 redirect** as the default. Provide an option for transparent rewrite during the migration development phase.

### Existing Solutions

No existing NuGet package or library specifically targets ASPX-to-Blazor URL rewriting. The closest solutions are:
- `Microsoft.AspNetCore.Rewrite` — general-purpose URL rewriting (our recommended foundation)
- YARP (Yet Another Reverse Proxy) — useful for incremental migration but overkill for URL stripping
- Various blog posts describe one-off regex rules, but no reusable library exists

### Recommended Approach

**Ship a documented extension method in the migration-toolkit**, not as a NuGet-installable middleware in the BWFC library itself.

**Rationale:**
1. This is a **migration concern**, not a runtime component concern — it belongs in the migration toolkit
2. It's ~20 lines of code — doesn't warrant its own NuGet package
3. Developers should understand what it does and remove it once migration is complete
4. The BWFC NuGet package should remain focused on Blazor components, not middleware

#### Recommended Code

Add to `migration-toolkit/scripts/` or as a documented snippet in `migration-toolkit/METHODOLOGY.md`:

```csharp
// ============================================================
// ASPX URL Redirect Middleware for Web Forms → Blazor Migration
// Add to Program.cs BEFORE app.UseRouting()
// Remove once all legacy URLs have been updated.
// ============================================================

using Microsoft.AspNetCore.Rewrite;

// Option 1: Simple one-liner (recommended for most migrations)
var rewriteOptions = new RewriteOptions()
    .AddRedirect(@"^(.+)\.aspx$", "$1", statusCode: 301);
app.UseRewriter(rewriteOptions);
app.UseRouting();

// Option 2: Handle Default.aspx → / (root page redirect)
var rewriteOptions = new RewriteOptions()
    .AddRedirect(@"^Default\.aspx$", "/", statusCode: 301)
    .AddRedirect(@"^(.+)\.aspx$", "$1", statusCode: 301);
app.UseRewriter(rewriteOptions);
app.UseRouting();
```

**Query string handling:** Query strings are automatically preserved because `AddRedirect` only matches against the URL path, not the query string. A request to `/Products.aspx?cat=shoes&page=2` redirects to `/Products?cat=shoes&page=2` with no additional configuration.

**Case sensitivity:** The regex is case-insensitive by default in `RewriteOptions`. Both `/Products.ASPX` and `/products.aspx` are handled.

**Default.aspx special case:** Many Web Forms apps use `Default.aspx` as their home page. The recommended approach adds an explicit rule for this before the general rule, redirecting to `/` (the root).

### Implementation Plan

| Item | Location | Action |
|---|---|---|
| **Code snippet** | `migration-toolkit/METHODOLOGY.md` | Add "URL Preservation" section with the recommended `RewriteOptions` code |
| **Checklist item** | `migration-toolkit/CHECKLIST.md` | Add checkbox: "Add ASPX URL redirect middleware to Program.cs" |
| **Migration script** | `migration-toolkit/scripts/bwfc-migrate-layer2.ps1` | Consider auto-injecting the rewrite code into `Program.cs` as a Layer 2 transform |
| **Documentation** | `docs/Migration/UrlRewriting.md` | Full guide with all approaches, trade-offs, and examples |
| **Migration skill** | `migration-toolkit/skills/migration-standards/SKILL.md` | Update with URL preservation guidance |

**Priority:** P2 — not blocking any current migration runs, but should be addressed before the migration toolkit is promoted as "production ready."

**Not recommended:** Adding this to the BWFC NuGet library itself. URL rewriting is infrastructure middleware, not a Blazor component. Mixing concerns would confuse the package's purpose.

---

## Summary

| Research Item | Recommendation | Priority |
|---|---|---|
| Second sample project | Build "EventManager" — purpose-built Control Gallery with Event Management domain | P2 (after migration toolkit stabilization) |
| ASPX URL rewriting | Document `RewriteOptions.AddRedirect` in migration-toolkit; don't ship as NuGet | P2 |
