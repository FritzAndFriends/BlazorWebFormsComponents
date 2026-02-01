using System.Text;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Services;

/// <summary>
/// Converts ASP.NET Web Forms user controls (.ascx) to Blazor Razor components (.razor)
/// </summary>
public class AscxToRazorConverter
{
    private readonly bool _useAi;
    private readonly AiAssistant? _aiAssistant;

    public AscxToRazorConverter(bool useAi = false)
    {
        _useAi = useAi;
        if (_useAi)
        {
            _aiAssistant = new AiAssistant();
        }
    }

    public async Task ConvertAsync(string inputPath, string? outputPath, bool recursive, bool overwrite)
    {
        if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
        {
            throw new FileNotFoundException($"Input path not found: {inputPath}");
        }

        outputPath ??= inputPath;

        if (File.Exists(inputPath))
        {
            // Single file conversion
            await ConvertFileAsync(inputPath, outputPath, overwrite);
        }
        else if (Directory.Exists(inputPath))
        {
            // Directory conversion
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var ascxFiles = Directory.GetFiles(inputPath, "*.ascx", searchOption);

            if (ascxFiles.Length == 0)
            {
                Console.WriteLine($"No .ascx files found in {inputPath}");
                return;
            }

            Console.WriteLine($"Found {ascxFiles.Length} .ascx file(s) to convert...\n");

            foreach (var ascxFile in ascxFiles)
            {
                var relativeDir = Path.GetDirectoryName(Path.GetRelativePath(inputPath, ascxFile)) ?? "";
                var outputDir = Path.Combine(outputPath, relativeDir);
                Directory.CreateDirectory(outputDir);

                await ConvertFileAsync(ascxFile, outputDir, overwrite);
            }

            Console.WriteLine($"\nConversion complete! Processed {ascxFiles.Length} file(s).");
        }
    }

    private async Task ConvertFileAsync(string ascxFilePath, string outputDirectory, bool overwrite)
    {
        var fileName = Path.GetFileNameWithoutExtension(ascxFilePath);
        var razorFileName = $"{fileName}.razor";
        var outputFilePath = Path.IsPathRooted(outputDirectory) && Directory.Exists(outputDirectory)
            ? Path.Combine(outputDirectory, razorFileName)
            : Path.Combine(Path.GetDirectoryName(ascxFilePath) ?? "", razorFileName);

        if (File.Exists(outputFilePath) && !overwrite)
        {
            Console.Write($"File {razorFileName} already exists. Overwrite? (y/n): ");
            var response = Console.ReadLine()?.ToLower();
            if (response != "y" && response != "yes")
            {
                Console.WriteLine($"Skipped: {ascxFilePath}");
                return;
            }
        }

        Console.WriteLine($"Converting: {Path.GetFileName(ascxFilePath)} -> {razorFileName}");

        var ascxContent = await File.ReadAllTextAsync(ascxFilePath);
        var razorContent = await ConvertContentAsync(ascxContent, fileName);

        await File.WriteAllTextAsync(outputFilePath, razorContent);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ Created: {outputFilePath}");
        Console.ResetColor();
    }

    private async Task<string> ConvertContentAsync(string ascxContent, string componentName)
    {
        var result = new StringBuilder();

        // Step 1: Convert control directive to @inherits or remove it
        var controlDirective = ExtractControlDirective(ascxContent);
        ascxContent = RemoveControlDirective(ascxContent);

        if (controlDirective.CodeBehindClass != null)
        {
            result.AppendLine($"@inherits {controlDirective.CodeBehindClass}");
            result.AppendLine();
        }

        // Step 2: Convert markup
        var convertedMarkup = ConvertMarkup(ascxContent);

        // Step 3: Use AI enhancement if enabled
        if (_useAi && _aiAssistant != null)
        {
            Console.WriteLine("  Using AI to enhance conversion...");
            convertedMarkup = await _aiAssistant.EnhanceConversionAsync(convertedMarkup, componentName);
        }

        result.Append(convertedMarkup);

        return result.ToString();
    }

    private ControlDirective ExtractControlDirective(string ascxContent)
    {
        var directive = new ControlDirective();
        
        // Match <%@ Control ... %> directive
        var match = Regex.Match(ascxContent, @"<%@\s*Control\s+([^%>]+)%>", RegexOptions.IgnoreCase);
        if (!match.Success)
            return directive;

        var attributes = match.Groups[1].Value;

        // Extract Inherits attribute
        var inheritsMatch = Regex.Match(attributes, @"Inherits\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
        if (inheritsMatch.Success)
        {
            directive.CodeBehindClass = inheritsMatch.Groups[1].Value;
        }

        // Extract Language attribute
        var languageMatch = Regex.Match(attributes, @"Language\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
        if (languageMatch.Success)
        {
            directive.Language = languageMatch.Groups[1].Value;
        }

        return directive;
    }

    private string RemoveControlDirective(string ascxContent)
    {
        return Regex.Replace(ascxContent, @"<%@\s*Control\s+[^%>]+%>\s*", "", RegexOptions.IgnoreCase);
    }

    private string ConvertMarkup(string markup)
    {
        var result = markup;

        // Convert ASP.NET server controls to Blazor components
        // Remove runat="server" attributes
        result = Regex.Replace(result, @"\s+runat\s*=\s*[""']server[""']", "", RegexOptions.IgnoreCase);

        // Convert asp: prefix to component names (remove asp:)
        // Examples: <asp:Button -> <Button, <asp:Label -> <Label
        result = Regex.Replace(result, @"<asp:(\w+)", "<$1", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"</asp:(\w+)", "</$1", RegexOptions.IgnoreCase);

        // Convert <%# data binding expressions (in templates) - convert to context
        result = Regex.Replace(result, @"<%#\s*([^%]+?)\s*%>", "@($1)");

        // Convert <%: expressions to @() in Blazor
        // <%: is the HTML encoding syntax in ASP.NET
        result = Regex.Replace(result, @"<%:\s*([^%]+?)\s*%>", "@($1)");

        // Convert <%= expressions to @()
        result = Regex.Replace(result, @"<%=\s*([^%]+?)\s*%>", "@($1)");

        // Convert <% code blocks to @{ } - requires careful handling
        result = Regex.Replace(result, @"<%\s*([^%=:]+?)\s*%>", "@{ $1 }");

        // Fix common template issues: Item should be context in Blazor
        // This is a known migration issue mentioned in the requirements
        result = result.Replace("Item.", "context.");

        // Convert ID attribute to id (lowercase, which is standard in HTML5/Blazor)
        // But preserve the original casing in other attributes
        // Note: Blazor components typically use @ref instead of ID

        return result.Trim();
    }

    private class ControlDirective
    {
        public string? CodeBehindClass { get; set; }
        public string? Language { get; set; }
    }
}
