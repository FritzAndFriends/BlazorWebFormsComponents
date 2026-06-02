using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class LegacyHelperStubTransformTests
{
    private readonly LegacyHelperStubTransform _transform = new();

    [Fact]
    public void SystemWeb_WithOnlyHttpContextCurrent_PassesThrough()
    {
        // Files with System.Web + HttpContext.Current should NOT be stubbed —
        // HttpContextAccessorTransform (order 108) handles those patterns
        var content = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Web;
            using TestApp.Models;

            namespace TestApp.Logic
            {
                public class ShoppingCartActions : IDisposable
                {
                    public string ShoppingCartId { get; set; }
                    private ProductContext _db = new ProductContext();
                    public const string CartSessionKey = "CartId";

                    public void AddToCart(int id)
                    {
                        ShoppingCartId = GetCartId();
                        var cartItem = _db.ShoppingCartItems.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == id);
                    }

                    public void Dispose()
                    {
                        if (_db != null) { _db.Dispose(); _db = null; }
                    }

                    public string GetCartId()
                    {
                        if (HttpContext.Current.Session[CartSessionKey] == null)
                        {
                            HttpContext.Current.Session[CartSessionKey] = Guid.NewGuid().ToString();
                        }
                        return HttpContext.Current.Session[CartSessionKey].ToString();
                    }

                    public List<CartItem> GetCartItems()
                    {
                        return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
                    }

                    public decimal GetTotal()
                    {
                        return 0m;
                    }

                    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] updates)
                    {
                        // complex logic
                    }

                    public struct ShoppingCartUpdates
                    {
                        public int ProductId;
                        public int PurchaseQuantity;
                        public bool RemoveItem;
                    }
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\Logic\ShoppingCartActions.cs",
            OutputFilePath = @"D:\output\Logic\ShoppingCartActions.cs",
            FileType = FileType.CodeFile,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        // Should NOT be stubbed — HttpContext.Current alone is handled by HttpContextAccessorTransform
        Assert.DoesNotContain("Auto-generated API-compatible stub", result);
        Assert.Equal(content, result);
    }

    [Fact]
    public void SystemWeb_WithHttpRuntime_IsStubbed()
    {
        // Files with truly untransformable APIs (HttpRuntime, HttpServerUtility) SHOULD be stubbed
        var content = """
            using System.Web;
            
            namespace TestApp.Logic;
            
            public class CacheHelper
            {
                public string GetAppPath()
                {
                    return HttpRuntime.AppDomainAppPath;
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\Logic\CacheHelper.cs",
            OutputFilePath = @"D:\output\Logic\CacheHelper.cs",
            FileType = FileType.CodeFile,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Contains("Auto-generated API-compatible stub", result);
        Assert.Contains("public string GetAppPath()", result);
    }

    [Fact]
    public void SystemWeb_WithoutHeavyApis_IsNotStubbed()
    {
        // Files that only use HttpUtility from System.Web should NOT be stubbed
        var content = """
            using System.Web;
            
            namespace TestApp.Logic;
            
            public class PayPalFunctions
            {
                public string Encode(string value) => HttpUtility.UrlEncode(value);
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\Logic\PayPalFunctions.cs",
            OutputFilePath = @"D:\output\Logic\PayPalFunctions.cs",
            FileType = FileType.CodeFile,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        // Should NOT be stubbed — just uses HttpUtility which can be rewritten
        Assert.DoesNotContain("Auto-generated API-compatible stub", result);
        Assert.Contains("HttpUtility.UrlEncode", result);
    }

    [Fact]
    public void LegacyIdentityNamespace_IsStubbed()
    {
        var content = """
            using Microsoft.AspNet.Identity;
            
            namespace TestApp;
            
            public class IdentityHelper
            {
                public void SignIn(string userName)
                {
                    var manager = new UserManager();
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\IdentityHelper.cs",
            OutputFilePath = @"D:\output\IdentityHelper.cs",
            FileType = FileType.CodeFile,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Contains("Auto-generated API-compatible stub", result);
        Assert.Contains("public void SignIn(string userName)", result);
    }

    [Theory]
    [InlineData(FileType.Page, @"D:\input\MyPage.aspx", @"D:\output\MyPage.razor.cs")]
    [InlineData(FileType.Master, @"D:\input\Site.master", @"D:\output\Site.razor.cs")]
    [InlineData(FileType.Control, @"D:\input\Widget.ascx", @"D:\output\Widget.razor.cs")]
    public void PageMasterAndControlCodeBehinds_AreSkipped(FileType fileType, string sourcePath, string outputPath)
    {
        var content = """
            using System.Configuration;
            
            namespace TestApp;
            
            public partial class MyPage
            {
                protected void Page_Load(object sender, EventArgs e)
                {
                    var setting = ConfigurationManager.AppSettings["Mode"];
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = sourcePath,
            OutputFilePath = outputPath,
            FileType = fileType,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Equal(content, result);
        Assert.DoesNotContain("Auto-generated API-compatible stub", result);
    }

    [Fact]
    public void ProtectedHelperMethods_ArePreservedInStub()
    {
        var content = """
            using System.Web.Security;

            namespace TestApp;

            public class SecurityHelper
            {
                protected void LoadSetting(object sender, EventArgs e)
                {
                    var role = Roles.GetRolesForUser("admin");
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\SecurityHelper.cs",
            OutputFilePath = @"D:\output\SecurityHelper.cs",
            FileType = FileType.CodeFile,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Contains("protected void LoadSetting(object sender, EventArgs e) { }", result);
    }

    [Fact]
    public void StaticClass_PreservesStaticModifier()
    {
        var content = """
            using System.Web.Security;
            
            namespace TestApp;
            
            public static class SecurityHelper
            {
                public static bool IsAdmin(string userName)
                {
                    return Roles.IsUserInRole(userName, "Admin");
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\SecurityHelper.cs",
            OutputFilePath = @"D:\output\SecurityHelper.cs",
            FileType = FileType.CodeFile,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Contains("public static partial class SecurityHelper", result);
        Assert.Contains("public static bool IsAdmin(string userName)", result);
    }
}
