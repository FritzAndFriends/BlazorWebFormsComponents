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
    /// Code fix that replaces a ViewState-backed property with a [Parameter] auto-property.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ViewStatePropertyPatternCodeFixProvider)), Shared]
    public class ViewStatePropertyPatternCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ViewStatePropertyPatternAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var token = root.FindToken(diagnosticSpan.Start);
            var property = token.Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
            if (property == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Convert to [Parameter] auto-property",
                    createChangedDocument: c => ConvertToParameterPropertyAsync(context.Document, property, c),
                    equivalenceKey: "Convert to [Parameter] auto-property"),
                diagnostic);
        }

        private async Task<Document> ConvertToParameterPropertyAsync(Document document, PropertyDeclarationSyntax property, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Build compact auto-property accessor list: { get; set; }
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithLeadingTrivia(SyntaxFactory.Space)
                .WithTrailingTrivia(SyntaxFactory.Space);

            var setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithLeadingTrivia(SyntaxFactory.Space);

            var autoAccessors = SyntaxFactory.AccessorList(
                SyntaxFactory.List(new[] { getAccessor, setAccessor }))
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(
                    SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                        .WithLeadingTrivia(SyntaxFactory.Space));

            // Replace the multi-line accessor list with compact auto-property
            var newProperty = property.WithAccessorList(autoAccessors);

            // Add [Parameter] attribute (same approach as MissingParameterAttributeCodeFixProvider)
            var parameterAttribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Parameter"));
            var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(parameterAttribute));
            newProperty = newProperty.AddAttributeLists(attributeList);

            var newRoot = root.ReplaceNode(property, newProperty);

            // Add using directive if needed
            var compilationUnit = newRoot as CompilationUnitSyntax;
            if (compilationUnit != null && !HasUsingDirective(compilationUnit, "Microsoft.AspNetCore.Components"))
            {
                var usingDirective = SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName("Microsoft.AspNetCore.Components"))
                    .WithTrailingTrivia(newRoot.DetectEndOfLine());

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
