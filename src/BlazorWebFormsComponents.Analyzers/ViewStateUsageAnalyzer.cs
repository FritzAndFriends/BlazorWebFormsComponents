using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects ViewState["key"] usage patterns.
    /// ViewState works as a migration shim; suggests native Blazor state for new code.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ViewStateUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC002";

        private static readonly LocalizableString Title = "ViewState usage detected";
        private static readonly LocalizableString MessageFormat = "'{0}' uses ViewState \u2014 this works as a migration shim via ViewStateDictionary. For new code, prefer component fields or [Parameter] properties.";
        private static readonly LocalizableString Description = "Flags ViewState[\"key\"] patterns in code. ViewState works during migration via ViewStateDictionary; prefer native Blazor state for new code.";
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
            context.RegisterSyntaxNodeAction(AnalyzeElementAccess, SyntaxKind.ElementAccessExpression);
        }

        private static void AnalyzeElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            if (!IsViewStateAccess(elementAccess))
                return;

            var containingName = GetContainingMemberName(elementAccess);
            var diagnostic = Diagnostic.Create(Rule, elementAccess.GetLocation(), containingName);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsViewStateAccess(ElementAccessExpressionSyntax elementAccess)
        {
            var expr = elementAccess.Expression;

            // ViewState["key"]
            if (expr is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "ViewState";

            // this.ViewState["key"]
            if (expr is MemberAccessExpressionSyntax memberAccess)
                return memberAccess.Name.Identifier.Text == "ViewState" &&
                       memberAccess.Expression is ThisExpressionSyntax;

            return false;
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
