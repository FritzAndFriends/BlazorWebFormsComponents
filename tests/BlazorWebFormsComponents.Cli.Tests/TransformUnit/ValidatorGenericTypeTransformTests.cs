using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class ValidatorGenericTypeTransformTests
{
    private readonly ValidatorGenericTypeTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void AddsObjectType_ToRequiredFieldValidator_WhenNoControlTypeHintExists()
    {
        var input = "<RequiredFieldValidator ControlToValidate=\"Email\" />";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<RequiredFieldValidator Type=\"object\" ControlToValidate=\"Email\" />", result);
    }

    [Fact]
    public void AddsStringType_ToRequiredFieldValidator_WhenTextBoxIsValidated()
    {
        var input = "<TextBox ID=\"Email\" /><RequiredFieldValidator ControlToValidate=\"Email\" />";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<RequiredFieldValidator Type=\"string\" ControlToValidate=\"Email\" />", result);
    }

    [Fact]
    public void AddsInputType_ToCompareValidator_WhenMissing()
    {
        var input = "<CompareValidator ControlToValidate=\"ConfirmPassword\" />";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<CompareValidator InputType=\"string\" ControlToValidate=\"ConfirmPassword\" />", result);
    }

    [Fact]
    public void AddsInputType_ToRangeValidator_WhenMissing()
    {
        var input = "<RangeValidator MinimumValue=\"1\" MaximumValue=\"9\" />";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<RangeValidator InputType=\"string\" MinimumValue=\"1\" MaximumValue=\"9\" />", result);
    }

    [Fact]
    public void DoesNotDuplicateExistingGenericArgument()
    {
        var input = "<RequiredFieldValidator Type=\"int?\" ControlToValidate=\"Quantity\" />";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }
}
