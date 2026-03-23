using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    SessionUsageAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    SessionUsageAnalyzer,
    SessionUsageCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC005: Session state and HttpContext.Current usage detected.
/// </summary>
public class SessionUsageAnalyzerTests
{
    private const string SessionStubs = @"
public class SessionCollection
{
    public object this[string key] { get { return null; } set { } }
}

public class HttpContextInstance
{
    public SessionCollection Session { get; set; }
}

public static class HttpContext
{
    public static HttpContextInstance Current { get; set; }
}
";

    private static DiagnosticResult ExpectBWFC005(string methodName) =>
        new DiagnosticResult(SessionUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(methodName);

    #region Positive cases — BWFC005 SHOULD fire

    [Fact]
    public async Task SessionIndexer_DirectAccess_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public SessionCollection Session { get; set; }

    public void LoadUser()
    {
        var name = {|#0:Session[""UserName""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, SessionStubs } },
            ExpectedDiagnostics = { ExpectBWFC005("LoadUser").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task SessionIndexer_WithThis_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public SessionCollection Session { get; set; }

    public void LoadUser()
    {
        var name = {|#0:this.Session[""UserName""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, SessionStubs } },
            ExpectedDiagnostics = { ExpectBWFC005("LoadUser").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task HttpContextCurrent_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void DoWork()
    {
        var ctx = {|#0:HttpContext.Current|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, SessionStubs } },
            ExpectedDiagnostics = { ExpectBWFC005("DoWork").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC005 should NOT fire

    [Fact]
    public async Task LocalVariableNamedSession_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void DoWork()
    {
        var session = new System.Collections.Generic.Dictionary<string, object>();
        var name = session[""UserName""];
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, SessionStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CustomClassWithCurrentProperty_NoDiagnostic()
    {
        var source = @"
public class AppState
{
    public static object Current { get; set; }
}

public class MyPage
{
    public void DoWork()
    {
        var state = AppState.Current;
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, SessionStubs } }
        };
        await test.RunAsync();
    }

    #endregion

    #region Code fix tests

    [Fact]
    public async Task CodeFix_SessionAccess_AddsTodoComment()
    {
        var testSource = @"
public class MyPage
{
    public SessionCollection Session { get; set; }

    public void LoadUser()
    {
        var name = {|#0:Session[""UserName""]|};
    }
}";

        var fixedSource = @"
public class MyPage
{
    public SessionCollection Session { get; set; }

    public void LoadUser()
    {
        // TODO: Replace Session[""UserName""] with scoped service or ProtectedSessionStorage
        ;
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, SessionStubs } },
            FixedState = { Sources = { fixedSource, SessionStubs } },
            ExpectedDiagnostics = { ExpectBWFC005("LoadUser").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
