using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Io;

/// <summary>
/// Copies App_Start files into the Blazor project root with Web Forms cleanup.
/// </summary>
public class AppStartCopier
{
    private readonly OutputWriter _outputWriter;

    public AppStartCopier(OutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    public async Task<int> CopyAsync(string sourcePath, string outputPath, MigrationReport report)
    {
        if (!Directory.Exists(sourcePath))
            return 0;

        var appStartDirs = Directory.EnumerateDirectories(sourcePath, "App_Start", SearchOption.AllDirectories).ToList();
        if (appStartDirs.Count == 0)
            return 0;

        var copied = 0;
        foreach (var appStartDir in appStartDirs)
        {
            foreach (var file in Directory.EnumerateFiles(appStartDir, "*.cs", SearchOption.TopDirectoryOnly))
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);
                var destFile = Path.Combine(
                    outputPath,
                    "migration-artifacts",
                    "App_Start",
                    Path.GetFileName(file) + ".txt");
                var content = await File.ReadAllTextAsync(file);
                content = TransformContent(content, Path.GetFileName(file), report, relativePath);

                await _outputWriter.WriteFileAsync(destFile, content, $"Manual App_Start artifact: {relativePath}");
                copied++;
            }
        }

        if (copied > 0)
            Console.WriteLine($"  App_Start files quarantined: {copied}");

        return copied;
    }

    private static string TransformContent(string content, string fileName, MigrationReport report, string relativePath)
    {
        var transformed = "// TODO: Review — auto-copied from App_Start. Blazor has no App_Start convention.\n"
            + "// TODO: Move relevant configuration to Program.cs or appropriate service registration.\n\n"
            + content;

        transformed = Regex.Replace(transformed, @"(?m)^\s*\[assembly:\s*[^\]]*\]\s*\r?\n?", "");
        transformed = Regex.Replace(transformed, @"using\s+System\.Web\.UI(\.\w+)*;\s*\r?\n?", "");
        transformed = Regex.Replace(transformed, @"using\s+System\.Web\.Security;\s*\r?\n?", "");

        if (Regex.IsMatch(transformed, @"\b(Bundle|BundleTable|BundleCollection)\b"))
        {
            transformed = Regex.Replace(
                transformed,
                @"using\s+System\.Web\.Optimization;\s*\r?\n?",
                "// using System.Web.Optimization; // BWFC: BundleConfig stubs available via BlazorWebFormsComponents namespace\n");
        }
        else
        {
            transformed = Regex.Replace(transformed, @"using\s+System\.Web\.Optimization;\s*\r?\n?", "");
        }

        if (Regex.IsMatch(transformed, @"\b(Route|RouteTable|RouteCollection)\b"))
        {
            transformed = Regex.Replace(
                transformed,
                @"using\s+System\.Web\.Routing;\s*\r?\n?",
                "// using System.Web.Routing; // BWFC: RouteConfig stubs available via BlazorWebFormsComponents namespace\n");
        }
        else
        {
            transformed = Regex.Replace(transformed, @"using\s+System\.Web\.Routing;\s*\r?\n?", "");
        }

        transformed = Regex.Replace(transformed, @"using\s+System\.Web(\.\w+)*;\s*\r?\n?", "");
        transformed = Regex.Replace(transformed, @"using\s+Microsoft\.AspNet(\.\w+)*;\s*\r?\n?", "");
        transformed = Regex.Replace(transformed, @"using\s+Microsoft\.Owin(\.\w+)*;\s*\r?\n?", "");
        transformed = Regex.Replace(transformed, @"using\s+Owin;\s*\r?\n?", "");

        if (fileName.Equals("WebApiConfig.cs", StringComparison.OrdinalIgnoreCase))
        {
            report.AddManualItem(relativePath, 0, "AppStart", "WebApiConfig detected — migrate to minimal API endpoints in Program.cs");
            transformed = "// TODO: BWFC — Web API configuration should be migrated to minimal API endpoints in Program.cs\n" + transformed;
        }

        return transformed;
    }
}
