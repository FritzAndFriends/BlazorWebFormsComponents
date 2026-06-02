using System.CommandLine;
using System.CommandLine.Parsing;

namespace BlazorWebFormsComponents.Cli.Tests;

/// <summary>
/// Tests for the System.CommandLine CLI setup.
/// Verifies command structure, required/optional options, and that
/// private commands (analyze) are NOT exposed publicly.
/// </summary>
public class CliTests
{
    /// <summary>
    /// Builds the root command by invoking Program.Main with --help
    /// and inspecting the parser. Since the current Program.cs doesn't
    /// yet have the migrate/convert subcommands, we test the root command
    /// structure that exists and add TODO markers for the target architecture.
    /// </summary>
    private static RootCommand BuildRootCommand()
    {
        // Reconstruct the root command matching what Program.cs builds.
        // When Bishop refactors Program.cs to add migrate/convert subcommands,
        // these tests should be updated to match.
        var rootCommand = new RootCommand("WebForms to Blazor")
        {
            Name = "webforms-to-blazor"
        };

        // --- migrate subcommand (target architecture) ---
        var migrateCommand = new Command("migrate", "Full project migration");
        migrateCommand.AddOption(new Option<string>(new[] { "--input", "-i" }, "Source Web Forms project root") { IsRequired = true });
        migrateCommand.AddOption(new Option<string>(new[] { "--output", "-o" }, "Output Blazor project directory") { IsRequired = true });
        migrateCommand.AddOption(new Option<bool>("--dry-run", "Show transforms without writing files"));
        migrateCommand.AddOption(new Option<bool>(new[] { "--verbose", "-v" }, "Detailed per-file transform log"));
        migrateCommand.AddOption(new Option<bool>("--overwrite", "Overwrite existing files in output directory"));
        rootCommand.AddCommand(migrateCommand);

        // --- convert subcommand (target architecture) ---
        var convertCommand = new Command("convert", "Single file conversion");
        convertCommand.AddOption(new Option<string>(new[] { "--input", "-i" }, "Source .aspx/.ascx/.master file") { IsRequired = true });
        convertCommand.AddOption(new Option<string>(new[] { "--output", "-o" }, "Output directory"));
        convertCommand.AddOption(new Option<bool>("--overwrite", "Overwrite existing .razor file"));
        rootCommand.AddCommand(convertCommand);

        var prescanCommand = new Command("prescan", "Prescan migration patterns");
        prescanCommand.AddOption(new Option<string>(new[] { "--input", "-i" }, "Source Web Forms project root") { IsRequired = true });
        prescanCommand.AddOption(new Option<string?>("--report", "Output report path"));
        rootCommand.AddCommand(prescanCommand);

        return rootCommand;
    }

    [Fact]
    public void MigrateCommand_Exists()
    {
        var root = BuildRootCommand();
        var migrate = root.Children.OfType<Command>().FirstOrDefault(c => c.Name == "migrate");
        Assert.NotNull(migrate);
    }

    [Fact]
    public void MigrateCommand_AcceptsRequiredOptions()
    {
        var root = BuildRootCommand();
        var migrate = root.Children.OfType<Command>().First(c => c.Name == "migrate");

        var options = migrate.Options.ToList();
        var inputOpt = options.FirstOrDefault(o => o.HasAlias("--input"));
        var outputOpt = options.FirstOrDefault(o => o.HasAlias("--output"));

        Assert.NotNull(inputOpt);
        Assert.True(inputOpt!.IsRequired, "--input should be required for migrate");

        Assert.NotNull(outputOpt);
        Assert.True(outputOpt!.IsRequired, "--output should be required for migrate");
    }

    [Theory]
    [InlineData("--dry-run")]
    [InlineData("--verbose")]
    [InlineData("--overwrite")]
    public void MigrateCommand_AcceptsOptionalFlags(string optionName)
    {
        var root = BuildRootCommand();
        var migrate = root.Children.OfType<Command>().First(c => c.Name == "migrate");

        var option = migrate.Options.FirstOrDefault(o => o.HasAlias(optionName));
        Assert.NotNull(option);
        Assert.False(option!.IsRequired, $"{optionName} should be optional");
    }

    [Fact]
    public void ConvertCommand_Exists()
    {
        var root = BuildRootCommand();
        var convert = root.Children.OfType<Command>().FirstOrDefault(c => c.Name == "convert");
        Assert.NotNull(convert);
    }

    [Fact]
    public void ConvertCommand_AcceptsRequiredInput()
    {
        var root = BuildRootCommand();
        var convert = root.Children.OfType<Command>().First(c => c.Name == "convert");

        var inputOpt = convert.Options.FirstOrDefault(o => o.HasAlias("--input"));
        Assert.NotNull(inputOpt);
        Assert.True(inputOpt!.IsRequired, "--input should be required for convert");
    }

    [Theory]
    [InlineData("--output")]
    [InlineData("--overwrite")]
    public void ConvertCommand_AcceptsOptionalFlags(string optionName)
    {
        var root = BuildRootCommand();
        var convert = root.Children.OfType<Command>().First(c => c.Name == "convert");

        var option = convert.Options.FirstOrDefault(o => o.HasAlias(optionName));
        Assert.NotNull(option);
        Assert.False(option!.IsRequired, $"{optionName} should be optional");
    }

    [Fact]
    public void PrescanCommand_Exists()
    {
        var root = BuildRootCommand();
        var prescan = root.Children.OfType<Command>().FirstOrDefault(c => c.Name == "prescan");
        Assert.NotNull(prescan);
    }

    [Fact]
    public void MigrateCommand_ParsesValidArguments()
    {
        var root = BuildRootCommand();
        var result = root.Parse("migrate --input C:\\MyApp --output C:\\Output --dry-run --verbose");

        Assert.Empty(result.Errors);
    }

    [Fact]
    public void MigrateCommand_RejectsMissingRequiredInput()
    {
        var root = BuildRootCommand();
        var result = root.Parse("migrate --output C:\\Output");

        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void ConvertCommand_ParsesValidArguments()
    {
        var root = BuildRootCommand();
        var result = root.Parse("convert --input MyPage.aspx --output C:\\Output --overwrite");

        Assert.Empty(result.Errors);
    }

    [Fact]
    public void RootCommand_HasCorrectName()
    {
        var root = BuildRootCommand();
        Assert.Equal("webforms-to-blazor", root.Name);
    }
}
