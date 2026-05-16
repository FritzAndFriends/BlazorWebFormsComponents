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
    public void AnalyzeLate_DoesNotQuarantineRedirectOnlyActionPageWithCompileSurfaceBlocker()
    {
        var metadata = CreateMetadata(
            @"D:\input\Handlers\ApplyCoupon.aspx",
            @"D:\output\Handlers\ApplyCoupon.razor",
            "<%@ Page Language=\"C#\" %>",
            "namespace TestApp.Handlers; public partial class ApplyCoupon { protected void Page_Load(object sender, EventArgs e) { Response.Redirect(\"~/Cart.aspx\"); } }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Compile surface blocker", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/ApplyCoupon\"\n<PageTitle>ApplyCoupon</PageTitle>\n<div>   </div>", metadata.CodeBehindContent, emissionPlan);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_StillQuarantinesRedirectOnlyIdentityPage()
    {
        var metadata = CreateMetadata(
            @"D:\input\Account\PasswordResetBridge.aspx",
            @"D:\output\Account\PasswordResetBridge.razor",
            "<%@ Page Language=\"C#\" %>",
            "using Microsoft.AspNet.Identity; namespace TestApp.Account; public partial class PasswordResetBridge { protected void Page_Load(object sender, EventArgs e) { Response.Redirect(\"~/Account/Login.aspx\"); } }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Compile surface blocker", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/PasswordResetBridge\"\n<div> </div>", metadata.CodeBehindContent, emissionPlan);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("ASP.NET Identity or membership APIs", decision.DetectedFeatures);
    }

    [Fact]
    public void AnalyzeLate_DoesNotChangeNormalPageBehaviorWhenRedirectIsPresent()
    {
        var metadata = CreateMetadata(
            @"D:\input\Admin\Orders.aspx",
            @"D:\output\Admin\Orders.razor",
            "<asp:Label ID=\"Title\" runat=\"server\" Text=\"Orders\" />",
            "namespace TestApp.Admin; public partial class Orders { protected void Page_Load(object sender, EventArgs e) { Response.Redirect(\"~/Admin/Orders.aspx\"); } }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Compile surface blocker", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Admin/Orders\"\n<asp:Label ID=\"Title\" runat=\"server\" Text=\"Orders\" />", metadata.CodeBehindContent, emissionPlan);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("Unresolved compile-surface blockers", decision.DetectedFeatures);
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

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineStudentsPageWithCompileSurfaceBlockerOnly()
    {
        // ContosoUniversity core page — must never be quarantined solely because of compile issues.
        var metadata = CreateMetadata(
            @"D:\input\Students.aspx",
            @"D:\output\Students.razor",
            "<asp:GridView ID=\"StudentsGrid\" SelectMethod=\"GetStudents\" runat=\"server\" />",
            "namespace TestApp; public partial class Students : WebFormsPageBase { }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Unresolved compile-surface blockers", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Students\"", metadata.CodeBehindContent, emissionPlan);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineCoursesPageWithCompileSurfaceBlockerOnly()
    {
        var metadata = CreateMetadata(
            @"D:\input\Courses.aspx",
            @"D:\output\Courses.razor",
            "<asp:GridView ID=\"CoursesGrid\" SelectMethod=\"GetCourses\" runat=\"server\" />",
            "namespace TestApp; public partial class Courses : WebFormsPageBase { }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Unresolved compile-surface blockers", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Courses\"", metadata.CodeBehindContent, emissionPlan);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineInstructorsPageWithCompileSurfaceBlockerOnly()
    {
        var metadata = CreateMetadata(
            @"D:\input\Instructors.aspx",
            @"D:\output\Instructors.razor",
            "<asp:GridView ID=\"InstructorsGrid\" SelectMethod=\"GetInstructors\" runat=\"server\" />",
            "namespace TestApp; public partial class Instructors : WebFormsPageBase { }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Unresolved compile-surface blockers", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Instructors\"", metadata.CodeBehindContent, emissionPlan);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_DoesNotQuarantineRootLevelPageWithCompileSurfaceBlockerOnly()
    {
        // Any root-level page (no subdirectory) is treated as essential and should not be
        // quarantined merely because of compile-surface blockers.
        var metadata = CreateMetadata(
            @"D:\input\Departments.aspx",
            @"D:\output\Departments.razor",
            "<asp:ListView ID=\"DeptList\" runat=\"server\" />",
            "namespace TestApp; public partial class Departments : WebFormsPageBase { }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Compile surface blocker", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Departments\"", metadata.CodeBehindContent, emissionPlan);

        Assert.False(decision.ShouldQuarantine);
    }

    [Fact]
    public void AnalyzeLate_QuarantinesAdminPageEvenWithOnlyCompileSurfaceBlocker()
    {
        // Admin pages on quarantinable paths are still quarantined even without payment/identity signals.
        var metadata = CreateMetadata(
            @"D:\input\Admin\Reports.aspx",
            @"D:\output\Admin\Reports.razor",
            "<asp:GridView ID=\"ReportsGrid\" runat=\"server\" />",
            "namespace TestApp.Admin; public partial class Reports { }");
        var emissionPlan = new CodeBehindEmissionPlan(EmitToCompileSurface: false, ArtifactReason: "Compile surface blocker", ArtifactContent: metadata.CodeBehindContent);

        var decision = _detector.AnalyzeLate(metadata, "@page \"/Admin/Reports\"", metadata.CodeBehindContent, emissionPlan);

        Assert.True(decision.ShouldQuarantine);
        Assert.Contains("Unresolved compile-surface blockers", decision.DetectedFeatures);
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
