---
name: l3-performance-optimization
description: "Post-migration async/await and .NET 10 performance optimization pass for Blazor apps migrated from Web Forms. Applies modern runtime patterns after the app builds and runs. WHEN: \"run L3 optimization\", \"apply async/await fixes\", \"optimize migrated Blazor app\", \"AsNoTracking queries\", \"StreamRendering\", \"IDbContextFactory pattern\", \"what .NET 10 optimizations can we apply\", \"generate L3 report\"."
---

# L3: Post-Migration Performance Optimization

This skill is the **optional fourth step** in the migration pipeline. It applies after the app builds and runs correctly following L1 (automated), L2 (structural), and L3-architecture (data/identity) passes.

**Related skills:**
- `/bwfc-migration` — Core markup migration (controls, expressions, layouts)
- `/bwfc-data-migration` — EF6 → EF Core, architecture decisions
- `/bwfc-identity-migration` — Authentication migration

---

## When to Use This Skill

Apply L3 optimizations when:
- The migrated app **builds and runs without errors**
- You want measurable performance improvements beyond functional correctness
- You are ready to modernize patterns to .NET 10 idioms

> **Do NOT run L3 before the app is functional.** L3 assumes L1 + L2 + L3-architecture are complete. Applying async patterns to broken code makes debugging harder.

---

## Confidence Levels

Each optimization in this skill is rated:

| Rating | Meaning |
|--------|---------|
| ✅ **Safe** | Drop-in change, identical behavior, no review needed |
| ⚠️ **Review** | Almost always correct, but verify behavior in your specific context |
| 🔴 **Risky** | Possible semantic difference — review before committing |

---

## Usage Patterns

```
# Apply all safe optimizations to the whole project
"Run L3 optimization on the migrated ContosoUniversity app"

# Apply a single category
"Apply async/await fixes to Students.razor.cs"

# Report only — no changes
"Generate an L3 optimization report for AfterWingtipToys"

# Ask what's applicable
"What .NET 10 optimizations can we apply to this file?"
```

---

## 1. Async/Await Modernization ✅ Safe

The most impactful change in the L3 pass. Synchronous EF Core calls block the thread pool, hurting throughput under load. Web Forms code-behind often used synchronous DB calls; migrated code preserves that pattern by default.

### 1a. `OnInitialized` → `OnInitializedAsync`

Applies when `OnInitialized` contains database queries or other I/O.

**Before (AfterContosoUniversity pattern):**
```csharp
protected override void OnInitialized()
{
    _students = studLogic.GetJoinedTableData();
    _courseNames = studLogic.GetCourseNames();
}
```

**After:**
```csharp
protected override async Task OnInitializedAsync()
{
    _students = await studLogic.GetJoinedTableDataAsync();
    _courseNames = await studLogic.GetCourseNamesAsync();
}
```

> **Why:** `OnInitializedAsync` is awaited by Blazor's rendering pipeline. Synchronous blocking inside `OnInitialized` ties up the thread for the duration of the database call, while `await` yields the thread back to the pool during I/O.

### 1b. EF Core Sync → Async

Replace synchronous EF Core terminal operators with their async counterparts.

| Before | After | Rating |
|--------|-------|--------|
| `db.Products.ToList()` | `await db.Products.ToListAsync()` | ✅ Safe |
| `db.Products.FirstOrDefault(...)` | `await db.Products.FirstOrDefaultAsync(...)` | ✅ Safe |
| `db.Products.SingleOrDefault(...)` | `await db.Products.SingleOrDefaultAsync(...)` | ✅ Safe |
| `db.Products.Find(id)` | `await db.Products.FindAsync(id)` | ✅ Safe |
| `db.SaveChanges()` | `await db.SaveChangesAsync()` | ✅ Safe |
| `db.Products.Count()` | `await db.Products.CountAsync()` | ✅ Safe |
| `db.Products.Any(...)` | `await db.Products.AnyAsync(...)` | ✅ Safe |

**Before:**
```csharp
private void LoadProducts()
{
    using var db = DbFactory.CreateDbContext();
    _products = db.Products.Where(p => p.CategoryID == _catId).ToList();
}
```

**After:**
```csharp
private async Task LoadProductsAsync()
{
    using var db = DbFactory.CreateDbContext();
    _products = await db.Products.Where(p => p.CategoryID == _catId).ToListAsync();
}
```

