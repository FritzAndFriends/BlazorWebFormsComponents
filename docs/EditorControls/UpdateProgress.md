# UpdateProgress

The **UpdateProgress** component displays a loading indicator while an asynchronous operation is in progress. In Web Forms, UpdateProgress was tied to an UpdatePanel and automatically showed/hid based on async postback state. In Blazor, loading states are managed via component state — UpdateProgress provides the familiar markup structure for migration while adapting to Blazor's state-driven rendering model.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.updateprogress?view=netframework-4.8

## Features Supported in Blazor

- **ProgressTemplate** — Child content rendered as the loading indicator
- **AssociatedUpdatePanelID** — Accepted for migration compatibility (string)
- **DisplayAfter** — Accepted for migration compatibility (int, milliseconds)
- **DynamicLayout** — Controls whether the progress indicator reserves space in the layout when hidden (`true` = no space reserved, `false` = space reserved)

### Blazor Notes

- In Blazor, you control visibility of UpdateProgress through component state (e.g., a `bool IsLoading` flag) rather than through automatic association with an UpdatePanel
- The `AssociatedUpdatePanelID` property is accepted but does not automatically detect when an UpdatePanel is updating — use explicit state management instead
- When `DynamicLayout` is `true` (default), the component uses `display:none` when hidden; when `false`, it uses `visibility:hidden` to reserve layout space

## Web Forms Features NOT Supported

- **Automatic UpdatePanel association** — Does not automatically show/hide based on UpdatePanel async postback state; use Blazor component state instead
- **ScriptManager integration** — Not applicable; Blazor does not use ScriptManager

## Web Forms Declarative Syntax

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <!-- Content here -->
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress
    AssociatedUpdatePanelID="string"
    DisplayAfter="integer"
    DynamicLayout="True|False"
    EnableViewState="True|False"
    ID="string"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    runat="server"
    Visible="True|False"
>
    <ProgressTemplate>
        <div>Loading, please wait...</div>
    </ProgressTemplate>
</asp:UpdateProgress>
```

## Blazor Razor Syntax

### Basic Usage

```razor
@if (isLoading)
{
    <UpdateProgress>
        <ProgressTemplate>
            <div>Loading, please wait...</div>
        </ProgressTemplate>
    </UpdateProgress>
}

@code {
    private bool isLoading = false;
}
```

### With Properties (Migration)

```razor
@if (isLoading)
{
    <UpdateProgress AssociatedUpdatePanelID="UpdatePanel1"
                    DisplayAfter="500"
                    DynamicLayout="true">
        <ProgressTemplate>
            <div class="loading-spinner">
                <img src="images/spinner.gif" alt="Loading..." />
                <span>Processing your request...</span>
            </div>
        </ProgressTemplate>
    </UpdateProgress>
}
```

## HTML Output

**Blazor:**
```razor
<UpdateProgress>
    <ProgressTemplate>
        <div>Loading...</div>
    </ProgressTemplate>
</UpdateProgress>
```

**Rendered HTML:**
```html
<div>
    <div>Loading...</div>
</div>
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Replace automatic show/hide with state** — Instead of relying on UpdatePanel association, use a `bool` flag to control when UpdateProgress is visible
2. **Keep `<ProgressTemplate>` markup** — The template content migrates directly
3. **Remove `asp:` prefix and `runat="server"`** — Standard Blazor migration
4. **Wrap in `@if` block** — Use conditional rendering to show/hide the progress indicator

!!! note "Key Difference"
    In Web Forms, UpdateProgress automatically detected when its associated UpdatePanel was performing an async postback and toggled visibility. In Blazor, **you control visibility explicitly** through component state. This gives you more precise control over when loading indicators appear.

### Before (Web Forms)

```html
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:GridView ID="GridView1" runat="server" />
        <asp:Button ID="btnLoad" runat="server"
                    Text="Load Data"
                    OnClick="btnLoad_Click" />
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress ID="UpdateProgress1" runat="server"
                    AssociatedUpdatePanelID="UpdatePanel1"
                    DisplayAfter="300">
    <ProgressTemplate>
        <div class="loading">Loading data...</div>
    </ProgressTemplate>
</asp:UpdateProgress>
```

```csharp
protected void btnLoad_Click(object sender, EventArgs e)
{
    System.Threading.Thread.Sleep(2000); // Simulate slow operation
    GridView1.DataSource = GetData();
    GridView1.DataBind();
}
```

### After (Blazor)

```razor
<UpdatePanel>
    @if (!isLoading)
    {
        <GridView DataSource="@data" AutoGenerateColumns="true" />
    }
    <Button Text="Load Data" OnClick="LoadData" />
</UpdatePanel>

@if (isLoading)
{
    <UpdateProgress AssociatedUpdatePanelID="UpdatePanel1"
                    DisplayAfter="300">
        <ProgressTemplate>
            <div class="loading">Loading data...</div>
        </ProgressTemplate>
    </UpdateProgress>
}

@code {
    private List<DataItem> data = new();
    private bool isLoading = false;

    async Task LoadData()
    {
        isLoading = true;
        StateHasChanged();

        data = await DataService.GetDataAsync();

        isLoading = false;
    }
}
```

## Examples

### Loading Spinner Pattern

```razor
<Button Text="Fetch Results" OnClick="FetchResults" />

@if (isLoading)
{
    <UpdateProgress>
        <ProgressTemplate>
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </ProgressTemplate>
    </UpdateProgress>
}
else
{
    <Label Text="@result" />
}

@code {
    private string result = "";
    private bool isLoading = false;

    async Task FetchResults()
    {
        isLoading = true;
        StateHasChanged();

        result = await SlowService.GetResultAsync();

        isLoading = false;
    }
}
```

### Progress with Custom Styling

```razor
@if (isLoading)
{
    <UpdateProgress DynamicLayout="false">
        <ProgressTemplate>
            <div style="text-align:center; padding:20px;">
                <p>⏳ Please wait while we process your request...</p>
                <progress />
            </div>
        </ProgressTemplate>
    </UpdateProgress>
}
```

## See Also

- [UpdatePanel](UpdatePanel.md) - Container for partial-page updates
- [ScriptManager](ScriptManager.md) - Script management stub (migration compatibility)
- [Timer](Timer.md) - Interval-based tick events
- [Migration — Getting Started](../Migration/readme.md)
