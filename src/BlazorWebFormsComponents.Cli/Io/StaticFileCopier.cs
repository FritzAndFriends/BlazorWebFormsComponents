using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Io;

/// <summary>
/// Copies static files (CSS, JS, images, fonts) to wwwroot/ in the output directory.
/// Preserves relative directory structure from the source project.
/// </summary>
public class StaticFileCopier
{
    private static readonly HashSet<string> StaticExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico",
        ".woff", ".woff2", ".ttf", ".eot", ".map", ".json",
        ".bmp", ".webp", ".mp4", ".webm", ".ogg", ".mp3", ".wav"
    };

    private static readonly HashSet<string> ExcludedDirs = new(StringComparer.OrdinalIgnoreCase)
    {
        "bin", "obj", "packages", "node_modules", ".vs", ".git"
    };

    private readonly OutputWriter _outputWriter;

    public StaticFileCopier(OutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    /// <summary>
    /// Scan the source directory for static files and copy them to output/wwwroot/.
    /// </summary>
    public int CopyStaticFiles(string sourcePath, string outputPath, bool verbose)
    {
        if (!Directory.Exists(sourcePath))
            return 0;

        var count = 0;
        var wwwrootOutput = Path.Combine(outputPath, "wwwroot");

        foreach (var file in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(file);
            if (!StaticExtensions.Contains(ext))
                continue;

            // Skip excluded directories
            var relativePath = Path.GetRelativePath(sourcePath, file);
            var topDir = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
            if (ExcludedDirs.Contains(topDir))
                continue;

            // Skip Web.config, packages.config, etc.
            var fileName = Path.GetFileName(file);
            if (fileName.EndsWith(".config", StringComparison.OrdinalIgnoreCase))
                continue;

            // Skip appsettings.json (we generate our own)
            if (fileName.Equals("appsettings.json", StringComparison.OrdinalIgnoreCase))
                continue;

            var destPath = Path.Combine(wwwrootOutput, relativePath);
            _outputWriter.CopyFile(file, destPath, $"Static: {relativePath}");
            count++;
        }

        if (verbose || count > 0)
            Console.WriteLine($"  Static files copied: {count} → wwwroot/");

        return count;
    }
}
