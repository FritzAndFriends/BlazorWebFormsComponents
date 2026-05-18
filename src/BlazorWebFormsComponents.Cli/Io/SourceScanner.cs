using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Io;

/// <summary>
/// Discovers .aspx, .ascx, .master files and pairs them with code-behind files.
/// </summary>
public class SourceScanner
{
    private static readonly string[] MarkupExtensions = [".aspx", ".ascx", ".master"];

    /// <summary>
    /// Scans the input path for Web Forms source files.
    /// </summary>
    public IReadOnlyList<SourceFile> Scan(string inputPath, string outputPath)
    {
        var files = new List<SourceFile>();

        if (File.Exists(inputPath))
        {
            // Single file mode
            var ext = Path.GetExtension(inputPath).ToLowerInvariant();
            if (MarkupExtensions.Contains(ext))
            {
                files.Add(CreateSourceFile(inputPath, inputPath, outputPath));
            }
            return files;
        }

        if (!Directory.Exists(inputPath))
            return files;

        foreach (var ext in MarkupExtensions)
        {
            foreach (var file in Directory.EnumerateFiles(inputPath, $"*{ext}", SearchOption.AllDirectories))
            {
                files.Add(CreateSourceFile(file, inputPath, outputPath));
            }
        }

        return files;
    }

    private static SourceFile CreateSourceFile(string filePath, string inputRoot, string outputRoot)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var fileType = ext switch
        {
            ".master" => FileType.Master,
            ".ascx" => FileType.Control,
            _ => FileType.Page
        };

        // Determine output path
        string outputPath;
        if (File.Exists(inputRoot))
        {
            // Single file — output to outputRoot directory
            var razorName = Path.GetFileNameWithoutExtension(filePath) + ".razor";
            outputPath = Path.Combine(outputRoot, razorName);
        }
        else
        {
            var relativePath = Path.GetRelativePath(inputRoot, filePath);
            var razorRelPath = ext switch
            {
                ".aspx" => Path.ChangeExtension(relativePath, ".razor"),
                ".ascx" => Path.ChangeExtension(relativePath, ".razor"),
                ".master" => Path.ChangeExtension(relativePath, ".razor"),
                _ => relativePath
            };
            outputPath = Path.Combine(outputRoot, razorRelPath);
        }

        // Look for code-behind
        var codeBehindPath = filePath + ".cs";
        if (!File.Exists(codeBehindPath))
        {
            codeBehindPath = filePath + ".vb";
            if (!File.Exists(codeBehindPath))
                codeBehindPath = null;
        }

        return new SourceFile
        {
            MarkupPath = filePath,
            CodeBehindPath = codeBehindPath,
            OutputPath = outputPath,
            FileType = fileType
        };
    }
}
