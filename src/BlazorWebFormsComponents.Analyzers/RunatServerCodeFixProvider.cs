using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Code fix that removes runat="server" substrings from string literals.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RunatServerCodeFixProvider)), Shared]
    public class RunatServerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RunatServerAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        // Matches runat="server" or runat='server' with optional surrounding whitespace before
        private static readonly Regex RunatPattern = new Regex(
            @"\s*runat\s*=\s*[""']server[""']",
            RegexOptions.IgnoreCase);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            var literalExpression = node as LiteralExpressionSyntax
                ?? node.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().FirstOrDefault();

            if (literalExpression == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Remove runat=\"server\" attribute",
                    createChangedDocument: c => RemoveRunatServerAsync(context.Document, literalExpression, c),
                    equivalenceKey: "Remove runat=\"server\" attribute"),
                diagnostic);
        }

        private async Task<Document> RemoveRunatServerAsync(Document document, LiteralExpressionSyntax literalExpression, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var originalText = literalExpression.Token.ValueText;

            // Remove runat="server" / runat='server' and any leading whitespace
            var cleanedText = RunatPattern.Replace(originalText, "");

            var newLiteral = SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(cleanedText));

            var newRoot = root.ReplaceNode(literalExpression, newLiteral.WithTriviaFrom(literalExpression));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
