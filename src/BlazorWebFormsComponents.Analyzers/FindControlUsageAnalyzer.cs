using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects FindControl("id") calls.
    /// Blazor does not have FindControl; use FindControlRecursive or @ref instead.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FindControlUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC021";

        private static readonly LocalizableString Title = "FindControl usage detected";
        private static readonly LocalizableString MessageFormat = "FindControl is not available in Blazor. Use FindControlRecursive from BWFC or @ref for component references.";
        private static readonly LocalizableString Description = "FindControl is a Web Forms API not available in Blazor. Use FindControlRecursive (available on BaseWebFormsComponent) or @ref for component references.";
        private const string Category = "Migration";

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
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (!IsFindControlCall(invocation))
                return;

            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsFindControlCall(InvocationExpressionSyntax invocation)
        {
            // FindControl("id")
            if (invocation.Expression is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "FindControl";

            // this.FindControl("id") or someObject.FindControl("id")
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                return memberAccess.Name.Identifier.Text == "FindControl";

            return false;
        }
    }
}
