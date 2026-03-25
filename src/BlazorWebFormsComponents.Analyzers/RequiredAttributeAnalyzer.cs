using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects when well-known BWFC component types are instantiated
    /// without setting critical properties (e.g., GridView without DataSource).
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequiredAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC010";

        private static readonly LocalizableString Title = "Required attribute may be missing on BWFC component";
        private static readonly LocalizableString MessageFormat = "'{0}' component typically requires '{1}' attribute for correct rendering";
        private static readonly LocalizableString Description = "Detects when well-known BWFC components are used without critical attributes that are needed for correct rendering.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Maps BWFC component type names to their required properties.
        /// </summary>
        internal static readonly Dictionary<string, string[]> RequiredProperties = new Dictionary<string, string[]>
        {
            { "GridView", new[] { "DataSource" } },
            { "HyperLink", new[] { "NavigateUrl" } },
            { "Image", new[] { "ImageUrl" } }
        };

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var creation = (ObjectCreationExpressionSyntax)context.Node;

            var typeName = GetUnqualifiedTypeName(creation.Type);
            if (typeName == null)
                return;

            // Strip generic arity (e.g., "GridView" from "GridView<T>")
            var baseTypeName = typeName;

            if (!RequiredProperties.TryGetValue(baseTypeName, out var requiredProps))
                return;

            // Verify the type resolves to a BWFC component
            var typeInfo = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken);
            var typeSymbol = typeInfo.Type;
            if (typeSymbol == null || !IsBwfcType(typeSymbol))
                return;

            // Collect property names assigned in the initializer
            var assignedProperties = GetInitializerAssignments(creation);

            // Collect property names assigned via subsequent statements on the same variable
            var variableName = GetAssignedVariableName(creation);
            if (variableName != null)
            {
                var containingBlock = creation.FirstAncestorOrSelf<BlockSyntax>();
                if (containingBlock != null)
                {
                    foreach (var assigned in GetSubsequentAssignments(containingBlock, variableName))
                    {
                        assignedProperties.Add(assigned);
                    }
                }
            }

            foreach (var required in requiredProps.Where(r => !assignedProperties.Contains(r)))
            {
                var diagnostic = Diagnostic.Create(Rule, creation.GetLocation(), baseTypeName, required);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static string GetUnqualifiedTypeName(TypeSyntax type)
        {
            switch (type)
            {
                case IdentifierNameSyntax id:
                    return id.Identifier.Text;
                case GenericNameSyntax generic:
                    return generic.Identifier.Text;
                case QualifiedNameSyntax qualified:
                    return GetUnqualifiedTypeName(qualified.Right);
                default:
                    return null;
            }
        }

        private static bool IsBwfcType(ITypeSymbol typeSymbol)
        {
            var current = typeSymbol;
            while (current != null)
            {
                var ns = current.ContainingNamespace?.ToDisplayString();
                if (ns != null && ns.StartsWith("BlazorWebFormsComponents"))
                    return true;
                current = current.BaseType;
            }
            return false;
        }

        private static HashSet<string> GetInitializerAssignments(ObjectCreationExpressionSyntax creation)
        {
            var result = new HashSet<string>();
            if (creation.Initializer != null)
            {
                foreach (var assignment in creation.Initializer.Expressions
                    .OfType<AssignmentExpressionSyntax>()
                    .Where(a => a.Left is IdentifierNameSyntax))
                {
                    result.Add(((IdentifierNameSyntax)assignment.Left).Identifier.Text);
                }
            }
            return result;
        }

        private static string GetAssignedVariableName(ObjectCreationExpressionSyntax creation)
        {
            // var x = new GridView(); → "x"
            if (creation.Parent is EqualsValueClauseSyntax equals &&
                equals.Parent is VariableDeclaratorSyntax declarator)
            {
                return declarator.Identifier.Text;
            }

            // x = new GridView(); → "x"
            if (creation.Parent is AssignmentExpressionSyntax assignment &&
                assignment.Left is IdentifierNameSyntax id)
            {
                return id.Identifier.Text;
            }

            return null;
        }

        private static IEnumerable<string> GetSubsequentAssignments(BlockSyntax block, string variableName)
        {
            return block.Statements
                .OfType<ExpressionStatementSyntax>()
                .Select(es => es.Expression)
                .OfType<AssignmentExpressionSyntax>()
                .Where(a => a.Left is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Expression is IdentifierNameSyntax target &&
                    target.Identifier.Text == variableName)
                .Select(a => ((MemberAccessExpressionSyntax)a.Left).Name.Identifier.Text);
        }
    }
}
