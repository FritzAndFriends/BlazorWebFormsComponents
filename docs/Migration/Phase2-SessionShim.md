# Session State Migration

The `SessionShim` allows migrated Web Forms code to access `Session["key"]` without modification. Your existing session access code — shopping carts, user preferences, wizard state — works unchanged in Blazor.

## Overview

**What it does:**
- Provides a `Session` indexer (`Session["key"]`) backed by ASP.NET Core `ISession` with automatic JSON serialization
- Falls back to an in-memory dictionary in interactive (SignalR) mode where HTTP session isn't available
- Supports type-safe access via `Session.Get<T>("key")` for explicit deserialization
- Registers automatically via `AddBlazorWebFormsComponents()`

**Why it matters:**
Web Forms applications use `Session["key"]` everywhere — shopping carts, user preferences, wizard step tracking, temporary form data. Without a shim, every `Session` access would need to be rewritten to use Blazor's state management patterns. The `SessionShim` eliminates that refactoring during migration, letting you focus on UI conversion first.

## Before and After

=== "Web Forms (Original)"
    ```csharp
    // ShoppingCart.aspx.cs
    protected void Page_Load(object sender, EventArgs e)
    {
        // Store cart ID in session
        if (Session["CartId"] == null)
        {
            Session["CartId"] = Guid.NewGuid().ToString();
        }

        var cartId = (string)Session["CartId"];
        LoadCart(cartId);
    }

    protected void AddToCart_Click(object sender, EventArgs e)
    {
        // Track item count
        int count = Session["ItemCount"] != null 
            ? (int)Session["ItemCount"] : 0;
        Session["ItemCount"] = count + 1;
    }

    protected void SetPreference_Click(object sender, EventArgs e)
    {
        // Store user preferences
        Session["Theme"] = "dark";
        Session["Language"] = "en-US";
    }
    ```

=== "Blazor with BWFC (Same Code Works!)"
    ```csharp
    // ShoppingCart.razor.cs
    protected void Page_Load()
    {
        // Same session access — SessionShim handles it
        if (Session["CartId"] == null)
        {
            Session["CartId"] = Guid.NewGuid().ToString();
        }

        var cartId = (string)Session["CartId"];
        LoadCart(cartId);
    }

    protected void AddToCart_Click()
    {
        // Same code — SessionShim serializes/deserializes automatically
        int count = Session["ItemCount"] != null 
            ? (int)Session["ItemCount"] : 0;
        Session["ItemCount"] = count + 1;
    }

    protected void SetPreference_Click()
    {
        // Same code
        Session["Theme"] = "dark";
        Session["Language"] = "en-US";
    }
    ```

**Key difference:** The `Session` indexer is provided by `SessionShim` instead of `HttpContext.Session`. Your code doesn't need to know the difference.

## Setup

### Automatic Registration

The `SessionShim` is registered automatically when you call `AddBlazorWebFormsComponents()`:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();
app.Run();
```

No additional setup needed for interactive (SignalR) mode — the shim uses in-memory storage automatically.

### SSR Session Persistence

If your Blazor app uses Server-Side Rendering (SSR) and you need session data to persist across HTTP requests, add the session middleware:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddSession();  // Enable ASP.NET Core session

var app = builder.Build();

app.UseSession();  // Add session middleware to the pipeline
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

!!! note
    `app.UseSession()` must be placed **before** `app.UseRouting()` in the middleware pipeline.

## How It Works

The `SessionShim` operates in two modes depending on the Blazor hosting model:

### SSR Mode (HTTP Context Available)

When an `HttpContext` with an active session is available, the shim wraps ASP.NET Core's `ISession`:

```
Session["CartId"] = "abc-123"
         │
         ▼
SessionShim.SetItem("CartId", "abc-123")
         │
         ▼
JSON.Serialize("abc-123") → ISession.SetString("CartId", json)
         │
         ▼
