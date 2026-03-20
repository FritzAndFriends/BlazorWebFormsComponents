using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects leftover runat="server" attributes in string literals.
    /// This Web Forms attribute has no effect in Blazor and should be removed.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RunatServerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC012";

        private static readonly LocalizableString Title = "Leftover runat=\"server\" attribute detected";
        private static readonly LocalizableString MessageFormat = "'{0}' contains runat=\"server\" \u2014 this is a Web Forms attribute that has no effect in Blazor and should be removed.";
        private static readonly LocalizableString Description = "When migrating .aspx files to .razor, developers often leave runat=\"server\" attributes. This attribute is meaningless in Blazor and is visual noise.";
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
            context.RegisterSyntaxNodeAction(AnalyzeStringLiteral, SyntaxKind.StringLiteralExpression);
        }

        private static void AnalyzeStringLiteral(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = (LiteralExpressionSyntax)context.Node;
            var text = literalExpression.Token.ValueText;

            if (!ContainsRunatServer(text))
                return;

            var containingName = GetContainingMemberName(literalExpression);
            var diagnostic = Diagnostic.Create(Rule, literalExpression.GetLocation(), containingName);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool ContainsRunatServer(string text)
        {
            // Check for runat="server" (double quotes) or runat='server' (single quotes)
            // Case-insensitive since HTML attributes are case-insensitive
            var lowerText = text.ToLowerInvariant();
            return lowerText.Contains("runat=\"server\"") || lowerText.Contains("runat='server'");
        }

        private static string GetContainingMemberName(SyntaxNode node)
        {
            var method = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (method != null)
                return method.Identifier.Text;

            var property = node.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (property != null)
                return property.Identifier.Text;

            var classDecl = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDecl != null)
                return classDecl.Identifier.Text;

            return "Unknown";
        }
    }
}
