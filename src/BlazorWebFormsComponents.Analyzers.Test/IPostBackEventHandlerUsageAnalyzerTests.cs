using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    IPostBackEventHandlerUsageAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC023: IPostBackEventHandler implementation detection (no code fix).
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
            ExpectedDiagnostics = { ExpectBWFC023().WithLocation(0) }
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
            ExpectedDiagnostics = { ExpectBWFC023().WithLocation(0) }
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
            ExpectedDiagnostics = { ExpectBWFC023().WithLocation(0) }
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
}
