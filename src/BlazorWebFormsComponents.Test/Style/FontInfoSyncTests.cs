using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Style;

/// <summary>
/// Tests for FontInfo Name/Names auto-sync behavior.
/// In ASP.NET Web Forms, setting Font.Name also sets Font.Names and vice versa.
/// These tests validate that contract after the auto-sync fix.
/// </summary>
public class FontInfoSyncTests
{
	// 1. Setting Name also updates Names
	[Fact]
	public void SettingName_UpdatesNames()
	{
		var font = new FontInfo();
		font.Name = "Arial";

		font.Names.ShouldBe("Arial");
	}

	// 2. Setting Names also updates Name (first font)
	[Fact]
	public void SettingNames_UpdatesName_ToFirstFont()
	{
		var font = new FontInfo();
		font.Names = "Verdana";

		font.Name.ShouldBe("Verdana");
	}

	// 3. Setting Names with multiple fonts sets Name to first font
	[Fact]
	public void SettingNames_WithMultipleFonts_SetsNameToFirst()
	{
		var font = new FontInfo();
		font.Names = "Arial, sans-serif";

		font.Name.ShouldBe("Arial");
	}

	// 4. Setting Name to null clears Names
	[Fact]
	public void SettingName_ToNull_ClearsNames()
	{
		var font = new FontInfo { Name = "Arial" };

		font.Name = null;

		font.Names.ShouldBeNullOrEmpty();
	}

	// 5. Setting Name to empty clears Names
	[Fact]
	public void SettingName_ToEmpty_ClearsNames()
	{
		var font = new FontInfo { Name = "Arial" };

		font.Name = "";

		font.Names.ShouldBeNullOrEmpty();
	}

	// 6. Setting Names to null clears Name
	[Fact]
	public void SettingNames_ToNull_ClearsName()
	{
		var font = new FontInfo { Names = "Arial" };

		font.Names = null;

		font.Name.ShouldBeNullOrEmpty();
	}

	// 7. Setting Names to empty clears Name
	[Fact]
	public void SettingNames_ToEmpty_ClearsName()
	{
		var font = new FontInfo { Names = "Arial" };

		font.Names = "";

		font.Name.ShouldBeNullOrEmpty();
	}

	// 8. Setting Names then Name — Name wins for both properties
	[Fact]
	public void SettingNames_ThenName_NameWins()
	{
		var font = new FontInfo();
		font.Names = "Verdana, sans-serif";
		font.Name = "Arial";

		font.Name.ShouldBe("Arial");
		font.Names.ShouldBe("Arial");
	}

	// 9. Setting Name then Names — Names wins for both properties
	[Fact]
	public void SettingName_ThenNames_NamesWins()
	{
		var font = new FontInfo();
		font.Name = "Arial";
		font.Names = "Verdana, sans-serif";

		font.Name.ShouldBe("Verdana");
		font.Names.ShouldBe("Verdana, sans-serif");
	}
}
