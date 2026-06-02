using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites cart/basket lookups that rely on Session.Id to a stable session-backed
/// cart key helper so SSR cart flows do not depend on the raw session identifier.
/// </summary>
public class CartSessionKeyTransform : ICodeBehindTransform
{
    public string Name => "CartSessionKey";
    public int Order => 390;

    private const string HelperMethodName = "GetOrCreateCartKey";
    private const string HelperSignature = "private string GetOrCreateCartKey()";

    private static readonly Regex RawSessionIdRegex = new(
        @"\b(?:this\.Session\.Id|Page\.Session\.Id|Session\.Id|HttpContext(?:\.Current)?\.Session\.Id)\b",
        RegexOptions.Compiled);

    private static readonly Regex CartContextStatementRegex = new(
        @"^(?<indent>[ \t]*)(?<statement>(?=[^;\r\n]*\b\w*(?:cart|basket)\w*\b)[^;\r\n]*\b(?:this\.Session\.Id|Page\.Session\.Id|Session\.Id|HttpContext(?:\.Current)?\.Session\.Id)\b[^;\r\n]*;)",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private|protected)\s+(?:partial\s+)?class\s+\w+[^\{]*\{)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!metadata.OutputFilePath.EndsWith(".razor.cs", StringComparison.OrdinalIgnoreCase))
            return content;

        if (!RawSessionIdRegex.IsMatch(content))
            return content;

        var replacements = 0;
        content = CartContextStatementRegex.Replace(content, match =>
        {
            var statement = match.Groups["statement"].Value;
            var trimmed = statement.TrimStart();
            if (trimmed.StartsWith("//", StringComparison.Ordinal) ||
                trimmed.StartsWith("/*", StringComparison.Ordinal) ||
                statement.Contains($"{HelperMethodName}()", StringComparison.Ordinal))
            {
                return match.Value;
            }

            var replacedStatement = RawSessionIdRegex.Replace(statement, $"{HelperMethodName}()");
            if (string.Equals(replacedStatement, statement, StringComparison.Ordinal))
                return match.Value;

            replacements++;
            return match.Groups["indent"].Value + replacedStatement;
        });

        if (replacements == 0)
            return content;

        if (!content.Contains(HelperSignature, StringComparison.Ordinal) && ClassOpenRegex.IsMatch(content))
        {
            var helperBlock = "\n\n    private string GetOrCreateCartKey()\n    {\n        var cartKey = Session[\"cart-key\"]?.ToString();\n        if (string.IsNullOrEmpty(cartKey))\n        {\n            cartKey = global::System.Guid.NewGuid().ToString();\n            Session[\"cart-key\"] = cartKey;\n        }\n\n        return cartKey;\n    }\n";

            content = ClassOpenRegex.Replace(content, "$1" + helperBlock, 1);
        }

        return content;
    }
}
