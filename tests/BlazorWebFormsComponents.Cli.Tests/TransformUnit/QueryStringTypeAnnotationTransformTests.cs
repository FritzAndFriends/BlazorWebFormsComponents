using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for QueryStringTypeAnnotationTransform — replaces var with string?
/// on Request.QueryString["key"] and Request["key"] assignments to prevent
/// StringValues capture in EF Core LINQ closures.
/// </summary>
public class QueryStringTypeAnnotationTransformTests
{
	private readonly QueryStringTypeAnnotationTransform _transform = new();

	private static FileMetadata TestMetadata(string content) => new()
	{
		SourceFilePath = "ProductDetails.aspx.cs",
		OutputFilePath = "ProductDetails.razor.cs",
		FileType = FileType.Page,
		OriginalContent = content
	};

	[Fact]
	public void ReplacesVar_WithQueryStringIndexer()
	{
		var input = """
			var productName = Request.QueryString["productName"];
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("string? productName = Request.QueryString[\"productName\"]", result);
		Assert.DoesNotContain("var productName", result);
	}

	[Fact]
	public void ReplacesVar_WithRequestIndexer()
	{
		var input = """
			var returnUrl = Request["ReturnUrl"];
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("string? returnUrl = Request[\"ReturnUrl\"]", result);
		Assert.DoesNotContain("var returnUrl", result);
	}

	[Fact]
	public void PreservesExplicitStringType()
	{
		var input = """
			string rawId = Request.QueryString["ProductID"];
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("string rawId = Request.QueryString[\"ProductID\"]", result);
	}

	[Fact]
	public void PreservesExplicitNullableStringType()
	{
		var input = """
			string? productName = Request.QueryString["productName"];
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("string? productName = Request.QueryString[\"productName\"]", result);
	}

	[Fact]
	public void HandlesMultipleAssignments()
	{
		var input = """
			var handler = Request.QueryString["handler"];
			string msg = Request.QueryString["msg"];
			var action = Request["action"];
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("string? handler = Request.QueryString[\"handler\"]", result);
		Assert.Contains("string msg = Request.QueryString[\"msg\"]", result);
		Assert.Contains("string? action = Request[\"action\"]", result);
	}

	[Fact]
	public void IgnoresNonQueryStringVar()
	{
		var input = """
			var count = items.Count();
			var name = GetName();
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("var count = items.Count()", result);
		Assert.Contains("var name = GetName()", result);
	}

	[Fact]
	public void DoesNotMatchPartialIdentifiers()
	{
		// "var" inside "variable" should not be matched
		var input = """
			variable = Request.QueryString["key"];
			""";

		var result = _transform.Apply(input, TestMetadata(input));

		Assert.Contains("variable = Request.QueryString[\"key\"]", result);
		Assert.DoesNotContain("string?iable", result);
	}
}
