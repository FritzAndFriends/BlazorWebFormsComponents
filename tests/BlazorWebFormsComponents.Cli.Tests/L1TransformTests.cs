namespace BlazorWebFormsComponents.Cli.Tests;

using BlazorWebFormsComponents.Cli.Pipeline;

/// <summary>
/// Parameterized L1 transform acceptance tests.
/// Each TC* test case reads input .aspx (and optional .aspx.cs), runs the full
/// transform pipeline, and compares against expected .razor (and optional .razor.cs).
///
/// These are the gate tests — the C# tool MUST pass all cases before the
/// PowerShell migration script is deprecated.
/// </summary>
public class L1TransformTests
{
    private static readonly string TestDataRoot = TestHelpers.GetTestDataRoot();
    private readonly MigrationPipeline _pipeline = TestHelpers.CreateDefaultPipeline();

    /// <summary>
    /// Provides all markup test case names for [Theory] parameterization.
    /// Discovers TC* files from TestData/inputs/*.aspx.
    /// </summary>
    public static IEnumerable<object[]> GetMarkupTestCases()
    {
        return TestHelpers.DiscoverTestCases()
            .Select(name => new object[] { name });
    }

    /// <summary>
    /// Provides code-behind test case names (only those with .aspx.cs + .razor.cs pairs).
    /// </summary>
    public static IEnumerable<object[]> GetCodeBehindTestCases()
    {
        return TestHelpers.DiscoverCodeBehindTestCases()
            .Select(name => new object[] { name });
    }

    [Theory]
    [MemberData(nameof(GetMarkupTestCases))]
    public void L1Transform_ProducesExpectedMarkup(string testCaseName)
    {
        // Arrange — determine input file (could be .aspx or .master)
        var inputPath = Path.Combine(TestDataRoot, "inputs", $"{testCaseName}.aspx");
        if (!File.Exists(inputPath))
            inputPath = Path.Combine(TestDataRoot, "inputs", $"{testCaseName}.master");

        var expectedPath = Path.Combine(TestDataRoot, "expected", $"{testCaseName}.razor");

        Assert.True(File.Exists(inputPath), $"Input file not found: {inputPath}");
        Assert.True(File.Exists(expectedPath), $"Expected file not found: {expectedPath}");

        var input = File.ReadAllText(inputPath);
        var expected = TestHelpers.NormalizeContent(File.ReadAllText(expectedPath));

        // Act
        var ext = Path.GetExtension(inputPath).ToLowerInvariant();
        var fileType = ext switch
        {
            ".master" => FileType.Master,
            ".ascx" => FileType.Control,
            _ => FileType.Page
        };
        var metadata = new FileMetadata
        {
            SourceFilePath = inputPath,
            OutputFilePath = ext == ".master"
                ? inputPath.Replace(".master", ".razor")
                : inputPath.Replace(".aspx", ".razor"),
            FileType = fileType,
            OriginalContent = input
        };

        var result = _pipeline.TransformMarkup(input, metadata);
        var actual = TestHelpers.NormalizeContent(result);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetCodeBehindTestCases))]
    public void L1Transform_ProducesExpectedCodeBehind(string testCaseName)
    {
        // Arrange
        var inputCsPath = Path.Combine(TestDataRoot, "inputs", $"{testCaseName}.aspx.cs");
        var expectedCsPath = Path.Combine(TestDataRoot, "expected", $"{testCaseName}.razor.cs");

        Assert.True(File.Exists(inputCsPath), $"Input code-behind not found: {inputCsPath}");
        Assert.True(File.Exists(expectedCsPath), $"Expected code-behind not found: {expectedCsPath}");

        var inputCs = File.ReadAllText(inputCsPath);
        var expectedCs = TestHelpers.NormalizeContent(File.ReadAllText(expectedCsPath));

        // Act
        var metadata = new FileMetadata
        {
            SourceFilePath = inputCsPath,
            OutputFilePath = inputCsPath.Replace(".aspx.cs", ".razor.cs"),
            FileType = FileType.Page,
            OriginalContent = inputCs
        };

        var result = _pipeline.TransformCodeBehind(inputCs, metadata);
        var actualCs = TestHelpers.NormalizeContent(result);

        // Assert
        Assert.Equal(expectedCs, actualCs);
    }

    [Fact]
    public void TestData_ContainsExpectedNumberOfTestCases()
    {
        // Verify we have the expected 34 markup test cases (TC01–TC21 + TC23–TC33)
        var testCases = TestHelpers.DiscoverTestCases().ToList();
        Assert.Equal(34, testCases.Count);
    }

    [Fact]
    public void TestData_AllInputsHaveExpectedOutputs()
    {
        var expectedDir = Path.Combine(TestDataRoot, "expected");
        foreach (var tc in TestHelpers.DiscoverTestCases())
        {
            var expectedPath = Path.Combine(expectedDir, $"{tc}.razor");
            Assert.True(File.Exists(expectedPath),
                $"Test case '{tc}' has input .aspx but no expected .razor");
        }
    }

    [Fact]
    public void TestData_CodeBehindPairsAreComplete()
    {
        var inputDir = Path.Combine(TestDataRoot, "inputs");
        var expectedDir = Path.Combine(TestDataRoot, "expected");

        var inputCsFiles = Directory.GetFiles(inputDir, "*.aspx.cs")
            .Select(f => Path.GetFileName(f).Replace(".aspx.cs", ""))
            .ToList();

        // Every .aspx.cs input should have a corresponding .razor.cs expected output
        foreach (var tc in inputCsFiles)
        {
            Assert.True(File.Exists(Path.Combine(expectedDir, $"{tc}.razor.cs")),
                $"Test case '{tc}' has input .aspx.cs but no expected .razor.cs");
        }

        // Verify we have 13 code-behind test cases (TC13–TC16, TC18–TC21, TC26–TC27, TC29, TC31, TC33)
        Assert.Equal(13, inputCsFiles.Count);
    }
}
