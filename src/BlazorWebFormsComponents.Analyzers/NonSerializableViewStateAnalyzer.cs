using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects ViewState assignments storing types that are
    /// unlikely to be JSON-serializable, which will fail at runtime in SSR mode.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonSerializableViewStateAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC025";

        private static readonly LocalizableString Title = "ViewState value may not be JSON-serializable";
        private static readonly LocalizableString MessageFormat = "ViewState assignment stores '{0}' which may not be JSON-serializable. SSR ViewState uses System.Text.Json serialization \u2014 ensure the type has a parameterless constructor and public properties.";
        private static readonly LocalizableString Description = "Detects ViewState assignments storing types that are unlikely to be JSON-serializable, such as IDisposable implementations, delegates, DataSet/DataTable, and System.Web types.";
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
            context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
        {
            var assignment = (AssignmentExpressionSyntax)context.Node;

            // Check if left side is ViewState["key"] or this.ViewState["key"]
            if (!(assignment.Left is ElementAccessExpressionSyntax elementAccess))
                return;

            if (!IsViewStateAccess(elementAccess))
                return;

            var typeInfo = context.SemanticModel.GetTypeInfo(assignment.Right, context.CancellationToken);
            if (typeInfo.Type != null && IsLikelyNonSerializable(typeInfo.Type))
            {
                var diagnostic = Diagnostic.Create(Rule, assignment.GetLocation(), typeInfo.Type.ToDisplayString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            // Check for ViewState.Set<T>("key", value) or this.ViewState.Set<T>("key", value)
            if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
                return;

            if (memberAccess.Name.Identifier.Text != "Set")
                return;

            if (!IsViewStateExpression(memberAccess.Expression))
                return;

            // Get the type argument from Set<T>
            if (memberAccess.Name is GenericNameSyntax genericName &&
                genericName.TypeArgumentList.Arguments.Count == 1)
            {
                var typeArg = genericName.TypeArgumentList.Arguments[0];
                var typeSymbol = context.SemanticModel.GetTypeInfo(typeArg, context.CancellationToken).Type;
                if (typeSymbol != null && IsLikelyNonSerializable(typeSymbol))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), typeSymbol.ToDisplayString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
            else if (invocation.ArgumentList.Arguments.Count >= 2)
            {
                // Non-generic Set("key", value) — check the value argument type
                var valueArg = invocation.ArgumentList.Arguments[1];
                var typeInfo = context.SemanticModel.GetTypeInfo(valueArg.Expression, context.CancellationToken);
                if (typeInfo.Type != null && IsLikelyNonSerializable(typeInfo.Type))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), typeInfo.Type.ToDisplayString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool IsViewStateAccess(ElementAccessExpressionSyntax elementAccess)
        {
            return IsViewStateExpression(elementAccess.Expression);
        }

        private static bool IsViewStateExpression(ExpressionSyntax expr)
        {
            // ViewState
            if (expr is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "ViewState";

            // this.ViewState
            if (expr is MemberAccessExpressionSyntax memberAccess)
                return memberAccess.Name.Identifier.Text == "ViewState" &&
                       memberAccess.Expression is ThisExpressionSyntax;

            return false;
        }

        private static bool IsLikelyNonSerializable(ITypeSymbol type)
        {
            // Delegates and event handlers are never serializable
            if (type.TypeKind == TypeKind.Delegate)
                return true;

            // Check for IDisposable (DB connections, streams, etc.)
            if (type.AllInterfaces.Any(i => i.Name == "IDisposable"))
                return true;

            var fullName = type.ToDisplayString();

            // System.Data types (DataSet, DataTable, etc.) — common Web Forms pattern, not JSON-friendly
            if (fullName.StartsWith("System.Data."))
                return true;

            // System.Web types — not available in Blazor
            if (fullName.StartsWith("System.Web."))
                return true;

            // Stream types
            if (fullName.StartsWith("System.IO.Stream") || fullName == "System.IO.MemoryStream" || fullName == "System.IO.FileStream")
                return true;

            // Network types
            if (fullName.StartsWith("System.Net."))
                return true;

            return false;
        }
    }
}
