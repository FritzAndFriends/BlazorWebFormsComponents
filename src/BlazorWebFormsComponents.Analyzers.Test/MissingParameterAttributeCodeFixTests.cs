using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using CodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    MissingParameterAttributeAnalyzer,
    MissingParameterAttributeCodeFixProvider,
    DefaultVerifier>;

/// <summary>
/// Tests for the code fix that adds [Parameter] attribute to flagged properties.
/// </summary>
public class MissingParameterAttributeCodeFixTests
{
    private const string StubSource = @"
namespace BlazorWebFormsComponents.CustomControls
{
    public class WebControl
    {
        public string ID { get; set; }
        public string CssClass { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
    }
    public class CompositeControl : WebControl { }
}

namespace Microsoft.AspNetCore.Components
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ParameterAttribute : System.Attribute { }
}
";

    private static DiagnosticResult ExpectBWFC001(string propertyName) =>
        new DiagnosticResult(MissingParameterAttributeAnalyzer.DiagnosticId, DiagnosticSeverity.Warning)
            .WithArguments(propertyName);

    [Fact]
    public async Task CodeFix_AddsParameterAttribute()
    {
        var testSource = @"
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class MyControl : WebControl
{
    public string {|#0:Text|} { get; set; }
}";

        var fixedSource = @"
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class MyControl : WebControl
{
    [Parameter]
    public string Text { get; set; }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC001("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CodeFix_AddsUsingDirective_WhenMissing()
    {
        var testSource = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    public string {|#0:Text|} { get; set; }
}";

        var fixedSource = @"
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class MyControl : WebControl
{
    [Parameter]
    public string Text { get; set; }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC001("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task CodeFix_DoesNotDuplicateUsing_WhenAlreadyPresent()
    {
        var testSource = @"
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class MyControl : WebControl
{
    public string {|#0:Text|} { get; set; }
}";

        // The using should NOT be duplicated
        var fixedSource = @"
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class MyControl : WebControl
{
    [Parameter]
    public string Text { get; set; }
}";

        var test = new CodeFixTest
        {
            TestState = { Sources = { testSource, StubSource } },
            FixedState = { Sources = { fixedSource, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC001("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }
}
