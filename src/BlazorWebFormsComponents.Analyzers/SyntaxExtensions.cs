using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace BlazorWebFormsComponents.Analyzers
{
    internal static class SyntaxExtensions
    {
        /// <summary>
        /// Detects the line ending style used in the syntax tree and returns a matching
        /// EndOfLine trivia. Falls back to <c>\n</c> if no existing line endings are found.
        /// </summary>
        internal static SyntaxTrivia DetectEndOfLine(this SyntaxNode root)
        {
            var eol = root.DescendantTrivia()
                .FirstOrDefault(t => t.IsKind(SyntaxKind.EndOfLineTrivia));
            return eol.IsKind(SyntaxKind.EndOfLineTrivia) ? eol : SyntaxFactory.EndOfLine("\n");
        }
    }
}
