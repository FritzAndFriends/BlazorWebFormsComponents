using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    ViewStatePropertyPatternAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    ViewStatePropertyPatternAnalyzer,
    ViewStatePropertyPatternCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC020: ViewState-backed property pattern detection and code fix.
/// </summary>
public class ViewStatePropertyPatternAnalyzerTests
{
    private const string StubSource = @"
using System.Collections.Generic;

public class PageBase
{
    public Dictionary<string, object> ViewState { get; } = new Dictionary<string, object>();
}

namespace Microsoft.AspNetCore.Components
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ParameterAttribute : System.Attribute { }
}
";

    private static DiagnosticResult ExpectBWFC020(string propertyName) =>
        new DiagnosticResult(ViewStatePropertyPatternAnalyzer.DiagnosticId, DiagnosticSeverity.Info)
            .WithArguments(propertyName);

    #region Positive cases — BWFC020 SHOULD fire

    [Fact]
    public async Task ViewStateBackedProperty_GetSet_ReportsDiagnostic()
    {
        var source = @"
public class MyControl : PageBase
{
    public string {|#0:Text|}
    {
        get { return (string)ViewState[""Text""]; }
        set { ViewState[""Text""] = value; }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC020("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewStateBackedProperty_ThisQualified_ReportsDiagnostic()
    {
        var source = @"
public class MyControl : PageBase
{
    public int {|#0:PageSize|}
    {
        get { return (int)this.ViewState[""PageSize""]; }
        set { this.ViewState[""PageSize""] = value; }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC020("PageSize").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewStateBackedProperty_GetOnly_ReportsDiagnostic()
    {
        var source = @"
public class MyControl : PageBase
{
    public string {|#0:Label|}
    {
        get { return (string)ViewState[""Label""]; }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC020("Label").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task MultipleViewStateProperties_ReportsAll()
    {
        var source = @"
public class MyControl : PageBase
{
    public string {|#0:Text|}
    {
        get { return (string)ViewState[""Text""]; }
        set { ViewState[""Text""] = value; }
    }

    public int {|#1:Count|}
    {
        get { return (int)ViewState[""Count""]; }
        set { ViewState[""Count""] = value; }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC020("Text").WithLocation(0),
                ExpectBWFC020("Count").WithLocation(1),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC020 should NOT fire

    [Fact]
    public async Task AutoProperty_NoDiagnostic()
    {
        var source = @"
public class MyControl : PageBase
{
    public string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PropertyWithBackingField_NoDiagnostic()
    {
        var source = @"
public class MyControl : PageBase
{
    private string _text;
    public string Text
    {
        get { return _text; }
        set { _text = value; }
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PropertyWithNoAccessorBody_NoDiagnostic()
    {
        var source = @"
public class MyControl : PageBase
{
    public string Text { get; set; }
    public int Count { get; }
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
    public async Task CodeFix_ConvertsToParameterAutoProperty()
    {
        var testSource = @"
using Microsoft.AspNetCore.Components;

public class MyControl : PageBase
{
    public string {|#0:Text|}
    {
        get { return (string)ViewState[""Text""]; }
        set { ViewState[""Text""] = value; }
    }
}";

        var fixedSource = @"
using Microsoft.AspNetCore.Components;

public class MyControl : PageBase
{
    [Parameter]
    public string Text { get; set; }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC020("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CodeFix_AddsUsingDirective_WhenMissing()
    {
        var testSource = @"
public class MyControl : PageBase
{
    public string {|#0:Text|}
    {
        get { return (string)ViewState[""Text""]; }
        set { ViewState[""Text""] = value; }
    }
}";

        var fixedSource = @"using Microsoft.AspNetCore.Components;

public class MyControl : PageBase
{
    [Parameter]
    public string Text { get; set; }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC020("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
