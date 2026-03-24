## What is BlazorWebFormsComponents?

**BlazorWebFormsComponents** is a comprehensive library of Blazor components designed to seamlessly emulate ASP.NET Web Forms controls. This enables developers to migrate existing Web Forms applications to modern Blazor with minimal markup changes.

Simply remove the `asp:` prefix and `runat="server"` attribute, and your familiar Web Forms markup works in Blazor. The library preserves the same control names, properties, and HTML output — ensuring your CSS, JavaScript, and application logic require minimal updates.

## Who This Is For

This library is specifically designed for **brown-field application developers** maintaining established ASP.NET Web Forms applications. If your application:

- Provides value and deserves continued support
- Cannot justify a complete rewrite
- Has well-maintained, organized code
- Needs a migration path to Blazor without total replacement

...then BlazorWebFormsComponents provides the bridge between Web Forms and the modern Blazor framework.

!!! note "Not for new applications"
    This library prioritizes **compatibility** with Web Forms over modern web standards. Some rendered HTML may follow older patterns to ensure drop-in compatibility. For new Blazor projects, use native Blazor components instead.

## Quick Start

### Installation

1. **Install the NuGet package:**
   ```bash
   dotnet add package Fritz.BlazorWebFormsComponents
   ```

2. **Register services in your `Program.cs`:**
   ```csharp
   var builder = WebApplicationBuilder.CreateBuilder(args);
   builder.Services.AddBlazorWebFormsComponents();
   builder.RootComponents.Add<App>("#app");
   ```

3. **Add imports to `_Imports.razor`:**
   ```csharp
   @using BlazorWebFormsComponents
   @using BlazorWebFormsComponents.Validations
   @using static BlazorWebFormsComponents.WebColor
   ```

4. **Start using components** — just remove the `asp:` prefix:
   ```razor
   @* Web Forms: <asp:Button ID="btnSave" Text="Save" runat="server" /> *@
   <Button Text="Save" OnClick="HandleSave" />

   <TextBox @bind-Text="myValue" />
   <RequiredFieldValidator ControlToValidate="..." Text="Required" />
   ```

## Component Overview

The library includes **7 major categories** of Web Forms controls:

- **[Editor Controls](EditorControls/Button.md)** — Input elements (TextBox, Button, CheckBox, Panel, etc.)
- **[Data Controls](DataControls/GridView.md)** — Tabular data display (GridView, Repeater, DataList, etc.)
- **[Validation Controls](ValidationControls/RequiredFieldValidator.md)** — Form validation (RequiredFieldValidator, RangeValidator, etc.)
- **[Navigation Controls](NavigationControls/Menu.md)** — Menu and tree controls
- **[Login Controls](LoginControls/Login.md)** — Authentication UI (Login, ChangePassword, etc.)
- **[AJAX Controls](EditorControls/UpdatePanel.md)** — Asynchronous updates
- **[Ajax Control Toolkit Extenders](AjaxToolkit/index.md)** — Rich interactive components

Check the **[Component Health Dashboard](dashboard.md)** to see implementation status for each control.

## Migration Resources

- **[Getting Started with Migration](Migration/readme.md)** — Step-by-step migration strategy and planning
- **[Migration Strategies](Migration/Strategies.md)** — Detailed guidance for custom controls, data sources, and patterns
- **[Automated Migration Tools](Migration/AutomatedMigration.md)** — Roslyn analyzers and automation helpers
- **[Implementation Status](../status.md)** — Track progress across all components

## Sample Application

Visit the **[Live Sample Site](https://blazorwebformscomponents.azurewebsites.net)** to see all components in action with interactive examples, code snippets, and a searchable catalog.

## Key Features

✅ **Same Names, Same Markup** — Use the exact same control names and properties
✅ **Identical HTML Output** — CSS and JavaScript integrations work unchanged
✅ **Event Bubbling** — Command events bubble through container components
✅ **Data Binding** — Familiar `@bind` syntax for two-way binding
✅ **Validation Integration** — Form validation with ValidationGroup support
✅ **Utility Helpers** — DataBinder, ViewState, ID rendering, and more

## What You Need to Know

This library prioritizes **compatibility** over modern best practices. Some behaviors and HTML structures follow Web Forms patterns for consistency. Once your projects are running on the new version of .NET, you can consider adding modern coding techniques to improve performance and code readibility.

The library supports **.NET 6+** as both a **Razor Class Library** and **static server-side rendering (SSR)** for Blazor Server and Blazor Web applications.

## See Also

- [Component List](../README.md#blazor-components-for-controls) — Full catalog of available components
- [Utility Features](UtilityFeatures/PageService.md) — Helpers like DataBinder, ViewState, and Page.IsPostBack
- [Custom Controls Migration](Migration/Custom-Controls.md) — Migrate your custom Web Forms controls
- [API Reference](AjaxToolkit/index.md) — Detailed property and event documentation
