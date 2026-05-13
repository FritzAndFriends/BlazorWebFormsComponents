using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class LegacyHelperStubTransformTests
{
    private readonly LegacyHelperStubTransform _transform = new();

    [Fact]
    public void SystemWeb_WithHttpContextCurrent_GeneratesApiAwareStub()
    {
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
            FileType = FileType.Page,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        // Verify it was stubbed (not passed through)
        Assert.Contains("Auto-generated API-compatible stub", result);
        Assert.Contains("namespace TestApp.Logic;", result);

        // Verify IDisposable interface preserved
        Assert.Contains("IDisposable", result);

        // Verify public methods preserved
        Assert.Contains("public void AddToCart(int id)", result);
        Assert.Contains("public void Dispose()", result);
        Assert.Contains("public string GetCartId()", result);
        Assert.Contains("public List<CartItem> GetCartItems()", result);
        Assert.Contains("public decimal GetTotal()", result);
        Assert.Contains("public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] updates)", result);

        // Verify property preserved
        Assert.Contains("public string ShoppingCartId { get; set; }", result);

        // Verify constant preserved
        Assert.Contains("public const string CartSessionKey = \"CartId\";", result);

        // Verify nested struct preserved
        Assert.Contains("public struct ShoppingCartUpdates", result);
        Assert.Contains("public int ProductId;", result);
        Assert.Contains("public int PurchaseQuantity;", result);
        Assert.Contains("public bool RemoveItem;", result);

        // Dispose gets empty body (not throw)
        Assert.Contains("public void Dispose() { }", result);

        // Other methods get throw NotImplementedException
        Assert.Contains("GetCartId()", result);
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
            FileType = FileType.Page,
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
            FileType = FileType.Page,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Contains("Auto-generated API-compatible stub", result);
        Assert.Contains("public void SignIn(string userName)", result);
    }

    [Fact]
    public void PageCodeBehind_IsSkipped()
    {
        var content = """
            using System.Web;
            
            namespace TestApp;
            
            public partial class MyPage
            {
                protected void Page_Load(object sender, EventArgs e)
                {
                    var x = HttpContext.Current;
                }
            }
            """;

        var metadata = new FileMetadata
        {
            SourceFilePath = @"D:\input\MyPage.aspx.cs",
            OutputFilePath = @"D:\output\MyPage.razor.cs",
            FileType = FileType.Page,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        // Page code-behinds are NOT processed by this transform
        Assert.DoesNotContain("Auto-generated API-compatible stub", result);
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
            FileType = FileType.Page,
            OriginalContent = content,
        };

        var result = _transform.Apply(content, metadata);

        Assert.Contains("public static partial class SecurityHelper", result);
        Assert.Contains("public static bool IsAdmin(string userName)", result);
    }
}
