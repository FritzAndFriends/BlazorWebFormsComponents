using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Render methods in WebControl/CompositeControl derived classes
    /// that don't call AddBaseAttributes() before RenderBeginTag().
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingAddBaseAttributesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC002";

        private static readonly LocalizableString Title = "Render method should call AddBaseAttributes()";
        private static readonly LocalizableString MessageFormat = "Render method should call AddBaseAttributes(writer) before the first RenderBeginTag() to include base styling properties";
        private static readonly LocalizableString Description = "When migrating Web Forms custom controls to Blazor, call AddBaseAttributes() before RenderBeginTag() to ensure ID, CssClass, and Style properties are applied.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            // Only analyze methods named "Render" or "RenderContents"
            var methodName = methodDeclaration.Identifier.Text;
            if (methodName != "Render" && methodName != "RenderContents")
                return;

            // Check if the method is protected and overridden
            var isProtected = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword));
            var isOverride = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword));
            if (!isProtected || !isOverride)
                return;

            // Check if the containing class derives from WebControl or CompositeControl
            var classDeclaration = methodDeclaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null)
                return;

            var semanticModel = context.SemanticModel;
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null || !DerivesFromWebControl(classSymbol))
                return;

            // Check if method body exists
            if (methodDeclaration.Body == null)
                return;

            // Check if the method calls AddBaseAttributes
            var callsAddBaseAttributes = methodDeclaration.Body.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Any(inv => inv.Expression.ToString().Contains("AddBaseAttributes"));

            // Check if the method calls RenderBeginTag
            var callsRenderBeginTag = methodDeclaration.Body.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Any(inv => inv.Expression.ToString().Contains("RenderBeginTag"));

            // If it calls RenderBeginTag but not AddBaseAttributes, report diagnostic
            if (callsRenderBeginTag && !callsAddBaseAttributes)
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool DerivesFromWebControl(INamedTypeSymbol classSymbol)
        {
            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                var baseTypeName = baseType.Name;
                if (baseTypeName == "WebControl" || baseTypeName == "CompositeControl")
                {
                    // Check namespace to ensure it's our WebControl/CompositeControl
                    var ns = baseType.ContainingNamespace?.ToDisplayString();
                    if (ns != null && ns.Contains("BlazorWebFormsComponents"))
                        return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}
