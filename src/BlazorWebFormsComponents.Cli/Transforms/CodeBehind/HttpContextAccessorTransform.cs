using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces legacy <c>HttpContext.Current</c> access patterns with
/// <c>IHttpContextAccessor</c>-based equivalents, enabling non-page .cs files
/// to pass the compile-surface check instead of being quarantined.
///
/// Common patterns handled:
/// <list type="bullet">
///   <item><c>HttpContext.Current.Session["key"]</c> → <c>_httpContextAccessor.HttpContext?.Session.GetString("key")</c></item>
///   <item><c>HttpContext.Current.User.Identity.Name</c> → <c>_httpContextAccessor.HttpContext?.User?.Identity?.Name</c></item>
///   <item><c>HttpContext.Current.Request</c> → <c>_httpContextAccessor.HttpContext?.Request</c></item>
///   <item><c>HttpContext.Current.Response</c> → <c>_httpContextAccessor.HttpContext?.Response</c></item>
/// </list>
/// </summary>
public class HttpContextAccessorTransform : ICodeBehindTransform
{
    public string Name => "HttpContextAccessor";
    public int Order => 108; // After DbContextInstantiation (107)

    // Detects page/component classes that use [Inject] DI pattern
    private static readonly Regex PageBaseRegex = new(
        @":\s*(?:WebFormsPageBase|ComponentBase|LayoutComponentBase|BaseWebFormsComponent)\b",
        RegexOptions.Compiled);

    // Matches any HttpContext.Current usage (including System.Web.HttpContext.Current)
    private static readonly Regex HttpContextCurrentRegex = new(
        @"(?:System\.Web\.)?HttpContext\.Current",
        RegexOptions.Compiled);

    // Session indexer: HttpContext.Current.Session["key"]
    private static readonly Regex SessionIndexerRegex = new(
        @"(?:System\.Web\.)?HttpContext\.Current\.Session\[(?<key>[^\]]+)\]",
        RegexOptions.Compiled);

    // Session assignment: HttpContext.Current.Session["key"] = value;
    // Uses (?!=) negative lookahead after first = to exclude == (comparison) and != patterns
    private static readonly Regex SessionAssignRegex = new(
        @"(?:System\.Web\.)?HttpContext\.Current\.Session\[(?<key>[^\]]+)\]\s*(?<!=)=(?!=)\s*(?<value>[^;]+);",
        RegexOptions.Compiled);

    // Matches class declaration to find insertion point
    private static readonly Regex ClassBodyRegex = new(
        @"((?:partial\s+)?class\s+(?<className>\w+)[^{]*\{)",
        RegexOptions.Compiled);

    private const string FieldName = "_httpContextAccessor";
    private const string AccessorType = "IHttpContextAccessor";

    public string Apply(string content, FileMetadata metadata)
    {
        if (!HttpContextCurrentRegex.IsMatch(content))
            return content;

        // Don't transform actual page/control/master code-behinds — WebFormsPageBase provides HttpContext shims.
        // Check by source file extension (reliable) rather than FileType (which SourceFileCopier sets to Page for all files).
        // Also check for WebFormsPageBase in content as a fallback.
        var sourcePath = metadata.SourceFilePath ?? "";
        var isPageCodeBehind = sourcePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".master", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase)
            || PageBaseRegex.IsMatch(content);
        if (isPageCodeBehind)
            return content;

        // Replace Session assignments: HttpContext.Current.Session["key"] = value;
        content = SessionAssignRegex.Replace(content, m =>
        {
            var key = m.Groups["key"].Value;
            var value = m.Groups["value"].Value.Trim();
            return $"{FieldName}.HttpContext?.Session.SetString({key}, {value}?.ToString() ?? string.Empty);";
        });

        // Replace Session reads: HttpContext.Current.Session["key"]
        content = SessionIndexerRegex.Replace(content, m =>
        {
            var key = m.Groups["key"].Value;
            return $"{FieldName}.HttpContext?.Session.GetString({key})";
        });

        // Replace HttpContext.Current.User.Identity.Name → _httpContextAccessor.HttpContext?.User?.Identity?.Name
        content = Regex.Replace(content,
            @"(?:System\.Web\.)?HttpContext\.Current\.User\.Identity\.Name",
            $"{FieldName}.HttpContext?.User?.Identity?.Name");

        // Replace HttpContext.Current.User.Identity.IsAuthenticated
        content = Regex.Replace(content,
            @"(?:System\.Web\.)?HttpContext\.Current\.User\.Identity\.IsAuthenticated",
            $"({FieldName}.HttpContext?.User?.Identity?.IsAuthenticated ?? false)");

        // Replace HttpContext.Current.User → _httpContextAccessor.HttpContext?.User
        content = Regex.Replace(content,
            @"(?:System\.Web\.)?HttpContext\.Current\.User\b",
            $"{FieldName}.HttpContext?.User");

        // Replace remaining HttpContext.Current.Property → _httpContextAccessor.HttpContext?.Property
        // Skip HttpContext.Current.Server — handled by ServerShimTransform (order 330)
        content = Regex.Replace(content,
            @"(?:System\.Web\.)?HttpContext\.Current\.(?!Server\b)(?<prop>\w+)",
            m => $"{FieldName}.HttpContext?.{m.Groups["prop"].Value}");

