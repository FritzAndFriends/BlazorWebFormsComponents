using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorWebFormsComponents.Cli.Analysis;

public sealed class AscxDescriptorAnalyzer
{
    private static readonly Regex InheritsDirectiveRegex = new(
        @"<%@\s*Control\b[^>]*\bInherits\s*=\s*""(?<inherits>[^""]+)""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex CodeBehindDirectiveRegex = new(
        @"<%@\s*Control\b[^>]*\bCode(?:Behind|File)\s*=\s*""(?<codeBehind>[^""]+)""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest);

    public AscxDescriptor Analyze(string ascxCodeBehindPath)
    {
        var markupPath = TryResolveMarkupPath(ascxCodeBehindPath);
        return AnalyzeInternal(markupPath, ascxCodeBehindPath, GetControlName(markupPath ?? ascxCodeBehindPath));
    }

    public AscxDescriptor AnalyzeControl(string ascxMarkupPath, string? codeBehindPath = null)
    {
        codeBehindPath ??= ResolveCodeBehindPath(ascxMarkupPath);
        return AnalyzeInternal(ascxMarkupPath, codeBehindPath, GetControlName(ascxMarkupPath));
    }

    private static AscxDescriptor AnalyzeInternal(string? markupPath, string? codeBehindPath, string controlName)
    {
        var diagnostics = new List<string>();
        var inheritsTypeName = ReadInheritsTypeName(markupPath, diagnostics);
        var codeBehindExists = !string.IsNullOrWhiteSpace(codeBehindPath) && File.Exists(codeBehindPath);

        if (!codeBehindExists)
        {
            if (!string.IsNullOrWhiteSpace(codeBehindPath))
            {
                diagnostics.Add($"Code-behind file not found: {codeBehindPath}");
            }

            return new AscxDescriptor
            {
                ControlName = controlName,
                MarkupPath = markupPath,
                CodeBehindPath = codeBehindPath,
                InheritsTypeName = inheritsTypeName,
                CodeBehindExists = false,
                ParseSucceeded = false,
                Diagnostics = diagnostics
            };
        }

        if (!string.Equals(Path.GetExtension(codeBehindPath), ".cs", StringComparison.OrdinalIgnoreCase))
        {
            diagnostics.Add($"Unsupported code-behind language: {Path.GetExtension(codeBehindPath)}");
            return new AscxDescriptor
            {
                ControlName = controlName,
                MarkupPath = markupPath,
                CodeBehindPath = codeBehindPath,
                InheritsTypeName = inheritsTypeName,
                CodeBehindExists = true,
                ParseSucceeded = false,
                Diagnostics = diagnostics
            };
        }

        string code;
        try
        {
            code = File.ReadAllText(codeBehindPath!);
        }
        catch (Exception ex)
        {
            diagnostics.Add($"Unable to read code-behind: {ex.Message}");
            return new AscxDescriptor
            {
                ControlName = controlName,
                MarkupPath = markupPath,
                CodeBehindPath = codeBehindPath,
                InheritsTypeName = inheritsTypeName,
                CodeBehindExists = true,
                ParseSucceeded = false,
                Diagnostics = diagnostics
            };
        }

        var syntaxTree = CSharpSyntaxTree.ParseText(code, ParseOptions, codeBehindPath ?? string.Empty);
        var root = syntaxTree.GetRoot();
        diagnostics.AddRange(syntaxTree.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .Select(static diagnostic => diagnostic.ToString()));

        var controlClass = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault(static candidate => candidate.Modifiers.Any(SyntaxKind.PublicKeyword))
            ?? root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        if (controlClass is null)
        {
            diagnostics.Add("No class declaration found in ASCX code-behind.");
            return new AscxDescriptor
            {
                ControlName = controlName,
                MarkupPath = markupPath,
                CodeBehindPath = codeBehindPath,
                InheritsTypeName = inheritsTypeName,
                CodeBehindExists = true,
                ParseSucceeded = diagnostics.Count == 0,
                Diagnostics = diagnostics
            };
        }

        var properties = controlClass.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(HasPublicModifier)
            .Select(static property => new AscxPropertyDescriptor(
                property.Identifier.ValueText,
                property.Type.ToString(),
                HasGetter(property),
                HasSetter(property),
                property.Initializer?.Value.ToString()))
            .ToList();

        var events = controlClass.Members
            .SelectMany(static member => member switch
            {
                EventFieldDeclarationSyntax eventField when HasPublicModifier(eventField) =>
                    eventField.Declaration.Variables.Select(variable =>
                        new AscxEventDescriptor(variable.Identifier.ValueText, eventField.Declaration.Type.ToString())),
                EventDeclarationSyntax eventDeclaration when HasPublicModifier(eventDeclaration) =>
                    [new AscxEventDescriptor(eventDeclaration.Identifier.ValueText, eventDeclaration.Type.ToString())],
                _ => []
            })
            .ToList();

        var methods = controlClass.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(HasPublicModifier)
            .Select(static method => new AscxMethodDescriptor(
                method.Identifier.ValueText,
                method.ReturnType.ToString(),
                method.ParameterList.Parameters
                    .Select(parameter => new AscxMethodParameterDescriptor(
                        parameter.Identifier.ValueText,
                        parameter.Type?.ToString() ?? "object"))
                    .ToList()))
            .ToList();

        var referencedControlIds = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(GetFindControlIdentifier)
            .Where(static controlId => !string.IsNullOrWhiteSpace(controlId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Cast<string>()
            .ToList();

        var hasDataBindCall = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Any(static invocation => string.Equals(GetInvocationName(invocation), "DataBind", StringComparison.Ordinal));

        var hasPageLoadOverride = controlClass.Members
            .OfType<MethodDeclarationSyntax>()
            .Any(static method =>
                string.Equals(method.Identifier.ValueText, "Page_Load", StringComparison.Ordinal)
                || (string.Equals(method.Identifier.ValueText, "OnLoad", StringComparison.Ordinal)
                    && method.Modifiers.Any(SyntaxKind.OverrideKeyword)));

        return new AscxDescriptor
        {
            ControlName = controlName,
            MarkupPath = markupPath,
            CodeBehindPath = codeBehindPath,
            InheritsTypeName = inheritsTypeName,
            ClassName = controlClass.Identifier.ValueText,
            BaseTypeName = controlClass.BaseList?.Types.FirstOrDefault()?.Type.ToString(),
            CodeBehindExists = true,
            ParseSucceeded = diagnostics.Count == 0,
            HasDataBindCall = hasDataBindCall,
            HasPageLoadOverride = hasPageLoadOverride,
            Properties = properties,
            Events = events,
            Methods = methods,
            ReferencedControlIds = referencedControlIds,
            Diagnostics = diagnostics
        };
    }

    private static string? ResolveCodeBehindPath(string ascxMarkupPath)
    {
        var directiveCodeBehindPath = ReadCodeBehindPath(ascxMarkupPath);
        if (!string.IsNullOrWhiteSpace(directiveCodeBehindPath))
        {
            var resolvedDirectivePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ascxMarkupPath)!, directiveCodeBehindPath));
            if (File.Exists(resolvedDirectivePath))
            {
                return resolvedDirectivePath;
            }
        }

        var csharpCodeBehind = ascxMarkupPath + ".cs";
        if (File.Exists(csharpCodeBehind))
        {
            return csharpCodeBehind;
        }

        var vbCodeBehind = ascxMarkupPath + ".vb";
        return File.Exists(vbCodeBehind) ? vbCodeBehind : csharpCodeBehind;
    }

