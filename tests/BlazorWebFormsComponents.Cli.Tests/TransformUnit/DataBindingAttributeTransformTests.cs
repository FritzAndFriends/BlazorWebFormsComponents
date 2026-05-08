using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class DataBindingAttributeTransformTests
{
	private readonly DataBindingAttributeTransform _transform = new();

	private static FileMetadata TestMeta => new()
	{
		SourceFilePath = "test.aspx",
		OutputFilePath = "test.razor",
		FileType = FileType.Page,
		OriginalContent = string.Empty
	};

	[Fact]
	public void HasExpectedMetadata()
	{
		Assert.Equal("DataBindingAttribute", _transform.Name);
		Assert.Equal(615, _transform.Order);
	}

	[Fact]
	public void ConvertsSingleQuotedDataBindingAttribute()
	{
		var input = "<HyperLink NavigateUrl='<%# Item.GetUrl() %>' Text='Plain' />";

		var result = _transform.Apply(input, TestMeta);

		Assert.Contains("NavigateUrl='@(Item.GetUrl())'", result);
		Assert.Contains("Text='Plain'", result);
	}

	[Fact]
	public void ConvertsDoubleQuotedDataBindingAttribute()
	{
		var input = "<HyperLink NavigateUrl=\"<%# Item.GetUrl() %>\" Text=\"<%# Item.Name %>\" />";

		var result = _transform.Apply(input, TestMeta);

		Assert.Contains("NavigateUrl=\"@(Item.GetUrl())\"", result);
		Assert.Contains("Text=\"@(Item.Name)\"", result);
	}

	[Fact]
	public void ConvertsUnencodedAttributeExpression()
	{
		var input = "<Image ImageUrl='<%= ResolveUrl(\"~/img/test.png\") %>' />";

		var result = _transform.Apply(input, TestMeta);

		Assert.Contains("ImageUrl='@(ResolveUrl(\"~/img/test.png\"))'", result);
	}

	[Fact]
	public void SwitchesQuoteStyleWhenExpressionContainsAttributeDelimiter()
	{
		var input = @"<HyperLink Text=""<%# Eval(""Name"") %>"" />";

		var result = _transform.Apply(input, TestMeta);

		Assert.Contains(@"Text='@(Eval(""Name""))'", result);
	}

	[Fact]
	public void LeavesContentExpressionsAlone()
	{
		var input = "<ItemTemplate><%# Item.Name %></ItemTemplate>";

		var result = _transform.Apply(input, TestMeta);

		Assert.Equal(input, result);
	}

	[Fact]
	public void PipelineTransformsAttributeAfterAspPrefixRemoval()
	{
		var pipeline = TestHelpers.CreateDefaultPipeline();
		var metadata = new FileMetadata
		{
			SourceFilePath = "Product.aspx",
			OutputFilePath = "Product.razor",
			FileType = FileType.Page,
			OriginalContent = string.Empty
		};

		var result = pipeline.TransformMarkup("<asp:HyperLink NavigateUrl='<%# Item.GetUrl() %>' Text='<%# Item.Name %>' runat='server' />", metadata);

		Assert.Contains("<HyperLink", result);
		Assert.Contains("NavigateUrl='@(Item.GetUrl())'", result);
		Assert.Contains("Text='@(Item.Name)'", result);
		Assert.DoesNotContain("<%#", result);
	}
}
