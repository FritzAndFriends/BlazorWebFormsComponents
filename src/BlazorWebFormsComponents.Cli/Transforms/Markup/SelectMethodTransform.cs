using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Preserves SelectMethod and CRUD method attributes on BWFC data-bound controls because
/// DataBoundComponent already supports string-based SelectMethod/InsertMethod/UpdateMethod/
/// DeleteMethod binding in markup.
/// </summary>
public class SelectMethodTransform : IMarkupTransform
{
    public string Name => "SelectMethod";
    public int Order => 520;

    public string Apply(string content, FileMetadata metadata)
    {
        return content;
    }
}
