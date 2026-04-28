using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

public sealed class AccountPagesSemanticPattern : ISemanticPattern
{
    private static readonly Regex LabelRegex = new(
        @"<Label\b[^>]*AssociatedControlID=""(?<id>[^""]+)""[^>]*>(?<text>[\s\S]*?)</Label>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex TextBoxRegex = new(
        @"<TextBox\b[^>]*/>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex CheckBoxRegex = new(
        @"<CheckBox\b[^>]*/>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ButtonRegex = new(
        @"<Button\b[^>]*/>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ValidatorRegex = new(
        @"<(ValidationSummary|RequiredFieldValidator|CompareValidator|RegularExpressionValidator|RangeValidator|CustomValidator)\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ContentBlockRegex = new(
        @"<Content\b[^>]*>(?<inner>[\s\S]*?)</Content>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex HyperLinkRegex = new(
        @"<HyperLink\b[^>]*(?:NavigateUrl=""(?<url>[^""]+)"")?[^>]*>(?<text>[\s\S]*?)</HyperLink>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AttributeRegex = new(
        @"\b(?<name>[A-Za-z]+)\s*=\s*""(?<value>[^""]*)""",
        RegexOptions.Compiled);

    public string Id => "pattern-account-pages";
    public int Order => 200;

    public SemanticPatternMatch Match(SemanticPatternContext context)
    {
        if (context.Metadata.FileType != FileType.Page)
        {
            return SemanticPatternMatch.NoMatch();
        }

        var fileName = Path.GetFileNameWithoutExtension(context.SourceFile.MarkupPath);
        var isAccountPage = context.SourceFile.MarkupPath.Contains($"{Path.DirectorySeparatorChar}Account{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                            || context.SourceFile.MarkupPath.Contains("/Account/", StringComparison.OrdinalIgnoreCase)
                            || IsKnownAccountPage(fileName);

        if (!isAccountPage)
        {
            return SemanticPatternMatch.NoMatch();
        }

        var hasCredentialControls = TextBoxRegex.Matches(context.Markup).Count >= 2
                                    && ButtonRegex.IsMatch(context.Markup);

        if (!hasCredentialControls)
        {
            return SemanticPatternMatch.NoMatch();
        }

        if (ValidatorRegex.IsMatch(context.Markup) || IsKnownAccountPage(fileName))
        {
            return SemanticPatternMatch.Match($"Normalized account page '{fileName}' to an SSR-safe auth form stub.");
        }

        return SemanticPatternMatch.NoMatch();
    }

    public SemanticPatternResult Apply(SemanticPatternContext context)
    {
        var fileName = Path.GetFileNameWithoutExtension(context.SourceFile.MarkupPath);
        var pageTitle = ToTitle(fileName);
        var formMarkup = BuildNormalizedAccountMarkup(context.Markup, fileName, pageTitle);
        var rewritten = ReplacePrimaryContent(context.Markup, formMarkup);
        rewritten = EnsureAccountQueryParameters(rewritten, fileName);

        return new SemanticPatternResult(
            rewritten,
            context.CodeBehind,
            $"Replaced validator-heavy {pageTitle} markup with an SSR-safe account form stub and auth TODOs.");
    }

    private static string BuildNormalizedAccountMarkup(string markup, string fileName, string pageTitle)
    {
        var contentInner = ExtractPrimaryContent(markup);
        var fields = ExtractFields(contentInner);
        var formAction = GetFormAction(fileName);
        var buttonMatch = ButtonRegex.Match(contentInner);
        var buttonAttributes = buttonMatch.Success
            ? ParseAttributes(buttonMatch.Value)
            : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var buttonText = buttonAttributes.TryGetValue("Text", out var buttonTextValue)
            ? buttonTextValue
            : DefaultButtonText(fileName);
        var buttonClass = buttonAttributes.TryGetValue("CssClass", out var buttonClassValue) && !string.IsNullOrWhiteSpace(buttonClassValue)
            ? buttonClassValue
            : "btn btn-default";

        var builder = new StringBuilder();
        builder.AppendLine($"<h1>{pageTitle}</h1>");
        builder.AppendLine("@* TODO(bwfc-identity): Wire this account page to ASP.NET Core Identity or your app's authentication service. *@");
        builder.AppendLine("@* TODO(bwfc-identity): Recreate validation and submit handling with EditForm, minimal APIs, or an equivalent SSR-safe endpoint. *@");
        if (fileName.Equals("Login", StringComparison.OrdinalIgnoreCase))
        {
            builder.AppendLine("@if (Registered.GetValueOrDefault() != 0)");
            builder.AppendLine("{");
            builder.AppendLine("    <p class=\"text-success\">Registration succeeded. Please log in.</p>");
            builder.AppendLine("}");
        }

        builder.AppendLine("@if (!string.IsNullOrWhiteSpace(Error))");
        builder.AppendLine("{");
        builder.AppendLine("    <p class=\"text-danger\">@Error</p>");
        builder.AppendLine("}");
        builder.AppendLine($"<form method=\"{GetFormMethod(fileName)}\" action=\"{formAction}\" class=\"form-horizontal\">");

        if (fileName.Equals("Login", StringComparison.OrdinalIgnoreCase))
        {
            builder.AppendLine("    @if (!string.IsNullOrWhiteSpace(ReturnUrl))");
            builder.AppendLine("    {");
            builder.AppendLine("        <input type=\"hidden\" name=\"returnUrl\" value=\"@ReturnUrl\" />");
            builder.AppendLine("    }");
        }

        foreach (var field in fields)
        {
            builder.AppendLine(RenderField(field));
        }

        builder.AppendLine("    <div class=\"form-group\">");
        builder.AppendLine("        <div class=\"col-md-offset-2 col-md-10\">");
        builder.AppendLine($"            <button type=\"submit\" class=\"{buttonClass}\">{buttonText}</button>");
        builder.AppendLine("        </div>");
        builder.AppendLine("    </div>");
        builder.AppendLine("</form>");

        foreach (var link in ExtractLinks(contentInner, fileName))
        {
            builder.AppendLine(link);
        }

        return builder.ToString().TrimEnd();
    }

    private static string ReplacePrimaryContent(string markup, string replacement)
    {
        var contentBlock = ContentBlockRegex.Match(markup);
        if (contentBlock.Success)
        {
            return markup[..contentBlock.Index]
                   + contentBlock.Value.Replace(contentBlock.Groups["inner"].Value, $"\n{SemanticPatternMarkupHelpers.Indent(replacement, "    ")}\n")
                   + markup[(contentBlock.Index + contentBlock.Length)..];
        }

        return replacement;
    }

    private static string ExtractPrimaryContent(string markup)
    {
        var contentBlock = ContentBlockRegex.Match(markup);
        return contentBlock.Success ? contentBlock.Groups["inner"].Value : markup;
    }

    private static List<AccountField> ExtractFields(string content)
    {
        var labelMap = LabelRegex.Matches(content)
            .ToDictionary(
                m => m.Groups["id"].Value,
                m => Regex.Replace(m.Groups["text"].Value, @"\s+", " ").Trim(),
                StringComparer.OrdinalIgnoreCase);

        var fields = new List<AccountField>();

        foreach (Match match in TextBoxRegex.Matches(content))
        {
            var attributes = ParseAttributes(match.Value);
            if (!attributes.TryGetValue("ID", out var id))
            {
                continue;
            }

            fields.Add(new AccountField(
                id,
                labelMap.GetValueOrDefault(id) ?? ToTitle(id),
                ToInputType(attributes.GetValueOrDefault("TextMode") ?? string.Empty, id),
                string.IsNullOrWhiteSpace(attributes.GetValueOrDefault("CssClass")) ? "form-control" : attributes["CssClass"],
                IsCheckbox: false));
        }

        foreach (Match match in CheckBoxRegex.Matches(content))
        {
            var attributes = ParseAttributes(match.Value);
            if (!attributes.TryGetValue("ID", out var id))
            {
                continue;
            }

            fields.Add(new AccountField(
                id,
                labelMap.GetValueOrDefault(id) ?? ToTitle(id),
                "checkbox",
                string.IsNullOrWhiteSpace(attributes.GetValueOrDefault("CssClass")) ? string.Empty : attributes["CssClass"],
                IsCheckbox: true));
        }

        return fields;
    }

    private static IEnumerable<string> ExtractLinks(string content, string fileName)
    {
        var links = HyperLinkRegex.Matches(content)
            .Select(m =>
            {
                var text = Regex.Replace(m.Groups["text"].Value, @"\s+", " ").Trim();
                var url = m.Groups["url"].Value;
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(url))
                {
                    url = text.Contains("register", StringComparison.OrdinalIgnoreCase)
                        ? "/Account/Register"
                        : text.Contains("log in", StringComparison.OrdinalIgnoreCase) || text.Contains("login", StringComparison.OrdinalIgnoreCase)
                            ? "/Account/Login"
                            : "#";
                }

                return $"<p><a href=\"{url}\">{text}</a></p>";
            })
            .Where(link => link is not null)
            .Cast<string>()
            .ToList();

        if (links.Count > 0)
        {
            return links;
        }

        if (fileName.Equals("Login", StringComparison.OrdinalIgnoreCase))
        {
            return ["<p><a href=\"/Account/Register\">Register as a new user</a></p>"];
        }

        if (fileName.Equals("Register", StringComparison.OrdinalIgnoreCase))
        {
            return ["<p><a href=\"/Account/Login\">Already have an account? Sign in</a></p>"];
        }

        return [];
    }

