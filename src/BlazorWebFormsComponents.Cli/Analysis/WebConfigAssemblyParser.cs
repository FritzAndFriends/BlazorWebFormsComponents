using System.Text.RegularExpressions;
using System.Xml.Linq;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Analysis;

public class WebConfigAssemblyParser
{
    private static readonly Regex RegisterDirectiveRegex = new(
        @"<%@\s*Register\b(?<attributes>.*?)%>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    private static readonly Regex AttributeRegex = new(
        @"(?<name>\w+)\s*=\s*""(?<value>[^""]*)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ControlRegistrationInfo Parse(string? webConfigPath)
    {
        if (string.IsNullOrWhiteSpace(webConfigPath) || !File.Exists(webConfigPath))
            return new();

        try
        {
            var document = XDocument.Load(webConfigPath);
            return Parse(document, webConfigPath);
        }
        catch (Exception ex)
        {
            return new ControlRegistrationInfo
            {
                WebConfigPath = webConfigPath,
                Error = $"Could not parse Web.config: {ex.Message}"
            };
        }
    }

    public ControlRegistrationInfo ParseProject(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            return new();

        var webConfigPaths = RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".config")
            .Where(file => Path.GetFileName(file).Equals("Web.config", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var assemblies = new List<AssemblyRegistration>();
        var namespaceTags = new List<NamespaceTagRegistration>();
        var errors = new List<string>();

        foreach (var webConfigPath in webConfigPaths)
        {
            var parsed = Parse(webConfigPath);
            if (!string.IsNullOrWhiteSpace(parsed.Error))
            {
                errors.Add($"{Path.GetRelativePath(sourcePath, webConfigPath)}: {parsed.Error}");
                continue;
            }

            assemblies.AddRange(parsed.Assemblies);
            namespaceTags.AddRange(parsed.NamespaceTags);
        }

        var registerDirectives = new List<RegisterDirectiveRegistration>();
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".aspx", ".ascx", ".master"))
        {
            string content;
            try
            {
                content = File.ReadAllText(file);
            }
            catch
            {
                continue;
            }

            foreach (Match match in RegisterDirectiveRegex.Matches(content))
            {
                var attributes = ParseAttributes(match.Groups["attributes"].Value);
                var tagPrefix = GetAttribute(attributes, "TagPrefix");
                if (tagPrefix == null)
                    continue;

                var tagName = GetAttribute(attributes, "TagName");
                var nameSpace = GetAttribute(attributes, "Namespace");
                var assemblyName = NormalizeAssemblyName(GetAttribute(attributes, "Assembly"));
                var sourceVirtualPath = GetAttribute(attributes, "Src");
                if (tagName == null && nameSpace == null && assemblyName == null && sourceVirtualPath == null)
                    continue;

                registerDirectives.Add(new RegisterDirectiveRegistration(
                    tagPrefix,
                    tagName,
                    nameSpace,
                    assemblyName,
                    sourceVirtualPath,
                    Path.GetRelativePath(sourcePath, file)));
            }
        }

        var distinctAssemblies = DistinctBy(
            assemblies,
            registration => string.Join("|", registration.AssemblyName, registration.Namespace ?? string.Empty));
        var distinctNamespaceTags = DistinctBy(
            namespaceTags,
            registration => string.Join("|", registration.TagPrefix, registration.Namespace, registration.AssemblyName ?? string.Empty));
        var distinctRegisterDirectives = DistinctBy(
            registerDirectives,
            registration => string.Join("|",
                registration.TagPrefix,
                registration.TagName ?? string.Empty,
                registration.Namespace ?? string.Empty,
                registration.AssemblyName ?? string.Empty,
                registration.SourceVirtualPath ?? string.Empty,
                registration.SourceFilePath));

        return new ControlRegistrationInfo
        {
            WebConfigPath = webConfigPaths.FirstOrDefault(),
            Error = errors.Count > 0 ? string.Join(" | ", errors) : null,
            Assemblies = distinctAssemblies,
            NamespaceTags = distinctNamespaceTags,
            RegisterDirectives = distinctRegisterDirectives,
            PrefixToNamespaceMap = BuildPrefixToNamespaceMap(distinctNamespaceTags, distinctRegisterDirectives)
        };
    }

