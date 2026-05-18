# ClientScript Migration Guide

When migrating from ASP.NET Web Forms to Blazor, one of the most critical patterns to understand is **JavaScript execution**. In Web Forms, `Page.ClientScript` and `ScriptManager` manage all client-side scripts. In Blazor, this responsibility falls to `IJSRuntime` and component lifecycle events.

This guide covers the major ClientScript patterns, why they differ in Blazor, and how to migrate each one.

---

## 🎯 Recommended: ClientScriptShim (Zero-Rewrite Path)

The **easiest path** for most migrations is the **ClientScriptShim** — a compatibility layer that provides the same API as `Page.ClientScript` but runs on Blazor's `IJSRuntime` internally.

### What It Is

`ClientScriptShim` is a scoped Blazor service included in BWFC that:
- **Accepts the same method calls** as Web Forms' `Page.ClientScript`
- **Requires zero code rewrites** — your existing `RegisterStartupScript()`, `RegisterClientScriptBlock()`, etc. calls work unchanged
- **Queues scripts** during component initialization
- **Auto-flushes** in `OnAfterRenderAsync` via `IJSRuntime`
- **Handles deduplication** by type+key (same behavior as Web Forms)

### Automatic Registration

When you call `AddBlazorWebFormsComponents()` in your Startup, `ClientScriptShim` is registered as a scoped service and ready to use.

### How to Use It

**For components inheriting `BaseWebFormsComponent`:**

The `ClientScript` property is automatically available — use it exactly as you would in Web Forms:

```csharp
protected override void OnInitialized()
{
    ClientScript.RegisterStartupScript(GetType(), "init",
        "alert('Page loaded!');", true);
}
```

**For any other component:**

Inject `ClientScriptShim` and use it the same way:

```razor
@inject ClientScriptShim ClientScript

@code {
    protected override void OnInitialized()
    {
        ClientScript.RegisterStartupScript(GetType(), "init",
            "alert('Page loaded!');", true);
    }
}
```

### Supported Methods

| Method | Status | Notes |
|--------|--------|-------|
| `RegisterStartupScript(Type, string, string, bool)` | ✅ Supported | Executes in `OnAfterRenderAsync` |
| `RegisterStartupScript(Type, string, string)` | ✅ Supported | `addScriptTags` defaults to false |
| `RegisterClientScriptBlock(Type, string, string, bool)` | ✅ Supported | Executes before startup scripts |
| `RegisterClientScriptBlock(Type, string, string)` | ✅ Supported | `addScriptTags` defaults to false |
| `RegisterClientScriptInclude(string, string)` | ✅ Supported | Dynamically appends `<script>` tag |
| `RegisterClientScriptInclude(Type, string, string)` | ✅ Supported | Type parameter ignored (for compatibility) |
| `IsStartupScriptRegistered(Type, string)` | ✅ Supported | Deduplication check |
| `IsClientScriptBlockRegistered(Type, string)` | ✅ Supported | Deduplication check |
| `IsClientScriptIncludeRegistered(string)` | ✅ Supported | Deduplication check |
| `GetPostBackEventReference(...)` | ✅ Supported (Phase 2) | Returns `__doPostBack()` string; handled by postback shim |
| `GetPostBackClientHyperlink(...)` | ✅ Supported (Phase 2) | Returns hyperlink-compatible postback string |
| `GetCallbackEventReference(...)` | ✅ Supported (Phase 2) | Returns callback bridge string; requires JS handler |

### How It Works Internally

1. When you call `RegisterStartupScript()`, the script is **queued** in memory (same deduplication as Web Forms)
2. During `OnAfterRenderAsync`, `BaseWebFormsComponent` calls `ClientScript.FlushAsync(IJSRuntime)`
3. Scripts execute in order: **script blocks first**, then **startup scripts**, then **includes**
4. The queue clears after each flush cycle

### Before/After Example

