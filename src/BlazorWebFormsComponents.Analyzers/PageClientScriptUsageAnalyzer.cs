using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Analyzer that detects Page.ClientScript usage patterns and provides
    /// method-specific migration guidance for Blazor.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageClientScriptUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BWFC022";

        private static readonly LocalizableString Title = "Page.ClientScript usage detected";
        private static readonly LocalizableString MessageFormat = "Page.ClientScript{0} is not available in Blazor. {1}";
        private static readonly LocalizableString Description = "Page.ClientScript methods like RegisterStartupScript and GetPostBackEventReference are not available in Blazor. Use IJSRuntime for JavaScript interop.";
        private const string Category = "Migration";

        internal const string FallbackGuidance = "Use IJSRuntime for JavaScript interop.";
        internal const string RegisterStartupScriptGuidance = "Use IJSRuntime.InvokeAsync in OnAfterRenderAsync(firstRender: true).";
        internal const string RegisterClientScriptIncludeGuidance = "Add <script src='...'/> to your layout or use IJSRuntime.";
        internal const string RegisterClientScriptBlockGuidance = "Use IJSRuntime.InvokeVoidAsync to execute script blocks.";
        internal const string GetPostBackEventReferenceGuidance = "Use @onclick or EventCallback<T> instead of postback events.";

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

            if (!IsClientScriptAccess(memberAccess))
                return;

            var (methodSuffix, guidance) = GetMethodSpecificGuidance(memberAccess);
            var diagnostic = Diagnostic.Create(Rule, memberAccess.GetLocation(), methodSuffix, guidance);
            context.ReportDiagnostic(diagnostic);
        }

        private static (string methodSuffix, string guidance) GetMethodSpecificGuidance(MemberAccessExpressionSyntax clientScriptAccess)
        {
            // Check if Page.ClientScript is followed by a specific method call
            if (clientScriptAccess.Parent is MemberAccessExpressionSyntax outerAccess)
            {
                var methodName = outerAccess.Name.Identifier.Text;
                switch (methodName)
                {
                    case "RegisterStartupScript":
                        return (".RegisterStartupScript()", RegisterStartupScriptGuidance);
                    case "RegisterClientScriptInclude":
                        return (".RegisterClientScriptInclude()", RegisterClientScriptIncludeGuidance);
                    case "RegisterClientScriptBlock":
                        return (".RegisterClientScriptBlock()", RegisterClientScriptBlockGuidance);
                    case "GetPostBackEventReference":
                        return (".GetPostBackEventReference()", GetPostBackEventReferenceGuidance);
                }
            }

            return ("", FallbackGuidance);
        }

        private static bool IsClientScriptAccess(MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess.Name.Identifier.Text != "ClientScript")
                return false;

            if (memberAccess.Expression is IdentifierNameSyntax identifier)
                return identifier.Identifier.Text == "Page";

            if (memberAccess.Expression is MemberAccessExpressionSyntax innerMember)
                return innerMember.Name.Identifier.Text == "Page" &&
                       innerMember.Expression is ThisExpressionSyntax;

            return false;
        }
    }
}
