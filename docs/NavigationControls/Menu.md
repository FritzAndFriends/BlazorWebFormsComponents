# Menu

The Menu component is meant to emulate the asp:Menu control in markup and is defined in the [System.Web.UI.WebControls.Menu class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.menu?view=netframework-4.8)

[Usage Notes](#usage-notes) | [Web Forms Syntax](#web-forms-declarative-syntax) | [Blazor Syntax](#blazor-syntax)

## Features supported in Blazor

- Simple static menu, as shown in the [ASP.NET example](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.menu?view=netframework-4.8#examples)
- Simple databinding to a Sitemap as shown in the [ASP.NET example](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.menu?view=netframework-4.8#examples)
- **Selection and Events** - `MenuItemClick` event, `SelectedItem`, `SelectedValue`
- **Orientation** - `Horizontal` or `Vertical` layout
- **Style Sub-Components** - `StaticMenuItemStyle`, `DynamicMenuItemStyle`, `StaticHoverStyle`, `DynamicHoverStyle`, `DynamicSelectedStyle`, `DynamicMenuStyle`
- **Navigation Properties** - `Target`, `MaximumDynamicDisplayLevels`, `SkipLinkText`

##### [Back to top](#menu)

## Usage Notes

- Databinding only works with sitemap.  Sitemap must be loaded and set as an XML document on DataSource

##### [Back to top](#menu)

## Web Forms Declarative Syntax

```html
<asp:Menu
    AccessKey="string"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    DataSource="string"
    DataSourceID="string"
    DisappearAfter="integer"
    DynamicBottomSeparatorImageUrl="uri"
    DynamicEnableDefaultPopOutImage="True|False"
    DynamicHorizontalOffset="integer"
    DynamicItemFormatString="string"
    DynamicPopOutImageTextFormatString="string"
    DynamicPopOutImageUrl="uri"
    DynamicTopSeparatorImageUrl="uri"
    DynamicVerticalOffset="integer"
    Enabled="True|False"
    EnableTheming="True|False"
    EnableViewState="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Overline="True|False"
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|
        Large|X-Large|XX-Large"
    Font-Strikeout="True|False"
    Font-Underline="True|False"
    ForeColor="color name|#dddddd"
    Height="size"
    ID="string"
    ItemWrap="True|False"
    MaximumDynamicDisplayLevels="integer"
    OnDataBinding="DataBinding event handler"
    OnDataBound="DataBound event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnMenuItemClick="MenuItemClick event handler"
    OnMenuItemDataBound="MenuItemDataBound event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    Orientation="Horizontal|Vertical"
    PathSeparator="string"
    runat="server"
    ScrollDownImageUrl="uri"
    ScrollDownText="string"
    ScrollUpImageUrl="uri"
    ScrollUpText="string"
    SkinID="string"
    SkipLinkText="string"
    StaticBottomSeparatorImageUrl="uri"
    StaticDisplayLevels="integer"
    StaticEnableDefaultPopOutImage="True|False"
    StaticItemFormatString="string"
    StaticPopOutImageTextFormatString="string"
    StaticPopOutImageUrl="uri"
    StaticSubMenuIndent="size"
    StaticTopSeparatorImageUrl="uri"
    Style="string"
    TabIndex="integer"
    Target="string"
    ToolTip="string"
    Visible="True|False"
    Width="size"
>
        <DataBindings>
                <asp:MenuItemBinding
                    DataMember="string"
                    Depth="integer"
                    Enabled="True|False"
                    EnabledField="string"
                    FormatString="string"
                    ImageUrl="uri"
                    ImageUrlField="string"
                    NavigateUrl="uri"
                    NavigateUrlField="string"
                    PopOutImageUrl="uri"
                    PopOutImageUrlField="string"
                    Selectable="True|False"
                    SelectableField="string"
                    SeparatorImageUrl="uri"
                    SeparatorImageUrlField="string"
                    Target="string"
                    TargetField="string"
                    Text="string"
                    TextField="string"
                    ToolTip="string"
                    ToolTipField="string"
                    Value="string"
                    ValueField="string"
                />
        </DataBindings>
        <DynamicHoverStyle />
        <DynamicItemTemplate>
            <!-- child controls -->
        </DynamicItemTemplate>
        <DynamicMenuItemStyle
            BackColor="color name|#dddddd"
            BorderColor="color name|#dddddd"
            BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|
                Groove|Ridge|Inset|Outset"
            BorderWidth="size"
            CssClass="string"
            Font-Bold="True|False"
            Font-Italic="True|False"
            Font-Names="string"
            Font-Overline="True|False"
            Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|
                Medium|Large|X-Large|XX-Large"
            Font-Strikeout="True|False"
            Font-Underline="True|False"
            ForeColor="color name|#dddddd"
            Height="size"
            HorizontalPadding="size"
            ItemSpacing="size"
            OnDisposed="Disposed event handler"
            VerticalPadding="size"
            Width="size"
        />
        <DynamicMenuStyle
            BackColor="color name|#dddddd"
            BorderColor="color name|#dddddd"
            BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|
                Groove|Ridge|Inset|Outset"
            BorderWidth="size"
            CssClass="string"
            Font-Bold="True|False"
            Font-Italic="True|False"
            Font-Names="string"
            Font-Overline="True|False"
            Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|
                Medium|Large|X-Large|XX-Large"
            Font-Strikeout="True|False"
            Font-Underline="True|False"
            ForeColor="color name|#dddddd"
            Height="size"
            HorizontalPadding="size"
            OnDisposed="Disposed event handler"
            VerticalPadding="size"
            Width="size"
        />
        <DynamicSelectedStyle
            BackColor="color name|#dddddd"
            BorderColor="color name|#dddddd"
            BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|
                Groove|Ridge|Inset|Outset"
            BorderWidth="size"
            CssClass="string"
            Font-Bold="True|False"
            Font-Italic="True|False"
            Font-Names="string"
            Font-Overline="True|False"
            Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|
                Medium|Large|X-Large|XX-Large"
            Font-Strikeout="True|False"
            Font-Underline="True|False"
            ForeColor="color name|#dddddd"
            Height="size"
            HorizontalPadding="size"
            ItemSpacing="size"
            OnDisposed="Disposed event handler"
            VerticalPadding="size"
            Width="size"
        />
        <Items />
        <LevelMenuItemStyles>
                <asp:MenuItemStyle
                    BackColor="color name|#dddddd"
                    BorderColor="color name|#dddddd"
                    BorderStyle="NotSet|None|Dotted|Dashed|Solid|
                        Double|Groove|Ridge|Inset|Outset"
                    BorderWidth="size"
                    CssClass="string"
                    Font-Bold="True|False"
                    Font-Italic="True|False"
                    Font-Names="string"
                    Font-Overline="True|False"
                    Font-Size="string|Smaller|Larger|XX-Small|
                        X-Small|Small|Medium|Large|X-Large|XX-Large"
                    Font-Strikeout="True|False"
                    Font-Underline="True|False"
                    ForeColor="color name|#dddddd"
                    Height="size"
                    HorizontalPadding="size"
                    ItemSpacing="size"
                    OnDisposed="Disposed event handler"
                    VerticalPadding="size"
                    Width="size"
                />
        </LevelMenuItemStyles>
        <LevelSelectedStyles>
                <asp:MenuItemStyle
                    BackColor="color name|#dddddd"
                    BorderColor="color name|#dddddd"
                    BorderStyle="NotSet|None|Dotted|Dashed|Solid|
                        Double|Groove|Ridge|Inset|Outset"
                    BorderWidth="size"
                    CssClass="string"
                    Font-Bold="True|False"
                    Font-Italic="True|False"
                    Font-Names="string"
                    Font-Overline="True|False"
                    Font-Size="string|Smaller|Larger|XX-Small|
                        X-Small|Small|Medium|Large|X-Large|XX-Large"
                    Font-Strikeout="True|False"
                    Font-Underline="True|False"
                    ForeColor="color name|#dddddd"
                    Height="size"
                    HorizontalPadding="size"
                    ItemSpacing="size"
                    OnDisposed="Disposed event handler"
                    VerticalPadding="size"
                    Width="size"
                />
        </LevelSelectedStyles>
        <LevelSubMenuStyles>
                <asp:SubMenuStyle
                    BackColor="color name|#dddddd"
                    BorderColor="color name|#dddddd"
                    BorderStyle="NotSet|None|Dotted|Dashed|Solid|
                        Double|Groove|Ridge|Inset|Outset"
                    BorderWidth="size"
                    CssClass="string"
                    Font-Bold="True|False"
                    Font-Italic="True|False"
                    Font-Names="string"
                    Font-Overline="True|False"
                    Font-Size="string|Smaller|Larger|XX-Small|
                        X-Small|Small|Medium|Large|X-Large|XX-Large"
                    Font-Strikeout="True|False"
                    Font-Underline="True|False"
                    ForeColor="color name|#dddddd"
                    Height="size"
                    HorizontalPadding="size"
                    OnDisposed="Disposed event handler"
                    VerticalPadding="size"
                    Width="size"
                />
        </LevelSubMenuStyles>
        <StaticHoverStyle />
        <StaticItemTemplate>
            <!-- child controls -->
        </StaticItemTemplate>
        <StaticMenuItemStyle
            BackColor="color name|#dddddd"
            BorderColor="color name|#dddddd"
            BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|
                Groove|Ridge|Inset|Outset"
            BorderWidth="size"
            CssClass="string"
            Font-Bold="True|False"
            Font-Italic="True|False"
            Font-Names="string"
            Font-Overline="True|False"
            Font-Size="string|Smaller|Larger|XX-Small|
                X-Small|Small|Medium|Large|X-Large|XX-Large"
            Font-Strikeout="True|False"
            Font-Underline="True|False"
            ForeColor="color name|#dddddd"
            Height="size"
            HorizontalPadding="size"
            ItemSpacing="size"
            OnDisposed="Disposed event handler"
            VerticalPadding="size"
            Width="size"
        />
        <StaticMenuStyle
            BackColor="color name|#dddddd"
            BorderColor="color name|#dddddd"
            BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|
                Groove|Ridge|Inset|Outset"
            BorderWidth="size"
            CssClass="string"
            Font-Bold="True|False"
            Font-Italic="True|False"
            Font-Names="string"
            Font-Overline="True|False"
            Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|
                Medium|Large|X-Large|XX-Large"
            Font-Strikeout="True|False"
            Font-Underline="True|False"
            ForeColor="color name|#dddddd"
            Height="size"
            HorizontalPadding="size"
            OnDisposed="Disposed event handler"
            VerticalPadding="size"
            Width="size"
        />
        <StaticSelectedStyle
            BackColor="color name|#dddddd"
            BorderColor="color name|#dddddd"
            BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|
                Groove|Ridge|Inset|Outset"
            BorderWidth="size"
            CssClass="string"
            Font-Bold="True|False"
            Font-Italic="True|False"
            Font-Names="string"
            Font-Overline="True|False"
            Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|
                Medium|Large|X-Large|XX-Large"
            Font-Strikeout="True|False"
            Font-Underline="True|False"
            ForeColor="color name|#dddddd"
            Height="size"
            HorizontalPadding="size"
            ItemSpacing="size"
            OnDisposed="Disposed event handler"
            VerticalPadding="size"
            Width="size"
        />
</asp:Menu>
```

##### [Back to top](#menu)

## Blazor Syntax

```html
<Menu id="NavigationMenu"
      DisappearAfter="2000"
      StaticDisplayLevels="2"
      StaticSubmenuIndent="10"
      orientation="Vertical"
      font-names="Arial"
      target="_blank">

    <StaticMenuItemStyle BackColor="@("LightSteelBlue")"
                         ForeColor="@("Black")" />
    <StaticHoverStyle BackColor="WebColor.LightSkyBlue" />
    <DynamicMenuItemStyle BackColor="WebColor.Black"
                          ForeColor="WebColor.Silver" />
    <DynamicHoverStyle BackColor="WebColor.LightSkyBlue"
                       ForeColor="WebColor.Black" />
    <DynamicSelectedStyle BackColor="WebColor.PapayaWhip"
                          ForeColor="WebColor.SteelBlue" />

    <Items>
        <MenuItem navigateurl="Home.aspx"
                  text="Home"
                  tooltip="Home">
            <MenuItem text="Music" tooltip="Music">
                <MenuItem navigateurl="Classical.aspx"
                          text="Classical"
                          tooltip="Classical" />
                <MenuItem navigateurl="Rock.aspx"
                          text="Rock"
                          tooltip="Rock" />
                <MenuItem navigateurl="Jazz.aspx"
                          text="Jazz"
                          tooltip="Jazz" />
            </MenuItem>
            <MenuItem navigateurl="Movies.aspx" text="Movies" tooltip="Movies">
                <MenuItem navigateurl="Action.aspx"
                          text="Action"
                          tooltip="Action" />
                <MenuItem navigateurl="Drama.aspx"
                          text="Drama"
                          tooltip="Drama" />
                <MenuItem navigateurl="Musical.aspx"
                          text="Musical"
                          tooltip="Musical" />
            </MenuItem>
        </MenuItem>
    </Items>

</Menu>
```

## Examples

### Basic Menu with Click Events

```razor
<Menu Orientation="Vertical"
      MenuItemClick="HandleMenuClick">
    <Items>
        <MenuItem Text="Home" Value="home" NavigateUrl="/" />
        <MenuItem Text="Products" Value="products">
            <MenuItem Text="Electronics" Value="electronics" NavigateUrl="/electronics" />
            <MenuItem Text="Clothing" Value="clothing" NavigateUrl="/clothing" />
        </MenuItem>
        <MenuItem Text="About" Value="about" NavigateUrl="/about" />
    </Items>
</Menu>

<p>Clicked: @clickedItem</p>

@code {
    private string clickedItem = "";

    private void HandleMenuClick(MenuEventArgs e)
    {
        clickedItem = e.Item.Text;
    }
}
```

### Horizontal Menu

```razor
<Menu Orientation="Horizontal"
      SkipLinkText="Skip main navigation"
      Target="_self">
    <Items>
        <MenuItem Text="Home" NavigateUrl="/" />
        <MenuItem Text="Products" NavigateUrl="/products" />
        <MenuItem Text="Services" NavigateUrl="/services" />
        <MenuItem Text="Contact" NavigateUrl="/contact" />
    </Items>
    <StaticMenuItemStyle BackColor="@("SteelBlue")" ForeColor="@("White")" />
    <StaticHoverStyle BackColor="@("LightSteelBlue")" ForeColor="@("Black")" />
</Menu>
```

### Menu with Dynamic Submenus and Styles

```razor
<Menu Orientation="Vertical"
      MaximumDynamicDisplayLevels="3"
      StaticDisplayLevels="2">
    <Items>
        <MenuItem Text="File" Value="file">
            <MenuItem Text="New" Value="new" />
            <MenuItem Text="Open" Value="open" />
            <MenuItem Text="Save" Value="save" />
        </MenuItem>
        <MenuItem Text="Edit" Value="edit">
            <MenuItem Text="Cut" Value="cut" />
            <MenuItem Text="Copy" Value="copy" />
            <MenuItem Text="Paste" Value="paste" />
        </MenuItem>
    </Items>
    <StaticMenuItemStyle BackColor="@("LightSteelBlue")" ForeColor="@("Black")" />
    <DynamicMenuItemStyle BackColor="WebColor.Black" ForeColor="WebColor.Silver" />
    <DynamicHoverStyle BackColor="WebColor.LightSkyBlue" ForeColor="WebColor.Black" />
    <DynamicSelectedStyle BackColor="WebColor.PapayaWhip" ForeColor="WebColor.SteelBlue" />
</Menu>
```

### Data Binding with SiteMap

```razor
@using System.Xml

<Menu DataSource="@sitemapXml"
      Orientation="Horizontal"
      StaticDisplayLevels="2">
    <DataBindings>
        <MenuItemBinding DataMember="siteMapNode"
                         TextField="title"
                         NavigateUrlField="url" />
    </DataBindings>
</Menu>

@code {
    private XmlDocument sitemapXml;

    protected override void OnInitialized()
    {
        sitemapXml = new XmlDocument();
        sitemapXml.LoadXml(@"
            <siteMap>
                <siteMapNode title='Home' url='/'>
                    <siteMapNode title='Products' url='/products' />
                    <siteMapNode title='About' url='/about' />
                </siteMapNode>
            </siteMap>");
    }
}
```

## Selection and Events Reference

| Property/Event | Type | Default | Description |
|----------------|------|---------|-------------|
| `MenuItemClick` | `EventCallback<MenuEventArgs>` | — | Fires when a menu item is clicked. |
| `MenuItemDataBound` | `EventCallback<MenuEventArgs>` | — | Fires when a menu item is data-bound. |
| `SelectedItem` | `MenuItem` | `null` | Read-only. The currently selected menu item. |
| `SelectedValue` | `string` | `null` | Read-only. The `Value` of the selected menu item. |

## Navigation Properties Reference

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Vertical` | Menu layout direction (`Horizontal` or `Vertical`). |
| `Target` | `string` | `null` | Default target frame for navigation links. |
| `MaximumDynamicDisplayLevels` | `int` | `3` | Max depth of dynamically displayed submenus. |
| `SkipLinkText` | `string` | `"Skip Navigation Links"` | Accessibility skip-link text for screen readers. |
| `StaticDisplayLevels` | `int` | `1` | Number of menu levels displayed statically. |

## MenuItem Properties Reference

| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Display text for the menu item. |
| `Value` | `string` | Value associated with the menu item. |
| `NavigateUrl` | `string` | URL to navigate to when clicked. |
| `ToolTip` | `string` | Tooltip text on hover. |
| `Target` | `string` | Target frame (falls back to `Menu.Target`). |
| `ChildContent` | `RenderFragment` | Nested child `MenuItem` components. |

## Style Sub-Components Reference

| Component | Applied To |
|-----------|-----------|
| `<StaticMenuItemStyle>` | Static (always-visible) menu items |
| `<StaticHoverStyle>` | Static menu items on hover |
| `<DynamicMenuItemStyle>` | Dynamically displayed submenu items |
| `<DynamicMenuStyle>` | Dynamic submenu container |
| `<DynamicHoverStyle>` | Dynamic menu items on hover |
| `<DynamicSelectedStyle>` | Selected item in dynamic menus |

All style components support `BackColor`, `ForeColor`, `CssClass`, `BorderColor`, `BorderStyle`, `BorderWidth`, `Font`, `Height`, and `Width` properties.

## See Also

- [TreeView](TreeView.md)
- [SiteMapPath](SiteMapPath.md)

