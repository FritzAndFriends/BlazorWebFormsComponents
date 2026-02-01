using System.CommandLine;
using BlazorWebFormsComponents.Cli.Services;

namespace BlazorWebFormsComponents.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("WebForms to Blazor - Convert ASP.NET Web Forms user controls (.ascx) to Blazor Razor components")
        {
            Name = "webforms-to-blazor"
        };

        var inputOption = new Option<string>(
            aliases: new[] { "--input", "-i" },
            description: "Path to .ascx file or directory containing .ascx files to convert")
        {
            IsRequired = true
        };

        var outputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            description: "Output directory for converted Razor components (defaults to input directory)");

        var recursiveOption = new Option<bool>(
            aliases: new[] { "--recursive", "-r" },
            description: "Recursively process all .ascx files in subdirectories",
            getDefaultValue: () => false);

        var overwriteOption = new Option<bool>(
            aliases: new[] { "--overwrite", "-f" },
            description: "Overwrite existing .razor files without prompting",
            getDefaultValue: () => false);

        var aiOption = new Option<bool>(
            aliases: new[] { "--use-ai" },
            description: "Use GitHub Copilot/OpenAI for enhanced conversion (requires GITHUB_TOKEN or OPENAI_API_KEY environment variable)",
            getDefaultValue: () => false);

        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(recursiveOption);
        rootCommand.AddOption(overwriteOption);
        rootCommand.AddOption(aiOption);

        rootCommand.SetHandler(async (input, output, recursive, overwrite, useAi) =>
        {
            try
            {
                var converter = new AscxToRazorConverter(useAi);
                await converter.ConvertAsync(input, output, recursive, overwrite);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }, inputOption, outputOption, recursiveOption, overwriteOption, aiOption);

        return await rootCommand.InvokeAsync(args);
    }
}
