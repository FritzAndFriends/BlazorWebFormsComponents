using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

/// <summary>
/// Converts common Web Forms SelectMethod + QueryString/RouteData pages into
/// SSR-friendly query-bound component properties plus a compile-safe SelectMethod wrapper.
/// </summary>
public sealed class QueryDetailsSemanticPattern : ISemanticPattern
{
    private const string Marker = "TODO(bwfc-query-details)";
    private static readonly Regex SelectMethodRegex = new(
        @"SelectMethod\s*=\s*""(?<method>[A-Za-z_][A-Za-z0-9_]*)""",
        RegexOptions.Compiled);
    private static readonly Regex QueryStringParameterRegex = new(
        @"\[QueryString(?:\(\s*""(?<binding>[^""]+)""\s*\))?\]\s*(?<type>[^,\r\n]+?)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);
    private static readonly Regex RouteDataParameterRegex = new(
        @"\[RouteData\]\s*(?<type>[^,\r\n]+?)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);
    private static readonly Regex TItemRegex = new(
        @"(?:TItem|ItemType)=""(?<type>[^""]+)""",
        RegexOptions.Compiled);
    private static readonly Regex ReturnItemTypeRegex = new(
        @"(?:IQueryable|IEnumerable|List|IList|IReadOnlyList)<(?<type>[^>]+)>",
        RegexOptions.Compiled);

    public string Id => "pattern-query-details";
    public int Order => 100;

    public SemanticPatternMatch Match(SemanticPatternContext context)
    {
        if (context.CodeBehind is null || context.Markup.Contains(Marker, StringComparison.Ordinal))
        {
            return SemanticPatternMatch.NoMatch();
        }

        var candidate = FindCandidate(context);
        return candidate is null
            ? SemanticPatternMatch.NoMatch()
            : SemanticPatternMatch.Match(
                $"Detected query-bound SelectMethod '{candidate.Method.Name}' on {candidate.ControlType}.");
    }

    public SemanticPatternResult Apply(SemanticPatternContext context)
    {
        var candidate = FindCandidate(context);
        if (candidate is null)
        {
            return SemanticPatternResult.FromContext(context);
        }

        var wrapperMethodName = $"{candidate.Method.Name}QueryDetails_SelectMethod";
        var rewrittenMarkup = SelectMethodRegex.Replace(
            context.Markup,
            match => string.Equals(match.Groups["method"].Value, candidate.Method.Name, StringComparison.Ordinal)
                ? $@"SelectMethod=""{wrapperMethodName}"""
                : match.Value,
            1);

        // Inject query parameters and wrapper method into code-behind (not @code block)
        var codeBehind = context.CodeBehind;
        var codeMembers = BuildCodeBehindMembers(candidate, wrapperMethodName);
        if (!string.IsNullOrEmpty(codeBehind))
        {
            codeBehind = CodeBehindInjector.InjectMembers(codeBehind, codeMembers);
        }

        var relativePath = SemanticPatternUtilities.RelativeMarkupPath(context);
        context.Report.AddManualItem(
            relativePath,
            0,
            "bwfc-query-details",
            $"{candidate.Method.Name} was normalized to a query-bound SelectMethod wrapper — port the original query into an injected service or DbContext.",
            "medium");

        if (!string.IsNullOrEmpty(codeBehind) && !codeBehind.Contains(Marker, StringComparison.Ordinal))
        {
            codeBehind = $"// {Marker}: {candidate.Method.Name} now binds through component query properties and a SelectMethod wrapper in the generated .razor file.{Environment.NewLine}{codeBehind}";
        }

        return new SemanticPatternResult(
            rewrittenMarkup,
            codeBehind,
            $"Normalized {candidate.Method.Name} to SelectMethod with {candidate.BoundParameters.Count} query/route binding stub(s).");
    }

    private static QueryDetailsCandidate? FindCandidate(SemanticPatternContext context)
    {
        if (context.CodeBehind is null)
        {
            return null;
        }

        var selectMethodMatches = SelectMethodRegex.Matches(context.Markup);
        if (selectMethodMatches.Count != 1)
        {
            return null;
        }

        var methodName = selectMethodMatches[0].Groups["method"].Value;
        if (!SemanticPatternUtilities.TryExtractMethod(context.CodeBehind, methodName, out var method))
        {
            return null;
        }

        var boundParameters = GetBoundParameters(method.Parameters);
        if (boundParameters.Count == 0)
        {
            return null;
        }

        var tagStart = context.Markup.LastIndexOf('<', selectMethodMatches[0].Index);
        var tagEnd = tagStart >= 0 ? context.Markup.IndexOf('>', tagStart) : -1;
        if (tagStart < 0 || tagEnd < 0)
        {
            return null;
        }

        var tagMarkup = context.Markup[tagStart..(tagEnd + 1)];
        var controlType = Regex.Match(tagMarkup, @"<(?<name>[A-Za-z_][A-Za-z0-9_]*)").Groups["name"].Value;
        if (string.IsNullOrWhiteSpace(controlType))
        {
            return null;
        }

        var itemType = ResolveItemType(tagMarkup, method.ReturnType);
        if (string.IsNullOrWhiteSpace(itemType) || string.Equals(itemType, "object", StringComparison.Ordinal))
        {
            return null;
        }

        return new QueryDetailsCandidate(controlType, method, boundParameters, itemType);
    }

    private static List<BoundParameter> GetBoundParameters(string parameters)
    {
        var boundParameters = new List<BoundParameter>();

        foreach (Match match in QueryStringParameterRegex.Matches(parameters))
        {
            var name = match.Groups["name"].Value;
            boundParameters.Add(new BoundParameter(
                "query",
                match.Groups["type"].Value.Trim(),
                name,
                match.Groups["binding"].Success ? match.Groups["binding"].Value : name,
                SemanticPatternUtilities.ToPropertyName(name)));
        }

        foreach (Match match in RouteDataParameterRegex.Matches(parameters))
        {
            var name = match.Groups["name"].Value;
            boundParameters.Add(new BoundParameter(
                "route",
                match.Groups["type"].Value.Trim(),
                name,
                name,
                SemanticPatternUtilities.ToPropertyName(name)));
        }

        return boundParameters;
    }

    private static string? ResolveItemType(string tagMarkup, string returnType)
    {
        var typeMatch = TItemRegex.Match(tagMarkup);
        if (typeMatch.Success)
        {
            return typeMatch.Groups["type"].Value.Trim();
        }

        var returnTypeMatch = ReturnItemTypeRegex.Match(returnType);
        return returnTypeMatch.Success
            ? returnTypeMatch.Groups["type"].Value.Trim()
            : null;
    }

    private static string BuildCodeBehindMembers(QueryDetailsCandidate candidate, string wrapperMethodName)
    {
        var builder = new StringBuilder();

        foreach (var parameter in candidate.BoundParameters)
        {
            var type = SemanticPatternUtilities.NormalizeBindingType(parameter.Type);
            if (string.Equals(parameter.Kind, "query", StringComparison.Ordinal))
            {
                builder.AppendLine(
                    $"    [Parameter, SupplyParameterFromQuery(Name = \"{parameter.BindingName}\")] public {type} {parameter.PropertyName} {{ get; set; }}");
            }
            else
            {
                builder.AppendLine($"    [Parameter] public {type} {parameter.PropertyName} {{ get; set; }}");
            }
        }

        builder.AppendLine();
        builder.AppendLine(
            $"    private global::System.Linq.IQueryable<{candidate.ItemType}> {wrapperMethodName}(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)");
        builder.AppendLine("    {");
        builder.AppendLine("        totalRowCount = 0;");
        builder.AppendLine(
            $"        // {Marker}: Wrapper delegates to the code-behind {candidate.Method.Name} method.");

        // Build argument list mapping component properties to the original method parameters
        var args = string.Join(", ", candidate.BoundParameters.Select(static p => p.PropertyName));
        builder.AppendLine(
            $"        var query = {candidate.Method.Name}({args});");
        builder.AppendLine(
            $"        if (query != null) totalRowCount = query.Count();");
        builder.AppendLine(
            $"        return query ?? global::System.Linq.Enumerable.Empty<{candidate.ItemType}>().AsQueryable();");
        builder.Append("    }");
        return builder.ToString();
    }

    private sealed record QueryDetailsCandidate(
        string ControlType,
        ExtractedMethod Method,
        IReadOnlyList<BoundParameter> BoundParameters,
        string ItemType);

    private sealed record BoundParameter(
        string Kind,
        string Type,
        string ParameterName,
        string BindingName,
        string PropertyName);
}
