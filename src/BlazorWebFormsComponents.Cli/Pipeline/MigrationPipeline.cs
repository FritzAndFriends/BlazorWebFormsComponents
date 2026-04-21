using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Io;
using BlazorWebFormsComponents.Cli.Scaffolding;
using BlazorWebFormsComponents.Cli.Transforms;

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

    /// <summary>
    /// Full constructor for DI — used by the CLI commands.
    /// </summary>
    public MigrationPipeline(
        IEnumerable<IMarkupTransform> markupTransforms,
        IEnumerable<ICodeBehindTransform> codeBehindTransforms,
        ProjectScaffolder scaffolder,
        GlobalUsingsGenerator globalUsings,
        ShimGenerator shimGenerator,
        WebConfigTransformer webConfigTransformer,
        OutputWriter outputWriter,
        StaticFileCopier staticFileCopier,
        SourceFileCopier sourceFileCopier)
    {
        _markupTransforms = markupTransforms.OrderBy(t => t.Order).ToList();
        _codeBehindTransforms = codeBehindTransforms.OrderBy(t => t.Order).ToList();
        _scaffolder = scaffolder;
        _globalUsings = globalUsings;
        _shimGenerator = shimGenerator;
        _webConfigTransformer = webConfigTransformer;
        _outputWriter = outputWriter;
        _staticFileCopier = staticFileCopier;
        _sourceFileCopier = sourceFileCopier;
    }

    /// <summary>
    /// Lightweight constructor for transform-only usage (tests, single-file convert).
    /// </summary>
    public MigrationPipeline(
        IEnumerable<IMarkupTransform> markupTransforms,
        IEnumerable<ICodeBehindTransform> codeBehindTransforms)
    {
        _markupTransforms = markupTransforms.OrderBy(t => t.Order).ToList();
        _codeBehindTransforms = codeBehindTransforms.OrderBy(t => t.Order).ToList();
        _scaffolder = null!;
        _globalUsings = null!;
        _shimGenerator = null!;
        _webConfigTransformer = null!;
        _outputWriter = null!;
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
        foreach (var sourceFile in context.SourceFiles)
        {
            try
            {
                await ProcessSourceFileAsync(sourceFile, context, report);
            }
            catch (Exception ex)
            {
                report.Errors.Add($"{sourceFile.MarkupPath}: {ex.Message}");
            }
        }

        // Step 4: Copy static files (CSS, JS, images, fonts) to wwwroot/
        if (_staticFileCopier != null)
        {
            report.StaticFilesCopied = _staticFileCopier.CopyStaticFiles(
                context.SourcePath, context.OutputPath, context.Options.Verbose);
        }

        // Step 5: Copy non-page source files (Models, Logic, etc.) with namespace transforms
        if (_sourceFileCopier != null)
        {
            var sourceCount = await _sourceFileCopier.CopySourceFilesAsync(
                context.SourcePath, context.OutputPath, context.SourceFiles, context.Options.Verbose);
            report.FilesWritten += sourceCount;
        }

        // Step 6: Generate report
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
        var projectName = Path.GetFileName(context.SourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrEmpty(projectName))
            projectName = "MigratedApp";

        Console.WriteLine($"Scaffolding project: {projectName}");

        var scaffoldResult = _scaffolder.Scaffold(context.SourcePath, context.OutputPath, projectName);
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

    private async Task ProcessSourceFileAsync(SourceFile sourceFile, MigrationContext context, MigrationReport report)
    {
        var markupContent = await File.ReadAllTextAsync(sourceFile.MarkupPath);
        var metadata = new FileMetadata
        {
            SourceFilePath = sourceFile.MarkupPath,
            OutputFilePath = sourceFile.OutputPath,
            FileType = sourceFile.FileType,
            OriginalContent = markupContent
        };

        // Read code-behind if present
        if (sourceFile.HasCodeBehind)
        {
            metadata.CodeBehindContent = await File.ReadAllTextAsync(sourceFile.CodeBehindPath!);
        }

        // Markup pipeline
        var markup = markupContent;
        foreach (var transform in _markupTransforms)
        {
            markup = transform.Apply(markup, metadata);
        }

        // Set markup content for code-behind transforms to reference/modify
        metadata.MarkupContent = markup;

        // Code-behind pipeline
        string? codeBehind = null;
        if (sourceFile.HasCodeBehind && metadata.CodeBehindContent != null)
        {
            codeBehind = metadata.CodeBehindContent;
            foreach (var transform in _codeBehindTransforms)
            {
                codeBehind = transform.Apply(codeBehind, metadata);
            }
        }

        // Use potentially modified markup content from code-behind transforms
        var finalMarkup = metadata.MarkupContent ?? markup;

        // Write markup output
        await _outputWriter.WriteFileAsync(sourceFile.OutputPath, finalMarkup,
            $"Converted {Path.GetFileName(sourceFile.MarkupPath)}");

        // Write code-behind output
        if (codeBehind != null)
        {
            var codeOutputPath = sourceFile.OutputPath + ".cs";
            await _outputWriter.WriteFileAsync(codeOutputPath, codeBehind,
                $"Code-behind for {Path.GetFileName(sourceFile.MarkupPath)}");
        }

        report.FilesProcessed++;
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
}
