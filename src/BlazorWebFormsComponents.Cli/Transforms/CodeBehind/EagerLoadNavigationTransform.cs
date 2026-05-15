using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Injects .Include() calls for navigation properties discovered from:
/// 1. BoundField DataField="X.Y" / Eval("X.Y") in paired .razor markup
/// 2. LINQ navigation access: select/where c.Nav.Prop in same-file code
/// 3. Member access in loops/assignments: item.Nav.Prop patterns
///
/// Handles three query styles:
/// - .ToList().AsQueryable() (SelectMethod wrapper pattern)
/// - .ToList() / .FirstOrDefault() etc. (direct method syntax on injected DbContext)
/// - LINQ query syntax (from x in _db.Table)
///
/// When a method returns a collection but nav access can't be confirmed in the same method,
/// a warning comment is emitted for developer review (hybrid B+D approach).
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

    // Matches return statements with .ToList().AsQueryable() (SelectMethod wrapper pattern)
    private static readonly Regex ReturnToListAsQueryableRegex = new(
        @"(?<prefix>return\s+(?<expr>[^;]+?))\.ToList\(\)\.AsQueryable\(\)\s*;",
        RegexOptions.Compiled);

    // Matches using var db = ... or var db = _contextFactory.CreateDbContext()
    private static readonly Regex DbContextVarRegex = new(
        @"(?:using\s+)?var\s+(?<var>\w+)\s*=\s*\w+\.CreateDbContext\(\)",
        RegexOptions.Compiled);

    // Matches LINQ query syntax: from x in dbVar.TableName
    private static readonly Regex LinqQuerySyntaxRegex = new(
        @"from\s+\w+\s+in\s+(?<dbExpr>\w+\.\w+)",
        RegexOptions.Compiled);

    public string Name => "EagerLoadNavigation";
    public int Order => 109; // Run right after SelectMethodMaterialize (108)

    // Matches navigation property access in LINQ: x.Nav.Property (dotted member access on a range variable)
    private static readonly Regex LinqNavAccessRegex = new(
        @"(?:select|where|orderby|let)\s+[^;]*?\b\w+\.(?<nav>[A-Z]\w*)\.(?<prop>[A-Z]\w*)",
        RegexOptions.Compiled);

    // Matches member access on variables: item.Nav.Prop or c.Nav.Prop (outside LINQ keywords)
    // Uses atomic group (?>...) on prop to prevent backtracking past method names
    private static readonly Regex MemberNavAccessRegex = new(
        @"\b\w+\.(?<nav>[A-Z]\w*)\.(?<prop>(?>(?:[A-Z]\w*)))(?!\s*\()",
        RegexOptions.Compiled);

    // Matches injected DbContext field access before LINQ method calls or direct materializers:
    // _db.TableName.Where(...), _context.Products.Select(...), _db.TableName.ToList()
    private static readonly Regex InjectedDbFieldBeforeLinqRegex = new(
        @"(?<dbset>_\w+\.\w+)(?=\s*\.(?:Where|OrderBy|OrderByDescending|Select|SingleOrDefault|FirstOrDefault|Any|Count|Take|Skip|GroupBy|ToList|ToArray|First|Single))",
        RegexOptions.Compiled);

    // Matches DbContext field declarations: private readonly XxxContext _field;
    private static readonly Regex DbContextFieldRegex = new(
        @"private\s+(?:readonly\s+)?\w*[Cc]ontext\w*\s+(?<field>_\w+)\s*[;=]",
        RegexOptions.Compiled);

    // Known property names that are NOT navigation properties (avoid false positives)
    private static readonly HashSet<string> NonNavPropertyNames = new(StringComparer.Ordinal)
    {
        "CartId", "ProductId", "ProductID", "CategoryId", "CategoryID",
        "OrderId", "OrderID", "ItemId", "ItemID", "UserId", "UserID",
        "Quantity", "UnitPrice", "DateCreated", "ToString", "Count",
        "Length", "Value", "Name", "Text", "Id", "ID"
    };

    public string Apply(string content, FileMetadata metadata)
    {
        HashSet<string> navProps;

        if (metadata.FileType == FileType.Page || metadata.FileType == FileType.Control)
        {
            // For pages/controls, read the paired markup file for nav property references
            var markupPath = metadata.OutputFilePath?.Replace(".razor.cs", ".razor");
            if (markupPath == null || !File.Exists(markupPath))
                return content;

            var markup = File.ReadAllText(markupPath);
            navProps = ExtractNavigationProperties(markup);

            // Also check code-behind for nav property access
            var codeNavProps = ExtractNavigationPropertiesFromCode(content);
            navProps.UnionWith(codeNavProps);
        }
        else if (metadata.FileType == FileType.CodeFile)
        {
            // For service/standalone classes, extract nav props from LINQ + member access
            navProps = ExtractNavigationPropertiesFromCode(content);
        }
        else
        {
            return content;
        }

        if (navProps.Count == 0)
            return content;

        // Detect DbContext field names for injected-field support
        var dbFields = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in DbContextFieldRegex.Matches(content))
            dbFields.Add(m.Groups["field"].Value);

        // Check if content has any query patterns that could benefit from .Include()
        var hasCreateDbContext = content.Contains("CreateDbContext()", StringComparison.Ordinal);
        var hasIQueryable = content.Contains("IQueryable<", StringComparison.Ordinal);
        var hasToList = content.Contains(".ToList()", StringComparison.Ordinal);
        var hasLinqSyntax = LinqQuerySyntaxRegex.IsMatch(content);
        var hasInjectedDbField = dbFields.Count > 0 &&
            dbFields.Any(f => content.Contains($"{f}.", StringComparison.Ordinal));

        if (!hasCreateDbContext && !hasIQueryable && !hasToList && !hasLinqSyntax && !hasInjectedDbField)
            return content;

        // Build the .Include() chain
        var includeChain = string.Join("", navProps.Select(p => $".Include(x => x.{p})"));

        var modified = content;

        // Strategy 1: .ToList().AsQueryable() (SelectMethod wrapper pattern)
        modified = ReturnToListAsQueryableRegex.Replace(modified, match =>
        {
            var expr = match.Groups["expr"].Value;
            if (expr.Contains(".Include(", StringComparison.Ordinal))
                return match.Value; // Already has Include
            return $"return {expr}{includeChain}.ToList().AsQueryable();";
        });

        // Strategy 2: Local DbContext variable from CreateDbContext()
        if (hasCreateDbContext)
        {
            var dbMatch = DbContextVarRegex.Match(modified);
            if (dbMatch.Success)
            {
                var dbVar = dbMatch.Groups["var"].Value;
                modified = Regex.Replace(modified,
                    $@"({Regex.Escape(dbVar)}\.\w+)(?=\s*(?:\.Where|\.OrderBy|\.Select|\.ToList|\.ToArray|\.First|\.FirstOrDefault|\.Single|\.SingleOrDefault|\s*;))",
                    m => m.Value.Contains(".Include(") ? m.Value : $"{m.Value}{includeChain}",
                    RegexOptions.None,
                    TimeSpan.FromMilliseconds(500));
            }
        }

        // Strategy 3: LINQ query syntax — from x in _db.Table
        if (hasLinqSyntax)
        {
            modified = LinqQuerySyntaxRegex.Replace(modified, match =>
            {
                var dbExpr = match.Groups["dbExpr"].Value;
                if (match.Value.Contains(".Include(", StringComparison.Ordinal))
                    return match.Value;
                return match.Value.Replace(dbExpr, dbExpr + includeChain);
            });
        }

        // Strategy 4: Injected DbContext field — _db.TableName.Where(...)
        if (hasInjectedDbField)
        {
            modified = InjectedDbFieldBeforeLinqRegex.Replace(modified, match =>
            {
                var dbset = match.Groups["dbset"].Value;
                // Only inject on known DbContext fields
                var fieldName = dbset.Split('.')[0];
                if (!dbFields.Contains(fieldName))
                    return match.Value;
                if (modified[..(match.Index + match.Length)].Contains(dbset + includeChain))
                    return match.Value; // Already injected by a prior strategy

                // Filter out nav properties that match the DbSet name to avoid self-referencing
                // e.g., _db.Products should not get .Include(x => x.Product) since Product doesn't have a Product nav
                var tableName = dbset.Split('.')[1];
                var filteredNavProps = navProps
                    .Where(p => !tableName.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (filteredNavProps.Count == 0)
                    return match.Value;

                var filteredChain = string.Join("", filteredNavProps.Select(p => $".Include(x => x.{p})"));
                return dbset + filteredChain;
            });
        }

        if (modified == content)
            return content; // No injection point found

        // Ensure Microsoft.EntityFrameworkCore using is present
        if (!modified.Contains("using Microsoft.EntityFrameworkCore;", StringComparison.Ordinal))
        {
            var lastUsing = Regex.Match(modified, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
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

    /// <summary>
    /// Extracts navigation property names from LINQ expressions and member access in code.
    /// Detects both LINQ keyword access (select c.Product.UnitPrice) and
    /// general member access (item.Product.ProductID) patterns.
    /// Filters out known non-navigation property names to reduce false positives.
    /// </summary>
    // Matches using directives, comments, and string literals that should be excluded from nav property detection
    private static readonly Regex NonCodeLineRegex = new(
        @"^\s*(?:using\s|//|#|\[assembly:)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    internal static HashSet<string> ExtractNavigationPropertiesFromCode(string content)
    {
        var navProps = new HashSet<string>(StringComparer.Ordinal);

        // Strip using directives and comment lines to avoid false positives
        // (e.g., System.Collections.Generic → "Collections", Microsoft.AspNetCore.Http → "AspNetCore")
        var codeOnly = string.Join("\n",
            content.Split('\n').Where(line => !NonCodeLineRegex.IsMatch(line)));

        // LINQ keyword access: select/where/orderby c.Nav.Prop
        foreach (Match m in LinqNavAccessRegex.Matches(codeOnly))
        {
            var nav = m.Groups["nav"].Value;
            if (!NonNavPropertyNames.Contains(nav))
                navProps.Add(nav);
        }

        // General member access: item.Nav.Prop (catches foreach loops, assignments, etc.)
        // Method calls are already excluded by the regex negative lookahead
        foreach (Match m in MemberNavAccessRegex.Matches(codeOnly))
        {
            var nav = m.Groups["nav"].Value;
            if (!NonNavPropertyNames.Contains(nav) && char.IsUpper(nav[0]) && nav.Length > 2)
            {
                navProps.Add(nav);
            }
        }

        return navProps;
    }
}