**Web Forms code-behind:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    Page.ClientScript.RegisterStartupScript(GetType(), "init", 
        "console.log('Page loaded');", true);
}
```

**Blazor code-behind (with ClientScriptShim — changes circled in red):**
```csharp
protected override void OnInitialized()
{
    ClientScript.RegisterStartupScript(GetType(), "init",
        "console.log('Page loaded');", true);
}
```

**The ClientScript call is identical.** Only the lifecycle method name changed.

---

## Strangler Fig Pattern Context

ClientScript migration fits within the broader **Strangler Fig migration pattern** — the overarching strategy for incrementally moving from Web Forms to Blazor while keeping both systems running side-by-side.

In the Strangler Fig approach:
- You migrate one page or feature at a time, not all at once
- The legacy Web Forms app continues running in parallel
- Traffic gradually shifts from Web Forms to Blazor
- **ClientScriptShim enables this** by letting your JavaScript patterns work unchanged during migration

This means you don't need to decide "now or never" on JavaScript refactoring. Use ClientScriptShim to get your migration done quickly (Phase 1), then optionally modernize to `IJSRuntime` or JS modules later when it makes sense (Phase 2).

For a detailed overview of how ClientScript fits into the incremental migration journey, see the **[Strangler Fig Pattern guide](StranglerFigPattern.md)**.

---

## Migration Approaches: A Comparison

Choose the approach that fits your migration timeline:

| Approach | Code Changes | Effort | When to Use |
|----------|-------------|--------|-------------|
| **ClientScriptShim** (recommended) | None — keep existing calls | ⭐ Minimal | Default for most migrations; fastest path to working code |
| Manual IJSRuntime rewrite | Rewrite to `IJSRuntime.InvokeVoidAsync()` | ⭐⭐ Moderate | When you want to modernize fully and leverage Blazor patterns |
| JS Module pattern | Extract to ES modules, use `IJSObjectReference` | ⭐⭐⭐ Full modernization | New code or heavy JavaScript interaction; long-term maintainability |

**📌 Bottom line:** Use ClientScriptShim for the first pass to get your migration done quickly. Refactor to modern patterns in Phase 2 if desired.

---

## Overview: Why ClientScript Patterns Differ

### What ClientScript Does in Web Forms

In Web Forms, `Page.ClientScript` (also called `ClientScriptManager`) enables server-side code to:
- Register startup scripts that run when the page loads
- Include external script files
- Generate postback event references (dynamic `__doPostBack` calls)
- Manage script deduplication and versioning

**Web Forms assumed:**
- Server-side postback lifecycle (`IsPostBack`)
- Automatic script injection into the rendered HTML
- Browser-managed form submission via `__doPostBack()`

### What Blazor Offers Instead

In Blazor, JavaScript interop is **explicit, component-scoped, and lifecycle-aware:**
- `IJSRuntime.InvokeVoidAsync()` / `InvokeAsync<T>()` for calling JavaScript from C#
- Component lifecycle hooks (`OnInitializedAsync`, `OnAfterRenderAsync`) for timing
- No postback model — events are direct component method calls
- Prerendering considerations (server-side rendering without browser interactivity)

**This is fundamentally more explicit** — you must choose *when* to run JavaScript and *where* it lives (HTML, JavaScript file, or inline in C#). This explicitness is actually safer and easier to reason about than implicit `ClientScript` injection.

---

## Quick Reference: ClientScript Patterns → Blazor Equivalents

| Web Forms Pattern | Blazor Equivalent | Difficulty |
|---|---|---|
| `RegisterStartupScript()` with inline script | `OnAfterRenderAsync(firstRender)` + `IJSRuntime.InvokeVoidAsync()` | ⭐ Easy |
| `RegisterClientScriptInclude()` | `<script src="">` in layout or JS module import | ⭐ Easy |
| `RegisterClientScriptBlock()` | Inline `<script>` in component or JS module | ⭐ Easy |
| `GetPostBackEventReference()` | `@onclick` or `EventCallback<T>` | ⭐⭐ Medium |
| Form validation with `Page.IsValid` | `EditContext` + `DataAnnotationsValidator` | ⭐⭐ Medium |
| `IPostBackEventHandler` implementation | `EventCallback<T>` parameter | ⭐⭐ Medium |
| `ScriptManager.SetFocus()` | `@ref` element + `JS.InvokeVoidAsync("focus", ref)` | ⭐⭐ Medium |
| `ScriptManager.RegisterAsyncPostBackControl()` | Remove (Blazor uses component binding) | ⭐⭐⭐ Complex |
| Dynamic form submission with `__doPostBack()` | Rewrite as component method calls | ⭐⭐⭐ Complex |

---

## 1. Startup Scripts — The Most Common Pattern

### What It Does

`RegisterStartupScript()` runs a block of JavaScript after the page fully loads. Used for initialization: theme application, jQuery plugins, validation setup, etc.

### 🎯 Easiest Approach: ClientScriptShim

For a zero-change migration, **use the ClientScriptShim**:

```csharp
protected override void OnInitialized()
{
    // No code change from Web Forms!
    ClientScript.RegisterStartupScript(GetType(), "InitializeTheme",
        "applyTheme('dark');", true);
}
```

See [ClientScriptShim (Zero-Rewrite Path)](#-recommended-clientscriptshim-zero-rewrite-path) above for full details.

### Alternative Approach: Modern IJSRuntime Rewrite

If you prefer to modernize, use `OnAfterRenderAsync()` + `IJSRuntime`. This follows current Blazor best practices but requires code changes.

#### Web Forms

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        Page.ClientScript.RegisterStartupScript(
            type: this.GetType(),
            key: "InitializeTheme",
            script: "$(function() { applyTheme('dark'); });",
            addScriptTags: true);
    }
}
```

The `if (!IsPostBack)` guard ensures the script runs only on first load, not on postbacks.

### Blazor Equivalent

In Blazor, the equivalent is **`OnAfterRenderAsync(bool firstRender)`**, which fires after the component renders and the DOM is available.

```razor
@inject IJSRuntime JS

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Script runs only on first render, like !IsPostBack
            await JS.InvokeVoidAsync("eval", "applyTheme('dark');");
            
            // Or better: define function in JavaScript and call it
            // await JS.InvokeVoidAsync("initializeTheme");
        }
    }
}
```

### Key Differences

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| **Guard** | `if (!IsPostBack)` | `if (firstRender)` |
| **Hook** | Page load (automatic) | `OnAfterRenderAsync` (explicit) |
| **Script location** | Injected by server | External file or JS module |
| **Timing** | After `<body>` closes | After component DOM renders |

### Best Practice: Use JavaScript Modules (If Modernizing)

For a modern Blazor approach that's cleaner long-term, **define a JavaScript function in a module** and call it. This is optional if you're using ClientScriptShim initially.

Rather than `eval()`, **define a JavaScript function in a module** and call it:

=== "JavaScript (app.js)"
    ```javascript
    export function initializeTheme() {
        const theme = localStorage.getItem('theme') || 'light';
        document.documentElement.setAttribute('data-theme', theme);
    }
    ```

=== "Blazor Component"
    ```razor
    @inject IJSRuntime JS
    
    @code {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Load the module and call the function
                var module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./app.js");
                await module.InvokeVoidAsync("initializeTheme");
            }
        }
    }
    ```

This is cleaner, type-safe (intellisense works), and easier to test.

---

## 2. Script Includes — External JavaScript Files

### What It Does

`RegisterClientScriptInclude()` references an external `.js` file, ensuring it loads before dependent scripts.

### 🎯 Easiest Approach: ClientScriptShim

For a zero-change migration, **use the ClientScriptShim**:

