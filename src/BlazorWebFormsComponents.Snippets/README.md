# BlazorWebFormsComponents Snippets

Visual Studio code snippets for the BlazorWebFormsComponents library.

## Overview

This extension provides a collection of code snippets to make it easier to work with BlazorWebFormsComponents in your Blazor applications. The snippets include:

- Static imports for enumeration types
- Common component patterns
- Validation control templates

## Installation

1. Download the `.vsix` file from the [releases page](https://github.com/FritzAndFriends/BlazorWebFormsComponents/releases)
2. Double-click the `.vsix` file to install it in Visual Studio
3. Restart Visual Studio if it's already running

Alternatively, you can install from within Visual Studio:
1. Go to **Extensions** > **Manage Extensions**
2. Search for "Blazor Web Forms Components Snippets"
3. Click **Download** and restart Visual Studio

## Available Snippets

### Static Import Snippets

These snippets help you add the necessary `@using static` directives to use enumeration types without fully qualifying them.

| Shortcut | Description |
|----------|-------------|
| `bwfall` | Adds all common static imports for BlazorWebFormsComponents enumerations |
| `bwfrepeat` | Adds static import for RepeatLayout enumeration |
| `bwfdatalist` | Adds static import for DataListEnum enumeration |
| `bwfvalidation` | Adds static imports for validation-related enumerations |

### Component Snippets

These snippets create common component markup patterns.

| Shortcut | Description |
|----------|-------------|
| `bwfgridview` | Creates a basic GridView component with columns |
| `bwfdl` | Creates a DataList component with ItemTemplate |
| `bwfrepeater` | Creates a Repeater component with ItemTemplate |
| `bwfformview` | Creates a FormView component with templates |
| `bwfbutton` | Creates a Button component with click handler |
| `bwftextbox` | Creates a TextBox component |
| `bwfcheckboxlist` | Creates a CheckBoxList component |
| `bwfrequired` | Creates a RequiredFieldValidator |
| `bwfvalsummary` | Creates a ValidationSummary component |

## Usage

To use a snippet:

1. In a `.razor` file, type the snippet shortcut (e.g., `bwfall`)
2. Press `Tab` twice to expand the snippet
3. Fill in any placeholders (use `Tab` to move between them)
4. Press `Enter` when done

## Example

To add all static imports:

1. Type `bwfall` at the top of your `.razor` file
2. Press `Tab` twice
3. The following code will be inserted:

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

Now you can use enumeration values like `Table`, `Flow`, `Horizontal`, `Vertical`, etc. directly without qualification.

## Requirements

- Visual Studio 2022 or later
- BlazorWebFormsComponents NuGet package installed in your project

## Contributing

Contributions are welcome! If you have ideas for additional snippets, please open an issue or submit a pull request at:
https://github.com/FritzAndFriends/BlazorWebFormsComponents

## License

MIT License - See LICENSE.txt for details

## More Information

- [BlazorWebFormsComponents Documentation](https://fritzandfriends.github.io/BlazorWebFormsComponents/)
- [GitHub Repository](https://github.com/FritzAndFriends/BlazorWebFormsComponents)
- [NuGet Package](https://www.nuget.org/packages/Fritz.BlazorWebFormsComponents/)