    private static string? TryResolveMarkupPath(string ascxCodeBehindPath)
    {
        if (ascxCodeBehindPath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || ascxCodeBehindPath.EndsWith(".ascx.vb", StringComparison.OrdinalIgnoreCase))
        {
            return Path.ChangeExtension(ascxCodeBehindPath, null);
        }

        return null;
    }

    private static string GetControlName(string path)
    {
        var fileName = Path.GetFileName(path);
        if (fileName.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".ascx.vb", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
        }

        return Path.GetFileNameWithoutExtension(path);
    }

    private static string? ReadCodeBehindPath(string ascxMarkupPath)
    {
        if (!File.Exists(ascxMarkupPath))
        {
            return null;
        }

        try
        {
            var markup = File.ReadAllText(ascxMarkupPath);
            var match = CodeBehindDirectiveRegex.Match(markup);
            return match.Success ? match.Groups["codeBehind"].Value : null;
        }
        catch
        {
            return null;
        }
    }

    private static string? ReadInheritsTypeName(string? markupPath, ICollection<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(markupPath) || !File.Exists(markupPath))
        {
            return null;
        }

        try
        {
            var markup = File.ReadAllText(markupPath);
            var match = InheritsDirectiveRegex.Match(markup);
            return match.Success ? match.Groups["inherits"].Value : null;
        }
        catch (Exception ex)
        {
            diagnostics.Add($"Unable to read ASCX markup: {ex.Message}");
            return null;
        }
    }

    private static bool HasPublicModifier(MemberDeclarationSyntax member) =>
        member.Modifiers.Any(SyntaxKind.PublicKeyword);

    private static bool HasGetter(PropertyDeclarationSyntax property) =>
        property.ExpressionBody is not null
        || property.AccessorList?.Accessors.Any(static accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) == true;

    private static bool HasSetter(PropertyDeclarationSyntax property) =>
        property.AccessorList?.Accessors.Any(static accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration)) == true
        || property.AccessorList?.Accessors.Any(static accessor => accessor.IsKind(SyntaxKind.InitAccessorDeclaration)) == true;

    private static string? GetFindControlIdentifier(InvocationExpressionSyntax invocation)
    {
        if (!string.Equals(GetInvocationName(invocation), "FindControl", StringComparison.Ordinal))
        {
            return null;
        }

        return invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression switch
        {
            LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression) => literal.Token.ValueText,
            _ => null
        };
    }

    private static string? GetInvocationName(InvocationExpressionSyntax invocation) =>
        invocation.Expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            MemberBindingExpressionSyntax memberBinding => memberBinding.Name.Identifier.ValueText,
            _ => null
        };
}
