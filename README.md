# BlazorWebFormsComponents

A collection of Blazor components that emulate the web forms components of the same name

[![Build and Test](https://github.com/FritzAndFriends/BlazorWebFormsComponents/actions/workflows/build.yml/badge.svg)](https://github.com/FritzAndFriends/BlazorWebFormsComponents/actions/workflows/build.yml)  [![Integration Tests](https://github.com/FritzAndFriends/BlazorWebFormsComponents/actions/workflows/integration-tests.yml/badge.svg)](https://github.com/FritzAndFriends/BlazorWebFormsComponents/actions/workflows/integration-tests.yml)  [![Join the chat at https://gitter.im/BlazorWebFormsComponents/community](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/BlazorWebFormsComponents/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)  [![docs](https://github.com/FritzAndFriends/BlazorWebFormsComponents/workflows/docs/badge.svg)](https://fritzandfriends.github.io/BlazorWebFormsComponents/)

[![Nuget](https://img.shields.io/nuget/v/Fritz.BlazorWebFormsComponents?color=violet)](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fritz.BlazorWebFormsComponents)](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)  [![Live Sample](https://img.shields.io/badge/-Live%20Sample-purple)](https://blazorwebformscomponents.azurewebsites.net)

[Live Samples running on Azure](https://blazorwebformscomponents.azurewebsites.net)

## Approach + Considerations

We believe that Web Forms applications that have been well maintained and provide value should have a path forward to the new user-interface frameworks with minimal changes.  This is not an application converted nor is it a patch that can be applied to your project that magically makes it work with ASP<span></span>.NET Core.  This repository contains a library and series of strategies that will allow you to re-use much of your markup, much of your business code and help shorten your application re-write process.

This is not for everyone, not everyone needs to migrate their application.  They can continue being supported as Web Forms for a very long time (2029 EOL at the time of this writing) and the applications that are considered for migration to Blazor may be better suited with a complete re-write.  For those applications that need to be migrated, this library should help make that process simpler by providing components with the same names, markup, and functionality as previously available.

[Documentation is available online](https://fritzandfriends.github.io/BlazorWebFormsComponents/).  [Get started with your migration, steps ahead, and strategy documentation](docs/Migration/readme.md) for various controls and tools used are available.  Live versions of these components are availalbe on the [Live Samples website](https://blazorwebformscomponents.azurewebsites.net)

Portions of the [original .NET Framework](https://github.com/microsoft/referencesource) are contributed to this project under their MIT license.

## Blazor Components for Controls

There are a significant number of controls in ASP.NET Web Forms, and we will focus on creating components in the following order:

  - Editor Controls
    - [AdRotator](docs/EditorControls/AdRotator.md)
    - [BulletedList](docs/EditorControls/BulletedList.md)
    - [Button](docs/EditorControls/Button.md)
    - Calendar
    - [CheckBox](docs/EditorControls/CheckBox.md)
    - [CheckBoxList](docs/EditorControls/CheckBoxList.md)
    - [DropDownList](docs/EditorControls/DropDownList.md)
    - [FileUpload](docs/EditorControls/FileUpload.md)
    - [HiddenField](docs/EditorControls/HiddenField.md)
    - [Image](docs/EditorControls/Image.md)
    - [ImageButton](docs/EditorControls/ImageButton.md)
    - ImageMap
    - [Label](docs/EditorControls/Label.md)
    - [LinkButton](docs/EditorControls/LinkButton.md)
    - [ListBox](docs/EditorControls/ListBox.md)
    - [Literal](docs/EditorControls/Literal.md)
    - Localize
    - MultiView
    - [Panel](docs/EditorControls/Panel.md)
    - [PlaceHolder](docs/EditorControls/PlaceHolder.md)
    - [RadioButton](docs/EditorControls/RadioButton.md)
    - [RadioButtonList](docs/EditorControls/RadioButtonList.md)
    - [Table](docs/EditorControls/Table.md)
    - [TextBox](docs/EditorControls/TextBox.md)
    - View
    - Xml
  - Data Controls
    - Chart(?)
    - [DataGrid](docs/DataControls/DataGrid.md)
    - [DataList](docs/DataControls/DataList.md)
    - [DataPager](docs/DataControls/DataPager.md)
    - DetailsView
    - [FormView](docs/DataControls/FormView.md)
    - [GridView](docs/DataControls/GridView.md)
    - [ListView](docs/DataControls/ListView.md)
    - [Repeater](docs/DataControls/Repeater.md)
  - Validation Controls
    - [CompareValidator](docs/ValidationControls/CompareValidator.md)
    - [CustomValidator](docs/ValidationControls/CustomValidator.md)
    - [RangeValidator](docs/ValidationControls/RangeValidator.md)
    - [RegularExpressionValidator](docs/ValidationControls/RegularExpressionValidator.md)
    - [RequiredFieldValidator](docs/ValidationControls/RequiredFieldValidator.md)
    - [ValidationSummary](docs/ValidationControls/ValidationSummary.md)
  - Navigation Controls
    - [HyperLink](docs/NavigationControls/HyperLink.md)
    - [Menu](docs/NavigationControls/Menu.md)
    - [SiteMapPath](docs/NavigationControls/SiteMapPath.md)
    - [TreeView](docs/NavigationControls/TreeView.md)
  - Login Controls
    - ChangePassword
    - CreateUserWizard
    - [Login](docs/LoginControls/Login.md)
    - [LoginName](docs/LoginControls/LoginName.md)
    - [LoginStatus](docs/LoginControls/LoginStatus.md)
    - [LoginView](docs/LoginControls/LoginView.md)
    - PasswordRecovery

We will NOT be converting any DataSource objects (SqlDataSource, ObjectDataSource, EntityDataSource, LinqDataSource, XmlDataSource, SiteMapDataSource, AccessDataSource), Wizard components, skins or themes.  Once this first collection of controls is written, we can consider additional features like modern tag formatting.

## Utility Features

There are a handful of features that augment the ASP<span></span>.NET development experience that are made available as part of this project in order to support migration efforts.  Importantly, these features are NOT implemented the same way that they are in Web Forms, but rather have the same API and behave in a proper Blazor fashion.  These features include:

  - [DataBinder](docs/UtilityFeatures/Databinder.md)
  - [ID Rendering](docs/UtilityFeatures/IDRendering.md) - Render HTML IDs for JavaScript integration
  - [JavaScript Setup](docs/UtilityFeatures/JavaScriptSetup.md) - Options for auto-loading required JavaScript
  - [ViewState](docs/UtilityFeatures/ViewState.md)

### Custom Control Migration Support

For applications with custom controls inheriting from `WebControl` or `CompositeControl`, the library provides:

- **Adapter Classes**: `WebControl`, `CompositeControl`, and `HtmlTextWriter` classes that enable minimal-change migration
  - Base attributes (ID, CssClass, Style) are automatically applied - no manual intervention needed
- **Roslyn Analyzers**: The `BlazorWebFormsComponents.Analyzers` NuGet package provides automated code analysis and fixes:
  - **BWFC001**: Detects public properties missing `[Parameter]` attributes with automatic fix

See the [Custom Controls Migration Guide](docs/Migration/Custom-Controls.md) for details.

## Compiling the project

There are three different types of .NET projects in this repository:  .NET Framework, .NET Core, and .NET Standard.  The sample projects are in the `/samples` folder, while the unit test project is next to the component library in the `/src` folder.  From the root of the repository, you should be able to execute:

`dotnet restore` to restore packages

`dotnet run --project samples/AfterBlazorServerSide` to start the Blazor Server-Side samples

## Testing

The project includes two types of tests:

### Unit Tests
Unit tests for the component library are located in `src/BlazorWebFormsComponents.Test/` and use xUnit with bUnit for component testing.

Run unit tests with:
```bash
dotnet test src/BlazorWebFormsComponents.Test
```

### Integration Tests
Integration tests using Playwright validate the sample application pages. These tests are located in `samples/AfterBlazorServerSide.Tests/`.

To run integration tests locally:
1. Install Playwright browsers (first time only):
   ```bash
   pwsh samples/AfterBlazorServerSide.Tests/bin/Debug/net10.0/playwright.ps1 install
   ```
2. Run the tests:
   ```bash
   dotnet test samples/AfterBlazorServerSide.Tests
   ```

See [samples/AfterBlazorServerSide.Tests/README.md](samples/AfterBlazorServerSide.Tests/README.md) for more details.

## Releasing

This project uses automated scripts with Nerdbank.GitVersioning to help with version publishing and release management. See the [scripts/README.md](scripts/README.md) for detailed documentation on:

- **prepare-release.sh** - Uses `nbgv` to update version.json and generates release notes
- **generate-release-notes.sh** - Creates formatted release notes from git commits
- **publish-release.sh** - Uses `nbgv tag` to create and publish release tags

Quick workflow:
1. On `dev` branch: `./scripts/prepare-release.sh 0.14`
2. Merge `dev` to `main`
3. On `main` branch: `./scripts/publish-release.sh`

The NuGet package is automatically published via GitHub Actions when a version tag is pushed.
