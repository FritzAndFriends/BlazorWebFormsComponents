using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for BaseClassStripTransform — removes Web Forms base class
/// declarations from code-behind partial classes.
/// </summary>
public class BaseClassStripTransformTests
{
    private readonly BaseClassStripTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void Strips_SystemWebUIPage()
    {
        var input = "public partial class MyPage : System.Web.UI.Page { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class MyPage { }", result);
        Assert.DoesNotContain("System.Web.UI.Page", result);
    }

    [Fact]
    public void Strips_SystemWebUIMasterPage()
    {
        var input = "public partial class SiteMaster : System.Web.UI.MasterPage { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class SiteMaster { }", result);
        Assert.DoesNotContain("MasterPage", result);
    }

    [Fact]
    public void Strips_SystemWebUIUserControl()
    {
        var input = "public partial class MyControl : System.Web.UI.UserControl { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class MyControl { }", result);
        Assert.DoesNotContain("UserControl", result);
    }

    [Fact]
    public void Strips_ShortFormPage()
    {
        var input = "public partial class Default : Page { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class Default { }", result);
        Assert.DoesNotContain(": Page", result);
    }

    [Fact]
    public void Strips_ShortFormMasterPage()
    {
        var input = "public partial class SiteMaster : MasterPage { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class SiteMaster { }", result);
    }

    [Fact]
    public void Strips_ShortFormUserControl()
    {
        var input = "public partial class MyCtrl : UserControl { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class MyCtrl { }", result);
    }

    [Fact]
    public void PreservesContent_WithoutBaseClass()
    {
        var input = "public partial class MyPage { void DoWork() { } }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void Strips_ComponentBase()
    {
        var input = "public partial class Default : ComponentBase { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class Default { }", result);
        Assert.DoesNotContain(": ComponentBase", result);
    }

    [Fact]
    public void Strips_FullyQualifiedComponentBase()
    {
        var input = "public partial class Default : Microsoft.AspNetCore.Components.ComponentBase { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("public partial class Default { }", result);
        Assert.DoesNotContain("ComponentBase", result);
    }

    [Fact]
    public void PreservesNonWebFormsBaseClass()
    {
        var input = "public partial class MyPage : SomeCustomBase { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void StripsBaseClass_InFullClassContext()
    {
        var input = @"namespace MyApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web.UI.Page", result);
        Assert.Contains("public partial class Default", result);
        Assert.Contains("Page_Load", result);
    }

    [Fact]
    public void OrderIs200()
    {
        Assert.Equal(200, _transform.Order);
    }
}
