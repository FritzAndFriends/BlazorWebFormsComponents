using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.SemanticPatterns;

namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Detects redirect-only pages and annotates Program.cs with minimal API migration TODOs.
/// </summary>
public class RedirectHandlerAnnotator
{
    private static readonly Regex QueryStringAccessRegex = new(
        @"Request\.QueryString\[""(?<name>[^""]+)""\]",
        RegexOptions.Compiled);
    private static readonly Regex RedirectLiteralRegex = new(
        @"Response\.Redirect\(\s*""(?<target>[^""]+)""",
        RegexOptions.Compiled);
    private static readonly Regex PageAndTitleRegex = new(
        @"(?is)^\s*@page\s+""[^""]+""\s*|<PageTitle>.*?</PageTitle>|<%@\s*\w+[^%]*%>",
        RegexOptions.Compiled);
    private static readonly Regex WrapperTagRegex = new(
        @"(?is)</?(html|head|body|form|div|span|section|main|title)(?:\s[^>]*)?>",
        RegexOptions.Compiled);

    private readonly OutputWriter _outputWriter;

    public RedirectHandlerAnnotator(OutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    public async Task<int> AnnotateAsync(MigrationContext context, MigrationReport report)
    {
        if (context.Options.SkipScaffold)
            return 0;

        var handlerBlocks = new List<string>();
        foreach (var sourceFile in context.SourceFiles.Where(f => f.FileType == FileType.Page && f.HasCodeBehind))
        {
            var markupContent = await File.ReadAllTextAsync(sourceFile.MarkupPath);
            if (!TryBuildActionHandlerStub(sourceFile, markupContent, out var handlerBlock, out var manualDescription))
                continue;

            var pageName = Path.GetFileNameWithoutExtension(sourceFile.MarkupPath);
            handlerBlocks.Add(handlerBlock);
            report.AddManualItem(
                Path.GetRelativePath(context.SourcePath, sourceFile.MarkupPath),
                0,
                "RedirectHandler",
                manualDescription);
        }

        if (context.SourceFiles.Any(IsLoginPage))
        {
            handlerBlocks.Add(BuildLoginHandlerBlock());
            report.AddManualItem("Account/Login.aspx", 0, "bwfc-identity", "Login.razor now posts to /Account/LoginHandler — verify the generated Identity sign-in flow matches your app's redirect and claims requirements.", "medium");
        }

        if (context.SourceFiles.Any(IsRegisterPage))
        {
            handlerBlocks.Add(BuildRegisterHandlerBlock());
            report.AddManualItem("Account/Register.aspx", 0, "bwfc-identity", "Register.razor now posts to /Account/RegisterHandler — verify the generated Identity registration flow matches your app's user profile and confirmation requirements.", "medium");
        }

        if (context.SourceFiles.Any(IsLoginPage) || context.SourceFiles.Any(IsRegisterPage))
        {
            handlerBlocks.Add(BuildLogoutHandlerBlock());
        }

        if (handlerBlocks.Count == 0)
            return 0;

        var programPath = Path.Combine(context.OutputPath, "Program.cs");
        if (!File.Exists(programPath))
            return handlerBlocks.Count;

        var programContent = await File.ReadAllTextAsync(programPath);
        if (programContent.Contains("// --- BWFC generated handler stubs ---", StringComparison.Ordinal))
            return handlerBlocks.Count;

        var insertion = $"// --- BWFC generated handler stubs ---{Environment.NewLine}{string.Join($"{Environment.NewLine}{Environment.NewLine}", handlerBlocks)}{Environment.NewLine}{Environment.NewLine}";
        if (programContent.Contains("app.MapRazorComponents<", StringComparison.Ordinal))
        {
            programContent = programContent.Replace("app.MapRazorComponents<", insertion + "app.MapRazorComponents<", StringComparison.Ordinal);
            await _outputWriter.WriteFileAsync(programPath, programContent, "Annotate Program.cs with generated handler stubs");
        }

        return handlerBlocks.Count;
    }

    private static bool TryBuildActionHandlerStub(SourceFile sourceFile, string markupContent, out string handlerBlock, out string manualDescription)
    {
        handlerBlock = string.Empty;
        manualDescription = string.Empty;

        var codeBehindPath = sourceFile.CodeBehindPath;
        if (string.IsNullOrEmpty(codeBehindPath) || !File.Exists(codeBehindPath))
            return false;

        var codeBehind = File.ReadAllText(codeBehindPath);
        if (!codeBehind.Contains("Response.Redirect", StringComparison.Ordinal))
            return false;

        var stripped = PageAndTitleRegex.Replace(markupContent, string.Empty);
        stripped = WrapperTagRegex.Replace(stripped, string.Empty);
        stripped = Regex.Replace(stripped, @"\s+|&nbsp;", string.Empty, RegexOptions.IgnoreCase);
        if (!string.IsNullOrEmpty(stripped))
            return false;

        var pageName = Path.GetFileNameWithoutExtension(sourceFile.MarkupPath);
        var redirectTarget = RedirectLiteralRegex.Match(codeBehind);
        var normalizedTarget = redirectTarget.Success
            ? SemanticPatternUtilities.NormalizeRoute(redirectTarget.Groups["target"].Value)
            : "/";
        var queryKeys = QueryStringAccessRegex.Matches(codeBehind)
            .Select(static match => match.Groups["name"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        var endpointRoute = ActionPagesSemanticPattern.GetEndpointRoute(pageName);
        var builder = new System.Text.StringBuilder();
        builder.AppendLine($"app.MapPost(\"{endpointRoute}\", async (HttpContext context) =>");
        builder.AppendLine("{");
        builder.AppendLine("    var form = await context.Request.ReadFormAsync();");
        foreach (var queryKey in queryKeys)
        {
            builder.AppendLine($"    var {SemanticPatternUtilities.ToPropertyName(queryKey)} = form[\"{queryKey}\"].ToString();");
        }
        builder.AppendLine($"    // TODO(bwfc-action-pages): move the original {pageName} side effect into this HTTP handler or a scoped service.");
        builder.AppendLine($"    return Results.Redirect(\"{normalizedTarget}\");");
        builder.AppendLine("}).DisableAntiforgery();");

        handlerBlock = builder.ToString().TrimEnd();
        manualDescription = $"{pageName} was a redirect handler (Response.Redirect in code-behind) — generated POST stub {endpointRoute} redirects to {normalizedTarget} until the side effect is migrated.";
        return true;
    }

    private static bool IsLoginPage(SourceFile sourceFile) =>
        sourceFile.MarkupPath.Contains($"{Path.DirectorySeparatorChar}Account{Path.DirectorySeparatorChar}Login.aspx", StringComparison.OrdinalIgnoreCase)
        || sourceFile.MarkupPath.EndsWith($"{Path.DirectorySeparatorChar}Login.aspx", StringComparison.OrdinalIgnoreCase);

    private static bool IsRegisterPage(SourceFile sourceFile) =>
        sourceFile.MarkupPath.Contains($"{Path.DirectorySeparatorChar}Account{Path.DirectorySeparatorChar}Register.aspx", StringComparison.OrdinalIgnoreCase)
        || sourceFile.MarkupPath.EndsWith($"{Path.DirectorySeparatorChar}Register.aspx", StringComparison.OrdinalIgnoreCase);

    private static string BuildLoginHandlerBlock() =>
        """
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();
    var rememberMe = string.Equals(form["RememberMe"], "on", StringComparison.OrdinalIgnoreCase)
        || string.Equals(form["RememberMe"], "true", StringComparison.OrdinalIgnoreCase);
    var hasLocalReturnUrl = !string.IsNullOrWhiteSpace(returnUrl)
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
        && returnUrl.StartsWith("/", StringComparison.Ordinal);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        var missingCredentialsUrl = hasLocalReturnUrl
            ? $"/Account/Login?error=Email%20and%20password%20are%20required&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Login?error=Email%20and%20password%20are%20required";
        return Results.LocalRedirect(missingCredentialsUrl);
    }

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
        var invalidLoginUrl = hasLocalReturnUrl
            ? $"/Account/Login?error=Invalid%20login%20attempt&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Login?error=Invalid%20login%20attempt";
        return Results.LocalRedirect(invalidLoginUrl);
    }

    return Results.LocalRedirect(hasLocalReturnUrl ? returnUrl : "/");
});
""";

    private static string BuildRegisterHandlerBlock() =>
        """
app.MapPost("/Account/RegisterHandler", async (HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();
    var returnUrl = form["ReturnUrl"].ToString();
    var hasLocalReturnUrl = !string.IsNullOrWhiteSpace(returnUrl)
        && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
        && returnUrl.StartsWith("/", StringComparison.Ordinal);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        var missingCredentialsUrl = hasLocalReturnUrl
            ? $"/Account/Register?error=Email%20and%20password%20are%20required&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Register?error=Email%20and%20password%20are%20required";
        return Results.LocalRedirect(missingCredentialsUrl);
    }

    if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
    {
        var mismatchedPasswordUrl = hasLocalReturnUrl
            ? $"/Account/Register?error=Passwords%20do%20not%20match&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : "/Account/Register?error=Passwords%20do%20not%20match";
        return Results.LocalRedirect(mismatchedPasswordUrl);
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email
    };

    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        var error = result.Errors.FirstOrDefault()?.Description ?? "Registration failed";
        var registrationErrorUrl = hasLocalReturnUrl
            ? $"/Account/Register?error={Uri.EscapeDataString(error)}&returnUrl={Uri.EscapeDataString(returnUrl)}"
            : $"/Account/Register?error={Uri.EscapeDataString(error)}";
        return Results.LocalRedirect(registrationErrorUrl);
    }

    await signInManager.SignInAsync(user, isPersistent: false);
    return Results.LocalRedirect(hasLocalReturnUrl ? returnUrl : "/");
});
""";

    private static string BuildLogoutHandlerBlock() =>
        """
app.MapPost("/Account/PerformLogout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();
""";
}
