using BlazorWebFormsComponents.Cli.Analysis;

namespace BlazorWebFormsComponents.Cli.Tests.Analysis;

public sealed class AscxDescriptorAnalyzerTests : IDisposable
{
    private readonly string _testRoot = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", $"ascx-analyzer-{Guid.NewGuid():N}");

    public AscxDescriptorAnalyzerTests()
    {
        Directory.CreateDirectory(_testRoot);
    }

    [Fact]
    public void AnalyzeControl_ParsesPublicSurfaceAndSignals()
    {
        var ascxPath = CreateMarkup("ProductsControl", """
            <%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductsControl.ascx.cs" Inherits="WingtipToys.ProductsControl" %>
            <asp:GridView ID="gvProducts" runat="server" />
            """);

        File.WriteAllText(ascxPath + ".cs", """
            using System;
            using System.Web.UI;

            namespace WingtipToys;

            public partial class ProductsControl : UserControl
            {
                public int? CategoryID { get; set; } = 7;
                public string Title => "Featured";
                public event EventHandler? OnProductSelected;

                public void LoadProducts(int categoryId, string categoryName)
                {
                    var grid = FindControl("gvProducts");
                    DataBind();
                }

                protected void Page_Load(object sender, EventArgs e)
                {
                }
            }
            """);

        var analyzer = new AscxDescriptorAnalyzer();

        var descriptor = analyzer.AnalyzeControl(ascxPath);

        Assert.Equal("ProductsControl", descriptor.ControlName);
        Assert.Equal("WingtipToys.ProductsControl", descriptor.InheritsTypeName);
        Assert.Equal("ProductsControl", descriptor.ClassName);
        Assert.Equal("UserControl", descriptor.BaseTypeName);
        Assert.True(descriptor.CodeBehindExists);
        Assert.True(descriptor.ParseSucceeded);
        Assert.True(descriptor.HasDataBindCall);
        Assert.True(descriptor.HasPageLoadOverride);
        Assert.Contains(descriptor.Properties, property => property.Name == "CategoryID"
            && property.Type == "int?"
            && property.HasGetter
            && property.HasSetter
            && property.DefaultValue == "7");
        Assert.Contains(descriptor.Properties, property => property.Name == "Title"
            && property.Type == "string"
            && property.HasGetter
            && !property.HasSetter);
        Assert.Contains(descriptor.Events, @event => @event.Name == "OnProductSelected" && @event.DelegateType == "EventHandler?");
        Assert.Contains(descriptor.Methods, method => method.Name == "LoadProducts"
            && method.ReturnType == "void"
            && method.Parameters.Count == 2
            && method.Parameters[0] == new AscxMethodParameterDescriptor("categoryId", "int")
            && method.Parameters[1] == new AscxMethodParameterDescriptor("categoryName", "string"));
        Assert.Contains("gvProducts", descriptor.ReferencedControlIds);
    }

    [Fact]
    public void AnalyzeControl_HandlesMissingCodeBehindGracefully()
    {
        var ascxPath = CreateMarkup("SummaryControl", """
            <%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SummaryControl.ascx.cs" Inherits="WingtipToys.SummaryControl" %>
            <asp:Label ID="lblSummary" runat="server" />
            """);

        var analyzer = new AscxDescriptorAnalyzer();

        var descriptor = analyzer.AnalyzeControl(ascxPath);

        Assert.Equal("SummaryControl", descriptor.ControlName);
        Assert.False(descriptor.CodeBehindExists);
        Assert.False(descriptor.ParseSucceeded);
        Assert.Empty(descriptor.Properties);
        Assert.NotEmpty(descriptor.Diagnostics);
    }

    [Fact]
    public void Analyze_HandlesMalformedCodeBehindWithoutThrowing()
    {
        var codeBehindPath = Path.Combine(_testRoot, "BrokenControl.ascx.cs");
        File.WriteAllText(codeBehindPath, """
            using System.Web.UI;

            public partial class BrokenControl : UserControl
            {
                public int CategoryID { get; set; }

                public void LoadProducts(
                {
                    DataBind(
                }
            """);

        var analyzer = new AscxDescriptorAnalyzer();

        var descriptor = analyzer.Analyze(codeBehindPath);

        Assert.Equal("BrokenControl", descriptor.ControlName);
        Assert.True(descriptor.CodeBehindExists);
        Assert.False(descriptor.ParseSucceeded);
        Assert.Contains(descriptor.Properties, property => property.Name == "CategoryID");
        Assert.NotEmpty(descriptor.Diagnostics);
    }

    private string CreateMarkup(string controlName, string content)
    {
        var path = Path.Combine(_testRoot, $"{controlName}.ascx");
        File.WriteAllText(path, content);
        return path;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            try
            {
                Directory.Delete(_testRoot, true);
            }
            catch
            {
            }
        }
    }
}
