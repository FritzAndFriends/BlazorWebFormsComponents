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
    /// Code fix provider that adds a TODO comment suggesting conversion
    /// from Web Forms event handler signature to Blazor EventCallback pattern.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventHandlerSignatureCodeFixProvider)), Shared]
    public class EventHandlerSignatureCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(EventHandlerSignatureAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add TODO: Convert to EventCallback pattern",
                    createChangedDocument: c => AddTodoCommentAsync(context.Document, node, c),
                    equivalenceKey: "Add TODO: Convert to EventCallback pattern"),
                diagnostic);
        }

        private async Task<Document> AddTodoCommentAsync(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var todoComment = SyntaxFactory.Comment(
                "// TODO: Convert to EventCallback pattern \u2014 remove sender parameter, change return type if needed");

            var newLeadingTrivia = method.GetLeadingTrivia()
                .Add(todoComment)
                .Add(SyntaxFactory.EndOfLine("\r\n"));

            // Collect indentation from original method
            foreach (var trivia in method.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    newLeadingTrivia = newLeadingTrivia.Add(trivia);
                    break;
                }
            }

            var newMethod = method.WithLeadingTrivia(newLeadingTrivia);
            var newRoot = root.ReplaceNode(method, newMethod);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
