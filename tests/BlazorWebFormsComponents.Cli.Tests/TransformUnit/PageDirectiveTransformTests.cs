using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Directives;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class PageDirectiveTransformTests
{
    private readonly PageDirectiveTransform _transform = new();

    [Fact]
    public void Apply_EmitsDirectoryAndFileNameRoutesForNestedAccountPage()
    {
        var result = _transform.Apply(PageDirective, CreateMetadata(@"D:\input\Account\Login.aspx"));

        Assert.StartsWith("@page \"/Account/Login\"\n@page \"/Login\"\n", result);
    }

    [Fact]
    public void Apply_EmitsDirectoryAndFileNameRoutesForNestedAdminPage()
    {
        var result = _transform.Apply(PageDirective, CreateMetadata(@"D:\input\Admin\Dashboard.aspx"));

        Assert.StartsWith("@page \"/Admin/Dashboard\"\n@page \"/Dashboard\"\n", result);
    }

    [Fact]
    public void Apply_LeavesRootLevelPageAsSingleRoute()
    {
        var result = _transform.Apply(PageDirective, CreateMetadata(@"D:\input\About.aspx"));

        Assert.Equal("@page \"/About\"\n<div>Hello</div>", result);
    }

    [Fact]
    public void Apply_LeavesDefaultPageHomeRouteUnchanged()
    {
        var result = _transform.Apply(PageDirective, CreateMetadata(@"D:\input\Default.aspx"));

        Assert.Equal("@page \"/\"\n<div>Hello</div>", result);
    }

    private static FileMetadata CreateMetadata(string sourceFilePath) => new()
    {
        SourceFilePath = sourceFilePath,
        SourceRootPath = @"D:\input",
        OutputFilePath = Path.ChangeExtension(sourceFilePath.Replace(@"D:\input", @"D:\output"), ".razor"),
        FileType = FileType.Page,
        OriginalContent = PageDirective
    };

    private const string PageDirective = "<%@ Page Language=\"C#\" %>\n<div>Hello</div>";
}
