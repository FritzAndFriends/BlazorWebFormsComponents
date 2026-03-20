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
    /// Code fix provider that adds a TODO comment for missing required attributes
    /// on BWFC component instantiations.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RequiredAttributeCodeFixProvider)), Shared]
    public class RequiredAttributeCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RequiredAttributeAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            var componentName = diagnostic.GetMessage().Split('\'')[1];
            var attributeName = diagnostic.GetMessage().Split('\'')[3];

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Add TODO for missing '{attributeName}' attribute",
                    createChangedDocument: c => AddTodoCommentAsync(context.Document, node, componentName, attributeName, c),
                    equivalenceKey: $"Add TODO for missing attribute"),
                diagnostic);
        }

        private async Task<Document> AddTodoCommentAsync(Document document, SyntaxNode node, string componentName, string attributeName, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Find the containing statement
            var statement = node.FirstAncestorOrSelf<StatementSyntax>();
            if (statement == null)
            {
                return document;
            }

            var todoComment = SyntaxFactory.Comment(
                $"// TODO: Set {componentName}.{attributeName} — required for correct rendering");

            var newLeadingTrivia = statement.GetLeadingTrivia()
                .Insert(0, todoComment)
                .Insert(1, SyntaxFactory.EndOfLine("\r\n"));

            var newStatement = statement.WithLeadingTrivia(newLeadingTrivia);
            var newRoot = root.ReplaceNode(statement, newStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
