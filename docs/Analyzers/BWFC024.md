# BWFC024: ScriptManager Code-Behind Usage

**Diagnostic ID:** `BWFC024`  
**Severity:** ⚠️ Warning  
**Category:** Migration  
**Status:** Active

---

## What It Detects

This analyzer warns when you use `ScriptManager` code-behind methods like `GetCurrent()`, `SetFocus()`, `RegisterAsyncPostBackControl()`, and similar — Web Forms APIs for managing client scripts and UpdatePanel behavior.

**Note:** Phase 2 now includes support for `GetCurrent()` and script registration methods via `ScriptManagerShim`. Methods like `SetFocus()` and `RegisterAsyncPostBackControl()` still require modernization.

**Detected patterns:**
- `ScriptManager.GetCurrent(Page)` / `ScriptManager.GetCurrent(this)` — ✅ Phase 2 supported
- `.SetFocus(control)` — ❌ Still requires JS interop
- `.RegisterAsyncPostBackControl(control)` — ❌ Still requires component binding
- `.RegisterUpdateProgress(...)` — ❌ Still requires component state
- `.RegisterPostBackControl(...)` — ❌ Not supported

---

## Example

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // ⚠️ BWFC024: ScriptManager.GetCurrent() and related methods are not available in Blazor.
    ScriptManager sm = ScriptManager.GetCurrent(Page);
    
    // Set focus
    sm.SetFocus(txtSearchBox);
    
    // Register for async postback
    sm.RegisterAsyncPostBackControl(gvData);
}
```

---

## Why It Matters

`ScriptManager` methods are deeply tied to **Web Forms' postback and UpdatePanel architecture**:

- `GetCurrent()` retrieves the page's ScriptManager instance
- `SetFocus()` ensures client focus after postback
- `RegisterAsyncPostBackControl()` enables AJAX partial-page updates
- `RegisterUpdateProgress()` shows progress during UpdatePanel operations

In Blazor:

- **No `ScriptManager` instance** — script management is component-scoped
- **No postback lifecycle** — focus handling is explicit
- **No UpdatePanel model** — components handle their own updates natively
- **No async postback concept** — Blazor uses real-time SignalR sync

These methods have **no direct equivalents**. Each requires a different approach.

---

## How to Fix

The fix depends on **which** ScriptManager method you're using and which Phase you're in.

### ✅ Phase 2: GetCurrent() and Script Registration Methods

`ScriptManager.GetCurrent()` now returns a working `ScriptManagerShim` that delegates to `ClientScriptShim`.

=== "Web Forms (Before)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager sm = ScriptManager.GetCurrent(Page);
        sm.RegisterStartupScript(this.GetType(), "init", "initPage();", true);
    }
    ```

=== "Blazor (After — Phase 2, Zero Rewrite)"
    ```csharp
    protected override void OnInitialized()
    {
        // Same code works! ScriptManagerShim returns the component's ClientScriptShim
        ScriptManager sm = ScriptManager.GetCurrent(this);
        sm.RegisterStartupScript(this.GetType(), "init", "initPage();", true);
    }
    ```

---

### Still Requiring Modernization

Other ScriptManager methods still require refactoring. See the fixes below.

### Fix 1: SetFocus() → JavaScript Interop

Focus management in Blazor uses `@ref` and `IJSRuntime`.

=== "Web Forms (Before)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptManager.SetFocus(txtSearchBox);
    }
    ```

=== "Blazor (After)"
    ```razor
    @inject IJSRuntime JS
    
    <input @ref="searchBox" />
    
    @code {
        private ElementReference searchBox;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("focus", searchBox);
            }
        }
    }
    ```

Or define a helper function in JavaScript:

```javascript
// app.js
export function focusElement(element) {
    element?.focus();
}
```

```csharp
// Component
await module.InvokeVoidAsync("focusElement", searchBox);
```

### Fix 2: RegisterAsyncPostBackControl() → Remove

`RegisterAsyncPostBackControl()` enables partial-page AJAX updates via UpdatePanel.

**Blazor does NOT support UpdatePanel-style AJAX postbacks.** Blazor components handle all updates natively via parameter binding.

=== "Web Forms (Before)"
    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        // Enable AJAX partial updates for GridView
        ScriptManager.RegisterAsyncPostBackControl(gvData);
    }
    
    protected void gvData_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Partial page refresh in UpdatePanel
        gvData.DataSource = GetUpdatedData();
        gvData.DataBind();
    }
    ```

