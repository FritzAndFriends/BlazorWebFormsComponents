using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.Markup;

public class MarkupCleanupOrphanTagTests
{
    private readonly MarkupCleanupTransform _transform = new();
    private static FileMetadata MakeMetadata() => new() { SourceFilePath = "Test.aspx", OutputFilePath = "Test.razor", FileType = FileType.Page, OriginalContent = "" };

    [Fact]
    public void Apply_OrphanClosingP_RemovesIt()
    {
        var input = "<div>\n<span>text</span>\n</p>\n</div>";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("</p>", result);
        Assert.Contains("</div>", result);
        Assert.Contains("<span>text</span>", result);
    }

    [Fact]
    public void Apply_BalancedTags_LeavesUnchanged()
    {
        var input = "<p>Hello</p>\n<div>World</div>";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("<p>Hello</p>", result);
        Assert.Contains("<div>World</div>", result);
    }

    [Fact]
    public void Apply_OrphanClosingDiv_RemovesIt()
    {
        var input = "<span>content</span>\n</div>";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("</div>", result);
        Assert.Contains("<span>content</span>", result);
    }

    [Fact]
    public void Apply_MultipleOrphans_RemovesAll()
    {
        var input = "<span>text</span>\n</p>\n</p>";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.DoesNotContain("</p>", result);
        Assert.Contains("<span>text</span>", result);
    }

    [Fact]
    public void Apply_OrphanOnLineWithOtherContent_LeavesIt()
    {
        // Only removes orphans that are alone on a line
        var input = "<span>text</span></p>";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("</p>", result);
    }

    [Fact]
    public void Apply_ClosingTagWithMatchingOpener_Preserved()
    {
        var input = "<p>First</p>\n<p>Second</p>\n</p>";
        var result = _transform.Apply(input, MakeMetadata());
        // 2 opens, 3 closes → remove 1 orphan
        Assert.Equal(2, CountOccurrences(result, "</p>"));
    }

    private static int CountOccurrences(string text, string pattern)
    {
        var count = 0;
        var idx = 0;
        while ((idx = text.IndexOf(pattern, idx, StringComparison.Ordinal)) >= 0)
        {
            count++;
            idx += pattern.Length;
        }
        return count;
    }
}
