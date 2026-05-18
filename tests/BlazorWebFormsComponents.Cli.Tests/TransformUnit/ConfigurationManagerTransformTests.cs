using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ConfigurationManagerTransform — strips System.Configuration using,
/// detects ConfigurationManager.AppSettings/ConnectionStrings patterns, and emits guidance.
/// </summary>
public class ConfigurationManagerTransformTests
{
    private readonly ConfigurationManagerTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void StripsSystemConfigurationUsing()
    {
        var input = @"using System;
using System.Configuration;
using System.Web;

namespace MyApp
{
    public partial class MyPage
    {
        void Process() { }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("// using System.Configuration;", result);
        Assert.Contains("BWFC provides ConfigurationManager shim", result);
        Assert.DoesNotContain("\nusing System.Configuration;\n", result);
    }

    [Fact]
    public void DetectsAppSettings_AddsGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var connStr = ConfigurationManager.AppSettings[""SiteName""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-config)", result);
        Assert.Contains("UseConfigurationManagerShim", result);
        Assert.Contains("SiteName", result);
    }

    [Fact]
    public void DetectsConnectionStrings_AddsGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var cs = ConfigurationManager.ConnectionStrings[""DefaultConnection""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-config)", result);
        Assert.Contains("DefaultConnection", result);
        Assert.Contains("ConnectionStrings", result);
    }

    [Fact]
    public void DetectsBothPatterns_ListsAllKeys()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var site = ConfigurationManager.AppSettings[""SiteName""];
            var env = ConfigurationManager.AppSettings[""Environment""];
            var cs = ConfigurationManager.ConnectionStrings[""DefaultConnection""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("SiteName", result);
        Assert.Contains("Environment", result);
        Assert.Contains("DefaultConnection", result);
    }

    [Fact]
    public void NoConfigurationManager_NoChanges()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process() { var x = 42; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void Idempotent_DoesNotDuplicateGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var cs = ConfigurationManager.AppSettings[""Key""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("ConfigurationManager Migration").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void OrderIs110()
    {
        Assert.Equal(110, _transform.Order);
    }
}
