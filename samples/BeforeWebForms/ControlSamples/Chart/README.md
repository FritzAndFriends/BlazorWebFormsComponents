# Chart Control Samples

This folder contains ASP.NET Web Forms Chart control samples that demonstrate various features and usage patterns. These samples serve as references for building a corresponding Blazor Chart component.

## Sample Files

### Default.aspx - Basic Column Chart
Demonstrates:
- Basic Chart control setup with Series and ChartAreas
- Column chart type (vertical bars)
- Styling properties (BackColor, BorderColor, etc.)
- Axis configuration with titles and grid lines
- Adding data points programmatically in code-behind
- Chart title configuration

**Key Features Shown:**
- `asp:Chart` control structure
- `asp:Series` with ChartType="Column"
- `asp:ChartArea` with axis configuration
- `asp:Title` for chart title
- Adding points with `Series.Points.AddXY()`

### PieChart.aspx - Pie Chart with Custom Colors
Demonstrates:
- Pie chart visualization for market share data
- Custom color assignment for each slice
- Percentage labels using format strings
- Legend configuration
- Smart label positioning

**Key Features Shown:**
- ChartType="Pie"
- Custom point colors using `Color.FromArgb()`
- Label formatting with `#PERCENT{P1}`
- `asp:Legend` configuration
- `SmartLabelStyle` for automatic label positioning

### LineChart.aspx - Multiple Series Line Chart
Demonstrates:
- Line chart with multiple data series
- Comparison visualization (Actual vs Target)
- Different line styles (solid vs dashed)
- Marker styles (Circle vs Diamond)
- Value labels on data points
- Legend positioning

**Key Features Shown:**
- Multiple `asp:Series` in one chart
- BorderDashStyle property
- MarkerStyle and MarkerSize properties
- IsValueShownAsLabel property
- Legend docking options

### DataBinding.aspx - Data Binding Example
Demonstrates:
- Binding Chart to data source (ObjectDataSource)
- Using XValueMember and YValueMembers for data mapping
- Bar chart (horizontal bars) visualization
- SelectMethod pattern for data retrieval
- Working with collection of objects

**Key Features Shown:**
- DataSourceID binding
- XValueMember and YValueMembers properties
- ObjectDataSource integration
- ChartType="Bar"
- Static method as data source

## Chart Control Reference

**Namespace:** `System.Web.UI.DataVisualization.Charting`  
**Assembly:** `System.Web.DataVisualization`

**Official Documentation:**
- [Chart Class Reference](https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.datavisualization.charting.chart?view=netframework-4.8.1)
- [Chart Control Samples (Archive)](https://github.com/microsoftarchive/msdn-code-gallery-community-a-c/tree/master/ASP.NET%20Samples%20Environment%20for%20Microsoft%20Chart%20Controls)

## Key Properties and Features to Emulate

### Chart Element Hierarchy
```
asp:Chart
├── Series (collection)
│   └── asp:Series
│       ├── Name
│       ├── ChartType (Column, Bar, Line, Pie, Area, etc.)
│       ├── Color
│       ├── BorderWidth
│       ├── BorderColor
│       ├── MarkerStyle
│       ├── Points (collection)
│       └── XValueMember/YValueMembers (for data binding)
├── ChartAreas (collection)
│   └── asp:ChartArea
│       ├── AxisX (configuration)
│       └── AxisY (configuration)
├── Titles (collection)
│   └── asp:Title
└── Legends (collection)
    └── asp:Legend
```

### Common Chart Types
- Column (vertical bars)
- Bar (horizontal bars)
- Line
- Pie
- Area
- Point (scatter)
- Spline
- Doughnut
- And many more...

### Important Features
1. **Series Management** - Multiple series for comparison
2. **Data Binding** - Support for DataSource, XValueMember, YValueMembers
3. **Styling** - Colors, borders, backgrounds, fonts
4. **Axes** - Titles, grid lines, labels, intervals
5. **Legends** - Position, style, alignment
6. **Titles** - Chart titles and subtitles
7. **Labels** - Value labels, custom formats
8. **Markers** - Data point markers for line charts

## Notes for Blazor Implementation

When creating the Blazor Chart component:

1. **Client-Side Rendering** - Consider using a JavaScript charting library (e.g., Chart.js, ApexCharts) to handle actual chart rendering
2. **Data Structure** - Design flexible data models that support various chart types
3. **Property Mapping** - Map ASP.NET Chart properties to equivalent JavaScript library options
4. **Series Configuration** - Support multiple series with different chart types
5. **Responsive Design** - Ensure charts adapt to different screen sizes
6. **Event Handling** - Consider click, hover, and selection events
7. **Data Binding** - Support binding to collections with flexible data mapping

## Testing These Samples

**Note:** These samples require:
- .NET Framework 4.8
- IIS or IIS Express
- Visual Studio for building the project

The project cannot be built with `dotnet build` as it requires MSBuild and Web Application targets that are only available in Visual Studio.

To run these samples:
1. Open `BlazorMeetsWebForms.sln` in Visual Studio
2. Set `BeforeWebForms` as the startup project
3. Run with IIS Express
4. Navigate to `/ControlSamples/Chart/Default.aspx`
