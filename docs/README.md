**Migrate your ASP.NET Web Forms application to Blazor — without a complete rewrite.**

BlazorWebFormsComponents gives you **52 Blazor components** that match the names, properties, and HTML output of the original ASP.NET Web Forms controls. Drop the `asp:` prefix, add a `@using`, and your existing markup just works. Your CSS stays intact, your layout doesn't break, and your team can migrate page by page instead of all at once.

!!! tip "Built for .NET 9 — supports Static SSR and ServerInteractive rendering modes."

## Quick Start

Install the NuGet package:

```shell
dotnet add package Fritz.BlazorWebFormsComponents
```

Register services in `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
```

Then use components with the same names and attributes you already know:

```html
<!-- Before (Web Forms) -->
<asp:Label ID="lblGreeting" runat="server" Text="Hello, World!" CssClass="header" />
<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />

<!-- After (Blazor) -->
<Label @ref="lblGreeting" Text="Hello, World!" CssClass="header" />
<Button @ref="btnSubmit" Text="Submit" OnClick="btnSubmit_Click" />
```

For full setup details including JavaScript interop and ViewState configuration, see [Service Registration](UtilityFeatures/ServiceRegistration.md).

## Component Coverage — 96% Complete

52 of 54 Web Forms controls are implemented across six categories. Only `Substitution` and `Xml` are [deferred](Migration/DeferredControls.md) — they have no meaningful Blazor equivalent.

| Category | Components | Status |
|----------|:----------:|--------|
| :material-form-textbox: [Editor Controls](EditorControls/Button.md) | 25 / 27 | Label, TextBox, Button, DropDownList, Calendar, FileUpload, and more |
| :material-table: [Data Controls](DataControls/GridView.md) | 9 / 9 | :white_check_mark: GridView, ListView, Repeater, DataGrid, Chart, DetailsView |
| :material-check-decagram: [Validation Controls](ValidationControls/RequiredFieldValidator.md) | 8 / 8 | :white_check_mark: RequiredFieldValidator, CompareValidator, RangeValidator, and more |
| :material-navigation: [Navigation Controls](NavigationControls/Menu.md) | 3 / 3 | :white_check_mark: Menu, TreeView, SiteMapPath |
| :material-account-lock: [Login Controls](LoginControls/Login.md) | 7 / 7 | :white_check_mark: Login, CreateUserWizard, ChangePassword, and more |
| :material-wrench: [Ajax Toolkit Extenders](AjaxToolkit/index.md) | 25+ | Accordion, ModalPopup, CalendarExtender, TabContainer, and more |

## Migration Tooling

BlazorWebFormsComponents isn't just a component library — it includes **automated tooling** to accelerate your migration.

- **PowerShell migration scripts** — Run `bwfc-migrate.ps1` to convert `.aspx` / `.ascx` / `.master` files into `.razor` components automatically. See the [Automated Migration Guide](Migration/AutomatedMigration.md).
- **Roslyn analyzers** (BWFC001–BWFC020+) — Catch migration issues at build time with diagnostics tailored to Web Forms patterns. See [Analyzers](Migration/Analyzers.md).
- **Custom Controls shim layer** — Migrating custom server controls? `WebControl`, `CompositeControl`, `HtmlTextWriter`, and `DataBoundWebControl` base classes let you bring your custom controls forward. See [Custom Controls](Migration/Custom-Controls.md).

## Utility Features

Web Forms wasn't just controls — it was a runtime with conventions your code depends on. BlazorWebFormsComponents provides shims for the patterns that matter most:

- [**ViewState & IsPostBack**](UtilityFeatures/ViewStateAndPostBack.md) — Familiar state management that adapts to Blazor's rendering modes
- [**DataBinder.Eval**](UtilityFeatures/Databinder.md) — Keep your data-binding expressions working
- [**Response.Redirect**](UtilityFeatures/ResponseRedirect.md) — Navigation that feels like Web Forms
- [**NamingContainer & ID Rendering**](UtilityFeatures/NamingContainer.md) — Predictable element IDs for JavaScript and CSS
- [**WebFormsPageBase**](UtilityFeatures/WebFormsPage.md) — A page base class with the lifecycle hooks you expect

## Migration Guides

Planning your migration? Start here:

- :material-rocket-launch: [Getting Started](Migration/readme.md) — Overview and first steps
- :material-strategy: [Migration Strategies](Migration/Strategies.md) — Incremental, page-by-page, and hybrid approaches
- :material-file-document: [Master Pages → Layouts](Migration/MasterPages.md) — Convert your site structure
- :material-account-box: [User Controls → Components](Migration/User-Controls.md) — Reuse your existing UI building blocks
- :material-alert-circle: [Known Fidelity Divergences](MigrationGuides/KnownFidelityDivergences.md) — Where Blazor output differs from Web Forms (and why)

!!! note "A migration bridge, not a greenfield framework"
    BlazorWebFormsComponents is designed for **existing applications** moving from Web Forms to Blazor. Some rendered HTML intentionally matches legacy Web Forms output rather than modern best practices — that's the point. Once migrated, you can incrementally modernize your markup at your own pace.

## Get Involved

BlazorWebFormsComponents is open source and maintained by [Jeff Fritz](https://github.com/csharpfritz) and the community. Contributions, bug reports, and feedback are welcome.

- :fontawesome-brands-github: [GitHub Repository](https://github.com/FritzAndFriends/BlazorWebFormsComponents)
- :material-package-variant: [NuGet Package](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents)
- :material-monitor-dashboard: [Component Health Dashboard](dashboard.md)
