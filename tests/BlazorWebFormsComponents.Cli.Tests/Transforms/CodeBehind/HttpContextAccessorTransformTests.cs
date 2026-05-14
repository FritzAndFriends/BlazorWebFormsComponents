using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class HttpContextAccessorTransformTests
{
    private readonly HttpContextAccessorTransform _sut = new();

    private static FileMetadata MakeMetadata(string source = "Logic/Test.cs") => new()
    {
        SourceFilePath = source,
        OutputFilePath = "Test.cs",
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    [Fact]
    public void SessionComparison_NotGarbled()
    {
        var input = @"using System;
public class CartActions
{
    public string GetCartId()
    {
        if (HttpContext.Current.Session[""CartKey""] == null)
        {
            HttpContext.Current.Session[""CartKey""] = Guid.NewGuid().ToString();
        }
        return HttpContext.Current.Session[""CartKey""].ToString();
    }
}";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldNotContain("SetString(\"CartKey\", = null");
        result.ShouldNotContain("SetString(\"CartKey\", == null");
        result.ShouldContain("GetString(\"CartKey\") == null");
    }

    [Fact]
    public void SessionAssignment_StillConverted()
    {
        var input = @"public class CartActions
{
    public void SetCart()
    {
        HttpContext.Current.Session[""CartKey""] = ""abc"";
    }
}";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldContain("SetString(\"CartKey\"");
    }

    [Fact]
    public void PageCodeBehind_Skipped()
    {
        var input = @"public class MyPage : WebFormsPageBase
{
    public void Test()
    {
        HttpContext.Current.Session[""key""] = ""val"";
    }
}";

        var result = _sut.Apply(input, MakeMetadata("Pages/MyPage.aspx.cs"));

        result.ShouldBe(input);
    }
}
