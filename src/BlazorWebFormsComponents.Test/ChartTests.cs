using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using BlazorWebFormsComponents;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

public class ChartTests : BunitContext
{
	public ChartTests()
	{
		JSInterop.Mode = JSRuntimeMode.Loose;
		Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
		Services.AddSingleton<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);
	}

	#region Component Rendering

	[Fact]
	public void Chart_RendersCanvasInsideDiv()
	{
		var cut = Render<Chart>(p => p
			.Add(c => c.ID, "myChart"));

		var div = cut.Find("div");
		div.ShouldNotBeNull();
		div.Id.ShouldBe("myChart");

		var canvas = cut.Find("canvas");
		canvas.ShouldNotBeNull();
	}

	[Fact]
	public void Chart_WidthHeight_AppliedAsStyle()
	{
		var cut = Render<Chart>(p => p
			.Add(c => c.ChartWidth, "400px")
			.Add(c => c.ChartHeight, "300px"));

		var div = cut.Find("div");
		var style = div.GetAttribute("style");
		style.ShouldContain("width:400px");
		style.ShouldContain("height:300px");
	}

	[Fact]
	public void Chart_WidthOnly_AppliedAsStyle()
	{
		var cut = Render<Chart>(p => p
			.Add(c => c.ChartWidth, "500px"));

		var div = cut.Find("div");
		var style = div.GetAttribute("style");
		style.ShouldContain("width:500px");
		style.ShouldNotContain("height");
	}

	[Fact]
	public void Chart_VisibleFalse_RendersNothing()
	{
		var cut = Render<Chart>(p => p
			.Add(c => c.Visible, false));

		cut.Markup.Trim().ShouldBeEmpty();
	}

	[Fact]
	public void Chart_CssClass_AppliedToDiv()
	{
		var cut = Render<Chart>(p => p
			.Add(c => c.CssClass, "my-chart-class"));

		var div = cut.Find("div");
		div.ClassList.ShouldContain("my-chart-class");
	}

	#endregion

	#region SeriesChartType Enum

	[Fact]
	public void SeriesChartType_Has35Values()
	{
		var values = Enum.GetValues<SeriesChartType>();
		values.Length.ShouldBe(35);
	}

	[Theory]
	[InlineData(SeriesChartType.Point, 0)]
	[InlineData(SeriesChartType.FastPoint, 1)]
	[InlineData(SeriesChartType.Bubble, 2)]
	[InlineData(SeriesChartType.Line, 3)]
	[InlineData(SeriesChartType.Spline, 4)]
	[InlineData(SeriesChartType.StepLine, 5)]
	[InlineData(SeriesChartType.FastLine, 6)]
	[InlineData(SeriesChartType.Bar, 7)]
	[InlineData(SeriesChartType.StackedBar, 8)]
	[InlineData(SeriesChartType.StackedBar100, 9)]
	[InlineData(SeriesChartType.Column, 10)]
	[InlineData(SeriesChartType.StackedColumn, 11)]
	[InlineData(SeriesChartType.StackedColumn100, 12)]
	[InlineData(SeriesChartType.Area, 13)]
	[InlineData(SeriesChartType.SplineArea, 14)]
	[InlineData(SeriesChartType.StackedArea, 15)]
	[InlineData(SeriesChartType.StackedArea100, 16)]
	[InlineData(SeriesChartType.Pie, 17)]
	[InlineData(SeriesChartType.Doughnut, 18)]
	[InlineData(SeriesChartType.Stock, 19)]
	[InlineData(SeriesChartType.Candlestick, 20)]
	[InlineData(SeriesChartType.Range, 21)]
	[InlineData(SeriesChartType.SplineRange, 22)]
	[InlineData(SeriesChartType.RangeBar, 23)]
	[InlineData(SeriesChartType.RangeColumn, 24)]
	[InlineData(SeriesChartType.Radar, 25)]
	[InlineData(SeriesChartType.Polar, 26)]
	[InlineData(SeriesChartType.ErrorBar, 27)]
	[InlineData(SeriesChartType.BoxPlot, 28)]
	[InlineData(SeriesChartType.Renko, 29)]
	[InlineData(SeriesChartType.ThreeLineBreak, 30)]
	[InlineData(SeriesChartType.Kagi, 31)]
	[InlineData(SeriesChartType.PointAndFigure, 32)]
	[InlineData(SeriesChartType.Funnel, 33)]
	[InlineData(SeriesChartType.Pyramid, 34)]
	public void SeriesChartType_MatchesWebFormsNumbering(SeriesChartType type, int expected)
	{
		((int)type).ShouldBe(expected);
	}

	#endregion

	#region ChartPalette Enum

	[Fact]
	public void ChartPalette_Has12Values()
	{
		var values = Enum.GetValues<ChartPalette>();
		values.Length.ShouldBe(12);
	}

	[Theory]
	[InlineData(ChartPalette.None, 0)]
	[InlineData(ChartPalette.BrightPastel, 1)]
	[InlineData(ChartPalette.Berry, 2)]
	[InlineData(ChartPalette.Chocolate, 3)]
	[InlineData(ChartPalette.EarthTones, 4)]
	[InlineData(ChartPalette.Excel, 5)]
	[InlineData(ChartPalette.Fire, 6)]
	[InlineData(ChartPalette.Grayscale, 7)]
	[InlineData(ChartPalette.Light, 8)]
	[InlineData(ChartPalette.Pastel, 9)]
	[InlineData(ChartPalette.SeaGreen, 10)]
	[InlineData(ChartPalette.SemiTransparent, 11)]
	public void ChartPalette_MatchesWebFormsNumbering(ChartPalette palette, int expected)
	{
		((int)palette).ShouldBe(expected);
	}

	#endregion

	#region Docking Enum

	[Theory]
	[InlineData(Docking.Top, 0)]
	[InlineData(Docking.Right, 1)]
	[InlineData(Docking.Bottom, 2)]
	[InlineData(Docking.Left, 3)]
	public void Docking_MatchesExpectedValues(Docking docking, int expected)
	{
		((int)docking).ShouldBe(expected);
	}

	[Fact]
	public void Docking_Has4Values()
	{
		Enum.GetValues<Docking>().Length.ShouldBe(4);
	}

	#endregion

	#region ChartDashStyle Enum

	[Fact]
	public void ChartDashStyle_Has6Values()
	{
		Enum.GetValues<ChartDashStyle>().Length.ShouldBe(6);
	}

	[Theory]
	[InlineData(ChartDashStyle.NotSet, 0)]
	[InlineData(ChartDashStyle.Dash, 1)]
	[InlineData(ChartDashStyle.DashDot, 2)]
	[InlineData(ChartDashStyle.DashDotDot, 3)]
	[InlineData(ChartDashStyle.Dot, 4)]
	[InlineData(ChartDashStyle.Solid, 5)]
	public void ChartDashStyle_MatchesExpectedValues(ChartDashStyle style, int expected)
	{
		((int)style).ShouldBe(expected);
	}

	#endregion

	#region DataPoint

	[Fact]
	public void DataPoint_DefaultValues()
	{
		var dp = new DataPoint();
		dp.XValue.ShouldBeNull();
		dp.YValues.ShouldNotBeNull();
		dp.YValues.Length.ShouldBe(0);
		dp.Label.ShouldBeNull();
		dp.Color.ShouldBeNull();
		dp.ToolTip.ShouldBeNull();
		dp.IsValueShownAsLabel.ShouldBeFalse();
	}

	[Fact]
	public void DataPoint_SetProperties()
	{
		var dp = new DataPoint
		{
			XValue = "Q1",
			YValues = new[] { 10.0, 20.0 },
			Label = "Quarter 1",
			ToolTip = "First quarter"
		};

		dp.XValue.ShouldBe("Q1");
		dp.YValues.Length.ShouldBe(2);
		dp.YValues[0].ShouldBe(10.0);
		dp.YValues[1].ShouldBe(20.0);
		dp.Label.ShouldBe("Quarter 1");
		dp.ToolTip.ShouldBe("First quarter");
	}

	[Fact]
	public void DataPoint_NumericXValue()
	{
		var dp = new DataPoint { XValue = 42 };
		dp.XValue.ShouldBe(42);
	}

	#endregion

	#region Axis

	[Fact]
	public void Axis_DefaultValues()
	{
		var axis = new Axis();
		axis.Title.ShouldBeNull();
		axis.Minimum.ShouldBeNull();
		axis.Maximum.ShouldBeNull();
		axis.Interval.ShouldBeNull();
		axis.IsLogarithmic.ShouldBeFalse();
	}

	[Fact]
	public void Axis_SetProperties()
	{
		var axis = new Axis
		{
			Title = "X Axis",
			Minimum = 0,
			Maximum = 100,
			Interval = 10,
			IsLogarithmic = true
		};

		axis.Title.ShouldBe("X Axis");
		axis.Minimum.ShouldBe(0);
		axis.Maximum.ShouldBe(100);
		axis.Interval.ShouldBe(10);
		axis.IsLogarithmic.ShouldBeTrue();
	}

	#endregion

	#region ChartConfigBuilder — Empty/Null Series

	[Fact]
	public void BuildConfig_NullSeries_ReturnsDefaultBarConfig()
	{
		var config = ChartConfigBuilder.BuildConfig(null, new List<ChartAreaConfig>(),
			new List<ChartTitleConfig>(), new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		config["type"].ShouldBe("bar");
		config.ShouldContainKey("data");
		config.ShouldContainKey("options");
	}

	[Fact]
	public void BuildConfig_EmptySeries_ReturnsDefaultBarConfig()
	{
		var config = ChartConfigBuilder.BuildConfig(new List<ChartSeriesConfig>(),
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		config["type"].ShouldBe("bar");
	}

	#endregion

	#region ChartConfigBuilder — Chart Type Mapping

	[Fact]
	public void BuildConfig_Column_MapsToBarType()
	{
		var config = BuildConfigWithType(SeriesChartType.Column);
		config["type"].ShouldBe("bar");
	}

	[Fact]
	public void BuildConfig_Bar_MapsToBarTypeWithIndexAxisY()
	{
		var config = BuildConfigWithType(SeriesChartType.Bar);
		config["type"].ShouldBe("bar");

		var options = (Dictionary<string, object>)config["options"];
		options["indexAxis"].ShouldBe("y");
	}

	[Fact]
	public void BuildConfig_Line_MapsToLineType()
	{
		var config = BuildConfigWithType(SeriesChartType.Line);
		config["type"].ShouldBe("line");
	}

	[Fact]
	public void BuildConfig_Pie_MapsToPieType()
	{
		var config = BuildConfigWithType(SeriesChartType.Pie);
		config["type"].ShouldBe("pie");
	}

	[Fact]
	public void BuildConfig_Area_MapsToLineTypeWithFill()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Area,
			Points = new List<DataPoint>
			{
				new() { XValue = "A", YValues = new[] { 1.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		config["type"].ShouldBe("line");

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];
		datasets[0]["fill"].ShouldBe(true);
	}

	[Fact]
	public void BuildConfig_Doughnut_MapsToDoughnutType()
	{
		var config = BuildConfigWithType(SeriesChartType.Doughnut);
		config["type"].ShouldBe("doughnut");
	}

	[Fact]
	public void BuildConfig_Point_MapsToScatterType()
	{
		var config = BuildConfigWithType(SeriesChartType.Point);
		config["type"].ShouldBe("scatter");
	}

	[Fact]
	public void BuildConfig_StackedColumn_MapsToBarWithStacked()
	{
		var config = BuildConfigWithType(SeriesChartType.StackedColumn);
		config["type"].ShouldBe("bar");

		var options = (Dictionary<string, object>)config["options"];
		var scales = (Dictionary<string, object>)options["scales"];
		var xScale = (Dictionary<string, object>)scales["x"];
		var yScale = (Dictionary<string, object>)scales["y"];

		xScale["stacked"].ShouldBe(true);
		yScale["stacked"].ShouldBe(true);
	}

	#endregion

	#region ChartConfigBuilder — Scatter data format

	[Fact]
	public void BuildConfig_Point_UsesXYDataFormat()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Point,
			Points = new List<DataPoint>
			{
				new() { XValue = 5, YValues = new[] { 10.0 } },
				new() { XValue = 15, YValues = new[] { 25.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];
		var points = (List<Dictionary<string, object>>)datasets[0]["data"];

		points.Count.ShouldBe(2);
		points[0]["x"].ShouldBe(5);
		points[0]["y"].ShouldBe(10.0);
		points[1]["x"].ShouldBe(15);
		points[1]["y"].ShouldBe(25.0);
	}

	#endregion

	#region ChartConfigBuilder — Unsupported chart types

	[Theory]
	[InlineData(SeriesChartType.FastPoint)]
	[InlineData(SeriesChartType.Bubble)]
	[InlineData(SeriesChartType.Spline)]
	[InlineData(SeriesChartType.StepLine)]
	[InlineData(SeriesChartType.FastLine)]
	[InlineData(SeriesChartType.StackedBar)]
	[InlineData(SeriesChartType.StackedBar100)]
	[InlineData(SeriesChartType.StackedColumn100)]
	[InlineData(SeriesChartType.SplineArea)]
	[InlineData(SeriesChartType.StackedArea)]
	[InlineData(SeriesChartType.StackedArea100)]
	[InlineData(SeriesChartType.Stock)]
	[InlineData(SeriesChartType.Candlestick)]
	[InlineData(SeriesChartType.Range)]
	[InlineData(SeriesChartType.SplineRange)]
	[InlineData(SeriesChartType.RangeBar)]
	[InlineData(SeriesChartType.RangeColumn)]
	[InlineData(SeriesChartType.Radar)]
	[InlineData(SeriesChartType.Polar)]
	[InlineData(SeriesChartType.ErrorBar)]
	[InlineData(SeriesChartType.BoxPlot)]
	[InlineData(SeriesChartType.Renko)]
	[InlineData(SeriesChartType.ThreeLineBreak)]
	[InlineData(SeriesChartType.Kagi)]
	[InlineData(SeriesChartType.PointAndFigure)]
	[InlineData(SeriesChartType.Funnel)]
	[InlineData(SeriesChartType.Pyramid)]
	public void BuildConfig_UnsupportedType_ThrowsNotSupportedException(SeriesChartType type)
	{
		var series = new ChartSeriesConfig { ChartType = type };

		Should.Throw<NotSupportedException>(() =>
			ChartConfigBuilder.BuildConfig(
				new List<ChartSeriesConfig> { series },
				new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
				new List<ChartLegendConfig>(), ChartPalette.BrightPastel));
	}

	#endregion

	#region ChartConfigBuilder — Title

	[Fact]
	public void BuildConfig_WithTitle_SetsPluginsTitleConfig()
	{
		var titles = new List<ChartTitleConfig>
		{
			new() { Text = "Sales Report", Docking = Docking.Top }
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), new List<ChartAreaConfig>(),
			titles, new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var plugins = (Dictionary<string, object>)options["plugins"];
		var title = (Dictionary<string, object>)plugins["title"];

		title["display"].ShouldBe(true);
		title["text"].ShouldBe("Sales Report");
		title["position"].ShouldBe("top");
	}

	[Fact]
	public void BuildConfig_WithTitle_BottomDocking()
	{
		var titles = new List<ChartTitleConfig>
		{
			new() { Text = "Bottom Title", Docking = Docking.Bottom }
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), new List<ChartAreaConfig>(),
			titles, new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var plugins = (Dictionary<string, object>)options["plugins"];
		var title = (Dictionary<string, object>)plugins["title"];
		title["position"].ShouldBe("bottom");
	}

	#endregion

	#region ChartConfigBuilder — Legend

	[Fact]
	public void BuildConfig_WithLegend_SetsPluginsLegendConfig()
	{
		var legends = new List<ChartLegendConfig>
		{
			new() { Docking = Docking.Right, Title = "Legend Title" }
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), new List<ChartAreaConfig>(),
			new List<ChartTitleConfig>(), legends, ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var plugins = (Dictionary<string, object>)options["plugins"];
		var legend = (Dictionary<string, object>)plugins["legend"];

		legend["display"].ShouldBe(true);
		legend["position"].ShouldBe("right");

		var legendTitle = (Dictionary<string, object>)legend["title"];
		legendTitle["display"].ShouldBe(true);
		legendTitle["text"].ShouldBe("Legend Title");
	}

	[Fact]
	public void BuildConfig_WithLegend_LeftDocking()
	{
		var legends = new List<ChartLegendConfig>
		{
			new() { Docking = Docking.Left }
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), new List<ChartAreaConfig>(),
			new List<ChartTitleConfig>(), legends, ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var plugins = (Dictionary<string, object>)options["plugins"];
		var legend = (Dictionary<string, object>)plugins["legend"];
		legend["position"].ShouldBe("left");
	}

	#endregion

	#region ChartConfigBuilder — Axis Configuration

	[Fact]
	public void BuildConfig_WithAxisTitle_SetsScalesConfig()
	{
		var areas = new List<ChartAreaConfig>
		{
			new()
			{
				AxisX = new Axis { Title = "Month" },
				AxisY = new Axis { Title = "Revenue" }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), areas,
			new List<ChartTitleConfig>(), new List<ChartLegendConfig>(),
			ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var scales = (Dictionary<string, object>)options["scales"];

		var xAxis = (Dictionary<string, object>)scales["x"];
		var xTitle = (Dictionary<string, object>)xAxis["title"];
		xTitle["text"].ShouldBe("Month");
		xTitle["display"].ShouldBe(true);

		var yAxis = (Dictionary<string, object>)scales["y"];
		var yTitle = (Dictionary<string, object>)yAxis["title"];
		yTitle["text"].ShouldBe("Revenue");
		yTitle["display"].ShouldBe(true);
	}

	[Fact]
	public void BuildConfig_WithAxisMinMax_SetsMinMaxOnScale()
	{
		var areas = new List<ChartAreaConfig>
		{
			new()
			{
				AxisY = new Axis { Minimum = 0, Maximum = 100 }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), areas,
			new List<ChartTitleConfig>(), new List<ChartLegendConfig>(),
			ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var scales = (Dictionary<string, object>)options["scales"];
		var yAxis = (Dictionary<string, object>)scales["y"];

		yAxis["min"].ShouldBe(0.0);
		yAxis["max"].ShouldBe(100.0);
	}

	[Fact]
	public void BuildConfig_WithAxisInterval_SetsStepSize()
	{
		var areas = new List<ChartAreaConfig>
		{
			new()
			{
				AxisX = new Axis { Interval = 5 }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), areas,
			new List<ChartTitleConfig>(), new List<ChartLegendConfig>(),
			ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var scales = (Dictionary<string, object>)options["scales"];
		var xAxis = (Dictionary<string, object>)scales["x"];
		var ticks = (Dictionary<string, object>)xAxis["ticks"];

		ticks["stepSize"].ShouldBe(5.0);
	}

	[Fact]
	public void BuildConfig_WithLogarithmicAxis_SetsLogType()
	{
		var areas = new List<ChartAreaConfig>
		{
			new()
			{
				AxisY = new Axis { IsLogarithmic = true }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(), areas,
			new List<ChartTitleConfig>(), new List<ChartLegendConfig>(),
			ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var scales = (Dictionary<string, object>)options["scales"];
		var yAxis = (Dictionary<string, object>)scales["y"];

		yAxis["type"].ShouldBe("logarithmic");
	}

	#endregion

	#region ChartConfigBuilder — Data Labels

	[Fact]
	public void BuildConfig_WithLabels_SetsDataLabels()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Column,
			Points = new List<DataPoint>
			{
				new() { Label = "Jan", YValues = new[] { 10.0 } },
				new() { Label = "Feb", YValues = new[] { 20.0 } },
				new() { Label = "Mar", YValues = new[] { 30.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var labels = (List<object>)data["labels"];

		labels.Count.ShouldBe(3);
		labels[0].ShouldBe("Jan");
		labels[1].ShouldBe("Feb");
		labels[2].ShouldBe("Mar");
	}

	[Fact]
	public void BuildConfig_WithXValueAsLabel_UsesXValue()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Column,
			Points = new List<DataPoint>
			{
				new() { XValue = "Q1", YValues = new[] { 100.0 } },
				new() { XValue = "Q2", YValues = new[] { 200.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var labels = (List<object>)data["labels"];

		labels[0].ShouldBe("Q1");
		labels[1].ShouldBe("Q2");
	}

	#endregion

	#region ChartConfigBuilder — Series Name

	[Fact]
	public void BuildConfig_WithSeriesName_SetsDatasetLabel()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Column,
			Name = "Revenue",
			Points = new List<DataPoint>
			{
				new() { YValues = new[] { 10.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];

		datasets[0]["label"].ShouldBe("Revenue");
	}

	#endregion

	#region ChartConfigBuilder — Responsive options

	[Fact]
	public void BuildConfig_AlwaysSetsResponsiveOptions()
	{
		var config = BuildConfigWithType(SeriesChartType.Column);

		var options = (Dictionary<string, object>)config["options"];
		options["responsive"].ShouldBe(true);
		options["maintainAspectRatio"].ShouldBe(false);
	}

	#endregion

	#region ChartConfigBuilder — Palette Colors

	[Fact]
	public void BuildConfig_BrightPastel_AssignsPaletteColorToDataset()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Column,
			Points = new List<DataPoint> { new() { YValues = new[] { 10.0 } } }
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];

		// BrightPastel palette should assign an rgba color
		datasets[0].ShouldContainKey("backgroundColor");
		((string)datasets[0]["backgroundColor"]).ShouldStartWith("rgba(");
	}

	[Fact]
	public void BuildConfig_NonePalette_NoColorAssigned()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Column,
			Points = new List<DataPoint> { new() { YValues = new[] { 10.0 } } }
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.None);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];

		// None palette should not assign colors from palette
		datasets[0].ShouldNotContainKey("backgroundColor");
	}

	[Theory]
	[InlineData(ChartPalette.Berry)]
	[InlineData(ChartPalette.Chocolate)]
	[InlineData(ChartPalette.EarthTones)]
	[InlineData(ChartPalette.Excel)]
	[InlineData(ChartPalette.Fire)]
	[InlineData(ChartPalette.Grayscale)]
	[InlineData(ChartPalette.Light)]
	[InlineData(ChartPalette.Pastel)]
	[InlineData(ChartPalette.SeaGreen)]
	[InlineData(ChartPalette.SemiTransparent)]
	public void BuildConfig_AllPalettes_AssignColors(ChartPalette palette)
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Column,
			Points = new List<DataPoint> { new() { YValues = new[] { 10.0 } } }
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), palette);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];

		datasets[0].ShouldContainKey("backgroundColor");
		((string)datasets[0]["backgroundColor"]).ShouldStartWith("rgba(");
	}

	#endregion

	#region ChartConfigBuilder — BorderWidth

	[Fact]
	public void BuildConfig_WithBorderWidth_SetsBorderWidthOnDataset()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Line,
			BorderWidth = 3,
			Points = new List<DataPoint>
			{
				new() { YValues = new[] { 10.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];
		datasets[0]["borderWidth"].ShouldBe(3);
	}

	#endregion

	#region ChartConfigBuilder — Bar indexAxis on dataset

	[Fact]
	public void BuildConfig_Bar_SetsIndexAxisOnDataset()
	{
		var series = new ChartSeriesConfig
		{
			ChartType = SeriesChartType.Bar,
			Points = new List<DataPoint>
			{
				new() { YValues = new[] { 10.0 } }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig> { series },
			new List<ChartAreaConfig>(), new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var data = (Dictionary<string, object>)config["data"];
		var datasets = (List<Dictionary<string, object>>)data["datasets"];
		datasets[0]["indexAxis"].ShouldBe("y");
	}

	#endregion

	#region ChartConfigBuilder — Stacked with axis config merging

	[Fact]
	public void BuildConfig_StackedColumn_WithAxis_MergesScales()
	{
		var areas = new List<ChartAreaConfig>
		{
			new()
			{
				AxisX = new Axis { Title = "Category" }
			}
		};

		var config = ChartConfigBuilder.BuildConfig(
			new List<ChartSeriesConfig>
			{
				new() { ChartType = SeriesChartType.StackedColumn }
			},
			areas, new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(), ChartPalette.BrightPastel);

		var options = (Dictionary<string, object>)config["options"];
		var scales = (Dictionary<string, object>)options["scales"];
		var xScale = (Dictionary<string, object>)scales["x"];

		// Should have both stacked and title merged
		xScale["stacked"].ShouldBe(true);
		var title = (Dictionary<string, object>)xScale["title"];
		title["text"].ShouldBe("Category");
	}

	#endregion

	#region Config Snapshot Classes

	[Fact]
	public void ChartSeriesConfig_DefaultValues()
	{
		var cfg = new ChartSeriesConfig();
		cfg.Name.ShouldBeNull();
		cfg.ChartType.ShouldBe(default);
		cfg.Points.ShouldBeNull();
		cfg.Color.ShouldBeNull();
		cfg.BorderWidth.ShouldBe(0);
		cfg.IsVisibleInLegend.ShouldBeTrue();
		cfg.ChartArea.ShouldBeNull();
	}

	[Fact]
	public void ChartAreaConfig_Properties()
	{
		var axis = new Axis { Title = "X" };
		var cfg = new ChartAreaConfig
		{
			Name = "Default",
			AxisX = axis
		};

		cfg.Name.ShouldBe("Default");
		cfg.AxisX.ShouldBeSameAs(axis);
	}

	[Fact]
	public void ChartTitleConfig_Properties()
	{
		var cfg = new ChartTitleConfig
		{
			Text = "My Title",
			Docking = Docking.Bottom
		};

		cfg.Text.ShouldBe("My Title");
		cfg.Docking.ShouldBe(Docking.Bottom);
	}

	[Fact]
	public void ChartLegendConfig_Properties()
	{
		var cfg = new ChartLegendConfig
		{
			Name = "Legend1",
			Docking = Docking.Left,
			Title = "Categories"
		};

		cfg.Name.ShouldBe("Legend1");
		cfg.Docking.ShouldBe(Docking.Left);
		cfg.Title.ShouldBe("Categories");
	}

	#endregion

	#region ChartSeries — Data Binding (via ChartSeriesDataBindingHelper)

	// These tests verify the expected data binding behavior that ChartSeries.ToConfig()
	// should implement. The helper simulates extracting values from Items using
	// XValueMember and YValueMembers properties.

	[Fact]
	public void DataBinding_ExtractsValuesFromItems()
	{
		// Arrange: Items with XValueMember and YValueMembers
		var items = new object[]
		{
			new { Month = "Jan", Sales = 100 },
			new { Month = "Feb", Sales = 200 },
			new { Month = "Mar", Sales = 300 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Month", yValueMembers: "Sales");

		// Assert: Data binding extracts X and Y values
		points.ShouldNotBeNull();
		points.Count.ShouldBe(3);

		points[0].XValue.ShouldBe("Jan");
		points[0].YValues[0].ShouldBe(100.0);

		points[1].XValue.ShouldBe("Feb");
		points[1].YValues[0].ShouldBe(200.0);

		points[2].XValue.ShouldBe("Mar");
		points[2].YValues[0].ShouldBe(300.0);
	}

	[Fact]
	public void DataBinding_NumericXValues()
	{
		// Arrange: Items with numeric XValue
		var items = new object[]
		{
			new { Year = 2020, Revenue = 1000 },
			new { Year = 2021, Revenue = 1500 },
			new { Year = 2022, Revenue = 2000 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Year", yValueMembers: "Revenue");

		// Assert
		points.Count.ShouldBe(3);
		points[0].XValue.ShouldBe(2020);
		points[0].YValues[0].ShouldBe(1000.0);
	}

	[Fact]
	public void DataBinding_DecimalYValues()
	{
		// Arrange: Items with decimal Y values
		var items = new object[]
		{
			new { Category = "A", Percentage = 25.5 },
			new { Category = "B", Percentage = 33.3 },
			new { Category = "C", Percentage = 41.2 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Category", yValueMembers: "Percentage");

		// Assert
		points.Count.ShouldBe(3);
		points[0].YValues[0].ShouldBe(25.5);
		points[1].YValues[0].ShouldBe(33.3);
		points[2].YValues[0].ShouldBe(41.2);
	}

	[Fact]
	public void DataBinding_ManualPoints_WorksWithoutItems()
	{
		// Arrange: Manual Points without Items (fallback case)
		var manualPoints = new List<DataPoint>
		{
			new() { XValue = "Q1", YValues = new[] { 100.0 } },
			new() { XValue = "Q2", YValues = new[] { 150.0 } },
			new() { XValue = "Q3", YValues = new[] { 200.0 } }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items: null, xValueMember: null, yValueMembers: null, fallbackPoints: manualPoints);

		// Assert: Manual points are used directly
		points.ShouldNotBeNull();
		points.Count.ShouldBe(3);
		points[0].XValue.ShouldBe("Q1");
		points[0].YValues[0].ShouldBe(100.0);
	}

	[Fact]
	public void DataBinding_EmptyItems_ProducesEmptyPoints()
	{
		// Arrange: Empty Items collection
		var items = Array.Empty<object>();

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Month", yValueMembers: "Sales");

		// Assert: Should produce empty points, not an error
		points.ShouldNotBeNull();
		points.Count.ShouldBe(0);
	}

	[Fact]
	public void DataBinding_NullItems_FallsBackToPoints()
	{
		// Arrange: Null Items with manual Points fallback
		var fallbackPoints = new List<DataPoint>
		{
			new() { XValue = "Fallback", YValues = new[] { 999.0 } }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items: null, xValueMember: "Month", yValueMembers: "Sales",
			fallbackPoints: fallbackPoints);

		// Assert: Falls back to manual Points
		points.ShouldNotBeNull();
		points.Count.ShouldBe(1);
		points[0].XValue.ShouldBe("Fallback");
		points[0].YValues[0].ShouldBe(999.0);
	}

	[Fact]
	public void DataBinding_NullItems_NoFallback_ProducesEmptyPoints()
	{
		// Arrange: Null Items with no manual Points
		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items: null, xValueMember: null, yValueMembers: null, fallbackPoints: null);

		// Assert: Should produce empty points (default)
		points.ShouldNotBeNull();
		points.Count.ShouldBe(0);
	}

	[Fact]
	public void DataBinding_MissingXValueMember_UsesNullXValue()
	{
		// Arrange: Items but no XValueMember
		var items = new object[]
		{
			new { Month = "Jan", Sales = 100 },
			new { Month = "Feb", Sales = 200 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: null, yValueMembers: "Sales");

		// Assert: XValues are null since XValueMember not specified
		points.Count.ShouldBe(2);
		points[0].XValue.ShouldBeNull();
		points[0].YValues[0].ShouldBe(100.0);
	}

	[Fact]
	public void DataBinding_MissingYValueMembers_UsesEmptyYValues()
	{
		// Arrange: Items but no YValueMembers
		var items = new object[]
		{
			new { Month = "Jan", Sales = 100 },
			new { Month = "Feb", Sales = 200 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Month", yValueMembers: null);

		// Assert: Points created with XValue but empty Y
		points.Count.ShouldBe(2);
		points[0].XValue.ShouldBe("Jan");
		points[0].YValues.Length.ShouldBe(0);
	}

	[Fact]
	public void DataBinding_IntYValue_ConvertsToDouble()
	{
		// Arrange: Items with integer Y values
		var items = new object[]
		{
			new { Name = "A", Count = 42 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Name", yValueMembers: "Count");

		// Assert: Int should be converted to double
		points[0].YValues[0].ShouldBeOfType<double>();
		points[0].YValues[0].ShouldBe(42.0);
	}

	[Fact]
	public void DataBinding_ItemsOverrideManualPoints()
	{
		// Arrange: Both Items and fallback Points provided — Items should win
		var items = new object[]
		{
			new { Month = "Jan", Sales = 100 }
		};
		var fallbackPoints = new List<DataPoint>
		{
			new() { XValue = "OldPoint", YValues = new[] { 999.0 } }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "Month", yValueMembers: "Sales", fallbackPoints: fallbackPoints);

		// Assert: Items takes precedence over manual Points
		points.Count.ShouldBe(1);
		points[0].XValue.ShouldBe("Jan");
		points[0].YValues[0].ShouldBe(100.0);
	}

	[Fact]
	public void DataBinding_InvalidPropertyName_ReturnsNullValue()
	{
		// Arrange: Invalid property name
		var items = new object[]
		{
			new { Month = "Jan", Sales = 100 }
		};

		// Act
		var points = ChartSeriesDataBindingHelper.ExtractDataPoints(
			items, xValueMember: "NonExistentProperty", yValueMembers: "AlsoNotReal");

		// Assert: Should handle gracefully — points created with null values
		points.Count.ShouldBe(1);
		points[0].XValue.ShouldBeNull();
		points[0].YValues.Length.ShouldBe(0);
	}

	#endregion

	#region Helpers

	private static List<ChartSeriesConfig> CreateSimpleSeries(
		SeriesChartType type = SeriesChartType.Column)
	{
		return new List<ChartSeriesConfig>
		{
			new()
			{
				ChartType = type,
				Points = new List<DataPoint>
				{
					new() { YValues = new[] { 10.0 } }
				}
			}
		};
	}

	private static Dictionary<string, object> BuildConfigWithType(SeriesChartType type)
	{
		return ChartConfigBuilder.BuildConfig(
			CreateSimpleSeries(type),
			new List<ChartAreaConfig>(),
			new List<ChartTitleConfig>(),
			new List<ChartLegendConfig>(),
			ChartPalette.BrightPastel);
	}

	#endregion
}

/// <summary>
/// Helper class that implements the data binding logic for ChartSeries.
/// This is the expected implementation that ChartSeries.ToConfig() should use
/// when extracting data points from Items using XValueMember and YValueMembers.
/// </summary>
internal static class ChartSeriesDataBindingHelper
{
	/// <summary>
	/// Extracts DataPoints from a collection of items using reflection-based property access.
	/// </summary>
	/// <param name="items">The data items to extract from (can be null)</param>
	/// <param name="xValueMember">Property name for X values (can be null)</param>
	/// <param name="yValueMembers">Property name for Y values (can be null)</param>
	/// <param name="fallbackPoints">Points to use if items is null or empty (optional)</param>
	/// <returns>List of DataPoint objects</returns>
	public static List<DataPoint> ExtractDataPoints(
		IEnumerable<object> items,
		string xValueMember,
		string yValueMembers,
		List<DataPoint> fallbackPoints = null)
	{
		// If items is null, fall back to manual points
		if (items == null)
		{
			return fallbackPoints ?? new List<DataPoint>();
		}

		var itemsList = items.ToList();

		// If items is empty, return empty list
		if (itemsList.Count == 0)
		{
			return new List<DataPoint>();
		}

		// Extract data points from items using reflection
		var points = new List<DataPoint>();
		foreach (var item in itemsList)
		{
			var point = new DataPoint();

			// Extract X value if XValueMember is specified
			if (!string.IsNullOrEmpty(xValueMember))
			{
				var xProp = item.GetType().GetProperty(xValueMember);
				point.XValue = xProp?.GetValue(item);
			}

			// Extract Y value if YValueMembers is specified
			if (!string.IsNullOrEmpty(yValueMembers))
			{
				var yProp = item.GetType().GetProperty(yValueMembers);
				var yValue = yProp?.GetValue(item);
				if (yValue != null)
				{
					point.YValues = new[] { Convert.ToDouble(yValue) };
				}
				else
				{
					point.YValues = Array.Empty<double>();
				}
			}
			else
			{
				point.YValues = Array.Empty<double>();
			}

			points.Add(point);
		}

		return points;
	}
}
