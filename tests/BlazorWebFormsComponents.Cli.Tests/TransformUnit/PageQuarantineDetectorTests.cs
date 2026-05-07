using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class PageQuarantineDetectorTests
{
    private readonly PageQuarantineDetector _detector = new();

    [Fact]
    public void AnalyzeLate_DetectsPaymentAndIdentitySignals()
    {
        var metadata = CreateMetadata(
            @"D:\input\Checkout.aspx",
            @"D:\output\Checkout.razor",
            "<asp:CreateUserWizard ID=\"RegisterWizard\" runat=\"server\" />",
            "using Microsoft.AspNet.Identity;\nusing Stripe;\nnamespace TestApp; public partial class Checkout { }");

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Checkout\"", metadata.CodeBehindContent, emissionPlan: null);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("ASP.NET Identity or membership APIs", decision.DetectedFeatures);
        Assert.Contains("Payment or checkout integration", decision.DetectedFeatures);
        Assert.Contains("Suggested migration approach:", decision.StubMarkup);
        Assert.Contains("public partial class Checkout : BlazorWebFormsComponents.WebFormsPageBase", decision.StubCodeBehind);
    }

    [Fact]
    public void AnalyzeLate_DetectsMobileSpecificPagesWithoutCodeBehind()
    {
        var metadata = CreateMetadata(
            @"D:\input\Mobile\Catalog.aspx",
            @"D:\output\Mobile\Catalog.razor",
            "<asp:Label ID=\"CatalogTitle\" runat=\"server\" Text=\"Catalog\" />",
            null);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Mobile/Catalog\"", transformedCodeBehind: null, emissionPlan: null);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("Mobile-specific page or shell", decision.DetectedFeatures);
        Assert.Equal("Mobile/Catalog.aspx", decision.RelativeSourcePath);
        Assert.Null(decision.ArtifactContent);
    }

    [Fact]
    public void AnalyzeLate_DetectsComplexAdminCrudPages()
    {
        var metadata = CreateMetadata(
            @"D:\input\Admin\Orders.aspx",
            @"D:\output\Admin\Orders.razor",
            """
            <asp:SqlDataSource ID="OrdersSource" runat="server" />
            <asp:ObjectDataSource ID="ProductsSource" runat="server" />
            <asp:GridView ID="OrdersGrid" runat="server" DataSourceID="OrdersSource" />
            <asp:DetailsView ID="OrderDetails" runat="server" DataSourceID="ProductsSource" />
            <asp:FormView ID="Summary" runat="server" DataSourceID="SummarySource" />
            """,
            "namespace TestApp.Admin; public partial class Orders { }");

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Admin/Orders\"", metadata.CodeBehindContent, emissionPlan: null);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains(decision.DetectedFeatures, feature => feature.StartsWith("Complex admin CRUD", StringComparison.Ordinal));
        Assert.Contains("Split the admin workflow", decision.SuggestedApproach);
    }

    private static FileMetadata CreateMetadata(string sourcePath, string outputPath, string originalMarkup, string? originalCodeBehind) => new()
    {
        SourceFilePath = sourcePath,
        OutputFilePath = outputPath,
        OutputRootPath = @"D:\output",
        SourceRootPath = @"D:\input",
        FileType = FileType.Page,
        OriginalContent = originalMarkup,
        CodeBehindContent = originalCodeBehind,
        ProjectNamespace = "TestApp"
    };
}
