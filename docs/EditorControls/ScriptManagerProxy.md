# ScriptManagerProxy

The **ScriptManagerProxy** component is a **migration compatibility stub** that renders no output. In Web Forms, ScriptManagerProxy was used in content pages and user controls to add scripts and services when the main ScriptManager was defined in a master page. In Blazor, this concept does not apply — ScriptManagerProxy exists solely to prevent compilation errors during migration.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.scriptmanagerproxy?view=netframework-4.8

!!! warning "Migration Stub Only"
    This component renders **nothing** to the page. It exists so that migrated Web Forms markup compiles without errors. Once your migration is stable, remove `<ScriptManagerProxy />` from your pages.

## Features Supported in Blazor

No features are actively supported. The component silently accepts its presence in markup for migration compatibility.

### Blazor Notes

- ScriptManagerProxy is a **no-op component** — it renders no HTML and performs no actions
- In Web Forms, this was needed because only one ScriptManager could exist per page, and content pages used ScriptManagerProxy to register additional scripts. In Blazor, there is no such restriction — use `IJSRuntime` anywhere.

## Web Forms Features NOT Supported

- **Script registration** (`Scripts` collection) — Use Blazor's `IJSRuntime`
- **Service references** (`Services` collection) — Use dependency injection and `HttpClient`
- **AuthenticationService, ProfileService, RoleService** — Use ASP.NET Core Identity

## Web Forms Declarative Syntax

```html
<asp:ScriptManagerProxy
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
    <Scripts>
        <asp:ScriptReference Path="~/Scripts/custom.js" />
    </Scripts>
    <Services>
        <asp:ServiceReference Path="~/WebServices/MyService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
```

## Blazor Razor Syntax

```razor
@* Include during migration to prevent compilation errors *@
<ScriptManagerProxy />
```

## HTML Output

ScriptManagerProxy renders **no HTML output**. It is a silent stub.

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Keep ScriptManagerProxy initially** — Include `<ScriptManagerProxy />` so your migrated content pages compile
2. **Remove when stable** — Delete `<ScriptManagerProxy />` during your cleanup pass
3. **Migrate script references** — Replace `<Scripts>` collections with Blazor's `IJSRuntime.InvokeAsync` or `<script>` tags in your host page
4. **Migrate service references** — Replace `<Services>` collections with dependency-injected services

!!! tip "Best Practice"
    ScriptManagerProxy was a workaround for Web Forms' single-ScriptManager-per-page limitation. Blazor has no such limitation — you can call `IJSRuntime` from any component. Remove ScriptManagerProxy as soon as your page compiles without it.

### Before (Web Forms — Content Page)

```html
<%@ Page Title="Dashboard" MasterPageFile="~/Site.Master" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/dashboard.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <asp:Label ID="lblStatus" runat="server" Text="Loading..." />
</asp:Content>
```

### After (Blazor)

```razor
@inject IJSRuntime JS

@* ScriptManagerProxy included for migration, can be removed *@
<ScriptManagerProxy />

<Label Text="@status" />

@code {
    private string status = "Loading...";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("dashboardInit");
        }
    }
}
```

## See Also

- [ScriptManager](ScriptManager.md) - Page-level script management stub (migration compatibility)
- [UpdatePanel](UpdatePanel.md) - Partial rendering wrapper (migration compatibility)
- [JavaScript Setup](../UtilityFeatures/JavaScriptSetup.md) - Blazor JavaScript interop guidance
- [Migration — Getting Started](../Migration/readme.md)
