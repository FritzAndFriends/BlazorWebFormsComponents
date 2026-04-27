Migration might not be the correct term for this process, it could appear to be more of a rewrite using Blazor. In this article, you will learn how to get started rewriting your Web Forms application using Blazor with the Blazor Web Forms Components package.

The output project from this operation should be a **.NET 10 Blazor Web App using static server-side rendering (SSR)**. BWFC migrations target SSR by default because it preserves the Web Forms request/response model — including `HttpContext`, cookies, redirects, and form posts — while still giving you a clean path to add interactivity only where it is truly needed.

## Step 0 - Acknowledgement

The first step is a step of acknowledgement.  This process is not 100% and is not guaranteed to deliver a Blazor application without some amount of rewriting.  Applications are written in many different ways, and the tools provided here are attempting to get your project *CLOSE* to Blazor so that you have to rewrite as little code as possible.

## Migration Philosophy: The Strangler Fig Pattern

BWFC is built around the **Strangler Fig migration pattern** — the practice of incrementally replacing a legacy system while keeping it running in parallel, rather than attempting a complete rewrite all at once.

Instead of taking your Web Forms app offline for weeks to rewrite everything, the Strangler Fig approach lets you:

- **Migrate one page or feature at a time** — without stopping the legacy app
- **Keep both running in parallel** — with traffic routing between them
- **Use zero-rewrite shims** — ClientScriptShim, SessionShim, and others let code work unchanged
- **Modernize at your own pace** — refactor shims to native Blazor patterns on your schedule

For a detailed guide on how to apply this pattern, see the **[Strangler Fig Pattern documentation](StranglerFigPattern.md)**.

## Phase 1: Just Make It Compile (Compilation Shims)

Before diving into deep architectural changes, BWFC provides **compilation shims** that allow migrated code to compile unchanged. This phase focuses on making your Web Forms code work in a Blazor project with minimal refactoring.

### Phase 1 Features

The BWFC migration toolkit includes:

| Feature | What It Does | More Info |
|---------|-------------|-----------|
| **ConfigurationManager Shim** | Migrated BLL/DAL code accesses `ConfigurationManager.AppSettings["key"]` and `ConnectionStrings["name"]` unchanged | [Configuration Manager Guide](Phase1-ConfigurationManager.md) |
| **App_Start Stubs** | `BundleConfig` and `RouteConfig` files compile as-is; bundle/route processing happens in Blazor alternatives | [App_Start Stubs Guide](Phase1-AppStartStubs.md) |
| **L1 Automated Script** | Mechanical transforms: `web.config` → `appsettings.json`, `.aspx` → `.razor`, `IsPostBack` guard unwrapping, selective `using` retention | [Automated Migration Guide](AutomatedMigration.md) |

### Phase 1 Workflow

1. **Scan** your Web Forms app: `bwfc-scan.ps1` inventories controls and readiness
2. **Run L1 Script**: `bwfc-migrate.ps1` performs mechanical transforms and web.config extraction
3. **Enable Shims**: Call `AddBlazorWebFormsComponents()` in `Program.cs`
4. **Compile**: Your migrated project compiles with minimal code changes
5. **Plan Phase 2**: Identify which shims to replace with Blazor-native patterns

### When to Move to Phase 2

Phase 1 gets your code **compiling**. Plan Phase 2 migration when you're ready to:

- Replace `ConfigurationManager` with dependency injection
- Replace bundling stubs with Blazor CSS/JS Isolation
- Implement proper routing with `@page` directives
- Add proper configuration providers (Azure Key Vault, etc.)

See [Automated Migration Guide](AutomatedMigration.md) for next steps.

## Step 1 - Readiness

There are good application architectures and there are not-so-good application architectures to be considered for migration to Blazor.  We've written another document to help you evaluate the [readiness of your application for migration](migration_readiness.md).  It is recommended you read through that documentation to understand what makes an application better prepared for migration to Blazor.

## Step 2 - Migrate Business Logic to .NET Standard

.NET Standard is the new recommended way to package and reuse business logic across projects and .NET runtimes.  We recommend you migrate:
  - Any class libraries referenced
  - Any classes in your web project that do NOT directly communicate with the web request or response

There is a separate [strategy document](NET-Standard.md) with instructions to migrate your code to .NET Standard libraries.  The goal of the exercise is to place all of your business logic into .NET Standard 2.0 libraries.  This version of .NET Standard will allow you to reference the libraries in both your existing Web Forms application and in your new Blazor application.

