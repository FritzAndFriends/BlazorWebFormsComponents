using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class IdentityCodeBehindQuarantineTransformTests
{
    private readonly IdentityCodeBehindQuarantineTransform _transform = new();

    [Fact]
    public void LoginCodeBehind_WithLegacyOwinMethods_IsStubbed()
    {
        var input = """
            using System;
            using Microsoft.AspNet.Identity.Owin;
            using Microsoft.Owin.Security;
            using Owin;
            using Microsoft.AspNetCore.Components;
            
            namespace TestApp.Account;
            
            public partial class Login : WebFormsPageBase
            {
                [Inject]
                public NavigationManager Navigation { get; set; } = default!;
            
                protected void Page_Load(object sender, EventArgs e)
                {
                    RegisterHyperLink.NavigateUrl = \"Register\";
                }
            
                protected void LogIn(object sender, EventArgs e)
                {
                    var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString[\"ReturnUrl\"], Response);
                }
            }
            """;

        var result = _transform.Apply(input, CreateMetadata(@"D:\input\Account\Login.aspx.cs", FileType.Page, input));

        Assert.Contains("[Inject]", result);
        Assert.Contains("public NavigationManager Navigation { get; set; } = default!;", result);
        Assert.Contains("protected void Page_Load(object sender, EventArgs e)", result);
        Assert.Contains("protected void LogIn(object sender, EventArgs e)", result);
        Assert.Contains("Identity flow handled by generated ASP.NET Core endpoints.", result);
        Assert.DoesNotContain("RegisterHyperLink.NavigateUrl =", result);
        Assert.DoesNotContain("GetOwinContext", result);
        Assert.DoesNotContain("IdentityHelper", result);
        Assert.DoesNotContain("using Microsoft.AspNet.Identity.Owin;", result);
        Assert.DoesNotContain("using Microsoft.Owin.Security;", result);
        Assert.DoesNotContain("using Owin;", result);
    }

    [Fact]
    public void RegisterCodeBehind_WithIdentityHelperSignals_IsStubbedByContent()
    {
        var input = """
            using System;
            using Microsoft.AspNet.Identity;
            
            namespace TestApp.Auth;
            
            public partial class Register : WebFormsPageBase
            {
                protected string CompleteRegistration(string email)
                {
                    IdentityHelper.SignIn(null!, null!, isPersistent: false);
                    return email;
                }
            }
            """;

        var result = _transform.Apply(input, CreateMetadata(@"D:\input\Auth\Register.aspx.cs", FileType.Page, input));

        Assert.Contains("protected string CompleteRegistration(string email)", result);
        Assert.Contains("return default!;", result);
        Assert.DoesNotContain("IdentityHelper.SignIn", result);
        Assert.DoesNotContain("using Microsoft.AspNet.Identity;", result);
    }

    [Fact]
    public void NonIdentityCodeBehind_IsUnchanged()
    {
        var input = """
            using System;
            
            namespace TestApp;
            
            public partial class Products : WebFormsPageBase
            {
                protected void Page_Load(object sender, EventArgs e)
                {
                    Title = \"Products\";
                }
            }
            """;

        var result = _transform.Apply(input, CreateMetadata(@"D:\input\Products.aspx.cs", FileType.Page, input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void IdentityCodeBehind_AlreadyClean_IsUnchanged()
    {
        var input = """
            using Microsoft.AspNetCore.Components;
            
            namespace TestApp.Account;
            
            public partial class Login : WebFormsPageBase
            {
                [Inject]
                public NavigationManager Navigation { get; set; } = default!;
            
                [Parameter]
                public string? ReturnUrl { get; set; }
            }
            """;

        var result = _transform.Apply(input, CreateMetadata(@"D:\input\Account\Login.aspx.cs", FileType.Page, input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void OrderIs905()
    {
        Assert.Equal(905, _transform.Order);
    }

    [Fact]
    public void NameIsIdentityCodeBehindQuarantine()
    {
        Assert.Equal("IdentityCodeBehindQuarantine", _transform.Name);
    }

    private static FileMetadata CreateMetadata(string sourcePath, FileType fileType, string originalContent) => new()
    {
        SourceFilePath = sourcePath,
        OutputFilePath = sourcePath.Replace(".aspx.cs", ".razor.cs", StringComparison.OrdinalIgnoreCase),
        FileType = fileType,
        OriginalContent = originalContent,
        CodeBehindContent = originalContent
    };
}