Stored in ASP.NET Core distributed session (cookies, Redis, SQL, etc.)
```

### Interactive Mode (SignalR Circuit)

When no HTTP session is available (interactive Blazor Server over SignalR), the shim falls back to an in-memory dictionary scoped to the current circuit:

```
Session["CartId"] = "abc-123"
         │
         ▼
SessionShim.SetItem("CartId", "abc-123")
         │
         ▼
Stored in ConcurrentDictionary<string, object> (per-circuit memory)
```

### JSON Serialization

All values are serialized to JSON when stored and deserialized when retrieved. This means:

- Primitive types (`string`, `int`, `bool`) work transparently
- Complex objects must be JSON-serializable
- The cast syntax `(string)Session["key"]` works because the shim deserializes back to the original type

## Type-Safe Access

For explicit type control, use the generic `Get<T>()` method:

```csharp
// Store a value
Session["ItemCount"] = 42;

// Retrieve with explicit type (recommended)
int count = Session.Get<int>("ItemCount");

// Retrieve with cast (also works)
int count = (int)Session["ItemCount"];

// Complex objects
Session["UserPrefs"] = new UserPreferences { Theme = "dark", Locale = "en-US" };
var prefs = Session.Get<UserPreferences>("UserPrefs");
```

!!! tip
    Prefer `Session.Get<T>()` over casting when the stored type might be ambiguous. JSON deserialization of numeric types can return `long` instead of `int`, and `Get<T>()` handles the conversion correctly.

## Limitations and Known Issues

| Feature | Behavior | Notes |
|---------|----------|-------|
| Interactive mode storage | In-memory (per-circuit) | Data is lost when the circuit disconnects |
| Cross-tab sharing | ❌ Not supported in interactive mode | Each SignalR circuit has its own session dictionary |
| Session timeout | Follows ASP.NET Core session config (SSR) or circuit lifetime (interactive) | Configure via `builder.Services.AddSession(options => ...)` |
| Non-serializable objects | ❌ Throws on store | Objects must be JSON-serializable |
| `Session.Abandon()` | Clears all keys | Does not destroy the underlying ASP.NET Core session in SSR mode |

### Interactive Mode Caveats

In interactive Blazor Server mode (SignalR), session state is **per-circuit**:

- Opening the same page in two browser tabs creates two separate session stores
- Refreshing the page creates a new circuit, losing in-memory session data
- Session data does not survive server restarts

!!! warning
    If your Web Forms app relied on session sharing across browser tabs or windows, you will need to migrate to a shared state solution (e.g., database-backed state, `ProtectedBrowserStorage`, or a distributed cache) in a later phase.

## Troubleshooting

### Session values are `null` after page refresh (Interactive mode)

This is expected in interactive mode — refreshing the page creates a new SignalR circuit with empty in-memory storage. To persist session data across refreshes:

1. Enable SSR session persistence (see [Setup](#ssr-session-persistence) above)
2. Or migrate to `ProtectedBrowserStorage` for client-side persistence

### "Object not serializable" exception

The `SessionShim` uses JSON serialization. Ensure your stored objects:
- Have parameterless constructors
- Have public properties (not fields)
- Don't contain circular references

## Summary

The `SessionShim`:
- ✅ Lets existing `Session["key"]` code work unchanged
- ✅ Wraps ASP.NET Core `ISession` with JSON serialization in SSR mode
- ✅ Falls back to in-memory dictionary in interactive mode
- ✅ Supports type-safe access via `Session.Get<T>("key")`
- ✅ Registers automatically via `AddBlazorWebFormsComponents()`
- ❌ Interactive mode is per-circuit (not shared across tabs)
- ❌ In-memory storage is lost on circuit disconnect or page refresh

Use it for **Phase 2 migrations** when your code uses `Session` extensively. For long-term state management, consider migrating to Blazor-native patterns like `ProtectedBrowserStorage`, cascading parameters, or a state management service.

See [ViewState and PostBack Shim](../UtilityFeatures/ViewStateAndPostBack.md) for related state management patterns.
