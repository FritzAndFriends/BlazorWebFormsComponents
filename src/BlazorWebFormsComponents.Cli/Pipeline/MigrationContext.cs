namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Per-file metadata and project-wide shared state for the migration pipeline.
/// </summary>
public class MigrationContext
{
    public required string SourcePath { get; init; }
    public required string OutputPath { get; init; }
    public required MigrationOptions Options { get; init; }
    public IReadOnlyList<SourceFile> SourceFiles { get; set; } = [];
    public TransformLog Log { get; } = new();
}

public class MigrationOptions
{
    public bool SkipScaffold { get; set; }
    public bool DryRun { get; set; }
    public bool Verbose { get; set; }
    public bool Overwrite { get; set; }
    public string? ReportPath { get; set; }
}

public class SourceFile
{
    public required string MarkupPath { get; init; }
    public string? CodeBehindPath { get; init; }
    public required string OutputPath { get; init; }
    public required FileType FileType { get; init; }

    public bool HasCodeBehind => CodeBehindPath != null && File.Exists(CodeBehindPath);
}

public class TransformLog
{
    private readonly List<TransformLogEntry> _entries = new();
    public IReadOnlyList<TransformLogEntry> Entries => _entries;

    public void Add(string file, string transform, string detail)
    {
        _entries.Add(new TransformLogEntry(file, transform, detail));
    }
}

public record TransformLogEntry(string File, string Transform, string Detail);
