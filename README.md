# BlazorWebFormsComponents

A collection of Blazor components that emulate the web forms components of the same name

[![Build status](https://dev.azure.com/FritzAndFriends/BlazorWebFormsComponents/_apis/build/status/BlazorWebFormsComponents-.NET%20Standard-CI)](https://dev.azure.com/FritzAndFriends/BlazorWebFormsComponents/_build/latest?definitionId=14)  

[![Nuget](https://img.shields.io/nuget/v/Fritz.BlazorWebFormsComponents?color=violet)](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fritz.BlazorWebFormsComponents)](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)

## Approach + Considerations

We believe that Web Forms applications that have been well maintained and provide value should have a path forward to the new user-interface frameworks with minimal changes.  This is not an application converted nor is it a patch that can be applied to your project that magically makes it work with ASP<span></span>.NET Core.  This repository contains a library and series of strategies that will allow you to re-use much of your markup, much of your business code and help shorten your application re-write process.

This is not for everyone, not everyone needs to migrate their application.  They can continue being supported as Web Forms for a very long time (2029 EOL at the time of this writing) and the applications that are considered for migration to Blazor may be better suited with a complete re-write.  For those applications that need to be migrated, this library should help make that process simpler by providing components with the same names, markup, and functionality as previously available.

[Strategies for your migration and steps ahead](docs/Migration/README.md) are available as part of this repository.

## Blazor Components for Controls

There are a significant number of controls in ASP.NET Web Forms, and we will focus on creating components in the following order:

  - Data Controls
    - Chart(?)
    - [DataList](docs/DataList.md)
    - DataPager
    - DetailsView
    - FormView
    - GridView
    - [ListView](docs/ListView.md)
    - [Repeater](docs/Repeater.md)
  - Validation Controls
    - CompareValidator
    - CustomValidator
    - RangeValidator
    - RegularExpressionValidator(?)
    - RequiredFieldValidator
    - ValidationSummary
  - Navigation Controls
    - Menu
    - SiteMapPath
    - TreeView
  - Login Controls
    - ChangePassword
    - Login
    - LoginName
    - LoginStatus
    - LoginView
    - PasswordRecovery

We will NOT be converting any DataSource objects, Wizard components, skins or themes.  Once this first collection of 23 controls is written, we can consider additional features like modern tag formatting.
