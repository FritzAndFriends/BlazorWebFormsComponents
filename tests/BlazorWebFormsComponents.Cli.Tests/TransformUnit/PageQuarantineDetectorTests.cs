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
            @"D:\input\Mobile\Offers.aspx",
            @"D:\output\Mobile\Offers.razor",
            "<asp:Label ID=\"OffersTitle\" runat=\"server\" Text=\"Offers\" />",
            null);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Mobile/Offers\"", transformedCodeBehind: null, emissionPlan: null);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("Mobile-specific page or shell", decision.DetectedFeatures);
        Assert.Equal("Mobile/Offers.aspx", decision.RelativeSourcePath);
        Assert.Null(decision.ArtifactContent);
    }

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineEssentialProductPagesForMinorIdentitySignals()
    {
        var metadata = CreateMetadata(
            @"D:\input\ProductList.aspx",
            @"D:\output\ProductList.razor",
            "<asp:Label ID=\"Title\" runat=\"server\" Text=\"Products\" />",
            "using Microsoft.AspNet.Identity; namespace TestApp; public partial class ProductList { }");

        var decision = _detector.AnalyzeLate(metadata, "@page \"/ProductList\"", metadata.CodeBehindContent, emissionPlan: null);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineEssentialShoppingCartPages()
    {
        var metadata = CreateMetadata(
            @"D:\input\ShoppingCart.aspx",
            @"D:\output\ShoppingCart.razor",
            "<asp:GridView ID=\"Cart\" runat=\"server\" />",
            "using Microsoft.AspNet.Identity; namespace TestApp; public partial class ShoppingCart { }");

        var decision = _detector.AnalyzeLate(metadata, "@page \"/ShoppingCart\"", metadata.CodeBehindContent, emissionPlan: null);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineDefaultPage()
    {
        var metadata = CreateMetadata(
            @"D:\input\Default.aspx",
            @"D:\output\Default.razor",
            "<asp:Label ID=\"HomeTitle\" runat=\"server\" Text=\"Home\" />",
            "using Microsoft.AspNet.Identity; namespace TestApp; public partial class _Default { }");

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Default\"", metadata.CodeBehindContent, emissionPlan: null);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_StillQuarantinesAccountManagePagesForIdentitySignals()
    {
        var metadata = CreateMetadata(
            @"D:\input\Account\Manage.aspx",
            @"D:\output\Account\Manage.razor",
            "<asp:Login ID=\"ManageLogin\" runat=\"server\" />",
            "using Microsoft.AspNet.Identity; namespace TestApp.Account; public partial class Manage { }");

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Account/Manage\"", metadata.CodeBehindContent, emissionPlan: null);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("ASP.NET Identity or membership APIs", decision.DetectedFeatures);
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