    private static string RenderField(AccountField field)
    {
        var elementId = ToKebabCase(field.Id);
        if (field.IsCheckbox)
        {
            return $$"""
    <div class="form-group">
        <div class="col-md-offset-2 col-md-10">
            <div class="checkbox">
                <input id="{{elementId}}" name="{{field.Id}}" type="checkbox" />
                <label for="{{elementId}}">{{field.Label}}</label>
            </div>
        </div>
    </div>
""";
        }

        return $$"""
    <div class="form-group">
        <label class="col-md-2 control-label" for="{{elementId}}">{{field.Label}}</label>
        <div class="col-md-10">
            <input id="{{elementId}}" name="{{field.Id}}" type="{{field.InputType}}" class="{{field.CssClass}}" />
        </div>
    </div>
""";
    }

    private static string ToInputType(string textMode, string id)
    {
        var normalizedTextMode = textMode.Trim().TrimStart('@');
        var enumSeparatorIndex = normalizedTextMode.LastIndexOf('.');
        if (enumSeparatorIndex >= 0 && enumSeparatorIndex < normalizedTextMode.Length - 1)
        {
            normalizedTextMode = normalizedTextMode[(enumSeparatorIndex + 1)..];
        }

        if (normalizedTextMode.Equals("Password", StringComparison.OrdinalIgnoreCase))
        {
            return "password";
        }

        if (normalizedTextMode.Equals("Email", StringComparison.OrdinalIgnoreCase) || id.Contains("email", StringComparison.OrdinalIgnoreCase))
        {
            return "email";
        }

        if (normalizedTextMode.Equals("Number", StringComparison.OrdinalIgnoreCase))
        {
            return "number";
        }

        return "text";
    }

