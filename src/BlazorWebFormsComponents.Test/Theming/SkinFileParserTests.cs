using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Theming;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Theming;

/// <summary>
/// Unit tests for the SkinFileParser (Wave 2 - WI-11).
/// Tests parsing of ASP.NET Web Forms .skin files into ThemeConfiguration.
/// </summary>
public class SkinFileParserTests
{
	[Fact]
	public void ParseSkinFile_DefaultButtonSkin()
	{
		// Arrange
		var skinContent = @"<asp:Button runat=""server"" BackColor=""#507CD1"" ForeColor=""White"" Font-Bold=""true"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("#507CD1");
		skin.ForeColor.ToHtml().ShouldBe("White");
		skin.Font.ShouldNotBeNull();
		skin.Font.Bold.ShouldBeTrue();
	}

	[Fact]
	public void ParseSkinFile_NamedSkin_WithSkinID()
	{
		// Arrange
		var skinContent = @"<asp:Button SkinID=""DangerButton"" runat=""server"" BackColor=""Red"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Button", "DangerButton");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("Red");
	}

	[Fact]
	public void ParseSkinFile_GridView_WithSubStyles()
	{
		// Arrange
		var skinContent = @"
<asp:GridView runat=""server"">
	<HeaderStyle BackColor=""#507CD1"" ForeColor=""White"" Font-Bold=""true"" />
	<RowStyle BackColor=""#EFF3FB"" />
</asp:GridView>";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("GridView");
		skin.ShouldNotBeNull();
		skin.SubStyles.ShouldNotBeNull();
		skin.SubStyles.ShouldContainKey("HeaderStyle");
		skin.SubStyles.ShouldContainKey("RowStyle");

		var headerStyle = skin.SubStyles["HeaderStyle"];
		headerStyle.BackColor.ToHtml().ShouldBe("#507CD1");
		headerStyle.ForeColor.ToHtml().ShouldBe("White");
		headerStyle.Font.ShouldNotBeNull();
		headerStyle.Font.Bold.ShouldBeTrue();

		var rowStyle = skin.SubStyles["RowStyle"];
		rowStyle.BackColor.ToHtml().ShouldBe("#EFF3FB");
	}

