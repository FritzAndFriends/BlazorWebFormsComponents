namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// A structured manual-intervention item for the migration report.
/// Category values use the bwfc-* slug convention for Copilot L2 orchestration.
/// </summary>
public record ManualItem(
    string File,
    int Line,
    string Category,
    string Description,
    string Severity
);
