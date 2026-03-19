# Blazor Parameter Aliases

## When to Use
When a Blazor component needs to support multiple parameter names for the same behavior — typically for Web Forms migration compatibility where event names use an `On` prefix in markup (`OnSorting`) but the component originally defined the property without it (`Sorting`).

## Pattern

### 1. Add the alias as an independent `[Parameter]` property

```csharp
// Existing property (keep as-is)
[Parameter] public EventCallback<GridViewSortEventArgs> Sorting { get; set; }

/// <summary>
/// Web Forms migration alias for Sorting.
/// </summary>
[Parameter] public EventCallback<GridViewSortEventArgs> OnSorting { get; set; }
```

**Critical:** Both properties must be independent auto-properties. Do NOT use a getter/setter that delegates to the other property — Blazor's parameter diffing sets properties by name and won't detect changes through delegation.

### 2. Coalesce at invocation sites

Wherever the component invokes the event internally, prefer the original if it has a delegate, falling back to the alias:

```csharp
var sortingHandler = Sorting.HasDelegate ? Sorting : OnSorting;
await sortingHandler.InvokeAsync(args);
```

### 3. Update HasDelegate guard checks

If the component uses `.HasDelegate` to conditionally show UI (e.g., command columns), check both:

```csharp
internal bool ShowCommandColumn => RowEditing.HasDelegate || OnRowEditing.HasDelegate;
```

## Why This Works

- Blazor's component diffing compares parameters by name. Each `[Parameter]` is set independently.
- If a consumer writes `<GridView OnSorting="Handler">`, Blazor sets the `OnSorting` property. The `Sorting` property remains default (no delegate).
- The coalescing pattern at invocation ensures the event fires regardless of which name the consumer used.
- Both names are non-breaking — existing code using `Sorting` still works; migrated code using `OnSorting` also works.

## Anti-Patterns

❌ **Don't delegate via getter/setter:**
```csharp
// WRONG — Blazor won't detect changes
[Parameter] public EventCallback<T> OnSorting
{
    get => Sorting;
    set => Sorting = value;
}
```

❌ **Don't rename the original property** — that would break existing consumers.

❌ **Don't invoke both callbacks** — the consumer only sets one, so only one should fire.