```csharp
protected override void OnInitialized()
{
    // No code change from Web Forms!
    ClientScript.RegisterClientScriptInclude(
        "jquery-ui",
        "lib/jquery-ui/jquery-ui.min.js");
}
```

The shim dynamically appends `<script>` tags via `IJSRuntime` in `OnAfterRenderAsync`.

### Alternative Approaches

#### Approach 1: Static `<script>` tags in layout (Recommended for Always-Needed Scripts)

```html
<!-- Pages/_Layout.html or index.html -->
<!DOCTYPE html>
<html>
<head>
    <script src="_framework/lib/jquery/jquery.min.js"></script>
    <script src="_framework/lib/jquery-ui/jquery-ui.min.js"></script>
    <script src="app.js"></script>
</head>
<body>
    <!-- Blazor app content -->
</body>
</html>
```

**Option 2: Dynamic import via IJSRuntime (For Conditional Loads)**

If the script is only needed conditionally (e.g., admin users only):

=== "TypeScript/JavaScript"
    ```javascript
    export async function loadAdminTools() {
        // Dynamically import the admin module
        const adminModule = await import('./admin-tools.js');
        adminModule.init();
    }
    ```

=== "Blazor Component"
    ```razor
    @inject IJSRuntime JS
    @inject AuthenticationStateProvider Auth
    
    @code {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var authState = await Auth.GetAuthenticationStateAsync();
                if (authState.User.IsInRole("Admin"))
                {
                    var module = await JS.InvokeAsync<IJSObjectReference>(
                        "import", "./app.js");
                    await module.InvokeVoidAsync("loadAdminTools");
                }
            }
        }
    }
    ```

### Key Differences

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| **Inclusion** | Server-side `RegisterClientScriptInclude()` | HTML `<script>` tags or JS `import()` |
| **Path** | `ResolveUrl("~/...")` | Web root paths (no `~` needed) |
| **Conditional** | Check in C# code | Check in component logic |
| **Timing** | Before `</body>` | Before component loads or on-demand |

---

## 3. Inline Script Blocks

### What It Does

`RegisterClientScriptBlock()` injects inline JavaScript code directly into the page, often for utility functions or event handlers.

### 🎯 Easiest Approach: ClientScriptShim

For a zero-change migration, **use the ClientScriptShim**:

```csharp
protected override void OnInitialized()
{
    string script = @"
        function togglePanel(id) {
            var panel = document.getElementById(id);
            panel.style.display = panel.style.display === 'none' ? 'block' : 'none';
        }
    ";
    
    // No code change from Web Forms!
    ClientScript.RegisterClientScriptBlock(
        this.GetType(),
        "TogglePanelScript",
        script,
        addScriptTags: true);
}
```

### Alternative Approaches

#### Approach 1: JavaScript Module (Recommended for Maintainability)

=== "JavaScript (utils.js)"
    ```javascript
    export function togglePanel(id) {
        const panel = document.getElementById(id);
        panel.style.display = panel.style.display === 'none' ? 'block' : 'none';
    }
    ```

=== "Blazor Component"
    ```razor
    @inject IJSRuntime JS
    
    <button @onclick="() => TogglePanel('myPanel')">Toggle</button>
    <div id="myPanel">Content</div>
    
    @code {
        private IJSObjectReference? module;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./utils.js");
            }
        }
        
        private async Task TogglePanel(string id)
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync("togglePanel", id);
            }
        }
    }
    ```

### Alternatively: Inline Script in Layout

For truly global scripts, you can inline them in `index.html` or the layout:

```html
<script>
    function togglePanel(id) {
        const panel = document.getElementById(id);
        panel.style.display = panel.style.display === 'none' ? 'block' : 'none';
    }
</script>
```

**But avoid this pattern** — it pollutes the global namespace and makes testing harder. Prefer JavaScript modules.

---

## 4. Postback Event References

### What It Does

`GetPostBackEventReference()` generates a dynamic JavaScript call to trigger a postback event, often used in client-side event handlers that need to notify the server. **Phase 2 now includes a working shim for this pattern.**

### 🎯 Easiest Approach: ClientScriptShim (Phase 2 — Zero Rewrite)

**Web Forms:**
```csharp
public string GetDeleteButtonScript()
{
    // Generate: javascript:__doPostBack('btnDelete','clicked')
    return Page.ClientScript.GetPostBackEventReference(
        new PostBackOptions(btnDelete, "clicked")
        {
            PerformValidation = false
        });
}

// Usage in markup:
// <a href='<%# GetDeleteButtonScript() %>'>Delete</a>
```

**Blazor with BWFC — Zero rewrite!**
```csharp
// Same code works! ClientScriptShim returns a working __doPostBack() JS string
public string GetDeleteButtonScript()
{
    return ClientScript.GetPostBackEventReference(
        new PostBackOptions(btnDelete, "clicked")
        {
            PerformValidation = false
        });
}

// Usage in markup:
// <a href="@GetDeleteButtonScript()">Delete</a>
```

### How It Works (Phase 2)

1. **`GetPostBackEventReference()` returns `__doPostBack('controlId', 'arg')`** — the exact same function name as Web Forms.

2. **BWFC ships `bwfc-postback.js`** which defines `__doPostBack()` as a JavaScript bridge function:
   ```javascript
   window.__doPostBack = async function(eventTarget, eventArgument) {
       // Bridge back into Blazor via JS interop
       await DotNet.invokeMethodAsync('BlazorWebFormsComponents', 'HandlePostBackFromJs', 
           eventTarget, eventArgument);
   };
   ```

3. **The page registers itself as a postback target** in `OnAfterRenderAsync`, exposing a .NET callback method via `DotNetObjectReference`.

4. **When JavaScript calls `__doPostBack()`**, it invokes the .NET `HandlePostBackFromJs` method via JS interop, which fires the page's `PostBack` event.

