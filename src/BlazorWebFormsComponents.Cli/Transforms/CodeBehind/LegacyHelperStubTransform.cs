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

    // Matches System.Web usage for heavy APIs that HttpContextAccessorTransform cannot handle
    private static readonly Regex SystemWebHeavyUsageRegex = new(
        @"\busing\s+System\.Web\b.*;\s*$",
        RegexOptions.Compiled | RegexOptions.Multiline);

    // APIs that HttpContextAccessorTransform CAN handle — these should NOT trigger stubbing alone
    private static readonly Regex HttpContextTransformableRegex = new(
        @"\bHttpContext\.Current\b",
        RegexOptions.Compiled);

    // APIs that are truly untransformable and require stubbing
    private static readonly Regex HeavyWebApiRegex = new(
        @"\bHttpRuntime\b|\bHttpServerUtility\b|\bHttpApplication\b",
        RegexOptions.Compiled);

    private static readonly Regex NamespaceRegex = new(
        @"namespace\s+(?<ns>[A-Za-z_][\w.]*)\s*(?:\{|;)",
        RegexOptions.Compiled);

    private static readonly Regex ClassDeclRegex = new(
        @"(?:public|internal)\s+(?<static>static\s+)?(?:(?:sealed|abstract|partial)\s+)*class\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)(?:\s*:\s*(?<bases>[^{\r\n]+))?",
        RegexOptions.Compiled);

    // Matches public/protected method signatures: [public|protected] [static] ReturnType MethodName(params)
    private static readonly Regex MethodRegex = new(
        @"(?<access>public|protected)\s+(?<mods>(?:(?:static|override|virtual|async)\s+)*)(?<returnType>[\w<>\[\]?,\s]+?)\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^)]*)\)\s*\{",
        RegexOptions.Compiled);

    // Matches public properties: public Type Name { get; set; }
    private static readonly Regex PropertyRegex = new(
        @"public\s+(?:static\s+)?(?:override\s+)?(?:virtual\s+)?(?<type>[\w<>\[\]?,\s]+?)\s+(?<name>[A-Za-z_]\w*)\s*\{\s*(?:get|set)",
        RegexOptions.Compiled);

    // Matches public constants: public const Type Name = value;
    private static readonly Regex ConstRegex = new(
        @"public\s+const\s+(?<type>\S+)\s+(?<name>\w+)\s*=\s*(?<value>[^;]+);",
        RegexOptions.Compiled);

    // Matches nested type declarations: public [sealed/static/abstract/partial] struct/class/enum Name { ... }
    private static readonly Regex NestedTypeRegex = new(
        @"public\s+(?:(?:sealed|static|abstract|partial)\s+)*(?<kind>struct|class|enum)\s+(?<name>[A-Za-z_]\w*)(?:\s*:\s*[^{\r\n]+)?\s*\{",
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
        var sourcePath = metadata.SourceFilePath ?? string.Empty;
        var outputPath = metadata.OutputFilePath ?? string.Empty;
        if (metadata.FileType == FileType.Page
            || metadata.FileType == FileType.Master
            || metadata.FileType == FileType.Control
            || sourcePath.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase)
            || outputPath.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
            || outputPath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || outputPath.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase)
            || outputPath.EndsWith(".razor.cs", StringComparison.OrdinalIgnoreCase))
            return content;

        // Check for legacy namespace imports that have no automated transform
        if (LegacyNamespaceRegex.IsMatch(content))
            return BuildApiAwareStub(content, metadata);

        // Check for System.Web usage — only stub if there are truly untransformable APIs
        // HttpContext.Current alone is handled by HttpContextAccessorTransform (order 108)
        if (SystemWebHeavyUsageRegex.IsMatch(content))
        {
            if (HeavyWebApiRegex.IsMatch(content))
                return BuildApiAwareStub(content, metadata);
            // System.Web + only HttpContext.Current → let HttpContextAccessorTransform handle it
        }

        return content;
    }

    internal static string BuildApiAwareStub(string content, FileMetadata metadata)
    {
        var nsMatch = NamespaceRegex.Match(content);
        var ns = nsMatch.Success ? nsMatch.Groups["ns"].Value : null;

        // Find all top-level class declarations
        var classMatches = ClassDeclRegex.Matches(content);
        if (classMatches.Count == 0)
        {
            // Fallback: generate a single stub with file name
            return BuildSingleClassStub(content, ns,
                Path.GetFileNameWithoutExtension(metadata.OutputFilePath),
                isStatic: false, interfaces: new List<string>());
        }

        if (classMatches.Count == 1)
        {
            var classMatch = classMatches[0];
            var className = classMatch.Groups["name"].Value;
            var isStatic = classMatch.Groups["static"].Success;
            var interfaces = ExtractInterfaces(classMatch);
            return BuildSingleClassStub(content, ns, className, isStatic, interfaces);
        }

        // Multiple top-level classes: scope each class to its own body
        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated API-compatible stub. Original referenced legacy Web Forms APIs.");
        sb.AppendLine("// TODO(bwfc-general): Rebuild method bodies using ASP.NET Core equivalents.");

        if (!string.IsNullOrWhiteSpace(ns))
        {
            sb.AppendLine();
            sb.AppendLine($"namespace {ns};");
        }

        var allClassNames = classMatches.Select(m => m.Groups["name"].Value).ToHashSet(StringComparer.Ordinal);

        for (var i = 0; i < classMatches.Count; i++)
        {
            var classMatch = classMatches[i];
            var className = classMatch.Groups["name"].Value;
            var isStatic = classMatch.Groups["static"].Success;
            var interfaces = ExtractInterfaces(classMatch);

            // Find the class body: from opening brace to matching close
            var braceStart = content.IndexOf('{', classMatch.Index + classMatch.Length - 1);
            if (braceStart < 0) continue;
            var braceEnd = FindMatchingBrace(content, braceStart);
            if (braceEnd < 0) continue;

            var classBody = content[braceStart..(braceEnd + 1)];

            var methods = ExtractMethods(classBody, className);
            var properties = ExtractProperties(classBody);
            var constants = ExtractConstants(classBody);
            var nestedTypes = ExtractNestedTypes(classBody)
                .Where(nt => !allClassNames.Contains(nt.Name))
                .ToList();

            sb.AppendLine();
            var staticMod = isStatic ? "static " : "";
            var interfaceList = interfaces.Count > 0 ? $" : {string.Join(", ", interfaces)}" : "";
            sb.AppendLine($"public {staticMod}partial class {className}{interfaceList}");
            sb.AppendLine("{");

            foreach (var c in constants)
                sb.AppendLine($"    public const {c.Type} {c.Name} = {c.Value};");
            if (constants.Count > 0) sb.AppendLine();

            foreach (var p in properties)
                sb.AppendLine($"    public {p.Type} {p.Name} {{ get; set; }}");
            if (properties.Count > 0) sb.AppendLine();

            foreach (var m in methods)
            {
                var modsPrefix = string.IsNullOrEmpty(m.Modifiers) ? "" : $"{m.Modifiers} ";
                var body = GetMethodBody(m, interfaces.Contains("IDisposable"));
                sb.AppendLine($"    {m.AccessModifier} {modsPrefix}{m.ReturnType} {m.Name}({m.Parameters}) {body}");
            }

            foreach (var nt in nestedTypes)
            {
                sb.AppendLine();
                sb.AppendLine($"    public {nt.Kind} {nt.Name}");
                sb.AppendLine("    {");
                foreach (var f in nt.Fields)
                    sb.AppendLine($"        public {f.Type} {f.Name};");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private static string BuildSingleClassStub(string content, string? ns,
        string className, bool isStatic, List<string> interfaces)
    {
        var methods = ExtractMethods(content, className);
        var properties = ExtractProperties(content);
        var constants = ExtractConstants(content);
        var nestedTypes = ExtractNestedTypes(content)
            .Where(nt => nt.Name != className)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("// Auto-generated API-compatible stub. Original referenced legacy Web Forms APIs.");
        sb.AppendLine("// TODO(bwfc-general): Rebuild method bodies using ASP.NET Core equivalents.");

        if (!string.IsNullOrWhiteSpace(ns))
        {
            sb.AppendLine();
            sb.AppendLine($"namespace {ns};");
        }

        sb.AppendLine();
        var staticMod = isStatic ? "static " : "";
        var interfaceList = interfaces.Count > 0 ? $" : {string.Join(", ", interfaces)}" : "";
        sb.AppendLine($"public {staticMod}partial class {className}{interfaceList}");
        sb.AppendLine("{");

        foreach (var c in constants)
            sb.AppendLine($"    public const {c.Type} {c.Name} = {c.Value};");
        if (constants.Count > 0) sb.AppendLine();

        foreach (var p in properties)
            sb.AppendLine($"    public {p.Type} {p.Name} {{ get; set; }}");
        if (properties.Count > 0) sb.AppendLine();

        foreach (var m in methods)
        {
            var modsPrefix = string.IsNullOrEmpty(m.Modifiers) ? "" : $"{m.Modifiers} ";
            var body = GetMethodBody(m, interfaces.Contains("IDisposable"));
            sb.AppendLine($"    {m.AccessModifier} {modsPrefix}{m.ReturnType} {m.Name}({m.Parameters}) {body}");
        }

        foreach (var nt in nestedTypes)
        {
            sb.AppendLine();
            sb.AppendLine($"    public {nt.Kind} {nt.Name}");
            sb.AppendLine("    {");
            foreach (var f in nt.Fields)
                sb.AppendLine($"        public {f.Type} {f.Name};");
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

            var accessModifier = m.Groups["access"].Value;
            methods.Add(new MethodInfo(accessModifier, mods, returnType, name, parameters));
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

    private sealed record MethodInfo(string AccessModifier, string Modifiers, string ReturnType, string Name, string Parameters);
    private sealed record PropertyInfo(string Type, string Name);
    private sealed record ConstInfo(string Type, string Name, string Value);
    private sealed record FieldInfo(string Type, string Name);
    private sealed record NestedTypeInfo(string Kind, string Name, List<FieldInfo> Fields);
}