> Call sites must also become `async Task` and use `await`. Update event handlers and lifecycle methods accordingly.

### 1c. `Task.Result` / `Task.Wait()` Anti-Patterns → `await`

These patterns deadlock under Blazor's synchronization context.

**Before:**
```csharp
var result = SomeAsyncMethod().Result;      // ❌ deadlock risk
SomeAsyncMethod().Wait();                   // ❌ deadlock risk
```

**After:**
```csharp
var result = await SomeAsyncMethod();       // ✅
await SomeAsyncMethod();                    // ✅
```

> **Rating:** ✅ Safe — `await` is semantically identical for Blazor components where there is no `SynchronizationContext` deadlock concern, but removing `.Result`/`.Wait()` is always correct.

### 1d. Event Handlers That Perform I/O → `async Task`

**Before:**
```csharp
private void btnInsert_Click()
{
    studLogic.InsertNewEntry(_firstName, _lastName, birth, _selectedCourse, _email);
    _students = studLogic.GetJoinedTableData();
}
```

**After:**
```csharp
private async Task btnInsert_Click()
{
    await studLogic.InsertNewEntryAsync(_firstName, _lastName, birth, _selectedCourse, _email);
    _students = await studLogic.GetJoinedTableDataAsync();
}
```

> **Note:** Blazor's `EventCallback` supports `async Task` handlers natively. The framework awaits them and triggers re-render automatically.

---

## 2. EF Core Query Optimization

### 2a. Add `AsNoTracking()` to Read-Only Queries ✅ Safe

EF Core tracks every entity it loads by default. For read-only list pages and reports, tracking adds CPU and memory overhead with no benefit.

**Before:**
```csharp
using var db = DbFactory.CreateDbContext();
_products = await db.Products.ToListAsync();
```

**After:**
```csharp
using var db = DbFactory.CreateDbContext();
_products = await db.Products.AsNoTracking().ToListAsync();
```

**When to apply:**
- ✅ List/read pages where you never call `db.SaveChanges()` on the loaded entities
- ✅ `OnInitializedAsync` data loads displayed in grids or detail views
- ❌ Do NOT add `AsNoTracking()` when you load an entity and then modify + save it in the same `DbContext` instance

### 2b. Replace String-Based `Include()` with Lambda `Include()` ✅ Safe

EF6 used string-based includes. EF Core supports both, but lambda includes are refactor-safe and get compile-time checking.

**Before (EF6 migration artifact):**
```csharp
db.Products.Include("Category").ToList()
```

**After:**
```csharp
await db.Products.Include(p => p.Category).ToListAsync()
```

### 2c. `AsSplitQuery()` for Multi-Collection Includes ⚠️ Review

When loading an entity with multiple collection navigations, EF Core generates a cartesian JOIN that multiplies result rows. Split queries issue separate SQL statements instead.

**Before:**
```csharp
var instructors = await db.Instructors
    .Include(i => i.Courses)
    .Include(i => i.OfficeAssignment)
    .ToListAsync();
```

**After:**
```csharp
var instructors = await db.Instructors
    .Include(i => i.Courses)
    .Include(i => i.OfficeAssignment)
    .AsSplitQuery()
    .ToListAsync();
```

