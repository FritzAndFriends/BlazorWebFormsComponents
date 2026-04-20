using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ComponentRefCodeBehindTransform — injects private field declarations
/// for @ref component references into the code-behind partial class.
/// </summary>
public class ComponentRefCodeBehindTransformTests
{
    private readonly ComponentRefCodeBehindTransform _transform = new();

    private static FileMetadata CreateMetadata(Dictionary<string, string>? refs = null)
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "test.aspx",
            OutputFilePath = "test.razor",
            FileType = FileType.Page,
            OriginalContent = ""
        };
        if (refs != null)
        {
            foreach (var (k, v) in refs)
                metadata.ComponentRefs[k] = v;
        }
        return metadata;
    }

    [Fact]
    public void OrderIs220()
    {
        Assert.Equal(220, _transform.Order);
    }

    [Fact]
    public void InjectsFieldForLabel()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                    protected void Page_Load() { }
                }
            }
            """;
        var metadata = CreateMetadata(new() { ["lblMessage"] = "Label" });

        var result = _transform.Apply(input, metadata);

        Assert.Contains("private Label lblMessage = default!;", result);
    }

    [Fact]
    public void InjectsFieldForGenericGridView()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                    protected void Page_Load() { }
                }
            }
            """;
        var metadata = CreateMetadata(new() { ["gvData"] = "GridView<object>" });

        var result = _transform.Apply(input, metadata);

        Assert.Contains("private GridView<object> gvData = default!;", result);
    }

    [Fact]
    public void InjectsMultipleFields()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                    protected void Page_Load() { }
                }
            }
            """;
        var metadata = CreateMetadata(new()
        {
            ["lblStatus"] = "Label",
            ["txtInput"] = "TextBox",
            ["btnSubmit"] = "Button"
        });

        var result = _transform.Apply(input, metadata);

        Assert.Contains("private Label lblStatus = default!;", result);
        Assert.Contains("private TextBox txtInput = default!;", result);
        Assert.Contains("private Button btnSubmit = default!;", result);
    }

    [Fact]
    public void InsertsAfterClassOpeningBrace()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                    protected void Page_Load() { }
                }
            }
            """;
        var metadata = CreateMetadata(new() { ["lbl1"] = "Label" });

        var result = _transform.Apply(input, metadata);

        // Field should appear between class opening brace and Page_Load
        var fieldIndex = result.IndexOf("private Label lbl1");
        var methodIndex = result.IndexOf("protected void Page_Load");
        Assert.True(fieldIndex > 0, "Field should be present");
        Assert.True(fieldIndex < methodIndex, "Field should appear before methods");
    }

    [Fact]
    public void SkipsWhenNoComponentRefs()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                    protected void Page_Load() { }
                }
            }
            """;
        var metadata = CreateMetadata();

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void SkipsWhenNoClassFound()
    {
        var input = "// just a comment, no class";
        var metadata = CreateMetadata(new() { ["lbl1"] = "Label" });

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void SkipsExistingFieldDeclaration()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                    private Label lblMessage;
                    protected void Page_Load() { }
                }
            }
            """;
        var metadata = CreateMetadata(new() { ["lblMessage"] = "Label" });

        var result = _transform.Apply(input, metadata);

        // Should not add a duplicate field — only the original declaration should exist
        var count = CountOccurrences(result, "lblMessage");
        Assert.Equal(1, count);
    }

    [Fact]
    public void FieldsAreSortedAlphabetically()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                }
            }
            """;
        var metadata = CreateMetadata(new()
        {
            ["txtInput"] = "TextBox",
            ["btnSubmit"] = "Button",
            ["lblStatus"] = "Label"
        });

        var result = _transform.Apply(input, metadata);

        var btnIndex = result.IndexOf("private Button btnSubmit");
        var lblIndex = result.IndexOf("private Label lblStatus");
        var txtIndex = result.IndexOf("private TextBox txtInput");

        Assert.True(btnIndex < lblIndex, "btnSubmit should come before lblStatus");
        Assert.True(lblIndex < txtIndex, "lblStatus should come before txtInput");
    }

    [Fact]
    public void WorksWithGenericListViewType()
    {
        var input = """
            namespace TestApp
            {
                public partial class TestPage
                {
                }
            }
            """;
        var metadata = CreateMetadata(new() { ["lvProducts"] = "ListView<Product>" });

        var result = _transform.Apply(input, metadata);

        Assert.Contains("private ListView<Product> lvProducts = default!;", result);
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
