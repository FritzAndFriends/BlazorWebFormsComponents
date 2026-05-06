using System.Reflection;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms;

namespace BlazorWebFormsComponents.Cli.Tests;

public class SourceFileCopierTransformTests : IDisposable
{
    private readonly string _rootDir = Path.Combine(Path.GetTempPath(), $"bwfc-source-copy-{Guid.NewGuid():N}");

    public void Dispose()
    {
        if (Directory.Exists(_rootDir))
        {
            try { Directory.Delete(_rootDir, recursive: true); }
            catch { }
        }
    }

    [Fact]
    public async Task CopySourceFilesAsync_RewritesDbContextConstructors()
    {
        var inputDir = Path.Combine(_rootDir, "input");
        var outputDir = Path.Combine(_rootDir, "output");
        Directory.CreateDirectory(Path.Combine(inputDir, "Models"));
        Directory.CreateDirectory(outputDir);

        var sourcePath = Path.Combine(inputDir, "Models", "ProductContext.cs");
        await File.WriteAllTextAsync(sourcePath, """
            using System.Data.Entity;
            
            namespace TestApp.Models;
            
            public class ProductContext : DbContext
            {
                public ProductContext() : base("WingtipToys")
                {
                }
            }
            """);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        await copier.CopySourceFilesAsync(inputDir, outputDir, [], verbose: false, report);

        var copied = await File.ReadAllTextAsync(Path.Combine(outputDir, "Models", "ProductContext.cs"));
        Assert.Contains("DbContextOptions<ProductContext> options", copied);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", copied);
    }

    [Fact]
    public async Task CopySourceFilesAsync_RewritesHttpUtilityCalls()
    {
        var inputDir = Path.Combine(_rootDir, "input");
        var outputDir = Path.Combine(_rootDir, "output");
        Directory.CreateDirectory(Path.Combine(inputDir, "Logic"));
        Directory.CreateDirectory(outputDir);

        var sourcePath = Path.Combine(inputDir, "Logic", "PayPalFunctions.cs");
        await File.WriteAllTextAsync(sourcePath, """
            using System.Web;
            
            namespace TestApp.Logic;
            
            public class PayPalFunctions
            {
                public string Encode(string value) => HttpUtility.UrlEncode(value);
            }
            """);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        await copier.CopySourceFilesAsync(inputDir, outputDir, [], verbose: false, report);

        var copied = await File.ReadAllTextAsync(Path.Combine(outputDir, "Logic", "PayPalFunctions.cs"));
        Assert.Contains("using System.Net;", copied);
        Assert.DoesNotContain("using System.Web;", copied);
        Assert.Contains("WebUtility.UrlEncode(value)", copied);
    }

    private static IReadOnlyList<ICodeBehindTransform> GetCodeBehindTransforms()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var field = typeof(MigrationPipeline).GetField("_codeBehindTransforms", BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (IReadOnlyList<ICodeBehindTransform>)field.GetValue(pipeline)!;
    }
}
