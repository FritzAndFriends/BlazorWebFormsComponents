using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for DataBindTransform — converts DataSource assignments and DataBind() calls.
/// Also tests the static InjectItemsAttributes method for cross-file markup injection.
/// </summary>
public class DataBindTransformTests
{
    private readonly DataBindTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void RecordsDataSourceAssignment_InMetadata()
    {
        var input = @"gvStudents.DataSource = GetStudents();";
        var metadata = TestMetadata(input);

        _transform.Apply(input, metadata);

        Assert.True(metadata.DataBindMap.ContainsKey("gvStudents"));
        Assert.Equal("GetStudents()", metadata.DataBindMap["gvStudents"]);
    }

    [Fact]
    public void RecordsThisPrefixedDataSourceAssignment_InMetadata()
    {
        var input = @"this.gvStudents.DataSource = GetStudents();";
        var metadata = TestMetadata(input);

        _transform.Apply(input, metadata);

        Assert.True(metadata.DataBindMap.ContainsKey("gvStudents"));
        Assert.Equal("GetStudents()", metadata.DataBindMap["gvStudents"]);
    }

    [Fact]
    public void RemovesDataBindCall()
    {
        var input = @"gvStudents.DataSource = GetStudents();
gvStudents.DataBind();
var x = 42;";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("DataBind()", result);
        Assert.Contains("var x = 42;", result);
    }

    [Fact]
    public void RemovesThisPrefixedDataBindCall_WithoutLeavingDanglingThis()
    {
        var input = """
            this.gvStudents.DataBind();
            var x = 42;
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("DataBind()", result);
        Assert.DoesNotContain("this.", result);
        Assert.Contains("var x = 42;", result);
    }

    [Fact]
    public void RemovesMultipleDataBindCalls()
    {
        var input = @"gvStudents.DataSource = students;
gvStudents.DataBind();
ddlDepts.DataSource = depts;
ddlDepts.DataBind();";
        var metadata = TestMetadata(input);
        var result = _transform.Apply(input, metadata);

        Assert.DoesNotContain("DataBind()", result);
        Assert.Equal(2, metadata.DataBindMap.Count);
    }

    [Fact]
    public void PreservesContent_WithoutDataBindPatterns()
    {
        var input = @"namespace MyApp
{
    public partial class Default
    {
        void DoWork() { var x = 42; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void PreservesDataSourceAssignment_LineInOutput()
    {
        var input = @"gvStudents.DataSource = GetStudents();";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("gvStudents.DataSource = GetStudents();", result);
    }

    [Fact]
    public void PreservesThisPrefixedDataSourceAssignment_LineInOutput()
    {
        var input = @"this.gvStudents.DataSource = GetStudents();";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("this.gvStudents.DataSource = GetStudents();", result);
    }

    // --- InjectItemsAttributes static method ---

    [Fact]
    public void InjectItemsAttributes_InjectsIntoMarkup()
    {
        var markup = @"<DropDownList id=""ddlDepts"" />";
        var map = new Dictionary<string, string> { ["ddlDepts"] = "departments" };

        var result = DataBindTransform.InjectItemsAttributes(markup, map);

        Assert.Contains(@"Items=""@(departments)""", result);
    }

    [Fact]
    public void InjectItemsAttributes_MultipleControls()
    {
        var markup = @"<DropDownList id=""ddlDepts"" />
<GridView id=""gvStudents"" />";
        var map = new Dictionary<string, string>
        {
            ["ddlDepts"] = "departments",
            ["gvStudents"] = "GetStudents()"
        };

        var result = DataBindTransform.InjectItemsAttributes(markup, map);

        Assert.Contains(@"Items=""@(departments)""", result);
        Assert.Contains(@"Items=""@(GetStudents())""", result);
    }

    [Fact]
    public void InjectItemsAttributes_NoMatchingControl_Unchanged()
    {
        var markup = @"<DropDownList id=""ddlOther"" />";
        var map = new Dictionary<string, string> { ["ddlMissing"] = "data" };

        var result = DataBindTransform.InjectItemsAttributes(markup, map);

        Assert.Equal(markup, result);
    }

    [Fact]
    public void InjectItemsAttributes_EmptyMap_Unchanged()
    {
        var markup = @"<DropDownList id=""ddl1"" />";
        var result = DataBindTransform.InjectItemsAttributes(markup, new Dictionary<string, string>());

        Assert.Equal(markup, result);
    }

    [Fact]
    public void OrderIs800()
    {
        Assert.Equal(800, _transform.Order);
    }
}
