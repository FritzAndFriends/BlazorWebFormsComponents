using BlazorWebFormsComponents.Enums;
using Shouldly;
using System;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Tests for OPP-2: Unit implicit string conversion operator.
/// Verifies that string values like "125px", "50%", "10em" can be
/// implicitly assigned to Unit-typed properties without Unit.Parse().
/// </summary>
public class UnitImplicitConversionTests
{
	[Fact]
	public void PixelString_ConvertsImplicitly()
	{
		Unit unit = "125px";

		unit.Value.ShouldBe(125);
		unit.Type.ShouldBe(UnitType.Pixel);
	}

	[Fact]
	public void PercentageString_ConvertsImplicitly()
	{
		Unit unit = "50%";

		unit.Value.ShouldBe(50);
		unit.Type.ShouldBe(UnitType.Percentage);
	}

	[Fact]
	public void EmString_ConvertsImplicitly()
	{
		Unit unit = "10em";

		unit.Value.ShouldBe(10);
		unit.Type.ShouldBe(UnitType.Em);
	}

	[Fact]
	public void PlainIntegerString_DefaultsToPixel()
	{
		Unit unit = "100";

		unit.Value.ShouldBe(100);
		unit.Type.ShouldBe(UnitType.Pixel);
	}

	[Fact]
	public void EmptyString_ReturnsEmptyUnit()
	{
		Unit unit = "";

		unit.IsEmpty.ShouldBeTrue();
	}

	[Fact]
	public void NullString_ReturnsEmptyUnit()
	{
		string s = null;
		Unit unit = s;

		unit.IsEmpty.ShouldBeTrue();
	}

	[Fact]
	public void InvalidString_ThrowsFormatException()
	{
		Should.Throw<FormatException>(() =>
		{
			Unit unit = "abc";
		});
	}

	[Fact]
	public void PixelRoundtrip_PreservesStringValue()
	{
		Unit unit = "125px";

		unit.ToString().ShouldBe("125px");
	}

	[Fact]
	public void PercentageRoundtrip_PreservesStringValue()
	{
		Unit unit = "50%";

		unit.ToString().ShouldBe("50%");
	}

	[Theory]
	[InlineData("10pt", 10, UnitType.Point)]
	[InlineData("5pc", 5, UnitType.Pica)]
	[InlineData("2in", 2, UnitType.Inch)]
	[InlineData("15mm", 15, UnitType.Mm)]
	[InlineData("3cm", 3, UnitType.Cm)]
	[InlineData("1ex", 1, UnitType.Ex)]
	public void VariousUnitTypes_ConvertCorrectly(string input, double expectedValue, UnitType expectedType)
	{
		Unit unit = input;

		unit.Value.ShouldBe(expectedValue);
		unit.Type.ShouldBe(expectedType);
	}

	[Fact]
	public void ImplicitConversion_MatchesExplicitParse()
	{
		Unit implicitResult = "125px";
		var explicitResult = Unit.Parse("125px");

		implicitResult.ShouldBe(explicitResult);
	}

	[Fact]
	public void ImplicitConversion_EqualsPixelFactory()
	{
		Unit implicitResult = "125px";

		implicitResult.ShouldBe(Unit.Pixel(125));
	}
}
