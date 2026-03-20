# Code Snippets Package

The BlazorWebFormsComponents Snippets package provides Visual Studio code snippets to accelerate development with the BlazorWebFormsComponents library.

## Overview

This Visual Studio extension includes snippets for:

1. **Static Imports** - Quickly add `@using static` directives for enumeration types
2. **Component Patterns** - Common component markup templates with placeholders
3. **Validation Controls** - Quick templates for validation scenarios

## Installation

### From VSIX File

1. Download the `.vsix` file from the [releases page](https://github.com/FritzAndFriends/BlazorWebFormsComponents/releases)
2. Double-click the `.vsix` file to install
3. Restart Visual Studio

### From Visual Studio Marketplace

1. Open Visual Studio
2. Go to **Extensions** > **Manage Extensions**
3. Search for "Blazor Web Forms Components Snippets"
4. Click **Download** and restart Visual Studio

## Building the Extension

**Note:** VSIX extensions can only be built on Windows with Visual Studio 2022 installed.

### Prerequisites
- Windows OS
- Visual Studio 2022 (any edition)
- Visual Studio SDK (VSSDK) component installed

### Build Steps

#### Using PowerShell Script (Recommended)
```powershell
cd src/BlazorWebFormsComponents.Snippets
.\Build-VSIX.ps1
```

#### Using Visual Studio
1. Open `BlazorMeetsWebForms.sln` in Visual Studio 2022
2. Right-click the `BlazorWebFormsComponents.Snippets` project
3. Select "Build"

The VSIX file will be created in `bin\Release\BlazorWebFormsComponents.Snippets.vsix`.

## Available Snippets

### Static Import Snippets

| Shortcut | Description | Output |
|----------|-------------|--------|
| `bwfall` | All common static imports | All enumeration static imports |
| `bwfrepeat` | RepeatLayout enumeration | `@using static BlazorWebFormsComponents.Enums.RepeatLayout` |
| `bwfdatalist` | DataListEnum enumeration | `@using static BlazorWebFormsComponents.Enums.DataListEnum` |
| `bwfvalidation` | Validation enumerations | Static imports for validation types |

### Component Snippets

| Shortcut | Component | Description |
|----------|-----------|-------------|
| `bwfgridview` | GridView | Basic GridView with columns |
| `bwfdl` | DataList | DataList with ItemTemplate |
| `bwfrepeater` | Repeater | Repeater with ItemTemplate |
| `bwflistview` | ListView | ListView with ItemTemplate |
| `bwfformview` | FormView | FormView with Item and Edit templates |
| `bwfbutton` | Button | Button with click handler |
| `bwftextbox` | TextBox | TextBox with binding |
| `bwfdropdown` | DropDownList | DropDownList with data binding |
| `bwfcheckboxlist` | CheckBoxList | CheckBoxList with data binding |
| `bwfradiolist` | RadioButtonList | RadioButtonList with data binding |

### Validation Snippets

| Shortcut | Control | Description |
|----------|---------|-------------|
| `bwfrequired` | RequiredFieldValidator | Required field validation |
| `bwfvalsummary` | ValidationSummary | Validation summary display |

## Usage Examples

### Example 1: Adding Static Imports

To use enumeration values without full qualification:

1. Type `bwfall` at the top of your `.razor` file
2. Press `Tab` twice
3. All common static imports are added

```razor
@using BlazorWebFormsComponents
@using static BlazorWebFormsComponents.Enums.RepeatLayout
@using static BlazorWebFormsComponents.Enums.DataListEnum
@using static BlazorWebFormsComponents.Enums.ValidationSummaryDisplayMode
@using static BlazorWebFormsComponents.Enums.ButtonType
@using static BlazorWebFormsComponents.Enums.ValidationDataType
@using static BlazorWebFormsComponents.Enums.ValidationCompareOperator
@using static BlazorWebFormsComponents.Enums.BorderStyle
@using static BlazorWebFormsComponents.Enums.HorizontalAlign
@using static BlazorWebFormsComponents.Enums.VerticalAlign
@using static BlazorWebFormsComponents.Enums.TextAlign
```

Now you can use values like `Table`, `Flow`, `Horizontal`, `Vertical` directly:

```razor
<DataList ItemType="Product" DataSource="@products" RepeatLayout="Table" RepeatDirection="Horizontal">
    ...
</DataList>
```

### Example 2: Creating a GridView

1. Type `bwfgridview` where you want the component
2. Press `Tab` twice
3. Fill in the placeholders:
   - First tab stop: Item type (e.g., `Product`)
   - Second tab stop: Data source variable (e.g., `products`)
4. Add more columns as needed

### Example 3: Adding Form Validation

1. Type `bwftextbox` to create a TextBox
2. Type `bwfrequired` to add validation
3. Type `bwfvalsummary` to add a validation summary

```razor
<TextBox @bind-Text="userName" CssClass="form-control" />
<RequiredFieldValidator ControlToValidate="userName" 
                        ErrorMessage="User name is required" 
                        Display="Dynamic" />

<ValidationSummary HeaderText="Please correct the following errors:" 
                   DisplayMode="BulletList" 
                   ShowSummary="true" />
```

## Why Use Static Imports?

BlazorWebFormsComponents uses static classes for enumeration types to provide a more object-oriented approach. Static imports allow you to use these values more concisely:

**Without static import:**
```razor
<DataList RepeatLayout="BlazorWebFormsComponents.Enums.RepeatLayout.Table" />
```

**With static import:**
```razor
@using static BlazorWebFormsComponents.Enums.RepeatLayout

<DataList RepeatLayout="Table" />
```

## Supported Languages

The snippets are available in:
- Razor (`.razor` files)
- C# (`.cs` files, for the static imports)
- HTML (for component markup)

## Requirements

- Visual Studio 2022 or later
- BlazorWebFormsComponents NuGet package in your project

## Contributing

To add new snippets:

1. Create a new `.snippet` file in the `Snippets/` folder
2. Follow the XML structure of existing snippets
3. Add appropriate shortcut, description, and code template
4. Build the project to test

## Structure

```
BlazorWebFormsComponents.Snippets/
├── BlazorWebFormsComponents.Snippets.csproj  # Project file
├── source.extension.vsixmanifest              # VSIX manifest
├── LICENSE.txt                                 # MIT license
├── icon.png                                    # Extension icon
├── README.md                                   # Documentation
└── Snippets/                                   # Snippet files
    ├── snippets.pkgdef                         # VS registration
    ├── AllStaticImports.snippet
    ├── StaticImportRepeatLayout.snippet
    ├── StaticImportDataListEnum.snippet
    ├── StaticImportValidation.snippet
    ├── GridViewComponent.snippet
    ├── DataListComponent.snippet
    ├── RepeaterComponent.snippet
    ├── FormViewComponent.snippet
    ├── ButtonComponent.snippet
    ├── TextBoxComponent.snippet
    ├── CheckBoxListComponent.snippet
    ├── RequiredFieldValidator.snippet
    └── ValidationSummary.snippet
```

## More Information

- [BlazorWebFormsComponents Documentation](https://fritzandfriends.github.io/BlazorWebFormsComponents/)
- [GitHub Repository](https://github.com/FritzAndFriends/BlazorWebFormsComponents)
- [NuGet Package](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)
- [Visual Studio Snippets Reference](https://learn.microsoft.com/en-us/visualstudio/ide/code-snippets)
