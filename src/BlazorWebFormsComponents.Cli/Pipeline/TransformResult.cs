namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Immutable result of each transform step.
/// </summary>
public sealed record TransformResult(
    string TransformName,
    string Content,
    bool WasModified,
    string? Detail = null
);
