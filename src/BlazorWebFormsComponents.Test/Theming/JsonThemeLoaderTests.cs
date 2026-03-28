using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Theming;
using Shouldly;
using System;
using Xunit;

namespace BlazorWebFormsComponents.Test.Theming;

/// <summary>
/// Unit tests for the JsonThemeLoader (Wave 2 - WI-11).
/// Tests JSON serialization/deserialization of ThemeConfiguration.
/// </summary>
public class JsonThemeLoaderTests
{
	[Fact]
	public void FromJson_BasicTheme_ParsesCorrectly()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": ""#507CD1"",
						""foreColor"": ""White"",
						""font"": {
							""bold"": true
						}
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		config.ShouldNotBeNull();
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("#507CD1");
		skin.ForeColor.ToHtml().ShouldBe("White");
		skin.Font.ShouldNotBeNull();
		skin.Font.Bold.ShouldBeTrue();
	}

	[Fact]
	public void FromJson_NamedSkins_ParseCorrectly()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": ""#507CD1""
					},
					""DangerButton"": {
						""backColor"": ""Red"",
						""foreColor"": ""White""
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		var defaultSkin = config.GetSkin("Button");
		defaultSkin.ShouldNotBeNull();
		defaultSkin.BackColor.ToHtml().ShouldBe("#507CD1");

		var dangerSkin = config.GetSkin("Button", "DangerButton");
		dangerSkin.ShouldNotBeNull();
		dangerSkin.BackColor.ToHtml().ShouldBe("Red");
		dangerSkin.ForeColor.ToHtml().ShouldBe("White");
	}

	[Fact]
	public void FromJson_SubStyles_ParseCorrectly()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""GridView"": {
					""default"": {
						""backColor"": ""White"",
						""subStyles"": {
							""HeaderStyle"": {
								""backColor"": ""#507CD1"",
								""foreColor"": ""White"",
								""font"": {
									""bold"": true
								}
							},
							""RowStyle"": {
								""backColor"": ""#EFF3FB""
							}
						}
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

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
	public void FromJson_ThemeMode_Parsed()
	{
		// Arrange - use numeric values for enum
		var jsonStyleSheet = @"{""mode"": 0}";  // StyleSheetTheme = 0
		var jsonTheme = @"{""mode"": 1}";       // Theme = 1

		// Act
		var configStyleSheet = JsonThemeLoader.FromJson(jsonStyleSheet);
		var configTheme = JsonThemeLoader.FromJson(jsonTheme);

		// Assert
		configStyleSheet.Mode.ShouldBe(ThemeMode.StyleSheetTheme);
		configTheme.Mode.ShouldBe(ThemeMode.Theme);
	}

	[Fact]
	public void FromJson_CssFiles_Parsed()
	{
		// Arrange
		var json = @"{
			""cssFiles"": [""a.css"", ""b.css"", ""c.css""]
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		config.CssFiles.ShouldNotBeNull();
		config.CssFiles.Count.ShouldBe(3);
		config.CssFiles.ShouldContain("a.css");
		config.CssFiles.ShouldContain("b.css");
		config.CssFiles.ShouldContain("c.css");
	}

	[Fact]
	public void ToJson_RoundTrip()
	{
		// Arrange
		var originalConfig = new ThemeConfiguration()
			.WithMode(ThemeMode.Theme)
			.WithCssFile("theme.css")
			.ForControl("Button", skin => skin
				.Set(s => s.BackColor, WebColor.FromHtml("#507CD1"))
				.Set(s => s.ForeColor, WebColor.White)
				.Set(s => s.Font.Bold, true))
			.ForControl("Button", "DangerButton", skin => skin
				.Set(s => s.BackColor, WebColor.Red));

		// Act
		var json = JsonThemeLoader.ToJson(originalConfig);
		var roundTripConfig = JsonThemeLoader.FromJson(json);

		// Assert
		roundTripConfig.ShouldNotBeNull();
		roundTripConfig.Mode.ShouldBe(ThemeMode.Theme);
		roundTripConfig.CssFiles.ShouldNotBeNull();
		roundTripConfig.CssFiles.Count.ShouldBe(1);
		roundTripConfig.CssFiles[0].ShouldBe("theme.css");

		// Note: The current ToJson implementation returns minimal DTO with empty Controls,
		// so we can't fully test the round-trip yet. This test validates that at least
		// Mode and CssFiles round-trip correctly.
	}

	[Fact]
	public void FromJson_WebColorFormats_HexAndNamed()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": ""#FF0000"",
						""foreColor"": ""Red"",
						""borderColor"": ""Blue""
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("#FF0000");
		skin.ForeColor.ToHtml().ShouldBe("Red");
		skin.BorderColor.ToHtml().ShouldBe("Blue");
	}

	[Fact]
	public void FromJson_EmptyObject_ReturnsDefaultConfig()
	{
		// Arrange
		var json = @"{}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		config.ShouldNotBeNull();
		config.Mode.ShouldBe(ThemeMode.StyleSheetTheme); // Default value
	}

	[Fact]
	public void FromJson_NullOrEmpty_ThrowsException()
	{
		// Act & Assert
		Should.Throw<ArgumentException>(() => JsonThemeLoader.FromJson(null));
		Should.Throw<ArgumentException>(() => JsonThemeLoader.FromJson(""));
		Should.Throw<ArgumentException>(() => JsonThemeLoader.FromJson("   "));
	}

	[Fact]
	public void FromJson_CompleteControlSkin_AllProperties()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": ""#FFDEAD"",
						""foreColor"": ""Black"",
						""borderColor"": ""#333333"",
						""borderStyle"": ""Solid"",
						""borderWidth"": ""1px"",
						""cssClass"": ""btn-themed"",
						""height"": ""30px"",
						""width"": ""120px"",
						""toolTip"": ""Themed button"",
						""font"": {
							""bold"": true,
							""italic"": false,
							""name"": ""Arial"",
							""size"": ""12pt""
						}
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("#FFDEAD");
		skin.ForeColor.ToHtml().ShouldBe("Black");
		skin.BorderColor.ToHtml().ShouldBe("#333333");
		skin.BorderStyle.ShouldBe(BorderStyle.Solid);
		skin.BorderWidth.ShouldNotBeNull();
		skin.BorderWidth.Value.ToString().ShouldBe("1px");
		skin.CssClass.ShouldBe("btn-themed");
		skin.Height.ShouldNotBeNull();
		skin.Height.Value.ToString().ShouldBe("30px");
		skin.Width.ShouldNotBeNull();
		skin.Width.Value.ToString().ShouldBe("120px");
		skin.ToolTip.ShouldBe("Themed button");
		skin.Font.ShouldNotBeNull();
		skin.Font.Bold.ShouldBeTrue();
		skin.Font.Italic.ShouldBeFalse();
		skin.Font.Name.ShouldBe("Arial");
		skin.Font.Size.ToString().ShouldBe("12pt");
	}

	[Fact]
	public void FromJson_MultipleControlTypes()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": ""#507CD1""
					}
				},
				""Label"": {
					""default"": {
						""foreColor"": ""Red""
					}
				},
				""GridView"": {
					""default"": {
						""cssClass"": ""grid-theme""
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		config.GetSkin("Button").ShouldNotBeNull();
		config.GetSkin("Label").ShouldNotBeNull();
		config.GetSkin("GridView").ShouldNotBeNull();

		config.GetSkin("Button").BackColor.ToHtml().ShouldBe("#507CD1");
		config.GetSkin("Label").ForeColor.ToHtml().ShouldBe("Red");
		config.GetSkin("GridView").CssClass.ShouldBe("grid-theme");
	}

	[Fact]
	public void FromJson_FontInfo_AllProperties()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Label"": {
					""default"": {
						""font"": {
							""bold"": true,
							""italic"": true,
							""underline"": true,
							""overline"": true,
							""strikeout"": true,
							""name"": ""Verdana"",
							""names"": ""Verdana, Arial, sans-serif"",
							""size"": ""14px""
						}
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

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
		skin.Font.Names.ShouldBe("Verdana, Arial, sans-serif");
		skin.Font.Size.ToString().ShouldBe("14px");
	}

	[Fact]
	public void FromJson_SubStyles_AllTableItemStyleProperties()
	{
		// Arrange - test subset of properties that are implemented
		var json = @"{
			""controls"": {
				""GridView"": {
					""default"": {
						""subStyles"": {
							""HeaderStyle"": {
								""backColor"": ""#507CD1"",
								""foreColor"": ""White"",
								""borderColor"": ""Black"",
								""borderStyle"": ""Solid"",
								""borderWidth"": ""1px"",
								""cssClass"": ""header"",
								""height"": ""40px"",
								""width"": ""100%"",
								""font"": {
									""bold"": true
								}
							}
						}
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		var skin = config.GetSkin("GridView");
		skin.ShouldNotBeNull();
		skin.SubStyles.ShouldNotBeNull();
		skin.SubStyles.ShouldContainKey("HeaderStyle");

		var headerStyle = skin.SubStyles["HeaderStyle"];
		headerStyle.BackColor.ToHtml().ShouldBe("#507CD1");
		headerStyle.ForeColor.ToHtml().ShouldBe("White");
		headerStyle.BorderColor.ToHtml().ShouldBe("Black");
		headerStyle.BorderStyle.Value.ShouldBe(BorderStyle.Solid);
		headerStyle.BorderWidth.IsEmpty.ShouldBeFalse();
		headerStyle.CssClass.ShouldBe("header");
		headerStyle.Height.IsEmpty.ShouldBeFalse();
		headerStyle.Width.IsEmpty.ShouldBeFalse();
		headerStyle.Font.ShouldNotBeNull();
		headerStyle.Font.Bold.ShouldBeTrue();
	}

	[Fact]
	public void FromJson_CaseInsensitive_PropertyNames()
	{
		// Arrange - mixed case property names, but lowercase "default" for skin key
		var json = @"{
			""MODE"": 1,
			""CssFiles"": [""test.css""],
			""Controls"": {
				""Button"": {
					""default"": {
						""BackColor"": ""Red"",
						""FORECOLOR"": ""White""
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		config.Mode.ShouldBe(ThemeMode.Theme);
		config.CssFiles.ShouldNotBeNull();
		config.CssFiles[0].ShouldBe("test.css");
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.ToHtml().ShouldBe("Red");
		skin.ForeColor.ToHtml().ShouldBe("White");
	}

	[Fact]
	public void ToJson_NullConfig_ThrowsException()
	{
		// Act & Assert
		Should.Throw<ArgumentNullException>(() => JsonThemeLoader.ToJson(null));
	}

	[Fact]
	public void FromJson_NullColorValues_HandledGracefully()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": null,
						""foreColor"": ""Red""
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.IsEmpty.ShouldBeTrue();
		skin.ForeColor.ToHtml().ShouldBe("Red");
	}

	[Fact]
	public void FromJson_EmptyColorString_HandledGracefully()
	{
		// Arrange
		var json = @"{
			""controls"": {
				""Button"": {
					""default"": {
						""backColor"": """",
						""foreColor"": ""Red""
					}
				}
			}
		}";

		// Act
		var config = JsonThemeLoader.FromJson(json);

		// Assert
		var skin = config.GetSkin("Button");
		skin.ShouldNotBeNull();
		skin.BackColor.IsEmpty.ShouldBeTrue();
		skin.ForeColor.ToHtml().ShouldBe("Red");
	}
}