=== "Blazor (After)"
    ```razor
    @page "/data"
    @inject HttpClient Http
    
    <GridView Data="@items" OnRowSelected="@HandleRowSelected" />
    
    @code {
        private List<Item> items;
        
        protected override async Task OnInitializedAsync()
        {
            items = await Http.GetFromJsonAsync<List<Item>>("/api/items");
        }
        
        private async Task HandleRowSelected(int itemId)
        {
            // Component automatically re-renders when data changes
            items = await Http.GetFromJsonAsync<List<Item>>("/api/items");
        }
    }
    ```

**Key difference:** Blazor components automatically re-render when state changes. No manual registration needed.

### Fix 3: RegisterUpdateProgress() → Use Component State

`RegisterUpdateProgress()` shows a message or spinner during UpdatePanel operations.

=== "Web Forms (Before)"
    ```csharp
    ScriptManager.RegisterUpdateProgress(updateProgress, masterUpdateProgress);
    
    // During async postback, the UpdateProgress displays
    protected void LongRunningOperation()
    {
        System.Threading.Thread.Sleep(5000);  // Simulates work
    }
    ```

=== "Blazor (After)"
    ```razor
    @if (isLoading)
    {
        <div class="loading-overlay">
            <p>Loading data...</p>
        </div>
    }
    
    <GridView Data="@items" />
    
    @code {
        private List<Item> items;
        private bool isLoading;
        
        private async Task LoadData()
        {
            isLoading = true;
            try
            {
                items = await Http.GetFromJsonAsync<List<Item>>("/api/items");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
    ```

### Fix 4: GetCurrent() → Remove

`ScriptManager.GetCurrent()` retrieves the page's ScriptManager instance for other operations.

**In Blazor, there is no page-level `ScriptManager`.** Remove calls to `GetCurrent()` and replace the specific methods (SetFocus, RegisterAsyncPostBackControl, etc.) with the patterns above.

=== "Web Forms (Before)"
    ```csharp
    ScriptManager sm = ScriptManager.GetCurrent(Page);
    sm.SetFocus(txtField);
    sm.RegisterAsyncPostBackControl(gridView);
    ```

=== "Blazor (After)"
    ```razor
    @inject IJSRuntime JS
    
    <input @ref="field" />
    <GridView Data="@items" OnRowSelected="@HandleRowSelected" />
    
    @code {
        private ElementReference field;
        private List<Item> items;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("focus", field);
            }
        }
        
        private async Task HandleRowSelected(int id)
        {
            // Update component state; Blazor handles re-render
            items = await FetchDataAsync();
        }
    }
    ```

---

## Migration Quick Reference

| Web Forms Method | Blazor Equivalent | Approach |
|---|---|---|
| `GetCurrent(Page)` | `ScriptManager.GetCurrent(this)` (Phase 2 — Zero Rewrite) | Easy |
| `.SetFocus(control)` | `JS.InvokeVoidAsync("focus", @ref)` | JavaScript interop |
| `.RegisterAsyncPostBackControl()` | Component parameter binding | Remove; use `@bind` or `EventCallback` |
| `.RegisterPostBackControl()` | — | Remove; not needed in Blazor |
| `.RegisterUpdateProgress()` | Component state flag | Use `@if (isLoading)` UI binding |
| `.IsInAsyncPostBack` | — | Remove; always synchronous in Blazor |

