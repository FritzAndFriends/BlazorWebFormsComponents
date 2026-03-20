using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    RequiredAttributeAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC010: required attribute may be missing on BWFC component.
/// </summary>
public class RequiredAttributeAnalyzerTests
{
    private const string BwfcStubs = @"
namespace BlazorWebFormsComponents
{
    public class GridView<ItemType>
    {
        public object DataSource { get; set; }
        public string CssClass { get; set; }
    }

    public class HyperLink
    {
        public string NavigateUrl { get; set; }
        public string Text { get; set; }
    }

    public class Image
    {
        public string ImageUrl { get; set; }
        public string AlternateText { get; set; }
    }
}
";

    private static DiagnosticResult ExpectBWFC010(string componentName, string attributeName) =>
        new DiagnosticResult(RequiredAttributeAnalyzer.DiagnosticId, DiagnosticSeverity.Info)
            .WithArguments(componentName, attributeName);

    #region Positive cases — BWFC010 SHOULD fire

    [Fact]
    public async Task GridView_WithoutDataSource_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var grid = {|#0:new GridView<string>()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } },
            ExpectedDiagnostics = { ExpectBWFC010("GridView", "DataSource").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task HyperLink_WithoutNavigateUrl_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var link = {|#0:new HyperLink()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } },
            ExpectedDiagnostics = { ExpectBWFC010("HyperLink", "NavigateUrl").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task Image_WithoutImageUrl_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var img = {|#0:new Image()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } },
            ExpectedDiagnostics = { ExpectBWFC010("Image", "ImageUrl").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task GridView_WithOnlyOtherProperties_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var grid = {|#0:new GridView<string> { CssClass = ""table"" }|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } },
            ExpectedDiagnostics = { ExpectBWFC010("GridView", "DataSource").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC010 should NOT fire

    [Fact]
    public async Task GridView_WithDataSource_InInitializer_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var grid = new GridView<string> { DataSource = new object() };
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task GridView_WithDataSource_ViaSubsequentAssignment_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var grid = new GridView<string>();
        grid.DataSource = new object();
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task HyperLink_WithNavigateUrl_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var link = new HyperLink { NavigateUrl = ""/home"" };
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task NonBwfcGridView_NoDiagnostic()
    {
        var source = @"
namespace OtherLibrary
{
    public class GridView<T>
    {
        public object DataSource { get; set; }
    }
}

public class MyPage
{
    public void Setup()
    {
        var grid = new OtherLibrary.GridView<string>();
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task Image_WithImageUrl_ViaAssignment_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents;

public class MyPage
{
    public void Setup()
    {
        var img = new Image();
        img.ImageUrl = ""/logo.png"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, BwfcStubs } }
        };
        await test.RunAsync();
    }

    #endregion
}
