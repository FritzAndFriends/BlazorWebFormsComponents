using BlazorWebFormsComponents.AspxMiddleware;
using Shouldly;

namespace BlazorWebFormsComponents.AspxMiddleware.Test;

public class AspxComponentRegistryTests
{
    [Theory]
    [InlineData("button", typeof(Button))]
    [InlineData("label", typeof(Label))]
    [InlineData("panel", typeof(Panel))]
    [InlineData("textbox", typeof(TextBox))]
    [InlineData("hyperlink", typeof(HyperLink))]
    [InlineData("image", typeof(Image))]
    [InlineData("checkbox", typeof(CheckBox))]
    [InlineData("dropdownlist", typeof(DropDownList<object>))]
    [InlineData("literal", typeof(Literal))]
    [InlineData("placeholder", typeof(PlaceHolder))]
    public void Resolve_KnownControl_ReturnsCorrectType(string tagName, Type expectedType)
    {
        var result = AspxComponentRegistry.Resolve(tagName);
        result.ShouldBe(expectedType);
    }

    [Fact]
    public void Resolve_UnknownControl_ReturnsNull()
    {
        var result = AspxComponentRegistry.Resolve("FancyWidget");
        result.ShouldBeNull();
    }

    [Fact]
    public void Resolve_IsCaseInsensitive()
    {
        var lower = AspxComponentRegistry.Resolve("button");
        var upper = AspxComponentRegistry.Resolve("BUTTON");
        var mixed = AspxComponentRegistry.Resolve("Button");

        lower.ShouldBe(upper);
        upper.ShouldBe(mixed);
    }

    [Theory]
    [InlineData("login", typeof(LoginControls.Login))]
    [InlineData("loginname", typeof(LoginControls.LoginName))]
    [InlineData("loginstatus", typeof(LoginControls.LoginStatus))]
    [InlineData("loginview", typeof(LoginControls.LoginView))]
    [InlineData("changepassword", typeof(LoginControls.ChangePassword))]
    [InlineData("createuserwizard", typeof(LoginControls.CreateUserWizard))]
    [InlineData("passwordrecovery", typeof(LoginControls.PasswordRecovery))]
    public void Resolve_LoginControls_ReturnsCorrectType(string tagName, Type expectedType)
    {
        var result = AspxComponentRegistry.Resolve(tagName);
        result.ShouldBe(expectedType);
    }

    [Theory]
    [InlineData("menu")]
    [InlineData("treeview")]
    [InlineData("sitemappath")]
    [InlineData("updatepanel")]
    [InlineData("timer")]
    [InlineData("calendar")]
    [InlineData("chart")]
    [InlineData("datapager")]
    public void Resolve_SpecializedControls_ReturnsNonNull(string tagName)
    {
        AspxComponentRegistry.Resolve(tagName).ShouldNotBeNull();
    }

    [Fact]
    public void Count_ReturnsExpectedNumberOfRegistrations()
    {
        // We expect a substantial number of component registrations
        AspxComponentRegistry.Count.ShouldBeGreaterThan(30);
    }

    [Fact]
    public void RegisteredTags_ContainsExpectedControls()
    {
        var tags = AspxComponentRegistry.RegisteredTags.ToList();
        tags.ShouldContain("button");
        tags.ShouldContain("label");
        tags.ShouldContain("gridview");
    }

    [Fact]
    public void RegisteredTags_ContainsDataControls()
    {
        // Generic data controls are registered with object as type arg
        AspxComponentRegistry.Resolve("gridview").ShouldNotBeNull();
        AspxComponentRegistry.Resolve("repeater").ShouldNotBeNull();
        AspxComponentRegistry.Resolve("listview").ShouldNotBeNull();
    }
}