5. **Your C# code handles the PostBack event**, just like Web Forms:
   ```csharp
   @code {
       protected override void OnInitialized()
       {
           PostBack += (sender, args) =>
           {
               // args.EventTarget — the control that triggered the postback
               // args.EventArgument — the argument passed
               HandleMyPostBack(args.EventTarget, args.EventArgument);
           };
       }
       
       private void HandleMyPostBack(string eventTarget, string eventArgument)
       {
           if (eventTarget == "btnDelete" && eventArgument == "clicked")
           {
               DeleteItem();
           }
       }
   }
   ```

### Usage Pattern

Use `GetPostBackEventReference()` in data-bound attributes or JavaScript event handlers that need to trigger server-side actions:

```razor
@foreach (var item in items)
{
    <a href="@ClientScript.GetPostBackEventReference(item, "edit")">
        Edit
    </a>
    <a href="@ClientScript.GetPostBackEventReference(item, "delete")">
        Delete
    </a>
}

@code {
    protected override void OnInitialized()
    {
        PostBack += (sender, args) =>
        {
            if (args.EventArgument == "edit")
            {
                EditItem(args.EventTarget);
            }
            else if (args.EventArgument == "delete")
            {
                DeleteItem(args.EventTarget);
            }
        };
    }
}
```

### Alternative: Modern Blazor Approach

If you prefer to modernize away from postback patterns, use `@onclick` or `EventCallback` instead:

=== "Simple Case: @onclick"
    ```razor
    @foreach (var item in items)
    {
        <button @onclick="() => EditItem(item.Id)">Edit</button>
        <button @onclick="() => DeleteItem(item.Id)">Delete</button>
    }
    
    @code {
        private async Task EditItem(int itemId) { ... }
        private async Task DeleteItem(int itemId) { ... }
    }
    ```

=== "Parameterized Case: EventCallback"
    ```razor
    <!-- Parent component -->
    @foreach (var item in items)
    {
        <ChildComponent Item="@item" OnEdit="HandleEdit" OnDelete="HandleDelete" />
    }
    
    @code {
        private async Task HandleEdit(Item item) { ... }
        private async Task HandleDelete(Item item) { ... }
    }
    
    <!-- ChildComponent.razor -->
    @code {
        [Parameter]
        public Item Item { get; set; }
        
        [Parameter]
        public EventCallback<Item> OnEdit { get; set; }
        
        [Parameter]
        public EventCallback<Item> OnDelete { get; set; }
        
        private async Task RaiseEdit() => await OnEdit.InvokeAsync(Item);
        private async Task RaiseDelete() => await OnDelete.InvokeAsync(Item);
    }
    ```

### Key Differences

| Aspect | Web Forms | Blazor (Phase 2 with Shim) | Blazor (Modern) |
|--------|-----------|--------------------------|----------------|
| **Mechanism** | `__doPostBack()` → HTTP POST | `__doPostBack()` → JS interop → .NET | Direct component method call |
| **Server roundtrip** | Full page reload | Blazor diff sync (no page reload) | Instant (no roundtrip) |
| **Compatibility** | Zero rewrite | Zero rewrite | Requires refactoring |
| **Best for** | Code migration (Phase 1) | Code migration (Phase 2) | New development |

---

## 5. Callback Event References (Phase 2)

### What It Does

`GetCallbackEventReference()` generates a JavaScript callback bridge for server callback processing (AJAX-style communication without UpdatePanel). **Phase 2 includes a working shim for this pattern.**

### Web Forms

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    string callback = Page.ClientScript.GetCallbackEventReference(
        this, 
        "arg",           // JavaScript argument to pass
        "onSuccess",     // JavaScript function to call on success
        "ctx",           // Context object to pass to callback
        "onError",       // JavaScript function to call on error
        true);           // useAsync
    
    // Inject the callback string into a JavaScript function
    Page.ClientScript.RegisterStartupScript(this.GetType(), "initCallback",
        $"var callback = '{callback}'; " +
        "function myCallback(arg) { callback(arg); }",
        true);
}

// In markup:
// <button onclick="myCallback('someData')">Call Server</button>

// Server-side callback handler:
public void RaiseCallbackEvent(string eventArgument)
{
    // Process eventArgument and prepare return value
}

public string GetCallbackResult()
{
    // Return result to JavaScript
    return "Server processed: " + eventArgument;
}
```

### Blazor with BWFC (Phase 2 — Zero Rewrite)

```csharp
// Same pattern works! ClientScriptShim provides the callback bridge
protected override void OnInitialized()
{
    string callback = ClientScript.GetCallbackEventReference(
        this, 
        "arg",
        "onSuccess",
        "ctx",
        "onError",
        useAsync: true);
    
    // Register the callback into the page
    ClientScript.RegisterStartupScript(this.GetType(), "initCallback",
        $"var callback = '{callback}'; " +
        "function myCallback(arg) { callback(arg); }",
        true);
}

// Handler methods (same as Web Forms):
public void RaiseCallbackEvent(string eventArgument)
{
    // Process eventArgument
}

