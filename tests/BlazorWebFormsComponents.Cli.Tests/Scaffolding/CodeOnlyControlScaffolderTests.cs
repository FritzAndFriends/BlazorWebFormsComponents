using BlazorWebFormsComponents.Cli.Analysis;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests.Scaffolding;

public sealed class CodeOnlyControlScaffolderTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", $"code-only-scaffold-{Guid.NewGuid():N}");

    public CodeOnlyControlScaffolderTests()
    {
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public async Task EmitAsync_WritesRazorSkeletonFiles()
    {
        var writer = new OutputWriter();
        var scaffolder = new CodeOnlyControlScaffolder();
        var controls = new[]
        {
            new CodeOnlyServerControlDescriptor
            {
                ClassName = "FancyCalendar",
                Namespace = "Contoso.Controls",
                BaseType = "WebControl",
                SourceFilePath = Path.Combine("Controls", "FancyCalendar.cs"),
                TagPrefixes = ["cc1"]
            }
        };

        var written = await scaffolder.EmitAsync(_tempDirectory, "MigratedApp", controls, writer);

        Assert.Equal(2, written);
        Assert.True(File.Exists(Path.Combine(_tempDirectory, "Generated", "CodeOnlyControls", "FancyCalendar.razor")));
        var codeBehind = Path.Combine(_tempDirectory, "Generated", "CodeOnlyControls", "FancyCalendar.razor.cs");
        Assert.True(File.Exists(codeBehind));
        Assert.Contains("Registered tag prefixes: cc1", await File.ReadAllTextAsync(codeBehind));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            try
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
            catch
            {
            }
        }
    }
}
