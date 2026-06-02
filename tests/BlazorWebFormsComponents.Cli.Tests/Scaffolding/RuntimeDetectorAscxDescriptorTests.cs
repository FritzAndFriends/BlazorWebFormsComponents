namespace BlazorWebFormsComponents.Cli.Tests.Scaffolding;

public class RuntimeDetectorAscxDescriptorTests
{
    [Fact]
    public void Detect_IncludesAscxDescriptorsForUserControls()
    {
        var dir = TestHelpers.CreateTempProjectDir("runtime-ascx");

        try
        {
            var ascxPath = Path.Combine(dir, "CartSummary.ascx");
            File.WriteAllText(ascxPath, """
                <%@ Control Language="C#" CodeBehind="CartSummary.ascx.cs" Inherits="WingtipToys.CartSummary" %>
                <asp:Label ID="SummaryLabel" runat="server" />
                """);

            File.WriteAllText(ascxPath + ".cs", """
                using System.Web.UI;

                namespace WingtipToys;

                public partial class CartSummary : UserControl
                {
                    public string CartId { get; set; } = string.Empty;
                }
                """);

            var detector = TestHelpers.CreateDefaultRuntimeDetector();

            var profile = detector.Detect(dir);

            var descriptor = Assert.Single(profile.AscxDescriptors);
            Assert.Equal("CartSummary", descriptor.ControlName);
            Assert.Contains(descriptor.Properties, property => property.Name == "CartId");
        }
        finally
        {
            TestHelpers.CleanupTempDir(dir);
        }
    }
}
