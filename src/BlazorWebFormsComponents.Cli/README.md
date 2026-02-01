# WebForms to Blazor CLI Tool

A command-line tool to convert ASP.NET Web Forms user controls (.ascx) to Blazor Razor components (.razor).

## Installation

### As a Global Tool

Install the tool globally using the .NET CLI:

```bash
dotnet tool install --global Fritz.WebFormsToBlazor.Cli
```

Or install from source:

```bash
cd src/BlazorWebFormsComponents.Cli
dotnet pack
dotnet tool install --global --add-source ./nupkg Fritz.WebFormsToBlazor.Cli
```

### As a Local Tool

Add to your project's local tool manifest:

```bash
dotnet tool install Fritz.WebFormsToBlazor.Cli
```

## Usage

### Basic Usage

Convert a single .ascx file:

```bash
webforms-to-blazor --input path/to/MyControl.ascx
```

Convert all .ascx files in a directory:

```bash
webforms-to-blazor --input path/to/controls/
```

### Options

- `-i, --input <path>` (Required): Path to .ascx file or directory containing .ascx files
- `-o, --output <path>`: Output directory for converted files (defaults to input directory)
- `-r, --recursive`: Process all .ascx files in subdirectories
- `-f, --overwrite`: Overwrite existing .razor files without prompting
- `--use-ai`: Enable AI-assisted conversion (requires OPENAI_API_KEY or GITHUB_TOKEN environment variable)

### Examples

Convert all controls in a directory recursively:

```bash
webforms-to-blazor -i ./UserControls -r -f
```

Convert to a different output directory:

```bash
webforms-to-blazor -i ./WebFormsControls -o ./BlazorComponents -r
```

Use AI assistance for better conversion (future feature):

```bash
export OPENAI_API_KEY="your-api-key"
webforms-to-blazor -i ./UserControls --use-ai
```

## What Gets Converted

The tool performs the following conversions:

### Control Directive
```html
<!-- Before -->
<%@ Control Language="C#" CodeBehind="MyControl.ascx.cs" Inherits="MyNamespace.MyControl" %>

<!-- After -->
@inherits MyNamespace.MyControl
```

### ASP.NET Server Controls
```html
<!-- Before -->
<asp:Button ID="btnSubmit" Text="Submit" runat="server" />

<!-- After -->
<Button ID="btnSubmit" Text="Submit" />
```

### Expression Syntax
```html
<!-- Before -->
<%: Model.Name %>
<%= GetValue() %>

<!-- After -->
@(Model.Name)
@(GetValue())
```

### Common Template Issues
The tool fixes the common `Item` vs `context` issue in Blazor templates:

```html
<!-- Before -->
Item.Name

<!-- After -->
context.Name
```

## Manual Steps After Conversion

After using this tool, you may need to:

1. **Update Event Handlers**: Convert Web Forms event handlers to Blazor syntax
   ```html
   <!-- Web Forms -->
   <asp:Button OnClick="Submit_Click" />
   
   <!-- Blazor -->
   <Button OnClick="HandleSubmit" />
   ```

2. **Update Data Binding**: Convert Web Forms data binding to Blazor's `@bind` syntax
   ```html
   <!-- Web Forms -->
   <asp:TextBox Text='<%# Bind("Name") %>' />
   
   <!-- Blazor -->
   <input @bind="Model.Name" />
   ```

3. **Add Using Directives**: Add necessary `@using` directives at the top of the file

4. **Code-Behind Files**: The tool converts markup only. You'll need to manually convert code-behind (.ascx.cs) files to Razor component code-behind (.razor.cs) or `@code` blocks.

## Limitations

- Code-behind (.ascx.cs) files are not automatically converted
- Complex data binding expressions may need manual adjustment
- ViewState references will need to be replaced with component state
- Postback logic needs to be rewritten using Blazor's event model

## Future Enhancements

- AI-powered conversion using GitHub Copilot SDK (coming soon)
- Code-behind file conversion
- Advanced data binding translation
- Interactive mode with preview

## Contributing

This tool is part of the BlazorWebFormsComponents project. Contributions are welcome!

See the main repository for contribution guidelines: [BlazorWebFormsComponents](https://github.com/FritzAndFriends/BlazorWebFormsComponents)

## License

MIT License - See LICENSE file in the repository root
