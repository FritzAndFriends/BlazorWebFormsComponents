using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace BlazorWebFormsComponents.Analyzers.Test;

using AnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    MissingParameterAttributeAnalyzer,
    DefaultVerifier>;

/// <summary>
/// Tests for BWFC001: public properties on WebControl/CompositeControl subclasses
/// should have [Parameter] attribute.
/// </summary>
public class MissingParameterAttributeAnalyzerTests
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
        public int TabIndex { get; set; }
        public string Style { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public string BorderColor { get; set; }
        public string BorderWidth { get; set; }
        public string BorderStyle { get; set; }
        public string Font { get; set; }
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

    #region Positive cases — BWFC001 SHOULD fire

    [Fact]
    public async Task PublicProperty_OnWebControlSubclass_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    public string {|#0:Text|} { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC001("Text").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PublicProperty_OnCompositeControlSubclass_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyComposite : CompositeControl
{
    public int {|#0:PageSize|} { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC001("PageSize").WithLocation(0) }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task MultipleProperties_MissingParameter_ReportsAll()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    public string {|#0:Text|} { get; set; }
    public int {|#1:PageSize|} { get; set; }
    public bool {|#2:AutoPostBack|} { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics =
            {
                ExpectBWFC001("Text").WithLocation(0),
                ExpectBWFC001("PageSize").WithLocation(1),
                ExpectBWFC001("AutoPostBack").WithLocation(2),
            }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PublicProperty_OnIndirectWebControlDescendant_ReportsDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MiddleBase : WebControl { }

public class MyControl : MiddleBase
{
    public string {|#0:Label|} { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } },
            ExpectedDiagnostics = { ExpectBWFC001("Label").WithLocation(0) }
        };
        await test.RunAsync();
    }

    #endregion

    #region Negative cases — BWFC001 should NOT fire

    [Fact]
    public async Task PropertyWithParameterAttribute_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class MyControl : WebControl
{
    [Parameter]
    public string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Theory]
    [InlineData("ID")]
    [InlineData("CssClass")]
    [InlineData("Enabled")]
    [InlineData("Visible")]
    [InlineData("TabIndex")]
    [InlineData("Style")]
    [InlineData("Width")]
    [InlineData("Height")]
    [InlineData("BackColor")]
    [InlineData("ForeColor")]
    [InlineData("BorderColor")]
    [InlineData("BorderWidth")]
    [InlineData("BorderStyle")]
    [InlineData("Font")]
    [InlineData("ViewState")]
    [InlineData("EnableViewState")]
    [InlineData("Parent")]
    [InlineData("Controls")]
    public async Task InheritedBaseProperty_NoDiagnostic(string propertyName)
    {
        var source = $@"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{{
    public new string {propertyName} {{ get; set; }}
}}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PropertyOnNonWebControlClass_NoDiagnostic()
    {
        var source = @"
public class PlainClass
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
    public async Task PrivateProperty_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    private string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ProtectedProperty_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    protected string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task InternalProperty_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    internal string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task StaticProperty_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl
{
    public static string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task EmptyWebControlSubclass_NoDiagnostic()
    {
        var source = @"
using BlazorWebFormsComponents.CustomControls;

public class MyControl : WebControl { }";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PropertyOnClassInWrongNamespace_NoDiagnostic()
    {
        // A class inheriting from a WebControl in a non-BWFC namespace should not trigger
        var source = @"
namespace SomeOtherLibrary
{
    public class WebControl { }
}

public class MyControl : SomeOtherLibrary.WebControl
{
    public string Text { get; set; }
}";

        var test = new AnalyzerTest
        {
            TestState = { Sources = { source, StubSource } }
        };
        await test.RunAsync();
    }

    #endregion
}
