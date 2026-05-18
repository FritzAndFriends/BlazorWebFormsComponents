# BWFC023: IPostBackEventHandler Usage

**Diagnostic ID:** `BWFC023`  
**Severity:** ⚠️ Warning  
**Category:** Migration  
**Status:** Active

---

## What It Detects

This analyzer warns when you implement `IPostBackEventHandler` — a Web Forms interface for custom controls to raise server events in response to client-side actions.

**Detected patterns:**
- `class MyControl : Control, IPostBackEventHandler`
- `public void RaisePostBackEvent(string eventArgument) { ... }`
- Any implementation of the `IPostBackEventHandler` interface

---

## Example

```csharp
public partial class MyCustomControl : UserControl, IPostBackEventHandler
{
    // ⚠️ BWFC023: IPostBackEventHandler is not available in Blazor.
    // Use EventCallback<T> for event handling instead.
    
    public event EventHandler OnCustomAction;
    
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "action")
        {
            OnCustomAction?.Invoke(this, EventArgs.Empty);
        }
    }
}
```

---

## Why It Matters

`IPostBackEventHandler` is tightly coupled to Web Forms' **postback event model**:

- A client-side event triggers `__doPostBack(controlId, eventArgument)`
- The server receives the POST, decodes the event data, and calls `RaisePostBackEvent()`
- The control can raise server-side events in response

In Blazor:

- **There is no postback cycle** — events are component method calls
- **No `__doPostBack()` mechanism** — client actions invoke C# methods directly
- **`EventCallback<T>` replaces postback events** — provides parent-child component communication

Without updating, your migrated code will have **no way to raise events** to parent components.

---

## How to Fix

Replace `IPostBackEventHandler` with `EventCallback<T>` parameters.

### Simple Case: No Arguments

=== "Web Forms (Before)"
    ```csharp
    public partial class MyControl : UserControl, IPostBackEventHandler
    {
        public event EventHandler OnAction;
        
        public void RaisePostBackEvent(string eventArgument)
        {
            OnAction?.Invoke(this, EventArgs.Empty);
        }
    }
    ```

=== "Blazor (After)"
    ```razor
    @* MyControl.razor *@
    
    <button @onclick="RaiseAction">Click Me</button>
    
    @code {
        [Parameter]
        public EventCallback OnAction { get; set; }
        
        private async Task RaiseAction()
        {
            await OnAction.InvokeAsync();
        }
    }
    
    @* Parent.razor *@
    <MyControl OnAction="@HandleAction" />
    
    @code {
        private async Task HandleAction()
        {
            // Handle the event
        }
    }
    ```

### With Arguments: Single Value

=== "Web Forms (Before)"
    ```csharp
    public partial class MyControl : UserControl, IPostBackEventHandler
    {
        public event EventHandler<ItemSelectedEventArgs> OnItemSelected;
        
        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("select-"))
            {
                string itemId = eventArgument.Substring("select-".Length);
                OnItemSelected?.Invoke(this, new ItemSelectedEventArgs { ItemId = itemId });
            }
        }
    }
    
    public class ItemSelectedEventArgs : EventArgs
    {
        public string ItemId { get; set; }
    }
    ```

=== "Blazor (After)"
    ```razor
    @* MyControl.razor *@
    
    @foreach (var item in Items)
    {
        <button @onclick="@(() => SelectItem(item.Id))">
            @item.Name
        </button>
    }
    
    @code {
        [Parameter]
        public List<Item> Items { get; set; }
        
        [Parameter]
        public EventCallback<string> OnItemSelected { get; set; }
        
        private async Task SelectItem(string itemId)
        {
            await OnItemSelected.InvokeAsync(itemId);
        }
    }
    
    @* Parent.razor *@
    <MyControl Items="@items" OnItemSelected="@HandleItemSelected" />
    
    @code {
        private async Task HandleItemSelected(string itemId)
        {
            // Handle selection
        }
    }
    ```

### With Arguments: Multiple Values (EventArgs Class)

If you need to pass multiple values, use a custom class:

=== "Web Forms (Before)"
    ```csharp
    public partial class GridControl : UserControl, IPostBackEventHandler
    {
        public event EventHandler<RowSelectedEventArgs> OnRowSelected;
        
        public void RaisePostBackEvent(string eventArgument)
        {
            // eventArgument format: "rowId|action"
            var parts = eventArgument.Split('|');
            var args = new RowSelectedEventArgs
            {
                RowId = int.Parse(parts[0]),
                Action = parts[1]
            };
            OnRowSelected?.Invoke(this, args);
        }
    }
    
    public class RowSelectedEventArgs : EventArgs
    {
        public int RowId { get; set; }
        public string Action { get; set; }
    }
    ```

