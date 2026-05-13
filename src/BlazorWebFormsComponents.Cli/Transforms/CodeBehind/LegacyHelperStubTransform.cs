using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Stubs non-page helper/utility classes that reference legacy Web Forms namespaces
/// which are not available in Blazor (e.g., System.Web, Microsoft.AspNet.Identity).
/// Extracts the public API surface (methods, properties, constants, interfaces, nested types)
/// so that callers still compile against the stub.
/// </summary>
public class LegacyHelperStubTransform : ICodeBehindTransform
{
    private static readonly Regex LegacyNamespaceRegex = new(
        @"\busing\s+(Microsoft\.AspNet\.Identity|Microsoft\.AspNet\.Identity\.EntityFramework|Microsoft\.AspNet\.Identity\.Owin|System\.Web\.Security|System\.Web\.Providers|System\.Web\.Configuration|System\.Web\.Profile|WebMatrix\.WebData|System\.Configuration)\b",
        RegexOptions.Compiled);

    // Matches System.Web usage for heavy APIs (HttpContext.Current, Session, etc.)
    // that can't be rewritten with simple using swaps
    private static readonly Regex SystemWebHeavyUsageRegex = new(
        @"\busing\s+System\.Web\b.*;\s*$",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex HeavyWebApiRegex = new(
        @"\bHttpContext\.Current\b|\bHttpRuntime\b|\bHttpServerUtility\b|\bHttpApplication\b",
        RegexOptions.Compiled);

    private static readonly Regex NamespaceRegex = new(
        @"namespace\s+(?<ns>[A-Za-z_][\w.]*)\s*(?:\{|;)",
        RegexOptions.Compiled);

    private static readonly Regex ClassDeclRegex = new(
        @"(?:public|internal)\s+(?<static>static\s+)?class\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)(?:\s*:\s*(?<bases>[^{\r\n]+))?",
        RegexOptions.Compiled);

    // Matches public method signatures: public [static] ReturnType MethodName(params)
    private static readonly Regex MethodRegex = new(
        @"public\s+(?<mods>(?:(?:static|override|virtual|async)\s+)*)(?<returnType>[\w<>\[\]?,\s]+?)\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^)]*)\)\s*\{",
        RegexOptions.Compiled);

    // Matches public properties: public Type Name { get; set; }
    private static readonly Regex PropertyRegex = new(
        @"public\s+(?:static\s+)?(?:override\s+)?(?:virtual\s+)?(?<type>[\w<>\[\]?,\s]+?)\s+(?<name>[A-Za-z_]\w*)\s*\{\s*(?:get|set)",
        RegexOptions.Compiled);

    // Matches public constants: public const Type Name = value;
    private static readonly Regex ConstRegex = new(
        @"public\s+const\s+(?<type>\S+)\s+(?<name>\w+)\s*=\s*(?<value>[^;]+);",
        RegexOptions.Compiled);

    // Matches nested type declarations: public struct/class/enum Name { ... }
    private static readonly Regex NestedTypeRegex = new(
        @"public\s+(?<kind>struct|class|enum)\s+(?<name>[A-Za-z_]\w*)\s*\{",
        RegexOptions.Compiled);

    // Matches fields inside nested types: public Type Name;
    private static readonly Regex FieldRegex = new(
        @"public\s+(?<type>\S+)\s+(?<name>\w+)\s*;",
        RegexOptions.Compiled);

    public string Name => "LegacyHelperStub";
    public int Order => 50; // Run before UsingStripTransform (100) so legacy using directives are still present

    public string Apply(string content, FileMetadata metadata)
    {
        // Only process standalone .cs files (not page/master/control code-behinds)
        var sourcePath = metadata.SourceFilePath ?? metadata.OutputFilePath;
        if (sourcePath.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase))
            return content;

        if (!LegacyNamespaceRegex.IsMatch(content))
        {
            // Check for System.Web + heavy API usage (HttpContext.Current, etc.)
            if (!(SystemWebHeavyUsageRegex.IsMatch(content) && HeavyWebApiRegex.IsMatch(content)))
                return content;
        }

        return BuildApiAwareStub(content, metadata);
    }

    internal static string BuildApiAwareStub(string content, FileMetadata metadata)
    {
        var nsMatch = NamespaceRegex.Match(content);
        var ns = nsMatch.Success ? nsMatch.Groups["ns"].Value : null;

        var classMatch = ClassDeclRegex.Match(content);
        var className = classMatch.Success
            ? classMatch.Groups["name"].Value
            : Path.GetFileNameWithoutExtension(metadata.OutputFilePath);
        var isStatic = classMatch.Success && classMatch.Groups["static"].Success;

        // Extract interfaces (filter out legacy base classes)
        var interfaces = ExtractInterfaces(classMatch);

        // Extract public API surface
        var methods = ExtractMethods(content, className);
        var properties = ExtractProperties(content);
        var constants = ExtractConstants(content);
        var nestedTypes = ExtractNestedTypes(content);

        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated API-compatible stub. Original referenced legacy Web Forms APIs.");
        sb.AppendLine("// TODO(bwfc-general): Rebuild method bodies using ASP.NET Core equivalents.");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(ns))
        {
            sb.AppendLine($"namespace {ns};");
            sb.AppendLine();
        }

        var staticMod = isStatic ? "static " : "";
        var interfaceList = interfaces.Count > 0 ? $" : {string.Join(", ", interfaces)}" : "";
        sb.AppendLine($"public {staticMod}partial class {className}{interfaceList}");
        sb.AppendLine("{");

