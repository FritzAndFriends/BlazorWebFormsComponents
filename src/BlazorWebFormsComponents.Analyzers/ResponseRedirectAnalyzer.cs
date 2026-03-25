using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Response.Redirect() usage, which is incompatible with Blazor.
    /// Blazor uses NavigationManager.NavigateTo() instead.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ResponseRedirectAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC004";

        private static readonly LocalizableString Title = "Response.Redirect usage detected";
        private static readonly LocalizableString MessageFormat = "'{0}' uses Response.Redirect — use NavigationManager.NavigateTo() in Blazor instead";
        private static readonly LocalizableString Description = "Response.Redirect() is not available in Blazor. Use NavigationManager.NavigateTo() for client-side navigation.";
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
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess && IsResponseRedirect(memberAccess))
            {
                var containingMethod = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                var memberName = containingMethod?.Identifier.Text ?? "<unknown>";

                var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), memberName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsResponseRedirect(MemberAccessExpressionSyntax memberAccess)
        {
            // Match: Response.Redirect(...)
            if (memberAccess.Name.Identifier.Text == "Redirect")
            {
                var expressionText = memberAccess.Expression.ToString();

                // Match Response.Redirect, this.Response.Redirect, HttpContext.Current.Response.Redirect
                if (expressionText == "Response" ||
                    expressionText == "this.Response" ||
                    expressionText.EndsWith(".Response") ||
                    expressionText == "HttpContext.Current.Response")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
