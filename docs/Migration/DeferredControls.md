# Deferred Controls — Chart, Substitution, and Xml

Some ASP.NET Web Forms controls have no practical Blazor equivalent and are **permanently deferred** from the BlazorWebFormsComponents library. This page explains what each control did in Web Forms, why it is not implemented, and what you should use instead when migrating to Blazor.

!!! note "These controls are not coming"
    Unlike other components in this library that are planned or in progress, these three controls have been permanently deferred. They will not be implemented. This page provides migration guidance so you can move forward without them.

---

## Chart

Original Microsoft documentation: [https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.datavisualization.charting.chart?view=netframework-4.8](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.datavisualization.charting.chart?view=netframework-4.8)

### What It Did in Web Forms

The `<asp:Chart>` control rendered data as bar charts, line charts, pie charts, area charts, and dozens of other chart types. It was a server-side rendering control that generated chart images (PNG, JPEG, or SVG) and served them to the browser. Under the hood, it used GDI+ (`System.Drawing`) to rasterize charts — a technology that does not exist in Blazor's browser-based rendering model.

```html
<asp:Chart ID="SalesChart" runat="server" Width="600" Height="400">
    <Series>
        <asp:Series Name="Sales" ChartType="Column"
                    XValueMember="Month" YValueMembers="Revenue" />
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="MainArea" />
    </ChartAreas>
</asp:Chart>
```

### Why It's Not Implemented

The Web Forms Chart control is **Very High complexity** to replicate in Blazor:

- It requires a full SVG or Canvas rendering engine — there is no equivalent Blazor primitive
- The original control relied on server-side GDI+ image generation, which is fundamentally incompatible with Blazor's component model
- Wrapping an external charting library would introduce a heavyweight dependency that doesn't align with this library's goal of lightweight Web Forms compatibility shims

### Recommended Blazor Alternatives

The Blazor ecosystem has mature charting libraries that are purpose-built for client-side rendering. Choose one based on your project needs:

| Library | License | Notes |
|---------|---------|-------|
| [Radzen Blazor Charts](https://blazor.radzen.com/chart) | Free (MIT) | SVG-based, good variety of chart types |
| [MudBlazor Charts](https://mudblazor.com/components/chart) | Free (MIT) | Simple API, integrates with MudBlazor component suite |
| [Syncfusion Blazor Charts](https://www.syncfusion.com/blazor-components/blazor-charts) | Commercial (free community license available) | Feature-rich, closest to Web Forms Chart in capability |
| [ApexCharts.Blazor](https://github.com/apexcharts/Blazor-ApexCharts) | Free (MIT) | Wrapper around ApexCharts.js, interactive charts |

### Migration Example

**Before (Web Forms):**

```html
<asp:Chart ID="SalesChart" runat="server" Width="600" Height="400">
    <Series>
        <asp:Series Name="Sales" ChartType="Column"
                    XValueMember="Month" YValueMembers="Revenue" />
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="MainArea" />
    </ChartAreas>
</asp:Chart>
```

```csharp
// Code-behind
SalesChart.DataSource = GetSalesData();
SalesChart.DataBind();
```

**After (Blazor with Radzen Charts):**

```razor
@using Radzen.Blazor

<RadzenChart>
    <RadzenColumnSeries Data="@salesData"
                        CategoryProperty="Month"
                        ValueProperty="Revenue"
                        Title="Sales" />
    <RadzenCategoryAxis />
    <RadzenValueAxis />
</RadzenChart>

@code {
    private List<SalesRecord> salesData;

    protected override void OnInitialized()
    {
        salesData = GetSalesData();
    }
}
```

!!! tip "Migration Approach"
    Don't try to replicate your `<asp:Chart>` markup one-to-one. Instead, identify what data your charts visualize and which chart types you use, then map those to the equivalent chart component in your chosen library. Most libraries support the same chart types — the markup syntax will simply be different.

---

## Substitution

Original Microsoft documentation: [https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.substitution?view=netframework-4.8](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.substitution?view=netframework-4.8)

### What It Did in Web Forms

The `<asp:Substitution>` control was a cache-control mechanism. When a Web Forms page was output-cached, `Substitution` marked a region of the page as **dynamic** — content that should be re-evaluated on every request even though the rest of the page was served from cache. It called a static method to generate fresh content for that region.

```html
<%@ OutputCache Duration="60" VaryByParam="none" %>

<p>This content is cached for 60 seconds.</p>

<asp:Substitution ID="TimeStamp" runat="server"
                  MethodName="GetCurrentTime" />

<p>This content is also cached.</p>
```

```csharp
// Code-behind — must be a static method
public static string GetCurrentTime(HttpContext context)
{
    return DateTime.Now.ToString("HH:mm:ss");
}
```

### Why It's Not Implemented

The `Substitution` control is **architecturally incompatible** with Blazor:

- Blazor does not use ASP.NET output caching. There is no page-level cache to punch holes in.
- In Blazor Server, the UI is maintained as a live component tree over a SignalR connection — every render is already "dynamic."
- In Blazor WebAssembly, the entire application runs in the browser — server-side output caching is not applicable.
- The concept of "cache substitution" simply does not exist in Blazor's rendering model.

### What to Do Instead

**No migration is needed.** Blazor's component lifecycle already provides what `Substitution` was designed to achieve — dynamic content that updates on every render.

If your Web Forms page used `Substitution` to show a timestamp, user-specific greeting, or other per-request content, that content will naturally be dynamic in Blazor:

**Before (Web Forms):**

```html
<%@ OutputCache Duration="60" VaryByParam="none" %>

<p>Welcome to our site!</p>
<asp:Substitution ID="UserGreeting" runat="server"
                  MethodName="GetUserGreeting" />
```

```csharp
public static string GetUserGreeting(HttpContext context)
{
    return $"Hello, {context.User.Identity.Name}!";
}
```

**After (Blazor):**

```razor
<p>Welcome to our site!</p>
<p>Hello, @username!</p>

@code {
    private string username;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        username = state.User.Identity?.Name ?? "Guest";
    }
}
```

!!! note "If you need caching in Blazor"
    If your Web Forms application relied heavily on output caching for performance, Blazor offers different caching strategies:

    - **`IMemoryCache`** or **`IDistributedCache`** for data-level caching in your services
    - **`@attribute [OutputCache]`** on Razor components in .NET 8+ static SSR mode
    - **`@attribute [StreamRendering]`** for progressive rendering while data loads

    These are applied at different levels than Web Forms output caching, but they solve the same performance problems.

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

| Control | Web Forms Purpose | Blazor Equivalent | Action Required |
|---------|-------------------|-------------------|-----------------|
| **Chart** | Server-side chart image rendering | Use a Blazor charting library (Radzen, MudBlazor, Syncfusion, ApexCharts) | Replace with a third-party library |
| **Substitution** | Dynamic content in cached pages | Not needed — Blazor renders dynamically by default | Remove the control; content is already dynamic |
| **Xml** | XML display and XSLT transforms | Direct data binding with Razor markup | Parse your XML data in C# and bind to components |

## See Also

- [Migration — Getting Started](readme.md)
- [Migration Strategies](Strategies.md)
- [Custom Controls](Custom-Controls.md)
