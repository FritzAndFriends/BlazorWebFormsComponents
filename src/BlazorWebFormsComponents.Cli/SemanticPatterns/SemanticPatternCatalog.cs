using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

/// <summary>
/// Ordered registry of semantic migration patterns. This gives the CLI a bounded
/// place to grow recurring page-shape rewrites without turning the main transform
/// list into one monolithic semantic pass.
/// </summary>
public sealed class SemanticPatternCatalog
{
    private readonly IReadOnlyList<ISemanticPattern> _patterns;

    public SemanticPatternCatalog(IEnumerable<ISemanticPattern> patterns)
    {
        _patterns = patterns.OrderBy(p => p.Order).ToList();
    }

    public SemanticPatternExecutionResult Apply(
        MigrationContext migrationContext,
        SourceFile sourceFile,
        FileMetadata metadata,
        string markup,
        string? codeBehind,
        MigrationReport report)
    {
        var appliedPatterns = new List<AppliedSemanticPattern>();

        foreach (var pattern in _patterns)
        {
            var context = new SemanticPatternContext
            {
                MigrationContext = migrationContext,
                SourceFile = sourceFile,
                Metadata = metadata,
                Report = report,
                Markup = markup,
                CodeBehind = codeBehind
            };

            var match = pattern.Match(context);
            if (!match.IsMatch)
            {
                continue;
            }

            var result = pattern.Apply(context);
            markup = result.Markup;
            codeBehind = result.CodeBehind;
            metadata.MarkupContent = markup;
            metadata.CodeBehindContent = codeBehind;

            var detail = result.Detail ?? match.Evidence ?? $"Applied semantic pattern '{pattern.Id}'.";
            appliedPatterns.Add(new AppliedSemanticPattern(pattern.Id, detail));
            migrationContext.Log.Add(sourceFile.MarkupPath, pattern.Id, detail);
            report.SemanticPatternsApplied++;
        }

        return new SemanticPatternExecutionResult(markup, codeBehind, appliedPatterns);
    }
}

