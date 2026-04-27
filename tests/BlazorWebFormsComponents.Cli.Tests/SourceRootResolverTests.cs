using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Tests;

public class SourceRootResolverTests : IDisposable
{
    private readonly string _tempDir;
    private readonly SourceRootResolver _resolver = new();

    public SourceRootResolverTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"bwfc-root-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (!Directory.Exists(_tempDir))
            return;

        try
        {
            Directory.Delete(_tempDir, recursive: true);
        }
        catch
        {
            // best effort cleanup
        }
    }

    [Fact]
    public void Resolve_UsesNestedAppFolder_WhenWrapperDirectoryContainsOnlyThatAppsMarkup()
    {
        var wrapperRoot = Path.Combine(_tempDir, "WingtipToys");
        var appRoot = Path.Combine(wrapperRoot, "WingtipToys");
        Directory.CreateDirectory(appRoot);
        Directory.CreateDirectory(Path.Combine(wrapperRoot, "packages"));
        File.WriteAllText(Path.Combine(wrapperRoot, "WingtipToys.sln"), string.Empty);
        File.WriteAllText(Path.Combine(appRoot, "Default.aspx"), "<%@ Page %>");

        var result = _resolver.Resolve(wrapperRoot);

        Assert.Equal(appRoot, result);
    }

    [Fact]
    public void Resolve_LeavesInputUnchanged_WhenMarkupExistsOutsideNestedAppFolder()
    {
        var wrapperRoot = Path.Combine(_tempDir, "WingtipToys");
        var appRoot = Path.Combine(wrapperRoot, "WingtipToys");
        Directory.CreateDirectory(appRoot);
        File.WriteAllText(Path.Combine(wrapperRoot, "Default.aspx"), "<%@ Page %>");
        File.WriteAllText(Path.Combine(appRoot, "About.aspx"), "<%@ Page %>");

        var result = _resolver.Resolve(wrapperRoot);

        Assert.Equal(wrapperRoot, result);
    }
}
