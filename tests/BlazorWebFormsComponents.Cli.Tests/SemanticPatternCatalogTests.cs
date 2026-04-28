using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Interop;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Scaffolding;
using BlazorWebFormsComponents.Cli.SemanticPatterns;
using BlazorWebFormsComponents.Cli.Transforms;

namespace BlazorWebFormsComponents.Cli.Tests;

public class SemanticPatternCatalogTests : IDisposable
{
    private readonly List<string> _tempDirs = [];

    public void Dispose()
    {
        foreach (var dir in _tempDirs)
        {
            if (!Directory.Exists(dir))
            {
                continue;
            }

            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch
            {
                // best effort
            }
        }
    }

    [Fact]
    public void Catalog_AppliesMatchingPatternsInOrder()
    {
        var catalog = new SemanticPatternCatalog(
        [
            new AppendSemanticPattern("pattern-b", 20, "|b", "|b-code"),
            new AppendSemanticPattern("pattern-a", 10, "|a", "|a-code")
        ]);

        var migrationContext = new MigrationContext
        {
            SourcePath = "input",
            OutputPath = "output",
            Options = new MigrationOptions()
        };

        var sourceFile = new SourceFile
        {
            MarkupPath = "Default.aspx",
            OutputPath = "Default.razor",
            FileType = FileType.Page
        };

        var metadata = new FileMetadata
        {
            SourceFilePath = sourceFile.MarkupPath,
            OutputFilePath = sourceFile.OutputPath,
            FileType = sourceFile.FileType,
            OriginalContent = "markup"
        };

        var report = new MigrationReport();
        var result = catalog.Apply(migrationContext, sourceFile, metadata, "markup", "code", report);

        Assert.Equal("markup|a|b", result.Markup);
        Assert.Equal("code|a-code|b-code", result.CodeBehind);
        Assert.Equal(["pattern-a", "pattern-b"], result.AppliedPatterns.Select(p => p.PatternId).ToArray());
        Assert.Equal(2, report.SemanticPatternsApplied);
        Assert.Equal(2, migrationContext.Log.Entries.Count);
    }

