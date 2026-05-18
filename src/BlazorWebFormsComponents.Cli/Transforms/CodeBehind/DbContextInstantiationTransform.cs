using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces <c>new XxxContext()</c> instantiations with an injected property,
/// converting inline DbContext creation to the DI pattern required in Blazor.
/// </summary>
public class DbContextInstantiationTransform : ICodeBehindTransform
{
    // Matches field/variable assignments like: XxxContext _db = new XxxContext();
    // or using blocks like: using (XxxContext _db = new XxxContext())
    // "Entities" covers EF6 T4-generated DbContext names like ContosoUniversityEntities.
    // "DataContext" covers LINQ-to-SQL DataContext subclasses.
    private static readonly Regex NewContextRegex = new(
        @"(?<type>\w+(?:Context|Entities|DataContext|Actions|Service|Manager|Helper|Repository|Provider))\s+(?<var>\w+)\s*=\s*new\s+\k<type>\s*\(\s*\)",
        RegexOptions.Compiled);

    // Matches just the `new XxxType()` expression for known service/context patterns,
    // with optional namespace prefix.
    // e.g., new ProductContext(), new ContosoUniversityEntities(), new ShoppingCartActions()
    private static readonly Regex NewContextExprRegex = new(
        @"new\s+(?:(?<ns>[\w.]+)\.)?(?<type>\w+(?:Context|Entities|DataContext|Actions|Service|Manager|Helper|Repository|Provider))\s*\(\s*\)",
        RegexOptions.Compiled);

    // Check if a class already has an [Inject] property for a given type
    private static readonly string InjectAttr = "[Inject]";

    public string Name => "DbContextInstantiation";
    public int Order => 107; // Right after EfContextConstructor (106)

