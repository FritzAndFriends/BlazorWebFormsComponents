using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Code fix that replaces FindControl with FindControlRecursive.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindControlUsageCodeFixProvider)), Shared]
    public class FindControlUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FindControlUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            var invocation = node as InvocationExpressionSyntax
                ?? node.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocation == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with FindControlRecursive",
                    createChangedDocument: c => ReplaceFindControlAsync(context.Document, invocation, c),
                    equivalenceKey: "Replace with FindControlRecursive"),
                diagnostic);
        }

        private async Task<Document> ReplaceFindControlAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax newExpression;

            if (invocation.Expression is IdentifierNameSyntax)
            {
                // FindControl("id") -> FindControlRecursive("id")
                newExpression = SyntaxFactory.IdentifierName("FindControlRecursive");
            }
            else if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                // this.FindControl("id") -> this.FindControlRecursive("id")
                newExpression = memberAccess.WithName(SyntaxFactory.IdentifierName("FindControlRecursive"));
            }
            else
            {
                return document;
            }

            var newInvocation = invocation.WithExpression(newExpression);
            var newRoot = root.ReplaceNode(invocation, newInvocation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
