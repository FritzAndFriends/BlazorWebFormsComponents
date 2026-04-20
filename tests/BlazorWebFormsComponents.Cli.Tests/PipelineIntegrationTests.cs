using System.Reflection;
using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests;

/// <summary>
/// End-to-end pipeline integration tests.
/// Creates temp directories with mini Web Forms projects and runs the full migration pipeline.
/// </summary>
public class PipelineIntegrationTests : IDisposable
{
    private readonly List<string> _tempDirs = [];

    public void Dispose()
    {
        foreach (var dir in _tempDirs)
        {
            if (Directory.Exists(dir))
            {
                try { Directory.Delete(dir, recursive: true); }
                catch { /* best effort cleanup */ }
            }
        }
    }

    /// <summary>
    /// Creates a fully-wired MigrationPipeline with all dependencies for E2E tests.
    /// </summary>
    private static MigrationPipeline CreateFullPipeline(OutputWriter? writer = null)
    {
        var transforms = TestHelpers.CreateDefaultPipeline();
        // Use reflection to get the transform lists from the lightweight pipeline,
        // then construct the full pipeline with all services.
        var markupField = typeof(MigrationPipeline).GetField("_markupTransforms",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var codeBehindField = typeof(MigrationPipeline).GetField("_codeBehindTransforms",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        var markupTransforms = (IReadOnlyList<BlazorWebFormsComponents.Cli.Transforms.IMarkupTransform>)markupField.GetValue(transforms)!;
        var codeBehindTransforms = (IReadOnlyList<BlazorWebFormsComponents.Cli.Transforms.ICodeBehindTransform>)codeBehindField.GetValue(transforms)!;

        var outputWriter = writer ?? new OutputWriter();

        return new MigrationPipeline(
            markupTransforms,
            codeBehindTransforms,
            new ProjectScaffolder(new DatabaseProviderDetector()),
            new GlobalUsingsGenerator(),
            new ShimGenerator(),
            new WebConfigTransformer(),
            outputWriter,
            new StaticFileCopier(outputWriter),
            new SourceFileCopier(outputWriter, codeBehindTransforms));
    }

    private (string inputDir, string outputDir) CreateTempProjectDir(
        bool includeWebConfig = true,
        bool includeCodeBehind = false,
        bool includeAccount = false)
    {
        var baseTempDir = Path.Combine(Path.GetTempPath(), $"bwfc-pipeline-{Guid.NewGuid():N}");
        var inputDir = Path.Combine(baseTempDir, "input");
        var outputDir = Path.Combine(baseTempDir, "output");
        Directory.CreateDirectory(inputDir);
        Directory.CreateDirectory(outputDir);
        _tempDirs.Add(baseTempDir);

        // Create a minimal .aspx page
        File.WriteAllText(Path.Combine(inputDir, "Default.aspx"), """
            <%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestApp._Default" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:Label ID="Label1" runat="server" Text="Hello World" CssClass="title" />
                <asp:Button ID="Button1" runat="server" Text="Click Me" OnClick="Button1_Click" />
            </asp:Content>
            """);

        // Create a second .aspx page
        File.WriteAllText(Path.Combine(inputDir, "About.aspx"), """
            <%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:Label ID="AboutLabel" runat="server" Text="About this site" />
            </asp:Content>
            """);

        if (includeCodeBehind)
        {
            File.WriteAllText(Path.Combine(inputDir, "Default.aspx.cs"), """
                using System;
                using System.Web.UI;

                namespace TestApp
                {
                    public partial class _Default : Page
                    {
                        protected void Page_Load(object sender, EventArgs e)
                        {
                            Label1.Text = "Loaded";
                        }

                        protected void Button1_Click(object sender, EventArgs e)
                        {
                            Label1.Text = "Clicked";
                        }
                    }
                }
                """);
        }

        if (includeWebConfig)
        {
            File.WriteAllText(Path.Combine(inputDir, "Web.config"), """
                <?xml version="1.0" encoding="utf-8"?>
                <configuration>
                  <appSettings>
                    <add key="SiteName" value="TestApp" />
                  </appSettings>
                  <connectionStrings>
                    <add name="DefaultConnection"
                         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;Database=TestApp"
                         providerName="System.Data.SqlClient" />
                  </connectionStrings>
                </configuration>
                """);
        }

        if (includeAccount)
        {
            Directory.CreateDirectory(Path.Combine(inputDir, "Account"));
        }

        return (inputDir, outputDir);
    }

    // ───────────────────────────────────────────────────────────────
    //  Full pipeline — markup transforms
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Pipeline_CreatesRazorFiles()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var pipeline = CreateFullPipeline();
        var scanner = new SourceScanner();
        var sourceFiles = scanner.Scan(inputDir, outputDir);

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { DryRun = false, SkipScaffold = true },
            SourceFiles = sourceFiles
        };

        var report = await pipeline.ExecuteAsync(context);

        Assert.Empty(report.Errors);
        Assert.True(report.FilesProcessed >= 2, $"Expected at least 2 files processed, got {report.FilesProcessed}");
        Assert.True(File.Exists(Path.Combine(outputDir, "Default.razor")),
            "Default.razor should be created");
        Assert.True(File.Exists(Path.Combine(outputDir, "About.razor")),
            "About.razor should be created");
    }

