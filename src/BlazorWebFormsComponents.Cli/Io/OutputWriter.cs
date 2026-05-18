using System.Text;

namespace BlazorWebFormsComponents.Cli.Io;

/// <summary>
/// Centralized output writer that respects --dry-run and tracks all written files.
/// UTF-8 encoding without BOM, creates directories as needed.
/// </summary>
public class OutputWriter
{
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    private readonly List<string> _writtenFiles = [];

    public bool DryRun { get; set; }
    public bool Verbose { get; set; }

    public IReadOnlyList<string> WrittenFiles => _writtenFiles;

    /// <summary>
    /// Write content to a file, respecting dry-run mode.
    /// </summary>
    public async Task WriteFileAsync(string path, string content, string description)
    {
        if (DryRun)
        {
            Console.WriteLine($"  [dry-run] Would write: {path}");
            if (Verbose)
                Console.WriteLine($"            ({description})");
            return;
        }

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(path, content, Utf8NoBom);
        _writtenFiles.Add(path);

        if (Verbose)
            Console.WriteLine($"  ✓ {path} ({description})");
    }

    /// <summary>
    /// Copy a file to the output, respecting dry-run mode.
    /// </summary>
    public void CopyFile(string source, string destination, string description)
    {
        if (DryRun)
        {
            Console.WriteLine($"  [dry-run] Would copy: {source} → {destination}");
            return;
        }

        var directory = Path.GetDirectoryName(destination);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.Copy(source, destination, overwrite: true);
        _writtenFiles.Add(destination);

        if (Verbose)
            Console.WriteLine($"  ✓ {destination} ({description})");
    }

    public void Reset()
    {
        _writtenFiles.Clear();
    }
}
