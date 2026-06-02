using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

/// <summary>
/// Immutable per-file context passed to semantic pattern matchers and applicators.
/// </summary>
public sealed class SemanticPatternContext
{
    public required MigrationContext MigrationContext { get; init; }
    public required SourceFile SourceFile { get; init; }
    public required FileMetadata Metadata { get; init; }
    public required MigrationReport Report { get; init; }
    public required string Markup { get; init; }
    public string? CodeBehind { get; init; }
}

/// <summary>
/// Result of a pattern-matching attempt.
/// </summary>
public sealed record SemanticPatternMatch(bool IsMatch, string? Evidence = null)
{
    public static SemanticPatternMatch NoMatch() => new(false);
    public static SemanticPatternMatch Match(string? evidence = null) => new(true, evidence);
}

/// <summary>
/// Replacement content emitted by a semantic pattern.
/// </summary>
public sealed record SemanticPatternResult(string Markup, string? CodeBehind, string? Detail = null)
{
    public static SemanticPatternResult FromContext(SemanticPatternContext context, string? detail = null) =>
        new(context.Markup, context.CodeBehind, detail);
}

/// <summary>
/// A single applied semantic pattern entry for diagnostics and tests.
/// </summary>
public sealed record AppliedSemanticPattern(string PatternId, string Detail);

/// <summary>
/// Aggregate execution result of the semantic pattern catalog for a single file.
/// </summary>
public sealed record SemanticPatternExecutionResult(
    string Markup,
    string? CodeBehind,
    IReadOnlyList<AppliedSemanticPattern> AppliedPatterns);

