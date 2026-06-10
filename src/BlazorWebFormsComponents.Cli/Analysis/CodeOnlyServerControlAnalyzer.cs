using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Scaffolding;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace BlazorWebFormsComponents.Cli.Analysis;

public sealed class CodeOnlyServerControlAnalyzer
{
    private static readonly Regex NamespaceRegex = new(
        @"^\s*namespace\s+(?<namespace>[A-Za-z_][\w.]*)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex ClassRegex = new(
        @"\bclass\s+(?<name>[A-Za-z_]\w*)\s*:\s*(?<base>[A-Za-z_][\w\.<>,:\s]*)",
        RegexOptions.Compiled);

    private static readonly HashSet<string> SupportedBaseTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Control",
        "WebControl",
        "CompositeControl",
        "DataBoundControl",
        "BaseDataBoundControl",
        "TemplatedControl"
    };

    // Properties already present on BWFC base classes — skip to avoid [Parameter] duplicates
    private static readonly HashSet<string> BwfcInheritedPropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "ID", "Visible", "Enabled", "TabIndex", "runat", "ViewState",
        "CssClass", "Width", "Height", "BackColor", "ForeColor", "BorderColor",
        "Font", "Style", "ClientID", "UniqueID"
    };

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest);

    public IReadOnlyList<CodeOnlyServerControlDescriptor> Analyze(
        string sourcePath,
        ControlRegistrationInfo controlRegistrations)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            return [];

        var descriptors = new Dictionary<string, CodeOnlyServerControlDescriptor>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".cs"))
        {
            if (IsExcludedCodeFile(file) || HasMarkupCompanion(file))
                continue;

            string content;
            try
            {
                content = File.ReadAllText(file);
            }
            catch
            {
                continue;
            }

            var namespaceName = ReadNamespace(content);
            if (string.IsNullOrWhiteSpace(namespaceName))
                continue;

            foreach (Match classMatch in ClassRegex.Matches(content))
            {
                var className = classMatch.Groups["name"].Value;
                var baseType = NormalizeBaseType(classMatch.Groups["base"].Value);
                if (!IsServerControlBase(baseType))
                    continue;

                var key = $"{namespaceName}.{className}";
                if (descriptors.ContainsKey(key))
                    continue;

                var matchingPrefixes = controlRegistrations.PrefixToNamespaceMap
                    .Where(pair => string.Equals(pair.Value, namespaceName, StringComparison.Ordinal))
                    .Select(pair => pair.Key)
                    .OrderBy(static prefix => prefix, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var (properties, events) = ExtractPublicSurface(content, className);

                descriptors[key] = new CodeOnlyServerControlDescriptor
                {
                    ClassName = className,
                    Namespace = namespaceName,
                    BaseType = baseType,
                    SourceFilePath = Path.GetRelativePath(sourcePath, file),
                    TagPrefixes = matchingPrefixes,
                    Properties = properties,
                    Events = events
                };
            }
        }

        return descriptors.Values
            .OrderBy(static descriptor => descriptor.ClassName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static descriptor => descriptor.Namespace, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static (IReadOnlyList<CodeOnlyPropertyDescriptor> Properties,
                    IReadOnlyList<CodeOnlyEventDescriptor> Events)
        ExtractPublicSurface(string content, string className)
    {
        try
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(content, ParseOptions);
            var root = syntaxTree.GetRoot();

            var classDecl = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.ValueText == className);

            if (classDecl is null)
                return ([], []);

            var properties = classDecl.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => HasPublicModifier(p) && HasSetter(p))
                .Select(p => p.Identifier.ValueText)
                .Where(name => !BwfcInheritedPropertyNames.Contains(name))
                .Select(name =>
                {
                    var prop = classDecl.Members
                        .OfType<PropertyDeclarationSyntax>()
                        .First(p => p.Identifier.ValueText == name);
                    return new CodeOnlyPropertyDescriptor(name, prop.Type.ToString());
                })
                .ToList();

            var events = classDecl.Members
                .SelectMany(static member => member switch
                {
                    EventFieldDeclarationSyntax ef when HasPublicModifier(ef) =>
                        ef.Declaration.Variables.Select(v =>
                            new CodeOnlyEventDescriptor(v.Identifier.ValueText, ef.Declaration.Type.ToString())),
                    EventDeclarationSyntax ed when HasPublicModifier(ed) =>
                        (IEnumerable<CodeOnlyEventDescriptor>)[new CodeOnlyEventDescriptor(ed.Identifier.ValueText, ed.Type.ToString())],
                    _ => []
                })
                .ToList();

            return (properties, events);
        }
        catch
        {
            return ([], []);
        }
    }

    private static bool IsExcludedCodeFile(string filePath) =>
        filePath.EndsWith(".designer.cs", StringComparison.OrdinalIgnoreCase)
        || filePath.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
        || filePath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
        || filePath.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase);

    private static bool HasMarkupCompanion(string filePath)
    {
        var candidate = Path.ChangeExtension(filePath, null);
        return File.Exists(candidate + ".aspx")
               || File.Exists(candidate + ".ascx")
               || File.Exists(candidate + ".master");
    }

    private static string? ReadNamespace(string content)
    {
        var match = NamespaceRegex.Match(content);
        return match.Success ? match.Groups["namespace"].Value : null;
    }

    private static string NormalizeBaseType(string baseType)
    {
        var trimmed = baseType.Trim();
        var genericIndex = trimmed.IndexOf('<');
        if (genericIndex >= 0)
            trimmed = trimmed[..genericIndex];

        var qualifiedName = trimmed.Split(',')[0].Trim();
        if (qualifiedName.StartsWith("global::", StringComparison.Ordinal))
            qualifiedName = qualifiedName["global::".Length..];

        var lastDotIndex = qualifiedName.LastIndexOf('.');
        return lastDotIndex >= 0 ? qualifiedName[(lastDotIndex + 1)..] : qualifiedName;
    }

    private static bool IsServerControlBase(string baseType) =>
        SupportedBaseTypes.Contains(baseType);

    private static bool HasPublicModifier(MemberDeclarationSyntax member) =>
        member.Modifiers.Any(SyntaxKind.PublicKeyword);

    private static bool HasSetter(PropertyDeclarationSyntax property) =>
        property.AccessorList?.Accessors.Any(static a =>
            a.IsKind(SyntaxKind.SetAccessorDeclaration)
            || a.IsKind(SyntaxKind.InitAccessorDeclaration)) == true;
}

public sealed class CodeOnlyServerControlDescriptor
{
    public string ClassName { get; init; } = string.Empty;
    public string Namespace { get; init; } = string.Empty;
    public string BaseType { get; init; } = string.Empty;
    public string SourceFilePath { get; init; } = string.Empty;
    public IReadOnlyList<string> TagPrefixes { get; init; } = [];
    public IReadOnlyList<CodeOnlyPropertyDescriptor> Properties { get; init; } = [];
    public IReadOnlyList<CodeOnlyEventDescriptor> Events { get; init; } = [];
}

public sealed record CodeOnlyPropertyDescriptor(string Name, string TypeName);
public sealed record CodeOnlyEventDescriptor(string Name, string DelegateType);