        // Constants
        foreach (var c in constants)
        {
            sb.AppendLine($"    public const {c.Type} {c.Name} = {c.Value};");
        }

        if (constants.Count > 0) sb.AppendLine();

        // Properties
        foreach (var p in properties)
        {
            sb.AppendLine($"    public {p.Type} {p.Name} {{ get; set; }}");
        }

        if (properties.Count > 0) sb.AppendLine();

        // Methods
        foreach (var m in methods)
        {
            var modsPrefix = string.IsNullOrEmpty(m.Modifiers) ? "" : $"{m.Modifiers} ";
            var body = GetMethodBody(m, interfaces.Contains("IDisposable"));
            sb.AppendLine($"    public {modsPrefix}{m.ReturnType} {m.Name}({m.Parameters}) {body}");
        }

        // Nested types
        foreach (var nt in nestedTypes)
        {
            sb.AppendLine();
            sb.AppendLine($"    public {nt.Kind} {nt.Name}");
            sb.AppendLine("    {");
            foreach (var f in nt.Fields)
            {
                sb.AppendLine($"        public {f.Type} {f.Name};");
            }
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static List<string> ExtractInterfaces(Match classMatch)
    {
        var interfaces = new List<string>();
        if (!classMatch.Success || !classMatch.Groups["bases"].Success)
            return interfaces;

        var bases = classMatch.Groups["bases"].Value;
        foreach (var part in bases.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var name = part.Trim();
            // Keep interfaces (IDisposable, etc.) and simple base types
            // Skip legacy Web Forms base classes
            if (name.StartsWith("I", StringComparison.Ordinal) && name.Length > 1 && char.IsUpper(name[1]))
            {
                interfaces.Add(name);
            }
        }
        return interfaces;
    }

    private static List<MethodInfo> ExtractMethods(string content, string className)
    {
        var methods = new List<MethodInfo>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in MethodRegex.Matches(content))
        {
            var name = m.Groups["name"].Value;
            var mods = m.Groups["mods"].Value.Trim();
            var returnType = m.Groups["returnType"].Value.Trim();
            var parameters = m.Groups["params"].Value.Trim();

            // Skip constructors and duplicates
            if (name == className) continue;

            // Clean parameters: remove attributes like [QueryString], [RouteData]
            parameters = Regex.Replace(parameters, @"\[[^\]]+\]\s*", "");

            // Replace System.Web types in parameters
            parameters = parameters
                .Replace("HttpContext", "Microsoft.AspNetCore.Http.HttpContext")
                .Replace("HttpRequest", "Microsoft.AspNetCore.Http.HttpRequest");

            var key = $"{name}({parameters})";
            if (!seen.Add(key)) continue;

            methods.Add(new MethodInfo(mods, returnType, name, parameters));
        }
        return methods;
    }

    private static List<PropertyInfo> ExtractProperties(string content)
    {
        var props = new List<PropertyInfo>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in PropertyRegex.Matches(content))
        {
            var name = m.Groups["name"].Value;
            var type = m.Groups["type"].Value.Trim();
            if (seen.Add(name))
            {
                props.Add(new PropertyInfo(type, name));
            }
        }
        return props;
    }

    private static List<ConstInfo> ExtractConstants(string content)
    {
        var consts = new List<ConstInfo>();
        foreach (Match m in ConstRegex.Matches(content))
        {
            consts.Add(new ConstInfo(
                m.Groups["type"].Value,
                m.Groups["name"].Value,
                m.Groups["value"].Value.Trim()));
        }
        return consts;
    }

    private static List<NestedTypeInfo> ExtractNestedTypes(string content)
    {
        var types = new List<NestedTypeInfo>();
        foreach (Match m in NestedTypeRegex.Matches(content))
        {
            var kind = m.Groups["kind"].Value;
            var name = m.Groups["name"].Value;
            var braceStart = m.Index + m.Length - 1;
            var braceEnd = FindMatchingBrace(content, braceStart);
            if (braceEnd < 0) continue;

            var body = content[(braceStart + 1)..braceEnd];
            var fields = new List<FieldInfo>();
            foreach (Match f in FieldRegex.Matches(body))
            {
                fields.Add(new FieldInfo(f.Groups["type"].Value, f.Groups["name"].Value));
            }
            types.Add(new NestedTypeInfo(kind, name, fields));
        }
        return types;
    }

    private static string GetMethodBody(MethodInfo method, bool hasDisposable)
    {
        // IDisposable.Dispose() gets an empty body
        if (method.Name == "Dispose" && string.IsNullOrEmpty(method.Parameters))
            return "{ }";

        var returnType = method.ReturnType;
        if (returnType == "void")
            return "{ }";

        // For value types, return default
        return $"=> throw new NotImplementedException();";
    }

    private static int FindMatchingBrace(string content, int openIndex)
    {
        var depth = 0;
        for (var i = openIndex; i < content.Length; i++)
        {
            if (content[i] == '{') depth++;
            else if (content[i] == '}')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    private sealed record MethodInfo(string Modifiers, string ReturnType, string Name, string Parameters);
    private sealed record PropertyInfo(string Type, string Name);
    private sealed record ConstInfo(string Type, string Name, string Value);
    private sealed record FieldInfo(string Type, string Name);
    private sealed record NestedTypeInfo(string Kind, string Name, List<FieldInfo> Fields);
}
