using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects classes implementing IPostBackEventHandler.
    /// This interface is not available in Blazor; use EventCallback&lt;T&gt; instead.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IPostBackEventHandlerUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC023";

        private static readonly LocalizableString Title = "IPostBackEventHandler implementation detected";
        private static readonly LocalizableString MessageFormat = "IPostBackEventHandler is not available in Blazor. Remove the interface, replace RaisePostBackEvent with EventCallback<T> parameters, and use @onclick handlers for user interactions.";
        private static readonly LocalizableString Description = "IPostBackEventHandler is a Web Forms interface not available in Blazor. Remove the interface implementation, convert RaisePostBackEvent logic into component methods, and expose events via EventCallback<T> parameters with @onclick handlers.";
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
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (classDeclaration.BaseList == null)
                return;

            var implementsInterface = classDeclaration.BaseList.Types
                .Any(baseType =>
                {
                    var typeName = baseType.Type.ToString();
                    return typeName == "IPostBackEventHandler" ||
                           typeName.EndsWith(".IPostBackEventHandler");
                });

            if (!implementsInterface)
                return;

            var diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
