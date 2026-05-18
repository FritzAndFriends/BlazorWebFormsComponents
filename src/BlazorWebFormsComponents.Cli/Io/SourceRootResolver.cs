namespace BlazorWebFormsComponents.Cli.Io;

/// <summary>
/// Resolves the effective Web Forms app root for migration input.
/// Some solutions wrap the real app in a child folder with the same name
/// as the solution root (for example samples\WingtipToys\WingtipToys\).
/// </summary>
public class SourceRootResolver
{
    private static readonly HashSet<string> MarkupExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".aspx", ".ascx", ".master"
    };

    public string Resolve(string inputPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath) || File.Exists(inputPath) || !Directory.Exists(inputPath))
            return inputPath;

        var inputDirectory = new DirectoryInfo(inputPath);
        var candidatePath = Path.Combine(inputDirectory.FullName, inputDirectory.Name);
        if (!Directory.Exists(candidatePath))
            return inputPath;

        if (!ContainsMarkup(candidatePath))
            return inputPath;

        var candidateRoot = EnsureTrailingSeparator(Path.GetFullPath(candidatePath));
        var markupFiles = Directory.EnumerateFiles(inputDirectory.FullName, "*.*", SearchOption.AllDirectories)
            .Where(file => MarkupExtensions.Contains(Path.GetExtension(file)));

        foreach (var markupFile in markupFiles)
        {
            var fullPath = Path.GetFullPath(markupFile);
            if (!fullPath.StartsWith(candidateRoot, StringComparison.OrdinalIgnoreCase))
                return inputPath;
        }

        return candidatePath;
    }

    private static bool ContainsMarkup(string directory)
    {
        return Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
            .Any(file => MarkupExtensions.Contains(Path.GetExtension(file)));
    }

    private static string EnsureTrailingSeparator(string path)
    {
        if (path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar))
            return path;

        return path + Path.DirectorySeparatorChar;
    }
}
