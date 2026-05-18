using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for IsPostBackTransform — unwraps guards, extracts else-branch,
/// and adds TODO for else-if patterns.
/// </summary>
public class IsPostBackTransformTests
{
    private readonly IsPostBackTransform _transform = new();
    private readonly FileMetadata _metadata = new()
    {
        SourceFilePath = "test.aspx.cs",
        OutputFilePath = "test.razor.cs",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void SimpleGuard_UnwrapsIfBody()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!IsPostBack)
            {
                Init();
            }
        }
    }
}";

        var result = _transform.Apply(input, _metadata);

        Assert.Contains("// BWFC: IsPostBack guard unwrapped", result);
        Assert.Contains("Init();", result);
        Assert.DoesNotContain("if (!IsPostBack)", result);
    }

    [Fact]
    public void ElseBranch_UnwrapsIfBody_And_ExtractsHandlePostBack()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!IsPostBack)
            {
                Init();
            }
            else
            {
                ProcessPostBack();
            }
        }
    }
}";

        var result = _transform.Apply(input, _metadata);

        // If body should be unwrapped
        Assert.Contains("// BWFC: IsPostBack guard unwrapped", result);
        Assert.Contains("Init();", result);

        // Else body should be extracted to HandlePostBack
        Assert.Contains("private void HandlePostBack()", result);
        Assert.Contains("ProcessPostBack();", result);
        Assert.Contains("// TODO(bwfc-ispostback): Wire HandlePostBack()", result);

        // Original if-else should be gone
        Assert.DoesNotContain("if (!IsPostBack)", result);
        Assert.DoesNotContain("else", result);
    }

    [Fact]
    public void ElseBranch_MultipleStatements_AllExtracted()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!IsPostBack)
            {
                Init();
            }
            else
            {
                ValidateForm();
                SubmitData();
            }
        }
    }
}";

        var result = _transform.Apply(input, _metadata);

        Assert.Contains("private void HandlePostBack()", result);
        Assert.Contains("ValidateForm();", result);
        Assert.Contains("SubmitData();", result);
    }

    [Fact]
    public void ElseIf_AddsTodoAndPreservesCode()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!IsPostBack)
            {
                Init();
            }
            else if (someCondition)
            {
                Other();
            }
        }
    }
}";

        var result = _transform.Apply(input, _metadata);

        // Should add TODO for else-if
        Assert.Contains("// TODO(bwfc-ispostback): IsPostBack guard with else-if clause", result);
        Assert.Contains("too complex for automated extraction", result);

        // Guard should be replaced to prevent re-matching
        Assert.Contains("if (true /* BWFC: was !IsPostBack */)", result);

        // Original else-if branch should be preserved
        Assert.Contains("else if (someCondition)", result);
        Assert.Contains("Other();", result);

        // Should NOT create HandlePostBack for else-if
        Assert.DoesNotContain("HandlePostBack", result);
    }

    [Fact]
    public void SingleLineElse_ExtractsStatement()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!IsPostBack)
            {
                Init();
            }
            else
                ProcessPostBack();
        }
    }
}";

        var result = _transform.Apply(input, _metadata);

        Assert.Contains("private void HandlePostBack()", result);
        Assert.Contains("ProcessPostBack();", result);
        Assert.Contains("// BWFC: IsPostBack guard unwrapped", result);
    }

    [Fact]
    public void PageIsPostBack_Variant_HandledCorrectly()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!Page.IsPostBack)
            {
                Init();
            }
            else
            {
                PostBack();
            }
        }
    }
}";

        var result = _transform.Apply(input, _metadata);

        Assert.Contains("private void HandlePostBack()", result);
        Assert.Contains("PostBack();", result);
        Assert.DoesNotContain("if (!Page.IsPostBack)", result);
    }

    [Fact]
    public void HandlePostBack_IndentedCorrectly()
    {
        var input = @"namespace MyApp
{
    public class Foo
    {
        void Page_Load()
        {
            if (!IsPostBack)
            {
                Init();
            }
            else
            {
                PostBack();
            }
        }
    }
}";

        var result = _transform.Apply(input, _metadata);
        var normalized = result.Replace("\r\n", "\n");
        var lines = normalized.Split('\n');

        // Find HandlePostBack declaration line
        var methodLine = lines.FirstOrDefault(l => l.Contains("private void HandlePostBack()"));
        Assert.NotNull(methodLine);

        // Method should be at member-level indent (8 spaces for standard 4-space indent)
        Assert.StartsWith("        ", methodLine);
    }
}
