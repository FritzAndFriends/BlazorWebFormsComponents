# BlazorWebFormsComponents.Analyzers

Roslyn analyzers and code fixes to help migrate Web Forms custom controls to Blazor.

## Features

This analyzer package provides automated assistance when migrating ASP.NET Web Forms custom controls (inheriting from `WebControl` or `CompositeControl`) to Blazor.

### BWFC001: Missing [Parameter] Attribute

**Severity**: Warning

Detects public properties in `WebControl` or `CompositeControl` derived classes that should have `[Parameter]` attributes for Blazor compatibility.

**Example:**

```csharp
public class HelloLabel : WebControl
{
    // ⚠️ BWFC001: Property 'Text' should have [Parameter] attribute
    public string Text { get; set; }  
    
    protected override void Render(HtmlTextWriter writer)
    {
        // ...
    }
}
```

**Code Fix**: Adds `[Parameter]` attribute and `using Microsoft.AspNetCore.Components;` if needed.

```csharp
public class HelloLabel : WebControl
{
    [Parameter]  // ✅ Fixed
    public string Text { get; set; }  
    
    protected override void Render(HtmlTextWriter writer)
    {
        // ...
    }
}
```

### BWFC002: Missing AddBaseAttributes() Call

**Severity**: Info

Detects `Render` or `RenderContents` methods that call `RenderBeginTag()` but don't call `AddBaseAttributes(writer)` first. This ensures that base styling properties (ID, CssClass, Style) are properly applied.

**Example:**

```csharp
protected override void Render(HtmlTextWriter writer)
{
    // ℹ️ BWFC002: Should call AddBaseAttributes(writer) before RenderBeginTag()
    writer.RenderBeginTag(HtmlTextWriterTag.Div);
    writer.Write("Content");
    writer.RenderEndTag();
}
```

**Code Fix**: Adds `AddBaseAttributes(writer);` at the beginning of the method.

```csharp
protected override void Render(HtmlTextWriter writer)
{
    AddBaseAttributes(writer);  // ✅ Fixed
    writer.RenderBeginTag(HtmlTextWriterTag.Div);
    writer.Write("Content");
    writer.RenderEndTag();
}
```

## Installation

### Option 1: NuGet Package (Recommended)

```bash
dotnet add package BlazorWebFormsComponents.Analyzers
```

The analyzer is automatically enabled when you install the package.

### Option 2: Project Reference

Add a project reference to your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/BlazorWebFormsComponents.Analyzers/BlazorWebFormsComponents.Analyzers.csproj" 
                    OutputItemType="Analyzer" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

## Usage

Once installed, the analyzers will automatically run during build and provide warnings/info messages with quick fixes in Visual Studio, VS Code, and other IDEs that support Roslyn analyzers.

### In Visual Studio

1. Warnings appear in the Error List window
2. Click on the warning or press `Ctrl+.` to see available code fixes
3. Select "Add [Parameter] attribute" or "Add AddBaseAttributes(writer) call"

### In VS Code

1. Warnings appear as squiggly underlines
2. Hover over the warning or press `Ctrl+.` to see available code fixes
3. Select the appropriate fix

### Command Line

```bash
dotnet build
```

Warnings will appear in the build output.

## Suppressing Warnings

If you have a valid reason to suppress a warning, use `#pragma` directives:

```csharp
#pragma warning disable BWFC001
public string SomeProperty { get; set; }  // Won't show warning
#pragma warning restore BWFC001
```

Or use `[SuppressMessage]` attribute:

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BWFC001")]
public string SomeProperty { get; set; }
```

## Requirements

- .NET Standard 2.0 or later
- Compatible with Visual Studio 2022, VS Code, and other Roslyn-powered IDEs
- Works with BlazorWebFormsComponents library

## See Also

- [Custom Controls Migration Guide](../../docs/Migration/Custom-Controls.md)
- [BlazorWebFormsComponents Documentation](https://github.com/FritzAndFriends/BlazorWebFormsComponents)
