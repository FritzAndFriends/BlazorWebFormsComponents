using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects DataSource assignments on @ref component fields inside OnInitializedAsync
/// and refactors them to backing-field + markup parameter binding.
///
/// Gate: Only applies when:
///   1. The assignment target is a known @ref field (from metadata.ComponentRefs)
///   2. The assignment is inside OnInitializedAsync/OnInitialized body
///   3. The assignment uses null-conditional (?.) indicating the ref may be null
///
/// Pattern detected:
///   this.gridRef?.DataSource = someExpression;
///   gridRef?.DataSource = someExpression;
///
/// Refactored to:
///   Code-behind: private object _gridRef_DataSource = null!;  (field declaration)
///                _gridRef_DataSource = someExpression;         (in OnInitializedAsync)
///   Markup:      DataSource="@_gridRef_DataSource"            (injected into component tag)
/// </summary>
public class DataSourceParameterBindingTransform : ICodeBehindTransform
{
    public string Name => "DataSourceParameterBinding";
    public int Order => 810; // After DataBindTransform (800), after ComponentRefNullSafety (605)

    // Matches: controlId?.DataSource = expression; (with optional this. prefix)
    private static readonly Regex NullSafeDataSourceAssignRegex = new(
        @"[ \t]*(?:this\.)?(\w+)\?\.DataSource\s*=\s*(.+?)\s*;[ \t]*\r?\n?",
        RegexOptions.Compiled);

    // Matches OnInitializedAsync or OnInitialized method signature
    private static readonly Regex LifecycleMethodRegex = new(
        @"(?:override\s+(?:async\s+)?(?:Task|void)\s+(?:OnInitializedAsync|OnInitialized)\s*\()",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.ComponentRefs.Count == 0)
            return content;

        if (!LifecycleMethodRegex.IsMatch(content))
            return content;

        var markup = metadata.MarkupContent;
        if (string.IsNullOrEmpty(markup))
            return content;

        // Find OnInitializedAsync body
        var lifecycleMatch = LifecycleMethodRegex.Match(content);
        if (!lifecycleMatch.Success)
            return content;

        var bracePos = content.IndexOf('{', lifecycleMatch.Index + lifecycleMatch.Length);
        if (bracePos < 0) return content;

        var closingBrace = FindMatchingBrace(content, bracePos);
        if (closingBrace < 0) return content;

        var bodyStart = bracePos + 1;
        var bodyEnd = closingBrace;
        var body = content[bodyStart..bodyEnd];

        // Find null-safe DataSource assignments on @ref fields within this method
        var matches = NullSafeDataSourceAssignRegex.Matches(body);
        if (matches.Count == 0)
            return content;

        var fieldsToAdd = new List<(string controlId, string fieldName, string expression)>();
        var modifiedBody = body;

        foreach (Match m in matches)
        {
            var controlId = m.Groups[1].Value;
            var expression = m.Groups[2].Value;

            // Gate: only process known @ref fields
            if (!metadata.ComponentRefs.ContainsKey(controlId))
                continue;

            var fieldName = $"_{controlId}_DataSource";
            fieldsToAdd.Add((controlId, fieldName, expression));

            // Replace the null-safe assignment with a backing field assignment
            modifiedBody = modifiedBody.Replace(m.Value, $"            {fieldName} = {expression};\n");
        }

        if (fieldsToAdd.Count == 0)
            return content;

        // Apply body changes
        content = content[..bodyStart] + modifiedBody + content[bodyEnd..];

        // Add backing field declarations after the @ref field declarations
        foreach (var (controlId, fieldName, _) in fieldsToAdd)
        {
            // Find the @ref field declaration to insert after
            var refFieldPattern = new Regex(
                $@"([ \t]*private\s+\w+(?:<\w+>)?\s+{Regex.Escape(controlId)}\s*=\s*default!;\s*\r?\n)",
                RegexOptions.Compiled);

            var refMatch = refFieldPattern.Match(content);
            if (refMatch.Success)
            {
                var insertPos = refMatch.Index + refMatch.Length;
                var indent = "    ";
                // Detect indent from the matched line
                var lineMatch = Regex.Match(refMatch.Value, @"^([ \t]*)");
                if (lineMatch.Success) indent = lineMatch.Groups[1].Value;

                content = content[..insertPos] +
                    $"{indent}private object {fieldName} = null!;\n" +
                    content[insertPos..];
            }
            else
            {
                // Fallback: insert before the class closing brace
                var classBodyPattern = new Regex(@"(\s+private\s+\w+)", RegexOptions.Compiled);
                var firstField = classBodyPattern.Match(content);
                if (firstField.Success)
                {
                    var insertPos = firstField.Index;
                    content = content[..insertPos] +
                        $"\n    private object {fieldName} = null!;\n" +
                        content[insertPos..];
                }
            }

            // Inject DataSource="@fieldName" into the markup component tag
            // Find the component by @ref="controlId"
            var markupRefPattern = new Regex(
                $@"(<\w+[^>]*@ref\s*=\s*""{Regex.Escape(controlId)}""[^>]*?)(\/?>)",
                RegexOptions.Compiled);

            if (markupRefPattern.IsMatch(markup))
            {
                // Don't inject if DataSource is already present
                var hasDataSource = Regex.IsMatch(markup,
                    $@"<\w+[^>]*@ref\s*=\s*""{Regex.Escape(controlId)}""[^>]*DataSource\s*=",
                    RegexOptions.Compiled);

                if (!hasDataSource)
                {
                    markup = markupRefPattern.Replace(markup, m =>
                    {
                        // Insert before the closing > or />
                        return $"{m.Groups[1].Value}\n                    DataSource=\"@{fieldName}\"{m.Groups[2].Value}";
                    }, 1);
                }
            }
        }

        // Also remove any remaining plain DataSource assignments that were already captured
        // (non-null-safe ones that DataBindTransform might have left with the controlId in DataBindMap)
        foreach (var (controlId, fieldName, _) in fieldsToAdd)
        {
            if (metadata.DataBindMap.ContainsKey(controlId))
            {
                metadata.DataBindMap.Remove(controlId);
            }
        }

        metadata.MarkupContent = markup;
        return content;
    }

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

            if (inSingleLineComment) { if (c == '\n') inSingleLineComment = false; continue; }
            if (inMultiLineComment) { if (c == '*' && next == '/') { inMultiLineComment = false; i++; } continue; }
            if (inVerbatimString) { if (c == '"' && next == '"') { i++; continue; } if (c == '"') inVerbatimString = false; continue; }
            if (inString) { if (c == '\\') { i++; continue; } if (c == '"') inString = false; continue; }
            if (c == '/' && next == '/') { inSingleLineComment = true; i++; continue; }
            if (c == '/' && next == '*') { inMultiLineComment = true; i++; continue; }
            if (c == '@' && next == '"') { inVerbatimString = true; i++; continue; }
            if (c == '"') { inString = true; continue; }

            if (c == '{') depth++;
            if (c == '}') { depth--; if (depth == 0) return i; }
        }
        return -1;
    }
}
