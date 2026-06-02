using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class TemplateContextTransformTests
{
    private readonly TemplateContextTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void AddsContextToItemTemplate()
    {
        var result = _transform.Apply("<ItemTemplate><span>@Item.Name</span></ItemTemplate>", TestMetadata);

        Assert.Contains("<ItemTemplate Context=\"Item\">", result);
    }

    [Fact]
    public void AddsContextToEditItemTemplate()
    {
        var result = _transform.Apply("<EditItemTemplate><TextBox Text=\"@Item.Name\" /></EditItemTemplate>", TestMetadata);

        Assert.Contains("<EditItemTemplate Context=\"Item\">", result);
    }

    [Fact]
    public void DoesNotDuplicateExistingContext()
    {
        var input = "<ItemTemplate Context=\"Item\"><span>@Item.Name</span></ItemTemplate>";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void PipelineAddsContextForGridTemplateField()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = TestMetadata;

        var result = pipeline.TransformMarkup("<asp:TemplateField><ItemTemplate><%#: Item.Name %></ItemTemplate></asp:TemplateField>", metadata);

        Assert.Contains("<TemplateField ItemType=\"object\">", result);
        Assert.Contains("<ItemTemplate Context=\"Item\">@Item.Name</ItemTemplate>", result);
    }

    [Fact]
    public void PipelineAddsNamedContextForGroupTemplatePlaceholder()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = TestMetadata;

        var result = pipeline.TransformMarkup("<asp:ListView ItemType=\"Widget\"><GroupTemplate><tr id=\"itemPlaceholder\" runat=\"server\"></tr></GroupTemplate></asp:ListView>", metadata);

        Assert.Contains("<GroupTemplate Context=\"items\">", result);
        Assert.Contains("@items", result);
    }

    [Fact]
    public void PipelineAddsNamedContextForLayoutTemplatePlaceholder()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = TestMetadata;

        var result = pipeline.TransformMarkup("<asp:ListView ItemType=\"Widget\"><LayoutTemplate><div id=\"groupPlaceholder\" runat=\"server\"></div></LayoutTemplate></asp:ListView>", metadata);

        Assert.Contains("<LayoutTemplate Context=\"groups\">", result);
        Assert.Contains("@groups", result);
    }
}
