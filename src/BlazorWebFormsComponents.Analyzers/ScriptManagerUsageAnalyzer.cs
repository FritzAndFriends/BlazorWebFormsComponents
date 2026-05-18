using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects ScriptManager code-behind usage patterns.
    /// ScriptManager methods like GetCurrent(), RegisterAsyncPostBackControl(),
    /// and SetFocus() have no direct Blazor equivalent.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ScriptManagerUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC024";

        private static readonly LocalizableString Title = "ScriptManager code-behind usage detected";
        private static readonly LocalizableString MessageFormat = "ScriptManager.{0} has no Blazor equivalent. {1}";
        private static readonly LocalizableString Description = "ScriptManager code-behind methods have no Blazor equivalent. Use IJSRuntime for script execution and component @ref for focus management.";
        private const string Category = "Migration";

        internal const string GetCurrentGuidance = "Use @inject IJSRuntime for JavaScript interop or remove if only used for UpdatePanel registration.";
        internal const string RegisterAsyncPostBackControlGuidance = "Blazor components re-render automatically. Remove this call — async postback controls are not needed.";
        internal const string SetFocusGuidance = "Use ElementReference with FocusAsync(), or IJSRuntime to call element.focus() directly.";
        internal const string RegisterStartupScriptGuidance = "Use IJSRuntime.InvokeAsync in OnAfterRenderAsync(firstRender: true).";
        internal const string RegisterClientScriptBlockGuidance = "Use IJSRuntime.InvokeVoidAsync to execute script blocks.";
        internal const string FallbackGuidance = "Use IJSRuntime for script execution and component @ref for focus management.";

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
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            if (!IsScriptManagerAccess(memberAccess, out var methodName))
                return;

            var guidance = GetMethodGuidance(methodName);
            var diagnostic = Diagnostic.Create(Rule, memberAccess.GetLocation(), methodName, guidance);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsScriptManagerAccess(MemberAccessExpressionSyntax memberAccess, out string methodName)
        {
            methodName = memberAccess.Name.Identifier.Text;

            // Match ScriptManager.MethodName(...)
            if (memberAccess.Expression is IdentifierNameSyntax identifier &&
                identifier.Identifier.Text == "ScriptManager")
            {
                return IsTargetMethod(methodName);
            }

            return false;
        }

        private static bool IsTargetMethod(string methodName)
        {
            switch (methodName)
            {
                case "GetCurrent":
                case "RegisterAsyncPostBackControl":
                case "SetFocus":
                case "RegisterStartupScript":
                case "RegisterClientScriptBlock":
                    return true;
                default:
                    return false;
            }
        }

        private static string GetMethodGuidance(string methodName)
        {
            switch (methodName)
            {
                case "GetCurrent":
                    return GetCurrentGuidance;
                case "RegisterAsyncPostBackControl":
                    return RegisterAsyncPostBackControlGuidance;
                case "SetFocus":
                    return SetFocusGuidance;
                case "RegisterStartupScript":
                    return RegisterStartupScriptGuidance;
                case "RegisterClientScriptBlock":
                    return RegisterClientScriptBlockGuidance;
                default:
                    return FallbackGuidance;
            }
        }
    }
}
