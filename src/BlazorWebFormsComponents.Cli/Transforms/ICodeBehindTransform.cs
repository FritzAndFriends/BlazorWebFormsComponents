using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms;

/// <summary>
/// Interface for code-behind transforms that convert Web Forms .cs code to Blazor .razor.cs code.
/// Transforms are applied in Order sequence (ascending).
/// </summary>
public interface ICodeBehindTransform
{
    string Name { get; }
    int Order { get; }
    string Apply(string content, FileMetadata metadata);
}
