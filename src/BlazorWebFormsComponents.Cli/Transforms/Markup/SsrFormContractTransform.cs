using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

public class SsrFormContractTransform : IMarkupTransform
{
    public string Name => "SsrFormContract";
    public int Order => 940;

    public string Apply(string content, FileMetadata metadata) =>
        FormAntiforgeryPostProcessor.Apply(content, metadata);
}