**A side benefit**: this is a good architecture practice that should allow you to test your business logic independently from your web project.  Try starting a unit test project with xUnit, NUnit or MSTest to exercise some of your business logic.  You will be able to run your tests either in the Visual Studio Test Runner or at the command line using `dotnet test`

## Step 3 - Create a new .NET 10 Blazor SSR Project

Create your new **.NET 10 Blazor Web App configured for static SSR**. The `webforms-to-blazor` CLI and `bwfc-migrate.ps1` scaffolds already target this setup automatically.

>> ADD IMAGES

If you are creating the host manually, choose the .NET 10 Blazor Web App template and keep the app in static SSR mode as your migration baseline. Do **not** start from global interactive server mode.

Add a NuGet reference to the BlazorWebFormsComponents package on the command-line as follows:

`dotnet add package Fritz.BlazorWebFormsComponents`

Next, add references to the projects converted to .NET Standard in the previous step.  In Visual Studio, follow these steps:

>> ADD IMAGES

On the command-line you can add references using the dotnet CLI tools in your .NET 10 Blazor project folder with syntax like:

`dotnet add reference ../MyLibrary`

For more details, see the [official .NET documentation for adding references](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference).

Next, add a using statement in the existing `./_Imports.razor` file for the BlazorWebFormsComponents:

`@using BlazorWebFormsComponents`

This will allow you to reference the components from the library directly.  Without this statement you would have to create tags that look like `<BlazorWebFormsComponents.ListView` and that's just ugly.  We want the simpler `<ListView` syntax that matches the markup used in ASP<span></span>.NET

## Step 4 - Master Pages

Master Pages in Web Forms are a combination of two concepts in Blazor: the app shell and a Blazor layout. In a .NET 10 Blazor SSR app, `Components/App.razor` defines the document shell and hosts your static CSS and JavaScript references.

You will want to place any CSS or JavaScript references from your MasterPages into this host page.  The rest of your MasterPage layout inside the `BODY` tag will need to be migrated to a layout razor file.  In the simplest scenario where you have one MasterPage with *ONLY* HTML content and it has one main content area, you will want to overwrite the content in `Shared/MainLayout.razor`

The default layout for your application will be defined in the `App.razor` file in a `RouteView` element as follows:

```xml
<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

You can add additional rules in this file to route and choose different layouts as appropriate for your application.

For more complex scenarios and a walk-through with samples, read the [MasterPages strategy documentation](MasterPages.md).

[Back to top](#step-0-acknowledgement)

## Step 5 - User Controls

User Controls in Web Forms are identified by their `.ascx` extension and inclusion in other controls or pages.  These controls most closely resemble Blazor components:

 - Mixture of HTML and code
 - Inline functions in file with markup
 - Potential 'code-behind' partial class with more coded logic

Components have no direct reference to the structure and contents of the parent Page or MasterPage.  If your controls rely on this capability of Web Forms, you will want to instead pass around a `CascadingValue` that your controls can receive as a `CascadingParameter`.

Components also do not have ViewState.  We are [considering implementing a ViewState-like object to help with conversion](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/93).  If you were storing the state of things in `ControlState` or `ViewState` we recommend allocating a class-level field or parameter to store those values.

We recommend copying your HTML and code into a `YOURCOMPONENT.razor` file with the same name and place it in the same folder structure under a `Components` folder as was located in your web forms project.  You can omit the `<% control` directive at the top of the file.  You should then migrate any methods to a `YOURCOMPONENT.razor.cs` partial class.

References to `<asp:` components can be easily edited to use the matching components with these steps:

[More information and advanced techniques can be found in the User Controls strategy document](User-Controls.md)

## Step 5.5 - JavaScript Setup (Optional)

BlazorWebFormsComponents requires a small JavaScript file to support features like `OnClientClick`. The easiest way to include it is to register the service in your `Program.cs`:

```csharp
builder.Services.AddBlazorWebFormsComponents();
```

This automatically loads the JavaScript module when components render - no manual script tags needed!

Alternatively, you can add the script manually to your layout:

```html
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

For more options and details, see the [JavaScript Setup documentation](../UtilityFeatures/JavaScriptSetup.md).

## Step 5.6 - Data-Bound Controls and Template Context

When migrating data-bound controls like `<asp:Repeater>`, `<asp:DataList>`, `<asp:GridView>`, `<asp:ListView>`, and `<asp:FormView>`, you'll encounter a key difference in how templates access the current data item.

### Web Forms vs. Blazor Context

In ASP.NET Web Forms, when using strongly-typed data-binding with the `ItemType` attribute, templates access the current item using the `Item` property. In Blazor, templated components use a context variable.

