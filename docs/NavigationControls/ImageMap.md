# ImageMap

The **ImageMap** component displays an image with defined clickable hot spot regions. Each hot spot can navigate to a URL or trigger a server-side event when clicked, emulating the ASP.NET Web Forms ImageMap control. This is useful for interactive images like site maps, diagrams, or region selectors.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagemap?view=netframework-4.8

## Features Supported in Blazor

- `ImageUrl` — URL of the image to display
- `AlternateText` — alternate text when the image is unavailable
- `DescriptionUrl` — URL to a detailed description for accessibility
- `ImageAlign` — alignment of the image relative to other elements (`NotSet`, `Left`, `Right`, `Baseline`, `Top`, `Middle`, `Bottom`, `AbsBottom`, `AbsMiddle`, `TextTop`)
- `ToolTip` — tooltip text displayed on hover
- `GenerateEmptyAlternateText` — generates an empty `alt=""` for decorative images
- `HotSpotMode` — default behavior for hot spots: `Navigate`, `PostBack`, `Inactive`, or `NotSet`
- `Target` — default target window/frame for navigation links
- `HotSpots` — collection of `HotSpot` objects defining clickable regions
- `OnClick` — event raised when a hot spot in `PostBack` mode is clicked, providing `ImageMapEventArgs`
- `Visible` — controls visibility

### Hot Spot Types

| Type | Shape | Properties | Coordinates Format |
|------|-------|------------|-------------------|
| `RectangleHotSpot` | `rect` | `Left`, `Top`, `Right`, `Bottom` | `left,top,right,bottom` |
| `CircleHotSpot` | `circle` | `X`, `Y`, `Radius` | `x,y,radius` |
| `PolygonHotSpot` | `poly` | `Coordinates` (string) | `x1,y1,x2,y2,...` |

### HotSpot Base Properties

All hot spot types share these properties:

| Property | Type | Description |
|----------|------|-------------|
| `AlternateText` | `string` | Alt text for the hot spot area |
| `HotSpotMode` | `HotSpotMode` | Behavior override (`NotSet` inherits from parent `ImageMap`) |
| `NavigateUrl` | `string` | URL to navigate to (when mode is `Navigate`) |
| `PostBackValue` | `string` | Value passed in event args (when mode is `PostBack`) |
| `Target` | `string` | Target window/frame for navigation |
| `TabIndex` | `short` | Tab order of the hot spot |
| `AccessKey` | `string` | Keyboard shortcut for the hot spot |

## Web Forms Features NOT Supported

- **Style properties** (`BackColor`, `ForeColor`, etc.) — Not applicable to the `<img>` and `<map>` elements; use CSS on a wrapper element
- **EnableTheming/SkinID** — ASP.NET theming is not available in Blazor
- **Lifecycle events** (`OnDataBinding`, `OnInit`, etc.) — Use Blazor lifecycle methods instead

## Web Forms Declarative Syntax

```html
<asp:ImageMap
    ID="string"
    ImageUrl="uri"
    AlternateText="string"
    HotSpotMode="Navigate|PostBack|Inactive|NotSet"
    OnClick="Click event handler"
    Target="string"
    ToolTip="string"
    runat="server">
    <asp:RectangleHotSpot
        AlternateText="string"
        Bottom="integer"
        HotSpotMode="Navigate|PostBack|Inactive|NotSet"
        Left="integer"
        NavigateUrl="uri"
        PostBackValue="string"
        Right="integer"
        Target="string"
        Top="integer" />
    <asp:CircleHotSpot
        AlternateText="string"
        HotSpotMode="Navigate|PostBack|Inactive|NotSet"
        NavigateUrl="uri"
        PostBackValue="string"
        Radius="integer"
        Target="string"
        X="integer"
        Y="integer" />
    <asp:PolygonHotSpot
        AlternateText="string"
        Coordinates="string"
        HotSpotMode="Navigate|PostBack|Inactive|NotSet"
        NavigateUrl="uri"
        PostBackValue="string"
        Target="string" />
</asp:ImageMap>
```

## Blazor Razor Syntax

### Navigation Hot Spots

```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums

<ImageMap ImageUrl="/images/floorplan.png"
          AlternateText="Office Floor Plan"
          HotSpotMode="HotSpotMode.Navigate"
          HotSpots="@hotSpots" />

@code {
    private List<HotSpot> hotSpots = new List<HotSpot>
    {
        new RectangleHotSpot
        {
            Left = 0, Top = 0, Right = 200, Bottom = 150,
            NavigateUrl = "/rooms/conference-a",
            AlternateText = "Conference Room A"
        },
        new CircleHotSpot
        {
            X = 300, Y = 200, Radius = 50,
            NavigateUrl = "/rooms/lobby",
            AlternateText = "Lobby"
        }
    };
}
```

### PostBack Hot Spots with Event Handling

```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums

<ImageMap ImageUrl="/images/regions.png"
          AlternateText="Select a Region"
          HotSpotMode="HotSpotMode.PostBack"
          OnClick="HandleRegionClick"
          HotSpots="@regions" />

<p>Selected region: @selectedRegion</p>

@code {
    private string selectedRegion = "None";

    private List<HotSpot> regions = new List<HotSpot>
    {
        new PolygonHotSpot
        {
            Coordinates = "10,10,100,10,100,100,10,100",
            PostBackValue = "North",
            AlternateText = "North Region"
        },
        new RectangleHotSpot
        {
            Left = 110, Top = 10, Right = 200, Bottom = 100,
            PostBackValue = "South",
            AlternateText = "South Region"
        }
    };

    void HandleRegionClick(ImageMapEventArgs args)
    {
        selectedRegion = args.PostBackValue;
    }
}
```