    // Detects page/component classes that use [Inject] DI pattern
    private static readonly Regex PageBaseRegex = new(
        @":\s*(?:WebFormsPageBase|ComponentBase|LayoutComponentBase|BaseWebFormsComponent)\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Skip DbContext class definitions themselves
        if (Regex.IsMatch(content, @"class\s+\w+Context\s*:\s*(?:Db|Identity)", RegexOptions.Compiled))
            return content;

        var matches = NewContextExprRegex.Matches(content);
        if (matches.Count == 0)
            return content;

        // Determine if this is a page/component (uses [Inject]) or a standalone class (uses constructor injection)
        var isPageOrComponent = metadata.FileType == FileType.Page
            || metadata.FileType == FileType.Control
            || PageBaseRegex.IsMatch(content);

        // Extract the containing class name to prevent self-injection
        var containingClassMatch = Regex.Match(content, @"class\s+(\w+)", RegexOptions.Compiled);
        var containingClassName = containingClassMatch.Success ? containingClassMatch.Groups[1].Value : "";

        // Collect unique context type names, excluding the containing class itself
        var contextTypes = matches
            .Select(m => m.Groups["type"].Value)
            .Where(t => !string.Equals(t, containingClassName, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        // Pre-scan for original variable names used with each context type.
        // e.g., "ProductContext _db = new ProductContext()" → type=ProductContext, var=_db
        var originalVarNames = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (Match m in NewContextRegex.Matches(content))
        {
            var type = m.Groups["type"].Value;
            var varName = m.Groups["var"].Value;
            if (!originalVarNames.ContainsKey(type))
                originalVarNames[type] = varName;
        }

        foreach (var contextType in contextTypes)
        {
            // Preserve the original variable name when possible; fall back to generated name
            var fieldName = originalVarNames.TryGetValue(contextType, out var origVar) && origVar.StartsWith('_')
                ? origVar
                : "_" + char.ToLowerInvariant(contextType[0]) + contextType[1..];

            // Replace field declarations like: XxxContext _db = new XxxContext();
            // or: Namespace.XxxContext _db = new Namespace.XxxContext();
            var fqnPattern = $@"(?:[\w.]+\.)?{Regex.Escape(contextType)}";
            var fieldDeclRegex = new Regex(
                $@"(?<indent>[ \t]*)(?:private\s+)?{fqnPattern}\s+\w+\s*=\s*new\s+{fqnPattern}\s*\(\s*\)\s*;",
                RegexOptions.Compiled);
            content = fieldDeclRegex.Replace(content, "");

            // Replace using declarations: using (XxxContext x = new XxxContext()) { ... }
            // Removes the using wrapper entirely, keeping the body contents
            var usingBlockRegex = new Regex(
                $@"using\s*\(\s*(?:var|{fqnPattern})\s+(?<var>\w+)\s*=\s*new\s+{fqnPattern}\s*\(\s*\)\s*\)\s*\{{",
                RegexOptions.Compiled);
            var usingBlockMatch = usingBlockRegex.Match(content);
            while (usingBlockMatch.Success)
            {
                var varName = usingBlockMatch.Groups["var"].Value;
                var openBracePos = content.IndexOf('{', usingBlockMatch.Index);
                var closingBracePos = FindMatchingBrace(content, openBracePos);
                if (closingBracePos >= 0)
                {
                    // Extract body between braces, dedented
                    var body = content[(openBracePos + 1)..closingBracePos];
                    // Replace the original variable name with the injected field
                    body = body.Replace(varName, fieldName);
                    var replacement = $"// DbContext '{contextType}' is injected via DI\n{body}";
                    content = content[..usingBlockMatch.Index] + replacement + content[(closingBracePos + 1)..];
                }
                else
                {
                    // Couldn't find matching brace, just replace the using line
                    content = usingBlockRegex.Replace(content, $"var {varName} = {fieldName}; // Injected via DI\n{{", 1);
                }
                usingBlockMatch = usingBlockRegex.Match(content);
            }

            // Fallback: using declarations without braces on same match (single-line using statement)
            var usingDeclRegex = new Regex(
                $@"using\s*\(\s*{fqnPattern}\s+\w+\s*=\s*new\s+{fqnPattern}\s*\(\s*\)\s*\)",
                RegexOptions.Compiled);
            content = usingDeclRegex.Replace(content, m =>
            {
                return $"// DbContext '{contextType}' is injected via DI";
            });

            // Also handle using var declarations: using var db = new XxxContext();
            var usingVarDeclRegex = new Regex(
                $@"using\s+(?:var|{fqnPattern})\s+(?<var>\w+)\s*=\s*new\s+{fqnPattern}\s*\(\s*\)\s*;",
                RegexOptions.Compiled);
            content = usingVarDeclRegex.Replace(content, m =>
            {
                var varName = m.Groups["var"].Value;
                return $"var {varName} = {fieldName}; // Injected via DI";
            });

            // Replace local variable declarations: var db = new XxxContext();
            var localVarRegex = new Regex(
                $@"(?<indent>[ \t]*)(?:var|{fqnPattern})\s+(?<var>\w+)\s*=\s*new\s+{fqnPattern}\s*\(\s*\)\s*;",
                RegexOptions.Compiled);
            content = localVarRegex.Replace(content, m =>
            {
                var varName = m.Groups["var"].Value;
                var indent = m.Groups["indent"].Value;
                return $"{indent}var {varName} = {fieldName}; // Injected via DI";
            });

            // Replace any remaining `new XxxContext()` expressions (including fully-qualified)
            var remainingNewRegex = new Regex(
                $@"new\s+{fqnPattern}\s*\(\s*\)",
                RegexOptions.Compiled);
            content = remainingNewRegex.Replace(content, fieldName);

            // Add injection for the context
            if (!content.Contains($"{contextType} {fieldName}", StringComparison.Ordinal))
            {
                if (isPageOrComponent)
                {
                    // Page/component: use [Inject] property
                    var classBodyRegex = new Regex(
                        @"((?:partial\s+)?class\s+\w+[^{]*\{)",
                        RegexOptions.Compiled);
                    var classMatch = classBodyRegex.Match(content);
                    if (classMatch.Success)
                    {
                        var insertPos = classMatch.Index + classMatch.Length;
                        var injection = $"\n    {InjectAttr}\n    protected {contextType} {fieldName} {{ get; set; }} = default!;\n";
                        content = content[..insertPos] + injection + content[insertPos..];
                    }
                }
                else
                {
                    // Standalone class: use constructor injection with private readonly field
                    var classBodyRegex = new Regex(
                        @"((?:partial\s+)?class\s+(?<className>\w+)[^{]*\{)",
                        RegexOptions.Compiled);
                    var classMatch = classBodyRegex.Match(content);
                    if (classMatch.Success)
                    {
                        var className = classMatch.Groups["className"].Value;
                        var insertPos = classMatch.Index + classMatch.Length;
                        var field = $"\n    private readonly {contextType} {fieldName};\n";

                        // Check if a constructor already exists
                        var existingCtorRegex = new Regex(
                            $@"public\s+{Regex.Escape(className)}\s*\(([^)]*)\)",
                            RegexOptions.Compiled);
                        var ctorMatch = existingCtorRegex.Match(content);
                        if (ctorMatch.Success)
                        {
                            // Add parameter to existing constructor
                            var existingParams = ctorMatch.Groups[1].Value.Trim();
                            var newParams = string.IsNullOrEmpty(existingParams)
                                ? $"{contextType} {fieldName.TrimStart('_')}"
                                : $"{existingParams}, {contextType} {fieldName.TrimStart('_')}";
                            content = content[..ctorMatch.Index]
                                + $"public {className}({newParams})"
                                + content[(ctorMatch.Index + ctorMatch.Length)..];

                            // Add field assignment in constructor body
                            var ctorBodyStart = content.IndexOf('{', ctorMatch.Index) + 1;
                            content = content[..ctorBodyStart]
                                + $"\n        {fieldName} = {fieldName.TrimStart('_')};"
                                + content[ctorBodyStart..];
                        }
                        else
                        {
                            // Create new constructor
                            var ctor = $"\n    public {className}({contextType} {fieldName.TrimStart('_')})\n    {{\n        {fieldName} = {fieldName.TrimStart('_')};\n    }}\n";
                            content = content[..insertPos] + field + ctor + content[insertPos..];
                        }

                        // Add field declaration if constructor path didn't already
                        if (!content.Contains($"readonly {contextType} {fieldName}", StringComparison.Ordinal))
                        {
                            content = content[..insertPos] + field + content[insertPos..];
                        }
                    }
                }
            }
        }

        // Ensure Microsoft.AspNetCore.Components using is present for [Inject] (only for pages)
        if (isPageOrComponent && content.Contains(InjectAttr) && !content.Contains("using Microsoft.AspNetCore.Components;", StringComparison.Ordinal))
        {
            var lastUsing = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                content = content[..insertAt] + "\nusing Microsoft.AspNetCore.Components;" + content[insertAt..];
            }
        }

        // Clean up empty lines left by removed field declarations
        content = Regex.Replace(content, @"\n{3,}", "\n\n");

        return content;
    }

    /// <summary>
    /// Finds the closing brace that matches the opening brace at the given position,
    /// accounting for nested braces, string literals, and comments.
    /// </summary>
    private static int FindMatchingBrace(string content, int openBracePos)
    {
        var depth = 1;
        var inString = false;
        var inVerbatimString = false;
        var inSingleLineComment = false;
        var inMultiLineComment = false;

        for (var i = openBracePos + 1; i < content.Length; i++)
        {
            var c = content[i];
            var next = i + 1 < content.Length ? content[i + 1] : '\0';

            if (inSingleLineComment)
            {
                if (c == '\n') inSingleLineComment = false;
                continue;
            }
            if (inMultiLineComment)
            {
                if (c == '*' && next == '/') { inMultiLineComment = false; i++; }
                continue;
            }
            if (inVerbatimString)
            {
                if (c == '"' && next == '"') { i++; continue; }
                if (c == '"') { inVerbatimString = false; }
                continue;
            }
            if (inString)
            {
                if (c == '\\') { i++; continue; }
                if (c == '"') { inString = false; }
                continue;
            }
            if (c == '/' && next == '/') { inSingleLineComment = true; i++; continue; }
            if (c == '/' && next == '*') { inMultiLineComment = true; i++; continue; }
            if (c == '@' && next == '"') { inVerbatimString = true; i++; continue; }
            if (c == '"') { inString = true; continue; }

            if (c == '{') depth++;
            if (c == '}')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }
}
