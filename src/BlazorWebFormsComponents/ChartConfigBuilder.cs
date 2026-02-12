using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents;

/// <summary>
/// Pure static class that converts the Chart component model into a Chart.js JSON configuration object.
/// Designed for testability without requiring a canvas/browser context.
/// </summary>
public static class ChartConfigBuilder
{
	// Phase 1 supported chart types (8 of 35)
	private static readonly HashSet<SeriesChartType> SupportedTypes = new()
	{
		SeriesChartType.Column,
		SeriesChartType.Bar,
		SeriesChartType.Line,
		SeriesChartType.Pie,
		SeriesChartType.Area,
		SeriesChartType.Doughnut,
		SeriesChartType.Point,       // Maps to Chart.js "scatter"
		SeriesChartType.StackedColumn
	};

	/// <summary>
	/// Builds a Chart.js configuration dictionary from the Chart component state.
	/// </summary>
	public static Dictionary<string, object> BuildConfig(
		IReadOnlyList<ChartSeriesConfig> series,
		IReadOnlyList<ChartAreaConfig> chartAreas,
		IReadOnlyList<ChartTitleConfig> titles,
		IReadOnlyList<ChartLegendConfig> legends,
		ChartPalette palette)
	{
		if (series == null || series.Count == 0)
		{
			return new Dictionary<string, object>
			{
				["type"] = "bar",
				["data"] = new Dictionary<string, object>(),
				["options"] = new Dictionary<string, object>()
			};
		}

		var primaryType = series[0].ChartType;
		ValidateChartType(primaryType);

		var chartJsType = MapChartType(primaryType);
		var config = new Dictionary<string, object>
		{
			["type"] = chartJsType
		};

		// Build data section
		config["data"] = BuildDataSection(series, palette);

		// Build options section
		config["options"] = BuildOptionsSection(series, chartAreas, titles, legends, primaryType);

		return config;
	}

	private static string MapChartType(SeriesChartType chartType)
	{
		return chartType switch
		{
			SeriesChartType.Column => "bar",
			SeriesChartType.Bar => "bar",
			SeriesChartType.Line => "line",
			SeriesChartType.Area => "line",
			SeriesChartType.Pie => "pie",
			SeriesChartType.Doughnut => "doughnut",
			SeriesChartType.Point => "scatter",
			SeriesChartType.StackedColumn => "bar",
			_ => throw new NotSupportedException($"SeriesChartType '{chartType}' is not supported in Phase 1.")
		};
	}

	private static void ValidateChartType(SeriesChartType chartType)
	{
		if (!SupportedTypes.Contains(chartType))
		{
			throw new NotSupportedException(
				$"SeriesChartType '{chartType}' is not supported in Phase 1. " +
				$"Supported types: {string.Join(", ", SupportedTypes)}.");
		}
	}

	private static Dictionary<string, object> BuildDataSection(
		IReadOnlyList<ChartSeriesConfig> seriesList,
		ChartPalette palette)
	{
		var data = new Dictionary<string, object>();
		var paletteColors = GetPaletteColors(palette);

		// Collect labels from the first series that has them
		var labels = new List<object>();
		foreach (var s in seriesList)
		{
			if (s.Points != null && s.Points.Count > 0)
			{
				foreach (var p in s.Points)
				{
					if (!string.IsNullOrEmpty(p.Label))
						labels.Add(p.Label);
					else if (p.XValue != null)
						labels.Add(p.XValue);
				}
				break;
			}
		}

		if (labels.Count > 0)
		{
			data["labels"] = labels;
		}

		var datasets = new List<Dictionary<string, object>>();
		for (var i = 0; i < seriesList.Count; i++)
		{
			var s = seriesList[i];
			ValidateChartType(s.ChartType);

			var dataset = new Dictionary<string, object>();

			if (!string.IsNullOrEmpty(s.Name))
				dataset["label"] = s.Name;

			// Data values
			if (s.Points != null && s.Points.Count > 0)
			{
				if (s.ChartType == SeriesChartType.Point)
				{
					dataset["data"] = s.Points.Select(p => new Dictionary<string, object>
					{
						["x"] = p.XValue ?? 0,
						["y"] = p.YValues?.Length > 0 ? p.YValues[0] : 0
					}).ToList();
				}
				else
				{
					dataset["data"] = s.Points
						.Select(p => p.YValues?.Length > 0 ? p.YValues[0] : 0.0)
						.ToList();
				}
			}

			// Color from series or palette
			if (s.Color != null && !s.Color.IsEmpty)
			{
				dataset["backgroundColor"] = ToRgbaString(s.Color);
				dataset["borderColor"] = ToRgbaString(s.Color);
			}
			else if (paletteColors.Length > 0)
			{
				var color = paletteColors[i % paletteColors.Length];
				dataset["backgroundColor"] = color;
				dataset["borderColor"] = color;
			}

			if (s.BorderWidth > 0)
				dataset["borderWidth"] = s.BorderWidth;

			// Area chart fill
			if (s.ChartType == SeriesChartType.Area)
				dataset["fill"] = true;

			// Per-series type override when mixing chart types
			if (i > 0 && s.ChartType != seriesList[0].ChartType)
				dataset["type"] = MapChartType(s.ChartType);

			// Horizontal bar
			if (s.ChartType == SeriesChartType.Bar)
				dataset["indexAxis"] = "y";

			datasets.Add(dataset);
		}

		data["datasets"] = datasets;
		return data;
	}

