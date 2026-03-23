using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Session state and HttpContext.Current usage,
    /// which are incompatible with Blazor's architecture.
    /// Blazor Server uses scoped services or ProtectedSessionStorage instead.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SessionUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC005";

        private static readonly LocalizableString Title = "Session state usage detected";
        private static readonly LocalizableString MessageFormat = "'{0}' uses Session state — Blazor Server uses scoped services or ProtectedSessionStorage instead";
        private static readonly LocalizableString Description = "Session state and HttpContext.Current are not available in Blazor. Use scoped services or ProtectedSessionStorage.";
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
            context.RegisterSyntaxNodeAction(AnalyzeElementAccess, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            var expressionText = elementAccess.Expression.ToString();

            // Match Session["key"], this.Session["key"]
            if (expressionText == "Session" || expressionText == "this.Session")
            {
                var containingMethod = elementAccess.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                var memberName = containingMethod?.Identifier.Text ?? "<unknown>";

                var diagnostic = Diagnostic.Create(Rule, elementAccess.GetLocation(), memberName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            // Match HttpContext.Current
            if (memberAccess.Name.Identifier.Text == "Current" &&
                memberAccess.Expression.ToString() == "HttpContext")
            {
                // Avoid double-reporting if this is part of a larger expression
                // that we'll catch elsewhere (e.g., HttpContext.Current.Session["key"])
                // Only report if this node is not already the expression of another
                // member access that we'd also flag.
                var parent = memberAccess.Parent;
                if (parent is MemberAccessExpressionSyntax parentMember)
                {
                    // If parent is HttpContext.Current.Session, and that's followed by element access,
                    // we'll catch the Session["key"] separately. But HttpContext.Current itself is the problem.
                    // Report it here regardless — the user needs to know HttpContext.Current is unavailable.
                }

                var containingMethod = memberAccess.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                var memberName = containingMethod?.Identifier.Text ?? "<unknown>";

                var diagnostic = Diagnostic.Create(Rule, memberAccess.GetLocation(), memberName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
