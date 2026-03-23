using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    PageClientScriptUsageAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC022: Page.ClientScript usage detection (no code fix).
/// </summary>
public class PageClientScriptUsageAnalyzerTests
{
    private const string StubSource = @"
public class ClientScriptManager
{
    public void RegisterStartupScript(System.Type type, string key, string script) { }
    public string GetPostBackEventReference(object control) => """";
    public void RegisterClientScriptBlock(System.Type type, string key, string script) { }
}

public class PageBase
{
    public ClientScriptManager ClientScript { get; } = new ClientScriptManager();
}
";

    private static DiagnosticResult ExpectBWFC022() =>
        new DiagnosticResult(PageClientScriptUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning);

    #region Positive cases — BWFC022 SHOULD fire

    [Fact]
    public async Task PageClientScript_RegisterStartupScript_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterStartupScript(GetType(), ""key"", ""alert('hi')"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_GetPostBackEventReference_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void DoWork()
    {
        var script = {|#0:Page.ClientScript|}.GetPostBackEventReference(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_MultipleUsages_ReportsAll()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterStartupScript(GetType(), ""key1"", ""alert('a')"");
        {|#1:Page.ClientScript|}.RegisterClientScriptBlock(GetType(), ""key2"", ""var x=1;"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0),
                ExpectBWFC022().WithLocation(1),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC022 should NOT fire

    [Fact]
    public async Task NonPageClientScript_NoDiagnostic()
    {
        var source = @"
public class ScriptHelper
{
    public ClientScriptManager ClientScript { get; } = new ClientScriptManager();
}

public class MyPage
{
    public void DoWork()
    {
        var helper = new ScriptHelper();
        helper.ClientScript.RegisterStartupScript(GetType(), ""key"", ""alert('hi')"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PlainPageProperty_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void DoWork()
    {
        var p = Page;
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    #endregion
}