        // Replace bare HttpContext.Current → _httpContextAccessor.HttpContext
        // Skip if followed by .Server (handled by ServerShimTransform)
        content = Regex.Replace(content,
            @"(?:System\.Web\.)?HttpContext\.Current(?!\.Server\b)",
            $"{FieldName}.HttpContext");

        // Remove System.Web.HttpContext method parameters — the class now uses injected _httpContextAccessor
        // e.g., GetCart(HttpContext context) → GetCart()
        // Also handles fully-qualified System.Web.HttpContext parameters
        content = Regex.Replace(content,
            @",\s*(?:System\.Web\.)?HttpContext\s+\w+",
            "");
        content = Regex.Replace(content,
            @"(?:System\.Web\.)?HttpContext\s+\w+\s*,\s*",
            "");
        // Sole parameter case: (HttpContext context) → ()
        content = Regex.Replace(content,
            @"\(\s*(?:System\.Web\.)?HttpContext\s+\w+\s*\)",
            "()");

        // Remove System.Web using if present (before injection adds Microsoft.AspNetCore.Http)
        content = Regex.Replace(content, @"^\s*using\s+System\.Web\s*;\s*\r?\n", "", RegexOptions.Multiline);

        // Check if the accessor field is actually referenced (HttpContext.Current may have been
        // fully handled by ServerShimTransform's skip pattern above, leaving nothing to inject for)
        if (!content.Contains(FieldName, StringComparison.Ordinal))
            return content;

        // Detect if all usages of the accessor are inside static methods
        var usesInStaticMethod = UsesAccessorInStaticMethod(content);

        // Inject the IHttpContextAccessor field/constructor
        if (!content.Contains(AccessorType, StringComparison.Ordinal))
        {
            var classMatch = ClassBodyRegex.Match(content);
            if (classMatch.Success)
            {
                var className = classMatch.Groups["className"].Value;
                var insertPos = classMatch.Index + classMatch.Length;

                if (usesInStaticMethod)
                {
                    // Static field + Configure() pattern for classes with static methods
                    var staticField = $"\n    private static {AccessorType} {FieldName} = default!;\n";
                    var configureMethod = $"\n    public static void Configure({AccessorType} httpContextAccessor)\n    {{\n        {FieldName} = httpContextAccessor;\n    }}\n";
                    content = content[..insertPos] + staticField + configureMethod + content[insertPos..];
                }
                else
                {
                    // Instance field + constructor injection (original pattern)
                    var field = $"\n    private readonly {AccessorType} {FieldName};\n";

                    // Check if constructor already exists
                    var existingCtorRegex = new Regex(
                        $@"public\s+{Regex.Escape(className)}\s*\(([^)]*)\)",
                        RegexOptions.Compiled);
                    var ctorMatch = existingCtorRegex.Match(content);
                    if (ctorMatch.Success)
                    {
                        // Add parameter to existing constructor
                        var existingParams = ctorMatch.Groups[1].Value.Trim();
                        var paramName = "httpContextAccessor";
                        var newParams = string.IsNullOrEmpty(existingParams)
                            ? $"{AccessorType} {paramName}"
                            : $"{existingParams}, {AccessorType} {paramName}";
                        content = content[..ctorMatch.Index]
                            + $"public {className}({newParams})"
                            + content[(ctorMatch.Index + ctorMatch.Length)..];

                        // Add field assignment in constructor body
                        var ctorBodyStart = content.IndexOf('{', ctorMatch.Index) + 1;
                        content = content[..ctorBodyStart]
                            + $"\n        {FieldName} = {paramName};"
                            + content[ctorBodyStart..];
                    }
                    else
                    {
                        // Create new constructor
                        var paramName = "httpContextAccessor";
                        var ctor = $"\n    public {className}({AccessorType} {paramName})\n    {{\n        {FieldName} = {paramName};\n    }}\n";
                        content = content[..insertPos] + field + ctor + content[insertPos..];
                    }

                    // Add field if not yet inserted
                    if (!content.Contains($"readonly {AccessorType} {FieldName}", StringComparison.Ordinal))
                    {
                        content = content[..insertPos] + field + content[insertPos..];
                    }
                }
            }
        }

        // Add Microsoft.AspNetCore.Http using if needed
        if (!content.Contains("using Microsoft.AspNetCore.Http;", StringComparison.Ordinal))
        {
            var lastUsing = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                content = content[..insertAt] + "\nusing Microsoft.AspNetCore.Http;" + content[insertAt..];
            }
        }

        return content;
    }

    /// <summary>
    /// Detects whether the accessor field reference appears inside a static method,
    /// indicating the class needs a static Configure() pattern instead of constructor injection.
    /// </summary>
    private static bool UsesAccessorInStaticMethod(string content)
    {
        // Find all static method bodies and check if they reference the field
        var staticMethodRegex = new Regex(
            @"\bstatic\b[^{]*\{",
            RegexOptions.Compiled);

        foreach (Match methodMatch in staticMethodRegex.Matches(content))
        {
            // Find the matching closing brace for this method
            var braceStart = methodMatch.Index + methodMatch.Length - 1;
            var depth = 1;
            var pos = braceStart + 1;
            while (pos < content.Length && depth > 0)
            {
                if (content[pos] == '{') depth++;
                else if (content[pos] == '}') depth--;
                pos++;
            }

            var methodBody = content[braceStart..pos];
            if (methodBody.Contains(FieldName, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}
