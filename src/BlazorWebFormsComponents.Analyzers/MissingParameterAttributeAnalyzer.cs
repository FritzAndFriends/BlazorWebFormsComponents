using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects public properties in WebControl/CompositeControl derived classes
    /// that should have [Parameter] attributes for Blazor compatibility.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingParameterAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC001";

        private static readonly LocalizableString Title = "Public property should have [Parameter] attribute";
        private static readonly LocalizableString MessageFormat = "Property '{0}' in WebControl-derived class should have [Parameter] attribute for Blazor compatibility";
        private static readonly LocalizableString Description = "When migrating Web Forms custom controls to Blazor, public properties that should accept values from markup need [Parameter] attributes.";
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
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            // Only analyze public properties
            if (!propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return;

            // Skip if property already has [Parameter] attribute
            if (HasParameterAttribute(propertyDeclaration))
                return;

            // Skip inherited properties like ID, CssClass, Style, etc.
            var propertyName = propertyDeclaration.Identifier.Text;
            if (IsInheritedProperty(propertyName))
                return;

            // Check if the containing class derives from WebControl or CompositeControl
            var classDeclaration = propertyDeclaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null)
                return;

            var semanticModel = context.SemanticModel;
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null)
                return;

            if (DerivesFromWebControl(classSymbol))
            {
                var diagnostic = Diagnostic.Create(Rule, propertyDeclaration.Identifier.GetLocation(), propertyName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool HasParameterAttribute(PropertyDeclarationSyntax property)
        {
            return property.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(attr => attr.Name.ToString().Contains("Parameter"));
        }

        private static bool IsInheritedProperty(string propertyName)
        {
            // Properties inherited from BaseWebFormsComponent or BaseStyledComponent
            var inheritedProperties = new[]
            {
                "ID", "Enabled", "TabIndex", "Visible", "ViewState", "EnableViewState", "runat",
                "BackColor", "BorderColor", "BorderStyle", "BorderWidth", "CssClass", "ForeColor",
                "Height", "Width", "Font", "Style", "Parent", "Controls"
            };

            return inheritedProperties.Contains(propertyName);
        }

        private static bool DerivesFromWebControl(INamedTypeSymbol classSymbol)
        {
            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                var baseTypeName = baseType.Name;
                if (baseTypeName == "WebControl" || baseTypeName == "CompositeControl")
                {
                    // Check namespace to ensure it's our WebControl/CompositeControl
                    var ns = baseType.ContainingNamespace?.ToDisplayString();
                    if (ns != null && ns.Contains("BlazorWebFormsComponents"))
                        return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}
