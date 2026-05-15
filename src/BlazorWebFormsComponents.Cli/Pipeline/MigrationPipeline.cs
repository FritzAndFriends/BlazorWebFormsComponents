using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Scaffolding;
using BlazorWebFormsComponents.Cli.SemanticPatterns;
using BlazorWebFormsComponents.Cli.Transforms;
using Microsoft.Extensions.DependencyInjection;
using NativeEdmxToEfCoreConverter = BlazorWebFormsComponents.Cli.Services.EdmxToEfCoreConverter;
using NativeNuGetStaticAssetExtractor = BlazorWebFormsComponents.Cli.Services.NuGetStaticAssetExtractor;

namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Orchestrates the full file conversion pipeline: scaffold, config, markup transforms, code-behind transforms, output.
/// </summary>
public class MigrationPipeline
{
    private readonly IReadOnlyList<IMarkupTransform> _markupTransforms;
    private readonly IReadOnlyList<ICodeBehindTransform> _codeBehindTransforms;
    private readonly ProjectScaffolder _scaffolder;
    private readonly GlobalUsingsGenerator _globalUsings;
    private readonly ShimGenerator _shimGenerator;
    private readonly WebConfigTransformer _webConfigTransformer;
    private readonly OutputWriter _outputWriter;
    private readonly StaticFileCopier? _staticFileCopier;
    private readonly SourceFileCopier? _sourceFileCopier;
    private readonly AppStartCopier? _appStartCopier;
    private readonly AppAssetInjector? _appAssetInjector;
    private readonly NativeNuGetStaticAssetExtractor? _nuGetStaticAssetExtractor;
    private readonly NativeEdmxToEfCoreConverter? _edmxConverterBridge;
    private readonly RedirectHandlerAnnotator? _redirectHandlerAnnotator;
    private readonly SemanticPatternCatalog _semanticPatternCatalog;
    private readonly PageQuarantineDetector _pageQuarantineDetector;
    private ScaffoldResult? _lastScaffoldResult;

    private static readonly Regex ScaffoldClassNameRegex = new(
        @"(?:public|internal)\s+(?:(?:partial|static|sealed|abstract)\s+)*class\s+(?<name>[A-Za-z_]\w*)",
        RegexOptions.Compiled);