public string GetCallbackResult()
{
    // Return result to JavaScript
}
```

### How It Works (Phase 2)

1. **`GetCallbackEventReference()` returns a JavaScript function call string** that bridges back to .NET:
   ```javascript
   // Returned string looks like:
   "WebForm_DoCallback('controlId',arg,onSuccess,ctx,onError,true)"
   ```

2. **BWFC ships `bwfc-callback.js`** which defines `WebForm_DoCallback()` as a bridge:
   ```javascript
   window.WebForm_DoCallback = async function(controlId, arg, onSuccess, ctx, onError, async) {
       try {
           const result = await DotNet.invokeMethodAsync('BlazorWebFormsComponents', 
               'HandleCallbackFromJs', controlId, arg);
           if (onSuccess) {
               onSuccess(result, ctx);
           }
       } catch (err) {
           if (onError) {
               onError(err, ctx);
           }
       }
   };
   ```

3. **Your C# methods handle the callback**, just like Web Forms:
   ```csharp
   public void RaiseCallbackEvent(string eventArgument)
   {
       // Process the callback argument
       // Set _callbackResult for GetCallbackResult()
   }
   
   public string GetCallbackResult()
   {
       // Return data back to the JavaScript callback
       return _callbackResult;
   }
   ```

4. **JavaScript receives the result** in the `onSuccess` callback:
   ```javascript
   function onSuccess(result, context) {
       console.log('Server returned:', result);
       // Update UI with server response
   }
   ```

### Usage Pattern

Use callback events for AJAX-style server communication without page reload:

```razor
@inject IJSRuntime JS

<button @onclick="FetchDataViaCallback">Fetch Data</button>
<div id="result"></div>

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Define JavaScript callback handlers
            await JS.InvokeVoidAsync("eval", @"
                window.onCallbackSuccess = function(result, context) {
                    document.getElementById('result').innerHTML = result;
                };
                window.onCallbackError = function(error, context) {
                    console.error('Callback error:', error);
                };
            ");
        }
    }
    
    private async Task FetchDataViaCallback()
    {
        // Get the callback reference
        string callback = ClientScript.GetCallbackEventReference(
            this,
            "\"userQuery\"",  // Argument to pass
            "onCallbackSuccess",
            "null",
            "onCallbackError",
            useAsync: true);
        
        // Execute it via JS interop
        await JS.InvokeVoidAsync("eval", $"{callback};");
    }
    
    public void RaiseCallbackEvent(string eventArgument)
    {
        // Process the query
        _callbackResult = $"Data for: {eventArgument}";
    }
    
    private string _callbackResult = "";
    
    public string GetCallbackResult()
    {
        return _callbackResult;
    }
}
```

### Alternative: Modern Blazor Approach

For new development, use `IJSRuntime` with direct method calls instead of callbacks:

```razor
@inject IJSRuntime JS
@inject HttpClient Http

<button @onclick="FetchData">Fetch Data</button>
<div id="result">@result</div>

@code {
    private string result = "";
    
    private async Task FetchData()
    {
        // Direct async call to server
        result = await Http.GetStringAsync("/api/data");
    }
}
```

This is cleaner, type-safe, and easier to test than callback-based patterns.

---

## 6. Form Validation Scripts

### What It Does

Web Forms uses `Page.IsValid` and `Page.Validate()` to check server-side validators. Client-side validation scripts often run before postback to prevent unnecessary round trips.

### Web Forms

```csharp
protected void btnSubmit_Click(object sender, EventArgs e)
{
    // Validators run server-side
    if (!Page.IsValid)
    {
        // Show error
        return;
    }
    
    // Process form
    SaveData();
}

// In markup:
// <asp:RequiredFieldValidator ControlToValidate="txtName" />
// <asp:RangeValidator ControlToValidate="txtAge" MinimumValue="0" MaximumValue="120" />
```

### Blazor Equivalent

Use `EditForm` with `EditContext` and `DataAnnotationsValidator`. Validation is **declarative** (via data annotations) and works on both client and server:

```razor
@inject HttpClient Http

<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <div class="form-group">
        <label for="name">Name:</label>
        <InputText id="name" @bind-Value="model.Name" />
        <ValidationMessage For="() => model.Name" />
    </div>
    
    <div class="form-group">
        <label for="age">Age:</label>
        <InputNumber id="age" @bind-Value="model.Age" />
        <ValidationMessage For="() => model.Age" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</EditForm>

@code {
    private FormModel model = new();
    
    private async Task HandleSubmit()
    {
        // Only called if validation passes
        await SaveDataAsync();
    }
}

public class FormModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
    public int Age { get; set; }
}
```

### Key Differences

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| **Declaration** | Server-side validator controls | C# data annotations |
| **Client-side validation** | Rendered JavaScript from validators | Built-in via `DataAnnotationsValidator` |
| **Validation timing** | Submit button click → postback | Form submission or real-time |
| **Custom rules** | Custom validators or `CustomValidator` control | `ValidationAttribute` subclass |

### Custom Validators

**Web Forms:**
```csharp
<asp:CustomValidator 
    OnServerValidate="ValidateDateRange"
    ErrorMessage="Date must be in the past" />
```

**Blazor:**
```csharp
public class DateInPastAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext ctx)
    {
        var date = (DateTime?)value;
        return date < DateTime.Now 
            ? ValidationResult.Success 
            : new ValidationResult("Date must be in the past");
    }
}

// Usage in model:
[DateInPast]
public DateTime EventDate { get; set; }
```

---

## 7. IPostBackEventHandler — Custom Event Binding

### What It Does

`IPostBackEventHandler` allows controls to raise custom events in response to postback data. Rarely used directly, but common in composite controls.

### Web Forms

```csharp
public partial class MyCustomControl : UserControl, IPostBackEventHandler
{
    public event EventHandler OnCustomAction;
    
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "myaction")
        {
            OnCustomAction?.Invoke(this, EventArgs.Empty);
        }
    }
    
    // Markup triggers postback:
    // <a href='<%# Page.ClientScript.GetPostBackEventReference(this, "myaction") %>'>
}
```

### Blazor Equivalent

Use `EventCallback<T>` parameters instead:

```razor
<!-- MyCustomComponent.razor -->
@code {
    [Parameter]
    public EventCallback OnCustomAction { get; set; }
    
    private async Task RaiseCustomAction()
    {
        await OnCustomAction.InvokeAsync();
    }
}

<!-- Usage in parent: -->
<MyCustomComponent OnCustomAction="HandleCustomAction" />

