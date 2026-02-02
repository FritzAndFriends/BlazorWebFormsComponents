# BulletedList

The BulletedList component renders items as a bulleted or numbered list. It supports multiple bullet styles, display modes (text, hyperlink, or clickable link buttons), and both static items and data-bound scenarios.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.bulletedlist?view=netframework-4.8

## Blazor Features Supported

- Static items via `StaticItems` parameter with `ListItemCollection`
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Multiple bullet styles: Disc, Circle, Square, Numbered, LowerAlpha, UpperAlpha, LowerRoman, UpperRoman, CustomImage
- Display modes: Text (default), HyperLink, LinkButton
- `FirstBulletNumber` for starting ordered lists at a specific number
- `BulletImageUrl` for custom bullet images
- `Target` attribute for hyperlinks (e.g., "_blank")
- `OnClick` event handler for LinkButton mode
- Disabled state via `Enabled` parameter (affects entire control or individual items)
- Style attributes (BackColor, ForeColor, Font, BorderStyle, etc.) and CssClass formatting

## WebForms Features Not Supported

- **AutoPostBack** - Not supported in Blazor; use `OnClick` event for LinkButton mode
- **DataSourceID** - Not supported; bind directly to collections via `Items` parameter

## WebForms Syntax

```html
<asp:BulletedList
    ID="string"
    runat="server"
    BulletImageUrl="uri"
    BulletStyle="NotSet|Numbered|LowerAlpha|UpperAlpha|LowerRoman|UpperRoman|Disc|Circle|Square|CustomImage"
    DataSourceID="string"
    DataTextField="string"
    DataValueField="string"
    DisplayMode="Text|HyperLink|LinkButton"
    Enabled="True|False"
    FirstBulletNumber="integer"
    Target="string|_blank|_parent|_search|_self|_top"
    Visible="True|False"
    OnClick="Click event handler">
    
    <asp:ListItem Value="value1" Text="Item 1" />
    <asp:ListItem Value="http://example.com" Text="Link" />
    
</asp:BulletedList>
```

## Blazor Syntax

### Static Items (Text Mode)

```razor
<BulletedList TItem="object" StaticItems="items" />

@code {
    private ListItemCollection items = new()
    {
        new ListItem("First item", "1"),
        new ListItem("Second item", "2"),
        new ListItem("Third item", "3")
    };
}
```

### Data Binding

```razor
<BulletedList TItem="Product"
              Items="products"
              DataTextField="Name"
              DataValueField="Url"
              DisplayMode="BulletedListDisplayMode.HyperLink" />

@code {
    private List<Product> products = new()
    {
        new Product { Name = "Laptop", Url = "/products/laptop" },
        new Product { Name = "Phone", Url = "/products/phone" },
        new Product { Name = "Tablet", Url = "/products/tablet" }
    };
    
    public class Product
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
```

### HyperLink Mode with Target

```razor
<BulletedList TItem="object"
              StaticItems="links"
              DisplayMode="BulletedListDisplayMode.HyperLink"
              Target="_blank" />

@code {
    private ListItemCollection links = new()
    {
        new ListItem("Microsoft", "https://microsoft.com"),
        new ListItem("GitHub", "https://github.com"),
        new ListItem("Azure", "https://azure.com")
    };
}
```

### LinkButton Mode with Click Handler

```razor
<BulletedList TItem="object"
              StaticItems="items"
              DisplayMode="BulletedListDisplayMode.LinkButton"
              OnClick="HandleItemClick" />

<p>@message</p>

@code {
    private string message = "";
    
    private ListItemCollection items = new()
    {
        new ListItem("Option A", "a"),
        new ListItem("Option B", "b"),
        new ListItem("Option C", "c")
    };
    
    private void HandleItemClick(BulletedListEventArgs e)
    {
        message = $"You clicked item at index {e.Index}";
    }
}
```

### Numbered List Starting at 5

```razor
<BulletedList TItem="object"
              StaticItems="items"
              BulletStyle="BulletStyle.Numbered"
              FirstBulletNumber="5" />
```

### Custom Bullet Image

```razor
<BulletedList TItem="object"
              StaticItems="items"
              BulletStyle="BulletStyle.CustomImage"
              BulletImageUrl="/images/custom-bullet.png" />
```

### With Styling

```razor
<BulletedList TItem="object"
              StaticItems="items"
              CssClass="my-list"
              BackColor="WebColor.LightGray"
              ForeColor="WebColor.DarkBlue"
              Width="Unit.Pixel(300)" />
```

## Bullet Styles

