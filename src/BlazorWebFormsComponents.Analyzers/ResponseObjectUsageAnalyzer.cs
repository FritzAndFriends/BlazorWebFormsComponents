using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Response.Write(), Response.WriteFile(), Response.Clear(),
    /// Response.Flush(), and Response.End() usage, which require manual refactoring for Blazor.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ResponseObjectUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC013";

        private static readonly LocalizableString Title = "Response object methods require manual refactoring";
        private static readonly LocalizableString MessageFormat = "'{0}' uses Response.{1}() — this method is not available in Blazor and requires manual refactoring";
        private static readonly LocalizableString Description = "Response object methods like Write, WriteFile, Clear, Flush, and End are not available in Blazor and require manual refactoring.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        private static readonly ImmutableHashSet<string> DetectedMethods = ImmutableHashSet.Create(
            "Write", "WriteFile", "Clear", "Flush", "End");

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

            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.Identifier.Text;

                if (DetectedMethods.Contains(methodName) && IsResponseAccess(memberAccess))
                {
                    var containingMethod = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                    var memberName = containingMethod?.Identifier.Text ?? "<unknown>";

                    var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), memberName, methodName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool IsResponseAccess(MemberAccessExpressionSyntax memberAccess)
        {
            var expressionText = memberAccess.Expression.ToString();

            // Match Response.Method, this.Response.Method, HttpContext.Current.Response.Method, *.Response.Method
            return expressionText == "Response" ||
                   expressionText == "this.Response" ||
                   expressionText.EndsWith(".Response");
        }
    }
}
