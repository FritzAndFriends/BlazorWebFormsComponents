using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    RequestObjectUsageAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    RequestObjectUsageAnalyzer,
    RequestObjectUsageCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC014: Request object access requires Blazor patterns.
/// </summary>
public class RequestObjectUsageAnalyzerTests
{
    private const string HttpStubs = @"
public class NameValueCollection
{
    public string this[string key] { get { return null; } set { } }
}

public class HttpFileCollection
{
    public int Count { get; set; }
}

public class HttpCookieCollection
{
    public string this[string key] { get { return null; } set { } }
}

public class HttpRequest
{
    public NameValueCollection Form { get; set; }
    public HttpCookieCollection Cookies { get; set; }
    public NameValueCollection Headers { get; set; }
    public HttpFileCollection Files { get; set; }
    public NameValueCollection QueryString { get; set; }
    public NameValueCollection ServerVariables { get; set; }
}

public class HttpContextInstance
{
    public HttpRequest Request { get; set; }
}

public static class HttpContext
{
    public static HttpContextInstance Current { get; set; }
}
";

    private static DiagnosticResult ExpectBWFC014(string methodName, string propertyName) =>
        new DiagnosticResult(RequestObjectUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(methodName, propertyName);

    #region Positive cases — BWFC014 SHOULD fire

    [Fact]
    public async Task RequestForm_DirectAccess_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadForm()
    {
        var name = {|#0:Request.Form[""username""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("ReadForm", "Form").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task RequestCookies_WithThis_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadCookie()
    {
        var token = {|#0:this.Request.Cookies[""auth_token""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("ReadCookie", "Cookies").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task RequestQueryString_ViaHttpContext_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void ReadQuery()
    {
        var id = {|#0:HttpContext.Current.Request.QueryString[""id""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("ReadQuery", "QueryString").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task RequestFiles_DirectProperty_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void HandleUpload()
    {
        var files = {|#0:Request.Files|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("HandleUpload", "Files").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task RequestHeaders_DirectAccess_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadHeaders()
    {
        var accept = {|#0:Request.Headers[""Accept""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("ReadHeaders", "Headers").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task RequestServerVariables_DirectAccess_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadServerVar()
    {
        var host = {|#0:Request.ServerVariables[""HTTP_HOST""]|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("ReadServerVar", "ServerVariables").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC014 should NOT fire

    [Fact]
    public async Task CustomClassWithFormProperty_NoDiagnostic()
    {
        var source = @"
public class FormData
{
    public string this[string key] { get { return null; } }
}

public class DataReader
{
    public FormData Form { get; set; }
}

public class MyPage
{
    public void ReadData()
    {
        var reader = new DataReader();
        var val = reader.Form[""key""];
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task LocalDictionaryNamedRequest_NoDiagnostic()
    {
        var source = @"
using System.Collections.Generic;

public class MyPage
{
    public void DoWork()
    {
        var request = new Dictionary<string, string>();
        var val = request[""key""];
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, HttpStubs } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task RequestPropertyNotInDetectedSet_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadUrl()
    {
        // Request.Url is not one of the detected properties
        var req = Request;
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
    public async Task CodeFix_RequestForm_AddsTodoComment()
    {
        var testSource = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadForm()
    {
        var name = {|#0:Request.Form[""username""]|};
    }
}";

        var fixedSource = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void ReadForm()
    {
        // TODO: Replace Request.Form access with Blazor equivalent
        ;
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, HttpStubs } },
            FixedState = { Sources = { fixedSource, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("ReadForm", "Form").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CodeFix_RequestFiles_AddsTodoComment()
    {
        var testSource = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void HandleUpload()
    {
        var files = {|#0:Request.Files|};
    }
}";

        var fixedSource = @"
public class MyPage
{
    public HttpRequest Request { get; set; }

    public void HandleUpload()
    {
        // TODO: Replace Request.Files access with Blazor equivalent
        ;
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, HttpStubs } },
            FixedState = { Sources = { fixedSource, HttpStubs } },
            ExpectedDiagnostics = { ExpectBWFC014("HandleUpload", "Files").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