	private static Dictionary<string, object> BuildOptionsSection(
		IReadOnlyList<ChartSeriesConfig> seriesList,
		IReadOnlyList<ChartAreaConfig> chartAreas,
		IReadOnlyList<ChartTitleConfig> titles,
		IReadOnlyList<ChartLegendConfig> legends,
		SeriesChartType primaryType)
	{
		var options = new Dictionary<string, object>
		{
			["responsive"] = true,
			["maintainAspectRatio"] = false
		};

		// Horizontal bar
		if (primaryType == SeriesChartType.Bar)
		{
			options["indexAxis"] = "y";
		}

		// Stacked charts
		if (primaryType == SeriesChartType.StackedColumn)
		{
			var scales = new Dictionary<string, object>
			{
				["x"] = new Dictionary<string, object> { ["stacked"] = true },
				["y"] = new Dictionary<string, object> { ["stacked"] = true }
			};
			options["scales"] = scales;
		}

		// Axis configuration from chart areas
		if (chartAreas != null && chartAreas.Count > 0)
		{
			var area = chartAreas[0];
			if (!options.ContainsKey("scales"))
			{
				options["scales"] = new Dictionary<string, object>();
			}

			var scales = (Dictionary<string, object>)options["scales"];

			if (area.AxisX != null)
			{
				var xAxis = BuildAxisConfig(area.AxisX);
				if (scales.ContainsKey("x"))
				{
					var existing = (Dictionary<string, object>)scales["x"];
					foreach (var kvp in xAxis)
						existing[kvp.Key] = kvp.Value;
				}
				else
				{
					scales["x"] = xAxis;
				}
			}

			if (area.AxisY != null)
			{
				var yAxis = BuildAxisConfig(area.AxisY);
				if (scales.ContainsKey("y"))
				{
					var existing = (Dictionary<string, object>)scales["y"];
					foreach (var kvp in yAxis)
						existing[kvp.Key] = kvp.Value;
				}
				else
				{
					scales["y"] = yAxis;
				}
			}
		}

		// Plugins section (title + legend)
		var plugins = new Dictionary<string, object>();

		// Title
		if (titles != null && titles.Count > 0)
		{
			var title = titles[0];
			var titleConfig = new Dictionary<string, object>
			{
				["display"] = true,
				["text"] = title.Text ?? ""
			};

			if (title.Docking.HasValue)
			{
				titleConfig["position"] = MapDocking(title.Docking.Value);
			}

			plugins["title"] = titleConfig;
		}

		// Legend
		if (legends != null && legends.Count > 0)
		{
			var legend = legends[0];
			var legendConfig = new Dictionary<string, object>
			{
				["display"] = true
			};

			if (legend.Docking.HasValue)
			{
				legendConfig["position"] = MapDocking(legend.Docking.Value);
			}

			if (!string.IsNullOrEmpty(legend.Title))
			{
				legendConfig["title"] = new Dictionary<string, object>
				{
					["display"] = true,
					["text"] = legend.Title
				};
			}

			plugins["legend"] = legendConfig;
		}

		if (plugins.Count > 0)
		{
			options["plugins"] = plugins;
		}

		return options;
	}

	private static Dictionary<string, object> BuildAxisConfig(Axis axis)
	{
		var config = new Dictionary<string, object>();

		if (!string.IsNullOrEmpty(axis.Title))
		{
			config["title"] = new Dictionary<string, object>
			{
				["display"] = true,
				["text"] = axis.Title
			};
		}

		if (axis.Minimum.HasValue)
			config["min"] = axis.Minimum.Value;

		if (axis.Maximum.HasValue)
			config["max"] = axis.Maximum.Value;

		if (axis.Interval.HasValue)
		{
			config["ticks"] = new Dictionary<string, object>
			{
				["stepSize"] = axis.Interval.Value
			};
		}

		if (axis.IsLogarithmic)
			config["type"] = "logarithmic";

		return config;
	}

	private static string MapDocking(Docking docking)
	{
		return docking switch
		{
			Docking.Top => "top",
			Docking.Bottom => "bottom",
			Docking.Left => "left",
			Docking.Right => "right",
			_ => "top"
		};
	}

	private static string ToRgbaString(WebColor color)
	{
		if (color == null || color.IsEmpty) return "rgba(0,0,0,1)";
		var c = color.ToColor();
		return $"rgba({c.R},{c.G},{c.B},{c.A / 255.0:F2})";
	}

