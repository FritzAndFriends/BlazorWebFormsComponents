using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for UsingStripTransform — strips Web Forms-specific using declarations
/// from code-behind files.
/// </summary>
public class UsingStripTransformTests
{
    private readonly UsingStripTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void Strips_SystemWebUI()
    {
        var input = "using System.Web.UI;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web.UI", result);
        Assert.Contains("namespace MyApp", result);
    }

    [Fact]
    public void Strips_SystemWebUIWebControls()
    {
        var input = "using System.Web.UI.WebControls;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web.UI.WebControls", result);
    }

    [Fact]
    public void Strips_SystemWebUIHtmlControls()
    {
        var input = "using System.Web.UI.HtmlControls;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web.UI.HtmlControls", result);
    }

    [Fact]
    public void Strips_SystemWebSecurity()
    {
        var input = "using System.Web.Security;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web.Security", result);
    }

    [Fact]
    public void Strips_SystemWeb()
    {
        var input = "using System.Web;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web", result);
    }

    [Fact]
    public void Strips_SystemWebExtensions()
    {
        var input = "using System.Web.Extensions;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("System.Web.Extensions", result);
    }

    [Fact]
    public void Strips_MicrosoftAspNet()
    {
        var input = "using Microsoft.AspNet.Identity;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Microsoft.AspNet", result);
    }

    [Fact]
    public void Strips_MicrosoftAspNetIdentityOwin()
    {
        var input = "using Microsoft.AspNet.Identity.Owin;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Microsoft.AspNet.Identity.Owin", result);
    }

    [Fact]
    public void Strips_MicrosoftOwin()
    {
        var input = "using Microsoft.Owin;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Microsoft.Owin", result);
    }

    [Fact]
    public void Strips_MicrosoftOwinSecurity()
    {
        var input = "using Microsoft.Owin.Security;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Microsoft.Owin.Security", result);
    }

    [Fact]
    public void Strips_BareOwin()
    {
        var input = "using Owin;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("using Owin;", result);
    }

    [Fact]
    public void PreservesNonWebUsings()
    {
        var input = @"using System;
using System.Linq;
using System.Collections.Generic;
namespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using System;", result);
        Assert.Contains("using System.Linq;", result);
        Assert.Contains("using System.Collections.Generic;", result);
    }

    [Fact]
    public void StripsMultipleWebUsings_PreservesOthers()
    {
        var input = @"using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Microsoft.AspNet.Identity;
using Owin;
namespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using System;", result);
        Assert.Contains("using System.Linq;", result);
        Assert.DoesNotContain("System.Web", result);
        Assert.DoesNotContain("Microsoft.AspNet", result);
        Assert.DoesNotContain("using Owin;", result);
    }

    [Fact]
    public void PreservesContent_WithNoUsings()
    {
        var input = "namespace MyApp { public class Foo { } }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void OrderIs100()
    {
        Assert.Equal(100, _transform.Order);
    }
}
