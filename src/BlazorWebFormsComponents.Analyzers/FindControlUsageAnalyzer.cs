using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects FindControl("id") calls on types that do not inherit
    /// from BaseWebFormsComponent. BWFC provides FindControl on BaseWebFormsComponent
    /// with recursive search built in, so only non-BWFC usages need migration.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FindControlUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC021";

        private static readonly LocalizableString Title = "FindControl usage detected";
        private static readonly LocalizableString MessageFormat = "FindControl from System.Web.UI is not available. BWFC provides FindControl on BaseWebFormsComponent with recursive search.";
        private static readonly LocalizableString Description = "FindControl from System.Web.UI is not available in Blazor. Inherit from BaseWebFormsComponent which provides FindControl with built-in recursive search.";
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

            // Don't flag FindControl calls on BWFC types — those use the built-in recursive implementation
            if (IsOnBwfcType(context, invocation))
                return;

            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsFindControlCall(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "FindControl";

            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                return memberAccess.Name.Identifier.Text == "FindControl";

            return false;
        }

        private static bool IsOnBwfcType(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);

            if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                return InheritsFromOrIs(methodSymbol.ContainingType, "BaseWebFormsComponent");

            return symbolInfo.CandidateSymbols
                .OfType<IMethodSymbol>()
                .Any(cm => InheritsFromOrIs(cm.ContainingType, "BaseWebFormsComponent"));
        }

        private static bool InheritsFromOrIs(INamedTypeSymbol type, string baseTypeName)
        {
            var current = type;
            while (current != null)
            {
                if (current.Name == baseTypeName)
                    return true;
                current = current.BaseType;
            }
            return false;
        }
    }
}
