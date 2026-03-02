# Timer

The **Timer** component performs asynchronous postbacks at a defined interval. In Web Forms, the Timer control worked with ScriptManager to trigger partial-page updates via async postbacks. In Blazor, this component uses `System.Threading.Timer` internally to raise tick events at the specified interval — no ScriptManager dependency is needed.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.timer?view=netframework-4.8

## Features Supported in Blazor

- **Interval** - The number of milliseconds between tick events (default: `60000`)
- **Enabled** - Controls whether the timer is actively firing tick events (default: `true`)
- **OnTick** - Event handler invoked each time the interval elapses

### Blazor Notes

- The Timer component uses `System.Threading.Timer` internally, not JavaScript `setInterval`
- When `Enabled` is set to `false`, the timer stops firing; set it back to `true` to resume
- The timer automatically disposes when the component is removed from the render tree
- No ScriptManager or UpdatePanel is required — Blazor's component model handles UI updates natively

## Web Forms Features NOT Supported

- **ScriptManager dependency** - Not applicable; Blazor does not use ScriptManager
- **Async postback triggers** - Blazor re-renders components automatically when state changes
- **EnableViewState** - Blazor handles state differently; use component state instead
- **OnDataBinding, OnDisposed, OnInit, OnLoad, OnPreRender, OnUnload** - Use Blazor lifecycle methods instead

## Web Forms Declarative Syntax

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:Timer
    Enabled="True|False"
    EnableViewState="True|False"
    ID="string"
    Interval="integer"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnTick="Tick event handler"
    OnUnload="Unload event handler"
    runat="server"
    Visible="True|False"
/>
```

## Blazor Razor Syntax

### Basic Timer

```razor
<Timer Interval="5000" OnTick="HandleTick" />

@code {
    void HandleTick()
    {
        // Called every 5 seconds
    }
}
```

### Timer with Enable/Disable

```razor
<Timer Interval="3000" Enabled="@isRunning" OnTick="HandleTick" />

<Button Text="@(isRunning ? "Stop" : "Start")" OnClick="ToggleTimer" />

@code {
    private bool isRunning = true;

    void ToggleTimer()
    {
        isRunning = !isRunning;
    }

    void HandleTick()
    {
        // Called every 3 seconds when enabled
    }
}
```

## HTML Output

The Timer component renders **no visible HTML output**. It is a purely behavioral component that fires events at the specified interval.

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove ScriptManager** — Timer no longer requires a ScriptManager on the page
2. **Remove UpdatePanel wrappers** — Blazor handles partial rendering automatically
3. **Remove `asp:` prefix and `runat="server"`** — Standard Blazor migration
4. **Update event handler syntax** — Change `OnTick="Timer1_Tick"` to `OnTick="HandleTick"`
5. **Remove `ID`** — Use `@ref` if you need a component reference

!!! note "Key Difference"
    In Web Forms, Timer triggers an async postback that refreshes an UpdatePanel region. In Blazor, the `OnTick` event handler runs server-side and Blazor's SignalR connection automatically pushes UI updates to the browser. The result is the same — periodic UI updates — but the mechanism is fundamentally different and more efficient.

### Before (Web Forms)

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Label ID="lblTime" runat="server" />
        <asp:Timer ID="Timer1" runat="server"
                   Interval="1000"
                   OnTick="Timer1_Tick" />
    </ContentTemplate>
</asp:UpdatePanel>
```

```csharp
protected void Timer1_Tick(object sender, EventArgs e)
{
    lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
}
```

### After (Blazor)

```razor
<Label Text="@currentTime" />
<Timer Interval="1000" OnTick="HandleTick" />

@code {
    private string currentTime = DateTime.Now.ToString("HH:mm:ss");

    void HandleTick()
    {
        currentTime = DateTime.Now.ToString("HH:mm:ss");
    }
}
```

## Examples

### Auto-Refreshing Dashboard

```razor
<Timer Interval="10000" OnTick="RefreshData" />

<GridView DataSource="@orders" AutoGenerateColumns="true" />

@code {
    private List<Order> orders = new();

    protected override void OnInitialized()
    {
        orders = OrderService.GetRecentOrders();
    }

    void RefreshData()
    {
        orders = OrderService.GetRecentOrders();
    }
}
```

### Countdown Timer

```razor
<Label Text="@($"Time remaining: {secondsLeft}s")" />
<Timer Interval="1000" Enabled="@(secondsLeft > 0)" OnTick="Countdown" />

@code {
    private int secondsLeft = 30;

    void Countdown()
    {
        secondsLeft--;
    }
}
```

## See Also

- [UpdatePanel](UpdatePanel.md) - Container for partial-page updates (migration compatibility)
- [ScriptManager](ScriptManager.md) - Script management stub (migration compatibility)
- [Migration — Getting Started](../Migration/readme.md)
