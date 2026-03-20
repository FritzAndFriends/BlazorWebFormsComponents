using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    RunatServerAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    RunatServerAnalyzer,
    RunatServerCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC012: Leftover runat="server" attribute detected.
/// </summary>
public class RunatServerAnalyzerTests
{
    private static DiagnosticResult ExpectBWFC012(string memberName) =>
        new DiagnosticResult(RunatServerAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(memberName);

    #region Positive cases — BWFC012 SHOULD fire

    [Fact]
    public async Task StringWithRunatServerDoubleQuotes_ReportsDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void Render()
    {
        var markup = {|#0:""<div runat=\""server\"" class=\""foo\"">Hello</div>""|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } },
            ExpectedDiagnostics = { ExpectBWFC012("Render").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task StringWithRunatServerSingleQuotes_ReportsDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void Render()
    {
        var markup = {|#0:""<input runat='server' type='text' />""|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } },
            ExpectedDiagnostics = { ExpectBWFC012("Render").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task StringInFieldInitializer_ReportsDiagnostic()
    {
        var source = @"
public class MyComponent
{
    private string _tag = {|#0:""<asp:Label runat=\""server\"" />""|};
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } },
            ExpectedDiagnostics = { ExpectBWFC012("MyComponent").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task StringWithRunatServerCaseInsensitive_ReportsDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void Render()
    {
        var markup = {|#0:""<div Runat=\""Server\"">Hello</div>""|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } },
            ExpectedDiagnostics = { ExpectBWFC012("Render").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC012 should NOT fire

    [Fact]
    public async Task StringContainingRunatWithoutServer_NoDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void DoWork()
    {
        var text = ""The runat attribute is used in Web Forms"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task VariableNamedRunat_NoDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void DoWork()
    {
        var runat = ""server"";
        System.Console.WriteLine(runat);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task StringContainingServerWithoutRunat_NoDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void DoWork()
    {
        var text = ""Connect to server on port 8080"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task EmptyString_NoDiagnostic()
    {
        var source = @"
public class MyComponent
{
    public void DoWork()
    {
        var text = """";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source } }
        };
        await test.RunAsync();
    }

    #endregion

    #region Code fix tests

    [Fact]
    public async Task CodeFix_RemovesRunatServerFromString()
    {
        var testSource = @"
public class MyComponent
{
    public void Render()
    {
        var markup = {|#0:""<div runat=\""server\"" class=\""foo\"">Hello</div>""|};
    }
}";

        var fixedSource = @"
public class MyComponent
{
    public void Render()
    {
        var markup = ""<div class=\""foo\"">Hello</div>"";
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource } },
            FixedState = { Sources = { fixedSource } },
            ExpectedDiagnostics = { ExpectBWFC012("Render").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CodeFix_RemovesRunatServerSingleQuotes()
    {
        var testSource = @"
public class MyComponent
{
    public void Render()
    {
        var markup = {|#0:""<input runat='server' type='text' />""|};
    }
}";

        var fixedSource = @"
public class MyComponent
{
    public void Render()
    {
        var markup = ""<input type='text' />"";
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource } },
            FixedState = { Sources = { fixedSource } },
            ExpectedDiagnostics = { ExpectBWFC012("Render").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
