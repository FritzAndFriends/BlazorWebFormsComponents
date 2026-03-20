using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects IsPostBack and Page.IsPostBack usage patterns.
    /// Blazor uses component lifecycle instead of the postback model.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IsPostBackUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC003";

        private static readonly LocalizableString Title = "IsPostBack usage detected";
        private static readonly LocalizableString MessageFormat = "'{0}' checks IsPostBack \u2014 Blazor doesn't have postbacks. Use lifecycle methods (OnInitialized, OnParametersSet) instead.";
        private static readonly LocalizableString Description = "Flags IsPostBack and Page.IsPostBack checks. Blazor uses component lifecycle instead of postback model.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeIdentifierName, SyntaxKind.IdentifierName);
        }

        private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierName = (IdentifierNameSyntax)context.Node;

            if (identifierName.Identifier.Text != "IsPostBack")
                return;

            // If this is the Name part of a MemberAccessExpression (e.g., Page.IsPostBack),
            // report at the parent MemberAccess location to cover the full expression.
            if (identifierName.Parent is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name == identifierName)
            {
                var containingName = GetContainingMemberName(identifierName);
                var diagnostic = Diagnostic.Create(Rule, memberAccess.GetLocation(), containingName);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // If this identifier IS the Expression of a MemberAccess (IsPostBack.Something),
            // skip — that's not a postback check pattern.
            if (identifierName.Parent is MemberAccessExpressionSyntax parentAccess &&
                parentAccess.Expression == identifierName)
            {
                return;
            }

            // Standalone IsPostBack reference
            var name = GetContainingMemberName(identifierName);
            var diag = Diagnostic.Create(Rule, identifierName.GetLocation(), name);
            context.ReportDiagnostic(diag);
        }

        private static string GetContainingMemberName(SyntaxNode node)
        {
            var method = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (method != null)
                return method.Identifier.Text;

            var classDecl = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDecl != null)
                return classDecl.Identifier.Text;

            return "Unknown";
        }
    }
}
