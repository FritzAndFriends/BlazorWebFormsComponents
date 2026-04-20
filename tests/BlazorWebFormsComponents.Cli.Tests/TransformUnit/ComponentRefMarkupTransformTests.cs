using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ComponentRefMarkupTransform — adds @ref bindings to BWFC component
/// tags that have an id attribute, enabling code-behind access to component instances.
/// </summary>
public class ComponentRefMarkupTransformTests
{
    private readonly ComponentRefMarkupTransform _transform = new();

    private static FileMetadata MetadataWithCodeBehind => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = "",
        CodeBehindContent = "public partial class Test { }"
    };

    private static FileMetadata MetadataWithoutCodeBehind => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void OrderIs750()
    {
        Assert.Equal(750, _transform.Order);
    }

    [Fact]
    public void AddsRefToLabel()
    {
        var input = @"<Label id=""lblMessage"" Text=""Hello"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""lblMessage""", result);
        Assert.Contains(@"id=""lblMessage""", result); // id preserved
        Assert.Single(metadata.ComponentRefs);
        Assert.Equal("Label", metadata.ComponentRefs["lblMessage"]);
    }

    [Fact]
    public void AddsRefToButton()
    {
        var input = @"<Button id=""btnSubmit"" Text=""Submit"" OnClick=""@Submit_Click"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""btnSubmit""", result);
        Assert.Equal("Button", metadata.ComponentRefs["btnSubmit"]);
    }

    [Fact]
    public void AddsRefToGenericGridView_WithObjectType()
    {
        var input = @"<GridView ItemType=""object"" id=""gvData"" AutoGenerateColumns=""false"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""gvData""", result);
        Assert.Equal("GridView<object>", metadata.ComponentRefs["gvData"]);
    }

    [Fact]
    public void AddsRefToGenericGridView_WithTItem()
    {
        var input = @"<GridView TItem=""Product"" id=""gvProducts"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""gvProducts""", result);
        Assert.Equal("GridView<Product>", metadata.ComponentRefs["gvProducts"]);
    }

    [Fact]
    public void AddsRefToGenericListView()
    {
        var input = @"<ListView ItemType=""object"" id=""lvItems"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""lvItems""", result);
        Assert.Equal("ListView<object>", metadata.ComponentRefs["lvItems"]);
    }

    [Fact]
    public void SkipsHtmlElements()
    {
        var input = @"<div id=""myDiv""><span id=""mySpan"">text</span></div>";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.DoesNotContain("@ref", result);
        Assert.Empty(metadata.ComponentRefs);
    }

    [Fact]
    public void SkipsWhenNoCodeBehind()
    {
        var input = @"<Label id=""lblMessage"" Text=""Hello"" />";
        var metadata = MetadataWithoutCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.DoesNotContain("@ref", result);
        Assert.Empty(metadata.ComponentRefs);
    }

    [Fact]
    public void SkipsTagsWithoutId()
    {
        var input = @"<Label Text=""Hello"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.DoesNotContain("@ref", result);
        Assert.Empty(metadata.ComponentRefs);
    }

    [Fact]
    public void HandlesMultipleComponentsInSameFile()
    {
        var input = """
            <Label id="lblStatus" Text="Loading..." />
            <TextBox id="txtInput" />
            <Button id="btnSubmit" Text="Submit" />
            """;
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""lblStatus""", result);
        Assert.Contains(@"@ref=""txtInput""", result);
        Assert.Contains(@"@ref=""btnSubmit""", result);
        Assert.Equal(3, metadata.ComponentRefs.Count);
    }

    [Fact]
    public void PreservesOtherAttributes()
    {
        var input = @"<Label id=""lbl1"" Text=""Hello"" CssClass=""title"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"Text=""Hello""", result);
        Assert.Contains(@"CssClass=""title""", result);
        Assert.Contains(@"@ref=""lbl1""", result);
    }

    [Fact]
    public void SkipsIfAlreadyHasRef()
    {
        var input = @"<Label id=""lbl1"" @ref=""lbl1"" Text=""Hello"" />";
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        // Should not double-add @ref
        Assert.Equal(1, CountOccurrences(result, "@ref"));
    }

    [Fact]
    public void HandlesMixedComponentsAndHtml()
    {
        var input = """
            <div id="container">
                <Label id="lblTitle" Text="Title" />
                <p id="description">Text</p>
                <Button id="btnSave" Text="Save" />
            </div>
            """;
        var metadata = MetadataWithCodeBehind;

        var result = _transform.Apply(input, metadata);

        Assert.Contains(@"@ref=""lblTitle""", result);
        Assert.Contains(@"@ref=""btnSave""", result);
        Assert.Equal(2, metadata.ComponentRefs.Count);
        // HTML elements should not have @ref
        Assert.DoesNotContain(@"@ref=""container""", result);
        Assert.DoesNotContain(@"@ref=""description""", result);
    }

    [Fact]
    public void NonGenericComponentGetsSimpleType()
    {
        var input = @"<TextBox id=""txtName"" />";
        var metadata = MetadataWithCodeBehind;

        _transform.Apply(input, metadata);

        Assert.Equal("TextBox", metadata.ComponentRefs["txtName"]);
    }

    [Fact]
    public void ResolveFieldType_NonGeneric_ReturnsTagName()
    {
        Assert.Equal("Label", ComponentRefMarkupTransform.ResolveFieldType("Label", ""));
        Assert.Equal("Button", ComponentRefMarkupTransform.ResolveFieldType("Button", ""));
        Assert.Equal("TextBox", ComponentRefMarkupTransform.ResolveFieldType("TextBox", ""));
        Assert.Equal("CheckBox", ComponentRefMarkupTransform.ResolveFieldType("CheckBox", ""));
    }

    [Fact]
    public void ResolveFieldType_Generic_WithItemType_ReturnsGenericType()
    {
        Assert.Equal("GridView<object>",
            ComponentRefMarkupTransform.ResolveFieldType("GridView", @"ItemType=""object"""));
        Assert.Equal("ListView<Product>",
            ComponentRefMarkupTransform.ResolveFieldType("ListView", @"TItem=""Product"""));
    }

    [Fact]
    public void ResolveFieldType_Generic_NoTypeParam_DefaultsToObject()
    {
        Assert.Equal("GridView<object>",
            ComponentRefMarkupTransform.ResolveFieldType("GridView", ""));
        Assert.Equal("DropDownList<object>",
            ComponentRefMarkupTransform.ResolveFieldType("DropDownList", ""));
    }

    private static int CountOccurrences(string text, string pattern)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(pattern, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += pattern.Length;
        }
        return count;
    }
}
