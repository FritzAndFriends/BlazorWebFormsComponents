# Web Forms Application Migration Readiness

This document is intended as a primer for prospective teams to review in preparation for migrating their Web Forms applications to Server-Side Blazor.  We believe the migration is a multi-step process and this application review and preparation is the first step in migrating / rewriting an application using Blazor.

## Definitions

Throughout this document, we will refer to several framework and technology terms.  This section will clarify those terms.

- **Web Forms**
  - The ASP<span></span>.NET user-interface technology that started with the release in 2002 and is typically identified by applications with web pages that have a `.ASPX` file-extension
- **MVC**
  - Model-View-Controller architecture introduced in 2009 as an alternative user-interface framework for ASP<span></span>.NET developers.  A project structure that uses the MVC framework typically has 3 or 4 folders in it named `Areas`, `Models`, `Views`, `Controllers`
- **Razor Views**
  - The template technology uses by modern ASP<span></span>.NET MVC and ASP<span></span>.NET Core applications.  Typically, these files are found in the `Views` folder and have a `.cshtml` file-extension

## High-Level Requirements

- ASPX in-line Visual Basic not supported on Razor components in .NET Core
  - Convert to C# or move out to a class library
- No data source controls on ASPX pages
- Business logic needs to be convertible to .NET Core
- Class libraries referenced need to be convertible to .NET Standard
- No 3rd party control libraries that don't have a shim for conversion (none available at the time of this writing)

## Application architecture suggestions

- Prefer model-binding techniques over handling Form life-cycle events like `Init`, `Load`, `PreRender`, and `Unload`
  - These event-handlers do not exist and function the same way in Blazor.  Avoid acting outside of the `Load` event.  The actions you are taking in `Load` can be executed at the conclusion of the `Initialize` event in Blazor
- Use a repository pattern with interfaces declared for the repositories
  - Inject the concrete implementation of the data access technology.  It _MAY_ change to HTTP access at some point in the future
- Minimal code embedded in ASPX files - this code will need to be updated to work with the components, where they were using full-featured controls.
- No calls through the control hierarchy.  E.g. `FindControl()`
- Any hybrid applications that contains both MVC and Web Forms user interface content will take some extra work to be converted, as the MVC components will need to be converted to ASP<span></span>.NET Core
- `System.Confuguration.ConfigurationManager` access will need to be rewritten to use `Microsoft.Extensions.Configuration.IConfiguration` objects that are injected
  - Push configuration access to a repository class object that can be migrated to the new Configuration model in .NET Core
- `HttpContext.Current` access will need to be reevaluated.  Direct `HttpContext` access should be avoided in Blazor
- Custom components will need to be rewritten
- User controls `.ASCX` files should be converted to Blazor components
- HttpHandlers and HttpModules will need to be rewritten as ASP<span></span>.NET Core middleware
- MasterPages can be converted to Layout components
- Mobile device detection is not available as part of Blazor.  `Site.mobile.master` will not be directly used by the framework.
  - Instead of using an alternate rendering strategy for mobile, we recommend you embrace an adaptive rendering strategy to ensure that mobile device visitors to your application get a good experience
