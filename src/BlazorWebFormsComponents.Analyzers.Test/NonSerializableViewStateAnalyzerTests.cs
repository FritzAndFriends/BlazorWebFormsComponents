using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    NonSerializableViewStateAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC025: ViewState value may not be JSON-serializable.
/// </summary>
public class NonSerializableViewStateAnalyzerTests
{
    private const string StubSource = @"
using System;
using System.Collections.Generic;

public class ViewStateDictionary : Dictionary<string, object>
{
    public void Set<T>(string key, T value) { this[key] = value; }
}

public class PageBase
{
    public ViewStateDictionary ViewState { get; } = new ViewStateDictionary();
}
";

    private static DiagnosticResult ExpectBWFC025(string typeName) =>
        new DiagnosticResult(NonSerializableViewStateAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(typeName);

    #region Negative cases — BWFC025 should NOT fire

    [Fact]
    public async Task ViewState_AssignString_NoDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        ViewState[""key""] = ""hello"";
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewState_AssignInt_NoDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        ViewState[""key""] = 42;
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewState_AssignBool_NoDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        ViewState[""key""] = true;
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewState_SetGeneric_String_NoDiagnostic()
    {
        var source = @"
public class MyPage : PageBase
{
    public void Page_Load()
    {
        ViewState.Set<string>(""key"", ""val"");
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    #endregion

    #region Positive cases — BWFC025 SHOULD fire

    [Fact]
    public async Task ViewState_AssignDisposable_Diagnostic()
    {
        var source = @"
using System.IO;

public class MyPage : PageBase
{
    public void Page_Load()
    {
        {|#0:ViewState[""key""] = new MemoryStream()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC025("System.IO.MemoryStream").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewState_AssignDataTable_Diagnostic()
    {
        // Use a minimal stub for DataTable since System.Data is not referenced
        var dataStub = @"
namespace System.Data
{
    public class DataTable
    {
        public string TableName { get; set; }
    }
}
";

        var source = @"
using System.Data;

public class MyPage : PageBase
{
    public void Page_Load()
    {
        {|#0:ViewState[""key""] = new DataTable()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource, dataStub } },
            ExpectedDiagnostics = { ExpectBWFC025("System.Data.DataTable").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ViewState_AssignDelegate_Diagnostic()
    {
        var source = @"
using System;

public class MyPage : PageBase
{
    public void Page_Load()
    {
        Action callback = () => { };
        {|#0:ViewState[""key""] = callback|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC025("System.Action").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ThisViewState_AssignDisposable_Diagnostic()
    {
        var source = @"
using System.IO;

public class MyPage : PageBase
{
    public void Page_Load()
    {
        {|#0:this.ViewState[""key""] = new MemoryStream()|};
    }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC025("System.IO.MemoryStream").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion
}