---

## Real-World Example: Search Page with Loading Indicator

=== "Web Forms (Before)"
    ```csharp
    public partial class SearchPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager sm = ScriptManager.GetCurrent(Page);
            sm.SetFocus(txtSearchBox);
            sm.RegisterUpdateProgress(updateProgress, masterUpdateProgress);
        }
        
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Long-running search
            var results = SearchDatabase(txtSearchBox.Text);
            gvResults.DataSource = results;
            gvResults.DataBind();
            // UpdateProgress shows during postback
        }
    }
    ```

=== "Blazor (After)"
    ```razor
    @page "/search"
    @inject IJSRuntime JS
    @inject HttpClient Http
    
    <input @ref="searchBox" placeholder="Search..." />
    <button @onclick="PerformSearch" disabled="@isLoading">Search</button>
    
    @if (isLoading)
    {
        <div class="loading-overlay">
            <p>Searching...</p>
            <div class="spinner"></div>
        </div>
    }
    
    <GridView Data="@results" />
    
    @code {
        private ElementReference searchBox;
        private string searchQuery;
        private List<SearchResult> results = new();
        private bool isLoading;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("focus", searchBox);
            }
        }
        
        private async Task PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return;
            
            isLoading = true;
            try
            {
                results = await Http.GetFromJsonAsync<List<SearchResult>>(
                    $"/api/search?q={Uri.EscapeDataString(searchQuery)}");
            }
            finally
            {
                isLoading = false;
            }
        }
    }
    
    public class SearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    ```

---

## Common Mistakes

### ❌ Don't: Try to Call ScriptManager Methods

```csharp
// ❌ WRONG: ScriptManager.GetCurrent() returns null in Blazor
ScriptManager sm = ScriptManager.GetCurrent(Page);  // null
sm.SetFocus(txtField);  // NullReferenceException!
```

### ✅ Do: Use Component-Based Patterns

```csharp
// ✅ CORRECT: Use @ref and IJSRuntime
@ref="field"
await JS.InvokeVoidAsync("focus", field);
```

### ❌ Don't: Use RegisterAsyncPostBackControl() for Partial Updates

```csharp
// ❌ WRONG: UpdatePanel AJAX is not supported
ScriptManager.RegisterAsyncPostBackControl(gridView);
// gridView.DataBind() won't trigger partial refresh
```

### ✅ Do: Let Components Handle Their Own Updates

```csharp
// ✅ CORRECT: Component re-renders on state change
items = await FetchUpdatedData();
// Blazor automatically syncs the UI
```

### ❌ Don't: Check IsInAsyncPostBack

```csharp
// ❌ WRONG: No async postback concept in Blazor
if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
{
    // This code path doesn't exist
}
```

### ✅ Do: Use Component Lifecycle Events

```csharp
// ✅ CORRECT: Blazor components are always "interactive"
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // Component is interactive; safe to use JS interop
    }
}
```

---

## Related Analyzers

- **[BWFC022](BWFC022.md)** — Page.ClientScript usage (see **ClientScriptShim** for easy migration)
- **[BWFC023](BWFC023.md)** — IPostBackEventHandler usage

---

## Configuration

To suppress this warning for a specific line:

```csharp
#pragma warning disable BWFC024
ScriptManager.GetCurrent(Page).SetFocus(txtField);
#pragma warning restore BWFC024
```

Or in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.BWFC024.severity = silent
```

---

## See Also

- 📖 [ClientScriptMigrationGuide.md](../Migration/ClientScriptMigrationGuide.md) — Section 7: ScriptManager Patterns
- 📖 [IJSRuntime Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability) — JavaScript interop
- 📖 [Component Lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle) — OnAfterRenderAsync
- 📖 [EditForm Component](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components) — Form handling in Blazor

---

**Status:** ✅ Active  
**Last Updated:** 2026-07-30  
**Owner:** Beast (Technical Writer)
