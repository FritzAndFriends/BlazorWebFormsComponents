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
    /// Code fix provider that replaces Response.Redirect() calls with a TODO comment
    /// pointing to NavigationManager.NavigateTo().
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ResponseRedirectCodeFixProvider)), Shared]
    public class ResponseRedirectCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ResponseRedirectAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with NavigationManager.NavigateTo() TODO",
                    createChangedDocument: c => ReplaceWithTodoCommentAsync(context.Document, node, c),
                    equivalenceKey: "Replace with NavigationManager.NavigateTo() TODO"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithTodoCommentAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var statement = node.FirstAncestorOrSelf<ExpressionStatementSyntax>();
            if (statement == null)
            {
                return document;
            }

            var expressionText = node.ToString();

            // Build comment: the EmptyStatement semicolon follows the // comment on the same line,
            // so it appears as part of the single-line comment text to readers.
            var todoComment = SyntaxFactory.Comment(
                "// TODO: Replace " + expressionText + " with NavigationManager.NavigateTo(\"url\")");

            var emptyStatement = SyntaxFactory.EmptyStatement()
                .WithLeadingTrivia(statement.GetLeadingTrivia().Add(todoComment))
                .WithTrailingTrivia(statement.GetTrailingTrivia());

            var newRoot = root.ReplaceNode(statement, emptyStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
