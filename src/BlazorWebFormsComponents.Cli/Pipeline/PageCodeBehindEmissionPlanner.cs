using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Pipeline;

internal static partial class PageCodeBehindEmissionPlanner
{
    private static readonly Regex PartialClassRegex = new(@"\bpartial\s+class\s+\w+", RegexOptions.Compiled);
    private static readonly Regex LowercaseHtmlIdRegex = new(@"<([a-z][\w:-]*)\b[^>]*\bid=""\w+""", RegexOptions.Compiled);

    private static readonly (Regex Pattern, string Reason)[] UnsafeMarkupPatterns =
    [
        (new Regex(@"<%", RegexOptions.Compiled), "Unresolved Web Forms server blocks remain in markup."),
        (new Regex(@"<webopt:", RegexOptions.Compiled | RegexOptions.IgnoreCase), "Unresolved Web Optimization markup remains in the page shell."),
        (new Regex(@"\bScripts\.Render\s*\(", RegexOptions.Compiled), "Bundling helper calls still remain in the generated page markup."),
        (LowercaseHtmlIdRegex, "Lowercase HTML elements with server IDs still require explicit server-element shims.")
    ];

    private static readonly (Regex Pattern, string Reason)[] UnsafeCodePatterns =
    [
        (new Regex(@"\bHttpContext\.Current\b", RegexOptions.Compiled), "Legacy HttpContext.Current access still requires runtime-specific migration."),
        (new Regex(@"\bGetOwinContext\s*\(", RegexOptions.Compiled), "OWIN authentication access still requires app-level migration."),
        (new Regex(@"\bFormsAuthentication\b", RegexOptions.Compiled), "FormsAuthentication usage is not page-codebehind safe."),
        (new Regex(@"\bPage\.PreLoad\b|\bPreLoad\s*\+=", RegexOptions.Compiled), "PreLoad event wiring still needs lifecycle normalization."),
        (new Regex(@"\bDataControlFieldCell\b|\bGridViewRow\b|\bRows\b|\bFindControl\s*\(", RegexOptions.Compiled), "Row-oriented server control traversal still needs semantic migration."),
        (new Regex(@"\bInnerText\b", RegexOptions.Compiled), "HTML server-element members still require explicit runtime shims."),
        (new Regex(@"\bAttributes\s*\[", RegexOptions.Compiled), "HTML attribute mutation still requires server-element shims.")
    ];

    public static CodeBehindEmissionPlan Create(FileMetadata metadata, string markup, string codeBehind)
    {
        if (metadata.FileType != FileType.Page)
        {
            return Artifact("Only page code-behind is emitted into the compile surface.");
        }

        if (!string.IsNullOrWhiteSpace(metadata.CompileSurfaceStubReason) && !string.IsNullOrWhiteSpace(metadata.CompileSurfaceOriginalCodeBehind))
        {
            return Stub(metadata.CompileSurfaceStubReason!, metadata.CompileSurfaceOriginalCodeBehind!);
        }

        if (!PartialClassRegex.IsMatch(codeBehind))
        {
            return Artifact("Transformed code-behind did not produce a compilable partial class.");
        }

        foreach (var (pattern, reason) in UnsafeMarkupPatterns)
        {
            if (pattern.IsMatch(markup))
            {
                return Artifact(reason);
            }
        }

        foreach (var (pattern, reason) in UnsafeCodePatterns)
        {
            if (pattern.IsMatch(codeBehind))
            {
                return Artifact(reason);
            }
        }

        return Compile();
    }

    private static CodeBehindEmissionPlan Compile() => new(true, null, null);
    private static CodeBehindEmissionPlan Stub(string reason, string artifactContent) => new(true, reason, artifactContent);
    private static CodeBehindEmissionPlan Artifact(string reason) => new(false, reason, null);
}

public sealed record CodeBehindEmissionPlan(bool EmitToCompileSurface, string? ArtifactReason, string? ArtifactContent);
