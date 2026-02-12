using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents;

public partial class Chart : BaseStyledComponent
{
	private ElementReference _canvasRef;
	private ChartJsInterop _chartInterop;
	private bool _chartCreated;

	private readonly List<ChartSeries> _series = new();
	private readonly List<ChartArea> _chartAreas = new();
	private readonly List<ChartLegend> _legends = new();
	private readonly List<ChartTitle> _titles = new();

	/// <summary>
	/// Gets the canvas element ID used for Chart.js.
	/// </summary>
	private string CanvasId => $"{ID ?? "chart"}_canvas_{_instanceId}";
	private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];

	/// <summary>
	/// Gets or sets the width of the chart in CSS units (e.g., "400px").
	/// </summary>
	[Parameter]
	public string ChartWidth { get; set; }

	/// <summary>
	/// Gets or sets the height of the chart in CSS units (e.g., "300px").
	/// </summary>
	[Parameter]
	public string ChartHeight { get; set; }

	/// <summary>
	/// Gets or sets the color palette for the chart.
	/// </summary>
	[Parameter]
	public ChartPalette Palette { get; set; } = ChartPalette.BrightPastel;

	/// <summary>
	/// Gets or sets the image type. For API compatibility only; not functional.
	/// </summary>
	[Parameter]
	public string ImageType { get; set; }

	/// <summary>
	/// Gets or sets the child content (ChartSeries, ChartArea, ChartLegend, ChartTitle).
	/// </summary>
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	/// <summary>
	/// Computed inline style for chart dimensions.
	/// </summary>
	private string ChartDimensionStyle
	{
		get
		{
			var parts = new List<string>();
			if (!string.IsNullOrEmpty(ChartWidth))
				parts.Add($"width:{ChartWidth}");
			if (!string.IsNullOrEmpty(ChartHeight))
				parts.Add($"height:{ChartHeight}");
			return string.Join(";", parts);
		}
	}

	internal void RegisterSeries(ChartSeries series)
	{
		if (!_series.Contains(series))
		{
			_series.Add(series);
		}
	}

	internal void RegisterChartArea(ChartArea area)
	{
		if (!_chartAreas.Contains(area))
		{
			_chartAreas.Add(area);
		}
	}

	internal void RegisterLegend(ChartLegend legend)
	{
		if (!_legends.Contains(legend))
		{
			_legends.Add(legend);
		}
	}

	internal void RegisterTitle(ChartTitle title)
	{
		if (!_titles.Contains(title))
		{
			_titles.Add(title);
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (firstRender)
		{
			_chartInterop = new ChartJsInterop(JsRuntime);

			// Defer chart creation to allow child components to register
			await Task.Yield();
			await CreateOrUpdateChartAsync();
			_chartCreated = true;
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

		if (_chartCreated)
		{
			await CreateOrUpdateChartAsync();
		}
	}

	private async Task CreateOrUpdateChartAsync()
	{
		if (_chartInterop == null) return;

		var config = ChartConfigBuilder.BuildConfig(
			_series.Select(s => s.ToConfig()).ToList(),
			_chartAreas.Select(a => a.ToConfig()).ToList(),
			_titles.Select(t => t.ToConfig()).ToList(),
			_legends.Select(l => l.ToConfig()).ToList(),
			Palette);

		try
		{
			if (_chartCreated)
			{
				await _chartInterop.UpdateChartAsync(CanvasId, config);
			}
			else
			{
				await _chartInterop.CreateChartAsync(CanvasId, config);
			}
		}
		catch (JSException)
		{
			// Chart.js may not be loaded in SSR/prerender scenarios
		}
	}

	protected override async ValueTask Dispose(bool disposing)
	{
		if (disposing && _chartInterop != null)
		{
			try
			{
				await _chartInterop.DestroyChartAsync(CanvasId);
			}
			catch (JSDisconnectedException)
			{
				// Circuit may already be disconnected
			}
			catch (ObjectDisposedException)
			{
				// Runtime may already be disposed
			}

			await _chartInterop.DisposeAsync();
			_chartInterop = null;
		}

		await base.Dispose(disposing);
	}
}