    /// <summary>
    /// Full constructor for DI — used by the CLI commands.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public MigrationPipeline(
        IEnumerable<IMarkupTransform> markupTransforms,
        IEnumerable<ICodeBehindTransform> codeBehindTransforms,
        SemanticPatternCatalog semanticPatternCatalog,
        ProjectScaffolder scaffolder,
        GlobalUsingsGenerator globalUsings,
        ShimGenerator shimGenerator,
        WebConfigTransformer webConfigTransformer,
        OutputWriter outputWriter,
        StaticFileCopier staticFileCopier,
        SourceFileCopier sourceFileCopier,
        AppStartCopier appStartCopier,
        AppAssetInjector appAssetInjector,
        NativeNuGetStaticAssetExtractor nuGetStaticAssetExtractor,
        NativeEdmxToEfCoreConverter edmxConverterBridge,
        RedirectHandlerAnnotator redirectHandlerAnnotator,
        PageQuarantineDetector pageQuarantineDetector)
    {
        _markupTransforms = markupTransforms.OrderBy(t => t.Order).ToList();
        _codeBehindTransforms = codeBehindTransforms.OrderBy(t => t.Order).ToList();
        _semanticPatternCatalog = semanticPatternCatalog;
        _scaffolder = scaffolder;
        _globalUsings = globalUsings;
        _shimGenerator = shimGenerator;
        _webConfigTransformer = webConfigTransformer;
        _outputWriter = outputWriter;
        _staticFileCopier = staticFileCopier;
        _sourceFileCopier = sourceFileCopier;
        _appStartCopier = appStartCopier;
        _appAssetInjector = appAssetInjector;
        _nuGetStaticAssetExtractor = nuGetStaticAssetExtractor;
        _edmxConverterBridge = edmxConverterBridge;
        _redirectHandlerAnnotator = redirectHandlerAnnotator;
        _pageQuarantineDetector = pageQuarantineDetector;
    }

    /// <summary>
    /// Lightweight constructor for transform-only usage (tests, single-file convert).
    /// </summary>
    public MigrationPipeline(
        IEnumerable<IMarkupTransform> markupTransforms,
        IEnumerable<ICodeBehindTransform> codeBehindTransforms,
        IEnumerable<ISemanticPattern>? semanticPatterns = null)
    {
        _markupTransforms = markupTransforms.OrderBy(t => t.Order).ToList();
        _codeBehindTransforms = codeBehindTransforms.OrderBy(t => t.Order).ToList();
        _semanticPatternCatalog = new SemanticPatternCatalog(semanticPatterns ?? []);
        _scaffolder = null!;
        _globalUsings = null!;
        _shimGenerator = null!;
        _webConfigTransformer = null!;
        _outputWriter = null!;
        _appStartCopier = null;
        _appAssetInjector = null;
        _nuGetStaticAssetExtractor = null;
        _edmxConverterBridge = null;
        _redirectHandlerAnnotator = null;
        _pageQuarantineDetector = new PageQuarantineDetector();
    }

    /// <summary>
    /// Run the full migration pipeline on all source files in the context.
    /// Sequence: 1) Scaffold project, 2) Transform config, 3) Transform source files, 4) Generate report.
    /// </summary>
    public async Task<MigrationReport> ExecuteAsync(MigrationContext context)
    {
        var report = new MigrationReport();
        _outputWriter.DryRun = context.Options.DryRun;
        _outputWriter.Verbose = context.Options.Verbose;
        _outputWriter.Reset();

        // Step 1: Scaffold project (if not --skip-scaffold)
        if (!context.Options.SkipScaffold)
        {
            await ScaffoldProjectAsync(context, report);
        }

        // Step 2: Transform config (web.config → appsettings.json)
        await TransformConfigAsync(context, report);

        // Step 3: For each source file — markup pipeline → code-behind pipeline → write output
        var quarantineEntries = new List<PageQuarantineManifestEntry>();
        foreach (var sourceFile in context.SourceFiles)
        {
            try
            {
                var quarantineEntry = await ProcessSourceFileAsync(sourceFile, context, report);
                if (quarantineEntry != null)
                {
                    quarantineEntries.Add(quarantineEntry);
                }
            }
            catch (Exception ex)
            {
                report.Errors.Add($"{sourceFile.MarkupPath}: {ex.Message}");
            }
        }

        await WriteQuarantineManifestAsync(context, quarantineEntries);

        // Step 4: Copy static files (CSS, JS, images, fonts) to wwwroot/
        if (_staticFileCopier != null)
        {
            report.StaticFilesCopied = _staticFileCopier.CopyStaticFiles(
                context.SourcePath, context.OutputPath, context.Options.Verbose);
        }

        var projectName = GetProjectName(context.SourcePath);

        // Step 5: Generate EF Core output from EDMX before source copy so original T4 artifacts can be skipped.
        ISet<string>? excludedSourceFiles = null;
        if (_edmxConverterBridge != null)
        {
            excludedSourceFiles = await _edmxConverterBridge.ConvertAsync(
                context.SourcePath,
                context.OutputPath,
                projectName,
                context.Options.DryRun,
                report);
        }

        // Step 6: Copy non-page source files (Models, Logic, etc.) with namespace transforms
        if (_sourceFileCopier != null)
        {
            // Extract class names from scaffolded .cs files to avoid duplicate stubs
            HashSet<string>? scaffoldedClassNames = null;
            if (_lastScaffoldResult != null)
            {
                scaffoldedClassNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (var file in _lastScaffoldResult.Files.Values)
                {
                    if (file.RelativePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (Match m in ScaffoldClassNameRegex.Matches(file.Content))
                        {
                            scaffoldedClassNames.Add(m.Groups["name"].Value);
                        }
                    }
                }
            }

            var copyResult = await _sourceFileCopier.CopySourceFilesAsync(
                context.SourcePath, context.OutputPath, context.SourceFiles, context.Options.Verbose, report, excludedSourceFiles, scaffoldedClassNames);
            report.FilesWritten += copyResult.TotalCount;

            // Post-process Program.cs to register discovered service classes in DI
            if (copyResult.DiscoveredServiceClasses.Count > 0 && !context.Options.SkipScaffold)
            {
                await InjectServiceRegistrationsAsync(context.OutputPath, copyResult.DiscoveredServiceClasses);
            }
        }

        // Step 7: Copy App_Start artifacts to the project root
        if (_appStartCopier != null)
        {
            var appStartCount = await _appStartCopier.CopyAsync(context.SourcePath, context.OutputPath, report);
            report.FilesWritten += appStartCount;
        }

        // Step 8: Extract package static assets
        if (_nuGetStaticAssetExtractor != null)
        {
            await _nuGetStaticAssetExtractor.ExtractAsync(
                context.SourcePath,
                context.OutputPath,
                context.Options.DryRun,
                report);
        }

        // Step 9: Inject CSS/JS references into App.razor
        if (_appAssetInjector != null && !context.Options.SkipScaffold)
        {
            await _appAssetInjector.InjectAsync(context.SourcePath, context.OutputPath);
        }

        // Step 10: Annotate Program.cs for redirect-only pages
        if (_redirectHandlerAnnotator != null)
        {
            await _redirectHandlerAnnotator.AnnotateAsync(context, report);
        }

        // Step 11: Generate report
        report.GeneratedFiles.AddRange(_outputWriter.WrittenFiles);
        report.FilesWritten = _outputWriter.WrittenFiles.Count;

        if (!string.IsNullOrEmpty(context.Options.ReportPath))
        {
            await report.WriteReportFileAsync(context.Options.ReportPath);
        }

        return report;
    }

    private async Task ScaffoldProjectAsync(MigrationContext context, MigrationReport report)
    {
        var projectName = GetProjectName(context.SourcePath);

        Console.WriteLine($"Scaffolding project: {projectName}");

        var scaffoldResult = _scaffolder.Scaffold(context.SourcePath, context.OutputPath, projectName);
        _lastScaffoldResult = scaffoldResult;
        await _scaffolder.WriteAsync(scaffoldResult, context.OutputPath, _outputWriter);
        report.ScaffoldFilesGenerated += scaffoldResult.Files.Count;

        // Generate GlobalUsings.cs
        await _globalUsings.WriteAsync(context.OutputPath, _outputWriter, scaffoldResult.HasIdentity);
        report.ScaffoldFilesGenerated++;

        // Generate shim files
        await _shimGenerator.WriteAsync(context.OutputPath, _outputWriter, scaffoldResult.HasIdentity);
        report.ScaffoldFilesGenerated++; // WebFormsShims.cs
        if (scaffoldResult.HasIdentity)
            report.ScaffoldFilesGenerated++; // IdentityShims.cs
    }

    private async Task TransformConfigAsync(MigrationContext context, MigrationReport report)
    {
        var configResult = _webConfigTransformer.Transform(context.SourcePath);
        if (configResult == null)
        {
            if (context.Options.Verbose)
                Console.WriteLine("  No Web.config settings found — skipping appsettings.json");
            return;
        }

        if (configResult.Error != null)
        {
            report.Warnings.Add(configResult.Error);
            return;
        }

        if (configResult.JsonContent != null)
        {
            var appSettingsPath = Path.Combine(context.OutputPath, "appsettings.json");
            await _outputWriter.WriteFileAsync(appSettingsPath, configResult.JsonContent,
                $"appsettings.json ({configResult.AppSettingsCount} app settings, {configResult.ConnectionStringsCount} connection strings)");

            if (configResult.ConnectionStringNames.Count > 0)
            {
                var connList = string.Join(", ", configResult.ConnectionStringNames);
                report.AddManualItem(
                    "Web.config",
                    0,
                    "bwfc-general",
                    $"ConnectionStrings extracted from Web.config — verify for target environment: {connList}",
                    "medium");
            }
        }
    }

    /// <summary>
    /// Post-processes Program.cs to add DI registrations for service classes that received
    /// constructor injection during source file transforms.
    /// </summary>
    private async Task InjectServiceRegistrationsAsync(
        string outputPath,
        IReadOnlyList<DiscoveredServiceClass> serviceClasses)
    {
        var programPath = Path.Combine(outputPath, "Program.cs");
        if (!File.Exists(programPath))
            return;

        var content = await File.ReadAllTextAsync(programPath);

        // Build the registration lines
        var registrations = new System.Text.StringBuilder();
        registrations.AppendLine();
        registrations.AppendLine("// Service classes discovered with constructor injection — registered for DI");
        foreach (var svc in serviceClasses)
        {
            registrations.AppendLine($"builder.Services.AddScoped<{svc.ClassName}>();");
        }

        // Insert before "builder.Services.AddBlazorWebFormsComponents();" (the BWFC anchor)
        const string anchor = "builder.Services.AddBlazorWebFormsComponents();";
        var anchorIndex = content.IndexOf(anchor, StringComparison.Ordinal);
        if (anchorIndex >= 0)
        {
            content = content[..anchorIndex] + registrations.ToString() + content[anchorIndex..];
        }
        else
        {
            // Fallback: insert before "var app = builder.Build();"
            const string buildAnchor = "var app = builder.Build();";
            var buildIndex = content.IndexOf(buildAnchor, StringComparison.Ordinal);
            if (buildIndex >= 0)
            {
                content = content[..buildIndex] + registrations.ToString() + "\n" + content[buildIndex..];
            }
        }

        // Add using statements for service class namespaces if needed
        foreach (var svc in serviceClasses)
        {
            if (svc.Namespace != null)
            {
                var usingLine = $"using {svc.Namespace};";
                if (!content.Contains(usingLine, StringComparison.Ordinal))
                {
                    // Insert after last using-directive (not using-block statements)
                    var lastUsing = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
                    if (lastUsing.Success)
                    {
                        var insertAt = lastUsing.Index + lastUsing.Length;
                        content = content[..insertAt] + "\n" + usingLine + content[insertAt..];
                    }
                }
            }
        }

        await File.WriteAllTextAsync(programPath, content);
        Console.WriteLine($"  Program.cs updated with {serviceClasses.Count} service class DI registration(s)");
    }

    private async Task<PageQuarantineManifestEntry?> ProcessSourceFileAsync(SourceFile sourceFile, MigrationContext context, MigrationReport report)
    {
        var markupContent = await File.ReadAllTextAsync(sourceFile.MarkupPath);
        var projectName = GetProjectName(context.SourcePath);

        var metadata = new FileMetadata
        {
            SourceFilePath = sourceFile.MarkupPath,
            OutputFilePath = sourceFile.OutputPath,
            FileType = sourceFile.FileType,
            OriginalContent = markupContent,
            OutputRootPath = context.OutputPath,
            SourceRootPath = context.SourcePath,
            ProjectNamespace = projectName
        };

        // Read code-behind if present
        if (sourceFile.HasCodeBehind)
        {
            metadata.CodeBehindContent = await File.ReadAllTextAsync(sourceFile.CodeBehindPath!);
        }
        else
        {
            // Generate a scaffold code-behind so transforms always have a target.
            // This eliminates the need for @code blocks in .razor files.
            var className = CodeBehindInjector.DeriveClassName(sourceFile.OutputPath);
            var subNamespace = CodeBehindInjector.DeriveSubNamespace(sourceFile.OutputPath, context.OutputPath);
            metadata.CodeBehindContent = CodeBehindInjector.GenerateScaffold(projectName, className, subNamespace);
            metadata.IsGeneratedCodeBehind = true;
        }

        // Markup pipeline
        var markup = markupContent;
        foreach (var transform in _markupTransforms)
        {
            markup = transform.Apply(markup, metadata);
        }

        // Set markup content for code-behind transforms to reference/modify
        metadata.MarkupContent = markup;

        // Code-behind pipeline (always runs — scaffold generated above when no source code-behind)
        var codeBehind = metadata.CodeBehindContent;
        if (codeBehind != null)
        {
            foreach (var transform in _codeBehindTransforms)
            {
                codeBehind = transform.Apply(codeBehind, metadata);
            }
        }

        // Use potentially modified markup content from code-behind transforms
        var semanticResult = _semanticPatternCatalog.Apply(
            context,
            sourceFile,
            metadata,
            metadata.MarkupContent ?? markup,
            codeBehind,
            report);

        var finalMarkup = semanticResult.Markup;
        codeBehind = semanticResult.CodeBehind;
        var quarantineDecision = metadata.QuarantineDecision;

        if (quarantineDecision?.ShouldQuarantine != true)
        {
            var emissionPlan = codeBehind == null
                ? null
                : PageCodeBehindEmissionPlanner.Create(metadata, finalMarkup, codeBehind);
            var lateDecision = _pageQuarantineDetector.AnalyzeLate(metadata, finalMarkup, codeBehind, emissionPlan);
            if (lateDecision.ShouldQuarantine)
            {
                quarantineDecision = lateDecision;
                metadata.QuarantineDecision = lateDecision;
                metadata.CompileSurfaceStubReason = lateDecision.Reason;
                metadata.CompileSurfaceOriginalCodeBehind = lateDecision.ArtifactContent;
            }
        }

        if (quarantineDecision?.ShouldQuarantine == true)
        {
            finalMarkup = quarantineDecision.StubMarkup;
            codeBehind = quarantineDecision.StubCodeBehind;
        }

        // Master page files are replaced by the scaffolded MainLayout.razor —
        // write them as artifacts only, not to the compile surface.
        if (sourceFile.FileType == FileType.Master)
        {
            var relativeMarkupPath = Path.GetRelativePath(context.OutputPath, sourceFile.OutputPath);
            var artifactPath = Path.Combine(
                context.OutputPath,
                "migration-artifacts",
                "codebehind",
                relativeMarkupPath + ".txt");
            await _outputWriter.WriteFileAsync(artifactPath, finalMarkup,
                $"Original master page markup for {Path.GetFileName(sourceFile.MarkupPath)}");

            if (codeBehind != null)
            {
                var cbArtifactPath = Path.Combine(
                    context.OutputPath,
                    "migration-artifacts",
                    "codebehind",
                    relativeMarkupPath + ".cs.txt");
                await _outputWriter.WriteFileAsync(cbArtifactPath, codeBehind,
                    $"Original master page code-behind for {Path.GetFileName(sourceFile.MarkupPath)}");
            }

            report.AddManualItem(relativeMarkupPath, 0, "bwfc-master-page",
                "Master page replaced by scaffolded MainLayout.razor — original preserved as artifact.", "info");
            return null;
        }

        // Mobile-specific user controls (e.g., ViewSwitcher.ascx) are not useful in Blazor —
        // responsive design replaces dedicated mobile views. Write as artifacts only.
        if (sourceFile.FileType == FileType.Control && IsMobileRelatedControl(sourceFile))
        {
            var relativeMarkupPath = Path.GetRelativePath(context.OutputPath, sourceFile.OutputPath);
            var artifactPath = Path.Combine(
                context.OutputPath,
                "migration-artifacts",
                "codebehind",
                relativeMarkupPath + ".txt");
            await _outputWriter.WriteFileAsync(artifactPath, finalMarkup,
                $"Mobile-specific control {Path.GetFileName(sourceFile.MarkupPath)} — preserved as artifact");

            if (codeBehind != null)
            {
                var cbArtifactPath = Path.Combine(
                    context.OutputPath,
                    "migration-artifacts",
                    "codebehind",
                    relativeMarkupPath + ".cs.txt");
                await _outputWriter.WriteFileAsync(cbArtifactPath, codeBehind,
                    $"Mobile-specific control code-behind for {Path.GetFileName(sourceFile.MarkupPath)}");
            }

            report.AddManualItem(relativeMarkupPath, 0, "bwfc-mobile-control",
                "Mobile-specific control excluded — use responsive design instead.", "info");
            return null;
        }

        // Write markup output
        await _outputWriter.WriteFileAsync(sourceFile.OutputPath, finalMarkup,
            $"Converted {Path.GetFileName(sourceFile.MarkupPath)}");

        PageQuarantineManifestEntry? manifestEntry = null;

        // Write code-behind output
        if (quarantineDecision?.ShouldQuarantine == true && codeBehind != null)
        {
            await _outputWriter.WriteFileAsync(sourceFile.OutputPath + ".cs", codeBehind,
                $"Quarantine stub for {Path.GetFileName(sourceFile.MarkupPath)}");

            var relativeMarkupPath = Path.GetRelativePath(context.OutputPath, sourceFile.OutputPath);
            var codeOutputPath = Path.Combine(
                context.OutputPath,
                "migration-artifacts",
                "codebehind",
                relativeMarkupPath + ".cs.txt");

            if (!string.IsNullOrWhiteSpace(quarantineDecision.ArtifactContent))
            {
                await _outputWriter.WriteFileAsync(codeOutputPath, quarantineDecision.ArtifactContent,
                    $"Original quarantined code-behind for {Path.GetFileName(sourceFile.MarkupPath)}");
            }

            report.AddManualItem(quarantineDecision.RelativeSourcePath, 0, "bwfc-compile-surface", quarantineDecision.Reason, "high");
            manifestEntry = quarantineDecision.ManifestEntry;
        }
        else if (codeBehind != null)
        {
            var emissionPlan = PageCodeBehindEmissionPlanner.Create(metadata, finalMarkup, codeBehind);
            var relativeMarkupPath = Path.GetRelativePath(context.OutputPath, sourceFile.OutputPath);
            var codeOutputPath = Path.Combine(
                context.OutputPath,
                "migration-artifacts",
                "codebehind",
                relativeMarkupPath + ".cs.txt");

            if (emissionPlan.EmitToCompileSurface)
            {
                await _outputWriter.WriteFileAsync(sourceFile.OutputPath + ".cs", codeBehind,
                    $"Converted code-behind for {Path.GetFileName(sourceFile.MarkupPath)}");

                if (!string.IsNullOrWhiteSpace(emissionPlan.ArtifactContent) && !string.IsNullOrWhiteSpace(emissionPlan.ArtifactReason))
                {
                    await _outputWriter.WriteFileAsync(codeOutputPath, emissionPlan.ArtifactContent,
                        $"Manual code-behind artifact for {Path.GetFileName(sourceFile.MarkupPath)}");
                    report.AddManualItem(Path.GetRelativePath(context.SourcePath, sourceFile.MarkupPath), 0, "bwfc-compile-surface", emissionPlan.ArtifactReason);
                }
            }
            else
            {
                // Save full transformed code-behind as artifact for manual review
                await _outputWriter.WriteFileAsync(codeOutputPath, codeBehind,
                    $"Manual code-behind artifact for {Path.GetFileName(sourceFile.MarkupPath)}");

                // Emit the full transformed code-behind to the compile surface anyway.
                // The "unsafe" patterns are typically runtime issues, not compile errors.
                // A file that compiles but needs L2 fixes is better than a skeleton missing members.
                await _outputWriter.WriteFileAsync(sourceFile.OutputPath + ".cs", codeBehind,
                    $"Code-behind (needs review) for {Path.GetFileName(sourceFile.MarkupPath)}");
                report.AddManualItem(Path.GetRelativePath(context.SourcePath, sourceFile.MarkupPath), 0, "bwfc-compile-surface",
                    $"{emissionPlan.ArtifactReason} Full code-behind emitted; review migration-artifacts/codebehind/ for original.");
            }
        }

        report.FilesProcessed++;
        return manifestEntry;
    }

    private async Task WriteQuarantineManifestAsync(MigrationContext context, IReadOnlyList<PageQuarantineManifestEntry> quarantineEntries)
    {
        if (quarantineEntries.Count == 0)
        {
            return;
        }

        var manifestPath = Path.Combine(context.OutputPath, "migration-artifacts", "quarantine-manifest.json");
        var manifestContent = _pageQuarantineDetector.BuildManifest(quarantineEntries);
        await _outputWriter.WriteFileAsync(manifestPath, manifestContent, "Compile-surface quarantine manifest");
    }

    /// <summary>
    /// Run only the markup pipeline on a single string (useful for single-file convert).
    /// </summary>
    public string TransformMarkup(string content, FileMetadata metadata)
    {
        foreach (var transform in _markupTransforms)
        {
            content = transform.Apply(content, metadata);
        }

        metadata.MarkupContent = content;
        return content;
    }

    /// <summary>
    /// Run only the code-behind pipeline on a single string.
    /// </summary>
    public string TransformCodeBehind(string content, FileMetadata metadata)
    {
        foreach (var transform in _codeBehindTransforms)
        {
            content = transform.Apply(content, metadata);
        }
        return content;
    }

    private static string GetProjectName(string sourcePath)
    {
        var projectName = Path.GetFileName(sourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        return string.IsNullOrEmpty(projectName) ? "MigratedApp" : projectName;
    }

    private static bool IsMobileRelatedControl(SourceFile sourceFile)
    {
        var fileName = Path.GetFileNameWithoutExtension(sourceFile.MarkupPath);
        // ViewSwitcher and similar mobile-only controls
        if (fileName.Equals("ViewSwitcher", StringComparison.OrdinalIgnoreCase))
            return true;
        // Controls in Mobile directories or with .Mobile. in name
        var relativePath = sourceFile.MarkupPath;
        return relativePath.Contains("Mobile", StringComparison.OrdinalIgnoreCase)
            && (relativePath.Contains("/Mobile/", StringComparison.OrdinalIgnoreCase)
                || relativePath.Contains("\\Mobile\\", StringComparison.OrdinalIgnoreCase)
                || relativePath.Contains(".Mobile.", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Builds a minimal skeleton code-behind (.razor.cs) for a page whose full
    /// transformed code-behind was deemed unsafe for the compile surface.
    /// The skeleton contains the class declaration, base class, and [Parameter] properties
    /// extracted from the @page route template.
    /// </summary>
    private static string? BuildSkeletonCodeBehind(FileMetadata metadata, string markup)
    {
        // Extract class name from original code-behind
        var className = ExtractClassName(metadata);
        if (className == null)
            return null;

        // Extract namespace
        var namespaceName = ExtractNamespace(metadata);

        // Extract route parameters from @page directives
        var routeParams = ExtractRouteParametersFromMarkup(markup);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("using BlazorWebFormsComponents;");
        sb.AppendLine("using Microsoft.AspNetCore.Components;");
        sb.AppendLine();

        if (namespaceName != null)
        {
            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
        }

        sb.AppendLine($"/// <summary>");
        sb.AppendLine($"/// Skeleton code-behind. Full transformed source is in migration-artifacts/codebehind/.");
        sb.AppendLine($"/// </summary>");
        sb.AppendLine($"public partial class {className} : WebFormsPageBase");
        sb.AppendLine("{");

        foreach (var (name, csharpType) in routeParams)
        {
            sb.AppendLine($"    [Parameter] public {csharpType} {name} {{ get; set; }}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string? ExtractClassName(FileMetadata metadata)
    {
        var content = metadata.CodeBehindContent ?? metadata.OriginalContent ?? "";
        var match = Regex.Match(content, @"(?:public|internal)\s+(?:partial\s+)?class\s+(?<name>[A-Za-z_]\w*)");
        if (match.Success) return match.Groups["name"].Value;

        // Fall back to file name
        var fileName = Path.GetFileNameWithoutExtension(metadata.OutputFilePath ?? metadata.SourceFilePath ?? "");
        return string.IsNullOrEmpty(fileName) ? null : fileName;
    }

    private static string? ExtractNamespace(FileMetadata metadata)
    {
        var content = metadata.CodeBehindContent ?? "";
        var match = Regex.Match(content, @"namespace\s+(?<ns>[A-Za-z_][\w.]*)\s*(?:\{|;)");
        return match.Success ? match.Groups["ns"].Value : metadata.ProjectNamespace;
    }

    private static List<(string Name, string CsharpType)> ExtractRouteParametersFromMarkup(string markup)
    {
        var result = new List<(string, string)>();
        var pageRouteRegex = new Regex(@"@page\s+""[^""]*\{(?<param>\w+)(?::(?<constraint>\w+))?\}[^""]*""");
        foreach (Match m in pageRouteRegex.Matches(markup))
        {
            var name = m.Groups["param"].Value;
            var constraint = m.Groups["constraint"].Success ? m.Groups["constraint"].Value : null;
            var csharpType = constraint switch
            {
                "int" => "int?",
                "long" => "long?",
                "bool" => "bool?",
                "double" => "double?",
                "decimal" => "decimal?",
                _ => "string?"
            };
            if (!result.Any(p => p.Item1.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add((name, csharpType));
            }
        }
        return result;
    }
}
