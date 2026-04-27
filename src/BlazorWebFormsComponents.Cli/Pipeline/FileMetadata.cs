namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Per-file metadata passed to each transform step.
/// </summary>
public class FileMetadata
{
    public required string SourceFilePath { get; init; }
    public required string OutputFilePath { get; init; }
    public required FileType FileType { get; init; }
    public required string OriginalContent { get; init; }
    public string? OutputRootPath { get; init; }
    public string? ProjectNamespace { get; init; }
    public string? CodeBehindContent { get; set; }
    public Dictionary<string, string> DataBindMap { get; set; } = new();

    /// <summary>
    /// Maps control ID → Blazor component type name (e.g., "lblStatus" → "Label", "gvData" → "GridView&lt;object&gt;").
    /// Populated by ComponentRefMarkupTransform, consumed by ComponentRefCodeBehindTransform.
    /// </summary>
    public Dictionary<string, string> ComponentRefs { get; set; } = new();

    /// <summary>
    /// Markup content after markup transforms. Set by pipeline before code-behind transforms.
    /// Code-behind transforms may modify this to update markup references (e.g., method renames).
    /// </summary>
    public string? MarkupContent { get; set; }

    public string FileName => Path.GetFileNameWithoutExtension(SourceFilePath);
}

public enum FileType
{
    Page,
    Master,
    Control
}
