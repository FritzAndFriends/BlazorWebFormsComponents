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
    /// Code fix provider that replaces Request object property access with a TODO comment
    /// pointing to BWFC migration docs.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RequestObjectUsageCodeFixProvider)), Shared]
    public class RequestObjectUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RequestObjectUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            // Extract the property name from the diagnostic
            var propertyName = "Property";
            if (node is ElementAccessExpressionSyntax elementAccess &&
                elementAccess.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                propertyName = memberAccess.Name.Identifier.Text;
            }
            else if (node is MemberAccessExpressionSyntax directAccess)
            {
                propertyName = directAccess.Name.Identifier.Text;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with TODO comment",
                    createChangedDocument: c => ReplaceWithTodoCommentAsync(context.Document, node, propertyName, c),
                    equivalenceKey: "Replace Request access with TODO"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithTodoCommentAsync(Document document, SyntaxNode node, string propertyName, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var statement = node.FirstAncestorOrSelf<StatementSyntax>();
            if (statement == null)
            {
                return document;
            }

            var todoComment = SyntaxFactory.Comment(
                "// TODO: Replace Request." + propertyName + " access with Blazor equivalent");

            // Collect indentation trivia from the original statement
            var indentation = new System.Collections.Generic.List<SyntaxTrivia>();
            foreach (var trivia in statement.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    indentation.Add(trivia);
            }

            // Build leading trivia: original leading + comment + newline + indentation for the ;
            var leading = statement.GetLeadingTrivia()
                .Add(todoComment)
                .Add(SyntaxFactory.EndOfLine("\r\n"))
                .AddRange(indentation);

            var emptyStatement = SyntaxFactory.EmptyStatement()
                .WithLeadingTrivia(leading)
                .WithTrailingTrivia(statement.GetTrailingTrivia());

            var newRoot = root.ReplaceNode(statement, emptyStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
