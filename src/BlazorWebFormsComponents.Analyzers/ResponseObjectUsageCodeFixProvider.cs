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
    /// Code fix provider that replaces Response object method calls with a TODO comment
    /// pointing to BWFC migration docs.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ResponseObjectUsageCodeFixProvider)), Shared]
    public class ResponseObjectUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ResponseObjectUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            // Extract the method name from the invocation
            var methodName = "Method";
            if (node is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                methodName = memberAccess.Name.Identifier.Text;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with TODO comment",
                    createChangedDocument: c => ReplaceWithTodoCommentAsync(context.Document, node, methodName, c),
                    equivalenceKey: "Replace Response method with TODO"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithTodoCommentAsync(Document document, SyntaxNode node, string methodName, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var statement = node.FirstAncestorOrSelf<ExpressionStatementSyntax>();
            if (statement == null)
            {
                return document;
            }

            var todoComment = SyntaxFactory.Comment(
                "// TODO: Replace Response." + methodName + "(...) with Blazor equivalent \u2014 see BWFC migration docs");

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
