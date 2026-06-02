using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Cross-file DataBind transform: ctrl.DataSource = expr → field assignment,
/// .DataBind() removal, field declarations.
/// Also exposes InjectItemsAttributes() for markup-side injection.
/// TC17 uses markup-only Bind expressions; this handles code-behind DataBind patterns.
/// </summary>
public class DataBindTransform : ICodeBehindTransform
{
    public string Name => "DataBind";
    public int Order => 800;

    // Matches: controlId.DataSource = expression;
    private static readonly Regex DataSourceAssignRegex = new(
        @"(?:this\.)?(\w+)\.DataSource\s*=\s*(.+?)\s*;",
        RegexOptions.Compiled);

    // Matches: controlId.DataBind();
    private static readonly Regex DataBindCallRegex = new(
        @"[ \t]*(?:this\.)?\w+\.DataBind\(\)\s*;[ \t]*\r?\n?",
        RegexOptions.Compiled);

    private static readonly Regex SelfDataBindCallRegex = new(
        @"[ \t]*(?:this\.)?DataBind\(\)\s*;[ \t]*\r?\n?",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Scan for DataSource assignments and store in metadata for cross-file correlation
        var dsMatches = DataSourceAssignRegex.Matches(content);
        foreach (Match m in dsMatches)
        {
            var controlId = m.Groups[1].Value;
            var expression = m.Groups[2].Value;
            metadata.DataBindMap[controlId] = expression;
        }

        // Remove .DataBind() calls — Blazor renders automatically
        content = DataBindCallRegex.Replace(content, "");

        var hasAscxDataBindSignal = metadata.FileType == FileType.Control
            && (metadata.AscxDescriptor?.HasDataBindCall ?? content.Contains("DataBind(", StringComparison.Ordinal));
        if (hasAscxDataBindSignal)
        {
            content = SelfDataBindCallRegex.Replace(content, "");
        }

        return content;
    }

    /// <summary>
    /// Injects Items="@fieldName" attributes into markup for controls that had DataSource assigned in code-behind.
    /// Called by the pipeline after code-behind transforms complete.
    /// </summary>
    public static string InjectItemsAttributes(string markup, Dictionary<string, string> dataBindMap)
    {
        foreach (var (controlId, expression) in dataBindMap)
        {
            // Find the control by id="controlId" and inject Items attribute
            var controlRegex = new Regex($@"(<\w+[^>]*\bid\s*=\s*""{Regex.Escape(controlId)}""[^>]*?)(\s*/?>)");
            if (controlRegex.IsMatch(markup))
            {
                markup = controlRegex.Replace(markup, $"$1 Items=\"@({expression})\"$2", 1);
            }
        }
        return markup;
    }
}
