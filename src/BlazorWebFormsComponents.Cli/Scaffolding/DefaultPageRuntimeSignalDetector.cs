namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Detects the default page for the application and sets DefaultPageRoute in the profile.
///
/// Gate: Only applies when:
///   1. A Default.aspx file exists at the root of the source project, OR
///   2. Web.config contains a &lt;defaultDocument&gt; section specifying a default page
///
/// The detected route is used by ProgramCsEmitter to generate:
///   app.MapGet("/", () => Results.Redirect("/DefaultPage"));
/// </summary>
public class DefaultPageRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        // Strategy 1: Look for Default.aspx at root level
        var defaultAspx = RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".aspx")
            .FirstOrDefault(f =>
                Path.GetFileName(f).Equals("Default.aspx", StringComparison.OrdinalIgnoreCase) &&
                IsRootLevel(sourcePath, f));

        if (defaultAspx != null)
        {
            // Default.aspx already gets @page "/" from PageDirectiveTransform — no redirect needed
            return;
        }

        // Strategy 2: Look for a Home.aspx at root level (common pattern)
        // Home.aspx now gets @page "/" from PageDirectiveTransform — no redirect needed
        var homeAspx = RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".aspx")
            .FirstOrDefault(f =>
                Path.GetFileName(f).Equals("Home.aspx", StringComparison.OrdinalIgnoreCase) &&
                IsRootLevel(sourcePath, f));

        if (homeAspx != null)
        {
            return;
        }

        // Strategy 3: Check web.config for <defaultDocument>
        var webConfigPath = FindWebConfig(sourcePath);
        if (webConfigPath != null)
        {
            try
            {
                var doc = System.Xml.Linq.XDocument.Load(webConfigPath);
                var defaultDoc = doc.Root?
                    .Element("system.webServer")?
                    .Element("defaultDocument")?
                    .Element("files")?
                    .Elements("add")
                    .FirstOrDefault()?
                    .Attribute("value")?.Value;

                if (!string.IsNullOrEmpty(defaultDoc))
                {
                    var route = Path.GetFileNameWithoutExtension(defaultDoc);
                    // Default and Index already get @page "/" from PageDirectiveTransform
                    if (route.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                        route.Equals("Index", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                    profile.DefaultPageRoute = $"/{route}";
                }
            }
            catch
            {
                // Ignore XML parse errors
            }
        }
    }

    private static bool IsRootLevel(string sourcePath, string filePath)
    {
        var relativePath = Path.GetRelativePath(sourcePath, filePath);
        // Root level means no directory separators (just a filename)
        return !relativePath.Contains(Path.DirectorySeparatorChar) &&
               !relativePath.Contains(Path.AltDirectorySeparatorChar);
    }

    private static string? FindWebConfig(string sourcePath)
    {
        var directPath = Path.Combine(sourcePath, "Web.config");
        if (File.Exists(directPath)) return directPath;

        // Check nested project folder
        foreach (var dir in Directory.EnumerateDirectories(sourcePath))
        {
            var nested = Path.Combine(dir, "Web.config");
            if (File.Exists(nested)) return nested;
        }

        return null;
    }
}
