using System.Text;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

internal static class SemanticPatternUtilities
{
    public static string RelativeMarkupPath(SemanticPatternContext context) =>
        Path.GetRelativePath(context.MigrationContext.SourcePath, context.SourceFile.MarkupPath);

    public static string ToPropertyName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Value";
        }

        var parts = Regex.Matches(value, @"[A-Za-z0-9]+")
            .Select(static match => match.Value)
            .Where(static part => !string.IsNullOrWhiteSpace(part))
            .ToArray();

        if (parts.Length == 0)
        {
            return "Value";
        }

        var builder = new StringBuilder();
        foreach (var part in parts)
        {
            builder.Append(char.ToUpperInvariant(part[0]));
            if (part.Length > 1)
            {
                builder.Append(part[1..]);
            }
        }

        return builder.ToString();
    }

    public static string NormalizeBindingType(string type)
    {
        var trimmed = type.Trim();
        return string.Equals(trimmed, "string", StringComparison.Ordinal)
            ? "string?"
            : trimmed;
    }

    public static string NormalizeRoute(string target)
    {
        var trimmed = target.Trim();
        if (trimmed.StartsWith("~/", StringComparison.Ordinal))
        {
            trimmed = "/" + trimmed[2..];
        }

        if (!trimmed.StartsWith("/", StringComparison.Ordinal))
        {
            trimmed = "/" + trimmed;
        }

        if (trimmed.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[..^5];
        }

        return trimmed;
    }

    public static string ExtractPageDirective(string markup, string pageName)
    {
        var pageDirective = Regex.Match(markup, @"(?m)^@page\s+""[^""]+""\s*$");
        return pageDirective.Success
            ? pageDirective.Value
            : $@"@page ""/{pageName}""";
    }

    public static bool TryExtractMethod(string content, string methodName, out ExtractedMethod method)
    {
        method = default!;

        var signatureRegex = new Regex(
            $@"(?s)(?<signature>(?:public|protected|internal|private)\s+(?:static\s+)?(?:async\s+)?(?<returnType>.+?)\s+{Regex.Escape(methodName)}\s*\((?<parameters>.*?)\))\s*\{{",
            RegexOptions.Compiled);

        var match = signatureRegex.Match(content);
        if (!match.Success)
        {
            return false;
        }

        var openBraceIndex = match.Index + match.Length - 1;
        var closeBraceIndex = FindMatchingBrace(content, openBraceIndex);
        if (closeBraceIndex < 0)
        {
            return false;
        }

        method = new ExtractedMethod(
            methodName,
            match.Groups["returnType"].Value.Trim(),
            match.Groups["parameters"].Value,
            content[match.Index..(closeBraceIndex + 1)]);
        return true;
    }

    public static int FindMatchingBrace(string content, int openBraceIndex)
    {
        var depth = 0;
        var inString = false;
        var stringDelimiter = '\0';
        var escapeNext = false;

        for (var index = openBraceIndex; index < content.Length; index++)
        {
            var current = content[index];

            if (inString)
            {
                if (escapeNext)
                {
                    escapeNext = false;
                    continue;
                }

                if (current == '\\')
                {
                    escapeNext = true;
                    continue;
                }

                if (current == stringDelimiter)
                {
                    inString = false;
                }

                continue;
            }

            if (current is '"' or '\'')
            {
                inString = true;
                stringDelimiter = current;
                continue;
            }

            if (current == '{')
            {
                depth++;
            }
            else if (current == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return index;
                }
            }
        }

        return -1;
    }
}

internal sealed record ExtractedMethod(
    string Name,
    string ReturnType,
    string Parameters,
    string FullText);
