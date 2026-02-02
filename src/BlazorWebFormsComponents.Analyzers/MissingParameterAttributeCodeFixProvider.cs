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
    /// Code fix provider that adds [Parameter] attribute to public properties.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingParameterAttributeCodeFixProvider)), Shared]
    public class MissingParameterAttributeCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MissingParameterAttributeAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add [Parameter] attribute",
                    createChangedDocument: c => AddParameterAttributeAsync(context.Document, declaration, c),
                    equivalenceKey: "Add [Parameter] attribute"),
                diagnostic);
        }

        private async Task<Document> AddParameterAttributeAsync(Document document, PropertyDeclarationSyntax propertyDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Create the [Parameter] attribute
            var parameterAttribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Parameter"));
            var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(parameterAttribute));

            // Add the attribute to the property
            var newProperty = propertyDecl.AddAttributeLists(attributeList);

            // Replace the old property with the new one
            var newRoot = root.ReplaceNode(propertyDecl, newProperty);

            // Check if we need to add using directive for Microsoft.AspNetCore.Components
            var compilationUnit = newRoot as CompilationUnitSyntax;
            if (compilationUnit != null && !HasUsingDirective(compilationUnit, "Microsoft.AspNetCore.Components"))
            {
                var usingDirective = SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName("Microsoft.AspNetCore.Components"))
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

                newRoot = compilationUnit.AddUsings(usingDirective);
            }

            return document.WithSyntaxRoot(newRoot);
        }

        private bool HasUsingDirective(CompilationUnitSyntax compilationUnit, string namespaceName)
        {
            return compilationUnit.Usings.Any(u => u.Name.ToString() == namespaceName);
        }
    }
}
