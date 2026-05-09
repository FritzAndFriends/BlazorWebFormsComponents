using System.Net;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Pipeline;

public sealed partial class PageQuarantineDetector
{
    private static readonly JsonSerializerOptions s_manifestJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Regex RouteRegex = new(
        "^\\s*@page\\s+\"(?<route>[^\"]+)\"",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex NamespaceRegex = new(
        @"namespace\s+(?<namespace>[A-Za-z_][\w.]*)\s*(?:\{|;)",
        RegexOptions.Compiled);

    private static readonly Regex PartialClassRegex = new(
        @"(?:(?<access>public|protected|internal|private)\s+)?partial\s+class\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);

    private static readonly Regex IdentityRegex = new(
        @"\bSystem\.Web\.Security\b|\bMicrosoft\.AspNet\.Identity\b|\bMembership\b|\bRoles\b|\bUserManager\b|\bSignInManager\b|\bApplicationUserManager\b|\bApplicationSignInManager\b|\bCreateUserWizard\b|\bPasswordRecovery\b|\bChangePassword\b|<\s*(Login|CreateUserWizard|PasswordRecovery|ChangePassword)\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PaymentRegex = new(
        @"\bPayPal\b|\bStripe\b|\bBraintree\b|\bAuthorizeNet\b|\bSquare\b|\bCyberSource\b|\bPaymentIntent\b|\bCheckoutSession\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex DataSourceRegex = new(
        @"<\s*(SqlDataSource|ObjectDataSource|EntityDataSource|LinqDataSource|AccessDataSource)\b|\bDataSourceID\s*=\s*""[^""]+""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PageAndTitleRegex = new(
        @"(?is)^\s*@page\s+""[^""]+""\s*|<PageTitle>.*?</PageTitle>",
        RegexOptions.Compiled);

    private static readonly Regex WrapperTagRegex = new(
        @"(?is)</?(html|head|body|form|div|span|section|main|title)(?:\s[^>]*)?>",
        RegexOptions.Compiled);

    public PageQuarantineDecision AnalyzeEarly(FileMetadata metadata, string transformedCodeBehind)
    {
        return Analyze(
            metadata,
            metadata.MarkupContent ?? metadata.OriginalContent,
            transformedCodeBehind,
            emissionPlan: null,
            useCompileSurfaceSignal: false);
    }

    public PageQuarantineDecision AnalyzeLate(FileMetadata metadata, string markup, string? transformedCodeBehind, CodeBehindEmissionPlan? emissionPlan)
    {
        return Analyze(
            metadata,
            markup,
            transformedCodeBehind,
            emissionPlan,
            useCompileSurfaceSignal: true);
    }

    public string BuildManifest(IReadOnlyList<PageQuarantineManifestEntry> entries)
    {
        return JsonSerializer.Serialize(new { pages = entries }, s_manifestJsonOptions);
    }

    private static PageQuarantineDecision Analyze(
        FileMetadata metadata,
        string markup,
        string? transformedCodeBehind,
        CodeBehindEmissionPlan? emissionPlan,
        bool useCompileSurfaceSignal)
    {
        if (metadata.FileType != FileType.Page)
        {
            return PageQuarantineDecision.None;
        }

        var relativeSourcePath = NormalizeRelativeSourcePath(metadata);
        if (IsHappyPathAuthPage(relativeSourcePath))
        {
            return PageQuarantineDecision.None;
        }

        if (IsEssentialPage(relativeSourcePath))
        {
            return PageQuarantineDecision.None;
        }

        var originalCodeBehind = metadata.CodeBehindContent ?? string.Empty;
        var sourceSignals = string.Join("\n", metadata.OriginalContent ?? string.Empty, originalCodeBehind);
        var hasIdentitySignals = IdentityRegex.IsMatch(sourceSignals);
        var hasPaymentSignals = PaymentRegex.IsMatch(sourceSignals);

        if (IsRedirectOnlyPage(markup, originalCodeBehind) && !hasIdentitySignals && !hasPaymentSignals)
        {
            return PageQuarantineDecision.None;
        }

        var features = new List<string>();
        var reasons = new List<string>();
        var suggestions = new List<string>();

        if (hasIdentitySignals)
        {
            AddSignal(
                features,
                reasons,
                suggestions,
                "ASP.NET Identity or membership APIs",
                "Detected ASP.NET Identity, membership, or heavyweight account-control usage that the CLI cannot safely preserve in the generated compile surface.",
                "Rebuild this flow with ASP.NET Core Identity, EditForm, and app-specific authentication endpoints before restoring the page logic.");
        }

        if (hasPaymentSignals)
        {
            AddSignal(
                features,
                reasons,
                suggestions,
                "Payment or checkout integration",
                "Detected payment-provider integration code that requires manual migration and secrets-safe server orchestration.",
                "Move payment orchestration into a dedicated server-side service or API boundary, then reconnect the page once the provider SDK is re-integrated.");
        }

        if (IsMobileSpecificPage(relativeSourcePath))
        {
            AddSignal(
                features,
                reasons,
                suggestions,
                "Mobile-specific page or shell",
                "Detected a mobile-specific page/shell that should be collapsed into a responsive Razor route instead of copied verbatim.",
                "Replace the dedicated mobile page with responsive Razor markup or a shared layout and migrate only the reusable business logic.");
        }

        var dataSourceCount = CountDataSourceSignals(sourceSignals);
        if (IsAdminPath(relativeSourcePath) && dataSourceCount >= 3)
        {
            AddSignal(
                features,
                reasons,
                suggestions,
                $"Complex admin CRUD with {dataSourceCount} data-source references",
                "Detected an admin page that coordinates three or more legacy Web Forms data sources, which exceeds the CLI's safe compile-surface migration boundary.",
                "Split the admin workflow into smaller Razor pages and move each query/update path behind injected services or EF Core-backed handlers.");
        }

        if (useCompileSurfaceSignal && emissionPlan is { EmitToCompileSurface: false, ArtifactReason: not null })
        {
            AddSignal(
                features,
                reasons,
                suggestions,
                "Unresolved compile-surface blockers",
                emissionPlan.ArtifactReason,
                "Port the remaining server-control or lifecycle logic into BWFC-compatible components, services, or semantic patterns before re-enabling the page.");
        }

        // Pages on clearly quarantinable paths (Account/*, Admin/*, Checkout/*, Mobile)
        // are always quarantined even without explicit signals — these paths consistently
        // produce compile errors from legacy APIs that the CLI cannot safely transform.
        if (features.Count == 0 && IsClearlyQuarantinablePath(relativeSourcePath))
        {
            AddSignal(
                features,
                reasons,
                suggestions,
                "Non-essential path (auto-quarantined)",
                $"Page is located in a non-essential path ({relativeSourcePath}) that typically requires manual migration of legacy Web Forms APIs.",
                "Migrate the page manually once core pages are stable, using injected services and Blazor patterns.");
        }

        if (features.Count == 0)
        {
            return PageQuarantineDecision.None;
        }

        if (features.Count < 2 && !IsClearlyQuarantinablePath(relativeSourcePath) && !HasStrongSingleSignal(features))
        {
            return PageQuarantineDecision.None;
        }

        var namespaceName = ResolveNamespace(metadata, transformedCodeBehind);
        var className = ResolveClassName(metadata, transformedCodeBehind);
        var route = ResolveRoute(metadata, markup);
        var featureSummary = string.Join(", ", features);
        var reason = string.Join(" ", reasons.Distinct(StringComparer.Ordinal));
        var suggestedApproach = string.Join(" ", suggestions.Distinct(StringComparer.Ordinal));
        var stubMarkup = BuildStubMarkup(route, relativeSourcePath, featureSummary, suggestedApproach);
        var stubCodeBehind = BuildStubCodeBehind(namespaceName, className, relativeSourcePath, featureSummary, reason);
        var artifactContent = string.IsNullOrWhiteSpace(transformedCodeBehind)
            ? null
            : transformedCodeBehind;

        return new PageQuarantineDecision(
            ShouldQuarantine: true,
            RelativeSourcePath: relativeSourcePath,
            DetectedFeatures: features,
            Reason: reason,
            SuggestedApproach: suggestedApproach,
            StubMarkup: stubMarkup,
            StubCodeBehind: stubCodeBehind,
            ArtifactContent: artifactContent,
            ManifestEntry: new PageQuarantineManifestEntry(relativeSourcePath, features, reason, suggestedApproach));
    }

    private static string BuildStubMarkup(string route, string relativeSourcePath, string featureSummary, string suggestedApproach)
    {
        return string.Join("\n", [
            $"@page \"{route}\"",
            "@inherits BlazorWebFormsComponents.WebFormsPageBase",
            string.Empty,
            "<div class=\"migration-pending\">",
            "    <h3>Page Not Yet Migrated</h3>",
            $"    <p>This page (<code>{WebUtility.HtmlEncode(relativeSourcePath)}</code>) requires manual migration.</p>",
            $"    <p>Original Web Forms features used: {WebUtility.HtmlEncode(featureSummary)}</p>",
            $"    <p>Suggested migration approach: {WebUtility.HtmlEncode(suggestedApproach)}</p>",
            "</div>"
        ]);
    }

    private static string BuildStubCodeBehind(string? namespaceName, string className, string relativeSourcePath, string featureSummary, string reason)
    {
        if (!string.IsNullOrWhiteSpace(namespaceName))
        {
            return string.Join("\n", [
                $"namespace {namespaceName};",
                string.Empty,
                $"public partial class {className} : BlazorWebFormsComponents.WebFormsPageBase",
                "{",
                $"    // TODO: Migrate from {relativeSourcePath}",
                $"    // Original features: {featureSummary}",
                $"    // Quarantine reason: {reason}",
                "}"
            ]);
        }

        return string.Join("\n", [
            $"public partial class {className} : BlazorWebFormsComponents.WebFormsPageBase",
            "{",
            $"    // TODO: Migrate from {relativeSourcePath}",
            $"    // Original features: {featureSummary}",
            $"    // Quarantine reason: {reason}",
            "}"
        ]);
    }

    private static string ResolveRoute(FileMetadata metadata, string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
        {
            var match = RouteRegex.Match(markup);
            if (match.Success)
            {
                return match.Groups["route"].Value;
            }
        }

        if (!string.IsNullOrWhiteSpace(metadata.OutputRootPath))
        {
            var relativePath = Path.GetRelativePath(metadata.OutputRootPath, metadata.OutputFilePath)
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');
            if (relativePath.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath[..^".razor".Length];
            }

            return "/" + relativePath.TrimStart('/');
        }

        return "/" + Path.GetFileNameWithoutExtension(metadata.OutputFilePath);
    }

    private static string? ResolveNamespace(FileMetadata metadata, string? transformedCodeBehind)
    {
        if (!string.IsNullOrWhiteSpace(transformedCodeBehind))
        {
            var namespaceMatch = NamespaceRegex.Match(transformedCodeBehind);
            if (namespaceMatch.Success)
            {
                return namespaceMatch.Groups["namespace"].Value;
            }
        }

        if (string.IsNullOrWhiteSpace(metadata.ProjectNamespace))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(metadata.OutputRootPath))
        {
            return metadata.ProjectNamespace;
        }

        var directory = Path.GetDirectoryName(metadata.OutputFilePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            return metadata.ProjectNamespace;
        }

        var relativeDirectory = Path.GetRelativePath(metadata.OutputRootPath, directory);
        if (relativeDirectory == ".")
        {
            return metadata.ProjectNamespace;
        }

        var namespaceSuffix = string.Join(
            ".",
            relativeDirectory
                .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
                .Select(static segment => SanitizeNamespaceSegment(segment)));

        return string.IsNullOrWhiteSpace(namespaceSuffix)
            ? metadata.ProjectNamespace
            : $"{metadata.ProjectNamespace}.{namespaceSuffix}";
    }

    private static string ResolveClassName(FileMetadata metadata, string? transformedCodeBehind)
    {
        if (!string.IsNullOrWhiteSpace(transformedCodeBehind))
        {
            var classMatch = PartialClassRegex.Match(transformedCodeBehind);
            if (classMatch.Success)
            {
                return classMatch.Groups["name"].Value;
            }
        }

        return Path.GetFileNameWithoutExtension(metadata.OutputFilePath);
    }

    private static string NormalizeRelativeSourcePath(FileMetadata metadata)
    {
        var relativePath = metadata.SourceFilePath;
        if (!string.IsNullOrWhiteSpace(metadata.SourceRootPath))
        {
            relativePath = Path.GetRelativePath(metadata.SourceRootPath, metadata.SourceFilePath);
        }

        return relativePath
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/');
    }

    private static bool IsHappyPathAuthPage(string relativeSourcePath)
    {
        return relativeSourcePath.EndsWith("Account/Login.aspx", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.EndsWith("Account/Register.aspx", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.EndsWith("Login.aspx", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.EndsWith("Register.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsEssentialPage(string relativeSourcePath)
    {
        var normalizedPath = relativeSourcePath.Replace('\\', '/');
        var fileName = Path.GetFileName(normalizedPath);

        if (fileName.Equals("Default.aspx", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("Index.aspx", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("Home.aspx", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("About.aspx", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("Contact.aspx", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return normalizedPath.Contains("Product", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Catalog", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Item", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Detail", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Cart", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Shopping", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("Basket", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.Contains("AddTo", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRedirectOnlyPage(string markup, string? codeBehind)
    {
        if (string.IsNullOrWhiteSpace(codeBehind)
            || !codeBehind.Contains("Response.Redirect", StringComparison.Ordinal))
        {
            return false;
        }

        var stripped = PageAndTitleRegex.Replace(markup, string.Empty);
        stripped = WrapperTagRegex.Replace(stripped, string.Empty);
        stripped = Regex.Replace(stripped, @"\s+|&nbsp;", string.Empty, RegexOptions.IgnoreCase);
        return string.IsNullOrEmpty(stripped);
    }

    private static bool IsMobileSpecificPage(string relativeSourcePath)
    {
        return relativeSourcePath.Contains("/Mobile/", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.Contains(".mobile.", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.Contains("Mobile", StringComparison.OrdinalIgnoreCase) && (relativeSourcePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) || relativeSourcePath.EndsWith(".master", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsAdminPath(string relativeSourcePath)
    {
        return relativeSourcePath.Contains("/Admin/", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.StartsWith("Admin/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAccountPath(string relativeSourcePath)
    {
        return relativeSourcePath.Contains("/Account/", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.StartsWith("Account/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCheckoutPath(string relativeSourcePath)
    {
        return relativeSourcePath.Contains("/Checkout/", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.StartsWith("Checkout/", StringComparison.OrdinalIgnoreCase)
            || relativeSourcePath.EndsWith("Checkout.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsClearlyQuarantinablePath(string relativeSourcePath)
    {
        return IsAccountPath(relativeSourcePath)
            || IsAdminPath(relativeSourcePath)
            || IsCheckoutPath(relativeSourcePath)
            || IsMobileSpecificPage(relativeSourcePath);
    }

    private static bool HasStrongSingleSignal(IReadOnlyList<string> features)
    {
        return features.Contains("Payment or checkout integration", StringComparer.Ordinal)
            || features.Contains("Unresolved compile-surface blockers", StringComparer.Ordinal);
    }

    private static int CountDataSourceSignals(string content)
    {
        return DataSourceRegex.Matches(content).Count;
    }

    private static void AddSignal(List<string> features, List<string> reasons, List<string> suggestions, string feature, string reason, string suggestion)
    {
        if (!features.Contains(feature, StringComparer.Ordinal))
        {
            features.Add(feature);
        }

        reasons.Add(reason);
        suggestions.Add(suggestion);
    }

    private static string SanitizeNamespaceSegment(string segment)
    {
        var cleaned = Regex.Replace(segment, @"[^A-Za-z0-9_]", string.Empty);
        if (string.IsNullOrWhiteSpace(cleaned))
        {
            return "Pages";
        }

        return char.IsDigit(cleaned[0])
            ? $"_{cleaned}"
            : cleaned;
    }
}

public sealed record PageQuarantineDecision(
    bool ShouldQuarantine,
    string RelativeSourcePath,
    IReadOnlyList<string> DetectedFeatures,
    string Reason,
    string SuggestedApproach,
    string StubMarkup,
    string StubCodeBehind,
    string? ArtifactContent,
    PageQuarantineManifestEntry? ManifestEntry)
{
    public static PageQuarantineDecision None { get; } = new(false, string.Empty, Array.Empty<string>(), string.Empty, string.Empty, string.Empty, string.Empty, null, null);
}

public sealed record PageQuarantineManifestEntry(
    string OriginalFilePath,
    IReadOnlyList<string> DetectedUnsupportedFeatures,
    string ReasonForQuarantine,
    string SuggestedMigrationApproach);
