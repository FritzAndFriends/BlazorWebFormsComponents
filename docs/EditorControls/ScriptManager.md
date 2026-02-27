# ScriptManager

The **ScriptManager** component is a **migration compatibility stub** that renders no output. In Web Forms, ScriptManager was a required page-level component that managed JavaScript libraries, partial-page rendering, and web service proxies. In Blazor, all of these concerns are handled natively by the framework — ScriptManager exists solely to prevent compilation errors during migration.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.scriptmanager?view=netframework-4.8

!!! warning "Migration Stub Only"
    This component renders **nothing** to the page. It exists so that migrated Web Forms markup compiles without errors. Once your migration is stable, remove `<ScriptManager />` from your pages.

## Features Supported in Blazor

The following properties are accepted as parameters for markup compatibility but **have no effect**:

- **EnablePartialRendering** — Blazor renders partially by default via its component model
- **EnablePageMethods** — Not applicable; use Blazor component methods directly
- **ScriptMode** — Not applicable; Blazor handles script delivery
- **AsyncPostBackTimeout** — Not applicable; Blazor uses SignalR with its own timeout configuration
- **EnableCdn** — Not applicable; use standard Blazor static asset hosting
- **EnableScriptGlobalization** — Not applicable; use .NET globalization APIs directly
- **EnableScriptLocalization** — Not applicable; use .NET localization APIs directly

### Blazor Notes

- ScriptManager is a **no-op component** — it renders no HTML and performs no actions
- All properties are accepted silently to avoid compilation errors during migration
- Blazor's built-in features replace every function ScriptManager provided

## Web Forms Features NOT Supported

Since this is a stub component, **all features are effectively unsupported**. The properties are accepted but ignored:

- **Script registration** (`RegisterClientScriptBlock`, `RegisterStartupScript`) — Use Blazor's `IJSRuntime` for JavaScript interop
- **Web service proxies** (`ServiceReference`) — Use `HttpClient` or gRPC
- **Partial rendering coordination** — Blazor components re-render automatically
- **Error handling** (`AsyncPostBackError`) — Use Blazor's `ErrorBoundary` component
- **Script combining/bundling** — Use standard ASP.NET Core bundling or a build tool

## Web Forms Declarative Syntax

```html
<asp:ScriptManager
    AsyncPostBackErrorMessage="string"
    AsyncPostBackTimeout="integer"
    EnableCdn="True|False"
    EnablePageMethods="True|False"
    EnablePartialRendering="True|False"
    EnableScriptGlobalization="True|False"
    EnableScriptLocalization="True|False"
    EnableViewState="True|False"
    ID="string"
    OnAsyncPostBackError="AsyncPostBackError event handler"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    runat="server"
    ScriptMode="Auto|Inherit|Debug|Release"
    Visible="True|False"
/>
```

## Blazor Razor Syntax

### Basic Usage (Migration)

```razor
@* Include during migration to prevent compilation errors *@
<ScriptManager />
```

### With Properties (All Ignored)

```razor
@* These properties are accepted but have no effect *@
<ScriptManager EnablePartialRendering="true"
               EnablePageMethods="true"
               ScriptMode="ScriptMode.Release"
               AsyncPostBackTimeout="90" />
```

## HTML Output

ScriptManager renders **no HTML output**. It is a silent stub.

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Keep ScriptManager initially** — Include `<ScriptManager />` in your migrated pages so the markup compiles
2. **Remove when stable** — Once your page is working in Blazor, delete the `<ScriptManager />` tag entirely
3. **Replace script registration** — Move any `ScriptManager.RegisterClientScriptBlock()` or `RegisterStartupScript()` calls to Blazor's `IJSRuntime`
4. **Replace web service calls** — Convert `PageMethods` and `ServiceReference` calls to `HttpClient` or direct Blazor service injection

!!! tip "Best Practice"
    Treat ScriptManager as scaffolding. Include it early in migration to keep pages compiling, then remove it as part of your cleanup pass. A page with `<ScriptManager />` works identically to one without it.

### Before (Web Forms)

```html
<asp:ScriptManager ID="ScriptManager1" runat="server"
                   EnablePartialRendering="true"
                   EnablePageMethods="true" />

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Label ID="lblResult" runat="server" />
        <asp:Button ID="btnLoad" runat="server"
                    Text="Load Data"
                    OnClick="btnLoad_Click" />
    </ContentTemplate>
</asp:UpdatePanel>
```

### After (Blazor)

```razor
@* ScriptManager can be included during migration but is not needed *@
<ScriptManager />

<Label Text="@result" />
<Button Text="Load Data" OnClick="LoadData" />

@code {
    private string result = "";

    void LoadData()
    {
        result = "Data loaded!";
    }
}
```

### After (Blazor — Cleaned Up)

```razor
@* ScriptManager removed — not needed in Blazor *@

<Label Text="@result" />
<Button Text="Load Data" OnClick="LoadData" />

@code {
    private string result = "";

    void LoadData()
    {
        result = "Data loaded!";
    }
}
```

## See Also

- [ScriptManagerProxy](ScriptManagerProxy.md) - Content page stub (migration compatibility)
- [UpdatePanel](UpdatePanel.md) - Partial rendering wrapper (migration compatibility)
- [Timer](Timer.md) - Interval-based tick events
- [JavaScript Setup](../UtilityFeatures/JavaScriptSetup.md) - Blazor JavaScript interop guidance
- [Migration — Getting Started](../Migration/readme.md)