	/// <summary>
	/// Returns an array of CSS color strings for the given palette.
	/// </summary>
	internal static string[] GetPaletteColors(ChartPalette palette)
	{
		return palette switch
		{
			ChartPalette.BrightPastel => new[]
			{
				"rgba(65,140,240,1)", "rgba(252,180,65,1)", "rgba(224,64,10,1)",
				"rgba(5,100,146,1)", "rgba(191,191,191,1)", "rgba(26,59,105,1)",
				"rgba(255,227,130,1)", "rgba(18,156,221,1)", "rgba(202,107,75,1)",
				"rgba(0,92,219,1)"
			},
			ChartPalette.Berry => new[]
			{
				"rgba(138,43,226,1)", "rgba(0,0,205,1)", "rgba(100,149,237,1)",
				"rgba(72,61,139,1)", "rgba(123,104,238,1)", "rgba(75,0,130,1)",
				"rgba(147,112,219,1)", "rgba(106,90,205,1)"
			},
			ChartPalette.Chocolate => new[]
			{
				"rgba(165,42,42,1)", "rgba(210,105,30,1)", "rgba(139,69,19,1)",
				"rgba(244,164,96,1)", "rgba(222,184,135,1)", "rgba(255,127,80,1)",
				"rgba(255,99,71,1)", "rgba(178,34,34,1)"
			},
			ChartPalette.EarthTones => new[]
			{
				"rgba(255,197,132,1)", "rgba(87,46,10,1)", "rgba(105,105,105,1)",
				"rgba(199,97,20,1)", "rgba(115,74,18,1)", "rgba(148,130,112,1)",
				"rgba(183,135,38,1)", "rgba(122,101,49,1)"
			},
			ChartPalette.Excel => new[]
			{
				"rgba(153,153,255,1)", "rgba(153,51,102,1)", "rgba(255,255,204,1)",
				"rgba(204,255,255,1)", "rgba(102,0,102,1)", "rgba(255,128,128,1)",
				"rgba(0,102,204,1)", "rgba(204,204,255,1)"
			},
			ChartPalette.Fire => new[]
			{
				"rgba(255,215,0,1)", "rgba(255,69,0,1)", "rgba(255,140,0,1)",
				"rgba(220,20,60,1)", "rgba(178,34,34,1)", "rgba(255,165,0,1)",
				"rgba(255,127,80,1)", "rgba(255,0,0,1)"
			},
			ChartPalette.Grayscale => new[]
			{
				"rgba(196,196,196,1)", "rgba(118,118,118,1)", "rgba(169,169,169,1)",
				"rgba(128,128,128,1)", "rgba(105,105,105,1)", "rgba(64,64,64,1)",
				"rgba(153,153,153,1)", "rgba(216,216,216,1)"
			},
			ChartPalette.Light => new[]
			{
				"rgba(224,224,255,1)", "rgba(255,224,192,1)", "rgba(192,255,192,1)",
				"rgba(255,192,255,1)", "rgba(192,255,255,1)", "rgba(255,255,192,1)",
				"rgba(192,192,255,1)", "rgba(255,192,192,1)"
			},
			ChartPalette.Pastel => new[]
			{
				"rgba(135,206,250,1)", "rgba(255,182,193,1)", "rgba(255,228,181,1)",
				"rgba(152,251,152,1)", "rgba(230,230,250,1)", "rgba(245,222,179,1)",
				"rgba(176,196,222,1)", "rgba(255,218,185,1)"
			},
			ChartPalette.SeaGreen => new[]
			{
				"rgba(46,139,87,1)", "rgba(60,179,113,1)", "rgba(0,128,128,1)",
				"rgba(32,178,170,1)", "rgba(0,139,139,1)", "rgba(143,188,143,1)",
				"rgba(102,205,170,1)", "rgba(72,209,204,1)"
			},
			ChartPalette.SemiTransparent => new[]
			{
				"rgba(255,0,0,0.7)", "rgba(0,128,0,0.7)", "rgba(0,0,255,0.7)",
				"rgba(255,255,0,0.7)", "rgba(0,255,255,0.7)", "rgba(255,0,255,0.7)",
				"rgba(128,0,0,0.7)", "rgba(0,128,128,0.7)"
			},
			_ => Array.Empty<string>()
		};
	}
}

/// <summary>
/// Configuration snapshot for a ChartSeries, used by ChartConfigBuilder.
/// </summary>
public class ChartSeriesConfig
{
	public string Name { get; set; }
	public SeriesChartType ChartType { get; set; }
	public IReadOnlyList<DataPoint> Points { get; set; }
	public WebColor Color { get; set; }
	public int BorderWidth { get; set; }
	public bool IsVisibleInLegend { get; set; } = true;
	public string ChartArea { get; set; }
}

/// <summary>
/// Configuration snapshot for a ChartArea, used by ChartConfigBuilder.
/// </summary>
public class ChartAreaConfig
{
	public string Name { get; set; }
	public WebColor BackColor { get; set; }
	public Axis AxisX { get; set; }
	public Axis AxisY { get; set; }
}

/// <summary>
/// Configuration snapshot for a ChartTitle, used by ChartConfigBuilder.
/// </summary>
public class ChartTitleConfig
{
	public string Text { get; set; }
	public Docking? Docking { get; set; }
}

/// <summary>
/// Configuration snapshot for a ChartLegend, used by ChartConfigBuilder.
/// </summary>
public class ChartLegendConfig
{
	public string Name { get; set; }
	public Docking? Docking { get; set; }
	public string Title { get; set; }
}
