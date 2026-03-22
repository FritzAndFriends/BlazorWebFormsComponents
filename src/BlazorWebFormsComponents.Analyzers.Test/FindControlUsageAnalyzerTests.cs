using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    FindControlUsageAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC021: FindControl usage detection.
/// The analyzer flags FindControl calls on non-BWFC types (e.g., System.Web.UI.Control)
/// but does NOT flag FindControl calls on BaseWebFormsComponent subclasses.
/// No code fix is offered — BWFC's FindControl is the canonical API name.
/// </summary>
public class FindControlUsageAnalyzerTests
{
    private const string StubSource = @"
public class Control
{
    public Control FindControl(string id) => null;
    public Control FindControlRecursive(string id) => null;
}

public class BaseWebFormsComponent
{
    public BaseWebFormsComponent FindControl(string id) => null;
}
";

    private static DiagnosticResult ExpectBWFC021() =>
        new DiagnosticResult(FindControlUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning);

    #region Positive cases — BWFC021 SHOULD fire

    [Fact]
    public async Task FindControl_DirectCall_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : Control
{
    public void Page_Load()
    {
        var ctl = {|#0:FindControl(""txtName"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC021().WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task FindControl_ThisQualified_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : Control
{
    public void Page_Load()
    {
        var ctl = {|#0:this.FindControl(""txtName"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC021().WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task FindControl_OnVariable_ReportsDiagnostic()
    {
        var source = @"
public class MyPage : Control
{
    public void Page_Load()
    {
        Control parent = this;
        var ctl = {|#0:parent.FindControl(""txtName"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC021().WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task MultipleFindControlCalls_ReportsAll()
    {
        var source = @"
public class MyPage : Control
{
    public void Page_Load()
    {
        var a = {|#0:FindControl(""txtA"")|};
        var b = {|#1:FindControl(""txtB"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC021().WithLocation(0),
                ExpectBWFC021().WithLocation(1),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC021 should NOT fire

    [Fact]
    public async Task FindControl_OnBwfcType_NoDiagnostic()
    {
        var source = @"
public class MyComponent : BaseWebFormsComponent
{
    public void OnInit()
    {
        var ctl = FindControl(""txtName"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task FindControl_OnBwfcType_ThisQualified_NoDiagnostic()
    {
        var source = @"
public class MyComponent : BaseWebFormsComponent
{
    public void OnInit()
    {
        var ctl = this.FindControl(""txtName"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task FindControl_OnBwfcVariable_NoDiagnostic()
    {
        var source = @"
public class MyComponent : BaseWebFormsComponent
{
    public void OnInit()
    {
        BaseWebFormsComponent parent = this;
        var ctl = parent.FindControl(""txtName"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task FindControlRecursive_NoDiagnostic()
    {
        var source = @"
public class MyPage : Control
{
    public void Page_Load()
    {
        var ctl = FindControlRecursive(""txtName"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task DifferentMethodName_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public object FindItem(string id) => null;

    public void Page_Load()
    {
        var item = FindItem(""key"");
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