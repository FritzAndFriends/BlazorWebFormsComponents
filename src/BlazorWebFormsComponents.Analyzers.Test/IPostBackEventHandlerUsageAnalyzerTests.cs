using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    IPostBackEventHandlerUsageAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC023: IPostBackEventHandler implementation detection with migration guidance.
/// </summary>
public class IPostBackEventHandlerUsageAnalyzerTests
{
    private const string StubSource = @"
public interface IPostBackEventHandler
{
    void RaisePostBackEvent(string eventArgument);
}
";

    private static DiagnosticResult ExpectBWFC023() =>
        new DiagnosticResult(IPostBackEventHandlerUsageAnalyzer.DiagnosticId, DiagnosticSeverity.Warning);

    #region Positive cases — BWFC023 SHOULD fire

    [Fact]
    public async Task ClassImplementingIPostBackEventHandler_ReportsDiagnostic()
    {
        var source = @"
public class {|#0:MyButton|} : IPostBackEventHandler
{
    public void RaisePostBackEvent(string eventArgument) { }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC023().WithLocation(0)
                    .WithMessage("IPostBackEventHandler is not available in Blazor. Remove the interface, replace RaisePostBackEvent with EventCallback<T> parameters, and use @onclick handlers for user interactions.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ClassImplementingMultipleInterfaces_IncludingIPostBackEventHandler_ReportsDiagnostic()
    {
        var source = @"
public interface IMyInterface { }

public class {|#0:MyControl|} : IMyInterface, IPostBackEventHandler
{
    public void RaisePostBackEvent(string eventArgument) { }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC023().WithLocation(0)
                    .WithMessage("IPostBackEventHandler is not available in Blazor. Remove the interface, replace RaisePostBackEvent with EventCallback<T> parameters, and use @onclick handlers for user interactions.")
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ClassInheritingBaseAndImplementingInterface_ReportsDiagnostic()
    {
        var source = @"
public class ControlBase { }

public class {|#0:MyButton|} : ControlBase, IPostBackEventHandler
{
    public void RaisePostBackEvent(string eventArgument) { }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC023().WithLocation(0)
                    .WithMessage("IPostBackEventHandler is not available in Blazor. Remove the interface, replace RaisePostBackEvent with EventCallback<T> parameters, and use @onclick handlers for user interactions.")
            }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC023 should NOT fire

    [Fact]
    public async Task ClassNotImplementingInterface_NoDiagnostic()
    {
        var source = @"
public class MyButton
{
    public void RaisePostBackEvent(string eventArgument) { }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ClassImplementingDifferentInterface_NoDiagnostic()
    {
        var source = @"
public interface IMyHandler
{
    void Handle(string arg);
}

public class MyHandler : IMyHandler
{
    public void Handle(string arg) { }
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

    #region TC38 — IPostBackEventHandler Enhanced Guidance

    /// <summary>
    /// TC38: BWFC023 message mentions EventCallback for postback migration.
    /// </summary>
    [Fact]
    public void TC38_DiagnosticMessage_MentionsEventCallback()
    {
        var analyzer = new IPostBackEventHandlerUsageAnalyzer();
        var descriptor = analyzer.SupportedDiagnostics.Single();

        Assert.Contains("EventCallback", descriptor.MessageFormat.ToString());
    }

    /// <summary>
    /// TC38: Class implementing IPostBackEventHandler with RaisePostBackEvent
    /// containing conditional logic fires BWFC023.
    /// </summary>
    [Fact]
    public async Task TC38_IPostBackEventHandler_WithConditionalLogic_ReportsDiagnostic()
    {
        var source = @"
public class {|#0:MyControl|} : IPostBackEventHandler
{
    public event System.EventHandler OnDeleteRequested;

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == ""delete"")
            OnDeleteRequested?.Invoke(this, System.EventArgs.Empty);
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC023().WithLocation(0) }
        };
        await test.RunAsync();
    }

    /// <summary>
    /// TC38: Class inheriting a base and implementing IPostBackEventHandler fires BWFC023
    /// (simulates UserControl-derived control).
    /// </summary>
    [Fact]
    public async Task TC38_UserControlDerived_WithIPostBackEventHandler_ReportsDiagnostic()
    {
        var source = @"
public class UserControl { }

public class {|#0:MyButton|} : UserControl, IPostBackEventHandler
{
    public void RaisePostBackEvent(string eventArgument) { }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC023().WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

}
