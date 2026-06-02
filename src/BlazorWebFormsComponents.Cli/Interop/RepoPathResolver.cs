namespace BlazorWebFormsComponents.Cli.Interop;

internal static class RepoPathResolver
{
    public static string FindFileFromRepoRoot(string relativePath)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, relativePath);
            if (File.Exists(candidate))
                return candidate;

            current = current.Parent;
        }

        throw new FileNotFoundException($"Could not locate '{relativePath}' from '{AppContext.BaseDirectory}'.");
    }
}
