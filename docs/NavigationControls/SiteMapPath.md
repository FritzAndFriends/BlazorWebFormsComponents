# SiteMapPath

The SiteMapPath component displays a breadcrumb navigation path showing the current page's location within a site hierarchy. It helps users understand where they are in your application and provides quick navigation back to parent pages.

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.sitemappath?view=netframework-4.8

## Features Supported in Blazor

- **PathSeparator** - Custom separator string between nodes (default: " > ")
- **PathSeparatorTemplate** - Custom template for rendering separators
- **PathDirection** - RootToCurrent or CurrentToRoot ordering
- **RenderCurrentNodeAsLink** - Whether the current page is clickable
- **ShowToolTips** - Display node descriptions as tooltips
- **ParentLevelsDisplayed** - Limit breadcrumb depth (-1 for all)
- **CurrentNodeTemplate** - Custom template for current page node
- **NodeTemplate** - Custom template for ancestor nodes
- **RootNodeTemplate** - Custom template for the root/home node
- **Style properties** - CurrentNodeStyle, NodeStyle, RootNodeStyle, PathSeparatorStyle

### Blazor Notes

Unlike Web Forms, which uses a `Web.sitemap` XML file with `SiteMapDataSource`, the Blazor implementation requires you to provide the site hierarchy programmatically via the `SiteMapProvider` property. This gives you more flexibility to build navigation hierarchies dynamically from databases, configuration, or other sources.

The `SiteMapNode` class provides a simple way to build hierarchies:

```csharp
var root = new SiteMapNode("Home", "/");
var products = new SiteMapNode("Products", "/products", "Browse our products");
root.AddChild(products);
```

## Web Forms Features NOT Supported

- **SiteMapDataSource** - Not supported; provide a `SiteMapNode` hierarchy directly
- **Web.sitemap XML file** - Not supported; build the hierarchy in code
- **Provider** - The `SiteMapProvider` property accepts a `SiteMapNode` root, not a provider name
- **SkipLinkText** - Accessibility skip link not implemented
- **ItemDataBound event** - Not supported; use templates for customization

## Web Forms Declarative Syntax

```html
<asp:SiteMapPath
    CurrentNodeStyle-BackColor="Yellow"
    CurrentNodeTemplate="..."
    ID="SiteMapPath1"
    NodeStyle-ForeColor="Blue"
    NodeTemplate="..."
    ParentLevelsDisplayed="-1"
    PathDirection="RootToCurrent"
    PathSeparator=" > "
    PathSeparatorStyle-Font-Bold="true"
    PathSeparatorTemplate="..."
    RenderCurrentNodeAsLink="false"
    RootNodeStyle-Font-Bold="true"
    RootNodeTemplate="..."
    ShowToolTips="true"
    SiteMapProvider="AspNetXmlSiteMapProvider"
    SkipLinkText=""
    runat="server" />
```

## Blazor Syntax

```razor
<SiteMapPath
    CurrentUrl="@CurrentPage"
    ParentLevelsDisplayed="-1"
    PathDirection="PathDirection.RootToCurrent"
    PathSeparator=" > "
    RenderCurrentNodeAsLink="false"
    ShowToolTips="true"
    SiteMapProvider="@SiteMap" />
```

## Usage Notes

1. **Build your site map in code** - Create a `SiteMapNode` hierarchy that represents your site structure
2. **Provide the current URL** - Set `CurrentUrl` to match against the site map nodes
3. **URL matching is flexible** - The component normalizes URLs, handling leading slashes and query strings
4. **Templates override default rendering** - When you provide a template, you're responsible for all markup including links

## Examples

### Basic Usage

```razor
@* Basic breadcrumb navigation *@
<SiteMapPath SiteMapProvider="@SiteMap" CurrentUrl="@CurrentUrl" />

@code {
    private string CurrentUrl = "/products/electronics";

    private SiteMapNode SiteMap = BuildSiteMap();

    private static SiteMapNode BuildSiteMap()
    {
        var root = new SiteMapNode("Home", "/");
        var products = new SiteMapNode("Products", "/products", "Browse our catalog");
        var electronics = new SiteMapNode("Electronics", "/products/electronics");
        
        root.AddChild(products);
        products.AddChild(electronics);
        
        return root;
    }
}
```

### Custom Separator

```razor
@* Using a custom separator *@
<SiteMapPath 
    SiteMapProvider="@SiteMap" 
    CurrentUrl="/products" 
    PathSeparator=" / " />

@* Using a template for the separator *@
<SiteMapPath SiteMapProvider="@SiteMap" CurrentUrl="/products">
    <PathSeparatorTemplate>
        <span class="mx-2">→</span>
    </PathSeparatorTemplate>
</SiteMapPath>
```

### Current Node as Link

```razor
@* Make the current page clickable *@
<SiteMapPath 
    SiteMapProvider="@SiteMap" 
    CurrentUrl="/products" 
    RenderCurrentNodeAsLink="true" />
```

### Limiting Parent Levels

```razor
@* Only show 2 parent levels *@
<SiteMapPath 
    SiteMapProvider="@SiteMap" 
    CurrentUrl="/products/electronics/phones/iphone" 
    ParentLevelsDisplayed="2" />
@* Renders: Electronics > Phones > iPhone (skips Home and Products) *@
```

### Custom Templates

```razor
@* Full template customization *@
<SiteMapPath SiteMapProvider="@SiteMap" CurrentUrl="/products/electronics">
    <RootNodeTemplate Context="node">
        <a href="@node.Url" class="text-primary">🏠 @node.Title</a>
    </RootNodeTemplate>
    <NodeTemplate Context="node">
        <a href="@node.Url" class="text-secondary">@node.Title</a>
    </NodeTemplate>
    <CurrentNodeTemplate Context="node">
        <strong class="text-dark">📍 @node.Title</strong>
    </CurrentNodeTemplate>
    <PathSeparatorTemplate>
        <span class="text-muted mx-1">/</span>
    </PathSeparatorTemplate>
</SiteMapPath>
```

### Reverse Path Direction

```razor
@* Show current page first, then parents *@
<SiteMapPath 
    SiteMapProvider="@SiteMap" 
    CurrentUrl="/products/electronics" 
    PathDirection="PathDirection.CurrentToRoot" />
@* Renders: Electronics > Products > Home *@
```

## See Also

- [Menu](Menu.md) - Hierarchical navigation menu
- [TreeView](TreeView.md) - Tree-style navigation
- [Migration Guide](../Migration/readme.md)
