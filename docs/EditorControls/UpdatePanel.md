# UpdatePanel

The **UpdatePanel** component wraps content in a `<div>` or `<span>` element, emulating the Web Forms UpdatePanel that enabled partial-page updates via AJAX postbacks. In Blazor, **all rendering is already partial and incremental** via SignalR — UpdatePanel serves as a structural wrapper that preserves HTML output for CSS and JavaScript compatibility.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.updatepanel?view=netframework-4.8

## Features Supported in Blazor

- **ChildContent** — Content rendered inside the wrapper element
- **UpdateMode** — Accepted for migration compatibility (`Always`, `Conditional`)
- **ChildrenAsTriggers** — Accepted for migration compatibility
- **RenderMode** — Controls whether the wrapper renders as a `<div>` (Block) or `<span>` (Inline)

### Blazor Notes

- UpdatePanel renders as a **simple wrapper element** — `<div>` by default, or `<span>` when `RenderMode` is set to `Inline`
- The `UpdateMode` and `ChildrenAsTriggers` properties are accepted but have no behavioral effect — Blazor's component model handles all incremental rendering automatically
- Triggers and conditional updates are handled by Blazor's component lifecycle, not by UpdatePanel

## Web Forms Features NOT Supported

- **Partial postback coordination** — Not applicable; Blazor re-renders incrementally by default
- **Triggers collection** (`<Triggers>`, `AsyncPostBackTrigger`, `PostBackTrigger`) — Not applicable; use Blazor event handlers
- **ContentTemplate as a separate element** — Use `ChildContent` directly between tags
- **ScriptManager dependency** — Not needed; Blazor manages updates natively

## Web Forms Declarative Syntax

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel
    ChildrenAsTriggers="True|False"
    EnableViewState="True|False"
    ID="string"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    RenderMode="Block|Inline"
    runat="server"
    UpdateMode="Always|Conditional"
    Visible="True|False"
>
    <ContentTemplate>
        <!-- Content here -->
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
```

## Blazor Razor Syntax

### Basic UpdatePanel

```razor
<UpdatePanel>
    <p>This content is wrapped in a div.</p>
</UpdatePanel>
```

### Inline Render Mode

```razor
<UpdatePanel RenderMode="UpdatePanelRenderMode.Inline">
    <span>This content is wrapped in a span.</span>
</UpdatePanel>
```

### With Properties (Migration)

```razor
<UpdatePanel UpdateMode="UpdatePanelUpdateMode.Conditional"
             ChildrenAsTriggers="true">
    <Label Text="@message" />
    <Button Text="Update" OnClick="HandleClick" />
</UpdatePanel>

@code {
    private string message = "Click the button to update.";

    void HandleClick()
    {
        message = $"Updated at {DateTime.Now:HH:mm:ss}";
    }
}
```

## HTML Output

### Block Mode (Default)

**Blazor:**
```razor
<UpdatePanel>
    <p>Content here</p>
</UpdatePanel>
```

**Rendered HTML:**
```html
<div>
    <p>Content here</p>
</div>
```

### Inline Mode

**Blazor:**
```razor
<UpdatePanel RenderMode="UpdatePanelRenderMode.Inline">
    Status: OK
</UpdatePanel>
```

**Rendered HTML:**
```html
<span>
    Status: OK
</span>
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove ScriptManager** — UpdatePanel no longer requires a ScriptManager on the page
2. **Remove `<ContentTemplate>` wrapper** — Place content directly between `<UpdatePanel>` tags
3. **Remove `<Triggers>` section** — Blazor event handlers replace trigger-based updates
4. **Remove `asp:` prefix and `runat="server"`** — Standard Blazor migration
5. **Keep UpdatePanel for HTML structure** — If your CSS or JavaScript targets the wrapper `<div>`/`<span>`, keeping UpdatePanel preserves that structure

!!! note "Key Difference"
    In Web Forms, UpdatePanel was essential for AJAX partial-page updates — without it, every server interaction caused a full page reload. In Blazor, **every component re-render is already a partial update** via SignalR. UpdatePanel in Blazor is purely a structural wrapper that preserves HTML output compatibility.

### Before (Web Forms)

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel ID="UpdatePanel1" runat="server"
                 UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Label ID="lblStatus" runat="server" Text="Ready" />
        <asp:Button ID="btnRefresh" runat="server"
                    Text="Refresh"
                    OnClick="btnRefresh_Click" />
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnRefresh"
                                  EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
```

```csharp
protected void btnRefresh_Click(object sender, EventArgs e)
{
    lblStatus.Text = $"Refreshed at {DateTime.Now:HH:mm:ss}";
}
```

### After (Blazor)

```razor
<UpdatePanel UpdateMode="UpdatePanelUpdateMode.Conditional">
    <Label Text="@status" />
    <Button Text="Refresh" OnClick="HandleRefresh" />
</UpdatePanel>

@code {
    private string status = "Ready";

    void HandleRefresh()
    {
        status = $"Refreshed at {DateTime.Now:HH:mm:ss}";
    }
}
```

## Examples

### Multiple Update Regions

```razor
<UpdatePanel>
    <h3>Section A</h3>
    <Label Text="@sectionA" />
    <Button Text="Update A" OnClick="UpdateA" />
</UpdatePanel>

<UpdatePanel>
    <h3>Section B</h3>
    <Label Text="@sectionB" />
    <Button Text="Update B" OnClick="UpdateB" />
</UpdatePanel>

@code {
    private string sectionA = "Initial A";
    private string sectionB = "Initial B";

    void UpdateA() => sectionA = $"A updated at {DateTime.Now:HH:mm:ss}";
    void UpdateB() => sectionB = $"B updated at {DateTime.Now:HH:mm:ss}";
}
```

### Inline UpdatePanel in Text

```razor
<p>
    The current status is:
    <UpdatePanel RenderMode="UpdatePanelRenderMode.Inline">
        <Label Text="@status" />
    </UpdatePanel>
</p>
```

## See Also

- [UpdateProgress](UpdateProgress.md) - Loading indicator during updates
- [ScriptManager](ScriptManager.md) - Script management stub (migration compatibility)
- [Timer](Timer.md) - Interval-based tick events
- [Panel](Panel.md) - General-purpose container control
- [Migration — Getting Started](../Migration/readme.md)
