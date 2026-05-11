using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Materializes IQueryable select methods that create a DbContext with CreateDbContext()
/// so the query can be safely enumerated after the method returns.
/// </summary>
public class SelectMethodMaterializeTransform : ICodeBehindTransform
{
    private static readonly Regex IQueryableMethodRegex = new(
        @"(?<method>(?<signature>^[ \t]*(?:public|protected|private|internal)[^{;\r\n]*IQueryable<[^;\r\n{]+\([^)]*\)\s*\{)(?<body>(?>[^{}]+|\{(?<depth>)|\}(?<-depth>))*)(?(depth)(?!))\})",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    private static readonly Regex UsingCreateDbContextRegex = new(
        @"using\s+var\s+(?<var>\w+)\s*=\s*(?<factory>\w+\.CreateDbContext\(\))\s*;",
        RegexOptions.Compiled);

    private static readonly Regex ReturnQueryableRegex = new(
        @"return\s+(?<expr>\w+)\s*;",
        RegexOptions.Compiled);

    public string Name => "SelectMethodMaterialize";
    public int Order => 108;

    public string Apply(string content, FileMetadata metadata)
    {
        if ((metadata.FileType != FileType.Page && metadata.FileType != FileType.Control) ||
            !content.Contains("IQueryable<", StringComparison.Ordinal) ||
            !content.Contains("CreateDbContext()", StringComparison.Ordinal))
        {
            return content;
        }

        return IQueryableMethodRegex.Replace(content, match => TransformMethod(match.Groups["method"].Value));
    }

    private static string TransformMethod(string methodContent)
    {
        if (!UsingCreateDbContextRegex.IsMatch(methodContent))
            return methodContent;

        var updatedMethod = UsingCreateDbContextRegex.Replace(methodContent, "var ${var} = ${factory};");
        updatedMethod = ReturnQueryableRegex.Replace(updatedMethod, match =>
        {
            var expression = match.Groups["expr"].Value;
            return $"return {expression}.ToList().AsQueryable();";
        });

        return updatedMethod;
    }
}
