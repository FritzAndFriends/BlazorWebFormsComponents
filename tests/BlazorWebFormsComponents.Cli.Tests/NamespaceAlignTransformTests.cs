using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests;

public class NamespaceAlignTransformTests
{
    [Fact]
    public void NamespaceAlignTransform_UsesOutputPathRelativeToProjectRoot()
    {
        var transform = new NamespaceAlignTransform();
        var content = """
namespace WingtipToys.Account
{
    public partial class Login
    {
    }
}
""";

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\source\WingtipToys\Account\Login.aspx",
            OutputFilePath = @"D:\output\Account\Login.razor",
            OutputRootPath = @"D:\output",
            ProjectNamespace = "WingtipToys",
            FileType = FileType.Page,
            OriginalContent = content
        };

        var result = transform.Apply(content, metadata);

        Assert.Contains("namespace WingtipToys.Account", result);
    }

    [Fact]
    public void NamespaceAlignTransform_LeavesContentUnchanged_WhenProjectContextMissing()
    {
        var transform = new NamespaceAlignTransform();
        var content = """
namespace WingtipToys.Account;

public partial class Login
{
}
""";

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\source\WingtipToys\Account\Login.aspx",
            OutputFilePath = @"D:\output\WingtipToys\Account\Login.razor",
            FileType = FileType.Page,
            OriginalContent = content
        };

        var result = transform.Apply(content, metadata);

        Assert.Equal(content, result);
    }

    [Fact]
    public void NamespaceAlignTransform_NormalizesProjectNamespaceReferencesToSourceFolderCasing()
    {
        var transform = new NamespaceAlignTransform();
        var sourceRoot = Path.Combine(Path.GetTempPath(), $"bwfc-ns-align-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(sourceRoot, "BLL"));

        try
        {
            var content = """
using TestApp.Bll;

namespace TestApp;

public partial class Students
{
}
""";

            var metadata = new FileMetadata
            {
                SourceFilePath = Path.Combine(sourceRoot, "Students.aspx.cs"),
                SourceRootPath = sourceRoot,
                OutputFilePath = Path.Combine(sourceRoot, "output", "Students.razor"),
                OutputRootPath = Path.Combine(sourceRoot, "output"),
                ProjectNamespace = "TestApp",
                FileType = FileType.Page,
                OriginalContent = content
            };

            var result = transform.Apply(content, metadata);

            Assert.Contains("using TestApp.BLL;", result);
            Assert.Contains("namespace TestApp;", result);
            Assert.DoesNotContain("TestApp.Bll", result);
        }
        finally
        {
            Directory.Delete(sourceRoot, recursive: true);
        }
    }
}