    private static bool IsKnownAccountPage(string fileName) =>
        fileName.Equals("Login", StringComparison.OrdinalIgnoreCase)
        || fileName.Equals("Register", StringComparison.OrdinalIgnoreCase)
        || fileName.Contains("Password", StringComparison.OrdinalIgnoreCase)
        || fileName.Contains("Phone", StringComparison.OrdinalIgnoreCase)
        || fileName.Contains("TwoFactor", StringComparison.OrdinalIgnoreCase)
        || fileName.Contains("Account", StringComparison.OrdinalIgnoreCase);

    private static string DefaultButtonText(string fileName) =>
        fileName.Equals("Login", StringComparison.OrdinalIgnoreCase) ? "Log in"
        : fileName.Equals("Register", StringComparison.OrdinalIgnoreCase) ? "Register"
        : "Submit";

    private static string GetFormAction(string fileName) =>
        fileName.Equals("Login", StringComparison.OrdinalIgnoreCase) ? "/Account/PerformLogin"
        : fileName.Equals("Register", StringComparison.OrdinalIgnoreCase) ? "/Account/PerformRegister"
        : $"/Account/{fileName}Handler";

    private static string GetFormMethod(string fileName) =>
        fileName.Equals("Login", StringComparison.OrdinalIgnoreCase)
        || fileName.Equals("Register", StringComparison.OrdinalIgnoreCase)
            ? "get"
            : "post";

