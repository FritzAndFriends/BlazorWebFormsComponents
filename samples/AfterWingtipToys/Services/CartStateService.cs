using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WingtipToys.Services;

/// <summary>
/// Manages the shopping cart ID for the current user session via a cookie-based approach.
/// </summary>
public class CartStateService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _cartId;

    public CartStateService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CartId
    {
        get
        {
            if (_cartId != null) return _cartId;

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                _cartId = context.Request.Cookies["CartId"];
                if (string.IsNullOrEmpty(_cartId))
                {
                    _cartId = Guid.NewGuid().ToString();
                    context.Response.Cookies.Append("CartId", _cartId, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTimeOffset.Now.AddDays(30)
                    });
                }
            }
            else
            {
                _cartId = Guid.NewGuid().ToString();
            }

            return _cartId;
        }
    }
}