| BulletStyle | List Type | CSS/HTML |
|-------------|-----------|----------|
| `NotSet` | Unordered | Browser default |
| `Disc` | Unordered | Filled circle (●) |
| `Circle` | Unordered | Empty circle (○) |
| `Square` | Unordered | Filled square (■) |
| `Numbered` | Ordered | 1, 2, 3... |
| `LowerAlpha` | Ordered | a, b, c... |
| `UpperAlpha` | Ordered | A, B, C... |
| `LowerRoman` | Ordered | i, ii, iii... |
| `UpperRoman` | Ordered | I, II, III... |
| `CustomImage` | Unordered | Custom image via `BulletImageUrl` |

## Display Modes

| DisplayMode | Description | Rendered As |
|-------------|-------------|-------------|
| `Text` | Plain text items (default) | `<span>` |
| `HyperLink` | Navigable links using Value as URL | `<a href="...">` |
| `LinkButton` | Clickable items that fire OnClick event | `<a href="javascript:void(0)">` |

## HTML Output

### Unordered List (Disc Style)

```html
<ul id="myList" style="list-style-type: disc;">
    <li><span>First item</span></li>
    <li><span>Second item</span></li>
</ul>
```

### Ordered List (Numbered)

```html
<ol id="myList" type="1" start="1" style="list-style-type: decimal;">
    <li><span>First item</span></li>
    <li><span>Second item</span></li>
</ol>
```

### HyperLink Mode

```html
<ul id="myList">
    <li><a href="https://example.com" target="_blank">Link Text</a></li>
    <li><a href="https://other.com" target="_blank">Another Link</a></li>
</ul>
```

### LinkButton Mode

```html
<ul id="myList">
    <li><a href="javascript:void(0)">Clickable Item 1</a></li>
    <li><a href="javascript:void(0)">Clickable Item 2</a></li>
</ul>
```

## Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StaticItems` | `ListItemCollection` | empty | Static list items |
| `Items` | `IEnumerable<TItem>` | null | Data-bound items |
| `DataTextField` | `string` | null | Property for display text |
| `DataValueField` | `string` | null | Property for value (used as URL in HyperLink mode) |
| `BulletStyle` | `BulletStyle` | NotSet | Style of bullets or numbers |
| `BulletImageUrl` | `string` | null | URL of custom bullet image |
| `DisplayMode` | `BulletedListDisplayMode` | Text | How items are rendered |
| `FirstBulletNumber` | `int` | 1 | Starting number for ordered lists |
| `Target` | `string` | null | Target window for hyperlinks |
| `OnClick` | `EventCallback<BulletedListEventArgs>` | - | Click handler for LinkButton mode |

## Key Differences from Web Forms

1. **Type Parameter**: Blazor BulletedList requires a `TItem` type parameter for data binding
2. **Property Names**: Use `StaticItems` for the item collection (not `Items`), as `Items` is reserved for data-bound scenarios
3. **Event Handling**: Uses `OnClick` with `EventCallback<BulletedListEventArgs>` instead of server-side postback
4. **Enum References**: Use `BulletStyle.Numbered` and `BulletedListDisplayMode.HyperLink` syntax
5. **No AutoPostBack**: Blazor's event model is immediate; events fire without postback

## BulletedListEventArgs

The `OnClick` event provides a `BulletedListEventArgs` object with:

| Property | Type | Description |
|----------|------|-------------|
| `Index` | `int` | Zero-based index of the clicked item |

```csharp
private void HandleClick(BulletedListEventArgs e)
{
    var clickedIndex = e.Index;
    var clickedItem = items[clickedIndex];
    Console.WriteLine($"Clicked: {clickedItem.Text}");
}
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. Remove the `asp:` prefix and `runat="server"` attribute
2. Add the `TItem="object"` type parameter (or your specific data type)
3. Remove `<asp:ListItem>` tags and define items in code-behind as `ListItemCollection`
4. For LinkButton mode, add an `OnClick` handler
5. Update enum references to Blazor syntax (e.g., `BulletStyle.Numbered`)

### Before (Web Forms):

```html
<asp:BulletedList ID="blLinks" 
                  runat="server"
                  BulletStyle="Numbered"
                  DisplayMode="HyperLink"
                  Target="_blank">
    <asp:ListItem Value="https://microsoft.com" Text="Microsoft" />
    <asp:ListItem Value="https://github.com" Text="GitHub" />
    <asp:ListItem Value="https://azure.com" Text="Azure" />
</asp:BulletedList>
```

### After (Blazor):

```razor
<BulletedList TItem="object"
              StaticItems="links"
              BulletStyle="BulletStyle.Numbered"
              DisplayMode="BulletedListDisplayMode.HyperLink"
              Target="_blank" />

@code {
    private ListItemCollection links = new()
    {
        new ListItem("Microsoft", "https://microsoft.com"),
        new ListItem("GitHub", "https://github.com"),
        new ListItem("Azure", "https://azure.com")
    };
}
```

## See Also

- [CheckBoxList](CheckBoxList.md) - Multi-select checkbox group
- [RadioButtonList](RadioButtonList.md) - Single-select radio button group
- [ListBox](ListBox.md) - Multi-select list control
