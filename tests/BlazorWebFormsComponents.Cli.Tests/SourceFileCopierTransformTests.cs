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

    [Fact]
    public async Task CopySourceFilesAsync_RewritesHelperMapPathCalls()
    {
        var inputDir = Path.Combine(_rootDir, "input");
        var outputDir = Path.Combine(_rootDir, "output");
        Directory.CreateDirectory(Path.Combine(inputDir, "Logic"));
        Directory.CreateDirectory(outputDir);

        await File.WriteAllTextAsync(Path.Combine(inputDir, "Logic", "ExceptionUtility.cs"), """
            using System.Web;
            
            namespace TestApp.Logic;
            
            public static class ExceptionUtility
            {
                public static string GetLogFile()
                {
                    return HttpContext.Current.Server.MapPath("~/App_Data/ErrorLog.txt");
                }
            }
            """);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        await copier.CopySourceFilesAsync(inputDir, outputDir, [], verbose: false, report);

        var copied = await File.ReadAllTextAsync(Path.Combine(outputDir, "Logic", "ExceptionUtility.cs"));
        Assert.Contains("using System.IO;", copied);
        Assert.DoesNotContain("using System.Web;", copied);
        Assert.Contains("Path.Combine(AppContext.BaseDirectory, \"App_Data\", \"ErrorLog.txt\")", copied);
    }

    [Fact]
    public async Task CopySourceFilesAsync_RewritesLogicSelfInstantiationPattern()
    {
        var inputDir = Path.Combine(_rootDir, "input");
        var outputDir = Path.Combine(_rootDir, "output");
        Directory.CreateDirectory(Path.Combine(inputDir, "Logic"));
        Directory.CreateDirectory(outputDir);

        await File.WriteAllTextAsync(Path.Combine(inputDir, "Logic", "ShoppingCartActions.cs"), """
            namespace TestApp.Logic;
            
            public class ShoppingCartActions
            {
                public string ShoppingCartId { get; set; } = string.Empty;
            
                public ShoppingCartActions GetCart()
                {
                    using (var cart = new ShoppingCartActions())
                    {
                        cart.ShoppingCartId = "abc";
                        return cart;
                    }
                }
            }
            """);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        await copier.CopySourceFilesAsync(inputDir, outputDir, [], verbose: false, report);

        var copied = await File.ReadAllTextAsync(Path.Combine(outputDir, "Logic", "ShoppingCartActions.cs"));
        Assert.DoesNotContain("new ShoppingCartActions()", copied);
        Assert.DoesNotContain("using (var cart =", copied);
        Assert.Contains("this.ShoppingCartId = \"abc\";", copied);
        Assert.Contains("return this;", copied);
    }

    [Fact]
    public async Task CopySourceFilesAsync_AppliesConstructorInjectionToBllFileWithInlineDbContext()
    {
        // Verifies that DbContextInstantiationTransform runs on BLL/Logic files copied
        // by SourceFileCopier, replacing inline new XxxEntities() with constructor injection.
        // Reproduces the ContosoUniversity pattern where BLL classes use inline DbContext creation.
        var inputDir = Path.Combine(_rootDir, "input");
        var outputDir = Path.Combine(_rootDir, "output");
        Directory.CreateDirectory(Path.Combine(inputDir, "BLL"));
        Directory.CreateDirectory(outputDir);

        await File.WriteAllTextAsync(Path.Combine(inputDir, "BLL", "Students_Logic.cs"), """
            namespace TestApp.BLL;

            public class Students_Logic
            {
                public void DeleteStudent(int id)
                {
                    ContosoUniversityEntities db = new ContosoUniversityEntities();
                    db.Students.Remove(db.Students.First(s => s.StudentID == id));
                    db.SaveChanges();
                }
            }
            """);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        await copier.CopySourceFilesAsync(inputDir, outputDir, [], verbose: false, report);

        var copied = await File.ReadAllTextAsync(Path.Combine(outputDir, "BLL", "Students_Logic.cs"));
        Assert.DoesNotContain("new ContosoUniversityEntities()", copied);
        Assert.Contains("private readonly ContosoUniversityEntities", copied);
        // Original variable name 'db' is preserved as the field name
        Assert.Contains("db", copied);
        // Constructor injection: parameter added to constructor
        Assert.Contains("public Students_Logic(ContosoUniversityEntities", copied);
    }

    [Fact]
    public async Task CopySourceFilesAsync_RegistersParameterlessBllClassesForDi()
    {
        var inputDir = Path.Combine(_rootDir, "input");
        var outputDir = Path.Combine(_rootDir, "output");
        Directory.CreateDirectory(Path.Combine(inputDir, "BLL"));
        Directory.CreateDirectory(outputDir);

        await File.WriteAllTextAsync(Path.Combine(inputDir, "BLL", "Courses_Logic.cs"), """
            namespace TestApp.BLL;

            public class Courses_Logic
            {
                public int CountCourses() => 3;
            }
            """);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        var result = await copier.CopySourceFilesAsync(inputDir, outputDir, [], verbose: false, report);

        Assert.Contains(result.DiscoveredServiceClasses, svc => svc.ClassName == "Courses_Logic" && svc.Namespace == "TestApp.BLL");
    }

    [Fact]
    public async Task CopySourceFilesAsync_NormalizesExcludedPathsAndPreservesFolderCasing()
    {
        var inputDir = Path.Combine(_rootDir, "InputRoot");
        var outputDir = Path.Combine(_rootDir, "OutputRoot");
        Directory.CreateDirectory(Path.Combine(inputDir, "BLL"));
        Directory.CreateDirectory(Path.Combine(inputDir, "Models"));
        Directory.CreateDirectory(outputDir);

        var bllFile = Path.Combine(inputDir, "BLL", "Courses_Logic.cs");
        var excludedModel = Path.Combine(inputDir, "Models", "Model1.Context.cs");

        await File.WriteAllTextAsync(bllFile, "namespace TestApp.BLL; public class Courses_Logic { }");
        await File.WriteAllTextAsync(excludedModel, "namespace TestApp.Models; public class Model1Context { }");

        var relativeInputDir = Path.GetRelativePath(Directory.GetCurrentDirectory(), inputDir);
        var relativeOutputDir = Path.GetRelativePath(Directory.GetCurrentDirectory(), outputDir);
        var relativeExcludedModel = Path.GetRelativePath(Directory.GetCurrentDirectory(), excludedModel);

        var copier = new SourceFileCopier(new OutputWriter(), GetCodeBehindTransforms());
        var report = new MigrationReport();

        await copier.CopySourceFilesAsync(
            relativeInputDir,
            relativeOutputDir,
            [],
            verbose: false,
            report,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { relativeExcludedModel });

        Assert.True(File.Exists(Path.Combine(outputDir, "BLL", "Courses_Logic.cs")));
        Assert.False(File.Exists(Path.Combine(outputDir, "Models", "Model1.Context.cs")));
    }

    private static IReadOnlyList<ICodeBehindTransform> GetCodeBehindTransforms()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var field = typeof(MigrationPipeline).GetField("_codeBehindTransforms", BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (IReadOnlyList<ICodeBehindTransform>)field.GetValue(pipeline)!;
    }
}
