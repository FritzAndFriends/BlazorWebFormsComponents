using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Injects .Include() calls for navigation properties referenced by BoundField DataField paths
/// (e.g., DataField="Product.ProductName" → .Include(x => x.Product)).
/// Reads the paired .razor markup to discover dotted DataField attributes, then adds
/// .Include() calls before .ToList() or return statements in SelectMethod/query methods.
/// </summary>
public class EagerLoadNavigationTransform : ICodeBehindTransform
{
    // Matches BoundField DataField="X.Y" or DataField="X.Y.Z" (dotted paths = navigation properties)
    private static readonly Regex BoundFieldDottedRegex = new(
        @"<BoundField[^>]+DataField\s*=\s*""(?<path>[A-Za-z_]\w*\.[A-Za-z_][\w.]*?)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Matches Eval("X.Y") or Eval("X.Y.Z") in ItemTemplate bindings
    private static readonly Regex EvalDottedRegex = new(
        @"Eval\(\s*""(?<path>[A-Za-z_]\w*\.[A-Za-z_][\w.]*?)""\s*\)",
        RegexOptions.Compiled);

    // Matches return statements with .ToList() that could have .Include() inserted before them
    private static readonly Regex ReturnToListRegex = new(
        @"(?<prefix>return\s+(?<expr>[^;]+?))\.ToList\(\)\.AsQueryable\(\)\s*;",
        RegexOptions.Compiled);

    // Matches return statements with plain IQueryable (no .ToList())
    private static readonly Regex ReturnQueryableRegex = new(
        @"return\s+(?<expr>\w[\w.]*)\s*;",
        RegexOptions.Compiled);

    // Matches using var db = ... or var db = _contextFactory.CreateDbContext()
    private static readonly Regex DbContextVarRegex = new(
        @"(?:using\s+)?var\s+(?<var>\w+)\s*=\s*\w+\.CreateDbContext\(\)",
        RegexOptions.Compiled);

    // Matches IQueryable<T> method signatures
    private static readonly Regex IQueryableMethodRegex = new(
        @"IQueryable<(?<type>\w+)>\s+(?<name>\w+)\s*\(",
        RegexOptions.Compiled);

    // Matches List<T> or IEnumerable<T> or IList<T> returning methods with LINQ queries
    private static readonly Regex CollectionMethodRegex = new(
        @"(?:List|IEnumerable|IList|ICollection)<(?<type>\w+)>\s+(?<name>\w+)\s*\(",
        RegexOptions.Compiled);

    public string Name => "EagerLoadNavigation";
    public int Order => 109; // Run right after SelectMethodMaterialize (108)

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page && metadata.FileType != FileType.Control)
            return content;

        // Read the paired markup file to find navigation property references
        var markupPath = metadata.OutputFilePath?.Replace(".razor.cs", ".razor");
        if (markupPath == null || !File.Exists(markupPath))
            return content;

        var markup = File.ReadAllText(markupPath);
        var navProps = ExtractNavigationProperties(markup);
        if (navProps.Count == 0)
            return content;

        // Check if content has any query methods that could benefit from .Include()
        if (!content.Contains("CreateDbContext()", StringComparison.Ordinal) &&
            !content.Contains("IQueryable<", StringComparison.Ordinal) &&
            !content.Contains(".ToList()", StringComparison.Ordinal))
            return content;

        // Ensure Microsoft.EntityFrameworkCore using is present
        var needsUsing = !content.Contains("using Microsoft.EntityFrameworkCore;", StringComparison.Ordinal);

        // Build the .Include() chain
        var includeChain = string.Join("", navProps.Select(p => $".Include(x => x.{p})"));

        // Try to inject .Include() before .ToList().AsQueryable()
        var modified = ReturnToListRegex.Replace(content, match =>
        {
            var expr = match.Groups["expr"].Value;
            return $"return {expr}{includeChain}.ToList().AsQueryable();";
        });

        // If no .ToList().AsQueryable() pattern, try plain return with db variable
        if (modified == content)
        {
            // Find methods that have a DbContext and return a queryable/collection
            var dbMatch = DbContextVarRegex.Match(content);
            if (dbMatch.Success)
            {
                var dbVar = dbMatch.Groups["var"].Value;
                // Insert .Include() after the db.Set<T>() or db.TableName reference
                modified = Regex.Replace(content,
                    $@"({Regex.Escape(dbVar)}\.\w+)(?=\s*(?:\.Where|\.OrderBy|\.Select|\s*;))",
                    $"$1{includeChain}",
                    RegexOptions.None,
                    TimeSpan.FromMilliseconds(500));
            }
        }

        if (modified == content)
            return content; // No injection point found

        // Add the using statement if needed
        if (needsUsing)
        {
            var lastUsing = Regex.Match(modified, @"^using\s+[^;]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                modified = modified[..insertAt] + "\nusing Microsoft.EntityFrameworkCore;" + modified[insertAt..];
            }
        }

        return modified;
    }

    /// <summary>
    /// Extracts unique first-segment navigation property names from dotted DataField/Eval paths in markup.
    /// E.g., DataField="Product.ProductName" → "Product"
    /// </summary>
    internal static HashSet<string> ExtractNavigationProperties(string markup)
    {
        var navProps = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in BoundFieldDottedRegex.Matches(markup))
        {
            var firstSegment = m.Groups["path"].Value.Split('.')[0];
            navProps.Add(firstSegment);
        }

        foreach (Match m in EvalDottedRegex.Matches(markup))
        {
            var firstSegment = m.Groups["path"].Value.Split('.')[0];
            navProps.Add(firstSegment);
        }

        return navProps;
    }
}
