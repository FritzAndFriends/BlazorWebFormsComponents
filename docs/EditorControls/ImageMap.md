# ImageMap

The **ImageMap** component enables you to create interactive images with clickable regions (hot spots) that can navigate to URLs or trigger server-side events. This is useful for creating image-based navigation, interactive diagrams, or clickable floor plans.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagemap?view=netframework-4.8

## Features Supported in Blazor

- **HotSpot Types** - Supports three types of clickable regions:
  - `RectangleHotSpot` - Rectangular regions defined by left, top, right, bottom coordinates
  - `CircleHotSpot` - Circular regions defined by center point (X, Y) and radius
  - `PolygonHotSpot` - Irregular regions defined by a series of coordinate pairs
- **HotSpot Modes** - Three interaction behaviors:
  - `Navigate` - Redirects to a URL when clicked
  - `PostBack` - Triggers a server-side Click event with PostBackValue
  - `Inactive` - No action (displays as non-clickable region)
- **ImageUrl** - Path to the image to display
- **AlternateText** - Alt text for accessibility
- **ToolTip** - Hover tooltip text
- **DescriptionUrl** - URL to detailed image description for accessibility
- **GenerateEmptyAlternateText** - Creates empty alt attribute for decorative images
- **ImageAlign** - Alignment relative to other page elements
- **Target** - Default target window for navigation (_blank, _self, etc.)
- **OnClick** - Event raised when a PostBack mode HotSpot is clicked
- **Visible** - Controls component visibility

### Blazor Notes

- Each HotSpot can override the ImageMap's default HotSpotMode
- HotSpots can have individual Target values that override the ImageMap's default
- The component renders standard HTML `<img>` and `<map>` elements with `<area>` tags
- PostBack mode uses Blazor's event handling rather than actual postbacks
- Navigation mode generates standard hyperlinks that work without JavaScript

## Web Forms Features NOT Supported

- **AccessKey** - Not implemented; use standard HTML attribute if needed
- **TabIndex** - Not implemented on ImageMap itself; HotSpots support TabIndex
- **Style Properties** - BackColor, ForeColor, BorderColor, CssClass, Height, Width not yet implemented
- **EnableTheming/SkinID** - ASP.NET theming not available in Blazor
- **ViewState** - Not needed; Blazor manages state differently
- **Lifecycle Events** - OnDataBinding, OnInit, etc. not supported; use Blazor lifecycle methods

## Web Forms Declarative Syntax

```aspx
<asp:ImageMap 
    ID="ImageMap1"
    ImageUrl="~/images/navigation.jpg"
    HotSpotMode="Navigate|PostBack|Inactive"
    Target="_blank"
    AlternateText="Navigation Map"
    OnClick="ImageMap1_Click"
    runat="server">
    
    <asp:RectangleHotSpot 
        Left="10" Top="10" Right="110" Bottom="60"
        AlternateText="Home"
        NavigateUrl="~/Default.aspx"
        HotSpotMode="Navigate" />
        
    <asp:CircleHotSpot 
        X="200" Y="35" Radius="25"
        AlternateText="Products"
        PostBackValue="Products"
        HotSpotMode="PostBack" />
        
    <asp:PolygonHotSpot 
        Coordinates="300,10,350,10,325,50"
        AlternateText="Contact"
        NavigateUrl="~/Contact.aspx" />
</asp:ImageMap>
```

## Blazor Syntax

```razor
<ImageMap 
    ImageUrl="/images/navigation.jpg"
    HotSpotMode="HotSpotMode.Navigate"
    Target="_blank"
    AlternateText="Navigation Map"
    OnClick="HandleMapClick"
    HotSpots="@hotSpotList">
</ImageMap>

@code {
    private List<HotSpot> hotSpotList = new()
    {
        new RectangleHotSpot 
        { 
            Left = 10, Top = 10, Right = 110, Bottom = 60,
            AlternateText = "Home",
            NavigateUrl = "/",
            HotSpotMode = HotSpotMode.Navigate
        },
        new CircleHotSpot 
        { 
            X = 200, Y = 35, Radius = 25,
            AlternateText = "Products",
            PostBackValue = "Products",
            HotSpotMode = HotSpotMode.PostBack
        },
        new PolygonHotSpot 
        { 
            Coordinates = "300,10,350,10,325,50",
            AlternateText = "Contact",
            NavigateUrl = "/contact"
        }
    };
    
    private void HandleMapClick(ImageMapEventArgs e)
    {
        // Handle postback - e.PostBackValue contains the clicked HotSpot's value
        var clickedArea = e.PostBackValue;
    }
}
```

## Usage Notes

### HotSpot Mode Precedence

When a HotSpot doesn't specify its own `HotSpotMode`, it inherits from the ImageMap's default mode. This allows you to set a common behavior for all regions while overriding specific ones:

```razor
<ImageMap HotSpotMode="HotSpotMode.Navigate" HotSpots="@regions" />

@code {
    private List<HotSpot> regions = new()
    {
        // This uses Navigate mode (inherited from ImageMap)
        new RectangleHotSpot { ..., NavigateUrl = "/page1" },
        
        // This overrides to use PostBack mode
        new CircleHotSpot 
        { 
            ..., 
            HotSpotMode = HotSpotMode.PostBack,
            PostBackValue = "SpecialAction"
        }
    };
}
```

