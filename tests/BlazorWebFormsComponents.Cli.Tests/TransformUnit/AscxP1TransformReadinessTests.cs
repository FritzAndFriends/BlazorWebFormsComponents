using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public sealed class AscxP1TransformReadinessTests
{
    [Fact]
    public void PageLifecycleTransform_ConvertsControlPageLoadToOnParametersSet()
    {
        var transform = new PageLifecycleTransform();
        var metadata = CreateMetadata();
        var input = """
            using System;
            using System.Threading.Tasks;

            namespace TestApp
            {
                public partial class ProductsControl
                {
                    protected void Page_Load(object sender, EventArgs e)
                    {
                        InitializeProducts();
                    }
                }
            }
            """;

        var result = transform.Apply(input, metadata);

        Assert.Contains("protected override void OnParametersSet()", result);
        Assert.Contains("base.OnParametersSet();", result);
        Assert.Contains("TODO(bwfc-lifecycle)", result);
        Assert.DoesNotContain("Page_Load(object sender, EventArgs e)", result);
    }

    [Fact]
    public void DataBindTransform_TracksDataSourceAndInjectsItemsBinding()
    {
        var transform = new DataBindTransform();
        var metadata = CreateMetadata();
        var codeBehind = """
            ProductsRepeater.DataSource = LoadProducts();
            ProductsRepeater.DataBind();
            """;

        var transformedCodeBehind = transform.Apply(codeBehind, metadata);

        Assert.Equal("LoadProducts()", metadata.DataBindMap["ProductsRepeater"]);
        Assert.DoesNotContain("DataBind()", transformedCodeBehind);

        var markup = """<Repeater id="ProductsRepeater" />""";
        var transformedMarkup = DataBindTransform.InjectItemsAttributes(markup, metadata.DataBindMap);
        Assert.Contains("Items=\"@(LoadProducts())\"", transformedMarkup);
    }

    [Fact]
    public void ComponentRefTransforms_AddRefAndInjectMatchingField()
    {
        var markupTransform = new ComponentRefMarkupTransform();
        var codeBehindTransform = new ComponentRefCodeBehindTransform();
        var metadata = CreateMetadata();

        var markup = """<TextBox id="SearchBox" />""";
        var transformedMarkup = markupTransform.Apply(markup, metadata);

        Assert.Contains("@ref=\"SearchBox\"", transformedMarkup);
        Assert.Equal("TextBox", metadata.ComponentRefs["SearchBox"]);

        var codeBehind = """
            namespace TestApp
            {
                public partial class ProductsControl
                {
                    protected void Lookup()
                    {
                        var control = FindControl("SearchBox");
                    }
                }
            }
            """;

        var transformedCodeBehind = codeBehindTransform.Apply(codeBehind, metadata);
        Assert.Contains("private TextBox SearchBox = default!;", transformedCodeBehind);
    }

    [Fact]
    public void ContentTemplateUnwrapper_RewritesTemplateBindingsAndAddsContext()
    {
        var transform = new ContentTemplateUnwrapperTransform();
        var metadata = CreateMetadata();
        var input = """
            <ContentTemplate>
                <Repeater id="rptProducts">
                    <ItemTemplate>
                        <%# Eval("ProductName") %>
                    </ItemTemplate>
                </Repeater>
            </ContentTemplate>
            """;

        var result = transform.Apply(input, metadata);

        Assert.DoesNotContain("ContentTemplate", result);
        Assert.Contains("<ItemTemplate>", result);
        Assert.Contains("@Item.ProductName", result);
    }

    private static FileMetadata CreateMetadata() => new()
    {
        SourceFilePath = "ProductsControl.ascx",
        OutputFilePath = "ProductsControl.razor",
        FileType = FileType.Control,
        OriginalContent = "",
        CodeBehindContent = "public partial class ProductsControl { }"
    };
}
