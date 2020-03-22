# BlazorWebFormsComponents

A collection of Blazor components that emulate the web forms components of the same name

[![Build status](https://dev.azure.com/FritzAndFriends/BlazorWebFormsComponents/_apis/build/status/BlazorWebFormsComponents-.NET%20Standard-CI)](https://dev.azure.com/FritzAndFriends/BlazorWebFormsComponents/_build/latest?definitionId=14)  [![Join the chat at https://gitter.im/BlazorWebFormsComponents/community](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/BlazorWebFormsComponents/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Nuget](https://img.shields.io/nuget/v/Fritz.BlazorWebFormsComponents?color=violet)](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fritz.BlazorWebFormsComponents)](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)

[Live Samples running on Azure](https://blazorwebformscomponents.azurewebsites.net)

## Approach + Considerations

We believe that Web Forms applications that have been well maintained and provide value should have a path forward to the new user-interface frameworks with minimal changes.  This is not an application converted nor is it a patch that can be applied to your project that magically makes it work with ASP<span></span>.NET Core.  This repository contains a library and series of strategies that will allow you to re-use much of your markup, much of your business code and help shorten your application re-write process.

This is not for everyone, not everyone needs to migrate their application.  They can continue being supported as Web Forms for a very long time (2029 EOL at the time of this writing) and the applications that are considered for migration to Blazor may be better suited with a complete re-write.  For those applications that need to be migrated, this library should help make that process simpler by providing components with the same names, markup, and functionality as previously available.

[Get started with your migration, steps ahead, and strategy documentation](docs/Migration/readme.md) for various controls and tools used are available.

Portions of the [original .NET Framework](https://github.com/microsoft/referencesource) are contributed to this project under their MIT license.

## Blazor Components for Controls

There are a significant number of controls in ASP.NET Web Forms, and we will focus on creating components in the following order:

  - Editor Controls
	  - [Button](docs/Button.md)
    - LinkButton
  - Data Controls
    - Chart(?)
    - [DataList](docs/DataList.md)
    - DataPager
    - DetailsView
    - FormView
    - [GridView](docs/GridView.md)
    - [ListView](docs/ListView.md)
    - [Repeater](docs/Repeater.md)
  - Validation Controls
    - CompareValidator
    - [CustomValidator](docs/CustomValidator.md)
    - RangeValidator
    - [RegularExpressionValidator](docs/RegularExpressionValidator.md)
    - [RequiredFieldValidator](docs/RequiredFieldValidator.md)
    - [ValidationSummary](docs/ValidationSummary.md)
  - Navigation Controls
    - Menu
    - SiteMapPath
    - [TreeView](docs/TreeView.md)
  - Login Controls
    - ChangePassword
    - Login
    - LoginName
    - LoginStatus
    - LoginView
    - PasswordRecovery

We will NOT be converting any DataSource objects, Wizard components, skins or themes.  Once this first collection of 23 controls is written, we can consider additional features like modern tag formatting.

## Utility Features

There are a handful of features that augment the ASP<span></span>.NET development experience that are made available as part of this project in order to support migration efforts.  Importantly, these features are NOT implemented the same way that they are in Web Forms, but rather have the same API and behave in a proper Blazor fashion.  These features include:

  - [DataBinder](docs/Databinder.md)
  - [ViewState](docs/ViewState.md)

## Compiling the project

There are three different types of .NET projects in this repository:  .NET Framework, .NET Core, and .NET Standard.  The sample projects are in the `/samples` folder, while the unit test project is next to the component library in the `/src` folder.  From the root of the repository, you should be able to execute:

`dotnet restore` to restore packages

`dotnet run --project samples/AfterBlazorServerSide` to start the Blazor Server-Side samples
