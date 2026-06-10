using System.Text;
using BlazorWebFormsComponents.Cli.Analysis;
using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public sealed class CodeOnlyControlScaffolder
{
    public async Task<int> EmitAsync(
        string outputRoot,
        string projectName,
        IReadOnlyList<CodeOnlyServerControlDescriptor> controls,
        OutputWriter writer)
    {
        if (controls.Count == 0)
            return 0;

        var generatedCount = 0;
        var usedNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var control in controls)
        {
            var componentName = GetComponentName(control.ClassName, usedNames);
            var namespaceName = $"{projectName}.Generated.CodeOnlyControls";
            var outputDirectory = Path.Combine(outputRoot, "Generated", "CodeOnlyControls");

            var markupPath = Path.Combine(outputDirectory, $"{componentName}.razor");
            var codeBehindPath = Path.Combine(outputDirectory, $"{componentName}.razor.cs");

            var markupContent = $"""
                                 @inherits {namespaceName}.{componentName}
                                 @ChildContent
                                 """;

            var codeBehindContent = BuildCodeBehind(namespaceName, componentName, control);

            await writer.WriteFileAsync(markupPath, markupContent, $"Code-only control skeleton: {control.ClassName}");
            await writer.WriteFileAsync(codeBehindPath, codeBehindContent, $"Code-only control code-behind: {control.ClassName}");
            generatedCount += 2;
        }

        return generatedCount;
    }

    private static string GetComponentName(string className, IDictionary<string, int> usedNames)
    {
        if (!usedNames.TryGetValue(className, out var count))
        {
            usedNames[className] = 1;
            return className;
        }

        count++;
        usedNames[className] = count;
        return $"{className}{count}";
    }

    /// <summary>
    /// Maps Web Forms base class names to their BWFC Blazor equivalent base classes.
    /// </summary>
    internal static string MapBwfcBaseClass(string webFormsBaseType) => webFormsBaseType switch
    {
        "WebControl" => "BaseStyledComponent",
        "CompositeControl" => "BaseWebFormsComponent",
        "DataBoundControl" or "BaseDataBoundControl" => "BaseWebFormsComponent",
        "TemplatedControl" => "BaseWebFormsComponent",
        _ => "BaseWebFormsComponent"
    };

    private static string BuildCodeBehind(string namespaceName, string componentName, CodeOnlyServerControlDescriptor control)
    {
        var bwfcBase = MapBwfcBaseClass(control.BaseType);
        var tagPrefixComment = control.TagPrefixes.Count == 0
            ? "// TODO(bwfc-control-scaffold): No Web.config tagPrefix registration was found for this control."
            : $"// TODO(bwfc-control-scaffold): Registered tag prefixes: {string.Join(", ", control.TagPrefixes)}";

        var sb = new StringBuilder();
        sb.AppendLine("using BlazorWebFormsComponents;");
        sb.AppendLine("using Microsoft.AspNetCore.Components;");
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine($"public partial class {componentName} : {bwfcBase}");
        sb.AppendLine("{");
        sb.AppendLine($"    // Source: {control.SourceFilePath} ({control.Namespace}.{control.ClassName} : {control.BaseType})");
        sb.AppendLine($"    {tagPrefixComment}");
        sb.AppendLine();
        sb.AppendLine("    [Parameter] public RenderFragment? ChildContent { get; set; }");

        // Emit preserved public properties as [Parameter] members
        foreach (var property in control.Properties)
        {
            sb.AppendLine($"    [Parameter] public {property.TypeName} {property.Name} {{ get; set; }}");
        }

        // Emit preserved public events as EventCallback parameters
        foreach (var evt in control.Events)
        {
            var callbackType = MapEventCallback(evt.DelegateType);
            sb.AppendLine($"    [Parameter] public {callbackType} {evt.Name} {{ get; set; }}");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Maps a Web Forms event delegate type to the Blazor EventCallback equivalent.
    /// EventHandler → EventCallback
    /// EventHandler&lt;TArgs&gt; → EventCallback&lt;TArgs&gt;
    /// </summary>
    internal static string MapEventCallback(string delegateType)
    {
        var trimmed = delegateType.Trim();

        // EventHandler<TArgs> → EventCallback<TArgs>
        if (trimmed.StartsWith("EventHandler<", StringComparison.OrdinalIgnoreCase)
            && trimmed.EndsWith(">", StringComparison.Ordinal))
        {
            var innerType = trimmed["EventHandler<".Length..^1].Trim();
            return $"EventCallback<{innerType}>";
        }

        // Plain EventHandler or unknown → EventCallback
        return "EventCallback";
    }
}
