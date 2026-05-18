using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    PageClientScriptUsageAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC022: Page.ClientScript usage detection with method-specific guidance.
/// </summary>
public class PageClientScriptUsageAnalyzerTests
{
    private const string StubSource = @"
public class ClientScriptManager
{
    public void RegisterStartupScript(System.Type type, string key, string script) { }
    public string GetPostBackEventReference(object control) => """";
    public string GetPostBackEventReference(object control, string argument) => """";
    public void RegisterClientScriptBlock(System.Type type, string key, string script) { }
    public void RegisterClientScriptInclude(string key, string url) { }
}

public class PageBase
{
    public ClientScriptManager ClientScript { get; } = new ClientScriptManager();
}
";

    private static DiagnosticResult ExpectBWFC022() =>
        new DiagnosticResult(PageClientScriptUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning);

    #region Positive cases — BWFC022 SHOULD fire with method-specific guidance

    [Fact]
    public async Task PageClientScript_RegisterStartupScript_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterStartupScript(GetType(), ""key"", ""alert('hi')"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0)
                    .WithMessage("Page.ClientScript.RegisterStartupScript() is not available in Blazor. Use IJSRuntime.InvokeAsync in OnAfterRenderAsync(firstRender: true).")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_GetPostBackEventReference_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void DoWork()
    {
        var script = {|#0:Page.ClientScript|}.GetPostBackEventReference(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0)
                    .WithMessage("Page.ClientScript.GetPostBackEventReference() is not available in Blazor. Use @onclick or EventCallback<T> instead of postback events.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_RegisterClientScriptBlock_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterClientScriptBlock(GetType(), ""key"", ""var x=1;"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0)
                    .WithMessage("Page.ClientScript.RegisterClientScriptBlock() is not available in Blazor. Use IJSRuntime.InvokeVoidAsync to execute script blocks.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_RegisterClientScriptInclude_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterClientScriptInclude(""jquery"", ""~/scripts/jquery.js"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0)
                    .WithMessage("Page.ClientScript.RegisterClientScriptInclude() is not available in Blazor. Add <script src='...'/> to your layout or use IJSRuntime.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_MultipleUsages_ReportsAllWithSpecificMessages()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterStartupScript(GetType(), ""key1"", ""alert('a')"");
        {|#1:Page.ClientScript|}.RegisterClientScriptBlock(GetType(), ""key2"", ""var x=1;"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0)
                    .WithMessage("Page.ClientScript.RegisterStartupScript() is not available in Blazor. Use IJSRuntime.InvokeAsync in OnAfterRenderAsync(firstRender: true)."),
                ExpectBWFC022().WithLocation(1)
                    .WithMessage("Page.ClientScript.RegisterClientScriptBlock() is not available in Blazor. Use IJSRuntime.InvokeVoidAsync to execute script blocks."),
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PageClientScript_GenericAccess_ReportsFallbackGuidance()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void DoWork()
    {
        var mgr = {|#0:Page.ClientScript|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0)
                    .WithMessage("Page.ClientScript is not available in Blazor. Use IJSRuntime for JavaScript interop.")
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region TC36 — Startup Scripts

    /// <summary>
    /// TC36: this.Page.ClientScript.RegisterStartupScript() alternate syntax fires BWFC022.
    /// </summary>
    [Fact]
    public async Task TC36_ThisPageClientScript_RegisterStartupScript_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:this.Page.ClientScript|}.RegisterStartupScript(GetType(), ""InitUI"", ""$(function() { init(); });"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC36: BWFC022 description mentions IJSRuntime for startup script migration.
    /// </summary>
    [Fact]
    public void TC36_DiagnosticMessage_MentionsIJSRuntime()
    {
        var analyzer = new PageClientScriptUsageAnalyzer();
        var descriptor = analyzer.SupportedDiagnostics.Single();

        Assert.Contains("IJSRuntime", descriptor.Description.ToString());
    }

    /// <summary>
    /// TC36: Multiple startup script registrations each fire BWFC022.
    /// </summary>
    [Fact]
    public async Task TC36_MultipleStartupScripts_ReportsEach()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterStartupScript(GetType(), ""init1"", ""alert('a')"");
        {|#1:Page.ClientScript|}.RegisterStartupScript(GetType(), ""init2"", ""alert('b')"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0),
                ExpectBWFC022().WithLocation(1),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region TC37 — Script Includes and Script Blocks

    /// <summary>
    /// TC37: RegisterClientScriptInclude fires BWFC022.
    /// </summary>
    [Fact]
    public async Task TC37_RegisterClientScriptInclude_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterClientScriptInclude(""jquery-ui"", ""~/lib/jquery-ui.min.js"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC37: this.Page.ClientScript.RegisterClientScriptInclude alternate syntax fires BWFC022.
    /// </summary>
    [Fact]
    public async Task TC37_ThisPageClientScript_RegisterClientScriptInclude_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:this.Page.ClientScript|}.RegisterClientScriptInclude(""myScript"", ""~/Scripts/myScript.js"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC37: RegisterClientScriptBlock fires BWFC022.
    /// </summary>
    [Fact]
    public async Task TC37_RegisterClientScriptBlock_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterClientScriptBlock(GetType(), ""block1"", ""<script>var x=1;</script>"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC37: Mix of include and block calls each fire BWFC022.
    /// </summary>
    [Fact]
    public async Task TC37_MixedIncludeAndBlock_ReportsAll()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterClientScriptInclude(""lib"", ""~/lib/utils.js"");
        {|#1:Page.ClientScript|}.RegisterClientScriptBlock(GetType(), ""init"", ""var ready=true;"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0),
                ExpectBWFC022().WithLocation(1),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region TC38 — Postback References

    /// <summary>
    /// TC38: GetPostBackEventReference with argument overload fires BWFC022.
    /// </summary>
    [Fact]
    public async Task TC38_GetPostBackEventReference_WithArgument_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public string GetPostBackValidation()
    {
        return {|#0:Page.ClientScript|}.GetPostBackEventReference(this, ""validate"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC38: this.Page.ClientScript.GetPostBackEventReference alternate syntax fires BWFC022.
    /// </summary>
    [Fact]
    public async Task TC38_ThisPageClientScript_GetPostBackEventReference_ReportsDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void DoWork()
    {
        var script = {|#0:this.Page.ClientScript|}.GetPostBackEventReference(this, ""delete"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC022().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC38: All three pattern categories in one class — startup, include, postback — each detected.
    /// </summary>
    [Fact]
    public async Task TC38_AllClientScriptPatterns_InOneClass_ReportsAll()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void Page_Load()
    {
        {|#0:Page.ClientScript|}.RegisterStartupScript(GetType(), ""init"", ""alert('hi')"");
        {|#1:Page.ClientScript|}.RegisterClientScriptInclude(""lib"", ""~/lib/utils.js"");
        {|#2:Page.ClientScript|}.RegisterClientScriptBlock(GetType(), ""block"", ""var x=1;"");
        var ref1 = {|#3:Page.ClientScript|}.GetPostBackEventReference(this);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC022().WithLocation(0),
                ExpectBWFC022().WithLocation(1),
                ExpectBWFC022().WithLocation(2),
                ExpectBWFC022().WithLocation(3),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC022 should NOT fire

    [Fact]
    public async Task NonPageClientScript_NoDiagnostic()
    {
        var source = @"
public class ScriptHelper
{
    public ClientScriptManager ClientScript { get; } = new ClientScriptManager();
}

public class MyPage
{
    public void DoWork()
    {
        var helper = new ScriptHelper();
        helper.ClientScript.RegisterStartupScript(GetType(), ""key"", ""alert('hi')"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PlainPageProperty_NoDiagnostic()
    {
        var source = @"
public class MyPage
{
    public PageBase Page { get; } = new PageBase();

    public void DoWork()
    {
        var p = Page;
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
