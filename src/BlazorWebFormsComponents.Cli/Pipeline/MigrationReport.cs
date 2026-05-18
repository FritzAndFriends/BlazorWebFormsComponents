using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Summary report from a migration run. Supports JSON serialization and console output.
/// </summary>
public class MigrationReport
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public int FilesProcessed { get; set; }
    public int FilesWritten { get; set; }
    public int TransformsApplied { get; set; }
    public int SemanticPatternsApplied { get; set; }
    public int ScaffoldFilesGenerated { get; set; }
    public int StaticFilesCopied { get; set; }
    public List<string> Errors { get; } = [];
    public List<string> Warnings { get; } = [];
    public List<ManualItem> ManualItems { get; } = [];
    public List<string> GeneratedFiles { get; } = [];

    /// <summary>
    /// Add a structured manual-intervention item to the report.
    /// </summary>
    public void AddManualItem(string file, int line, string category, string description, string severity = "medium")
    {
        ManualItems.Add(new ManualItem(file, line, category, description, severity));
    }

    /// <summary>
    /// Serialize the report to JSON for the --report flag.
    /// </summary>
    public string ToJson()
    {
        return JsonSerializer.Serialize(new
        {
            FilesProcessed,
            FilesWritten,
            TransformsApplied,
            SemanticPatternsApplied,
            ScaffoldFilesGenerated,
            StaticFilesCopied,
            ErrorCount = Errors.Count,
            WarningCount = Warnings.Count,
            ManualItemCount = ManualItems.Count,
            Errors,
            Warnings,
            ManualItems,
            GeneratedFiles
        }, s_jsonOptions);
    }

    /// <summary>
    /// Print a console summary of the migration.
    /// </summary>
    public void PrintSummary()
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine("  Migration Report");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine($"  Files processed:    {FilesProcessed}");
        Console.WriteLine($"  Files written:      {FilesWritten}");
        Console.WriteLine($"  Transforms applied: {TransformsApplied}");
        Console.WriteLine($"  Semantic patterns:  {SemanticPatternsApplied}");
        Console.WriteLine($"  Scaffold files:     {ScaffoldFilesGenerated}");
        Console.WriteLine($"  Static files:       {StaticFilesCopied}");

        if (ManualItems.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  Manual items:       {ManualItems.Count}");
            Console.ResetColor();
        }

        if (Warnings.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  Warnings:           {Warnings.Count}");
            foreach (var w in Warnings)
                Console.WriteLine($"    ⚠ {w}");
            Console.ResetColor();
        }

        if (Errors.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Errors:             {Errors.Count}");
            foreach (var e in Errors)
                Console.WriteLine($"    ✗ {e}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Errors:             0 ✓");
            Console.ResetColor();
        }

        Console.WriteLine("═══════════════════════════════════════════════");
    }

    /// <summary>
    /// Write JSON report to file if a path is specified.
    /// </summary>
    public async Task WriteReportFileAsync(string? reportPath)
    {
        if (string.IsNullOrEmpty(reportPath))
            return;

        var directory = Path.GetDirectoryName(reportPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(reportPath, ToJson());
        Console.WriteLine($"  Report written to: {reportPath}");
    }
}
