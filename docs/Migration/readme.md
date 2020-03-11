# Migration - Getting Started

Migration might not be the correct term for this process, it could appear to be more of a rewrite using Blazor.  In this article, you will learn how to get started rewriting your Web Forms application using Blazor with the Blazor Web Forms Components package.

The output project from this operation will be a .NET Core 3.1 project running server-side Blazor.  This is the current desired result as this is the supported LTS version of .NET Core and Blazor that was published in December 2019.  With the schedule of .NET releases (as of Feb. 2020), we should expect to update to the next LTS version, .NET 6 between November 2021 and November 2022.

<!-- TOC depthFrom:2 -->

- [Step 0 - Acknowledgement](#step-0---acknowledgement)
- [Step 1 - Readiness](#step-1---readiness)
- [Step 2 - Migrate Business Logic to .NET Standard](#step-2---migrate-business-logic-to-net-standard)
- [Step 3 - Create a new Blazor Server Project](#step-3---create-a-new-blazor-server-project)
- [Step 4 - Master Pages](#step-4---master-pages)
- [Step 5 - User Controls](#step-5---user-controls)
- [Step 6 - Pages](#step-6---pages)
- [Step 7 - Custom Controls](#step-7---custom-controls)
- [Step X - Convert inline Visual Basic](#step-x---convert-inline-visual-basic)
- [Follow-up: Move components to Razor Component Library](#follow-up-move-components-to-razor-component-library)

<!-- /TOC -->

## Step 0 - Acknowledgement

The first step is a step of acknowledgement.  This process is not 100% and is not guaranteed to deliver a Blazor application without some amount of rewriting.  Applications are written in many different ways, and the tools provided here are attempting to get your project *CLOSE* to Blazor so that you have to rewrite as little code as possible.

[Back to top](#Migration---Getting-Started)

## Step 1 - Readiness

There are good application architectures and there are not-so-good application architectures to be considered for migration to Blazor.  We've written another document to help you evaluate the [readiness of your application for migration](migration_readiness.md).  It is recommended you read through that documentation to understand what makes an application better prepared for migration to Blazor.

[Back to top](#Migration---Getting-Started)

## Step 2 - Migrate Business Logic to .NET Standard

.NET Standard is the new recommended way to package and reuse business logic across projects and .NET runtimes.  We recommend you migrate:
  - Any class libraries referenced
  - Any classes in your web project that do NOT directly communicate with the web request or response

There is a separate [strategy document](NET-Standard.md) with instructions to migrate your code to .NET Standard libraries.  The goal of the exercise is to place all of your business logic into .NET Standard 2.0 libraries.  This version of .NET Standard will allow you to reference the libraries in both your existing Web Forms application and in your new Blazor application.

**A side benefit**: this is a good architecture practice that should allow you to test your business logic independently from your web project.  Try starting a unit test project with xUnit, NUnit or MSTest to exercise some of your business logic.  You will be able to run your tests either in the Visual Studio Test Runner or at the command line using `dotnet test`

[Back to top](#Migration---Getting-Started)

## Step 3 - Create a new Blazor Server Project

Create your new Blazor Server-Side project either in Visual Studio 2019, Visual Studio for Mac, or at the command line.  With Visual Studio, follow these steps:

>> ADD IMAGES

At the command-line you can execute the following command to create your Blazor Server-Side project:

`dotnet new blazorserver -f netcoreapp3.1 -o <<DESTINATION FOLDER>>`

Add a NuGet reference to the BlazorWebFormsComponents package on the command-line as follows:

`dotnet add package Fritz.BlazorWebFormsComponents`

Next, add references to the projects converted to .NET Standard in the previous step.  In Visual Studio, follow these steps:

>> ADD IMAGES

On the command-line you can add references using the dotnet CLI tools in your Blazor Server-Side project folder with syntax like:

`dotnet add reference ../MyLibrary`

For more details, see the [official .NET documentation for adding references](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference). 

Next, add a using statement in the existing `./_Imports.razor` file for the BlazorWebFormsComponents:

`@using BlazorWebFormsComponents`

This will allow you to reference the components from the library directly.  Without this statement you would have to create tags that look like `<BlazorWebFormsComponents.ListView` and that's just ugly.  We want the simpler `<ListView` syntax that matches the markup used in ASP<span></span>.NET

[Back to top](#Migration---Getting-Started)

## Step 4 - Master Pages

Master Pages in Web Forms are a combination of two concepts in Blazor: a host page and a Blazor layout.  In Blazor Server-Side the host page is a razor page by default in `Pages/_Host.cshtml` that bootstraps the Blazor application and hosts all static CSS and JavaScript references.

You will want to place any CSS or JavaScript references from your MasterPages into this host page.  The rest of your MasterPage layout will need to be migrated to a layout razor file.  In the simplest scenario where you have one MasterPage with *ONLY* HTML content and it has one main content area, you will want to overwrite the content in `Shared/MainLayout.razor`

For more complex scenarios, read the [MasterPages strategy documentation](Strategy/MasterPages.md).

[Back to top](#Migration---Getting-Started)

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

[Back to top](#Migration---Getting-Started)

## Step 6 - Pages

 - Copy `ASPX` and `ASPX.cs` files into `Pages` folder
 - Rename and convert to `razor` and `razor.cs`

[Back to top](#Migration---Getting-Started)

## Step 7 - Custom Controls

At this time, these controls will need to be re-written to target the ComponentBase class instead.  We have an [issue opened to look into making migration of these classes easier](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/92), but it is targeted for later in the project.

More information and strategy for re-writing these controls can be found in the [custom controls strategy document](Custom-Controls.md).

[Back to top](#Migration---Getting-Started)

## Step X - Convert inline Visual Basic

Use the tool from Telerik at: https://converter.telerik.com/

[Back to top](#Migration---Getting-Started)

## Follow-up: Move components to Razor Component Library

[Back to top](#Migration---Getting-Started)


