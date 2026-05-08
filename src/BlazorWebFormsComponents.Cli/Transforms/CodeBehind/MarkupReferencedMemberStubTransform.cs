using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Adds compile-safe fallback members for identifiers that the converted markup
/// still references after the deterministic code-behind transform pass.
/// </summary>
public class MarkupReferencedMemberStubTransform : ICodeBehindTransform
{
    public string Name => "MarkupReferencedMemberStub";
    public int Order => 900;

    private static readonly Regex ClassOpenRegex = new(
        @"partial\s+class\s+\w+[^\{]*\{",
        RegexOptions.Compiled);

    private static readonly Regex MethodCallRegex = new(
        @"(?<![\w.])@(?<name>[A-Za-z_]\w*)\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex FieldReferenceRegex = new(
        @"(?<![\w.])@(?<name>_[A-Za-z]\w*)\b",
        RegexOptions.Compiled);

    private static readonly Regex ParenthesizedFieldReferenceRegex = new(
        @"@\(\s*(?<name>_[A-Za-z]\w*)\s*\)",
        RegexOptions.Compiled);

    private static readonly Regex EventHandlerRegex = new(
        @"\b(?:OnClick|OnCommand|OnTextChanged|OnSelectedIndexChanged|OnCheckedChanged|OnRowCommand|OnRowEditing|OnRowUpdating|OnRowCancelingEdit|OnRowDeleting|OnRowDataBound|OnPageIndexChanging|OnSorting|OnItemCommand|OnItemDataBound|OnDataBound|OnLoad|OnInit|OnPreRender|OnSelectedDateChanged|OnDayRender|OnVisibleMonthChanged|OnServerValidate|OnCreatingUser|OnCreatedUser|OnAuthenticate|OnLoggedIn|OnLoggingIn)\s*=\s*""@(?<name>[A-Za-z_]\w*)""",
        RegexOptions.Compiled);

    private static readonly HashSet<string> RazorKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "await",
        "else",
        "for",
        "foreach",
        "if",
        "inherits",
        "inject",
        "layout",
        "namespace",
        "page",
        "section",
        "switch",
        "using",
        "while"
    };

    public string Apply(string content, FileMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata.MarkupContent))
        {
            return content;
        }

        var classMatch = ClassOpenRegex.Match(content);
        if (!classMatch.Success)
        {
            return content;
        }

        var classOpenBraceIndex = content.IndexOf('{', classMatch.Index);
        if (classOpenBraceIndex < 0)
        {
            return content;
        }

        var classCloseBraceIndex = FindMatchingBrace(content, classOpenBraceIndex);
        if (classCloseBraceIndex < 0)
        {
            return content;
        }

        var fieldNames = CollectMatches(metadata.MarkupContent, FieldReferenceRegex)
            .Concat(CollectMatches(metadata.MarkupContent, ParenthesizedFieldReferenceRegex))
            .Distinct(StringComparer.Ordinal)
            .Where(name => !HasDeclaredMember(content, name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        var eventHandlerNames = CollectMatches(metadata.MarkupContent, EventHandlerRegex)
            .Distinct(StringComparer.Ordinal)
            .Where(name => !HasDeclaredMethod(content, name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        var renderMethodNames = CollectMatches(metadata.MarkupContent, MethodCallRegex)
            .Where(name => !RazorKeywords.Contains(name))
            .Where(name => !eventHandlerNames.Contains(name, StringComparer.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .Where(name => !HasDeclaredMethod(content, name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        if (fieldNames.Count == 0 && renderMethodNames.Count == 0 && eventHandlerNames.Count == 0)
        {
            return content;
        }

        var stubs = new List<string>();
        stubs.AddRange(fieldNames.Select(name => $"    private object? {name}; // TODO: migrate from Web Forms code-behind"));
        stubs.AddRange(renderMethodNames.Select(CreateRenderMethodStub));
        stubs.AddRange(eventHandlerNames.Select(CreateEventHandlerStub));

        var stubBlock = Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine + Environment.NewLine, stubs) + Environment.NewLine;
        return content.Insert(classCloseBraceIndex, stubBlock);
    }

    private static IEnumerable<string> CollectMatches(string markup, Regex regex)
    {
        foreach (Match match in regex.Matches(markup))
        {
            var name = match.Groups["name"].Value;
            if (!string.IsNullOrWhiteSpace(name))
            {
                yield return name;
            }
        }
    }

    private static string CreateRenderMethodStub(string name) =>
        $"    protected object? {name}()\n    {{\n        // TODO: migrate from Web Forms code-behind\n        return null;\n    }}";

    private static string CreateEventHandlerStub(string name) =>
        $"    protected void {name}(object? sender, EventArgs e)\n    {{\n        // TODO: migrate from Web Forms code-behind\n    }}";

    private static bool HasDeclaredMember(string content, string name)
    {
        var escapedName = Regex.Escape(name);
        return Regex.IsMatch(
                   content,
                   $@"(?:public|protected|private|internal)\s+(?:static\s+|readonly\s+|const\s+|volatile\s+|new\s+)*[\w<>,?.\[\]]+\s+{escapedName}\s*(?:[;={{])",
                   RegexOptions.Multiline)
               || HasDeclaredMethod(content, name);
    }

    private static bool HasDeclaredMethod(string content, string name)
    {
        var escapedName = Regex.Escape(name);
        return Regex.IsMatch(
            content,
            $@"(?:public|protected|private|internal)\s+(?:static\s+|async\s+|virtual\s+|override\s+|sealed\s+|partial\s+|new\s+)*[\w<>,?.\[\]]+\s+{escapedName}\s*\(",
            RegexOptions.Multiline);
    }

    private static int FindMatchingBrace(string content, int openBraceIndex)
    {
        var depth = 0;
        for (var i = openBraceIndex; i < content.Length; i++)
        {
            if (content[i] == '{')
            {
                depth++;
            }
            else if (content[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