**Blazor's Default Behavior:**
By default, Blazor uses `@context` as the variable name inside templates:

```razor
<Repeater Items="widgets" ItemType="Widget">
    <ItemTemplate>@context.Name</ItemTemplate>
</Repeater>
```

**Web Forms Compatibility:**
For better compatibility with Web Forms conventions and to reduce migration friction, **use the `Context="Item"` attribute** on data-bound components. This allows you to use `@Item` (similar to Web Forms' `Item` property):

```razor
<Repeater Items="widgets" ItemType="Widget" Context="Item">
    <ItemTemplate>@Item.Name</ItemTemplate>
</Repeater>
```

This pattern applies to all templated data controls:
- `<Repeater>` - ItemTemplate, AlternatingItemTemplate, HeaderTemplate, FooterTemplate
- `<DataList>` - ItemTemplate, AlternatingItemTemplate, HeaderTemplate, FooterTemplate
- `<ListView>` - ItemTemplate, AlternatingItemTemplate, LayoutTemplate, GroupTemplate
- `<FormView>` - ItemTemplate, EditItemTemplate, InsertItemTemplate
- `<GridView>` with `<TemplateField>` - ItemTemplate

**Note:** While it's one extra attribute per component, using `Context="Item"` eliminates the need to learn Blazor-specific `@context` syntax and makes your templates more readable for developers familiar with Web Forms.

## Common Properties on All Styled Controls

All BlazorWebFormsComponents controls that inherit from `BaseStyledComponent` support a set of common properties that match ASP.NET Web Forms' `WebControl` base class. This means properties like `CssClass`, `BackColor`, `ForeColor`, `Font`, `Width`, `Height`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Enabled`, `Visible`, and **`ToolTip`** are available on every styled control — just as they are in Web Forms.

!!! tip "ToolTip renders as the HTML `title` attribute"
    The `ToolTip` property renders as the standard HTML `title` attribute, matching Web Forms behavior. This means browser-native tooltip display works automatically.

```razor
<Button Text="Save" ToolTip="Click to save your changes" />
<TextBox @bind-Text="name" ToolTip="Enter your full name" />
<Label Text="Status" ToolTip="Current processing status" />
```

You do **not** need to add `ToolTip` support per-control — it is inherited from the shared base and works on all styled controls including Button, TextBox, Label, Panel, CheckBox, DropDownList, GridView, and more.

## Step 6 - Pages

Page migration is a process very similar to UserControls, except working on files with the `.aspx` extension.

The page starts with a directive to connect the markup to .NET  code like the following:

```html
<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms._Default" %>
```

These features defined here will be migrated / removed as follows:
  - `Title` is not used in Blazor.  [We're looking into a work-around](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/100)
  - `Language` is not used - everything on the razor template is C#
  - `MasterPageFile` is now a Layout and defined by default in the `App.razor` file.  You can override it by adding a `@layout MyLayout` directive at the top of the file
  - `AutoEventWireup` is not used in Blazor
  - `CodeBehind` is not used in Blazor
  - `Inherits` is converted to an `@inherits` directive that specifies the base class that the Blazor component should inherit from, typically `ComponentBase`.

Additionally, pages are defined with a route in Blazor.  You will need to add a page directive to the top of the `.razor` file with syntax to declare what URL segment this page is listening on:

```
@page "/Home"
```

See the official Blazor documentation for more information on the [Page directive](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-3.1&tabs=visual-studio#custom-routes) and how to handle routing.

## Step 7 - Custom Controls

At this time, these controls will need to be re-written to target the ComponentBase class instead.  We have an [issue opened to look into making migration of these classes easier](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/92), but it is targeted for later in the project.

More information and strategy for re-writing these controls can be found in the [custom controls strategy document](Custom-Controls.md).

## Step X - Convert inline Visual Basic

Use the tool from Telerik at: https://converter.telerik.com/

## Follow-up: Clean Up with Roslyn Analyzers

After migrating your markup and code-behind, install the **BlazorWebFormsComponents.Analyzers** NuGet package to catch leftover Web Forms patterns in your C# code:

```shell
dotnet add package BlazorWebFormsComponents.Analyzers
```

The analyzer suite includes 8 rules that detect `ViewState` usage, `IsPostBack` checks, `Response.Redirect()` calls, `Session` access, missing `[Parameter]` attributes, and more. Each diagnostic comes with an automatic code fix. See the [Roslyn Analyzers guide](Analyzers.md) for full details.

## Follow-up: Move components to Razor Component Library
