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
	public EnumParameter<ChartPalette> Palette { get; set; } = ChartPalette.BrightPastel;

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

	#region Phase 1 Properties — Web Forms Chart parity (Pattern B+)

#pragma warning disable CS0618 // Obsolete members used in default assignments

	/// <summary>
	/// Gets or sets the anti-aliasing style. Canvas rendering always applies anti-aliasing;
	/// this property is accepted for migration compatibility only.
	/// </summary>
	[Parameter]
	[Obsolete("Canvas rendering always applies anti-aliasing. This property is accepted for migration compatibility.")]
	public EnumParameter<AntiAliasingStyles> AntiAliasing { get; set; } = AntiAliasingStyles.All;

	/// <summary>
	/// Gets or sets the gradient style for the chart background. When set to a value other
	/// than None, a CSS linear-gradient or radial-gradient is applied to the container using
	/// BackColor and BackSecondaryColor.
	/// </summary>
	[Parameter]
	public EnumParameter<GradientStyle> BackGradientStyle { get; set; } = GradientStyle.None;

	/// <summary>
	/// Gets or sets the hatch pattern style for the chart background. GDI+ hatch patterns
	/// have no direct CSS or Canvas equivalent; this property is accepted for migration
	/// compatibility only.
	/// </summary>
	[Parameter]
	[Obsolete("GDI+ hatch patterns cannot be rendered in CSS/Canvas. This property is accepted for migration compatibility.")]
	public EnumParameter<ChartHatchStyle> BackHatchStyle { get; set; } = ChartHatchStyle.None;

	/// <summary>
	/// Gets or sets the secondary background color, used as the second color stop in gradient fills.
	/// </summary>
	[Parameter]
	public WebColor BackSecondaryColor { get; set; }

	/// <summary>
	/// Gets or sets the border line dash style. Maps to CSS border-style on the container div.
	/// </summary>
	[Parameter]
	public EnumParameter<ChartDashStyle> BorderlineDashStyle { get; set; } = ChartDashStyle.NotSet;

	/// <summary>
	/// Gets or sets the image file location. Client-side Chart.js rendering does not generate
	/// server images; this property is accepted for migration compatibility only.
	/// </summary>
	[Parameter]
	[Obsolete("Client-side Chart.js rendering does not generate server images. This property is accepted for migration compatibility.")]
	public string ImageLocation { get; set; } = "";

	/// <summary>
	/// Gets or sets the image storage mode. Client-side Chart.js rendering does not generate
	/// server images; this property is accepted for migration compatibility only.
	/// </summary>
	[Parameter]
	[Obsolete("Chart.js renders directly to canvas. Server-side image storage is not applicable.")]
	public EnumParameter<ImageStorageMode> ImageStorageMode { get; set; } = Enums.ImageStorageMode.UseHttpHandler;

	/// <summary>
	/// Gets or sets the text anti-aliasing quality. Browser canvas text rendering is always
	/// high-quality; this property is accepted for migration compatibility only.
	/// </summary>
	[Parameter]
	[Obsolete("Browser canvas text rendering is always high-quality. This property is accepted for migration compatibility.")]
	public EnumParameter<TextAntiAliasingQuality> TextAntiAliasingQuality { get; set; } = Enums.TextAntiAliasingQuality.High;

	/// <summary>
	/// Fires to allow customization of map areas. Canvas-based charts do not use HTML image
	/// maps; this event is accepted for migration compatibility only and is never raised.
	/// </summary>
	[Parameter]
	[Obsolete("Canvas-based charts do not use HTML image maps. Use Chart.js click handlers instead.")]
	public EventCallback CustomizeMapAreas { get; set; }

#pragma warning restore CS0618

	/// <summary>
	/// Fires to allow customization of the chart legend before rendering.
	/// </summary>
	[Parameter]
	public EventCallback CustomizeLegend { get; set; }

	#endregion

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

	/// <summary>
	/// Computed inline style for gradient background and border dash style.
	/// </summary>
	private string ChartPropertyStyle
	{
		get
		{
			var parts = new List<string>();

			var gradient = GetGradientCss();
			if (!string.IsNullOrEmpty(gradient))
				parts.Add($"background:{gradient}");

			var border = GetBorderStyleCss();
			if (!string.IsNullOrEmpty(border))
				parts.Add($"border-style:{border}");

			return string.Join(";", parts);
		}
	}

	private string GetGradientCss()
	{
		var style = (GradientStyle)BackGradientStyle;
		if (style == GradientStyle.None)
			return null;

		if (BackSecondaryColor == default(WebColor))
			return null;

		var primary = BackColor != default(WebColor) ? BackColor.ToHtml() : "transparent";
		var secondary = BackSecondaryColor.ToHtml();

		return style switch
		{
			GradientStyle.TopBottom => $"linear-gradient(to bottom, {primary}, {secondary})",
			GradientStyle.BottomTop => $"linear-gradient(to top, {primary}, {secondary})",
			GradientStyle.LeftRight => $"linear-gradient(to right, {primary}, {secondary})",
			GradientStyle.RightLeft => $"linear-gradient(to left, {primary}, {secondary})",
			GradientStyle.Center => $"radial-gradient(circle, {primary}, {secondary})",
			GradientStyle.DiagonalLeft => $"linear-gradient(to bottom right, {primary}, {secondary})",
			GradientStyle.DiagonalRight => $"linear-gradient(to bottom left, {primary}, {secondary})",
			GradientStyle.HorizontalCenter => $"linear-gradient(to right, {secondary}, {primary}, {secondary})",
			GradientStyle.VerticalCenter => $"linear-gradient(to bottom, {secondary}, {primary}, {secondary})",
			_ => null
		};
	}

	private string GetBorderStyleCss()
	{
		return (ChartDashStyle)BorderlineDashStyle switch
		{
			ChartDashStyle.Solid => "solid",
			ChartDashStyle.Dash => "dashed",
			ChartDashStyle.Dot => "dotted",
			ChartDashStyle.DashDot => "dashed",
			ChartDashStyle.DashDotDot => "dashed",
			_ => null
		};
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
