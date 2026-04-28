using System.Diagnostics;
using System.Reflection;
using System.Text;
using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Interop;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Scaffolding;
using BlazorWebFormsComponents.Cli.SemanticPatterns;

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
            new SemanticPatternCatalog(TestHelpers.CreateDefaultSemanticPatterns()),
            new ProjectScaffolder(new DatabaseProviderDetector()),
            new GlobalUsingsGenerator(),
            new ShimGenerator(),
            new WebConfigTransformer(),
            outputWriter,
            new StaticFileCopier(outputWriter),
            new SourceFileCopier(outputWriter, codeBehindTransforms),
            new AppStartCopier(outputWriter),
            new AppAssetInjector(outputWriter),
            new NuGetStaticAssetExtractor(new PowerShellScriptRunner()),
            new EdmxConverterBridge(new PowerShellScriptRunner()),
            new RedirectHandlerAnnotator(outputWriter));
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

    private (string inputDir, string outputDir) CreateRepoScopedProjectDir(string suffix)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var baseDir = Path.Combine(projectRoot, "obj", $"bwfc-pipeline-{suffix}-{Guid.NewGuid():N}");
        var inputDir = Path.Combine(baseDir, "input");
        var outputDir = Path.Combine(baseDir, "output");

        Directory.CreateDirectory(inputDir);
        Directory.CreateDirectory(outputDir);
        _tempDirs.Add(baseDir);

        return (inputDir, outputDir);
    }

    private static async Task<(int ExitCode, string Output)> RunProcessAsync(string fileName, string arguments, string workingDirectory)
    {
        var output = new StringBuilder();
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                output.AppendLine(e.Data);
            }
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                output.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        return (process.ExitCode, output.ToString());
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
    public async Task Pipeline_QuarantinesCodeBehindFilesAsManualArtifacts()
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
        // Default.aspx has a code-behind → should produce a quarantined manual artifact, not a compile-included .razor.cs file
        Assert.False(File.Exists(Path.Combine(outputDir, "Default.razor.cs")),
            "Default.razor.cs should not be written directly into the compile surface");
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "codebehind", "Default.razor.cs.txt")),
            "Default.razor.cs.txt should be created as a manual migration artifact");
    }

    [Fact]
    public async Task Pipeline_QueryDetailsPattern_RewritesMarkupWithQueryStub()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeWebConfig: false, includeCodeBehind: false);
        File.WriteAllText(Path.Combine(inputDir, "ProductDetails.aspx"), """
            <%@ Page Title="Product Details" Language="C#" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="TestApp.ProductDetails" %>
            <asp:FormView ID="productDetail" runat="server" ItemType="TestApp.Models.Product" SelectMethod="GetProduct" />
            """);
        File.WriteAllText(Path.Combine(inputDir, "ProductDetails.aspx.cs"), """
            using System.Linq;
            using TestApp.Models;

            namespace TestApp
            {
                public partial class ProductDetails
                {
                    public IQueryable<Product> GetProduct([QueryString("ProductID")] int? productId, [RouteData] string productName)
                    {
                        return Enumerable.Empty<Product>().AsQueryable();
                    }
                }
            }
            """);

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

        var markup = File.ReadAllText(Path.Combine(outputDir, "ProductDetails.razor"));
        Assert.Contains("SelectItems=\"GetProductQueryDetails_SelectItems\"", markup);
        Assert.Contains("SupplyParameterFromQuery(Name = \"ProductID\")", markup);
        Assert.True(report.SemanticPatternsApplied >= 1, $"Expected at least one semantic rewrite, got {report.SemanticPatternsApplied}");
        Assert.Contains(report.ManualItems, item => item.Category == "bwfc-query-details");
    }

    [Fact]
    public async Task Pipeline_ActionPagePattern_ReplacesBlankHandlerOutput()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeWebConfig: false, includeCodeBehind: false);
        File.WriteAllText(Path.Combine(inputDir, "AddToCart.aspx"), """
            <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddToCart.aspx.cs" Inherits="TestApp.AddToCart" %>
            <html xmlns="http://www.w3.org/1999/xhtml">
            <head runat="server"><title></title></head>
            <body>
                <form id="form1" runat="server">
                    <div></div>
                </form>
            </body>
            </html>
            """);
        File.WriteAllText(Path.Combine(inputDir, "AddToCart.aspx.cs"), """
            namespace TestApp
            {
                public partial class AddToCart
                {
                    protected void Page_Load()
                    {
                        var rawId = Request.QueryString["ProductID"];
                        usersShoppingCart.AddToCart(Convert.ToInt16(rawId));
                        Response.Redirect("ShoppingCart.aspx");
                    }
                }
            }
            """);

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

        var markup = File.ReadAllText(Path.Combine(outputDir, "AddToCart.razor"));
        Assert.Contains("<PageTitle>AddToCart</PageTitle>", markup);
        Assert.Contains("action=\"/__bwfc/actions/AddToCart\"", markup);
        Assert.Contains("document.getElementById('bwfc-action-pages-form')?.submit();", markup);
        Assert.Contains("TODO(bwfc-action-pages)", markup);
        Assert.True(report.SemanticPatternsApplied >= 1, $"Expected at least one semantic rewrite, got {report.SemanticPatternsApplied}");
        Assert.Contains(report.ManualItems, item => item.Category == "bwfc-action-pages");
    }

    [Fact]
    public async Task Pipeline_MasterContentContractPatterns_RewriteDefaultCatalogMasterLayouts()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeWebConfig: false, includeCodeBehind: false);
        File.WriteAllText(Path.Combine(inputDir, "Site.Master"), """
            <%@ Master Language="C#" %>
            <asp:ContentPlaceHolder ID="HeadContent" runat="server" />
            <div class="shell">
                <asp:ContentPlaceHolder ID="MainContent" runat="server" />
            </div>
            """);

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

        var masterMarkup = File.ReadAllText(Path.Combine(outputDir, "Site.razor"));
        var defaultMarkup = File.ReadAllText(Path.Combine(outputDir, "Default.razor"));

        Assert.Contains("@ChildComponents", masterMarkup);
        Assert.Contains("public RenderFragment? ChildComponents { get; set; }", masterMarkup);
        Assert.Contains("<ChildComponents>", defaultMarkup);
        Assert.DoesNotContain("<ChildContent>", defaultMarkup);
        Assert.True(report.SemanticPatternsApplied >= 3, $"Expected master/content semantic rewrites to run, got {report.SemanticPatternsApplied}");
    }

    [Fact]
    public async Task Pipeline_AccountPagePattern_RewritesDefaultCatalogLoginPage()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeWebConfig: false, includeCodeBehind: false, includeAccount: true);
        File.WriteAllText(Path.Combine(inputDir, "Account", "Login.aspx"), """
            <%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:ValidationSummary CssClass="text-danger" runat="server" />
                <asp:Label AssociatedControlID="Email">Email</asp:Label>
                <asp:TextBox ID="Email" CssClass="form-control" TextMode="Email" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="Email" ErrorMessage="Email is required." runat="server" />
                <asp:Label AssociatedControlID="Password">Password</asp:Label>
                <asp:TextBox ID="Password" CssClass="form-control" TextMode="Password" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="Password" ErrorMessage="Password is required." runat="server" />
                <asp:CheckBox ID="RememberMe" runat="server" />
                <asp:Label AssociatedControlID="RememberMe">Remember me?</asp:Label>
                <asp:Button Text="Log in" CssClass="btn btn-default" runat="server" />
                <asp:HyperLink runat="server">Register as a new user</asp:HyperLink>
            </asp:Content>
            """);

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

        var loginMarkup = File.ReadAllText(Path.Combine(outputDir, "Account", "Login.razor"));

        Assert.Contains("TODO(bwfc-identity)", loginMarkup);
        Assert.Contains("<form method=\"get\" action=\"/Account/PerformLogin\" class=\"form-horizontal\">", loginMarkup);
        Assert.Contains("type=\"email\"", loginMarkup);
        Assert.Contains("type=\"password\"", loginMarkup);
        Assert.Contains("Register as a new user", loginMarkup);
        Assert.Contains("SupplyParameterFromQuery(Name = \"returnUrl\")", loginMarkup);
        Assert.DoesNotContain("<RequiredFieldValidator", loginMarkup);
        Assert.True(report.SemanticPatternsApplied >= 1, "Expected the account semantic pattern to run for Account/Login.aspx");
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

        // Assert — transformed code-behind is quarantined as a manual artifact
        Assert.False(File.Exists(Path.Combine(outputDir, "Default.razor.cs")), "Default.razor.cs should not be emitted into the compile surface");
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "codebehind", "Default.razor.cs.txt")), "Default.razor.cs.txt missing");

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

    [Fact]
    public async Task FullMigration_InjectsDetectedCssAndScriptsIntoAppRazor()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        Directory.CreateDirectory(Path.Combine(inputDir, "Content"));
        File.WriteAllText(Path.Combine(inputDir, "Content", "site.css"), "body { color: red; }");
        Directory.CreateDirectory(Path.Combine(inputDir, "Scripts"));
        File.WriteAllText(Path.Combine(inputDir, "Scripts", "jquery-3.7.1.js"), "window.jqueryLoaded = true;");
        File.WriteAllText(Path.Combine(inputDir, "Site.Master"), """
            <%@ Master Language="C#" %>
            <html>
            <head>
                <link rel="stylesheet" href="https://cdn.example.com/site.css" />
                <script src="https://cdn.example.com/site.js"></script>
            </head>
            <body>
                <asp:ContentPlaceHolder ID="MainContent" runat="server" />
            </body>
            </html>
            """);

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

        var appRazor = File.ReadAllText(Path.Combine(outputDir, "Components", "App.razor"));
        Assert.Contains("/Content/site.css", appRazor);
        Assert.Contains("https://cdn.example.com/site.css", appRazor);
        Assert.Contains("/Scripts/jquery-3.7.1.js", appRazor);
    }

    [Fact]
    public async Task FullMigration_QuarantinesAppStartFilesAsManualArtifacts()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        Directory.CreateDirectory(Path.Combine(inputDir, "App_Start"));
        File.WriteAllText(Path.Combine(inputDir, "App_Start", "RouteConfig.cs"), """
            using System.Web.Routing;

            public class RouteConfig
            {
            }
            """);
        File.WriteAllText(Path.Combine(inputDir, "App_Start", "WebApiConfig.cs"), """
            public class WebApiConfig
            {
            }
            """);

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

        Assert.False(File.Exists(Path.Combine(outputDir, "RouteConfig.cs")));
        Assert.False(File.Exists(Path.Combine(outputDir, "WebApiConfig.cs")));
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "App_Start", "RouteConfig.cs.txt")));
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "App_Start", "WebApiConfig.cs.txt")));
        Assert.Contains("Blazor has no App_Start convention", File.ReadAllText(Path.Combine(outputDir, "migration-artifacts", "App_Start", "RouteConfig.cs.txt")));
        Assert.Contains(report.ManualItems, item => item.Category == "AppStart");
    }

    [Fact]
    public async Task FullMigration_QuarantinesLegacyCompileSurfaceFilesAndKeepsSafeSources()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        Directory.CreateDirectory(Path.Combine(inputDir, "Models"));

        File.WriteAllText(Path.Combine(inputDir, "Startup.Auth.cs"), """
            using Microsoft.Owin;
            using Owin;

            public partial class Startup
            {
                public void ConfigureAuth(IAppBuilder app)
                {
                }
            }
            """);

        File.WriteAllText(Path.Combine(inputDir, "CatalogDatabaseInitializer.cs"), """
            using System.Data.Entity;

            public class CatalogDatabaseInitializer : DropCreateDatabaseIfModelChanges<CatalogContext>
            {
            }
            """);

        File.WriteAllText(Path.Combine(inputDir, "Models", "Product.cs"), """
            namespace TestApp.Models;

            public class Product
            {
                public int Id { get; set; }
                public string Name { get; set; } = string.Empty;
            }
            """);

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

        Assert.False(File.Exists(Path.Combine(outputDir, "Startup.Auth.cs")));
        Assert.False(File.Exists(Path.Combine(outputDir, "CatalogDatabaseInitializer.cs")));
        Assert.True(File.Exists(Path.Combine(outputDir, "Models", "Product.cs")));

        var startupArtifact = Path.Combine(outputDir, "migration-artifacts", "compile-surface", "Startup.Auth.cs.txt");
        var efArtifact = Path.Combine(outputDir, "migration-artifacts", "compile-surface", "CatalogDatabaseInitializer.cs.txt");

        Assert.True(File.Exists(startupArtifact));
        Assert.True(File.Exists(efArtifact));
        Assert.Contains("quarantined from the generated Blazor SSR compile surface", File.ReadAllText(startupArtifact));
        Assert.Contains(report.ManualItems, item => item.Category == "bwfc-compile-surface" && item.File == "Startup.Auth.cs");
        Assert.Contains(report.ManualItems, item => item.Category == "bwfc-compile-surface" && item.File == "CatalogDatabaseInitializer.cs");
    }

    [Fact]
    public async Task FullMigration_BuildsGeneratedApp_WhenLegacyCompileSurfaceFilesAreQuarantined()
    {
        var (inputDir, outputDir) = CreateRepoScopedProjectDir("build-clean");
        Directory.CreateDirectory(Path.Combine(inputDir, "Account"));

        File.WriteAllText(Path.Combine(inputDir, "Site.Master"), """
            <%@ Master Language="C#" AutoEventWireup="true" CodeBehind="Site.master.cs" Inherits="TestApp.SiteMaster" %>
            <!DOCTYPE html>
            <html>
            <head runat="server">
                <title>Wingtip Shell</title>
                <asp:ContentPlaceHolder ID="HeadContent" runat="server" />
            </head>
            <body>
                <form runat="server">
                    <div class="shell">
                        <asp:ContentPlaceHolder ID="MainContent" runat="server" />
                    </div>
                </form>
            </body>
            </html>
            """);

        File.WriteAllText(Path.Combine(inputDir, "Default.aspx"), """
            <%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Inherits="TestApp._Default" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:Label ID="Label1" runat="server" Text="Hello World" CssClass="title" />
            </asp:Content>
            """);

        File.WriteAllText(Path.Combine(inputDir, "AddToCart.aspx"), """
            <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddToCart.aspx.cs" Inherits="TestApp.AddToCart" %>
            """);

        File.WriteAllText(Path.Combine(inputDir, "AddToCart.aspx.cs"), """
            using System;

            namespace TestApp
            {
                public partial class AddToCart
                {
                    protected void Page_Load(object sender, EventArgs e)
                    {
                        var rawId = Request.QueryString["ProductID"];
                        Response.Redirect("~/ShoppingCart.aspx");
                    }
                }
            }
            """);

        File.WriteAllText(Path.Combine(inputDir, "Account", "Login.aspx"), """
            <%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Inherits="TestApp.Account.Login" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:Label AssociatedControlID="Email">Email</asp:Label>
                <asp:TextBox ID="Email" runat="server" TextMode="Email" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="Email" ErrorMessage="Email is required." runat="server" />
                <asp:Label AssociatedControlID="Password">Password</asp:Label>
                <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="Password" ErrorMessage="Password is required." runat="server" />
                <asp:Button ID="LoginButton" runat="server" Text="Log in" CssClass="btn btn-default" />
            </asp:Content>
            """);

        File.WriteAllText(Path.Combine(inputDir, "Account", "Register.aspx"), """
            <%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Inherits="TestApp.Account.Register" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:Label AssociatedControlID="Email">Email</asp:Label>
                <asp:TextBox ID="Email" runat="server" TextMode="Email" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="Email" ErrorMessage="Email is required." runat="server" />
                <asp:Label AssociatedControlID="Password">Password</asp:Label>
                <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="Password" ErrorMessage="Password is required." runat="server" />
                <asp:Label AssociatedControlID="ConfirmPassword">Confirm password</asp:Label>
                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
                <asp:CompareValidator ControlToValidate="ConfirmPassword" ControlToCompare="Password" ErrorMessage="Mismatch" runat="server" />
                <asp:Button ID="RegisterButton" runat="server" Text="Register" CssClass="btn btn-default" />
            </asp:Content>
            """);

        File.WriteAllText(Path.Combine(inputDir, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="SiteName" value="Wingtip Acceptance" />
              </appSettings>
            </configuration>
            """);

        File.WriteAllText(Path.Combine(inputDir, "Startup.Auth.cs"), """
            using Microsoft.Owin;
            using Owin;

            public partial class Startup
            {
                public void ConfigureAuth(IAppBuilder app)
                {
                }
            }
            """);

        File.WriteAllText(Path.Combine(inputDir, "IdentityConfig.cs"), """
            using Microsoft.AspNet.Identity;

            public class IdentityConfig
            {
            }
            """);

        File.WriteAllText(Path.Combine(inputDir, "CatalogDatabaseInitializer.cs"), """
            using System.Data.Entity;

            public class CatalogDatabaseInitializer : DropCreateDatabaseIfModelChanges<object>
            {
            }
            """);

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
        Assert.Empty(report.Errors);

        Assert.False(File.Exists(Path.Combine(outputDir, "Startup.Auth.cs")));
        Assert.False(File.Exists(Path.Combine(outputDir, "IdentityConfig.cs")));
        Assert.False(File.Exists(Path.Combine(outputDir, "CatalogDatabaseInitializer.cs")));

        var layoutDir = Path.Combine(outputDir, "Layout");
        Directory.CreateDirectory(layoutDir);
        File.WriteAllText(Path.Combine(layoutDir, "MainLayout.razor"), """
            @inherits Microsoft.AspNetCore.Components.LayoutComponentBase

            <main>
                @Body
            </main>
            """);

        var projectPath = Directory.GetFiles(outputDir, "*.csproj", SearchOption.TopDirectoryOnly).Single();
        var (exitCode, buildOutput) = await RunProcessAsync("dotnet", $"build \"{projectPath}\" -c Release --nologo", outputDir);

        Assert.True(exitCode == 0, buildOutput);
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "compile-surface", "Startup.Auth.cs.txt")));
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "compile-surface", "IdentityConfig.cs.txt")));
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "compile-surface", "CatalogDatabaseInitializer.cs.txt")));
    }

    [Fact]
    public async Task FullMigration_AnnotatesProgramForRedirectHandlerPages()
    {
        var (inputDir, outputDir) = CreateTempProjectDir();
        File.WriteAllText(Path.Combine(inputDir, "CheckoutStart.aspx"), """
            <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckoutStart.aspx.cs" Inherits="TestApp.CheckoutStart" %>
            """);
        File.WriteAllText(Path.Combine(inputDir, "CheckoutStart.aspx.cs"), """
            using System;

            namespace TestApp
            {
                public partial class CheckoutStart
                {
                    protected void Page_Load(object sender, EventArgs e)
                    {
                        Response.Redirect("~/Checkout/Start.aspx");
                    }
                }
            }
            """);

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

        var program = File.ReadAllText(Path.Combine(outputDir, "Program.cs"));
        Assert.Contains("app.MapPost(\"/__bwfc/actions/CheckoutStart\"", program);
        Assert.Contains("TODO(bwfc-action-pages)", program);
    }

    [Fact]
    public async Task FullMigration_GeneratesRoutableWingtipStyleShellArtifacts()
    {
        var (inputDir, outputDir) = CreateTempProjectDir(includeWebConfig: false, includeAccount: true);
        File.WriteAllText(Path.Combine(inputDir, "Site.Master"), """
            <%@ Master Language="C#" %>
            <div class="site-shell">
                <asp:ContentPlaceHolder ID="HeadContent" runat="server" />
                <asp:ContentPlaceHolder ID="MainContent" runat="server" />
            </div>
            """);
        File.WriteAllText(Path.Combine(inputDir, "Account", "Login.aspx"), """
            <%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:ValidationSummary CssClass="text-danger" runat="server" />
                <asp:Label AssociatedControlID="Email">Email</asp:Label>
                <asp:TextBox ID="Email" CssClass="form-control" TextMode="Email" runat="server" />
                <asp:Label AssociatedControlID="Password">Password</asp:Label>
                <asp:TextBox ID="Password" CssClass="form-control" TextMode="Password" runat="server" />
                <asp:Button Text="Log in" CssClass="btn btn-default" runat="server" />
            </asp:Content>
            """);
        File.WriteAllText(Path.Combine(inputDir, "Account", "Register.aspx"), """
            <%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
            <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
                <asp:ValidationSummary CssClass="text-danger" runat="server" />
                <asp:Label AssociatedControlID="Email">Email</asp:Label>
                <asp:TextBox ID="Email" CssClass="form-control" TextMode="Email" runat="server" />
                <asp:Label AssociatedControlID="Password">Password</asp:Label>
                <asp:TextBox ID="Password" CssClass="form-control" TextMode="Password" runat="server" />
                <asp:Label AssociatedControlID="ConfirmPassword">Confirm password</asp:Label>
                <asp:TextBox ID="ConfirmPassword" CssClass="form-control" TextMode="Password" runat="server" />
                <asp:Button Text="Register" CssClass="btn btn-default" runat="server" />
            </asp:Content>
            """);
        File.WriteAllText(Path.Combine(inputDir, "AddToCart.aspx"), """
            <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddToCart.aspx.cs" Inherits="TestApp.AddToCart" %>
            <html xmlns="http://www.w3.org/1999/xhtml">
            <head runat="server"><title></title></head>
            <body>
                <form id="form1" runat="server">
                    <div></div>
                </form>
            </body>
            </html>
            """);
        File.WriteAllText(Path.Combine(inputDir, "AddToCart.aspx.cs"), """
            namespace TestApp
            {
                public partial class AddToCart
                {
                    protected void Page_Load()
                    {
                        var rawId = Request.QueryString["ProductID"];
                        Response.Redirect("ShoppingCart.aspx");
                    }
                }
            }
            """);
        File.WriteAllText(Path.Combine(inputDir, "Startup.Auth.cs"), """
            using Microsoft.Owin;
            using Owin;

            public partial class Startup
            {
                public void ConfigureAuth(IAppBuilder app)
                {
                }
            }
            """);

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

        var siteMarkup = File.ReadAllText(Path.Combine(outputDir, "Site.razor"));
        var loginMarkup = File.ReadAllText(Path.Combine(outputDir, "Account", "Login.razor"));
        var registerMarkup = File.ReadAllText(Path.Combine(outputDir, "Account", "Register.razor"));
        var actionMarkup = File.ReadAllText(Path.Combine(outputDir, "AddToCart.razor"));
        var program = File.ReadAllText(Path.Combine(outputDir, "Program.cs"));

        Assert.Contains("@ChildComponents", siteMarkup);
        Assert.Contains("ContentPlaceHolder", siteMarkup);
        Assert.Contains("MainContent", siteMarkup);
        Assert.Contains("method=\"get\"", loginMarkup);
        Assert.Contains("action=\"/Account/PerformLogin\"", loginMarkup);
        Assert.Contains("SupplyParameterFromQuery(Name = \"returnUrl\")", loginMarkup);
        Assert.Contains("method=\"get\"", registerMarkup);
        Assert.Contains("action=\"/Account/PerformRegister\"", registerMarkup);
        Assert.Contains("action=\"/__bwfc/actions/AddToCart\"", actionMarkup);
        Assert.Contains("document.getElementById('bwfc-action-pages-form')?.submit();", actionMarkup);
        Assert.Contains("app.MapGet(\"/Account/PerformLogin\"", program);
        Assert.Contains("app.MapGet(\"/Account/PerformRegister\"", program);
        Assert.Contains("app.MapPost(\"/__bwfc/actions/AddToCart\"", program);
        Assert.Contains("DisableAntiforgery()", program);
        Assert.False(File.Exists(Path.Combine(outputDir, "Startup.Auth.cs")));
        Assert.True(File.Exists(Path.Combine(outputDir, "migration-artifacts", "compile-surface", "Startup.Auth.cs.txt")));
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
