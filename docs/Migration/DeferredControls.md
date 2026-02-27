# Deferred Controls

Some ASP.NET Web Forms controls have been **permanently deferred** from implementation in this library. This page documents each deferred control, explains why it's excluded, and provides recommended alternatives for your Blazor migration.

## Chart — Implemented

!!! success "Chart is FULLY IMPLEMENTED"
    The [Chart component](../DataControls/Chart.md) has been implemented with Chart.js and supports 8 chart types: Column, Bar, Line, Pie, Area, Doughnut, Scatter (Point), and StackedColumn. See the [Chart documentation](../DataControls/Chart.md) for full details.

### Unsupported Chart Types

The following 27 chart types from the Web Forms `SeriesChartType` enum are **not supported** and will throw `NotSupportedException`. If your application uses any of these, you will need an alternative approach:

| Chart Type | Alternative |
|------------|-------------|
| Stock, Candlestick | Use a dedicated financial charting library (e.g., [Lightweight Charts](https://github.com/nicksenger/lightweight-charts-blazor)) |
| Bubble | Can be approximated with a scatter chart and custom point sizes via Chart.js plugins |
| Radar, Polar | Chart.js supports these natively — consider a Chart.js plugin or custom integration |
| Funnel, Pyramid | Use a dedicated visualization library or custom SVG |
| Spline, SplineArea, SplineRange | Use `Line`/`Area` as an approximation; Chart.js tension options can create curved lines |
| StackedBar, StackedArea, StackedColumn100, StackedBar100, StackedArea100 | Use `StackedColumn` or `Bar` as a starting point; full stacking support may be added in a future release |
| Range, RangeBar, RangeColumn | Use a dedicated charting library for range-style visualizations |
| BoxPlot, ErrorBar | Use a statistical charting library |
| Renko, ThreeLineBreak, Kagi, PointAndFigure | Specialized financial chart types — use a dedicated financial charting library |
| FastPoint, FastLine, StepLine | Use `Point` or `Line` respectively; Chart.js handles performance optimization automatically |

---

## Substitution — Implemented

!!! success "Substitution is IMPLEMENTED"
    The [Substitution component](../EditorControls/Substitution.md) has been implemented as a migration compatibility component. In Web Forms, Substitution provided post-cache substitution; in Blazor, it renders callback output directly since all rendering is dynamic. See the [Substitution documentation](../EditorControls/Substitution.md) for full details.

---

## Xml

Original Microsoft documentation: [https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.xml?view=netframework-4.8](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.xml?view=netframework-4.8)

### What It Did in Web Forms

The `<asp:Xml>` control displayed the contents of an XML document or the results of an XSLT transformation. It could take an XML source (inline, from a file, or from a `System.Xml.XmlDocument`) and optionally transform it using an XSLT stylesheet before rendering the output.

```html
<asp:Xml ID="XmlDisplay" runat="server"
         DocumentSource="~/App_Data/catalog.xml"
         TransformSource="~/App_Data/catalog.xslt" />
```

Or with inline XML:

```html
<asp:Xml ID="XmlInline" runat="server"
         TransformSource="~/App_Data/transform.xslt">
    <book>
        <title>Blazor in Action</title>
        <author>Chris Sainty</author>
    </book>
</asp:Xml>
```

### Why It's Not Implemented

XSLT transforms via `<asp:Xml>` are a **legacy pattern with near-zero adoption** in modern projects:

- XSLT is rarely used in new development — it has been superseded by direct data binding, JSON APIs, and component-based rendering
- The control existed for a very specific early-2000s pattern of XML-driven content rendering that has no meaningful migration demand
- Building an XSLT transformation engine as a Blazor component would add complexity for a feature almost no one migrating to Blazor will need

### What to Do Instead

**Replace with direct data binding or Razor markup.** If your Web Forms application used `<asp:Xml>` to display structured data, the Blazor equivalent is simply binding that data to components or HTML directly.

**Before (Web Forms — XML + XSLT to render a list):**

```html
<asp:Xml ID="BookList" runat="server"
         DocumentSource="~/App_Data/books.xml"
         TransformSource="~/App_Data/books.xslt" />
```

```xml
<!-- books.xml -->
<books>
    <book><title>Blazor in Action</title><author>Chris Sainty</author></book>
    <book><title>ASP.NET Core in Action</title><author>Andrew Lock</author></book>
</books>
```

```xslt
<!-- books.xslt -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/books">
        <ul>
            <xsl:for-each select="book">
                <li><xsl:value-of select="title"/> by <xsl:value-of select="author"/></li>
            </xsl:for-each>
        </ul>
    </xsl:template>
</xsl:stylesheet>
```

**After (Blazor — direct data binding):**

```razor
<ul>
    @foreach (var book in books)
    {
        <li>@book.Title by @book.Author</li>
    }
</ul>

@code {
    private List<Book> books;

    protected override void OnInitialized()
    {
        books = BookService.GetBooks();
    }
}
```

!!! tip "If you genuinely need XSLT in Blazor"
    If your application logic truly depends on XSLT transformations (e.g., you receive XML from a third-party system and must apply an XSLT stylesheet), you can still use `System.Xml.Xsl.XslCompiledTransform` in your C# code and render the result as a `MarkupString`:

    ```razor
    @((MarkupString)transformedHtml)

    @code {
        private string transformedHtml;

        protected override void OnInitialized()
        {
            var xslt = new XslCompiledTransform();
            xslt.Load("transform.xslt");

            using var writer = new StringWriter();
            xslt.Transform("source.xml", null, writer);
            transformedHtml = writer.ToString();
        }
    }
    ```

    This approach keeps the XSLT logic in C# where it belongs, rather than embedding it in a UI control.

---

## Summary

| Control | Status | Recommendation |
|---------|--------|----------------|
| **Chart** | ✅ Complete | [Implemented](../DataControls/Chart.md) with 8 chart types via Chart.js. Unsupported types need alternative libraries. |
| **Substitution** | ✅ Complete | [Implemented](../EditorControls/Substitution.md) — renders callback output directly. Post-cache substitution not applicable in Blazor. |
| **Xml** | ❌ Deferred | Use `XDocument` + data binding or `XslCompiledTransform` + `MarkupString` |

## See Also

- [Migration — Getting Started](readme.md)
- [Migration Strategies](Strategies.md)
- [Custom Controls](Custom-Controls.md)