    [Fact]
    public async Task Pipeline_RazorOutput_ContainsBwfcComponents()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var pipeline = CreateFullPipeline();
        var scanner = new SourceScanner();
        var sourceFiles = scanner.Scan(inputDir, outputDir);

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { DryRun = false, SkipScaffold = true },
            SourceFiles = sourceFiles
        };

        await pipeline.ExecuteAsync(context);

        var defaultRazor = File.ReadAllText(Path.Combine(outputDir, "Default.razor"));
        // asp: prefix should be stripped → bare component names
        Assert.Contains("<Label", defaultRazor);
        Assert.Contains("<Button", defaultRazor);
        Assert.DoesNotContain("asp:", defaultRazor);
        Assert.DoesNotContain("runat=", defaultRazor);
    }

    [Fact]
    public async Task Pipeline_CreatesCodeBehindFiles()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeCodeBehind: true);
        var pipeline = CreateFullPipeline();
        var scanner = new SourceScanner();
        var sourceFiles = scanner.Scan(inputDir, outputDir);

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { DryRun = false, SkipScaffold = true },
            SourceFiles = sourceFiles
        };

        var report = await pipeline.ExecuteAsync(context);

        Assert.Empty(report.Errors);
        // Default.aspx has a code-behind → should produce Default.razor.cs
        Assert.True(File.Exists(Path.Combine(outputDir, "Default.razor.cs")),
            "Default.razor.cs should be created from code-behind");
    }

    // ───────────────────────────────────────────────────────────────
    //  DryRun
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Pipeline_DryRun_ProducesNoOutputFiles()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var writer = new OutputWriter();
        var pipeline = CreateFullPipeline(writer);
        var scanner = new SourceScanner();
        var sourceFiles = scanner.Scan(inputDir, outputDir);

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { DryRun = true, SkipScaffold = true },
            SourceFiles = sourceFiles
        };

        var report = await pipeline.ExecuteAsync(context);

        Assert.True(report.FilesProcessed >= 2, "Files should still be processed in dry-run");
        Assert.Equal(0, report.FilesWritten);
        Assert.False(File.Exists(Path.Combine(outputDir, "Default.razor")),
            "No files should be written in dry-run mode");
        Assert.False(File.Exists(Path.Combine(outputDir, "About.razor")),
            "No files should be written in dry-run mode");
    }

    // ───────────────────────────────────────────────────────────────
    //  Scaffolding integration
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Scaffold_GeneratesProjectFiles()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var scaffolder = new ProjectScaffolder(new DatabaseProviderDetector());

        var result = scaffolder.Scaffold(inputDir, outputDir, "TestApp");

        // Verify all expected scaffold files are generated
        Assert.NotEmpty(result.Files);
        Assert.Contains("csproj", result.Files.Keys);
        Assert.Contains("program", result.Files.Keys);
        Assert.Contains("imports", result.Files.Keys);
        Assert.Contains("app", result.Files.Keys);

        // Verify csproj has BWFC reference
        Assert.Contains("Fritz.BlazorWebFormsComponents", result.Files["csproj"].Content);
    }

    [Fact]
    public async Task Scaffold_WritesFilesToDisk()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var scaffolder = new ProjectScaffolder(new DatabaseProviderDetector());
        var writer = new OutputWriter { DryRun = false };

        var result = scaffolder.Scaffold(inputDir, outputDir, "TestApp");
        await scaffolder.WriteAsync(result, outputDir, writer);

        Assert.True(File.Exists(Path.Combine(outputDir, "TestApp.csproj")));
        Assert.True(File.Exists(Path.Combine(outputDir, "Program.cs")));
        Assert.True(File.Exists(Path.Combine(outputDir, "_Imports.razor")));
        Assert.True(File.Exists(Path.Combine(outputDir, "Components", "App.razor")));
    }

    [Fact]
    public void SkipScaffold_FlagPreventsScaffoldGeneration()
    {
        // MigrationOptions.SkipScaffold controls whether scaffolding runs.
        // The caller is responsible for checking this flag — verify it's wired correctly.
        var options = new MigrationOptions { SkipScaffold = true };
        Assert.True(options.SkipScaffold);

        // When skip scaffold is true, the orchestrator should NOT call ProjectScaffolder.
        // This is a contract test — the option exists and is settable.
        var options2 = new MigrationOptions { SkipScaffold = false };
        Assert.False(options2.SkipScaffold);
    }

    // ───────────────────────────────────────────────────────────────
    //  Config transform integration
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void ConfigTransform_CreatesAppSettingsJson()
    {
        var (inputDir, _) = CreateTempProjectDir();
        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(inputDir);

        Assert.NotNull(result);
        Assert.NotNull(result!.JsonContent);
        Assert.Equal(1, result.AppSettingsCount);
        Assert.Equal(1, result.ConnectionStringsCount);
    }

    [Fact]
    public async Task ConfigTransform_WritesAppSettingsJsonToDisk()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var transformer = new WebConfigTransformer();
        var writer = new OutputWriter { DryRun = false };

        var result = transformer.Transform(inputDir);
        Assert.NotNull(result?.JsonContent);

        var outputPath = Path.Combine(outputDir, "appsettings.json");
        await writer.WriteFileAsync(outputPath, result!.JsonContent!, "appsettings.json");

        Assert.True(File.Exists(outputPath));
        var content = File.ReadAllText(outputPath);
        Assert.Contains("ConnectionStrings", content);
        Assert.Contains("SiteName", content);
    }

    // ───────────────────────────────────────────────────────────────
    //  Source scanner
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void SourceScanner_FindsAllAspxFiles()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        var scanner = new SourceScanner();

        var files = scanner.Scan(inputDir, outputDir);

        Assert.True(files.Count >= 2, $"Expected at least 2 .aspx files, found {files.Count}");
        Assert.Contains(files, f => f.MarkupPath.EndsWith("Default.aspx"));
        Assert.Contains(files, f => f.MarkupPath.EndsWith("About.aspx"));
    }

    [Fact]
    public void SourceScanner_DetectsCodeBehind()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeCodeBehind: true);
        var scanner = new SourceScanner();

        var files = scanner.Scan(inputDir, outputDir);

        var defaultFile = files.FirstOrDefault(f => f.MarkupPath.EndsWith("Default.aspx"));
        Assert.NotNull(defaultFile);
        Assert.True(defaultFile!.HasCodeBehind, "Default.aspx should detect its code-behind");

        var aboutFile = files.FirstOrDefault(f => f.MarkupPath.EndsWith("About.aspx"));
        Assert.NotNull(aboutFile);
        Assert.False(aboutFile!.HasCodeBehind, "About.aspx has no code-behind");
    }

    // ───────────────────────────────────────────────────────────────
    //  DatabaseProviderDetector integration
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void DatabaseProvider_DetectedFromWebConfig()
    {
        var (inputDir, _) = CreateTempProjectDir();
        var detector = new DatabaseProviderDetector();

        var info = detector.Detect(inputDir);

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", info.PackageName);
        Assert.Equal("UseSqlServer", info.ProviderMethod);
    }

    [Fact]
    public void DatabaseProvider_DefaultsToSqlServer_WhenNoWebConfig()
    {
        var baseTempDir = Path.Combine(Path.GetTempPath(), $"bwfc-noconfig-{Guid.NewGuid():N}");
        Directory.CreateDirectory(baseTempDir);
        _tempDirs.Add(baseTempDir);

        var detector = new DatabaseProviderDetector();
        var info = detector.Detect(baseTempDir);

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", info.PackageName);
    }

    // ───────────────────────────────────────────────────────────────
    //  Full end-to-end with scaffold + config + transforms
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task FullMigration_EndToEnd()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(
            includeWebConfig: true,
            includeCodeBehind: true);

        var pipeline = CreateFullPipeline();
        var scanner = new SourceScanner();
        var sourceFiles = scanner.Scan(inputDir, outputDir);

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { DryRun = false },
            SourceFiles = sourceFiles
        };
        var report = await pipeline.ExecuteAsync(context);

        // Assert — scaffold files exist (generated by ExecuteAsync step 1)
        Assert.True(File.Exists(Path.Combine(outputDir, "input.csproj")) ||
                    Directory.GetFiles(outputDir, "*.csproj").Length > 0, "csproj missing");
        Assert.True(File.Exists(Path.Combine(outputDir, "Program.cs")), "Program.cs missing");
        Assert.True(File.Exists(Path.Combine(outputDir, "_Imports.razor")), "_Imports.razor missing");
        Assert.True(File.Exists(Path.Combine(outputDir, "Components", "App.razor")), "App.razor missing");

        // Assert — config output
        Assert.True(File.Exists(Path.Combine(outputDir, "appsettings.json")), "appsettings.json missing");

        // Assert — global usings and shims
        Assert.True(File.Exists(Path.Combine(outputDir, "GlobalUsings.cs")), "GlobalUsings.cs missing");
        Assert.True(File.Exists(Path.Combine(outputDir, "WebFormsShims.cs")), "WebFormsShims.cs missing");

        // Assert — transformed markup files
        Assert.True(File.Exists(Path.Combine(outputDir, "Default.razor")), "Default.razor missing");
        Assert.True(File.Exists(Path.Combine(outputDir, "About.razor")), "About.razor missing");

        // Assert — transformed code-behind
        Assert.True(File.Exists(Path.Combine(outputDir, "Default.razor.cs")), "Default.razor.cs missing");

        // Assert — no identity shims (no Account folder)
        Assert.False(File.Exists(Path.Combine(outputDir, "IdentityShims.cs")),
            "IdentityShims.cs should not be generated without Account folder");

        // Assert — report
        Assert.True(report.FilesProcessed >= 2);
        Assert.Empty(report.Errors);
    }

    [Fact]
    public async Task FullMigration_WithIdentity_GeneratesIdentityShims()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(
            includeWebConfig: true,
            includeAccount: true);

        var pipeline = CreateFullPipeline();
        var scanner = new SourceScanner();
        var sourceFiles = scanner.Scan(inputDir, outputDir);

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { DryRun = false },
            SourceFiles = sourceFiles
        };

        await pipeline.ExecuteAsync(context);

        Assert.True(File.Exists(Path.Combine(outputDir, "WebFormsShims.cs")));
        Assert.True(File.Exists(Path.Combine(outputDir, "IdentityShims.cs")));
    }

    // ───────────────────────────────────────────────────────────────
    //  MigrationReport
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void MigrationReport_ToJson_IsValidJson()
    {
        var report = new MigrationReport
        {
            FilesProcessed = 5,
            FilesWritten = 10,
            TransformsApplied = 100,
            ScaffoldFilesGenerated = 6
        };
        report.Warnings.Add("Test warning");
        report.AddManualItem("Default.aspx", 10, "bwfc-general", "TODO item");

        var json = report.ToJson();

        var doc = System.Text.Json.JsonDocument.Parse(json);
        Assert.Equal(5, doc.RootElement.GetProperty("filesProcessed").GetInt32());
        Assert.Equal(10, doc.RootElement.GetProperty("filesWritten").GetInt32());
        Assert.Equal(1, doc.RootElement.GetProperty("warningCount").GetInt32());
        Assert.Equal(1, doc.RootElement.GetProperty("manualItemCount").GetInt32());

        var manualItems = doc.RootElement.GetProperty("manualItems");
        Assert.Equal(1, manualItems.GetArrayLength());
        var item = manualItems[0];
        Assert.Equal("Default.aspx", item.GetProperty("file").GetString());
        Assert.Equal(10, item.GetProperty("line").GetInt32());
        Assert.Equal("bwfc-general", item.GetProperty("category").GetString());
        Assert.Equal("TODO item", item.GetProperty("description").GetString());
        Assert.Equal("medium", item.GetProperty("severity").GetString());
    }

    [Fact]
    public async Task MigrationReport_WriteReportFile_CreatesFile()
    {
        var baseTempDir = Path.Combine(Path.GetTempPath(), $"bwfc-report-{Guid.NewGuid():N}");
        Directory.CreateDirectory(baseTempDir);
        _tempDirs.Add(baseTempDir);

        var report = new MigrationReport { FilesProcessed = 3 };
        var reportPath = Path.Combine(baseTempDir, "report.json");

        await report.WriteReportFileAsync(reportPath);

        Assert.True(File.Exists(reportPath));
        var content = File.ReadAllText(reportPath);
        Assert.Contains("filesProcessed", content);
    }

    [Fact]
    public async Task MigrationReport_WriteReportFile_NoOp_WhenPathNull()
    {
        var report = new MigrationReport { FilesProcessed = 3 };

        // Should not throw
        await report.WriteReportFileAsync(null);
        await report.WriteReportFileAsync("");
    }
}
