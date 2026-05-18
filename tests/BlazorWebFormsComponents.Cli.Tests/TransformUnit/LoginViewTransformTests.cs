using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for LoginViewTransform — converts asp:LoginView to AuthorizeView.
/// Corresponds to TC22-LoginView test case.
/// </summary>
public class LoginViewTransformTests
{
    private readonly LoginViewTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void ConvertsLoginViewOpenAndClose()
    {
        var input = @"<asp:LoginView runat=""server"">
</asp:LoginView>";
        var expected = @"<AuthorizeView>
</AuthorizeView>";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsAnonymousTemplate()
    {
        var input = @"<AnonymousTemplate>
    <p>Please log in</p>
</AnonymousTemplate>";
        var expected = @"<NotAuthorized>
    <p>Please log in</p>
</NotAuthorized>";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsLoggedInTemplate()
    {
        var input = @"<LoggedInTemplate>
    <p>Welcome back!</p>
</LoggedInTemplate>";
        var expected = @"<Authorized>
    <p>Welcome back!</p>
</Authorized>";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void StripsRunatAndIdAttributes()
    {
        var input = @"<asp:LoginView ID=""LoginView1"" runat=""server"">
</asp:LoginView>";
        var expected = @"<AuthorizeView>
</AuthorizeView>";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsRoleGroupsToTodoComment()
    {
        var input = @"<RoleGroups>
    <asp:RoleGroup Roles=""Admin"">
        <ContentTemplate>Admin content</ContentTemplate>
    </asp:RoleGroup>
</RoleGroups>";
        var expected = @"@* TODO(bwfc-identity): Convert RoleGroups to policy-based AuthorizeView *@";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HandlesSelfClosingLoginView()
    {
        var input = @"<asp:LoginView ID=""lv1"" runat=""server"" />";
        var expected = @"<AuthorizeView />";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsFullLoginViewWithAllTemplates()
    {
        var input = @"<asp:LoginView ID=""LoginView1"" runat=""server"">
    <AnonymousTemplate>
        <a href=""~/Login.aspx"">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <span>Welcome, user!</span>
    </LoggedInTemplate>
    <RoleGroups>
        <asp:RoleGroup Roles=""Admin"">
            <ContentTemplate><span>Admin Panel</span></ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>";

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<AuthorizeView>", result);
        Assert.Contains("</AuthorizeView>", result);
        Assert.Contains("<NotAuthorized>", result);
        Assert.Contains("</NotAuthorized>", result);
        Assert.Contains("<Authorized>", result);
        Assert.Contains("</Authorized>", result);
        Assert.Contains("@* TODO(bwfc-identity): Convert RoleGroups to policy-based AuthorizeView *@", result);
        Assert.DoesNotContain("asp:LoginView", result);
        Assert.DoesNotContain("runat", result);
        Assert.DoesNotContain("ID=\"LoginView1\"", result);
    }

    [Fact]
    public void PreservesNonLoginViewContent()
    {
        var input = @"<div class=""header""><span>Hello</span></div>";

        var result = _transform.Apply(input, TestMetadata);
        Assert.Equal(input, result);
    }

    [Fact]
    public void OrderIs510()
    {
        Assert.Equal(510, _transform.Order);
    }
}
