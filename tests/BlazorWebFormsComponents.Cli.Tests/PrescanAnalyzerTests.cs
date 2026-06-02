using BlazorWebFormsComponents.Cli.Config;

namespace BlazorWebFormsComponents.Cli.Tests;

public class PrescanAnalyzerTests
{
    [Fact]
    public void PrescanAnalyzer_FindsCommonMigrationPatterns()
    {
        var dir = TestHelpers.CreateTempProjectDir("prescan");
        try
        {
            File.WriteAllText(Path.Combine(dir, "Sample.cs"), """
                public class Sample
                {
                    public string Name { get; set; }

                    public void Go()
                    {
                        if (IsPostBack)
                        {
                            Response.Redirect("~/Done.aspx");
                        }

                        var x = ViewState["key"];
                    }
                }
                """);

            var analyzer = new PrescanAnalyzer();
            var result = analyzer.Analyze(dir);

            Assert.True(result.TotalFiles >= 1);
            Assert.True(result.FilesWithMatches >= 1);
            Assert.Contains("BWFC002", result.Summary.Keys);
            Assert.Contains("BWFC003", result.Summary.Keys);
            Assert.Contains("BWFC004", result.Summary.Keys);
        }
        finally
        {
            TestHelpers.CleanupTempDir(dir);
        }
    }

    [Fact]
    public void PrescanAnalyzer_ToJson_ProducesExpectedShape()
    {
        var result = new PrescanResult
        {
            SourcePath = @"D:\src\LegacyApp",
            TotalFiles = 1,
            FilesWithMatches = 1,
            TotalMatches = 2
        };
        result.Summary["BWFC003"] = new PrescanSummary("IsPostBack", "IsPostBack checks", 2, 1);
        result.Files.Add(new PrescanFileResult("Default.aspx.cs", [new PrescanFileMatch("BWFC003", "IsPostBack", 2, [10, 14])]));

        var json = PrescanAnalyzer.ToJson(result);

        Assert.Contains("\"SourcePath\"", json);
        Assert.Contains("\"BWFC003\"", json);
        Assert.Contains("\"Default.aspx.cs\"", json);
    }

    [Fact]
    public void PrescanAnalyzer_IncludesAscxDescriptors()
    {
        var dir = TestHelpers.CreateTempProjectDir("prescan-ascx");
        try
        {
            var ascxPath = Path.Combine(dir, "ProductsControl.ascx");
            File.WriteAllText(ascxPath, """
                <%@ Control Language="C#" CodeBehind="ProductsControl.ascx.cs" Inherits="WingtipToys.ProductsControl" %>
                <asp:GridView ID="gvProducts" runat="server" />
                """);

            File.WriteAllText(ascxPath + ".cs", """
                using System;
                using System.Web.UI;

                namespace WingtipToys;

                public partial class ProductsControl : UserControl
                {
                    public int? CategoryID { get; set; }
                    public event EventHandler? ProductSelected;
                }
                """);

            var analyzer = new PrescanAnalyzer();
            var result = analyzer.Analyze(dir);

            var descriptor = Assert.Single(result.AscxDescriptors);
            Assert.Equal("ProductsControl", descriptor.ControlName);
            Assert.Contains(descriptor.Properties, property => property.Name == "CategoryID");

            var json = PrescanAnalyzer.ToJson(result);
            Assert.Contains("\"AscxDescriptors\"", json);
            Assert.Contains("\"ProductsControl\"", json);
        }
        finally
        {
            TestHelpers.CleanupTempDir(dir);
        }
    }

    [Fact]
    public void PrescanAnalyzer_ReportsCustomControlRegistrations()
    {
        var dir = TestHelpers.CreateTempProjectDir("prescan-controls");
        try
        {
            File.WriteAllText(Path.Combine(dir, "Web.config"), """
                <?xml version="1.0" encoding="utf-8"?>
                <configuration>
                  <system.web>
                    <pages>
                      <controls>
                        <add assembly="Microsoft.AspNet.Web.Optimization.WebForms" namespace="Microsoft.AspNet.Web.Optimization.WebForms" tagPrefix="webopt" />
                      </controls>
                    </pages>
                  </system.web>
                </configuration>
                """);

            File.WriteAllText(Path.Combine(dir, "Students.aspx"), """
                <%@ Page Language="C#" %>
                <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
                """);

            var analyzer = new PrescanAnalyzer();

            var result = analyzer.Analyze(dir);
            var json = PrescanAnalyzer.ToJson(result);

            Assert.Contains(result.CustomControlRegistrations.NamespaceTags, registration =>
                registration.TagPrefix == "webopt"
                && registration.Namespace == "Microsoft.AspNet.Web.Optimization.WebForms");
            Assert.Contains(result.CustomControlRegistrations.RegisterDirectives, registration =>
                registration.TagPrefix == "ajaxToolkit"
                && registration.AssemblyName == "AjaxControlToolkit");
            Assert.Contains("\"CustomControlRegistrations\"", json);
            Assert.Contains("\"webopt\"", json);
        }
        finally
        {
            TestHelpers.CleanupTempDir(dir);
        }
    }
}