@code {
    private async Task HandleCustomAction()
    {
        // Handle the event
    }
}
```

### With Arguments

If the postback event passes data:

=== "Web Forms"
    ```csharp
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument.StartsWith("select-"))
        {
            string itemId = eventArgument.Replace("select-", "");
            OnItemSelected?.Invoke(this, new ItemSelectedEventArgs { ItemId = itemId });
        }
    }
    ```

=== "Blazor"
    ```razor
    @code {
        [Parameter]
        public EventCallback<string> OnItemSelected { get; set; }
        
        private async Task SelectItem(string itemId)
        {
            await OnItemSelected.InvokeAsync(itemId);
        }
    }
    ```

---

## 8. ScriptManager Code-Behind Patterns

### SetFocus()

**Web Forms:**
```csharp
ScriptManager.SetFocus(txtUserName);
```

**Blazor:**
```razor
@inject IJSRuntime JS

<input @ref="userNameRef" />

@code {
    private ElementReference userNameRef;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("focus", userNameRef);
        }
    }
}
```

### RegisterAsyncPostBackControl()

**Web Forms:**
```csharp
ScriptManager.RegisterAsyncPostBackControl(gvData);
// Enables AJAX partial page updates via UpdatePanel
```

**Blazor:**
`RegisterAsyncPostBackControl()` **has no equivalent** in Blazor because Blazor components handle updates natively via parameter binding and `EventCallback`. Remove this line.

Instead, let the component update naturally:

```razor
@page "/data"

<GridView Data="@items" OnRowSelected="HandleRowSelected" />

@code {
    private List<Item> items;
    
    private async Task HandleRowSelected(int itemId)
    {
        // Component updates automatically via @bind or parameters
        var item = await FetchItemAsync(itemId);
        items = await FetchItemsAsync(); // Re-render with new data
    }
}
```

### RegisterUpdateProgress()

**Web Forms:**
```csharp
ScriptManager.RegisterUpdateProgress(updateProgress, masterUpdateProgress);
// Shows during async postback
```

**Blazor:**
Show a loading indicator using component state:

```razor
<div class="update-progress" style="@(isLoading ? "display:block" : "display:none")">
    <p>Loading...</p>
</div>

<button @onclick="FetchData" disabled="@isLoading">Fetch</button>

@code {
    private bool isLoading;
    
    private async Task FetchData()
    {
        isLoading = true;
        await Task.Delay(2000); // Simulate async work
        isLoading = false;
    }
}
```

### GetCurrent() and Related Methods (Phase 2)

**Web Forms:**
```csharp
ScriptManager sm = ScriptManager.GetCurrent(Page);
sm.RegisterStartupScript(this.GetType(), "init", "initPage();", true);
sm.SetFocus(txtSearch);
```

**Blazor with BWFC (Phase 2 — Zero Rewrite):**
```csharp
// Same pattern works! ScriptManagerShim wraps ClientScriptShim
ScriptManager sm = ScriptManager.GetCurrent(this);  // 'this' is the component (replaces Page)
sm.RegisterStartupScript(this.GetType(), "init", "initPage();", true);
// SetFocus still requires JS interop (see above)
```

### How It Works (Phase 2)

1. **`ScriptManager.GetCurrent(page)`** extracts the `ClientScriptShim` from the component's dependency injection context.

2. **All `RegisterStartupScript`, `RegisterClientScriptBlock`, `RegisterClientScriptInclude` calls delegate to `ClientScriptShim`**, which queues scripts during initialization.

3. **Scripts are flushed in `OnAfterRenderAsync`** via `IJSRuntime`, exactly like Phase 1.

4. **Focus and other component methods require JavaScript interop**, as documented in the sections above.

### Pattern: RegisterStartupScript via ScriptManager

Instead of calling `Page.ClientScript` directly, you can use `ScriptManager.GetCurrent()`:

```csharp
// Web Forms
ScriptManager sm = ScriptManager.GetCurrent(Page);
sm.RegisterStartupScript(this.GetType(), "init", "console.log('loaded');", true);

// Blazor (Phase 2)
ScriptManager sm = ScriptManager.GetCurrent(this);
sm.RegisterStartupScript(this.GetType(), "init", "console.log('loaded');", true);
// Zero code change! Same methods, same behavior
```

### Key Differences

| Method | Phase 1 | Phase 2 | Notes |
|--------|---------|---------|-------|
| `RegisterStartupScript()` | ✅ ClientScriptShim | ✅ Via ScriptManager | Both work; ScriptManager delegates to ClientScriptShim |
| `RegisterClientScriptBlock()` | ✅ ClientScriptShim | ✅ Via ScriptManager | Both work; same delegation |
| `RegisterClientScriptInclude()` | ✅ ClientScriptShim | ✅ Via ScriptManager | Both work; same delegation |
| `GetCurrent()` | ❌ Unsupported | ✅ Phase 2 | Returns the component's ClientScriptShim |
| `SetFocus()` | ❌ Unsupported | ❌ Still not supported | Use `JS.InvokeVoidAsync("focus", @ref)` instead |
| `RegisterAsyncPostBackControl()` | ❌ Unsupported | ❌ Still not supported | UpdatePanel is not emulated; use component binding |

### When to Use ScriptManager vs ClientScriptShim

- **Directly** — Both `ClientScript` property (Phase 1) and `ScriptManager.GetCurrent()` (Phase 2) work
- **No functional difference** — ScriptManager just wraps ClientScriptShim for API compatibility
- **Choose based on your Web Forms code** — If you used `ScriptManager`, keep using it; if you used `Page.ClientScript`, use `ClientScript` property

---

## 9. Common Pitfalls and Solutions

### Pitfall 1: Script Runs Multiple Times Due to Re-renders

**Problem:**
```razor
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    // ❌ WRONG: Runs every render, not just first
    await JS.InvokeVoidAsync("applyTheme");
}
```

**Solution:**
```razor
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    // ✅ CORRECT: Only on first render
    if (firstRender)
    {
        await JS.InvokeVoidAsync("applyTheme");
    }
}
```

### Pitfall 2: Prerendering Issues

In SSR (Server-Side Rendering) or prerendering mode, `OnAfterRenderAsync` runs on the server *without* browser interactivity. `IJSRuntime` calls fail silently.

**Problem:**
```razor
// ❌ WRONG: Fails during prerendering
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await JS.InvokeVoidAsync("applyTheme"); // No JS in SSR
    }
}
```

**Solution:**
```razor
// ✅ CORRECT: Guard with try-catch or check if interactive
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        try
        {
            await JS.InvokeVoidAsync("applyTheme");
        }
        catch (InvalidOperationException)
        {
            // Running in SSR mode; skip JS interop
        }
    }
}

