using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EventHandlerSignatureAnalyzer,
    DefaultVerifier>;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EventHandlerSignatureAnalyzer,
    EventHandlerSignatureCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC011: Web Forms event handler signature detected.
/// </summary>
public class EventHandlerSignatureAnalyzerTests
{
    private const string ComponentBaseStub = @"
namespace Microsoft.AspNetCore.Components
{
    public abstract class ComponentBase { }
}

namespace System
{
    public class EventArgs { }
}

namespace System.Web.UI.WebControls
{
    public class GridViewCommandEventArgs : System.EventArgs { }
    public class ListViewEditEventArgs : System.EventArgs { }
}
";

    private static DiagnosticResult ExpectBWFC011(string methodName) =>
        new DiagnosticResult(EventHandlerSignatureAnalyzer.DiagnosticId, DiagnosticSeverity.Info)
            .WithArguments(methodName);

    #region Positive cases — BWFC011 SHOULD fire

    [Fact]
    public async Task ButtonClick_InComponentBase_ReportsDiagnostic()
    {
        var source = @"
using Microsoft.AspNetCore.Components;
using System;

public class MyComponent : ComponentBase
{
    protected void {|#0:Button_Click|}(object sender, EventArgs e)
    {
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } },
            ExpectedDiagnostics = { ExpectBWFC011("Button_Click").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task GridRowCommand_WithDerivedEventArgs_ReportsDiagnostic()
    {
        var source = @"
using Microsoft.AspNetCore.Components;
using System;
using System.Web.UI.WebControls;

public class MyComponent : ComponentBase
{
    protected void {|#0:Grid_RowCommand|}(object sender, GridViewCommandEventArgs e)
    {
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } },
            ExpectedDiagnostics = { ExpectBWFC011("Grid_RowCommand").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task MultipleHandlers_ReportsAll()
    {
        var source = @"
using Microsoft.AspNetCore.Components;
using System;
using System.Web.UI.WebControls;

public class MyComponent : ComponentBase
{
    protected void {|#0:Button_Click|}(object sender, EventArgs e) { }
    protected void {|#1:ListView_ItemEditing|}(object sender, ListViewEditEventArgs e) { }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } },
            ExpectedDiagnostics =
            {
                ExpectBWFC011("Button_Click").WithLocation(0),
                ExpectBWFC011("ListView_ItemEditing").WithLocation(1),
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC011 should NOT fire

    [Fact]
    public async Task EventHandlerSignature_InPlainClass_NoDiagnostic()
    {
        var source = @"
using System;

public class PlainService
{
    public void OnClick(object sender, EventArgs e)
    {
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task DifferentSignature_InComponentBase_NoDiagnostic()
    {
        var source = @"
using Microsoft.AspNetCore.Components;

public class MyComponent : ComponentBase
{
    protected void HandleClick(string value)
    {
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ThreeParameters_NoDiagnostic()
    {
        var source = @"
using Microsoft.AspNetCore.Components;
using System;

public class MyComponent : ComponentBase
{
    protected void HandleEvent(object sender, EventArgs e, int extra)
    {
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task NoParameters_NoDiagnostic()
    {
        var source = @"
using Microsoft.AspNetCore.Components;

public class MyComponent : ComponentBase
{
    protected void HandleClick()
    {
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, ComponentBaseStub } }
        };
        await test.RunAsync();
    }

    #endregion

    #region Code Fix tests

    [Fact]
    public async Task CodeFix_AddsTodoComment()
    {
        var testSource = @"
using Microsoft.AspNetCore.Components;
using System;

public class MyComponent : ComponentBase
{
    protected void {|#0:Button_Click|}(object sender, EventArgs e)
    {
    }
}";

        var fixedSource = @"
using Microsoft.AspNetCore.Components;
using System;

public class MyComponent : ComponentBase
{
    // TODO: Convert to EventCallback pattern — remove sender parameter, change return type if needed
    protected void {|#0:Button_Click|}(object sender, EventArgs e)
    {
    }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, ComponentBaseStub } },
            FixedState =
            {
                Sources = { fixedSource, ComponentBaseStub },
            },
            ExpectedDiagnostics = { ExpectBWFC011("Button_Click").WithLocation(0) },
            // The code fix adds a TODO comment but doesn't change the method signature,
            // so the diagnostic persists. Limit to one iteration.
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        await test.RunAsync();
    }

    #endregion
}
