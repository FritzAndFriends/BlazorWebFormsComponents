using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public partial class EntityFrameworkRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".cs"))
        {
            var content = File.ReadAllText(file);
            var classMatch = DbContextClassRegex().Match(content);
            if (!classMatch.Success)
                continue;

            profile.NeedsEntityFramework = true;
            var contextName = classMatch.Groups["name"].Value;

            if (profile.DbContextClassName is null)
            {
                profile.DbContextClassName = contextName;
                var namespaceMatch = NamespaceRegex().Match(content);
                if (namespaceMatch.Success)
                    profile.DbContextNamespace = namespaceMatch.Groups["name"].Value;
            }
            else if (!string.Equals(contextName, profile.DbContextClassName, StringComparison.Ordinal)
                     && !profile.AdditionalDbContextNames.Contains(contextName, StringComparer.Ordinal))
            {
                profile.AdditionalDbContextNames.Add(contextName);
            }

            if (classMatch.Groups["baseType"].Value.Contains("IdentityDbContext", StringComparison.Ordinal))
                profile.NeedsIdentity = true;

            foreach (Match connectionMatch in BaseConnectionRegex().Matches(content))
            {
                profile.ConnectionStringNames.Add(connectionMatch.Groups["name"].Value.Trim());
            }
        }

        foreach (var webConfigPath in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".config"))
        {
            if (!Path.GetFileName(webConfigPath).Equals("Web.config", StringComparison.OrdinalIgnoreCase))
                continue;

            TryAddConnectionStrings(webConfigPath, profile.ConnectionStringNames);
        }
    }

    private static void TryAddConnectionStrings(string webConfigPath, ICollection<string> connectionStrings)
    {
        try
        {
            var doc = XDocument.Load(webConfigPath);
            foreach (var addElement in doc.Root?.Element("connectionStrings")?.Elements("add") ?? [])
            {
                var name = addElement.Attribute("name")?.Value;
                if (!string.IsNullOrWhiteSpace(name))
                    connectionStrings.Add(name);
            }
        }
        catch
        {
            // Ignore malformed Web.config files during scaffold detection.
        }
    }

    [GeneratedRegex(@"class\s+(?<name>\w+)\s*:\s*(?:global::)?(?:[\w.]+\.)?(?<baseType>IdentityDbContext(?:<[^>{]+>)?|DbContext)", RegexOptions.Multiline)]
    private static partial Regex DbContextClassRegex();

    [GeneratedRegex(@"namespace\s+(?<name>[\w.]+)", RegexOptions.Multiline)]
    private static partial Regex NamespaceRegex();

    [GeneratedRegex(@":\s*base\(\s*@?""(?:name=)?(?<name>[^""]+)""\s*\)", RegexOptions.Multiline)]
    private static partial Regex BaseConnectionRegex();
}
