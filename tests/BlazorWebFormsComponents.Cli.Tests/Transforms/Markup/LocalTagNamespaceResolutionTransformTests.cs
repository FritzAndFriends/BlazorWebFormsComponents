using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.Markup;

public class LocalTagNamespaceResolutionTransformTests
{
    private readonly LocalTagNamespaceResolutionTransform _transform = new();

    private static FileMetadata MakeMetadata(Dictionary<string, string>? prefixMap = null) => new()
    {
        SourceFilePath = "Test.aspx",
        OutputFilePath = "Test.razor",
        FileType = FileType.Page,
        OriginalContent = "",
        CustomControlPrefixToNamespace = prefixMap
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    };

    [Fact]
    public void Apply_NoPrefixMap_ReturnsUnchanged()
    {
        var input = "<local:SectionPanel Title=\"Test\" />";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_LocalPrefix_SelfClosing_StripsPrefix()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["local"] = "DepartmentPortal.Controls"
        };
        var input = "<local:SectionPanel Title=\"Section\" />";
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.Equal("<SectionPanel Title=\"Section\" />", result);
    }

    [Fact]
    public void Apply_LocalPrefix_BlockElement_StripsOpenAndCloseTag()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["local"] = "DepartmentPortal.Controls"
        };
        var input = """
            <local:EmployeeCard EmployeeId="42">
                <p>Content</p>
            </local:EmployeeCard>
            """;
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.Contains("<EmployeeCard", result);
        Assert.Contains("</EmployeeCard>", result);
        Assert.DoesNotContain("local:", result);
    }

    [Fact]
    public void Apply_MultipleCustomPrefixes_BothStripped()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["local"] = "DepartmentPortal.Controls",
            ["dept"] = "DepartmentPortal.UI.Controls"
        };
        var input = "<local:SectionPanel /><dept:EmployeeGrid />";
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.Equal("<SectionPanel /><EmployeeGrid />", result);
    }

    [Fact]
    public void Apply_AspPrefix_IsSkipped()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["asp"] = "System.Web.UI.WebControls",
            ["local"] = "DepartmentPortal.Controls"
        };
        // asp: should NOT be stripped here (handled by AspPrefixTransform)
        var input = "<asp:Button Text=\"Go\" /><local:SectionPanel />";
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.Contains("<asp:Button", result);
        Assert.Contains("<SectionPanel />", result);
    }

    [Fact]
    public void Apply_AjaxToolkitPrefix_IsSkipped()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ajaxToolkit"] = "AjaxControlToolkit",
            ["local"] = "DepartmentPortal.Controls"
        };
        var input = "<ajaxToolkit:Accordion /><local:SectionPanel />";
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.Contains("<ajaxToolkit:Accordion />", result);
        Assert.Contains("<SectionPanel />", result);
    }

    [Fact]
    public void Apply_CaseInsensitivePrefix_StripsCorrectly()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Local"] = "DepartmentPortal.Controls"
        };
        var input = "<local:SectionPanel />";
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.Equal("<SectionPanel />", result);
    }

    [Fact]
    public void Apply_WithAttributes_PreservesAttributesAfterPrefixStrip()
    {
        var prefixMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["uc"] = "Contoso.Controls"
        };
        var input = """<uc:EmployeeCard EmployeeId="@Model.Id" CssClass="card" runat="server" />""";
        var result = _transform.Apply(input, MakeMetadata(prefixMap));
        Assert.StartsWith("<EmployeeCard", result);
        Assert.Contains("EmployeeId=\"@Model.Id\"", result);
        Assert.Contains("CssClass=\"card\"", result);
    }
}