=== "Blazor (After)"
    ```razor
    @* GridControl.razor *@
    
    @foreach (var row in Rows)
    {
        <tr @onclick="@(() => HandleRowClick(row.Id, "edit"))">
            <td>@row.Name</td>
        </tr>
    }
    
    @code {
        [Parameter]
        public List<GridRow> Rows { get; set; }
        
        [Parameter]
        public EventCallback<RowSelectedEventArgs> OnRowSelected { get; set; }
        
        private async Task HandleRowClick(int rowId, string action)
        {
            var args = new RowSelectedEventArgs { RowId = rowId, Action = action };
            await OnRowSelected.InvokeAsync(args);
        }
    }
    
    public class RowSelectedEventArgs
    {
        public int RowId { get; set; }
        public string Action { get; set; }
    }
    
    @* Parent.razor *@
    <GridControl Rows="@rows" OnRowSelected="@HandleRowSelected" />
    
    @code {
        private async Task HandleRowSelected(RowSelectedEventArgs args)
        {
            if (args.Action == "edit")
            {
                await EditRowAsync(args.RowId);
            }
        }
    }
    ```

---

## Key Differences

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| **Event mechanism** | IPostBackEventHandler + `__doPostBack()` | EventCallback<T> |
| **Event declaration** | `event EventHandler OnEvent` | `[Parameter] EventCallback<T> OnEvent` |
| **Event invocation** | `OnEvent?.Invoke(...)` in `RaisePostBackEvent()` | `await OnEvent.InvokeAsync(...)` |
| **Data passing** | Custom EventArgs classes (optional) | Parameter type `T` (any type) |
| **Parent binding** | Automatic via postback | Explicit `OnEvent="@handler"` parameter |

---

## Real-World Example: Custom Picker Control

=== "Web Forms (Before)"
    ```csharp
    public partial class DatePickerControl : UserControl, IPostBackEventHandler
    {
        public event EventHandler<DatePickedEventArgs> OnDatePicked;
        
        public void RaisePostBackEvent(string eventArgument)
        {
            if (DateTime.TryParse(eventArgument, out var date))
            {
                OnDatePicked?.Invoke(this, new DatePickedEventArgs { SelectedDate = date });
            }
        }
    }
    
    public class DatePickedEventArgs : EventArgs
    {
        public DateTime SelectedDate { get; set; }
    }
    ```

=== "Blazor (After)"
    ```razor
    @* DatePickerControl.razor *@
    
    <input type="date" @onchange="@HandleDateChange" />
    
    @code {
        [Parameter]
        public EventCallback<DateTime> OnDatePicked { get; set; }
        
        private async Task HandleDateChange(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out var date))
            {
                await OnDatePicked.InvokeAsync(date);
            }
        }
    }
    
    @* Parent.razor *@
    <DatePickerControl OnDatePicked="@HandleDatePicked" />
    
    @code {
        private async Task HandleDatePicked(DateTime date)
        {
            SelectedDate = date;
        }
    }
    ```

---

## Common Mistakes

### ❌ Don't: Forget `await` When Invoking

```csharp
// ❌ WRONG: Not awaiting the callback
private async Task SelectItem(string itemId)
{
    OnItemSelected.InvokeAsync(itemId);  // Missing await!
}
```

### ✅ Do: Always `await` EventCallback Invocations

```csharp
// ✅ CORRECT: Properly awaiting
private async Task SelectItem(string itemId)
{
    await OnItemSelected.InvokeAsync(itemId);
}
```

### ❌ Don't: Check if Callback is Null

```csharp
// ❌ WRONG: EventCallback<T> is never null
if (OnItemSelected != null)
{
    await OnItemSelected.InvokeAsync(itemId);
}
```

### ✅ Do: Just Invoke (EventCallback Handles Null)

```csharp
// ✅ CORRECT: EventCallback<T> is safe to invoke directly
await OnItemSelected.InvokeAsync(itemId);
```

---

## Related Analyzers

- **[BWFC022](BWFC022.md)** — Page.ClientScript usage (see **ClientScriptShim** for easy migration)
- **[BWFC024](BWFC024.md)** — ScriptManager code-behind usage

---

## Configuration

To suppress this warning for a specific line:

```csharp
#pragma warning disable BWFC023
public void RaisePostBackEvent(string eventArgument) { }
#pragma warning restore BWFC023
```

Or in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.BWFC023.severity = silent
```

---

## See Also

- 📖 [ClientScriptMigrationGuide.md](../Migration/ClientScriptMigrationGuide.md) — Section 6: IPostBackEventHandler
- 📖 [EventCallback Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/event-handling) — Blazor event handling
- 📖 [Component Parameters](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/index) — Parameter binding in Blazor

---

**Status:** ✅ Active  
**Last Updated:** 2026-07-30  
**Owner:** Beast (Technical Writer)