    private static ControlRegistrationInfo Parse(XDocument document, string webConfigPath)
    {
        var assemblies = document
            .Descendants()
            .Where(element => IsNamed(element, "assemblies") && element.Parent is not null && IsNamed(element.Parent, "compilation"))
            .SelectMany(element => element.Descendants().Where(child => IsNamed(child, "add")))
            .Select(element =>
            {
                var assemblyName = NormalizeAssemblyName(GetAttribute(element, "assembly"));
                return assemblyName == null
                    ? null
                    : new AssemblyRegistration(assemblyName, GetAttribute(element, "namespace"));
            })
            .Where(static registration => registration is not null)
            .Cast<AssemblyRegistration>();

        var namespaceTags = document
            .Descendants()
            .Where(element => IsNamed(element, "controls") && element.Parent is not null && IsNamed(element.Parent, "pages"))
            .SelectMany(element => element.Descendants().Where(child => IsNamed(child, "add")))
            .Select(element =>
            {
                var tagPrefix = GetAttribute(element, "tagPrefix");
                var nameSpace = GetAttribute(element, "namespace");
                if (tagPrefix == null || nameSpace == null)
                    return null;

                return new NamespaceTagRegistration(tagPrefix, nameSpace, NormalizeAssemblyName(GetAttribute(element, "assembly")));
            })
            .Where(static registration => registration is not null)
            .Cast<NamespaceTagRegistration>();

        var distinctAssemblies = DistinctBy(
            assemblies,
            registration => string.Join("|", registration.AssemblyName, registration.Namespace ?? string.Empty));
        var distinctNamespaceTags = DistinctBy(
            namespaceTags,
            registration => string.Join("|", registration.TagPrefix, registration.Namespace, registration.AssemblyName ?? string.Empty));

        return new ControlRegistrationInfo
        {
            WebConfigPath = webConfigPath,
            Assemblies = distinctAssemblies,
            NamespaceTags = distinctNamespaceTags,
            PrefixToNamespaceMap = BuildPrefixToNamespaceMap(distinctNamespaceTags, [])
        };
    }

    private static Dictionary<string, string> ParseAttributes(string attributeBlock)
    {
        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in AttributeRegex.Matches(attributeBlock))
        {
            attributes[match.Groups["name"].Value] = match.Groups["value"].Value.Trim();
        }

        return attributes;
    }

    private static string? FindWebConfig(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            return null;

        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".config"))
        {
            if (Path.GetFileName(file).Equals("Web.config", StringComparison.OrdinalIgnoreCase))
                return file;
        }

        return null;
    }

    private static string? GetAttribute(XElement element, string attributeName)
    {
        return element.Attributes()
            .FirstOrDefault(attribute => string.Equals(attribute.Name.LocalName, attributeName, StringComparison.OrdinalIgnoreCase))
            ?.Value
            .Trim()
            .NullIfEmpty();
    }

    private static string? GetAttribute(IReadOnlyDictionary<string, string> attributes, string attributeName)
    {
        return attributes.TryGetValue(attributeName, out var value)
            ? value.NullIfEmpty()
            : null;
    }

    private static bool IsNamed(XElement element, string localName) =>
        string.Equals(element.Name.LocalName, localName, StringComparison.OrdinalIgnoreCase);

    private static string? NormalizeAssemblyName(string? assemblyName)
    {
        if (string.IsNullOrWhiteSpace(assemblyName))
            return null;

        var commaIndex = assemblyName.IndexOf(',');
        var normalized = commaIndex >= 0 ? assemblyName[..commaIndex] : assemblyName;
        return normalized.Trim().NullIfEmpty();
    }

    private static IReadOnlyList<T> DistinctBy<T>(IEnumerable<T> values, Func<T, string> keySelector)
    {
        return values
            .GroupBy(keySelector, StringComparer.OrdinalIgnoreCase)
            .Select(static group => group.First())
            .ToList();
    }

    private static IReadOnlyDictionary<string, string> BuildPrefixToNamespaceMap(
        IEnumerable<NamespaceTagRegistration> namespaceTags,
        IEnumerable<RegisterDirectiveRegistration> registerDirectives)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in namespaceTags.OrderBy(static item => item.TagPrefix, StringComparer.OrdinalIgnoreCase))
        {
            if (!map.ContainsKey(registration.TagPrefix))
                map[registration.TagPrefix] = registration.Namespace;
        }

        foreach (var registration in registerDirectives
                     .Where(static item => !string.IsNullOrWhiteSpace(item.Namespace))
                     .OrderBy(static item => item.TagPrefix, StringComparer.OrdinalIgnoreCase))
        {
            if (!map.ContainsKey(registration.TagPrefix))
                map[registration.TagPrefix] = registration.Namespace!;
        }

        return map;
    }
}

public sealed record ControlRegistrationInfo
{
    public string? WebConfigPath { get; init; }
    public string? Error { get; init; }
    public IReadOnlyList<AssemblyRegistration> Assemblies { get; init; } = [];
    public IReadOnlyList<NamespaceTagRegistration> NamespaceTags { get; init; } = [];
    public IReadOnlyList<RegisterDirectiveRegistration> RegisterDirectives { get; init; } = [];
    public IReadOnlyDictionary<string, string> PrefixToNamespaceMap { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

public sealed record AssemblyRegistration(string AssemblyName, string? Namespace);
public sealed record NamespaceTagRegistration(string TagPrefix, string Namespace, string? AssemblyName);
public sealed record RegisterDirectiveRegistration(
    string TagPrefix,
    string? TagName,
    string? Namespace,
    string? AssemblyName,
    string? SourceVirtualPath,
    string SourceFilePath);

internal static class StringExtensions
{
    public static string? NullIfEmpty(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