### Mixed Hot Spot Modes

Individual hot spots can override the parent `HotSpotMode`:

```razor
<ImageMap ImageUrl="/images/map.png"
          AlternateText="Interactive Map"
          HotSpotMode="HotSpotMode.Navigate"
          OnClick="HandleClick"
          HotSpots="@mixedSpots" />

@code {
    private List<HotSpot> mixedSpots = new List<HotSpot>
    {
        new RectangleHotSpot
        {
            Left = 0, Top = 0, Right = 100, Bottom = 50,
            NavigateUrl = "/page1",
            AlternateText = "Navigate to Page 1"
        },
        new CircleHotSpot
        {
            X = 200, Y = 100, Radius = 30,
            HotSpotMode = HotSpotMode.PostBack,
            PostBackValue = "circle-clicked",
            AlternateText = "Click for action"
        },
        new RectangleHotSpot
        {
            Left = 300, Top = 0, Right = 400, Bottom = 50,
            HotSpotMode = HotSpotMode.Inactive,
            AlternateText = "Disabled region"
        }
    };

    void HandleClick(ImageMapEventArgs args)
    {
        // Only PostBack hot spots trigger this event
    }
}
```

### Navigation with Target Window

```razor
<ImageMap ImageUrl="/images/partners.png"
          AlternateText="Our Partners"
          HotSpotMode="HotSpotMode.Navigate"
          Target="_blank"
          HotSpots="@partnerSpots" />

@code {
    private List<HotSpot> partnerSpots = new List<HotSpot>
    {
        new RectangleHotSpot
        {
            Left = 0, Top = 0, Right = 150, Bottom = 80,
            NavigateUrl = "https://partner1.example.com",
            AlternateText = "Partner 1"
        },
        new RectangleHotSpot
        {
            Left = 160, Top = 0, Right = 310, Bottom = 80,
            NavigateUrl = "https://partner2.example.com",
            AlternateText = "Partner 2",
            Target = "_self"  // Override: open in same window
        }
    };
}
```

## HTML Output

**Blazor Input:**
```razor
<ImageMap ImageUrl="/images/map.png"
          AlternateText="Site Map"
          HotSpotMode="HotSpotMode.Navigate"
          HotSpots="@spots" />
```

**Rendered HTML:**
```html
<img src="/images/map.png" usemap="#ImageMap_1" alt="Site Map" />

<map name="ImageMap_1">
    <area shape="rect" coords="0,0,200,150" href="/page1" alt="Page 1" />
    <area shape="circle" coords="300,200,50" href="/page2" alt="Page 2" />
    <area shape="poly" coords="10,10,100,10,55,80" href="/page3" alt="Page 3" />
</map>
```

## HotSpotMode Reference

| Mode | Behavior |
|------|----------|
| `NotSet` | Inherits the mode from the parent `ImageMap.HotSpotMode` |
| `Navigate` | Navigates to the URL specified in `NavigateUrl` |
| `PostBack` | Raises the `OnClick` event with `ImageMapEventArgs` containing the `PostBackValue` |
| `Inactive` | Renders the area with `nohref`; no click action |

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** — Change `<asp:ImageMap>` to `<ImageMap>` and `<asp:RectangleHotSpot>` to C# objects
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Convert child elements to C# objects** — Web Forms used declarative child elements for hot spots. In Blazor, create `List<HotSpot>` in your `@code` block
4. **Update event handler syntax** — Change `OnClick="ImageMap1_Click"` to `OnClick="HandleClick"`
5. **Event args change** — The handler receives `ImageMapEventArgs` directly (no `object sender` parameter)

### Before (Web Forms)

```html
<asp:ImageMap ID="imgMap1" 
              ImageUrl="~/images/map.png"
              HotSpotMode="PostBack"
              OnClick="imgMap1_Click"
              runat="server">
    <asp:RectangleHotSpot 
        Top="0" Left="0" Bottom="100" Right="200"
        PostBackValue="region1"
        AlternateText="Region 1" />
    <asp:CircleHotSpot 
        X="300" Y="150" Radius="50"
        PostBackValue="region2"
        AlternateText="Region 2" />
</asp:ImageMap>
```

```csharp
protected void imgMap1_Click(object sender, ImageMapEventArgs e)
{
    string region = e.PostBackValue;
    lblResult.Text = "You clicked: " + region;
}
```

### After (Blazor)

```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums

<ImageMap ImageUrl="/images/map.png"
          HotSpotMode="HotSpotMode.PostBack"
          OnClick="HandleClick"
          HotSpots="@hotSpots" />

<Label Text="@resultText" />

@code {
    private string resultText = "";

    private List<HotSpot> hotSpots = new List<HotSpot>
    {
        new RectangleHotSpot
        {
            Top = 0, Left = 0, Bottom = 100, Right = 200,
            PostBackValue = "region1",
            AlternateText = "Region 1"
        },
        new CircleHotSpot
        {
            X = 300, Y = 150, Radius = 50,
            PostBackValue = "region2",
            AlternateText = "Region 2"
        }
    };

    void HandleClick(ImageMapEventArgs args)
    {
        resultText = "You clicked: " + args.PostBackValue;
    }
}
```

!!! note "Declarative vs. Programmatic Hot Spots"
    In Web Forms, hot spots were declared as child elements in markup. In Blazor, hot spots are defined as C# objects in a `List<HotSpot>` and passed via the `HotSpots` parameter. This is a structural difference, but the rendered HTML output is identical.

## See Also

- [Image](../EditorControls/Image.md) — The basic Image component
- [ImageButton](../EditorControls/ImageButton.md) — A clickable image button
- [Microsoft Docs: ImageMap Control](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagemap?view=netframework-4.8)