    [Fact]
    public async Task Pipeline_RunsSemanticPatterns_AfterMarkupAndCodeBehindTransforms()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"bwfc-semantic-patterns-{Guid.NewGuid():N}");
        var inputDir = Path.Combine(tempRoot, "input");
        var outputDir = Path.Combine(tempRoot, "output");
        Directory.CreateDirectory(inputDir);
        Directory.CreateDirectory(outputDir);
        _tempDirs.Add(tempRoot);

        File.WriteAllText(Path.Combine(inputDir, "Default.aspx"), "<page />");
        File.WriteAllText(Path.Combine(inputDir, "Default.aspx.cs"), "code");

        var sourceFiles = new[]
        {
            new SourceFile
            {
                MarkupPath = Path.Combine(inputDir, "Default.aspx"),
                CodeBehindPath = Path.Combine(inputDir, "Default.aspx.cs"),
                OutputPath = Path.Combine(outputDir, "Default.razor"),
                FileType = FileType.Page
            }
        };

        var outputWriter = new OutputWriter();
        var pipeline = new MigrationPipeline(
            [new AppendMarkupTransform()],
            [new AppendCodeBehindTransform()],
            new SemanticPatternCatalog([new VerifyAndAppendSemanticPattern()]),
            new ProjectScaffolder(new DatabaseProviderDetector()),
            new GlobalUsingsGenerator(),
            new ShimGenerator(),
            new WebConfigTransformer(),
            outputWriter,
            new StaticFileCopier(outputWriter),
            new SourceFileCopier(outputWriter, []),
            new AppStartCopier(outputWriter),
            new AppAssetInjector(outputWriter),
            new NuGetStaticAssetExtractor(new PowerShellScriptRunner()),
            new EdmxConverterBridge(new PowerShellScriptRunner()),
            new RedirectHandlerAnnotator(outputWriter));

        var context = new MigrationContext
        {
            SourcePath = inputDir,
            OutputPath = outputDir,
            Options = new MigrationOptions { SkipScaffold = true },
            SourceFiles = sourceFiles
        };

        var report = await pipeline.ExecuteAsync(context);

        var markupOutput = await File.ReadAllTextAsync(Path.Combine(outputDir, "Default.razor"));
        var codeBehindOutput = await File.ReadAllTextAsync(Path.Combine(outputDir, "migration-artifacts", "codebehind", "Default.razor.cs.txt"));

        Assert.Equal("<page />|markup|code-markup|semantic", markupOutput);
        Assert.Equal("code|codebehind|semantic", codeBehindOutput);
        Assert.Single(context.Log.Entries);
        Assert.Equal("verify-semantic-order", context.Log.Entries[0].Transform);
        Assert.Equal(1, report.SemanticPatternsApplied);
        Assert.Empty(report.Errors);
    }

    [Fact]
    public void Catalog_QueryDetailsPattern_RewritesSelectMethodToQueryBoundStub()
    {
        var catalog = new SemanticPatternCatalog([new QueryDetailsSemanticPattern()]);
        var migrationContext = new MigrationContext
        {
            SourcePath = "input",
            OutputPath = "output",
            Options = new MigrationOptions()
        };
        var sourceFile = new SourceFile
        {
            MarkupPath = Path.Combine("input", "ProductDetails.aspx"),
            OutputPath = Path.Combine("output", "ProductDetails.razor"),
            FileType = FileType.Page
        };
        var metadata = new FileMetadata
        {
            SourceFilePath = sourceFile.MarkupPath,
            OutputFilePath = sourceFile.OutputPath,
            FileType = sourceFile.FileType,
            OriginalContent = "<FormView />"
        };
        var report = new MigrationReport();

        var result = catalog.Apply(
            migrationContext,
            sourceFile,
            metadata,
            """
            @page "/ProductDetails"

            <FormView TItem="Product" SelectMethod="GetProduct" />
            """,
            """
            using WingtipToys.Models;

            public partial class ProductDetails
            {
                public IQueryable<Product> GetProduct(
                    [QueryString("ProductID")] int? productId,
                    [RouteData] string productName)
                {
                    return Enumerable.Empty<Product>().AsQueryable();
                }
            }
            """,
            report);

        Assert.Contains("SelectItems=\"GetProductQueryDetails_SelectItems\"", result.Markup);
        Assert.Contains("[Parameter, SupplyParameterFromQuery(Name = \"ProductID\")] public int? ProductId { get; set; }", result.Markup);
        Assert.Contains("public string? ProductName { get; set; }", result.Markup);
        Assert.Contains("TODO(bwfc-query-details)", result.Markup);
        Assert.Single(result.AppliedPatterns);
        Assert.Equal("pattern-query-details", result.AppliedPatterns[0].PatternId);
        Assert.Single(report.ManualItems);
        Assert.Equal("bwfc-query-details", report.ManualItems[0].Category);
    }

    [Fact]
    public void Catalog_ActionPagesPattern_ReplacesInertMarkupWithHandlerStub()
    {
        var catalog = new SemanticPatternCatalog([new ActionPagesSemanticPattern()]);
        var migrationContext = new MigrationContext
        {
            SourcePath = "input",
            OutputPath = "output",
            Options = new MigrationOptions()
        };
        var sourceFile = new SourceFile
        {
            MarkupPath = Path.Combine("input", "AddToCart.aspx"),
            OutputPath = Path.Combine("output", "AddToCart.razor"),
            FileType = FileType.Page
        };
        var metadata = new FileMetadata
        {
            SourceFilePath = sourceFile.MarkupPath,
            OutputFilePath = sourceFile.OutputPath,
            FileType = sourceFile.FileType,
            OriginalContent = "<html></html>"
        };
        var report = new MigrationReport();

        var result = catalog.Apply(
            migrationContext,
            sourceFile,
            metadata,
            """
            @page "/AddToCart"

            <div></div>
            """,
            """
            public partial class AddToCart
            {
                protected void Page_Load()
                {
                    var rawId = Request.QueryString["ProductID"];
                    usersShoppingCart.AddToCart(Convert.ToInt16(rawId));
                    Response.Redirect("ShoppingCart.aspx");
                }
            }
            """,
            report);

        Assert.Contains("<PageTitle>AddToCart</PageTitle>", result.Markup);
        Assert.Contains("TODO(bwfc-action-pages)", result.Markup);
        Assert.Contains("action=\"/__bwfc/actions/AddToCart\"", result.Markup);
        Assert.Contains("document.getElementById('bwfc-action-pages-form')?.submit();", result.Markup);
        Assert.Contains("[Parameter, SupplyParameterFromQuery(Name = \"ProductID\")] public string? ProductID { get; set; }", result.Markup);
        Assert.Contains("private const string HandlerRoute = \"/__bwfc/actions/AddToCart\";", result.Markup);
        Assert.Single(result.AppliedPatterns);
        Assert.Equal("pattern-action-pages", result.AppliedPatterns[0].PatternId);
        Assert.Single(report.ManualItems);
        Assert.Equal("bwfc-action-pages", report.ManualItems[0].Category);
    }

    private sealed class AppendSemanticPattern(string id, int order, string markupSuffix, string codeSuffix) : ISemanticPattern
    {
        public string Id => id;
        public int Order => order;

        public SemanticPatternMatch Match(SemanticPatternContext context) => SemanticPatternMatch.Match($"{Id} matched");

        public SemanticPatternResult Apply(SemanticPatternContext context) =>
            new($"{context.Markup}{markupSuffix}", $"{context.CodeBehind}{codeSuffix}", $"{Id} applied");
    }

    private sealed class AppendMarkupTransform : IMarkupTransform
    {
        public string Name => "append-markup";
        public int Order => 10;

        public string Apply(string content, FileMetadata metadata) => $"{content}|markup";
    }

    private sealed class AppendCodeBehindTransform : ICodeBehindTransform
    {
        public string Name => "append-codebehind";
        public int Order => 10;

        public string Apply(string content, FileMetadata metadata)
        {
            metadata.MarkupContent = $"{metadata.MarkupContent}|code-markup";
            return $"{content}|codebehind";
        }
    }

    private sealed class VerifyAndAppendSemanticPattern : ISemanticPattern
    {
        public string Id => "verify-semantic-order";
        public int Order => 10;

        public SemanticPatternMatch Match(SemanticPatternContext context)
        {
            return context.Markup == "<page />|markup|code-markup" && context.CodeBehind == "code|codebehind"
                ? SemanticPatternMatch.Match("semantic pattern saw post-transform state")
                : SemanticPatternMatch.NoMatch();
        }

        public SemanticPatternResult Apply(SemanticPatternContext context) =>
            new($"{context.Markup}|semantic", $"{context.CodeBehind}|semantic", "semantic pattern applied after transforms");
    }
}
