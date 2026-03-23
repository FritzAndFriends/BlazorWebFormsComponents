using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects properties using ViewState for backing storage.
    /// These should be converted to [Parameter] properties for Blazor compatibility.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ViewStatePropertyPatternAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC020";

        private static readonly LocalizableString Title = "ViewState-backed property detected";
        private static readonly LocalizableString MessageFormat = "Property '{0}' uses ViewState for storage. Convert to a [Parameter] property for Blazor compatibility.";
        private static readonly LocalizableString Description = "Properties that use ViewState for backing storage should be converted to auto-properties with [Parameter] for Blazor.";
        private const string Category = "Migration";

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
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;

            if (property.AccessorList == null)
                return;

            // Check if any accessor body contains a ViewState element access
            var hasViewStateAccess = property.AccessorList.Accessors
                .Any(accessor => ContainsViewStateAccess(accessor));

            if (!hasViewStateAccess)
                return;

            var diagnostic = Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool ContainsViewStateAccess(AccessorDeclarationSyntax accessor)
        {
            // Check body (block syntax) and expression body
            var bodyNode = (SyntaxNode)accessor.Body ?? accessor.ExpressionBody;
            if (bodyNode == null)
                return false;

            return bodyNode.DescendantNodes()
                .OfType<ElementAccessExpressionSyntax>()
                .Any(ea => IsViewStateAccess(ea));
        }

        private static bool IsViewStateAccess(ElementAccessExpressionSyntax elementAccess)
        {
            var expr = elementAccess.Expression;

            if (expr is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "ViewState";

            if (expr is MemberAccessExpressionSyntax memberAccess)
                return memberAccess.Name.Identifier.Text == "ViewState" &&
                       memberAccess.Expression is ThisExpressionSyntax;

            return false;
        }
    }
}
