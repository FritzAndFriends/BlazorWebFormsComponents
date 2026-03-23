using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects methods with classic Web Forms event handler signatures
    /// (object sender, EventArgs e) in Blazor component classes. These should be
    /// converted to EventCallback patterns.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventHandlerSignatureAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC011";

        private static readonly LocalizableString Title = "Web Forms event handler signature detected";
        private static readonly LocalizableString MessageFormat = "Method '{0}' has a Web Forms event handler signature (object, EventArgs). Consider using Blazor EventCallback patterns instead";
        private static readonly LocalizableString Description = "Methods with the classic (object sender, EventArgs e) signature don't work directly with Blazor's EventCallback system. Convert to EventCallback patterns.";
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

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            // Must have exactly 2 parameters
            if (method.ParameterList.Parameters.Count != 2)
                return;

            var firstParam = method.ParameterList.Parameters[0];
            var secondParam = method.ParameterList.Parameters[1];

            // First parameter type must be 'object'
            if (!IsObjectType(firstParam, context.SemanticModel))
                return;

            // Second parameter type name must end with 'EventArgs'
            if (!IsEventArgsType(secondParam, context.SemanticModel))
                return;

            // Method must be in a class that derives from ComponentBase or a BWFC base class
            var classDeclaration = method.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null)
                return;

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null)
                return;

            if (!DerivesFromComponentBase(classSymbol))
                return;

            var diagnostic = Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsObjectType(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            if (parameter.Type == null)
                return false;

            var typeInfo = semanticModel.GetTypeInfo(parameter.Type);
            if (typeInfo.Type != null)
            {
                return typeInfo.Type.SpecialType == SpecialType.System_Object;
            }

            // Fallback: check syntax name
            var typeName = parameter.Type.ToString();
            return typeName == "object" || typeName == "Object" || typeName == "System.Object";
        }

        private static bool IsEventArgsType(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            if (parameter.Type == null)
                return false;

            // Check the type name ends with "EventArgs"
            var typeInfo = semanticModel.GetTypeInfo(parameter.Type);
            if (typeInfo.Type != null)
            {
                return typeInfo.Type.Name.EndsWith("EventArgs");
            }

            // Fallback: check syntax name
            var typeName = parameter.Type.ToString();
            return typeName.EndsWith("EventArgs");
        }

        private static bool DerivesFromComponentBase(INamedTypeSymbol classSymbol)
        {
            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                var name = baseType.Name;
                // Blazor ComponentBase
                if (name == "ComponentBase")
                    return true;

                // BWFC base classes
                if (name == "WebControl" || name == "CompositeControl" ||
                    name == "BaseWebFormsComponent" || name == "BaseStyledComponent" ||
                    name == "WebFormsPageBase")
                    return true;

                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}
