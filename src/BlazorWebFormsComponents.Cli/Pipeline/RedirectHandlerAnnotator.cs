using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Detects redirect-only pages and annotates Program.cs with minimal API migration TODOs.
/// </summary>
public class RedirectHandlerAnnotator
{
    private readonly OutputWriter _outputWriter;

    public RedirectHandlerAnnotator(OutputWriter outputWriter)
    {
        _outputWriter = outputWriter;
    }

    public async Task<int> AnnotateAsync(MigrationContext context, MigrationReport report)
    {
        if (context.Options.SkipScaffold)
            return 0;

        var handlers = new List<string>();
        foreach (var sourceFile in context.SourceFiles.Where(f => f.FileType == FileType.Page && f.HasCodeBehind))
        {
            var markupContent = await File.ReadAllTextAsync(sourceFile.MarkupPath);
            if (!IsRedirectHandler(markupContent, sourceFile.CodeBehindPath!))
                continue;

            var pageName = Path.GetFileNameWithoutExtension(sourceFile.MarkupPath);
            handlers.Add(pageName);
            report.AddManualItem(Path.GetRelativePath(context.SourcePath, sourceFile.MarkupPath), 0, "RedirectHandler", $"{pageName} was a redirect handler (Response.Redirect in code-behind) — convert to minimal API endpoint");
        }

        if (handlers.Count == 0)
            return 0;

        var programPath = Path.Combine(context.OutputPath, "Program.cs");
        if (!File.Exists(programPath))
            return handlers.Count;

        var programContent = await File.ReadAllTextAsync(programPath);
        var comments = string.Join(Environment.NewLine, handlers.Select(handler => $"// TODO: {handler} was a redirect handler — convert to minimal API endpoint"));
        var insertion = $"// --- Redirect Handler Pages (convert to minimal API endpoints) ---{Environment.NewLine}{comments}{Environment.NewLine}{Environment.NewLine}";

        if (!programContent.Contains("// --- Redirect Handler Pages", StringComparison.Ordinal))
        {
            programContent = programContent.Replace("app.Run();", insertion + "app.Run();", StringComparison.Ordinal);
            await _outputWriter.WriteFileAsync(programPath, programContent, "Annotate Program.cs with redirect handler TODOs");
        }

        return handlers.Count;
    }

    private static bool IsRedirectHandler(string markupContent, string codeBehindPath)
    {
        if (!File.Exists(codeBehindPath))
            return false;

        var codeBehind = File.ReadAllText(codeBehindPath);
        if (!codeBehind.Contains("Response.Redirect", StringComparison.Ordinal))
            return false;

        var stripped = Regex.Replace(markupContent, @"<%@\s*\w+[^%]*%>\s*", string.Empty).Trim();
        return stripped.Length < 100;
    }
}