    private static string ToTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Account";
        }

        var spaced = Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
        return Regex.Replace(spaced, @"\s+", " ").Trim();
    }

    private static string ToKebabCase(string value)
    {
        var kebab = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2");
        return kebab.Replace("_", "-").ToLowerInvariant();
    }

    private static Dictionary<string, string> ParseAttributes(string tag) =>
        AttributeRegex.Matches(tag)
            .ToDictionary(
                m => m.Groups["name"].Value,
                m => m.Groups["value"].Value,
                StringComparer.OrdinalIgnoreCase);

    private static string EnsureAccountQueryParameters(string markup, string fileName)
    {
        var parameterLines = new List<string>();
        AddParameterIfMissing(markup, parameterLines, "Error", "error", "string?");

        if (fileName.Equals("Login", StringComparison.OrdinalIgnoreCase))
        {
            AddParameterIfMissing(markup, parameterLines, "Registered", "registered", "int?");
            AddParameterIfMissing(markup, parameterLines, "ReturnUrl", "returnUrl", "string?");
        }

        if (parameterLines.Count == 0)
        {
            return markup;
        }

        var parameterBlock = string.Join(Environment.NewLine + Environment.NewLine, parameterLines);
        var codeIndex = markup.LastIndexOf("@code", StringComparison.Ordinal);
        if (codeIndex < 0)
        {
            return $"{markup.TrimEnd()}{Environment.NewLine}{Environment.NewLine}@code {{{Environment.NewLine}{parameterBlock}{Environment.NewLine}}}";
        }

        var openBraceIndex = markup.IndexOf('{', codeIndex);
        var closeBraceIndex = markup.LastIndexOf('}');
        if (openBraceIndex < 0 || closeBraceIndex <= openBraceIndex)
        {
            return $"{markup.TrimEnd()}{Environment.NewLine}{Environment.NewLine}@code {{{Environment.NewLine}{parameterBlock}{Environment.NewLine}}}";
        }

        var body = markup.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1).Trim('\r', '\n');
        var rewrittenBody = string.IsNullOrWhiteSpace(body)
            ? parameterBlock
            : $"{body}{Environment.NewLine}{Environment.NewLine}{parameterBlock}";

        return markup[..codeIndex]
               + $"@code {{{Environment.NewLine}{rewrittenBody}{Environment.NewLine}}}"
               + markup[(closeBraceIndex + 1)..];
    }

    private static void AddParameterIfMissing(string markup, ICollection<string> parameterLines, string propertyName, string queryName, string type)
    {
        if (Regex.IsMatch(markup, $@"\b{Regex.Escape(propertyName)}\b\s*\{{\s*get\s*;\s*set\s*;\s*\}}", RegexOptions.IgnoreCase))
        {
            return;
        }

        parameterLines.Add($"    [Parameter, SupplyParameterFromQuery(Name = \"{queryName}\")] public {type} {propertyName} {{ get; set; }}");
    }

    private sealed record AccountField(string Id, string Label, string InputType, string CssClass, bool IsCheckbox);
}
