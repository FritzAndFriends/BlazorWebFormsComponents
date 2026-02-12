# Deferred Controls

Some ASP.NET Web Forms controls have been **permanently deferred** from implementation in this library. This page documents each deferred control, explains why it's excluded, and provides recommended alternatives for your Blazor migration.

## Chart — Partially Implemented

!!! note "Chart is NOW IMPLEMENTED (Phase 1)"
    The [Chart component](../DataControls/Chart.md) has been implemented with Chart.js and supports 8 chart types: Column, Bar, Line, Pie, Area, Doughnut, Scatter (Point), and StackedColumn. See the [Chart documentation](../DataControls/Chart.md) for full details.

### Unsupported Chart Types

The following 27 chart types from the Web Forms `SeriesChartType` enum are **not yet supported** and will throw `NotSupportedException`. If your application uses any of these, you will need an alternative approach:

| Chart Type | Alternative |
|------------|-------------|
| Stock, Candlestick | Use a dedicated financial charting library (e.g., [Lightweight Charts](https://github.com/nicksenger/lightweight-charts-blazor)) |
| Bubble | Can be approximated with a scatter chart and custom point sizes via Chart.js plugins |
| Radar, Polar | Chart.js supports these natively — Phase 2/3 candidates |
| Funnel, Pyramid | Use a dedicated visualization library or custom SVG |
| Spline, SplineArea, SplineRange | Use `Line`/`Area` as an approximation; Chart.js tension options can create curved lines |
| StackedBar, StackedArea, StackedColumn100, StackedBar100, StackedArea100 | Use `StackedColumn` or `Bar` as a starting point; full stacking support is a Phase 2/3 candidate |
| Range, RangeBar, RangeColumn | Use a dedicated charting library for range-style visualizations |
| BoxPlot, ErrorBar | Use a statistical charting library |
| Renko, ThreeLineBreak, Kagi, PointAndFigure | Specialized financial chart types — use a dedicated financial charting library |
| FastPoint, FastLine, StepLine | Use `Point` or `Line` respectively; Chart.js handles performance optimization automatically |

---

## Substitution

### What It Did in Web Forms

The [`Substitution`](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.substitution?view=netframework-4.8) control specified a section of an output-cached page that was exempt from caching. It allowed dynamic content to be injected into an otherwise cached response via a static callback method.

```html
<asp:Substitution ID="Sub1" runat="server"
    MethodName="GetCurrentTime" />
```

### Why It's Not Implemented

Blazor does not use the same output caching model as Web Forms. There is no equivalent of the `HttpCachePolicy`-based page output cache that `Substitution` was designed to work with. Blazor Server uses SignalR circuits (always dynamic), and Blazor WebAssembly runs entirely on the client.

### Recommended Alternative

For dynamic content in cached pages, use standard Blazor component lifecycle methods. If you need server-side output caching, use ASP.NET Core [Response Caching middleware](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response) with `<cache>` tag helpers in Razor Pages or MVC, not in Blazor components.

---

## Xml

### What It Did in Web Forms

The [`Xml`](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.xml?view=netframework-4.8) control displayed the contents of an XML document or the results of an XSL Transformation (XSLT), rendered directly into the page.

```html
<asp:Xml ID="Xml1" runat="server"
    DocumentSource="~/data/catalog.xml"
    TransformSource="~/transforms/catalog.xslt" />
```

### Why It's Not Implemented

XSLT transforms are a legacy technology with near-zero adoption in modern web applications. XML display can be handled with standard data binding. The migration demand for this control is extremely low.

### Recommended Alternative

- **Displaying XML data**: Parse XML with `System.Xml.Linq` (`XDocument`) and bind the data to standard Blazor components like `Repeater` or `GridView`
- **XSLT transforms**: If you must run XSLT, use `System.Xml.Xsl.XslCompiledTransform` in your service layer and render the HTML result with `MarkupString`

```razor
@* Before: Web Forms *@
@* <asp:Xml DocumentSource="~/data.xml" TransformSource="~/style.xslt" /> *@

@* After: Blazor *@
@((MarkupString)transformedHtml)

@code {
    private string transformedHtml;

    protected override void OnInitialized()
    {
        var xslt = new System.Xml.Xsl.XslCompiledTransform();
        xslt.Load("wwwroot/style.xslt");
        using var writer = new StringWriter();
        xslt.Transform("wwwroot/data.xml", null, writer);
        transformedHtml = writer.ToString();
    }
}
```

---

## Summary

| Control | Status | Recommendation |
|---------|--------|----------------|
| **Chart** | ✅ Partial (Phase 1) | [Implemented](../DataControls/Chart.md) with 8 chart types via Chart.js. Unsupported types need alternative libraries. |
| **Substitution** | ❌ Deferred | Use Blazor component lifecycle; not applicable to Blazor's rendering model |
| **Xml** | ❌ Deferred | Use `XDocument` + data binding or `XslCompiledTransform` + `MarkupString` |
