using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for AttributeNormalizeTransform — normalizes boolean, enum, and unit
/// attribute values in converted Blazor markup.
/// </summary>
public class AttributeNormalizeTransformTests
{
    private readonly AttributeNormalizeTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    // --- Boolean normalization ---

    [Fact]
    public void NormalizesTrue_ToLowercase()
    {
        var input = @"<Button Visible=""True"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Visible=""true""", result);
    }

    [Fact]
    public void NormalizesFalse_ToLowercase()
    {
        var input = @"<TextBox Enabled=""False"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Enabled=""false""", result);
    }

    [Fact]
    public void SkipsBooleanNormalization_ForTextAttributes()
    {
        var input = @"<Label Text=""True"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Text=""True""", result);
    }

    [Fact]
    public void SkipsBooleanNormalization_ForTitle()
    {
        var input = @"<Panel Title=""False"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Title=""False""", result);
    }

    [Fact]
    public void SkipsBooleanNormalization_ForHeaderText()
    {
        var input = @"<BoundField HeaderText=""True"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"HeaderText=""True""", result);
    }

    // --- Enum normalization ---

    [Fact]
    public void QualifiesEnum_GridLines()
    {
        var input = @"<GridView GridLines=""Both"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"GridLines=""@GridLines.Both""", result);
    }

    [Fact]
    public void QualifiesEnum_TextMode()
    {
        var input = @"<TextBox TextMode=""MultiLine"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"TextMode=""@TextBoxMode.MultiLine""", result);
    }

    [Fact]
    public void QualifiesEnum_HorizontalAlign()
    {
        var input = @"<TableCell HorizontalAlign=""Center"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"HorizontalAlign=""@HorizontalAlign.Center""", result);
    }

    [Fact]
    public void DoesNotQualifyEnum_WhenValueIsBooleanTrue()
    {
        var input = @"<Panel GridLines=""True"" />";
        var result = _transform.Apply(input, TestMetadata);

        // "True" is a boolean value, not an enum member, so it should not be @GridLines.True
        Assert.DoesNotContain(@"@GridLines.True", result);
    }

    // --- Unit normalization ---

    [Fact]
    public void StripsPx_FromWidth()
    {
        var input = @"<Panel Width=""100px"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Width=""100""", result);
        Assert.DoesNotContain("px", result);
    }

    [Fact]
    public void StripsPx_FromHeight()
    {
        var input = @"<Panel Height=""200px"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Height=""200""", result);
    }

    [Fact]
    public void StripsPx_FromBorderWidth()
    {
        var input = @"<Panel BorderWidth=""2px"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"BorderWidth=""2""", result);
    }

    [Fact]
    public void StripsPx_FromCellPadding()
    {
        var input = @"<GridView CellPadding=""5px"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"CellPadding=""5""", result);
    }

    // --- No-op ---

    [Fact]
    public void PreservesContent_WithNoNormalizableAttributes()
    {
        var input = @"<div class=""wrapper""><span>Hello world</span></div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void HandlesMultipleAttributesOnSameElement()
    {
        var input = @"<GridView Visible=""True"" GridLines=""Both"" Width=""500px"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Visible=""true""", result);
        Assert.Contains(@"GridLines=""@GridLines.Both""", result);
        Assert.Contains(@"Width=""500""", result);
    }

    [Fact]
    public void OrderIs810()
    {
        Assert.Equal(810, _transform.Order);
    }

    // --- Additional unit suffix tests ---

    [Fact]
    public void StripsEm_FromWidth()
    {
        var input = @"<Panel Width=""10em"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Width=""10""", result);
    }

    [Fact]
    public void StripsPt_FromHeight()
    {
        var input = @"<Panel Height=""14pt"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Height=""14""", result);
    }

    [Fact]
    public void StripsPercent_FromWidth()
    {
        var input = @"<Panel Width=""100%"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Width=""100""", result);
    }

    // --- Hyphenated attribute removal ---

    [Fact]
    public void RemovesFontSize_HyphenatedAttribute()
    {
        var input = @"<GridView Font-Size=""30px"" CellPadding=""5"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("Font-Size", result);
        Assert.Contains(@"CellPadding=""5""", result);
    }

    [Fact]
    public void RemovesFontWeight_HyphenatedAttribute()
    {
        var input = @"<Label Font-Weight=""Bold"" Text=""Hello"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("Font-Weight", result);
        Assert.Contains(@"Text=""Hello""", result);
    }

    [Fact]
    public void RemovesMultipleHyphenatedAttributes()
    {
        var input = @"<Panel Font-Size=""12px"" Font-Style=""Italic"" Width=""100"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("Font-Size", result);
        Assert.DoesNotContain("Font-Style", result);
        Assert.Contains(@"Width=""100""", result);
    }
}
