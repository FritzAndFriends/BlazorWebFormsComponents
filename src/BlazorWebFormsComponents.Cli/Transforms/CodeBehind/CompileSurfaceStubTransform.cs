using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces high-risk page code-behind with build-safe stubs while preserving the transformed original in migration artifacts.
/// </summary>
public class CompileSurfaceStubTransform : ICodeBehindTransform
{
    private static readonly Regex RouteRegex = new(
        "^\\s*@page\\s+\"(?<route>[^\"]+)\"",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex NamespaceRegex = new(
        @"namespace\s+(?<namespace>[A-Za-z_][\w.]*)\s*(?:\{|;)",
        RegexOptions.Compiled);

    private static readonly Regex PartialClassRegex = new(
        @"(?:(?<access>public|protected|internal|private)\s+)?partial\s+class\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);

    private static readonly (Regex Pattern, string Reason)[] DetectionRules =
    [
        (new Regex(@"\bMicrosoft\.AspNet\.Identity\b|\bIdentityDbContext\b|\bUserManager\b|\bSignInManager\b|\bApplicationUserManager\b|\bApplicationSignInManager\b", RegexOptions.Compiled), "ASP.NET Identity references still require application-specific migration."),
        (new Regex(@"\bMicrosoft\.Owin\b|\bOwin\b|\bGetOwinContext\s*\(|\bIAppBuilder\b|\bDefaultAuthenticationTypes\b", RegexOptions.Compiled), "OWIN authentication/bootstrap code should not compile directly in the generated Blazor surface."),
        (new Regex(@":\s*OpenAuthProviders\b|\bOpenAuthProviders\b", RegexOptions.Compiled), "Legacy OpenAuth provider pages require a manual authentication migration path."),
        (new Regex(@"\bPayPal\b|\bStripe\b", RegexOptions.Compiled), "External payment-service integrations require manual migration before they can compile safely.")
    ];

    public string Name => "CompileSurfaceStub";
    public int Order => 850;

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page || IsHappyPathAuthPage(metadata.SourceFilePath))
        {
            return content;
        }

        if (!ShouldStub(metadata.SourceFilePath, metadata.CodeBehindContent ?? content, out var reason))
        {
            return content;
        }

        metadata.CompileSurfaceStubReason = reason;
        metadata.CompileSurfaceOriginalCodeBehind = content;
        metadata.MarkupContent = BuildStubMarkup(metadata);

        return BuildStubCodeBehind(content, metadata);
    }

    private static bool ShouldStub(string sourceFilePath, string codeBehind, out string reason)
    {
        if (IsInfrastructurePath(sourceFilePath))
        {
            reason = "Account/Admin page infrastructure was replaced with a build-safe stub until the real service wiring is migrated.";
            return true;
        }

        foreach (var (pattern, detectedReason) in DetectionRules)
        {
            if (pattern.IsMatch(codeBehind))
            {
                reason = detectedReason;
                return true;
            }
        }

        reason = string.Empty;
        return false;
    }

    private static string BuildStubMarkup(FileMetadata metadata)
    {
        var route = ResolveRoute(metadata);
        return string.Join("\n", [
            $"@page \"{route}\"",
            string.Empty,
            "<h3>This page is under migration</h3>",
            "<p>The original Web Forms page has been preserved in migration-artifacts/.</p>"
        ]);
    }

    private static string BuildStubCodeBehind(string content, FileMetadata metadata)
    {
        var namespaceMatch = NamespaceRegex.Match(content);
        var classMatch = PartialClassRegex.Match(content);
        var className = classMatch.Success ? classMatch.Groups["name"].Value : Path.GetFileNameWithoutExtension(metadata.OutputFilePath);
        var accessModifier = classMatch.Success && classMatch.Groups["access"].Success
            ? classMatch.Groups["access"].Value
            : "public";

        if (namespaceMatch.Success)
        {
            var namespaceName = namespaceMatch.Groups["namespace"].Value;
            return string.Join("\n", [
                "using Microsoft.AspNetCore.Components;",
                string.Empty,
                $"namespace {namespaceName}",
                "{",
                $"    {accessModifier} partial class {className} : ComponentBase",
                "    {",
                "    }",
                "}"
            ]);
        }

        return string.Join("\n", [
            "using Microsoft.AspNetCore.Components;",
            string.Empty,
            $"{accessModifier} partial class {className} : ComponentBase",
            "{",
            "}"
        ]);
    }

    private static string ResolveRoute(FileMetadata metadata)
    {
        if (!string.IsNullOrWhiteSpace(metadata.MarkupContent))
        {
            var match = RouteRegex.Match(metadata.MarkupContent);
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

    private static bool IsInfrastructurePath(string sourceFilePath)
    {
        return ContainsPathSegment(sourceFilePath, "Account") || ContainsPathSegment(sourceFilePath, "Admin");
    }

    private static bool IsHappyPathAuthPage(string sourceFilePath)
    {
        var fileName = Path.GetFileName(sourceFilePath);
        return fileName.Equals("Login.aspx", StringComparison.OrdinalIgnoreCase)
            || fileName.Equals("Register.aspx", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsPathSegment(string path, string segment)
    {
        return path.Contains($"\\{segment}\\", StringComparison.OrdinalIgnoreCase)
            || path.Contains($"/{segment}/", StringComparison.OrdinalIgnoreCase);
    }
}
