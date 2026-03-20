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
    /// Code fix provider that replaces Session/HttpContext.Current usage with a TODO comment
    /// pointing to scoped services or ProtectedSessionStorage.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SessionUsageCodeFixProvider)), Shared]
    public class SessionUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SessionUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with scoped service TODO",
                    createChangedDocument: c => ReplaceWithTodoCommentAsync(context.Document, node, c),
                    equivalenceKey: "Replace with scoped service TODO"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithTodoCommentAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var statement = node.FirstAncestorOrSelf<StatementSyntax>();
            if (statement == null)
            {
                return document;
            }

            // Extract the flagged expression text for the TODO comment
            var expressionText = node.ToString();

            var todoComment = SyntaxFactory.Comment(
                "// TODO: Replace " + expressionText + " with scoped service or ProtectedSessionStorage");
            var endOfLine = SyntaxFactory.EndOfLine("\r\n");

            // Preserve original indentation for the TODO comment line
            var leadingTrivia = statement.GetLeadingTrivia();
            var indentTrivia = leadingTrivia.Where(t => t.IsKind(SyntaxKind.WhitespaceTrivia)).ToList();

            var newLeading = SyntaxFactory.TriviaList(leadingTrivia
                .AddRange(new[] { todoComment, endOfLine })
                .AddRange(indentTrivia));

            var newStatement = statement.WithLeadingTrivia(newLeading);
            var newRoot = root.ReplaceNode(statement, newStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
