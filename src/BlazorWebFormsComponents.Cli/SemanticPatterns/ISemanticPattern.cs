using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

/// <summary>
/// Contract for isolated semantic page-pattern rewrites that run after the
/// syntactic markup and code-behind transforms have established a compile-safe shape.
/// </summary>
public interface ISemanticPattern
{
    string Id { get; }
    int Order { get; }
    SemanticPatternMatch Match(SemanticPatternContext context);
    SemanticPatternResult Apply(SemanticPatternContext context);
}

