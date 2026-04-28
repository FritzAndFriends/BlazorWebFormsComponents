using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

/// <summary>
/// Rewrites blank redirect/action-only pages into visible SSR handler stubs so the
/// generated project contains actionable migration guidance instead of inert output.
/// </summary>
public sealed class ActionPagesSemanticPattern : ISemanticPattern
{
    private const string Marker = "TODO(bwfc-action-pages)";
    private static readonly Regex QueryStringAccessRegex = new(
        @"Request\.QueryString\[""(?<name>[^""]+)""\]",
        RegexOptions.Compiled);
    private static readonly Regex RedirectLiteralRegex = new(
        @"Response\.Redirect\(\s*""(?<target>[^""]+)""",
        RegexOptions.Compiled);
    private static readonly Regex ActionCallRegex = new(
        @"\b(?<action>AddToCart|RemoveFromCart|UpdateShoppingCartDatabase|Process\w+)\s*\(",
        RegexOptions.Compiled);
    private static readonly Regex PageAndTitleRegex = new(
        @"(?is)^\s*@page\s+""[^""]+""\s*|<PageTitle>.*?</PageTitle>",
        RegexOptions.Compiled);
    private static readonly Regex WrapperTagRegex = new(
        @"(?is)</?(html|head|body|form|div|span|section|main|title)(?:\s[^>]*)?>",
        RegexOptions.Compiled);

    public string Id => "pattern-action-pages";
    public int Order => 200;

    public SemanticPatternMatch Match(SemanticPatternContext context)
    {
        if (context.SourceFile.FileType != FileType.Page || context.CodeBehind is null || context.Markup.Contains(Marker, StringComparison.Ordinal))
        {
            return SemanticPatternMatch.NoMatch();
        }

        var candidate = FindCandidate(context);
        return candidate is null
            ? SemanticPatternMatch.NoMatch()
            : SemanticPatternMatch.Match(
                $"Detected action-only page '{candidate.PageName}' that redirects to '{candidate.RedirectTarget ?? "manual target"}'.");
    }

    public SemanticPatternResult Apply(SemanticPatternContext context)
    {
        var candidate = FindCandidate(context);
        if (candidate is null)
        {
            return SemanticPatternResult.FromContext(context);
        }

        var relativePath = SemanticPatternUtilities.RelativeMarkupPath(context);
        context.Report.AddManualItem(
            relativePath,
            0,
            "bwfc-action-pages",
            $"{candidate.PageName} is an action-only page — move the side effect into a scoped service or minimal API endpoint before redirecting.",
            "high");

        var markup = BuildMarkup(context.Markup, candidate);
        var codeBehind = context.CodeBehind;
        if (!string.IsNullOrEmpty(codeBehind) && !codeBehind.Contains(Marker, StringComparison.Ordinal))
        {
            var redirectSummary = candidate.RedirectTarget is null
                ? "manual redirect target"
                : candidate.RedirectTarget;
            codeBehind = $"// {Marker}: {candidate.PageName} now renders a visible SSR handler stub. Preserve the side effect, then redirect to {redirectSummary}.{Environment.NewLine}{codeBehind}";
        }

        return new SemanticPatternResult(
            markup,
            codeBehind,
            $"Converted action-only page {candidate.PageName} into an SSR handler stub.");
    }

