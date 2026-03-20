using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    IsPostBackUsageAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    IsPostBackUsageAnalyzer,
    IsPostBackUsageCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC003: IsPostBack usage detected.
/// </summary>
public class IsPostBackUsageAnalyzerTests
{
    private const string StubSource = @"
public class PageBase
{
    public bool IsPostBack { get; set; }
    public PageBase Page => this;
    protected void DoInit() { }
}
";

    private static DiagnosticResult ExpectBWFC003(string memberName) =>
        new DiagnosticResult(IsPostBackUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(memberName);

    #region Positive cases — BWFC003 SHOULD fire

    [Fact]
    public async Task IsPostBack_InIfCondition_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        if (!{|#0:IsPostBack|})
        {
            DoInit();
        }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC003("Page_Load").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageIsPostBack_InIfCondition_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        if (!{|#0:Page.IsPostBack|})
        {
            DoInit();
        }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC003("Page_Load").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC003 should NOT fire

    [Fact]
    public async Task LowercaseIsPostBack_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void DoWork()
    {
        var isPostBack = false;
        if (!isPostBack)
        {
        }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    #endregion

    #region Code fix tests

    [Fact]
    public async Task CodeFix_CommentsOutIsPostBackCheck()
    {
        var testSource = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        var check = {|#0:IsPostBack|};
    }
}";

        var fixedSource = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        // TODO: Replace IsPostBack check with Blazor lifecycle (OnInitialized/OnParametersSet)
        // var check = IsPostBack;
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC003("Page_Load").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
