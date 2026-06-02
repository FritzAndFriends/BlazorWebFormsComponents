using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for BaseClassStripTransform — replaces Web Forms base class
/// declarations with WebFormsPageBase for Page/Master types, or strips for Control/CodeFile.
/// </summary>
public class BaseClassStripTransformTests
{
    private readonly BaseClassStripTransform _transform = new();

    private static FileMetadata PageMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    private static FileMetadata MasterMetadata(string content) => new()
    {
        SourceFilePath = "Site.Master.cs",
        OutputFilePath = "MainLayout.razor.cs",
        FileType = FileType.Master,
        OriginalContent = content
    };

    private static FileMetadata ControlMetadata(string content) => new()
    {
        SourceFilePath = "MyControl.ascx.cs",
        OutputFilePath = "MyControl.razor.cs",
        FileType = FileType.Control,
        OriginalContent = content
    };

    // --- Page type: replaces with WebFormsPageBase ---

    [Fact]
    public void Page_ReplacesSystemWebUIPage_WithWebFormsPageBase()
    {
        var input = "public partial class MyPage : System.Web.UI.Page { }";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.Contains("public partial class MyPage : WebFormsPageBase { }", result);
        Assert.DoesNotContain("System.Web.UI.Page", result);
    }

    [Fact]
    public void Page_ReplacesShortFormPage_WithWebFormsPageBase()
    {
        var input = "public partial class Default : Page { }";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.Contains("public partial class Default : WebFormsPageBase { }", result);
        Assert.DoesNotContain(": Page {", result);
    }

    [Fact]
    public void Page_ReplacesComponentBase_WithWebFormsPageBase()
    {
        var input = "public partial class Default : ComponentBase { }";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.Contains("public partial class Default : WebFormsPageBase { }", result);
        Assert.DoesNotContain(": ComponentBase", result);
    }

    [Fact]
    public void Page_ReplacesFullyQualifiedComponentBase_WithWebFormsPageBase()
    {
        var input = "public partial class Default : Microsoft.AspNetCore.Components.ComponentBase { }";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.Contains("public partial class Default : WebFormsPageBase { }", result);
        Assert.DoesNotContain("ComponentBase", result);
    }

    // --- Master type: replaces with WebFormsPageBase ---

    [Fact]
    public void Master_ReplacesSystemWebUIMasterPage_WithWebFormsPageBase()
    {
        var input = "public partial class SiteMaster : System.Web.UI.MasterPage { }";
        var result = _transform.Apply(input, MasterMetadata(input));

        Assert.Contains("public partial class SiteMaster : WebFormsPageBase { }", result);
        Assert.DoesNotContain("MasterPage", result);
    }

    [Fact]
    public void Master_ReplacesShortFormMasterPage_WithWebFormsPageBase()
    {
        var input = "public partial class SiteMaster : MasterPage { }";
        var result = _transform.Apply(input, MasterMetadata(input));

        Assert.Contains("public partial class SiteMaster : WebFormsPageBase { }", result);
    }

    // --- Control type: preserves WebControl/UserControl/CompositeControl (BWFC equivalents exist) ---

    [Fact]
    public void Control_PreservesUserControlFromFullyQualified()
    {
        var input = "public partial class MyControl : System.Web.UI.UserControl { }";
        var result = _transform.Apply(input, ControlMetadata(input));

        Assert.Contains("public partial class MyControl : UserControl", result);
        Assert.DoesNotContain("System.Web.UI", result);
    }

    [Fact]
    public void Control_PreservesShortFormUserControl()
    {
        var input = "public partial class MyCtrl : UserControl { }";
        var result = _transform.Apply(input, ControlMetadata(input));

        Assert.Contains("public partial class MyCtrl : UserControl", result);
    }

    // --- Shared behavior ---

    [Fact]
    public void PreservesContent_WithoutBaseClass()
    {
        var input = "public partial class MyPage { void DoWork() { } }";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void PreservesNonWebFormsBaseClass()
    {
        var input = "public partial class MyPage : SomeCustomBase { }";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void Page_ReplacesBaseClass_InFullClassContext()
    {
        var input = @"namespace MyApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }
    }
}";
        var result = _transform.Apply(input, PageMetadata(input));

        Assert.DoesNotContain("System.Web.UI.Page", result);
        Assert.Contains("public partial class Default : WebFormsPageBase", result);
        Assert.Contains("Page_Load", result);
    }

    [Fact]
    public void OrderIs200()
    {
        Assert.Equal(200, _transform.Order);
    }
}
