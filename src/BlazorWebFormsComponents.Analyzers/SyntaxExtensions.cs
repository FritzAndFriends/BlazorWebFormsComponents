using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
            foreach (var trivia in root.DescendantTrivia())
            {
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return trivia;
                }
            }

            return SyntaxFactory.EndOfLine("\n");
        }
    }
}