// OR use a runtime check:
@inject IComponentRenderingContext RenderContext

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && RenderContext.IsInteractive)
    {
        await JS.InvokeVoidAsync("applyTheme");
    }
}
```

### Pitfall 3: Script Timing — Waiting for DOM Elements

**Problem:**
```javascript
// ❌ WRONG: Element might not exist yet
document.getElementById("myDiv").classList.add("highlight");
```

**Solution:**
```csharp
// ✅ CORRECT: Call from OnAfterRenderAsync, after render
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await JS.InvokeVoidAsync("highlightElement", "myDiv");
    }
}
```

JavaScript:
```javascript
export function highlightElement(id) {
    const elem = document.getElementById(id);
    if (elem) {
        elem.classList.add("highlight");
    }
}
```

### Pitfall 4: Module Import Caching

**Problem:**
```razor
// ❌ WRONG: Imports module every render
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    var module = await JS.InvokeAsync<IJSObjectReference>("import", "./app.js");
    await module.InvokeVoidAsync("init");
}
```

**Solution:**
```razor
// ✅ CORRECT: Cache the module
private IJSObjectReference? module;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        module = await JS.InvokeAsync<IJSObjectReference>("import", "./app.js");
        await module.InvokeVoidAsync("init");
    }
}
```

### Pitfall 5: Script Deduplication

In Web Forms, `RegisterStartupScript` with the same key runs only once per page. In Blazor, you must deduplicate manually.

**Problem:**
```razor
// Component rendered multiple times
foreach (var item in items)
{
    <MyComponent />
}

// ❌ WRONG: Each instance calls applyTheme()
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await JS.InvokeVoidAsync("applyTheme");
    }
}
```

**Solution:**
```razor
<!-- Parent component calls once -->
@foreach (var item in items)
{
    <MyComponent Item="@item" />
}

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("applyTheme"); // Once, not per child
        }
    }
}
```

Or use a static flag to prevent duplicate initialization:

```csharp
private static bool isAppInitialized;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && !isAppInitialized)
    {
        isAppInitialized = true;
        await JS.InvokeVoidAsync("applyTheme");
    }
}
```

---

## 10. What We Don't Support (And Why)

### `__doPostBack()` and Postback Events

**Why not?**
- Web Forms postback is an HTTP POST with form-encoded data and event validation
- Blazor is **component-based** with direct method calls, not form postbacks
- Emulating `__doPostBack()` would require replicating the entire Web Forms postback protocol, which defeats the purpose of using Blazor

**Alternative:**
Use `@onclick`, `EventCallback<T>`, or form submission with `EditForm`.

### UpdatePanel Async Postback Semantics

**Why not?**
- UpdatePanel enables partial-page updates via AJAX postback
- Blazor components handle updates natively via parameter binding
- A compatibility layer would be complex, fragile, and undermine Blazor's design

**Alternative:**
Use Blazor component parameters, `@bind`, and `EventCallback` for interactive updates.

### Automatic Form Validation Conversion

**Why not?**
- Web Forms validators are declarative controls with complex state management
- Blazor validation is based on data annotations, which are independent of the component model
- Conversion would require semantic analysis of validator configurations and cannot be automated reliably

**Alternative:**
Manually rewrite validators as data annotations on your model classes.

### `ScriptManager` Full API Surface

**Why not?**
- Only a few `ScriptManager` methods are commonly used; most are framework internals
- Each method has a different (or no) Blazor equivalent
- A full compatibility wrapper would create maintenance burden with minimal benefit

**Alternative:**
Our Roslyn analyzers (BWFC022, BWFC023, BWFC024) detect problematic patterns and guide you to Blazor equivalents.

---

## 11. Analyzers and CLI Transforms

To help automate migration detection, BWFC provides three diagnostic rules:

### BWFC022: PageClientScript Usage Analyzer

Detects `Page.ClientScript` usage and suggests patterns for each method call.

**Example:**
```csharp
// ⚠️ BWFC022: Page.ClientScript is not available in Blazor.
// Migration path depends on the pattern:
// - If RegisterStartupScript(): Use OnAfterRenderAsync(IJSRuntime) with firstRender guard.
// - If RegisterClientScriptInclude(): Add <script> tag to layout or import via JS.InvokeAsync().
// - If GetPostBackEventReference(): Use @onclick or EventCallback<T> instead.

Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "script");
```

See [BWFC022 Reference](../Analyzers/BWFC022.md) for details.

### BWFC023: IPostBackEventHandler Usage Analyzer

Detects `IPostBackEventHandler` implementation and suggests `EventCallback<T>`.

**Example:**
```csharp
// ⚠️ BWFC023: IPostBackEventHandler is not available in Blazor.
// Use EventCallback<T> for event handling instead.

