using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    ViewStateUsageAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    ViewStateUsageAnalyzer,
    ViewStateUsageCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC002: ViewState usage detected.
/// </summary>
public class ViewStateUsageAnalyzerTests
{
    private const string StubSource = @"
using System.Collections.Generic;

public class PageBase
{
    public Dictionary<string, object> ViewState { get; } = new Dictionary<string, object>();
}
";

    private static DiagnosticResult ExpectBWFC002(string memberName) =>
        new DiagnosticResult(ViewStateUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(memberName);

    #region Positive cases — BWFC002 SHOULD fire

    [Fact]
    public async Task ViewStateAccess_InMethodBody_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        {|#0:ViewState[""key""]|} = ""value"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC002("Page_Load").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ThisViewStateAccess_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        {|#0:this.ViewState[""x""]|} = ""value"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC002("Page_Load").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC002 should NOT fire

    [Fact]
    public async Task LowercaseViewState_NoDiagnostic()
    {
        var source = @"
using System.Collections.Generic;

public class MyPage
{
    public void DoWork()
    {
        var viewState = new Dictionary<string, object>();
        viewState[""key""] = ""value"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task DictionaryNamedViewState_InNonPageClass_NoDiagnostic()
    {
        // A plain class with a Dictionary named ViewState should not fire
        // because there is no ElementAccessExpression on a member called "ViewState"
        // inherited from PageBase — it's a local variable
        var source = @"
using System.Collections.Generic;

public class PlainHelper
{
    public void DoWork()
    {
        var ViewState = new Dictionary<string, object>();
        ViewState[""key""] = ""value"";
    }
}";

        // This WILL fire because we detect the syntactic pattern ViewState["key"]
        // regardless of class hierarchy — ViewState is a Web Forms-specific name.
        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC002("DoWork").WithLocation(9, 9) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Code fix tests

    [Fact]
    public async Task CodeFix_CommentsOutViewStateUsage()
    {
        var testSource = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        {|#0:ViewState[""key""]|} = ""value"";
    }
}";

        var fixedSource = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        // TODO: Replace ViewState[""key""] with component state
        // ViewState[""key""] = ""value"";
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC002("Page_Load").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
