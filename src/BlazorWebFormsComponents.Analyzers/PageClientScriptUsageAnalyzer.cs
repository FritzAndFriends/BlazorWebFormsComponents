using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Page.ClientScript usage patterns.
    /// Page.ClientScript is not available in Blazor; use IJSRuntime instead.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageClientScriptUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC022";

        private static readonly LocalizableString Title = "Page.ClientScript usage detected";
        private static readonly LocalizableString MessageFormat = "Page.ClientScript is not available in Blazor. Use IJSRuntime for JavaScript interop.";
        private static readonly LocalizableString Description = "Page.ClientScript methods like RegisterStartupScript and GetPostBackEventReference are not available in Blazor. Use IJSRuntime for JavaScript interop.";
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
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            if (!IsClientScriptAccess(memberAccess))
                return;

            // Report at the Page.ClientScript location
            var diagnostic = Diagnostic.Create(Rule, memberAccess.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsClientScriptAccess(MemberAccessExpressionSyntax memberAccess)
        {
            // We're looking for patterns where "ClientScript" is accessed on "Page"
            // e.g., Page.ClientScript.RegisterStartupScript(...)

            if (memberAccess.Name.Identifier.Text != "ClientScript")
                return false;

            // Check that the expression is "Page" or "this.Page"
            if (memberAccess.Expression is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "Page";

            if (memberAccess.Expression is MemberAccessExpressionSyntax innerMember)
                return innerMember.Name.Identifier.Text == "Page" &&
                       innerMember.Expression is ThisExpressionSyntax;

            return false;
        }
    }
}