public class MyControl : BaseWebFormsComponent, IPostBackEventHandler
{
    public void RaisePostBackEvent(string eventArgument) { }
}
```

See [BWFC023 Reference](../Analyzers/BWFC023.md) for details.

### BWFC024: ScriptManager Code-Behind Usage Analyzer

Detects `ScriptManager.GetCurrent()` and method calls like `SetFocus()`, `RegisterAsyncPostBackControl()`.

**Example:**
```csharp
// ⚠️ BWFC024: ScriptManager.GetCurrent() and related methods are not available in Blazor.
// SetFocus: Use JavaScript interop with element @ref.
// RegisterAsyncPostBackControl: Blazor does not use UpdatePanel postback model — use component binding instead.

ScriptManager.GetCurrent(Page).SetFocus(txtSearch);
```

See [BWFC024 Reference](../Analyzers/BWFC024.md) for details.

---

## 12. Real-World Examples

### Example 1: jQuery Plugin Initialization

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        Page.ClientScript.RegisterClientScriptInclude(
            "jqueryui",
            ResolveUrl("~/lib/jquery-ui/jquery-ui.min.js"));
        
        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            "initDatepicker",
            "$(function() { $('#txtDate').datepicker(); });",
            true);
    }
}
```

**Blazor:**
=== "app.js"
    ```javascript
    export function initializeDatepicker() {
        $('#txtDate').datepicker();
    }
    ```

=== "MyComponent.razor"
    ```razor
    @inject IJSRuntime JS
    
    <input @ref="dateInput" id="txtDate" type="text" />
    
    @code {
        private ElementReference dateInput;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./app.js");
                await module.InvokeVoidAsync("initializeDatepicker");
            }
        }
    }
    ```

HTML layout must include jQuery UI:
```html
<script src="lib/jquery/jquery.min.js"></script>
<script src="lib/jquery-ui/jquery-ui.min.js"></script>
```

### Example 2: Dynamic Data Grid with Inline Editing

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // Include script for inline editing
        Page.ClientScript.RegisterClientScriptInclude(
            "grideditor",
            ResolveUrl("~/lib/grid-editor.js"));
    }
}

public class GridData
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

**Blazor:**
```razor
@page "/data-grid"
@inject HttpClient Http

<GridView Data="@items" OnRowSelected="HandleRowSelected">
    <GridViewColumn Binding="@(x => x.Id)" Header="ID" />
    <GridViewColumn Binding="@(x => x.Name)" Header="Name" />
</GridView>

<button @onclick="Refresh">Refresh</button>

@code {
    private List<GridData> items;
    
    protected override async Task OnInitializedAsync()
    {
        items = await Http.GetFromJsonAsync<List<GridData>>("/api/data");
    }
    
    private async Task HandleRowSelected(int id)
    {
        // Update data directly, no __doPostBack needed
        var item = items.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            item.Name = await PromptForNewName();
            await UpdateItemAsync(item);
        }
    }
    
    private async Task Refresh()
    {
        items = await Http.GetFromJsonAsync<List<GridData>>("/api/data");
    }
}

public class GridData
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### Example 3: Form with Custom Validation and Theme Toggle

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // Validation scripts
        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            "validate",
            "window.validateForm = function() { return $('#form').valid(); };",
            true);
        
        // Theme toggle
        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            "theme",
            "$(function() { applyUserTheme(); });",
            true);
    }
}

protected void btnSubmit_Click(object sender, EventArgs e)
{
    if (!Page.IsValid) return;
    
    // Process
}
```

**Blazor:**
```razor
@page "/form"
@inject IJSRuntime JS

<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <div class="form-group">
        <label>Name:</label>
        <InputText @bind-Value="model.Name" />
        <ValidationMessage For="() => model.Name" />
    </div>
    
    <div class="form-group">
        <label>Email:</label>
        <InputText @bind-Value="model.Email" />
        <ValidationMessage For="() => model.Email" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
    <button type="button" @onclick="ToggleTheme" class="btn btn-secondary">Toggle Theme</button>
</EditForm>

@code {
    private FormModel model = new();
    private IJSObjectReference? module;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./app.js");
            await module.InvokeVoidAsync("applyUserTheme");
        }
    }
    
    private async Task HandleSubmit()
    {
        // Only called if validation passes (DataAnnotationsValidator)
        await SaveFormAsync();
    }
    
    private async Task ToggleTheme()
    {
        if (module is not null)
        {
            await module.InvokeVoidAsync("toggleTheme");
        }
    }
}

public class FormModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
}
```

---

## Summary

| Web Forms | Blazor | Learn More |
|-----------|--------|------------|
| `RegisterStartupScript()` | `OnAfterRenderAsync(IJSRuntime)` | Section 1 |
| `RegisterClientScriptInclude()` | `<script src="">` in layout | Section 2 |
| `RegisterClientScriptBlock()` | JS module + import | Section 3 |
| `GetPostBackEventReference()` | `@onclick` or `EventCallback<T>` | Section 4 |
| Form validation with `Page.IsValid` | `EditForm` + `DataAnnotationsValidator` | Section 5 |
| `IPostBackEventHandler` | `EventCallback<T>` | Section 6 |
| `ScriptManager.SetFocus()` | `@ref` + `JS.InvokeVoidAsync()` | Section 7 |
| `ScriptManager` other methods | Remove (Blazor handles natively) | Section 7 |

**Next Steps:**
1. Review the [Analyzer Reference Pages](../Analyzers/BWFC022.md) to understand diagnostic messages
2. Check the [Roslyn Analyzers](Analyzers.md) documentation for CLI integration
3. Explore the [Live Samples](https://blazorwebformscomponents.azurewebsites.net) to see ClientScript patterns in action
4. Review the [IJSRuntime Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability) for advanced scenarios

---

**Last Updated:** 2026-07-30  
**Status:** Complete  
**Component:** Beast (Technical Writer)
