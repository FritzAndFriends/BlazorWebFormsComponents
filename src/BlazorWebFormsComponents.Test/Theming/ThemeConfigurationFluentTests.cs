using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Theming;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Theming;

/// <summary>
/// Unit tests for the ThemeConfiguration fluent API and SkinBuilder (Issue #364).
/// </summary>
public class ThemeConfigurationFluentTests
{
	[Fact]
	public void ForControl_DefaultSkin_SetsBackColor()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#FFDEAD")));

		var skin = theme.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("#FFDEAD");
	}

	[Fact]
	public void ForControl_NamedSkin_SetsProperties()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", "goButton", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#006633"))
				.Set(s => s.Width, new Unit("120px")));

		var skin = theme.GetSkin("Button", "goButton");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("#006633");
		skin.Width.ShouldNotBeNull();
		skin.Width.Value.Value.ShouldBe(120);
	}

	[Fact]
	public void ForControl_DefaultAndNamed_AreSeparate()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#FFDEAD")))
			.ForControl("Button", "goButton", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#006633")));

		var defaultSkin = theme.GetSkin("Button");
		var namedSkin = theme.GetSkin("Button", "goButton");

		defaultSkin.ShouldNotBeNull();
		namedSkin.ShouldNotBeNull();
		defaultSkin.BackColor.ToHtml().ShouldBe("#FFDEAD");
		namedSkin.BackColor.ToHtml().ShouldBe("#006633");
	}

	[Fact]
	public void ForControl_NestedFontBold_SetsValue()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.Font.Bold, true));

		var skin = theme.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.Font.ShouldNotBeNull();
		skin.Font.Bold.ShouldBeTrue();
	}

	[Fact]
	public void ForControl_NestedFontName_SetsValue()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Label", skin => skin
				.Set(s => s.Font.Name, "Arial")
				.Set(s => s.Font.Italic, true));

		var skin = theme.GetSkin("Label");
		skin.Font.ShouldNotBeNull();
		skin.Font.Name.ShouldBe("Arial");
		skin.Font.Italic.ShouldBeTrue();
	}

	[Fact]
	public void ForControl_MultipleProperties_AllSet()
	{
		var theme = new ThemeConfiguration()
			.ForControl("GridView", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#FFFFFF"))
				.Set(s => s.ForeColor, WebColor.FromHtml("#000000"))
				.Set(s => s.CssClass, "grid-theme")
				.Set(s => s.BorderStyle, BorderStyle.Solid)
				.Set(s => s.BorderWidth, new Unit(1))
				.Set(s => s.Height, new Unit("200px"))
				.Set(s => s.Width, new Unit("100%"))
				.Set(s => s.ToolTip, "Themed GridView"));

		var skin = theme.GetSkin("GridView");
		skin.ShouldNotBeNull();
		skin.CssClass.ShouldBe("grid-theme");
		skin.BorderStyle.ShouldBe(BorderStyle.Solid);
		skin.BorderWidth.ShouldNotBeNull();
		skin.Height.ShouldNotBeNull();
		skin.Width.ShouldNotBeNull();
		skin.ToolTip.ShouldBe("Themed GridView");
	}

	[Fact]
	public void ForControl_ChainingMultipleControlTypes_Works()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.Red))
			.ForControl("Label", skin => skin
				.Set(s => s.ForeColor, WebColor.Blue))
			.ForControl("GridView", skin => skin
				.Set(s => s.CssClass, "themed"));

		theme.GetSkin("Button").ShouldNotBeNull();
		theme.GetSkin("Label").ShouldNotBeNull();
		theme.GetSkin("GridView").ShouldNotBeNull();
	}

	[Fact]
	public void ForControl_CaseInsensitiveLookup_Works()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.Red));

		theme.GetSkin("button").ShouldNotBeNull();
		theme.GetSkin("BUTTON").ShouldNotBeNull();
		theme.GetSkin("Button").ShouldNotBeNull();
	}

	[Fact]
	public void GetSkin_MissingSkinID_ReturnsNull()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.Red));

		theme.GetSkin("Button", "nonExistent").ShouldBeNull();
	}

	[Fact]
	public void GetSkin_MissingControlType_ReturnsNull()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.Red));

		theme.GetSkin("TextBox").ShouldBeNull();
	}

	[Fact]
	public void HasSkins_ReturnsTrueForRegisteredControl()
	{
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.Red));

		theme.HasSkins("Button").ShouldBeTrue();
		theme.HasSkins("TextBox").ShouldBeFalse();
	}

	[Fact]
	public void WebColor_FromHtml_CreatesColor()
	{
		var color = WebColor.FromHtml("#FFDEAD");
		color.ShouldNotBeNull();
		color.IsEmpty.ShouldBeFalse();
		color.ToHtml().ShouldBe("#FFDEAD");
	}

	[Fact]
	public void WebColor_FromHtml_NamedColor()
	{
		var color = WebColor.FromHtml("Red");
		color.ShouldNotBeNull();
		color.IsEmpty.ShouldBeFalse();
	}

	[Fact]
	public void FullFluentExample_MatchesSpecSignature()
	{
		// This test validates the exact API signature from the POC plan
		var theme = new ThemeConfiguration()
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#FFDEAD"))
				.Set(s => s.Font.Bold, true))
			.ForControl("Button", "goButton", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#006633"))
				.Set(s => s.Width, new Unit("120px")));

		var defaultSkin = theme.GetSkin("Button");
		defaultSkin.BackColor.ToHtml().ShouldBe("#FFDEAD");
		defaultSkin.Font.Bold.ShouldBeTrue();

		var goSkin = theme.GetSkin("Button", "goButton");
		goSkin.BackColor.ToHtml().ShouldBe("#006633");
		goSkin.Width.Value.Value.ShouldBe(120);
	}
}