	[Fact]
	public void ParseSkinFile_Comments_AreStripped()
	{
		// Arrange
		var skinContent = @"
<%-- This is a comment --%>
<asp:Label runat=""server"" ForeColor=""Red"" />
<%-- Another comment --%>";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Label");
		skin.ShouldNotBeNull();
		skin.ForeColor.ToHtml().ShouldBe("Red");
	}

	[Fact]
	public void ParseSkinFile_MultipleControlTypes()
	{
		// Arrange
		var skinContent = @"
<asp:Button runat=""server"" BackColor=""#507CD1"" />
<asp:Label runat=""server"" ForeColor=""Red"" />
<asp:Panel runat=""server"" BorderColor=""Black"" BorderWidth=""1px"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		config.GetSkin("Button").ShouldNotBeNull();
		config.GetSkin("Label").ShouldNotBeNull();
		config.GetSkin("Panel").ShouldNotBeNull();

		config.GetSkin("Button").BackColor.ToHtml().ShouldBe("#507CD1");
		config.GetSkin("Label").ForeColor.ToHtml().ShouldBe("Red");
		config.GetSkin("Panel").BorderColor.ToHtml().ShouldBe("Black");
	}

	[Fact]
	public void ParseSkinFile_FontProperties()
	{
		// Arrange
		var skinContent = @"<asp:Label runat=""server"" Font-Name=""Arial"" Font-Size=""14px"" Font-Italic=""true"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Label");
		skin.ShouldNotBeNull();
		skin.Font.ShouldNotBeNull();
		skin.Font.Name.ShouldBe("Arial");
		skin.Font.Size.ToString().ShouldBe("14px");
		skin.Font.Italic.ShouldBeTrue();
	}

	[Fact]
	public void ParseSkinFile_UnknownAttributes_Ignored()
	{
		// Arrange
		var skinContent = @"<asp:Button runat=""server"" SomeUnknownProp=""value"" BackColor=""Red"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert - should not throw, and known properties should still be parsed
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("Red");
	}

	[Fact]
	public void ParseSkinFile_CaseInsensitive()
	{
		// Arrange
		var skinContent = @"<asp:button RUNAT=""SERVER"" BACKCOLOR=""Red"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert - case-insensitive lookup
		config.GetSkin("button").ShouldNotBeNull();
		config.GetSkin("Button").ShouldNotBeNull();
		config.GetSkin("BUTTON").ShouldNotBeNull();
		
		var skin = config.GetSkin("button");
		skin.BackColor.ToHtml().ShouldBe("Red");
	}

	[Fact]
	public void ParseSkinFile_EmptyContent_ReturnsEmptyConfig()
	{
		// Arrange
		var skinContent = "";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		config.ShouldNotBeNull();
		config.HasSkins("Button").ShouldBeFalse();
	}

	[Fact]
	public void ParseSkinFile_NullContent_ReturnsEmptyConfig()
	{
		// Arrange
		string skinContent = null;

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		config.ShouldNotBeNull();
		config.HasSkins("Button").ShouldBeFalse();
	}

	[Fact]
	public void ParseSkinFile_ComplexFontAttributes()
	{
		// Arrange
		var skinContent = @"<asp:Label runat=""server"" 
			Font-Bold=""true"" 
			Font-Italic=""true"" 
			Font-Underline=""true"" 
			Font-Overline=""true"" 
			Font-Strikeout=""true"" 
			Font-Name=""Verdana"" 
			Font-Size=""12pt"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Label");
		skin.ShouldNotBeNull();
		skin.Font.ShouldNotBeNull();
		skin.Font.Bold.ShouldBeTrue();
		skin.Font.Italic.ShouldBeTrue();
		skin.Font.Underline.ShouldBeTrue();
		skin.Font.Overline.ShouldBeTrue();
		skin.Font.Strikeout.ShouldBeTrue();
		skin.Font.Name.ShouldBe("Verdana");
		skin.Font.Size.ToString().ShouldBe("12pt");
	}

	[Fact]
	public void ParseSkinFile_BorderStyleEnum()
	{
		// Arrange
		var skinContent = @"<asp:Panel runat=""server"" BorderStyle=""Solid"" BorderWidth=""2px"" BorderColor=""#333333"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Panel");
		skin.ShouldNotBeNull();
		skin.BorderStyle.ShouldBe(BorderStyle.Solid);
		skin.BorderWidth.ShouldNotBeNull();
		skin.BorderWidth.Value.ToString().ShouldBe("2px");
		skin.BorderColor.ToHtml().ShouldBe("#333333");
	}

	[Fact]
	public void ParseSkinFile_MixedDefaultAndNamedSkins()
	{
		// Arrange
		var skinContent = @"
<asp:Button runat=""server"" BackColor=""#507CD1"" />
<asp:Button SkinID=""DangerButton"" runat=""server"" BackColor=""Red"" />
<asp:Button SkinID=""SuccessButton"" runat=""server"" BackColor=""Green"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		config.GetSkin("Button").ShouldNotBeNull();
		config.GetSkin("Button").BackColor.ToHtml().ShouldBe("#507CD1");

		config.GetSkin("Button", "DangerButton").ShouldNotBeNull();
		config.GetSkin("Button", "DangerButton").BackColor.ToHtml().ShouldBe("Red");

		config.GetSkin("Button", "SuccessButton").ShouldNotBeNull();
		config.GetSkin("Button", "SuccessButton").BackColor.ToHtml().ShouldBe("Green");
	}

	[Fact]
	public void ParseSkinFile_AddsToExistingConfig()
	{
		// Arrange
		var existingConfig = new ThemeConfiguration()
			.ForControl("Label", skin => skin.Set(s => s.ForeColor, WebColor.Blue));

		var skinContent = @"<asp:Button runat=""server"" BackColor=""Red"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent, existingConfig);

		// Assert
		config.GetSkin("Label").ShouldNotBeNull();
		config.GetSkin("Button").ShouldNotBeNull();
		config.GetSkin("Label").ForeColor.ToHtml().ShouldBe("Blue");
		config.GetSkin("Button").BackColor.ToHtml().ShouldBe("Red");
	}

	[Fact]
	public void ParseSkinFile_WidthAndHeight_Units()
	{
		// Arrange
		var skinContent = @"<asp:Panel runat=""server"" Width=""200px"" Height=""150px"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Panel");
		skin.ShouldNotBeNull();
		skin.Width.ShouldNotBeNull();
		skin.Width.Value.ToString().ShouldBe("200px");
		skin.Height.ShouldNotBeNull();
		skin.Height.Value.ToString().ShouldBe("150px");
	}

	[Fact]
	public void ParseSkinFile_CssClassAndToolTip()
	{
		// Arrange
		var skinContent = @"<asp:Button runat=""server"" CssClass=""btn-primary"" ToolTip=""Click me!"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.CssClass.ShouldBe("btn-primary");
		skin.ToolTip.ShouldBe("Click me!");
	}

	[Fact]
	public void ParseSkinFile_GridView_MultipleSubStyles()
	{
		// Arrange
		var skinContent = @"
<asp:GridView runat=""server"">
	<HeaderStyle BackColor=""#507CD1"" ForeColor=""White"" Font-Bold=""true"" />
	<RowStyle BackColor=""#EFF3FB"" />
	<AlternatingRowStyle BackColor=""White"" />
	<FooterStyle BackColor=""#507CD1"" ForeColor=""White"" Font-Bold=""true"" />
</asp:GridView>";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("GridView");
		skin.ShouldNotBeNull();
		skin.SubStyles.ShouldNotBeNull();
		skin.SubStyles.Count.ShouldBe(4);
		skin.SubStyles.ShouldContainKey("HeaderStyle");
		skin.SubStyles.ShouldContainKey("RowStyle");
		skin.SubStyles.ShouldContainKey("AlternatingRowStyle");
		skin.SubStyles.ShouldContainKey("FooterStyle");

		skin.SubStyles["AlternatingRowStyle"].BackColor.ToHtml().ShouldBe("White");
		skin.SubStyles["FooterStyle"].ForeColor.ToHtml().ShouldBe("White");
	}

	[Fact]
	public void ParseSkinFile_MultilineComment_Stripped()
	{
		// Arrange
		var skinContent = @"
<%-- 
This is a multiline comment
that spans several lines
--%>
<asp:Button runat=""server"" BackColor=""Blue"" />";

		// Act
		var config = SkinFileParser.ParseSkinFile(skinContent);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("Blue");
	}
}
