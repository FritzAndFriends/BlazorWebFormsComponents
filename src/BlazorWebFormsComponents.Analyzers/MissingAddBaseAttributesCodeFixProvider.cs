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
    /// Code fix provider that adds AddBaseAttributes(writer) call at the beginning of Render method.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingAddBaseAttributesCodeFixProvider)), Shared]
    public class MissingAddBaseAttributesCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MissingAddBaseAttributesAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add AddBaseAttributes(writer) call",
                    createChangedDocument: c => AddBaseAttributesCallAsync(context.Document, declaration, c),
                    equivalenceKey: "Add AddBaseAttributes(writer) call"),
                diagnostic);
        }

        private async Task<Document> AddBaseAttributesCallAsync(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (methodDecl.Body == null)
                return document;

            // Get the parameter name (usually "writer")
            var parameterName = methodDecl.ParameterList.Parameters.FirstOrDefault()?.Identifier.Text ?? "writer";

            // Create the AddBaseAttributes(writer) call
            var addBaseAttributesCall = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.IdentifierName("AddBaseAttributes"),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameterName))))))
                .WithLeadingTrivia(methodDecl.Body.Statements.First().GetLeadingTrivia())
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            // Insert the call at the beginning of the method body
            var newStatements = methodDecl.Body.Statements.Insert(0, addBaseAttributesCall);
            var newBody = methodDecl.Body.WithStatements(newStatements);
            var newMethod = methodDecl.WithBody(newBody);

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
