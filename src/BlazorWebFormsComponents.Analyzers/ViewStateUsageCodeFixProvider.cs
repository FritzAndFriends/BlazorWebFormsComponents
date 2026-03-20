using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Code fix that comments out ViewState usage with a TODO comment.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ViewStateUsageCodeFixProvider)), Shared]
    public class ViewStateUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ViewStateUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Comment out ViewState usage with TODO",
                    createChangedDocument: c => CommentOutViewStateAsync(context.Document, node, c),
                    equivalenceKey: "Comment out ViewState usage"),
                diagnostic);
        }

        private async Task<Document> CommentOutViewStateAsync(Document document, SyntaxNode diagnosticNode, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var statement = diagnosticNode.FirstAncestorOrSelf<StatementSyntax>();
            if (statement == null)
                return document;

            // Extract the key from ViewState["key"]
            var elementAccess = diagnosticNode as ElementAccessExpressionSyntax
                ?? diagnosticNode.DescendantNodesAndSelf().OfType<ElementAccessExpressionSyntax>().FirstOrDefault();
            var keyArg = elementAccess?.ArgumentList.Arguments.FirstOrDefault()?.ToString() ?? "\"key\"";
            var todoComment = "// TODO: Replace ViewState[" + keyArg + "] with component state";

            var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            // Determine indentation from the statement's line
            var startLine = sourceText.Lines.GetLineFromPosition(statement.SpanStart);
            var lineText = startLine.ToString();
            var indent = new string(lineText.TakeWhile(c => c == ' ' || c == '\t').ToArray());

            // Get the statement text from the Span (excludes leading/trailing trivia)
            var stmtText = sourceText.GetSubText(statement.Span).ToString();

            // Detect line ending used in the source
            var newLine = GetLineEnding(sourceText);

            // Comment out each line of the statement
            var stmtLines = stmtText.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            var commentedLines = new string[stmtLines.Length];
            for (var i = 0; i < stmtLines.Length; i++)
            {
                var line = stmtLines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    commentedLines[i] = line;
                    continue;
                }

                var ws = 0;
                while (ws < line.Length && (line[ws] == ' ' || line[ws] == '\t'))
                    ws++;

                var lineIndent = line.Substring(0, ws);
                var lineCode = line.Substring(ws);

                if (i == 0)
                    commentedLines[i] = "// " + lineCode;
                else
                    commentedLines[i] = lineIndent + "// " + lineCode;
            }
            var commentedText = string.Join(newLine, commentedLines);

            // Build replacement: TODO comment + newline + indent + commented original
            var replacement = todoComment + newLine + indent + commentedText;

            // Replace the statement Span (preserving surrounding trivia)
            var newSourceText = sourceText.Replace(statement.Span, replacement);
            return document.WithText(newSourceText);
        }

        private static string GetLineEnding(SourceText sourceText)
        {
            if (sourceText.Lines.Count < 2)
                return "\r\n";
            var firstLine = sourceText.Lines[0];
            var endLength = firstLine.EndIncludingLineBreak - firstLine.End;
            if (endLength == 0)
                return "\r\n";
            return sourceText.ToString(new TextSpan(firstLine.End, endLength));
        }
    }
}
