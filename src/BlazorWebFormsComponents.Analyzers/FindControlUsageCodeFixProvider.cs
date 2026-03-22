using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.Analyzers
{
    /// <summary>
    /// Code fix provider for BWFC021. No automatic fix is offered because BWFC now
    /// provides FindControl directly on BaseWebFormsComponent with recursive search.
    /// The migration path is to inherit from BaseWebFormsComponent, which requires
    /// broader refactoring than a simple rename.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindControlUsageCodeFixProvider)), Shared]
    public class FindControlUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FindControlUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => null;

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            // No automatic code fix — BWFC provides FindControl on BaseWebFormsComponent
            // with recursive search. Migrate by inheriting from BaseWebFormsComponent.
            return Task.CompletedTask;
        }
    }
}
