# Adding EventCallback Aliases to BWFC Components

## When to Use
When adding new EventCallback parameters to a Blazor component that emulates a Web Forms control. Every event needs BOTH a bare name (`ItemCommand`) and an On-prefix (`OnItemCommand`) as independent `[Parameter]` auto-properties.

## Checklist

### 1. Create the EventArgs class (if needed)
- File: `src/BlazorWebFormsComponents/{ControlName}{EventName}EventArgs.cs`
- Namespace: `BlazorWebFormsComponents`
- Inherit from `System.EventArgs`
- Include `public object Sender { get; set; }` for BWFC convention
- Follow existing naming: `GridViewRowEventArgs`, `DataListCommandEventArgs`, `RepeaterItemEventArgs`

### 2. Add both parameter properties
```csharp
/// <summary>
/// Occurs when {description}.
/// </summary>
[Parameter]
public EventCallback<MyEventArgs> EventName { get; set; }

/// <summary>
/// Web Forms migration alias for EventName.
/// </summary>
[Parameter]
public EventCallback<MyEventArgs> OnEventName { get; set; }
```

**Critical:** BOTH must be independent auto-properties. Never delegate one to the other.

### 3. Coalesce at invocation site
```csharp
var handler = EventName.HasDelegate ? EventName : OnEventName;
await handler.InvokeAsync(args);
```

### 4. Update HasDelegate guard checks
If the component uses `.HasDelegate` to conditionally render UI:
```csharp
bool ShowSomething => EventName.HasDelegate || OnEventName.HasDelegate;
```

## Gotchas

### Method-name collision
If the component already has an internal method with the same name as the new parameter (e.g., DataList had a `ItemDataBound()` method), rename the method to `{Name}Internal()`. **Never rename the parameter** — it must match Web Forms markup.

### Cross-component references
If another component directly accesses the EventCallback property (e.g., `ButtonField` accessing `GridView.OnRowCommand`), update it to coalesce both names.

### Past vs. present tense EventArgs
Web Forms distinguishes `-ing` (before) from `-ed` (after) events with different EventArgs types:
- `FormViewInsertEventArgs` → for `ItemInserting` (present/before)
- `FormViewInsertedEventArgs` → for `ItemInserted` (past/after)

Don't mix them up. The `-ed` version typically has `AffectedRows`, `Exception`, `ExceptionHandled`.

## Reference Files
- `GridView.razor.cs` — comprehensive example with 10+ event pairs
- `FormView.razor.cs` — CRUD events with bare + On-prefix aliases
- `DataList.razor.cs` — method-rename pattern (`ItemDataBoundInternal`)
- `.ai-team/skills/blazor-parameter-aliases/SKILL.md` — general alias pattern
