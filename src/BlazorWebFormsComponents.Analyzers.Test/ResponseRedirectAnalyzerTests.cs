using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    ResponseRedirectAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    ResponseRedirectAnalyzer,
    ResponseRedirectCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC004: Response.Redirect usage detected.
/// </summary>
public class ResponseRedirectAnalyzerTests
{
    private const string HttpStubs = @"
public class HttpResponse
{
    public void Redirect(string url) { }
    public void Redirect(string url, bool endResponse) { }
}

public class HttpContextInstance
{
    public HttpResponse Response { get; set; }
}

public static class HttpContext
{
    public static HttpContextInstance Current { get; set; }
}
";

    private static DiagnosticResult ExpectBWFC004(string methodName) =>
        new DiagnosticResult(ResponseRedirectAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(methodName);

    #region Positive cases — BWFC004 SHOULD fire

    [Fact]
    public async Task ResponseRedirect_DirectCall_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void DoRedirect()
    {
        {|#0:Response.Redirect(""~/Home"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC004("DoRedirect").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ResponseRedirect_WithThis_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void DoRedirect()
    {
        {|#0:this.Response.Redirect(""url"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC004("DoRedirect").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task HttpContextCurrentResponseRedirect_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void DoRedirect()
    {
        {|#0:HttpContext.Current.Response.Redirect(""~/Home"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC004("DoRedirect").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC004 should NOT fire

    [Fact]
    public async Task CustomClassWithRedirectMethod_NoDiagnostic()
    {
        var source = @"
public class Router
{
    public void Redirect(string url) { }
}

public class MyPage
{
    public void Navigate()
    {
        var router = new Router();
        router.Redirect(""~/Home"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task NavigationManagerNavigateTo_NoDiagnostic()
    {
        var source = @"
public class NavigationManager
{
    public void NavigateTo(string url) { }
}

public class MyPage
{
    private NavigationManager _nav = new NavigationManager();

    public void DoNavigate()
    {
        _nav.NavigateTo(""~/Home"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } }
        };
        await test.RunAsync();
    }

    #endregion

    #region Code fix tests

    [Fact]
    public async Task CodeFix_AddsNavigationManagerTodoComment()
    {
        var testSource = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void DoRedirect()
    {
        {|#0:Response.Redirect(""~/Home"")|};
    }
}";

        var fixedSource = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void DoRedirect()
    {
        // TODO: Replace Response.Redirect(""~/Home"") with NavigationManager.NavigateTo(""url"")
        Response.Redirect(""~/Home"");
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, HttpStubs } },
            FixedState = { Sources = { fixedSource, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC004("DoRedirect").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
