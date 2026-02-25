The AdRotator component is meant to emulate the `asp:AdRotator` control in markup and is defined in the [System.Web.UI.WebControls.AdRotator class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.adrotator?view=netframework-4.8)

## Blazor Features Supported

### XML File-Based Advertisement
- `AdvertisementFile` - The path to an XML file that contains advertisement information.
- `Target` - The name of the browser window or frame that displays the contents of the Web page linked to when the AdRotator component is clicked.
- `KeywordFilter` - Filter advertisements by keyword.
- `AlternateTextField` - The field name in the data source that provides alternate text for the ad image (default: "AlternateText").
- `ImageUrlField` - The field name in the data source that provides the image URL (default: "ImageUrl").
- `NavigateUrlField` - The field name in the data source that provides the navigation URL (default: "NavigateUrl").

### Data Binding
- `DataSource` - Bind to a collection of `Advertisment` objects programmatically. When using DataSource, you can provide a `List<Advertisment>` or other collection types.
- `DataMember` - Specifies the data table within a DataSet to bind to when using complex data sources.
- `OnDataBound` - Event fired after the data has been bound to the control.

### Events
- `OnAdCreated` - Event fired when an advertisement is created, allowing you to modify ad properties before rendering.

### Styling
- Standard Web Forms styling properties: `BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Font`, `Height`, `Width`.

## Usage Examples

### Using XML Advertisement File

```html
<AdRotator AdvertisementFile="Ads.xml" Target="_blank" />
```

XML File Format (Ads.xml):
```xml
<Advertisements>
  <Ad>
    <ImageUrl>~/images/ad1.jpg</ImageUrl>
    <NavigateUrl>http://www.example1.com</NavigateUrl>
    <AlternateText>Visit Example 1</AlternateText>
    <Width>468</Width>
    <Height>60</Height>
    <Keyword>technology</Keyword>
    <Impressions>80</Impressions>
  </Ad>
  <Ad>
    <ImageUrl>~/images/ad2.jpg</ImageUrl>
    <NavigateUrl>http://www.example2.com</NavigateUrl>
    <AlternateText>Visit Example 2</AlternateText>
    <Width>468</Width>
    <Height>60</Height>
    <Keyword>sports</Keyword>
    <Impressions>20</Impressions>
  </Ad>
</Advertisements>
```

### Using DataSource with List of Advertisements

```razor
@code {
    private List<Advertisment> _ads = new()
    {
        new Advertisment
        {
            ImageUrl = "/images/tech-ad.jpg",
            NavigateUrl = "http://www.techsite.com",
            AlternateText = "Visit Tech Site",
            Width = "468",
            Height = "60",
            Keyword = "technology"
        },
        new Advertisment
        {
            ImageUrl = "/images/sports-ad.jpg",
            NavigateUrl = "http://www.sportssite.com",
            AlternateText = "Visit Sports Site",
            Width = "468",
            Height = "60",
            Keyword = "sports"
        }
    };
}

<AdRotator DataSource="@_ads" Target="_blank" />
```

### Using KeywordFilter

Filter advertisements by keyword:

```html
<AdRotator AdvertisementFile="Ads.xml" KeywordFilter="technology" Target="_blank" />
```

Or with DataSource:

```html
<AdRotator DataSource="@_ads" KeywordFilter="technology" Target="_blank" />
```

### Handling OnAdCreated Event

Modify advertisement properties before rendering:

```razor
<AdRotator AdvertisementFile="Ads.xml" OnAdCreated="HandleAdCreated" />

@code {
    private void HandleAdCreated(AdCreatedEventArgs e)
    {
        // Modify the ad before it's rendered
        e.AlternateText = $"Special Offer: {e.AlternateText}";
        
        // Access all ad properties
        var imageUrl = e.ImageUrl;
        var navigateUrl = e.NavigateUrl;
        
        // Access additional properties via AdProperties dictionary
        var keyword = e.AdProperties["Keyword"];
    }
}
```

### Handling OnDataBound Event

Execute code after data binding:

```razor
<AdRotator DataSource="@_ads" OnDataBound="HandleDataBound" />

@code {
    private void HandleDataBound(EventArgs e)
    {
        // Perform actions after data is bound
        Console.WriteLine("AdRotator data bound successfully");
    }
}
```

### Using with Styling Properties

```html
<AdRotator 
    AdvertisementFile="Ads.xml" 
    Target="_blank"
    CssClass="ad-banner"
    BorderWidth="1px"
    BorderColor="#cccccc"
    BorderStyle="Solid" />
```

## Notes

- When both `DataSource` and `AdvertisementFile` are specified, `DataSource` takes priority.
- The AdRotator randomly selects an advertisement from the available pool each time it renders.
- The `Impressions` field in the XML affects the probability of an ad being shown (ads with higher impression values appear more frequently).
- Custom field names can be specified using `AlternateTextField`, `ImageUrlField`, and `NavigateUrlField` properties.

## Web Forms Features NOT Supported

- `DataSourceID` - DataSource controls are not used in Blazor. Use the `DataSource` property instead to bind data programmatically.
- Lifecycle events like `OnInit`, `OnLoad`, `OnPreRender`, `OnUnload` - These are part of the Web Forms page lifecycle and don't apply to Blazor components.
- `EnableTheming` / `SkinID` - Blazor uses CSS for theming.
- `EnableViewState` - Blazor doesn't use ViewState.

## Web Forms Declarative Syntax

```html
<asp:AdRotator
    AccessKey="string"
    AdvertisementFile="uri"
    AlternateTextField="string"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    DataMember="string"
    DataSource="string"
    DataSourceID="string"
    Enabled="True|False"
    EnableTheming="True|False"
    EnableViewState="True|False"
    ForeColor="color name|#dddddd"
    Height="size"
    ID="string"
    ImageUrlField="string"
    KeywordFilter="string"
    NavigateUrlField="string"
    OnAdCreated="AdCreated event handler"
    OnDataBinding="DataBinding event handler"
    OnDataBound="DataBound event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    runat="server"
    SkinID="string"
    Style="string"
    TabIndex="integer"
    Target="string|_blank|_parent|_search|_self|_top"
    ToolTip="string"
    Visible="True|False"
    Width="size"
/>
```
