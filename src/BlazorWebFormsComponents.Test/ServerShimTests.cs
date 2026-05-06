using System;
using System.IO;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for <see cref="ServerShim"/>.
/// </summary>
public class ServerShimTests
{
    private const string ContentRoot = @"C:\app";
    private const string WebRoot = @"C:\app\wwwroot";

    private ServerShim CreateShim(string? webRootPath = WebRoot, MockNavigationManager? navigationManager = null)
    {
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.ContentRootPath).Returns(ContentRoot);
        mockEnv.Setup(e => e.WebRootPath).Returns(webRootPath!);
        return new ServerShim(mockEnv.Object, navigationManager);
    }

    #region MapPath

    [Fact]
    public void MapPath_TildeSlash_ResolvesToWebRootPath()
    {
        var shim = CreateShim();

        var result = shim.MapPath("~/images/logo.png");

        result.ShouldBe(Path.Combine(WebRoot, "images", "logo.png"));
    }

    [Fact]
    public void MapPath_TildeSlash_SubDirectory_ResolvesCorrectly()
    {
        var shim = CreateShim();

        var result = shim.MapPath("~/css/site.css");

        result.ShouldBe(Path.Combine(WebRoot, "css", "site.css"));
    }

    [Fact]
    public void MapPath_TildeSlash_WhenWebRootNull_FallsBackToContentRoot()
    {
        var shim = CreateShim(webRootPath: null);

        var result = shim.MapPath("~/images/logo.png");

        result.ShouldBe(Path.Combine(ContentRoot, "images", "logo.png"));
    }

    [Fact]
    public void MapPath_RelativePath_ResolvesToContentRoot()
    {
        var shim = CreateShim();

        var result = shim.MapPath("App_Data/users.xml");

        result.ShouldBe(Path.Combine(ContentRoot, "App_Data", "users.xml"));
    }

    [Fact]
    public void MapPath_LeadingSlash_ResolvesToContentRoot()
    {
        var shim = CreateShim();

        var result = shim.MapPath("/bin/debug.log");

        result.ShouldBe(Path.Combine(ContentRoot, "bin", "debug.log"));
    }

    [Fact]
    public void MapPath_EmptyString_ReturnsContentRootPath()
    {
        var shim = CreateShim();

        var result = shim.MapPath("");

        result.ShouldBe(ContentRoot);
    }

    [Fact]
    public void MapPath_Null_ReturnsContentRootPath()
    {
        var shim = CreateShim();

        var result = shim.MapPath(null!);

        result.ShouldBe(ContentRoot);
    }

    #endregion

    #region HtmlEncode / HtmlDecode

    [Fact]
    public void HtmlEncode_EncodesSpecialCharacters()
    {
        var shim = CreateShim();

        var encoded = shim.HtmlEncode("<script>alert('xss')</script>");

        encoded.ShouldContain("&lt;");
        encoded.ShouldContain("&gt;");
    }

    [Fact]
    public void HtmlEncode_HtmlDecode_RoundTrips()
    {
        var shim = CreateShim();
        var original = "<div class=\"test\">&amp;</div>";

        var encoded = shim.HtmlEncode(original);
        var decoded = shim.HtmlDecode(encoded);

        decoded.ShouldBe(original);
    }

    #endregion

    #region UrlEncode / UrlDecode

    [Fact]
    public void UrlEncode_EncodesSpacesAndSpecialChars()
    {
        var shim = CreateShim();

        var encoded = shim.UrlEncode("hello world&foo=bar");

        encoded.ShouldContain("+");
        encoded.ShouldContain("%26");
    }

    [Fact]
    public void UrlEncode_UrlDecode_RoundTrips()
    {
        var shim = CreateShim();
        var original = "name=John Doe&city=New York";

        var encoded = shim.UrlEncode(original);
        var decoded = shim.UrlDecode(encoded);

        decoded.ShouldBe(original);
    }

    #endregion

    #region Transfer / Error compatibility

    [Fact]
    public void GetLastError_ReturnsNull()
    {
        var shim = CreateShim();

        shim.GetLastError().ShouldBeNull();
    }

    [Fact]
    public void ClearError_DoesNotThrow()
    {
        var shim = CreateShim();

        Should.NotThrow(() => shim.ClearError());
    }

    [Fact]
    public void Transfer_NavigatesToTargetPath()
    {
        var nav = new MockNavigationManager();
        var shim = CreateShim(navigationManager: nav);

        shim.Transfer("/products/details");

        nav.LastUri.ShouldBe("/products/details");
    }

    #endregion
}
