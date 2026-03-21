using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Request.Form["key"], Request.Cookies["key"], Request.Headers["key"],
    /// Request.Files, Request.QueryString["key"], and Request.ServerVariables["key"] usage,
    /// which require Blazor-compatible patterns.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequestObjectUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC014";

        private static readonly LocalizableString Title = "Request object access requires Blazor patterns";
        private static readonly LocalizableString MessageFormat = "'{0}' accesses Request.{1} — use Blazor-compatible patterns instead (see BWFC migration docs)";
        private static readonly LocalizableString Description = "Request object properties like Form, Cookies, Headers, Files, QueryString, and ServerVariables are not available in Blazor. Use Blazor-compatible patterns instead.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        private static readonly ImmutableHashSet<string> IndexedProperties = ImmutableHashSet.Create(
            "Form", "Cookies", "Headers", "QueryString", "ServerVariables");

        private static readonly ImmutableHashSet<string> DirectProperties = ImmutableHashSet.Create(
            "Files");

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

            // Match Request.Form["key"], this.Request.Form["key"], HttpContext.Current.Request.Form["key"]
            if (elementAccess.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var propertyName = memberAccess.Name.Identifier.Text;

                if (IndexedProperties.Contains(propertyName) && IsRequestAccess(memberAccess.Expression))
                {
                    var containingMethod = elementAccess.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                    var memberName = containingMethod?.Identifier.Text ?? "<unknown>";

                    var diagnostic = Diagnostic.Create(Rule, elementAccess.GetLocation(), memberName, propertyName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            var propertyName = memberAccess.Name.Identifier.Text;

            // Match Request.Files, this.Request.Files, HttpContext.Current.Request.Files
            if (DirectProperties.Contains(propertyName) && IsRequestAccess(memberAccess.Expression))
            {
                // Avoid double-reporting: skip if this member access is already part of an element access
                // that we would catch in AnalyzeElementAccess
                if (memberAccess.Parent is ElementAccessExpressionSyntax)
                    return;

                var containingMethod = memberAccess.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                var memberName = containingMethod?.Identifier.Text ?? "<unknown>";

                var diagnostic = Diagnostic.Create(Rule, memberAccess.GetLocation(), memberName, propertyName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsRequestAccess(ExpressionSyntax expression)
        {
            var expressionText = expression.ToString();

            // Match Request, this.Request, HttpContext.Current.Request, *.Request
            return expressionText == "Request" ||
                   expressionText == "this.Request" ||
                   expressionText.EndsWith(".Request");
        }
    }
}
