using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    ScriptManagerUsageAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC024: ScriptManager code-behind usage detection with method-specific guidance.
/// </summary>
public class ScriptManagerUsageAnalyzerTests
{
    private const string StubSource = @"
public class ScriptManager
{
    public static ScriptManager GetCurrent(object page) => new ScriptManager();
    public void RegisterAsyncPostBackControl(object control) { }
    public static void SetFocus(object control) { }
    public static void RegisterStartupScript(object control, System.Type type, string key, string script, bool addTags) { }
    public static void RegisterClientScriptBlock(object control, System.Type type, string key, string script, bool addTags) { }
}
";

    private static DiagnosticResult ExpectBWFC024() =>
        new DiagnosticResult(ScriptManagerUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning);

    #region Positive cases — BWFC024 SHOULD fire

    [Fact]
    public async Task ScriptManager_GetCurrent_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void Page_Load()
    {
        var sm = {|#0:ScriptManager.GetCurrent|}(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC024().WithLocation(0)
                    .WithMessage("ScriptManager.GetCurrent has no Blazor equivalent. Use @inject IJSRuntime for JavaScript interop or remove if only used for UpdatePanel registration.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ScriptManager_RegisterAsyncPostBackControl_OnInstance_NoDiagnostic()
    {
        // RegisterAsyncPostBackControl is an instance method called on a local variable,
        // not ScriptManager.RegisterAsyncPostBackControl — should NOT fire.
        var source = @"
public class MyPage
{
    public void Page_Load()
    {
        var sm = new ScriptManager();
        sm.RegisterAsyncPostBackControl(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ScriptManager_SetFocus_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void Page_Load()
    {
        {|#0:ScriptManager.SetFocus|}(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC024().WithLocation(0)
                    .WithMessage("ScriptManager.SetFocus has no Blazor equivalent. Use ElementReference with FocusAsync(), or IJSRuntime to call element.focus() directly.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ScriptManager_RegisterStartupScript_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void Page_Load()
    {
        {|#0:ScriptManager.RegisterStartupScript|}(this, GetType(), ""key"", ""alert('hi')"", true);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC024().WithLocation(0)
                    .WithMessage("ScriptManager.RegisterStartupScript has no Blazor equivalent. Use IJSRuntime.InvokeAsync in OnAfterRenderAsync(firstRender: true).")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ScriptManager_RegisterClientScriptBlock_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public void Page_Load()
    {
        {|#0:ScriptManager.RegisterClientScriptBlock|}(this, GetType(), ""key"", ""var x=1;"", true);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC024().WithLocation(0)
                    .WithMessage("ScriptManager.RegisterClientScriptBlock has no Blazor equivalent. Use IJSRuntime.InvokeVoidAsync to execute script blocks.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ScriptManager_MultipleUsages_ReportsAll()
    {
        var source = @"
public class MyPage
{
    public void Page_Load()
    {
        var sm = {|#0:ScriptManager.GetCurrent|}(this);
        {|#1:ScriptManager.SetFocus|}(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC024().WithLocation(0)
                    .WithMessage("ScriptManager.GetCurrent has no Blazor equivalent. Use @inject IJSRuntime for JavaScript interop or remove if only used for UpdatePanel registration."),
                ExpectBWFC024().WithLocation(1)
                    .WithMessage("ScriptManager.SetFocus has no Blazor equivalent. Use ElementReference with FocusAsync(), or IJSRuntime to call element.focus() directly."),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC024 should NOT fire

    [Fact]
    public async Task NonScriptManagerClass_NoDiagnostic()
    {
        var source = @"
public class MyManager
{
    public static MyManager GetCurrent(object page) => new MyManager();
}

public class MyPage
{
    public void DoWork()
    {
        var mgr = MyManager.GetCurrent(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ScriptManagerPropertyAccess_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public ScriptManager ScriptManager { get; } = new ScriptManager();

    public void DoWork()
    {
        var sm = ScriptManager;
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task EmptyClass_NoDiagnostic()
    {
        var source = @"
public class MyClass { }";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    #endregion
}
