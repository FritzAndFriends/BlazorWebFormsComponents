using System.CommandLine;
using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Interop;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Scaffolding;
using BlazorWebFormsComponents.Cli.SemanticPatterns;
using BlazorWebFormsComponents.Cli.Transforms;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using BlazorWebFormsComponents.Cli.Transforms.Directives;
using BlazorWebFormsComponents.Cli.Transforms.Markup;
using Microsoft.Extensions.DependencyInjection;
using NativeEdmxToEfCoreConverter = BlazorWebFormsComponents.Cli.Services.EdmxToEfCoreConverter;
using NativeNuGetStaticAssetExtractor = BlazorWebFormsComponents.Cli.Services.NuGetStaticAssetExtractor;
using BlazorWebFormsComponents.Cli.Services;

namespace BlazorWebFormsComponents.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("WebForms to Blazor - Convert ASP.NET Web Forms projects to Blazor using BWFC")
        {
            Name = "webforms-to-blazor"
        };

        rootCommand.AddCommand(BuildMigrateCommand());
        rootCommand.AddCommand(BuildConvertCommand());
        rootCommand.AddCommand(BuildPrescanCommand());
        rootCommand.AddCommand(BuildScanCommand());
        rootCommand.AddCommand(BuildAssetsCommand());
        rootCommand.AddCommand(BuildEdmxCommand());

        return await rootCommand.InvokeAsync(args);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        // Register markup transforms in order
        services.AddSingleton<IMarkupTransform, PageDirectiveTransform>();
        services.AddSingleton<IMarkupTransform, MasterDirectiveTransform>();
        services.AddSingleton<IMarkupTransform, ControlDirectiveTransform>();
        services.AddSingleton<IMarkupTransform, ImportDirectiveTransform>();
        services.AddSingleton<IMarkupTransform, RegisterDirectiveTransform>();
        services.AddSingleton<IMarkupTransform, MasterPageTransform>();
        services.AddSingleton<IMarkupTransform, ScriptManagerStripTransform>();
        services.AddSingleton<IMarkupTransform, ContentWrapperTransform>();
        services.AddSingleton<IMarkupTransform, FormWrapperTransform>();
        services.AddSingleton<IMarkupTransform, DisplayExpressionTransform>();
        services.AddSingleton<IMarkupTransform, ExpressionTransform>();
        services.AddSingleton<IMarkupTransform, ServerCodeBlockTransform>();
        services.AddSingleton<IMarkupTransform, LoginViewTransform>();
        services.AddSingleton<IMarkupTransform, SelectMethodTransform>();
        services.AddSingleton<IMarkupTransform, AjaxToolkitPrefixTransform>();
        services.AddSingleton<IMarkupTransform, AspPrefixTransform>();
        services.AddSingleton<IMarkupTransform, DataBindingAttributeTransform>();
        services.AddSingleton<IMarkupTransform, ValidatorGenericTypeTransform>();
        services.AddSingleton<IMarkupTransform, TemplateFieldChildComponentsTransform>();
        services.AddSingleton<IMarkupTransform, DataControlChildComponentsTransform>();
        services.AddSingleton<IMarkupTransform, AttributeStripTransform>();
        services.AddSingleton<IMarkupTransform, GridViewColumnItemTypeTransform>();
        services.AddSingleton<IMarkupTransform, EventWiringTransform>();
        services.AddSingleton<IMarkupTransform, ComponentRefMarkupTransform>();
        services.AddSingleton<IMarkupTransform, UrlReferenceTransform>();
        services.AddSingleton<IMarkupTransform, TemplatePlaceholderTransform>();
        services.AddSingleton<IMarkupTransform, TemplateContextTransform>();
        services.AddSingleton<IMarkupTransform, AttributeNormalizeTransform>();
        services.AddSingleton<IMarkupTransform, DataSourceIdTransform>();
        services.AddSingleton<IMarkupTransform, EnhancedNavAnnotationTransform>();
        services.AddSingleton<IMarkupTransform, GetRouteUrlMarkupTransform>();
        services.AddSingleton<IMarkupTransform, HtmlBoilerplateStripTransform>();
        services.AddSingleton<IMarkupTransform, MarkupCleanupTransform>();
        services.AddSingleton<IMarkupTransform, LabelFieldBindTransform>();

        // Register code-behind transforms in order
        services.AddSingleton<ICodeBehindTransform, TodoHeaderTransform>();
        services.AddSingleton<ICodeBehindTransform, NestedClassCollisionTransform>();
        services.AddSingleton<ICodeBehindTransform, UsingStripTransform>();
        services.AddSingleton<ICodeBehindTransform, IdentityUsingTransform>();
        services.AddSingleton<ICodeBehindTransform, HttpUtilityRewriteTransform>();
        services.AddSingleton<ICodeBehindTransform, EntityFrameworkTransform>();
        services.AddSingleton<ICodeBehindTransform, EfContextConstructorTransform>();
        services.AddSingleton<ICodeBehindTransform, DbContextInstantiationTransform>();
        services.AddSingleton<ICodeBehindTransform, HttpContextAccessorTransform>();
        services.AddSingleton<ICodeBehindTransform, SelectMethodMaterializeTransform>();
        services.AddSingleton<ICodeBehindTransform, EagerLoadNavigationTransform>();
        services.AddSingleton<ICodeBehindTransform, HttpExceptionTransform>();
        services.AddSingleton<ICodeBehindTransform, ConfigurationManagerTransform>();
        services.AddSingleton<ICodeBehindTransform, BaseClassStripTransform>();
        services.AddSingleton<ICodeBehindTransform, ClassNameAlignTransform>();
        services.AddSingleton<ICodeBehindTransform, NamespaceAlignTransform>();
        services.AddSingleton<ICodeBehindTransform, MethodNameCollisionTransform>();
        services.AddSingleton<ICodeBehindTransform, ComponentRefCodeBehindTransform>();
        services.AddSingleton<ICodeBehindTransform, ComponentRefNullSafetyTransform>();
        services.AddSingleton<ICodeBehindTransform, LabelFieldBindCodeBehindTransform>();
        services.AddSingleton<ICodeBehindTransform, ResponseRedirectTransform>();
        services.AddSingleton<ICodeBehindTransform, RequestFormTransform>();
        services.AddSingleton<ICodeBehindTransform, QueryStringTypeAnnotationTransform>();
        services.AddSingleton<ICodeBehindTransform, ServerShimTransform>();
        services.AddSingleton<ICodeBehindTransform, GetRouteUrlTransform>();
        services.AddSingleton<ICodeBehindTransform, CartSessionKeyTransform>();
        services.AddSingleton<ICodeBehindTransform, SessionDetectTransform>();
        services.AddSingleton<ICodeBehindTransform, SessionGetStringNullSafetyTransform>();
        services.AddSingleton<ICodeBehindTransform, ViewStateDetectTransform>();
        services.AddSingleton<ICodeBehindTransform, IsPostBackTransform>();
        services.AddSingleton<ICodeBehindTransform, PageLifecycleTransform>();
        services.AddSingleton<ICodeBehindTransform, EventHandlerSignatureTransform>();
        services.AddSingleton<ICodeBehindTransform, DataBindTransform>();
        services.AddSingleton<ICodeBehindTransform, ClientScriptTransform>();
        services.AddSingleton<ICodeBehindTransform, UrlCleanupTransform>();
        services.AddSingleton<PageQuarantineDetector>();
        services.AddSingleton<ICodeBehindTransform>(sp => new CompileSurfaceStubTransform(sp.GetRequiredService<PageQuarantineDetector>()));
        services.AddSingleton<ICodeBehindTransform, LegacyHelperStubTransform>();
        services.AddSingleton<ICodeBehindTransform, RouteParameterWiringTransform>();
        services.AddSingleton<ICodeBehindTransform, RouteDataParameterPromotionTransform>();
        services.AddSingleton<ICodeBehindTransform, TitlePropertyCodeBehindTransform>();
        services.AddSingleton<ICodeBehindTransform, MarkupReferencedMemberStubTransform>();
        services.AddSingleton<ICodeBehindTransform, TypeMismatchFixTransform>();
        services.AddSingleton<ICodeBehindTransform, DisposeReadonlyFieldTransform>();

        // Scaffolding
        services.AddSingleton<IRuntimeSignalDetector, EntityFrameworkRuntimeSignalDetector>();
        services.AddSingleton<IRuntimeSignalDetector, SessionRuntimeSignalDetector>();
        services.AddSingleton<IRuntimeSignalDetector, IdentityRuntimeSignalDetector>();
        services.AddSingleton<IRuntimeSignalDetector, GlobalAsaxRuntimeSignalDetector>();
        services.AddSingleton<IRuntimeSignalDetector, WebConfigRuntimeSignalDetector>();
        services.AddSingleton<RuntimeDetector>();
        services.AddSingleton<ProgramCsEmitter>();
        services.AddSingleton<MasterPageToLayoutConverter>();
        services.AddSingleton<ProjectScaffolder>();
        services.AddSingleton<GlobalUsingsGenerator>();
        services.AddSingleton<ShimGenerator>();
        services.AddSingleton<AppAssetInjector>();

        // Config
        services.AddSingleton<DatabaseProviderDetector>();
        services.AddSingleton<WebConfigTransformer>();
        services.AddSingleton<PrescanAnalyzer>();
        services.AddSingleton<NativeNuGetStaticAssetExtractor>();
        services.AddSingleton<NativeEdmxToEfCoreConverter>();

        // I/O
        services.AddSingleton<OutputWriter>();
        services.AddSingleton<SourceRootResolver>();
        services.AddSingleton<SourceScanner>();
        services.AddSingleton<StaticFileCopier>();
        services.AddSingleton<SourceFileCopier>();
        services.AddSingleton<AppStartCopier>();

         // Pipeline
          services.AddSingleton<RedirectHandlerAnnotator>();
         // Registration order is significant when patterns share the same Order value.
         services.AddSingleton<ISemanticPattern, QueryDetailsSemanticPattern>();
         services.AddSingleton<ISemanticPattern, MasterContentContractsSemanticPattern>();
         services.AddSingleton<ISemanticPattern, ActionPagesSemanticPattern>();
         services.AddSingleton<ISemanticPattern, AccountPagesSemanticPattern>();
         services.AddSingleton<SemanticPatternCatalog>();
        services.AddSingleton(sp => new MigrationPipeline(
            sp.GetServices<IMarkupTransform>(),
            sp.GetServices<ICodeBehindTransform>(),
            sp.GetRequiredService<SemanticPatternCatalog>(),
            sp.GetRequiredService<ProjectScaffolder>(),
            sp.GetRequiredService<GlobalUsingsGenerator>(),
            sp.GetRequiredService<ShimGenerator>(),
            sp.GetRequiredService<WebConfigTransformer>(),
            sp.GetRequiredService<OutputWriter>(),
            sp.GetRequiredService<StaticFileCopier>(),
            sp.GetRequiredService<SourceFileCopier>(),
            sp.GetRequiredService<AppStartCopier>(),
            sp.GetRequiredService<AppAssetInjector>(),
            sp.GetRequiredService<NativeNuGetStaticAssetExtractor>(),
            sp.GetRequiredService<NativeEdmxToEfCoreConverter>(),
            sp.GetRequiredService<RedirectHandlerAnnotator>(),
            sp.GetRequiredService<PageQuarantineDetector>()));

        return services.BuildServiceProvider();
    }

    private static Command BuildMigrateCommand()
    {
        var migrateCommand = new Command("migrate", "Full project migration from Web Forms to Blazor SSR on .NET 10");

        var inputOption = new Option<string>(
            aliases: ["--input", "-i"],
            description: "Source Web Forms project root (required)")
        { IsRequired = true };

        var outputOption = new Option<string>(
            aliases: ["--output", "-o"],
            description: "Output .NET 10 Blazor SSR project directory (required)")
        { IsRequired = true };

        var skipScaffoldOption = new Option<bool>(
            name: "--skip-scaffold",
            description: "Skip .NET 10 Blazor SSR scaffold generation (.csproj, Program.cs, _Imports.razor, App.razor, Routes.razor)",
            getDefaultValue: () => false);

        var dryRunOption = new Option<bool>(
            name: "--dry-run",
            description: "Show transforms without writing files",
            getDefaultValue: () => false);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Detailed per-file transform log",
            getDefaultValue: () => false);

        var overwriteOption = new Option<bool>(
            name: "--overwrite",
            description: "Overwrite existing files in output directory",
            getDefaultValue: () => false);

        var reportOption = new Option<string?>(
            name: "--report",
            description: "Write JSON migration report to file");

        migrateCommand.AddOption(inputOption);
        migrateCommand.AddOption(outputOption);
        migrateCommand.AddOption(skipScaffoldOption);
        migrateCommand.AddOption(dryRunOption);
        migrateCommand.AddOption(verboseOption);
        migrateCommand.AddOption(overwriteOption);
        migrateCommand.AddOption(reportOption);

        migrateCommand.SetHandler(async (input, output, skipScaffold, dryRun, verbose, overwrite, report) =>
        {
            try
            {
                using var sp = BuildServiceProvider();
                var sourceRootResolver = sp.GetRequiredService<SourceRootResolver>();
                var scanner = sp.GetRequiredService<SourceScanner>();
                var pipeline = sp.GetRequiredService<MigrationPipeline>();
                var effectiveInput = sourceRootResolver.Resolve(input);

                var context = new MigrationContext
                {
                    SourcePath = effectiveInput,
                    OutputPath = output,
                    Options = new MigrationOptions
                    {
                        SkipScaffold = skipScaffold,
                        DryRun = dryRun,
                        Verbose = verbose,
                        Overwrite = overwrite,
                        ReportPath = report
                    }
                };

                context.SourceFiles = scanner.Scan(effectiveInput, output);

                Console.WriteLine($"Found {context.SourceFiles.Count} Web Forms file(s) to migrate...");
                if (verbose && !string.Equals(input, effectiveInput, StringComparison.OrdinalIgnoreCase))
                    Console.WriteLine($"Resolved source root: {effectiveInput}");
                if (dryRun)
                    Console.WriteLine("(dry-run mode — no files will be written)");

                var migrationReport = await pipeline.ExecuteAsync(context);

                migrationReport.PrintSummary();

                if (migrationReport.Errors.Count > 0)
                    Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, outputOption, skipScaffoldOption, dryRunOption, verboseOption, overwriteOption, reportOption);

        return migrateCommand;
    }

    private static Command BuildConvertCommand()
    {
        var convertCommand = new Command("convert", "Single file conversion from Web Forms to a Blazor SSR-compatible Razor file");

        var inputOption = new Option<string>(
            aliases: ["--input", "-i"],
            description: ".aspx, .ascx, or .master file (required)")
        { IsRequired = true };

        var outputOption = new Option<string?>(
            aliases: ["--output", "-o"],
            description: "Output directory (default: same directory)");

        var overwriteOption = new Option<bool>(
            name: "--overwrite",
            description: "Overwrite existing .razor file",
            getDefaultValue: () => false);

        convertCommand.AddOption(inputOption);
        convertCommand.AddOption(outputOption);
        convertCommand.AddOption(overwriteOption);

        convertCommand.SetHandler(async (input, output, overwrite) =>
        {
            try
            {
                if (!File.Exists(input))
                    throw new FileNotFoundException($"Input file not found: {input}");

                var outputDir = output ?? Path.GetDirectoryName(input) ?? ".";
                var razorName = Path.GetFileNameWithoutExtension(input) + ".razor";
                var outputPath = Path.Combine(outputDir, razorName);

                if (File.Exists(outputPath) && !overwrite)
                {
                    Console.Error.WriteLine($"Output file already exists: {outputPath}. Use --overwrite to replace.");
                    Environment.Exit(1);
                }

                using var sp = BuildServiceProvider();
                var pipeline = sp.GetRequiredService<MigrationPipeline>();

                var markupContent = await File.ReadAllTextAsync(input);
                var ext = Path.GetExtension(input).ToLowerInvariant();
                var fileType = ext switch
                {
                    ".master" => FileType.Master,
                    ".ascx" => FileType.Control,
                    _ => FileType.Page
                };

                var metadata = new FileMetadata
                {
                    SourceFilePath = input,
                    OutputFilePath = outputPath,
                    FileType = fileType,
                    OriginalContent = markupContent
                };

                var result = pipeline.TransformMarkup(markupContent, metadata);

                // Code-behind transform for convert command
                var codeBehindPath = input + ".cs";
                string? codeBehind = null;
                if (File.Exists(codeBehindPath))
                {
                    metadata.CodeBehindContent = await File.ReadAllTextAsync(codeBehindPath);
                    codeBehind = pipeline.TransformCodeBehind(metadata.CodeBehindContent, metadata);
                }

                Directory.CreateDirectory(outputDir);
                await File.WriteAllTextAsync(outputPath, result);

                if (codeBehind != null)
                {
                    var codeOutputPath = outputPath + ".cs";
                    await File.WriteAllTextAsync(codeOutputPath, codeBehind);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ {Path.GetFileName(input)} → {razorName}");
                if (codeBehind != null)
                    Console.WriteLine($"✓ {Path.GetFileName(codeBehindPath)} → {razorName}.cs");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, outputOption, overwriteOption);

        return convertCommand;
    }

    private static Command BuildPrescanCommand()
    {
        var prescanCommand = new Command("prescan", "Scan source files for common Web Forms migration patterns and emit a JSON summary");

        var inputOption = new Option<string>(
            aliases: ["--input", "-i"],
            description: "Source Web Forms project root (required)")
        { IsRequired = true };

        var reportOption = new Option<string?>(
            name: "--report",
            description: "Write prescan JSON output to file");

        prescanCommand.AddOption(inputOption);
        prescanCommand.AddOption(reportOption);

        prescanCommand.SetHandler(async (input, report) =>
        {
            try
            {
                using var sp = BuildServiceProvider();
                var analyzer = sp.GetRequiredService<PrescanAnalyzer>();
                var result = analyzer.Analyze(input);
                var json = PrescanAnalyzer.ToJson(result);

                if (!string.IsNullOrEmpty(report))
                {
                    var directory = Path.GetDirectoryName(report);
                    if (!string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);

                    await File.WriteAllTextAsync(report, json);
                }

                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, reportOption);

        return prescanCommand;
    }

    private static Command BuildScanCommand()
    {
        var scanCommand = new Command("scan", "Scan source files for migration patterns and emit a JSON summary");

        var inputOption = new Option<string>(
            aliases: ["--input", "-i"],
            description: "Source Web Forms project root (required)")
        { IsRequired = true };

        var reportOption = new Option<string?>(
            aliases: ["--output", "-o"],
            description: "Write scan JSON output to file");

        scanCommand.AddOption(inputOption);
        scanCommand.AddOption(reportOption);

        scanCommand.SetHandler(async (input, output) =>
        {
            try
            {
                using var sp = BuildServiceProvider();
                var analyzer = sp.GetRequiredService<PrescanAnalyzer>();
                var result = analyzer.Analyze(input);
                var json = PrescanAnalyzer.ToJson(result);

                if (!string.IsNullOrEmpty(output))
                {
                    var directory = Path.GetDirectoryName(output);
                    if (!string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);

                    await File.WriteAllTextAsync(output, json);
                }

                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, reportOption);

        return scanCommand;
    }

    private static Command BuildAssetsCommand()
    {
        var assetsCommand = new Command("assets", "Work with NuGet-backed static assets");
        var extractCommand = new Command("extract", "Extract CSS, JS, font, and image assets from NuGet packages");

        var inputOption = new Option<string>(
            aliases: ["--input", "-i"],
            description: "Source Web Forms project root (required)")
        { IsRequired = true };

        var outputOption = new Option<string>(
            aliases: ["--output", "-o"],
            description: "Output project root for manifest and extracted assets (required)")
        { IsRequired = true };

        var packagesPathOption = new Option<string?>(
            name: "--packages-path",
            description: "Optional explicit packages directory to search before the global cache");

        var manifestOnlyOption = new Option<bool>(
            name: "--manifest-only",
            description: "Generate asset-manifest.json and AssetReferences.html without copying files",
            getDefaultValue: () => false);

        extractCommand.AddOption(inputOption);
        extractCommand.AddOption(outputOption);
        extractCommand.AddOption(packagesPathOption);
        extractCommand.AddOption(manifestOnlyOption);

        extractCommand.SetHandler(async (input, output, packagesPath, manifestOnly) =>
        {
            try
            {
                using var sp = BuildServiceProvider();
                var extractor = sp.GetRequiredService<NativeNuGetStaticAssetExtractor>();
                var result = await extractor.ExtractAsync(new NuGetAssetExtractionOptions(input, output, packagesPath, manifestOnly));

                if (!result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"Error: {result.ErrorMessage}");
                    Console.ResetColor();
                    Environment.Exit(1);
                }

                if (result.Skipped)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(result.ErrorMessage);
                    Console.ResetColor();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"NuGet asset {(manifestOnly ? "analysis" : "extraction")} complete: {result.PackagesWithAssets} package(s), {result.TotalFilesExtracted} file(s).");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, outputOption, packagesPathOption, manifestOnlyOption);

        assetsCommand.AddCommand(extractCommand);
        return assetsCommand;
    }

    private static Command BuildEdmxCommand()
    {
        var edmxCommand = new Command("edmx", "Work with EDMX models");
        var convertCommand = new Command("convert", "Convert an EDMX file into EF Core entities and a DbContext");

        var inputOption = new Option<string>(
            aliases: ["--input", "-i"],
            description: "Path to the EDMX file (required)")
        { IsRequired = true };

        var outputOption = new Option<string>(
            aliases: ["--output", "-o"],
            description: "Output directory for generated .cs files (required)")
        { IsRequired = true };

        var namespaceOption = new Option<string?>(
            name: "--namespace",
            description: "Namespace for generated EF Core types");

        convertCommand.AddOption(inputOption);
        convertCommand.AddOption(outputOption);
        convertCommand.AddOption(namespaceOption);

        convertCommand.SetHandler(async (input, output, @namespace) =>
        {
            try
            {
                using var sp = BuildServiceProvider();
                var converter = sp.GetRequiredService<NativeEdmxToEfCoreConverter>();
                var result = await converter.ConvertAsync(new EdmxConversionOptions(input, output, @namespace));

                if (!result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"Error: {result.ErrorMessage}");
                    Console.ResetColor();
                    Environment.Exit(1);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"EDMX conversion complete: {result.EntitiesGenerated} entity file(s) and DbContext generated.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, outputOption, namespaceOption);

        edmxCommand.AddCommand(convertCommand);
        return edmxCommand;
    }
}

