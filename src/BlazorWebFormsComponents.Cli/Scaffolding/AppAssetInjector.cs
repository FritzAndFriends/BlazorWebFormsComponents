using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Injects detected CSS and JavaScript references into Components/App.razor.
/// </summary>
public class AppAssetInjector
{
    private readonly OutputWriter _outputWriter;

    public AppAssetInjector(OutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    public async Task<AssetInjectionResult> InjectAsync(string sourcePath, string outputPath)
    {
        var appRazorPath = Path.Combine(outputPath, "Components", "App.razor");
        if (!File.Exists(appRazorPath))
            return new AssetInjectionResult();

        var appContent = await File.ReadAllTextAsync(appRazorPath);
        var cssRefs = GetCssReferences(sourcePath, outputPath)
            .Where(reference => !appContent.Contains(reference.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
        var scriptRefs = GetScriptReferences(sourcePath, outputPath)
            .Where(reference => !appContent.Contains(reference.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (cssRefs.Count == 0 && scriptRefs.Count == 0)
            return new AssetInjectionResult();

        if (cssRefs.Count > 0 && appContent.Contains("<HeadOutlet", StringComparison.Ordinal))
        {
            var cssBlock = string.Join(Environment.NewLine, cssRefs) + Environment.NewLine;
            appContent = new Regex(@"(\s*<HeadOutlet\s*/?>)").Replace(appContent, Environment.NewLine + cssBlock + "$1", 1);
        }

        if (scriptRefs.Count > 0 && appContent.Contains("</body>", StringComparison.OrdinalIgnoreCase))
        {
            var scriptBlock = string.Join(Environment.NewLine, scriptRefs) + Environment.NewLine;
            appContent = new Regex(@"\s*</body>", RegexOptions.IgnoreCase).Replace(appContent, Environment.NewLine + scriptBlock + "</body>", 1);
        }

        await _outputWriter.WriteFileAsync(appRazorPath, appContent, "Post-process Components/App.razor asset references");
        return new AssetInjectionResult(cssRefs.Count, scriptRefs.Count);
    }

    private static List<string> GetCssReferences(string sourcePath, string outputPath)
    {
        var references = new List<string>();
        references.AddRange(GetCdnHeadReferences(sourcePath));
        references.AddRange(GetLocalCssReferences(outputPath));
        references.AddRange(GetNuGetAssetReferences(outputPath, isCss: true));
        return references.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static List<string> GetScriptReferences(string sourcePath, string outputPath)
    {
        var references = new List<string>();
        references.AddRange(GetLocalScriptReferences(outputPath));
        references.AddRange(GetNuGetAssetReferences(outputPath, isCss: false));
        return references.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static IEnumerable<string> GetCdnHeadReferences(string sourcePath)
    {
        var masterFile = Directory.EnumerateFiles(sourcePath, "Site.Master", SearchOption.AllDirectories).FirstOrDefault();
        if (masterFile is null)
            return [];

        var masterContent = File.ReadAllText(masterFile);
        var refs = new List<string>();

        var cdnLinkRegex = new Regex(@"<link\s[^>]*href\s*=\s*""(https?://[^""]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs)[^""]*)""[^>]*>", RegexOptions.IgnoreCase);
        refs.AddRange(cdnLinkRegex.Matches(masterContent).Select(m => "    " + m.Value.Trim()));

        var cdnScriptRegex = new Regex(@"<script\s[^>]*src\s*=\s*""(https?://[^""]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs|jquery)[^""]*)""[^>]*>\s*</script>", RegexOptions.IgnoreCase);
        refs.AddRange(cdnScriptRegex.Matches(masterContent).Select(m => "    " + m.Value.Trim()));

        return refs;
    }

    private static IEnumerable<string> GetLocalCssReferences(string outputPath)
    {
        var wwwroot = Path.Combine(outputPath, "wwwroot");
        if (!Directory.Exists(wwwroot))
            return [];

        var refs = new List<string>();
        refs.AddRange(GetCssFiles(Path.Combine(wwwroot, "Content"), wwwroot));

        if (refs.Count == 0)
            refs.AddRange(GetCssFiles(Path.Combine(wwwroot, "css"), wwwroot));

        refs.AddRange(Directory.EnumerateFiles(wwwroot, "*.css", SearchOption.TopDirectoryOnly)
            .Select(file => $"    <link rel=\"stylesheet\" href=\"/{Path.GetFileName(file)}\" />"));

        return refs;
    }

    private static IEnumerable<string> GetCssFiles(string directory, string wwwroot)
    {
        if (!Directory.Exists(directory))
            return [];

        return Directory.EnumerateFiles(directory, "*.css", SearchOption.AllDirectories)
            .Select(file => $"    <link rel=\"stylesheet\" href=\"/{Path.GetRelativePath(wwwroot, file).Replace('\\', '/')}\" />");
    }

    private static IEnumerable<string> GetLocalScriptReferences(string outputPath)
    {
        var scriptsDir = Path.Combine(outputPath, "wwwroot", "Scripts");
        if (!Directory.Exists(scriptsDir))
            return [];

        var files = Directory.EnumerateFiles(scriptsDir, "*.js", SearchOption.TopDirectoryOnly)
            .Where(file =>
            {
                var fileName = Path.GetFileName(file);
                return !fileName.Contains("intellisense", StringComparison.OrdinalIgnoreCase)
                    && !fileName.Equals("_references.js", StringComparison.OrdinalIgnoreCase);
            })
            .Select(Path.GetFileName)
            .Where(static fileName => fileName is not null)
            .Cast<string>()
            .ToList();

        var ordered = new List<string>();
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^jquery.*\.min\.js$", RegexOptions.IgnoreCase));
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^jquery-[\d.]+\.js$", RegexOptions.IgnoreCase));
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^modernizr.*\.js$", RegexOptions.IgnoreCase));
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^respond.*\.min\.js$", RegexOptions.IgnoreCase));
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^respond.*\.js$", RegexOptions.IgnoreCase));
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^bootstrap.*\.min\.js$", RegexOptions.IgnoreCase));
        AddFirstMatch(files, ordered, name => Regex.IsMatch(name, @"^bootstrap.*\.js$", RegexOptions.IgnoreCase));

        ordered.AddRange(files.Where(file => !ordered.Contains(file, StringComparer.OrdinalIgnoreCase)).OrderBy(file => file, StringComparer.OrdinalIgnoreCase));

        return ordered.Select(file => $"    <script src=\"/Scripts/{file}\"></script>");
    }

    private static void AddFirstMatch(IEnumerable<string> files, ICollection<string> ordered, Func<string, bool> predicate)
    {
        var match = files.FirstOrDefault(predicate);
        if (!string.IsNullOrEmpty(match) && !ordered.Contains(match, StringComparer.OrdinalIgnoreCase))
            ordered.Add(match);
    }

    private static IEnumerable<string> GetNuGetAssetReferences(string outputPath, bool isCss)
    {
        var snippetPath = Path.Combine(outputPath, "AssetReferences.html");
        if (!File.Exists(snippetPath))
            return [];

        var pattern = isCss ? "<link " : "<script ";
        return File.ReadLines(snippetPath)
            .Select(line => line.TrimEnd())
            .Where(line => line.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            .Select(line => "    " + line.Trim());
    }
}

public sealed record AssetInjectionResult(int CssReferencesInjected = 0, int ScriptReferencesInjected = 0);
