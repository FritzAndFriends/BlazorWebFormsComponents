using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    ResponseObjectUsageAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    ResponseObjectUsageAnalyzer,
    ResponseObjectUsageCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC013: Response object methods require manual refactoring.
/// </summary>
public class ResponseObjectUsageAnalyzerTests
{
    private const string HttpStubs = @"
public class HttpResponse
{
    public void Write(string s) { }
    public void Write(object obj) { }
    public void WriteFile(string filename) { }
    public void Clear() { }
    public void Flush() { }
    public void End() { }
    public void Redirect(string url) { }
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

    private static DiagnosticResult ExpectBWFC013(string methodName, string responseMethod) =>
        new DiagnosticResult(ResponseObjectUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(methodName, responseMethod);

    #region Positive cases — BWFC013 SHOULD fire

    [Fact]
    public async Task ResponseWrite_DirectCall_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void RenderOutput()
    {
        {|#0:Response.Write(""Hello"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("RenderOutput", "Write").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ResponseWriteFile_WithThis_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void DownloadFile()
    {
        {|#0:this.Response.WriteFile(""report.pdf"")|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("DownloadFile", "WriteFile").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ResponseClear_ViaHttpContext_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void ResetOutput()
    {
        {|#0:HttpContext.Current.Response.Clear()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("ResetOutput", "Clear").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ResponseFlush_DirectCall_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void SendBuffer()
    {
        {|#0:Response.Flush()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("SendBuffer", "Flush").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ResponseEnd_DirectCall_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void TerminateResponse()
    {
        {|#0:Response.End()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("TerminateResponse", "End").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC013 should NOT fire

    [Fact]
    public async Task CustomClassWithWriteMethod_NoDiagnostic()
    {
        var source = @"
public class Logger
{
    public void Write(string msg) { }
}

public class MyPage
{
    public void DoWork()
    {
        var logger = new Logger();
        logger.Write(""message"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ResponseRedirect_NotDetectedByThisAnalyzer()
    {
        // Response.Redirect is handled by BWFC004, not BWFC013
        var source = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void DoRedirect()
    {
        Response.Redirect(""~/Home"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task StreamWrite_NoDiagnostic()
    {
        var source = @"
public class StreamWriter
{
    public void Write(string s) { }
}

public class MyPage
{
    public void WriteToStream()
    {
        var writer = new StreamWriter();
        writer.Write(""data"");
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
    public async Task CodeFix_ResponseWrite_AddsTodoComment()
    {
        var testSource = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void RenderOutput()
    {
        {|#0:Response.Write(""Hello"")|};
    }
}";

        var fixedSource = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void RenderOutput()
    {
        // TODO: Replace Response.Write(...) with Blazor equivalent — see BWFC migration docs
        ;
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, HttpStubs } },
            FixedState = { Sources = { fixedSource, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("RenderOutput", "Write").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CodeFix_ResponseEnd_AddsTodoComment()
    {
        var testSource = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void TerminateResponse()
    {
        {|#0:Response.End()|};
    }
}";

        var fixedSource = @"
public class MyPage
{
    public HttpResponse Response { get; set; }

    public void TerminateResponse()
    {
        // TODO: Replace Response.End(...) with Blazor equivalent — see BWFC migration docs
        ;
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, HttpStubs } },
            FixedState = { Sources = { fixedSource, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC013("TerminateResponse", "End").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
