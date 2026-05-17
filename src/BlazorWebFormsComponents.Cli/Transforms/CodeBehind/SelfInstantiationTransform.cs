using System.IO;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

public sealed class SelfInstantiationTransform : ICodeBehindTransform
{
    private static readonly Regex ClassNameRegex = new(
        @"(?:public|protected|internal|private)?\s*(?:(?:abstract|sealed|partial|static)\s+)*class\s+(?<name>[A-Za-z_]\w*)",
        RegexOptions.Compiled);

    private static readonly Regex ConstructorRegex = new(
        @"public\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^)]*)\)",
        RegexOptions.Compiled);

    public string Name => "SelfInstantiation";
    public int Order => 218;

    public string Apply(string content, FileMetadata metadata)
    {
        var classMatch = ClassNameRegex.Match(content);
        if (!classMatch.Success)
            return content;

        var className = classMatch.Groups["name"].Value;
        if (string.IsNullOrWhiteSpace(className) || !IsDependencyManagedClass(content, metadata, className))
            return content;

        var rewritten = Regex.Replace(
            content,
            $@"(?<![\w.])new\s+{Regex.Escape(className)}\s*\(\s*\)",
            "this",
            RegexOptions.None,
            TimeSpan.FromMilliseconds(500));

        rewritten = UnwrapUsingBlocks(rewritten, className);
        rewritten = CollapseSelfAliasReturnPatterns(rewritten, className);
        return rewritten;
    }

    private static bool IsDependencyManagedClass(string content, FileMetadata metadata, string className)
    {
        if (content.Contains("[Inject]", StringComparison.Ordinal))
            return true;

        foreach (Match match in ConstructorRegex.Matches(content))
        {
            if (!string.Equals(match.Groups["name"].Value, className, StringComparison.Ordinal))
                continue;

            if (!string.IsNullOrWhiteSpace(match.Groups["params"].Value))
                return true;
        }

        if (IsServicePath(metadata.SourceFilePath) || IsServicePath(metadata.OutputFilePath))
            return true;

        return IsRegisteredInProgram(metadata.OutputRootPath, className);
    }

    private static bool IsServicePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var segments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return segments.Any(segment =>
            string.Equals(segment, "Logic", StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, "BLL", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsRegisteredInProgram(string? outputRootPath, string className)
    {
        if (string.IsNullOrWhiteSpace(outputRootPath))
            return false;

        var programPath = Path.Combine(outputRootPath, "Program.cs");
        if (!File.Exists(programPath))
            return false;

        var programContent = File.ReadAllText(programPath);
        return Regex.IsMatch(
            programContent,
            $@"Add(?:Scoped|Singleton|Transient)\s*<\s*{Regex.Escape(className)}\s*>",
            RegexOptions.None,
            TimeSpan.FromMilliseconds(500));
    }

    private static string UnwrapUsingBlocks(string content, string className)
    {
        var usingRegex = new Regex(
            $@"using\s*\(\s*(?:var|{Regex.Escape(className)})\s+(?<var>[A-Za-z_]\w*)\s*=\s*this\s*\)\s*\{{",
            RegexOptions.Compiled);

        var match = usingRegex.Match(content);
        while (match.Success)
        {
            var openBracePos = content.IndexOf('{', match.Index);
            var closeBracePos = FindMatchingBrace(content, openBracePos);
            if (closeBracePos < 0)
                break;

            var variableName = match.Groups["var"].Value;
            var body = content[(openBracePos + 1)..closeBracePos];
            body = ReplaceSelfAliasReferences(body, variableName);
            body = Regex.Replace(body, $@"\breturn\s+{Regex.Escape(variableName)}\s*;", "return this;");

            content = content[..match.Index] + body + content[(closeBracePos + 1)..];
            match = usingRegex.Match(content);
        }

        return content;
    }

    private static string CollapseSelfAliasReturnPatterns(string content, string className)
    {
        var aliasRegex = new Regex(
            $@"(?<declIndent>^[ \t]*)(?:var|{Regex.Escape(className)})\s+(?<var>[A-Za-z_]\w*)\s*=\s*this\s*;\r?\n(?<body>(?:(?!^[ \t]*return\s+\k<var>\s*;).*(?:\r?\n|$))*?)(?<returnIndent>^[ \t]*)return\s+\k<var>\s*;",
            RegexOptions.Compiled | RegexOptions.Multiline);

        return aliasRegex.Replace(content, match =>
        {
            var variableName = match.Groups["var"].Value;
            var body = ReplaceSelfAliasReferences(match.Groups["body"].Value, variableName);
            var returnIndent = match.Groups["returnIndent"].Value;
            return $"{body}{returnIndent}return this;";
        });
    }

    private static string ReplaceSelfAliasReferences(string body, string variableName)
    {
        return Regex.Replace(body, $@"\b{Regex.Escape(variableName)}\.", "this.");
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

            if (inSingleLineComment)
            {
                if (c == '\n') inSingleLineComment = false;
                continue;
            }

            if (inMultiLineComment)
            {
                if (c == '*' && next == '/')
                {
                    inMultiLineComment = false;
                    i++;
                }
                continue;
            }

            if (inVerbatimString)
            {
                if (c == '"' && next == '"')
                {
                    i++;
                    continue;
                }

                if (c == '"')
                    inVerbatimString = false;
                continue;
            }

            if (inString)
            {
                if (c == '\\')
                {
                    i++;
                    continue;
                }

                if (c == '"')
                    inString = false;
                continue;
            }

            if (c == '/' && next == '/')
            {
                inSingleLineComment = true;
                i++;
                continue;
            }

            if (c == '/' && next == '*')
            {
                inMultiLineComment = true;
                i++;
                continue;
            }

            if (c == '@' && next == '"')
            {
                inVerbatimString = true;
                i++;
                continue;
            }

            if (c == '"')
            {
                inString = true;
                continue;
            }

            if (c == '{')
                depth++;
            else if (c == '}')
            {
                depth--;
                if (depth == 0)
                    return i;
            }
        }

        return -1;
    }
}
