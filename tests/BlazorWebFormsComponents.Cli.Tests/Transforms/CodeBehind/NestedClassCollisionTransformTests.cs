using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class NestedClassCollisionTransformTests
{
    private readonly NestedClassCollisionTransform _sut = new();

    private static FileMetadata MakeMetadata() => new()
    {
        SourceFilePath = "Logic/Test.cs",
        OutputFilePath = "Logic/Test.cs",
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    [Fact]
    public void NestedSameName_IsRenamed()
    {
        var input = @"public class NVPAPICaller
{
    public void Call() { }

    public class NVPAPICaller
    {
        public string Value { get; set; }
    }
}";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldContain("class NVPAPICallerInner");
        var firstClassIndex = result.IndexOf("class NVPAPICaller", StringComparison.Ordinal);
        firstClassIndex.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void DifferentNames_NotModified()
    {
        var input = @"public class PayPalHelper
{
    public class NVPCodec { }
}";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldBe(input);
    }

    [Fact]
    public void SingleClass_NotModified()
    {
        const string input = @"public class MyClass { }";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldBe(input);
    }
}
