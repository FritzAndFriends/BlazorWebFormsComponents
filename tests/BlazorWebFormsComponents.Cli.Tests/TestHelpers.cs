using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using BlazorWebFormsComponents.Cli.Transforms.Directives;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests;

/// <summary>
/// Shared test utilities for L1 transform testing.
/// Normalization logic ported from migration-toolkit/tests/Run-L1Tests.ps1.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Normalizes content for comparison by:
    /// 1. Normalizing line endings (CRLF → LF)
    /// 2. Trimming trailing whitespace from each line
    /// 3. Removing trailing empty lines (collapses to zero trailing blanks)
    /// 4. Trimming leading/trailing whitespace from the entire result
    /// </summary>
    public static string NormalizeContent(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        // Normalize CRLF → LF
        var normalized = text.Replace("\r\n", "\n");

        // Split into lines, trim trailing whitespace from each line
        var lines = normalized.Split('\n')
            .Select(line => line.TrimEnd())
            .ToList();

        // Remove trailing empty lines
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
        {
            lines.RemoveAt(lines.Count - 1);
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Resolves the TestData directory relative to the test assembly output.
    /// Works both from IDE (bin/Debug) and dotnet test contexts.
    /// </summary>
    public static string GetTestDataRoot()
    {
        // Try output directory first (for CopyToOutputDirectory items)
        var assemblyDir = Path.GetDirectoryName(typeof(TestHelpers).Assembly.Location)!;
        var outputTestData = Path.Combine(assemblyDir, "TestData");
        if (Directory.Exists(outputTestData))
            return outputTestData;

        // Fallback: walk up from assembly location to find the project TestData
        var dir = new DirectoryInfo(assemblyDir);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "TestData");
            if (Directory.Exists(candidate) &&
                Directory.Exists(Path.Combine(candidate, "inputs")) &&
                Directory.Exists(Path.Combine(candidate, "expected")))
            {
                return candidate;
            }
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate TestData directory. Ensure TestData/inputs and TestData/expected exist.");
    }

    /// <summary>
    /// Discovers all TC* test case names from the TestData/inputs directory.
    /// Returns base names without extensions (e.g., "TC01-AspPrefix").
    /// </summary>
    public static IEnumerable<string> DiscoverTestCases()
    {
        var inputDir = Path.Combine(GetTestDataRoot(), "inputs");
        var aspxFiles = Directory.GetFiles(inputDir, "*.aspx")
            .Select(f => Path.GetFileNameWithoutExtension(f));
        var masterFiles = Directory.GetFiles(inputDir, "*.master")
            .Select(f => Path.GetFileNameWithoutExtension(f));
        return aspxFiles.Concat(masterFiles)
            .OrderBy(name => name)
            .Distinct();
    }

    /// <summary>
    /// Discovers test case names that have code-behind files (.aspx.cs inputs and .razor.cs expected).
    /// </summary>
    public static IEnumerable<string> DiscoverCodeBehindTestCases()
    {
        var inputDir = Path.Combine(GetTestDataRoot(), "inputs");
        var expectedDir = Path.Combine(GetTestDataRoot(), "expected");

        return Directory.GetFiles(inputDir, "*.aspx.cs")
            .Select(f => Path.GetFileName(f).Replace(".aspx.cs", ""))
            .Where(name => File.Exists(Path.Combine(expectedDir, $"{name}.razor.cs")))
            .OrderBy(name => name)
            .Distinct();
    }

    /// <summary>
    /// Creates a temp directory with sample Web Forms files for integration tests.
    /// Returns the path to the temp directory. Caller is responsible for cleanup.
    /// </summary>
    public static string CreateTempProjectDir(string? suffix = null)
    {
        var dirName = $"bwfc-test-{suffix ?? Guid.NewGuid().ToString("N")}";
        var tempDir = Path.Combine(Path.GetTempPath(), dirName);
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
        Directory.CreateDirectory(tempDir);

        File.WriteAllText(Path.Combine(tempDir, "Default.aspx"), """
            <%@ Page Title="Home" Language="C#" AutoEventWireup="true" %>
            <asp:Content ID="Body" ContentPlaceHolderID="MainContent" runat="server">
                <asp:Label ID="Label1" runat="server" Text="Hello" />
            </asp:Content>
            """);

        File.WriteAllText(Path.Combine(tempDir, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="TestKey" value="TestValue" />
              </appSettings>
            </configuration>
            """);

        return tempDir;
    }

    /// <summary>
    /// Cleans up a temp directory created by CreateTempProjectDir.
    /// </summary>
    public static void CleanupTempDir(string path)
    {
        if (Directory.Exists(path))
        {
            try { Directory.Delete(path, recursive: true); }
            catch { /* best effort */ }
        }
    }

    /// <summary>
    /// Creates a fully configured MigrationPipeline with all markup and code-behind
    /// transforms registered in the canonical order.
    /// </summary>
    public static MigrationPipeline CreateDefaultPipeline()
    {
        var markupTransforms = new List<IMarkupTransform>
        {
            // Order 100-120: Directives
            new PageDirectiveTransform(),
            new MasterDirectiveTransform(),
            new ControlDirectiveTransform(),
            // Order 200-210: Imports & Registers
            new ImportDirectiveTransform(),
            new RegisterDirectiveTransform(),
            // Order 250: Master page layout conversion
            new MasterPageTransform(),
            // Order 300-310: Content/Form wrappers
            new ContentWrapperTransform(),
            new FormWrapperTransform(),
            // Order 500: Expressions
            new ExpressionTransform(),
            // Order 510-520: Semantic controls
            new LoginViewTransform(),
            new SelectMethodTransform(),
            // Order 600-610: Prefix stripping (Ajax before Asp)
            new AjaxToolkitPrefixTransform(),
            new AspPrefixTransform(),
            // Order 700-750: Attributes & refs
            new AttributeStripTransform(),
            new EventWiringTransform(),
            new ComponentRefMarkupTransform(),
            new UrlReferenceTransform(),
            // Order 800-820: Normalize & templates
            new TemplatePlaceholderTransform(),
            new AttributeNormalizeTransform(),
            new DataSourceIdTransform(),
        };

        var codeBehindTransforms = new List<ICodeBehindTransform>
        {
            new TodoHeaderTransform(),
            new UsingStripTransform(),
            new IdentityUsingTransform(),
            new EntityFrameworkTransform(),
            new ConfigurationManagerTransform(),
            new BaseClassStripTransform(),
            new ClassNameAlignTransform(),
            new MethodNameCollisionTransform(),
            new ComponentRefCodeBehindTransform(),
            new ResponseRedirectTransform(),
            new RequestFormTransform(),
            new ServerShimTransform(),
            new GetRouteUrlTransform(),
            new SessionDetectTransform(),
            new ViewStateDetectTransform(),
            new IsPostBackTransform(),
            new PageLifecycleTransform(),
            new EventHandlerSignatureTransform(),
            new DataBindTransform(),
            new ClientScriptTransform(),
            new UrlCleanupTransform(),
        };

        return new MigrationPipeline(markupTransforms, codeBehindTransforms);
    }
}