### Coordinate Systems

- **RectangleHotSpot**: Coordinates are in pixels from top-left corner of image
  - Left/Top = top-left corner, Right/Bottom = bottom-right corner
- **CircleHotSpot**: X,Y is center point, Radius is distance from center
- **PolygonHotSpot**: Comma-separated pairs of X,Y coordinates defining vertices

### Accessibility Considerations

Always provide meaningful `AlternateText` for each HotSpot to ensure screen readers can describe the clickable regions:

```csharp
new RectangleHotSpot 
{ 
    Left = 0, Top = 0, Right = 100, Bottom = 50,
    AlternateText = "Navigate to Products page",  // Descriptive text
    NavigateUrl = "/products"
}
```

## Examples

### Basic Navigation Map

```razor
@page "/image-map-demo"

<h2>Site Navigation</h2>

<ImageMap 
    ImageUrl="/images/site-map.jpg"
    AlternateText="Site navigation map"
    HotSpots="@navigationRegions" />

@code {
    private List<HotSpot> navigationRegions = new()
    {
        new RectangleHotSpot 
        { 
            Left = 20, Top = 50, Right = 180, Bottom = 120,
            AlternateText = "Home Page",
            NavigateUrl = "/"
        },
        new RectangleHotSpot 
        { 
            Left = 200, Top = 50, Right = 360, Bottom = 120,
            AlternateText = "Products",
            NavigateUrl = "/products"
        },
        new RectangleHotSpot 
        { 
            Left = 380, Top = 50, Right = 540, Bottom = 120,
            AlternateText = "Contact Us",
            NavigateUrl = "/contact"
        }
    };
}
```

### Interactive Diagram with PostBack

```razor
@page "/floor-plan"

<h2>Office Floor Plan</h2>
<p>Click on a room to see details</p>

<ImageMap 
    ImageUrl="/images/floor-plan.png"
    HotSpotMode="HotSpotMode.PostBack"
    OnClick="ShowRoomDetails"
    HotSpots="@roomRegions" />

@if (!string.IsNullOrEmpty(selectedRoom))
{
    <div class="room-info">
        <h3>@selectedRoom</h3>
        <p>@roomDetails</p>
    </div>
}

@code {
    private string selectedRoom = "";
    private string roomDetails = "";
    
    private List<HotSpot> roomRegions = new()
    {
        new PolygonHotSpot 
        { 
            Coordinates = "50,50,150,50,150,150,50,150",
            AlternateText = "Conference Room A",
            PostBackValue = "ConfA"
        },
        new CircleHotSpot 
        { 
            X = 300, Y = 100, Radius = 40,
            AlternateText = "Break Room",
            PostBackValue = "Break"
        }
    };
    
    private void ShowRoomDetails(ImageMapEventArgs e)
    {
        selectedRoom = e.PostBackValue switch
        {
            "ConfA" => "Conference Room A",
            "Break" => "Break Room",
            _ => "Unknown Room"
        };
        
        roomDetails = e.PostBackValue switch
        {
            "ConfA" => "Capacity: 12 people. Equipped with projector and whiteboard.",
            "Break" => "Kitchen facilities, coffee maker, and seating for 8.",
            _ => ""
        };
    }
}
```

### Mixed Mode Map

```razor
<ImageMap 
    ImageUrl="/images/product-diagram.jpg"
    HotSpotMode="HotSpotMode.Navigate"
    HotSpots="@mixedRegions"
    OnClick="HandleAction" />

@code {
    private List<HotSpot> mixedRegions = new()
    {
        // External link - opens in new window
        new RectangleHotSpot 
        { 
            Left = 10, Top = 10, Right = 100, Bottom = 60,
            AlternateText = "Documentation",
            NavigateUrl = "https://docs.example.com",
            Target = "_blank",
            HotSpotMode = HotSpotMode.Navigate
        },
        
        // Server action - triggers event
        new CircleHotSpot 
        { 
            X = 150, Y = 35, Radius = 25,
            AlternateText = "Download",
            PostBackValue = "StartDownload",
            HotSpotMode = HotSpotMode.PostBack
        },
        
        // Inactive region - informational only
        new PolygonHotSpot 
        { 
            Coordinates = "200,10,250,10,225,50",
            AlternateText = "Coming Soon",
            HotSpotMode = HotSpotMode.Inactive
        }
    };
    
    private void HandleAction(ImageMapEventArgs e)
    {
        if (e.PostBackValue == "StartDownload")
        {
            // Initiate download logic
        }
    }
}
```

## Migration Tips

1. **Convert Declarative HotSpots to Collection**: In Web Forms, HotSpots are declared as child elements. In Blazor, create a List<HotSpot> in your @code block.

2. **Event Handler Signature**: Web Forms uses `ImageMapEventHandler` with sender and `ImageMapEventArgs`. Blazor simplifies this - you only need the `ImageMapEventArgs` parameter.

3. **Coordinate Validation**: Consider validating HotSpot coordinates are within image bounds to prevent rendering issues.

4. **Dynamic HotSpots**: In Blazor, you can easily add/remove HotSpots dynamically by modifying the List<HotSpot> and calling `StateHasChanged()`.

## See Also

- [Image](Image.md) - Display static images
- [ImageButton](ImageButton.md) - Clickable image that acts as a button
- HyperLink - Text or image hyperlinks (not yet implemented)
