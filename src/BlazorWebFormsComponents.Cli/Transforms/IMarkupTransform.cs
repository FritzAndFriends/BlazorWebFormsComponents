using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms;

/// <summary>
/// Interface for markup transforms that convert Web Forms markup to Blazor Razor syntax.
/// Transforms are applied in Order sequence (ascending).
/// </summary>
public interface IMarkupTransform
{
    string Name { get; }
    int Order { get; }
    string Apply(string content, FileMetadata metadata);
}