    private static ActionPageCandidate? FindCandidate(SemanticPatternContext context)
    {
        if (context.CodeBehind is null || !IsInertMarkup(context.Markup))
        {
            return null;
        }

        var redirectMatch = RedirectLiteralRegex.Match(context.CodeBehind);
        if (!redirectMatch.Success)
        {
            return null;
        }

        var queryKeys = QueryStringAccessRegex.Matches(context.CodeBehind)
            .Select(static match => match.Groups["name"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var actionCall = ActionCallRegex.Match(context.CodeBehind);
        var pageName = Path.GetFileNameWithoutExtension(context.SourceFile.MarkupPath);
        return new ActionPageCandidate(
            pageName,
            queryKeys,
            SemanticPatternUtilities.NormalizeRoute(redirectMatch.Groups["target"].Value),
            actionCall.Success ? actionCall.Groups["action"].Value : null);
    }

    private static bool IsInertMarkup(string markup)
    {
        var stripped = PageAndTitleRegex.Replace(markup, string.Empty);
        stripped = WrapperTagRegex.Replace(stripped, string.Empty);
        stripped = Regex.Replace(stripped, @"\s+|&nbsp;", string.Empty, RegexOptions.IgnoreCase);
        return string.IsNullOrEmpty(stripped);
    }

    private static string BuildMarkup(string existingMarkup, ActionPageCandidate candidate)
    {
        var pageDirective = SemanticPatternUtilities.ExtractPageDirective(existingMarkup, candidate.PageName);
        var title = $"{candidate.PageName} handler";
        var builder = new StringBuilder();
        builder.AppendLine(pageDirective);
        builder.AppendLine("@inject NavigationManager Navigation");
        builder.AppendLine();
        builder.AppendLine($"<PageTitle>{title}</PageTitle>");
        builder.AppendLine();
        builder.AppendLine("<div class=\"alert alert-warning\">");
        builder.AppendLine($"    <h1>{candidate.PageName} needs an SSR action rewrite</h1>");
        builder.AppendLine(
            "    <p>Web Forms used this page as an action-only endpoint. Move the side effect into a scoped service or minimal API endpoint, then redirect after the action succeeds.</p>");
        if (candidate.QueryKeys.Length > 0)
        {
            builder.AppendLine(
                $"    <p>Detected query string inputs: {string.Join(", ", candidate.QueryKeys)}.</p>");
        }

        if (!string.IsNullOrEmpty(candidate.ActionCall))
        {
            builder.AppendLine($"    <p>Detected side effect: <code>{candidate.ActionCall}</code>.</p>");
        }

        if (!string.IsNullOrEmpty(candidate.RedirectTarget))
        {
            builder.AppendLine(
                $"    <p>Expected redirect target: <a href=\"{candidate.RedirectTarget}\">{candidate.RedirectTarget}</a>.</p>");
        }

        builder.AppendLine("</div>");

        if (!string.IsNullOrEmpty(candidate.RedirectTarget))
        {
            builder.AppendLine();
        builder.AppendLine(
            $"<p><a class=\"btn btn-primary\" href=\"{candidate.RedirectTarget}\">Continue to {candidate.RedirectTarget.TrimStart('/')}</a></p>");
        }

        builder.AppendLine();
        builder.AppendLine("@code {");
        foreach (var queryKey in candidate.QueryKeys)
        {
            var propertyName = SemanticPatternUtilities.ToPropertyName(queryKey);
            builder.AppendLine(
                $"    [Parameter, SupplyParameterFromQuery(Name = \"{queryKey}\")] public string? {propertyName} {{ get; set; }}");
        }

        if (candidate.QueryKeys.Length > 0)
        {
            builder.AppendLine();
        }

        if (!string.IsNullOrEmpty(candidate.RedirectTarget))
        {
            builder.AppendLine(
                $"    private const string RedirectTarget = \"{candidate.RedirectTarget}\";");
            builder.AppendLine();
        }

        builder.AppendLine("    protected override void OnParametersSet()");
        builder.AppendLine("    {");
        builder.AppendLine(
            $"        // {Marker}: move the original Web Forms side effect out of {candidate.PageName} and into a scoped service or minimal API endpoint.");
        if (!string.IsNullOrEmpty(candidate.RedirectTarget))
        {
            builder.AppendLine(
                "        // After the action succeeds, call Navigation.NavigateTo(RedirectTarget, replace: true).");
        }
        builder.AppendLine("    }");
        builder.Append('}');
        return builder.ToString();
    }

    private sealed record ActionPageCandidate(
        string PageName,
        string[] QueryKeys,
        string? RedirectTarget,
        string? ActionCall);
}