> **⚠️ Review:** Split queries use multiple round-trips. They are faster when the cartesian product is large, but slower if network latency dominates. Measure before committing. See [EF Core split queries docs](https://learn.microsoft.com/ef/core/querying/single-split-queries).

### 2d. Identify N+1 Query Patterns ⚠️ Review

N+1 occurs when code lazily loads a navigation property inside a loop.

**Before (N+1):**
```csharp
// Loads N students, then issues 1 additional query per student to load Enrollments
foreach (var student in _students)
{
    var count = student.Enrollments.Count; // lazy load per student
}
```

**After:**
```csharp
// Single query with eager loading
_students = await db.Students
    .Include(s => s.Enrollments)
    .AsNoTracking()
    .ToListAsync();
```

> **How to find N+1:** Enable EF Core logging (`optionsBuilder.LogTo(Console.WriteLine)`) during development and look for repeated identical queries with only the ID parameter changing.

---

## 3. .NET 10 Runtime Patterns

### 3a. `[SupplyParameterFromQuery]` for Route/Query Parameters ✅ Safe

Replaces manual `NavigationManager.Uri` parsing for query string values.

**Before:**
```csharp
[Inject] private NavigationManager Navigation { get; set; } = default!;

protected override void OnInitialized()
{
    var uri = new Uri(Navigation.Uri);
    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
    _productAction = query["ProductAction"];
}
```

**After:**
```csharp
[SupplyParameterFromQuery(Name = "ProductAction")]
public string? ProductAction { get; set; }
```

> **Note:** `[SupplyParameterFromQuery]` only works on routable components (those with `@page`). For non-routable child components, continue using `[Parameter]` passed from the parent.

### 3b. `[StreamRendering]` for Pages with Async Data Loading ⚠️ Review

`[StreamRendering]` lets Blazor send the initial HTML to the browser immediately, then stream in the loaded content. This reduces time-to-first-byte for data-heavy pages.

**Before:**
```razor
@page "/Students"

@code {
    protected override async Task OnInitializedAsync()
    {
        _students = await studLogic.GetJoinedTableDataAsync();
    }
}
```

**After:**
```razor
@page "/Students"
@attribute [StreamRendering]

@code {
    protected override async Task OnInitializedAsync()
    {
        _students = await studLogic.GetJoinedTableDataAsync();
    }
}
```

Add a loading placeholder in the markup:

```razor
@if (_students is null)
{
    <p>Loading...</p>
}
else
{
    <GridView Items="@_students" ... />
}
```

> **⚠️ Review:** `[StreamRendering]` requires the component to be in static SSR mode or to handle the null/loading state correctly. It works best with `InteractiveServer` global mode when the page has a clear loading placeholder. Verify that your layout does not depend on data being ready during the initial render.

### 3c. Limit `@rendermode InteractiveServer` to Pages That Need It ⚠️ Review

The current migration standard sets `InteractiveServer` globally in `App.razor`. This is correct for apps heavy on interactivity. For apps where most pages are read-only displays, you can reduce server resource usage by using static SSR for read-only pages.

**Global (current standard — correct for most migrated apps):**
```razor
@* App.razor *@
<Routes @rendermode="InteractiveServer" />
```

**Per-page opt-in (only for apps where most pages are non-interactive):**
```razor
@* InteractivePages/Edit.razor *@
@rendermode InteractiveServer
```

> **⚠️ Review:** Changing from global interactive to per-page requires auditing every page for event handlers and two-way bindings. Pages without `@rendermode` become static SSR and will not process Blazor events. Only consider this if you have a clear majority of truly static display pages. Most migrated Web Forms apps have enough interactivity that global `InteractiveServer` is the right choice.

### 3d. String Interpolation Over Concatenation ✅ Safe

**Before:**
```csharp
var message = "Student " + firstName + " " + lastName + " enrolled on " + date.ToString("d");
```

**After:**
```csharp
var message = $"Student {firstName} {lastName} enrolled on {date:d}";
```

---

## 4. Blazor Component Optimization

### 4a. `@key` on `@foreach` Loops Rendering Components ✅ Safe

Without `@key`, Blazor diffs lists by position. When items are inserted, removed, or reordered, components at those positions are unnecessarily destroyed and recreated. Adding `@key` lets Blazor track items by identity.

**Before:**
```razor
@foreach (var product in _products)
{
    <ProductCard Product="@product" />
}
```

**After:**
```razor
@foreach (var product in _products)
{
    <ProductCard @key="product.ProductID" Product="@product" />
}
```

> **Use the entity's primary key as the `@key` value.** Avoid using loop index (`i`) as `@key` — it defeats the purpose because the index is positional, not identity-based.

### 4b. `ShouldRender()` for Components with Frequent Parent Re-Renders ⚠️ Review

Components that receive the same data repeatedly (e.g., a static header, a read-only summary panel) still re-render on every parent state change by default.

**Before:**
```csharp
// Component re-renders on every parent state change even when _summary hasn't changed
```

**After:**
```csharp
private CourseSummary? _previousSummary;

protected override bool ShouldRender()
{
    if (_summary == _previousSummary) return false;
    _previousSummary = _summary;
    return true;
}
```

> **⚠️ Review:** `ShouldRender()` overrides can suppress necessary re-renders if state comparison logic is incomplete. Only apply to leaf components with stable, comparable inputs. Do not apply to components that receive `RenderFragment` parameters — those cannot be compared for equality.

### 4c. Extract Heavy Inline `@code` Blocks to Code-Behind ✅ Safe

Inline `@code` blocks longer than ~50 lines hurt maintainability and slow down IDE tooling. Move them to partial class code-behind files.

**Before:**
```razor
@* BigPage.razor *@
@page "/BigPage"
@code {
    // 150+ lines of logic
    [Inject] private IDbContextFactory<AppContext> DbFactory { get; set; } = default!;
    private List<Order> _orders = new();
    // ... more fields ...
    protected override async Task OnInitializedAsync() { ... }
    private async Task SaveOrder() { ... }
    // ... more methods ...
}
```

**After:**

```razor
@* BigPage.razor *@
@page "/BigPage"
@* No @code block — logic lives in BigPage.razor.cs *@
```

```csharp
// BigPage.razor.cs
namespace MyApp.Pages;

public partial class BigPage
{
    [Inject] private IDbContextFactory<AppContext> DbFactory { get; set; } = default!;
    private List<Order> _orders = new();
    // ...
    protected override async Task OnInitializedAsync() { ... }
    private async Task SaveOrder() { ... }
}
```

### 4d. `[EditorRequired]` on Mandatory Parameters ✅ Safe

Prevents accidental omission of required parameters at the call site (build warning, not error).

**Before:**
```csharp
[Parameter]
public Product Product { get; set; } = default!;
```

**After:**
```csharp
[Parameter, EditorRequired]
public Product Product { get; set; } = default!;
```

---

## 5. Dependency Injection Best Practices

### 5a. `@inject DbContext` → `IDbContextFactory<T>` ✅ Safe

Direct `DbContext` injection (`AddDbContext`) uses a scoped lifetime that matches a Blazor circuit, not a request. Long-lived circuits accumulate tracked entities and can serve stale data. `IDbContextFactory` creates short-lived contexts per operation.

**Before:**
```csharp
// Program.cs
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(connectionString));

// Component
[Inject] private ProductContext Db { get; set; } = default!;

private async Task LoadProducts()
{
    _products = await Db.Products.ToListAsync();
}
```

**After:**
```csharp
// Program.cs
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(connectionString));

// Component
[Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

private async Task LoadProducts()
{
    using var db = DbFactory.CreateDbContext();
    _products = await db.Products.AsNoTracking().ToListAsync();
}
```

> The `AfterWingtipToys` sample already uses this pattern correctly. Apply it to any remaining `AddDbContext` registrations in migrated apps.

### 5b. Consistent `[Inject]` Attribute vs `@inject` Directive ✅ Safe

Both work, but mixing styles in the same component is inconsistent. The recommended style for code-behind files is `[Inject]` attribute.

| Location | Recommended |
|----------|-------------|
| `.razor` file (no code-behind) | `@inject IService Service` |
| `.razor.cs` code-behind file | `[Inject] private IService Service { get; set; } = default!;` |

**Before (mixed styles in code-behind):**
```csharp
// AdminPage.razor.cs
[Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
// AdminPage.razor (inline)
@inject NavigationManager NavigationManager
```

**After (all injections in code-behind):**
```csharp
// AdminPage.razor.cs
[Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
[Inject] private NavigationManager NavigationManager { get; set; } = default!;
```

### 5c. Dispose Scoped Services in Components ⚠️ Review

Components that create disposable resources (opened DB connections, HTTP clients, streams) must implement `IAsyncDisposable` or `IDisposable`.

**Before:**
```csharp
public partial class ReportPage
{
    private AppDbContext? _db;

    protected override async Task OnInitializedAsync()
    {
        _db = DbFactory.CreateDbContext();
        _report = await _db.Reports.ToListAsync();
    }
    // ❌ _db never disposed — connection leak
}
```

**After (preferred — per-operation context, no field):**
```csharp
protected override async Task OnInitializedAsync()
{
    using var db = DbFactory.CreateDbContext();
    _report = await db.Reports.AsNoTracking().ToListAsync();
    // db disposed here — no field needed
}
```

**After (if context must live across methods — implement disposal):**
```csharp
public partial class ReportPage : IAsyncDisposable
{
    private AppDbContext? _db;

    protected override async Task OnInitializedAsync()
    {
        _db = DbFactory.CreateDbContext();
        _report = await _db.Reports.AsNoTracking().ToListAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_db is not null)
            await _db.DisposeAsync();
    }
}
```

---

## Applying L3: Recommended Order

Apply optimizations in this order to minimize risk:

1. **`IDbContextFactory` pattern** (§5a) — foundational; enables safe async DB operations
2. **`OnInitialized` → `OnInitializedAsync`** (§1a) + **EF Core sync → async** (§1b) — biggest throughput gain
3. **`AsNoTracking()` on read-only queries** (§2a) — safe, measurable memory reduction
4. **`@key` on foreach loops** (§4a) — DOM diffing improvement
5. **`[SupplyParameterFromQuery]`** (§3a) — cleanup / remove boilerplate
6. **Code-behind extraction** (§4c) — maintainability
7. **Review `[StreamRendering]`** (§3b) — advanced; only after above are done
8. **Review `ShouldRender()`** (§4b) — only for specific high-frequency-render components

---

## L3 Report Template

When generating a report, use this format:

```markdown
## L3 Optimization Report — [ProjectName]

**Date:** [date]
**Files reviewed:** [N]
**Files changed:** [N]

### Applied Changes

| File | Optimization | Category | Confidence |
|------|-------------|---------|----------|
| Students.razor.cs | OnInitialized → OnInitializedAsync | Async/Await | ✅ Safe |
| Students.razor.cs | GetJoinedTableData → async + ToListAsync | Async/Await | ✅ Safe |
| Courses.razor | @key added to foreach | Component | ✅ Safe |

### Skipped / Needs Review

| File | Optimization | Reason |
|------|-------------|--------|
| Students.razor | [StreamRendering] | Loading state placeholder not present |
| Instructors.razor.cs | ShouldRender() | Complex state comparison needed |

### Before/After Summary

**Students.razor.cs — OnInitialized sync → async**

Before:
```csharp
protected override void OnInitialized()
{
    _students = studLogic.GetJoinedTableData();
}
```

After:
```csharp
protected override async Task OnInitializedAsync()
{
    _students = await studLogic.GetJoinedTableDataAsync();
}
```
```

---

## Anti-Patterns

### ❌ Applying L3 to a Broken Build

```
# WRONG — run only after the app builds and runs
"Apply L3 optimizations" [while there are compile errors]
```

Async patterns surface errors that were previously hidden. Fix all build errors first.

### ❌ Adding `AsNoTracking()` to Write Operations

```csharp
// WRONG — loading with AsNoTracking then trying to save
using var db = DbFactory.CreateDbContext();
var product = await db.Products.AsNoTracking().FirstAsync(p => p.ID == id);
product.Price = newPrice;
await db.SaveChangesAsync(); // ❌ throws — entity not tracked
```

```csharp
// RIGHT — no AsNoTracking when you intend to modify and save
using var db = DbFactory.CreateDbContext();
var product = await db.Products.FirstAsync(p => p.ID == id);
product.Price = newPrice;
await db.SaveChangesAsync(); // ✅
```

### ❌ Using Loop Index as `@key`

```razor
@* WRONG — positional key defeats diffing optimization *@
@for (int i = 0; i < _products.Count; i++)
{
    <ProductCard @key="i" Product="@_products[i]" />
}

@* RIGHT — identity-based key *@
@foreach (var product in _products)
{
    <ProductCard @key="product.ProductID" Product="@product" />
}
```

### ❌ Making Service Interfaces Async Without Updating Implementations

```csharp
// If you add async to the interface...
public interface IStudentsLogic
{
    Task<List<object>> GetJoinedTableDataAsync();
}

// ...you MUST update the implementation too
public class StudentsListLogic : IStudentsLogic
{
    public async Task<List<object>> GetJoinedTableDataAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.Students.AsNoTracking().Select(...).ToListAsync();
    }
}
```

---

## References

- [EF Core async operations](https://learn.microsoft.com/ef/core/miscellaneous/async)
- [EF Core no-tracking queries](https://learn.microsoft.com/ef/core/querying/tracking)
- [EF Core split queries](https://learn.microsoft.com/ef/core/querying/single-split-queries)
- [Blazor render modes (.NET 10)](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes)
- [Blazor StreamRendering](https://learn.microsoft.com/aspnet/core/blazor/components/rendering#streaming-rendering)
- [SupplyParameterFromQuery](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/routing#query-strings)
- [IDbContextFactory in Blazor](https://learn.microsoft.com/ef/core/dbcontext-configuration/#using-a-dbcontext-factory-eg-for-blazor)
- [Blazor component lifecycle](https://learn.microsoft.com/aspnet/core/blazor/components/lifecycle)
