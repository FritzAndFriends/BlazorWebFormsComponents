using BlazorWebFormsComponents.Cli.Config;

namespace BlazorWebFormsComponents.Cli.Tests;

public class PrescanAnalyzerTests
{
    [Fact]
    public void PrescanAnalyzer_FindsCommonMigrationPatterns()
    {
        var dir = TestHelpers.CreateTempProjectDir("prescan");
        try
        {
            File.WriteAllText(Path.Combine(dir, "Sample.cs"), """
                public class Sample
                {
                    public string Name { get; set; }

                    public void Go()
                    {
                        if (IsPostBack)
                        {
                            Response.Redirect("~/Done.aspx");
                        }

                        var x = ViewState["key"];
                    }
                }
                """);

            var analyzer = new PrescanAnalyzer();
            var result = analyzer.Analyze(dir);

            Assert.True(result.TotalFiles >= 1);
            Assert.True(result.FilesWithMatches >= 1);
            Assert.Contains("BWFC002", result.Summary.Keys);
            Assert.Contains("BWFC003", result.Summary.Keys);
            Assert.Contains("BWFC004", result.Summary.Keys);
        }
        finally
        {
            TestHelpers.CleanupTempDir(dir);
        }
    }

    [Fact]
    public void PrescanAnalyzer_ToJson_ProducesExpectedShape()
    {
        var result = new PrescanResult
        {
            SourcePath = @"D:\src\LegacyApp",
            TotalFiles = 1,
            FilesWithMatches = 1,
            TotalMatches = 2
        };
        result.Summary["BWFC003"] = new PrescanSummary("IsPostBack", "IsPostBack checks", 2, 1);
        result.Files.Add(new PrescanFileResult("Default.aspx.cs", [new PrescanFileMatch("BWFC003", "IsPostBack", 2, [10, 14])]));

        var json = PrescanAnalyzer.ToJson(result);

        Assert.Contains("\"SourcePath\"", json);
        Assert.Contains("\"BWFC003\"", json);
        Assert.Contains("\"Default.aspx.cs\"", json);
    }
}
