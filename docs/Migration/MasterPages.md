# MasterPages in BlazorWebFormsComponents

**MasterPages** in ASP.NET Web Forms provide a way to create a consistent layout and structure across multiple pages in a web application. In Blazor, this concept is replaced by **Layouts**, which use a similar pattern but with different syntax and mechanics.

The BlazorWebFormsComponents library provides `MasterPage`, `ContentPlaceHolder`, and `Content` components that bridge the gap between Web Forms and Blazor, allowing developers to maintain familiar syntax while leveraging Blazor's layout system.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.masterpage

## Understanding MasterPages vs Blazor Layouts

### Web Forms MasterPage Approach

In Web Forms, a MasterPage defines the template for your pages:

```html
<!-- Site.Master -->
<%@ Master Language="C#" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title><%: Page.Title %></title>
</head>
<body>
    <form runat="server">
        <div class="header">
            <h1>My Website</h1>
        </div>
        
        <asp:ContentPlaceHolder ID="MainContent" runat="server">
            <p>Default content if page doesn't override</p>
        </asp:ContentPlaceHolder>
        
        <div class="footer">
            <p>&copy; 2024 My Company</p>
        </div>
    </form>
</body>
</html>
```

Child pages reference the master page:

```html
<!-- MyPage.aspx -->
<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to my page!</h2>
    <p>This is the content.</p>
</asp:Content>
```

### Blazor Layout Approach

In Blazor, layouts are components that inherit from `LayoutComponentBase`:

```razor
<!-- MainLayout.razor -->
@inherits LayoutComponentBase

<div class="header">
    <h1>My Website</h1>
</div>

@Body

<div class="footer">
    <p>&copy; 2024 My Company</p>
</div>
```

Pages use the `@layout` directive:

```razor
<!-- MyPage.razor -->
@page "/"
@layout MainLayout

<h2>Welcome to my page!</h2>
<p>This is the content.</p>
```

## Using MasterPage Components in Blazor

The BlazorWebFormsComponents library provides components that allow you to use Web Forms-style syntax:

### Creating a MasterPage

```razor
<!-- SiteMasterPage.razor -->
@using BlazorWebFormsComponents

<MasterPage>
    <div class="header">
        <h1>My Website</h1>
        <nav>
            <!-- Navigation menu -->
        </nav>
    </div>
    
    <div class="main-content">
        <ContentPlaceHolder ID="MainContent">
            <p>Default content goes here</p>
        </ContentPlaceHolder>
    </div>
    
    <div class="sidebar">
        <ContentPlaceHolder ID="Sidebar">
            <p>Default sidebar content</p>
        </ContentPlaceHolder>
    </div>
    
    <div class="footer">
        <p>&copy; @DateTime.Now.Year My Company</p>
    </div>
</MasterPage>
```

### Using the MasterPage in a Page

While the components support Web Forms-style syntax, in practice you should use Blazor's native layout system for new development. These components are primarily useful for migration scenarios.

## Features Supported

### MasterPage Component

- **ChildContent** - the template content containing the layout structure and ContentPlaceHolder controls
- **Visible** - controls visibility of the master page
- **Enabled** - inherited from BaseWebFormsComponent
- **CssClass**, **BackColor**, **ForeColor** - styling properties

### ContentPlaceHolder Component

- **ID** - identifies the placeholder for matching with Content controls
- **ChildContent** - default content displayed if no Content control provides a replacement
- **Visible** - controls visibility of the content area
- All styling properties from BaseWebFormsComponent

### Content Component

- **ContentPlaceHolderID** - specifies which ContentPlaceHolder this content is for
- **ChildContent** - the content to inject into the ContentPlaceHolder
- **Visible** - controls visibility

## Migration Strategy

### Strategy 1: Direct to Blazor Layouts (Recommended)

The cleanest migration path is to convert MasterPages directly to Blazor layouts:

**Before (Web Forms MasterPage):**
```html
<%@ Master Language="C#" %>
<asp:ContentPlaceHolder ID="MainContent" runat="server">
</asp:ContentPlaceHolder>
```

**After (Blazor Layout):**
```razor
@inherits LayoutComponentBase
@Body
```

**Before (Web Forms Page):**
```html
<%@ Page MasterPageFile="~/Site.Master" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1>Hello</h1>
</asp:Content>
```

**After (Blazor Page):**
```razor
@page "/hello"
@layout MainLayout
<h1>Hello</h1>
```

### Strategy 2: Using MasterPage Components for Gradual Migration

If you need to maintain Web Forms-style syntax temporarily:

1. Convert your `.Master` files to `.razor` components
2. Replace `<%@ Master %>` with `<MasterPage>` wrapper
3. Convert `<asp:ContentPlaceHolder>` to `<ContentPlaceHolder>`
4. In child pages, use standard Blazor syntax with layout directive

### Strategy 3: Nested MasterPages

Web Forms supports nested master pages. In Blazor, use nested layouts:

```razor
<!-- BaseLayout.razor -->
@inherits LayoutComponentBase
<div class="wrapper">
    @Body
</div>

<!-- SiteLayout.razor -->
@inherits LayoutComponentBase
@layout BaseLayout
<header>Header</header>
@Body
<footer>Footer</footer>
```

## Key Differences

| Feature | Web Forms | Blazor |
|---------|-----------|--------|
| **File Extension** | `.Master` | `.razor` |
| **Directive** | `<%@ Master %>` | `@inherits LayoutComponentBase` |
| **Content Area** | `<asp:ContentPlaceHolder>` | `@Body` or `@RenderSection` |
| **Page Declaration** | `<%@ Page MasterPageFile="~/Site.Master" %>` | `@layout MainLayout` |
| **Multiple Sections** | Multiple ContentPlaceHolders | `@RenderSection` for named sections |
| **Default Content** | Inside ContentPlaceHolder tags | Not typically used; define in layout |
| **Nesting** | `MasterPageFile` on master page | `@layout` directive in layout component |
| **HTML Head Management** | `<head runat="server">` in MasterPage | `<HeadOutlet>` in App.razor with `<HeadContent>` components |
| **Page Title** | `<%: Page.Title %>` or `Page.Title` property | `<PageTitle>` component |
| **Scripts/Styles** | In `<head>` or ContentPlaceHolders | `<HeadContent>` component or `<script>`/`<link>` in App.razor |

## Managing HTML Head and Scripts

**This is a critical difference between Web Forms MasterPages and Blazor layouts.**

### Web Forms MasterPage Head Management

In Web Forms, the MasterPage controls the entire HTML document including the `<head>` section:

```html
<!-- Site.Master -->
<%@ Master Language="C#" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <title><%: Page.Title %> - My Site</title>
    
    <!-- Scripts can be added here -->
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
    
    <!-- Stylesheets -->
    <link href="~/Content/site.css" rel="stylesheet" />
    
    <!-- Child pages can add to head -->
    <asp:ContentPlaceHolder ID="HeadContent" runat="server">
    </asp:ContentPlaceHolder>
</head>
<body>
    <!-- Body content -->
</body>
</html>
```

Child pages can add to the head:

```html
<%@ Page Title="My Page" MasterPageFile="~/Site.Master" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/css/custom.css" rel="stylesheet" />
    <script src="/js/custom.js"></script>
</asp:Content>
```

### Blazor Head Management

In Blazor, the HTML structure is defined in `App.razor` (or `_Host.cshtml` in older versions), **not in layouts**:

**App.razor** (root component):
```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    
    <!-- Static stylesheets -->
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="css/site.css" />
    
    <!-- HeadOutlet renders PageTitle and HeadContent from pages/layouts -->
    <HeadOutlet />
</head>
<body>
    <!-- Routes component renders pages with their layouts -->
    <Routes />
    
    <!-- Scripts at bottom of body -->
    <script src="_framework/blazor.web.js"></script>
    <script src="js/site.js"></script>
</body>
</html>
```

**Individual Pages** can add content to `<head>`:
```razor
@page "/mypage"

<PageTitle>My Page Title</PageTitle>

<HeadContent>
    <link href="css/custom.css" rel="stylesheet" />
    <script src="js/custom.js"></script>
    <meta name="description" content="My page description" />
</HeadContent>

<h1>Page Content</h1>
```

**Layouts** can also add head content:
```razor
@inherits LayoutComponentBase

<HeadContent>
    <!-- Common head content for all pages using this layout -->
    <link href="css/layout-specific.css" rel="stylesheet" />
</HeadContent>

<div class="page">
    @Body
</div>
```

### Migration Strategy for Head Content

When migrating from Web Forms MasterPages to Blazor:

#### 1. **Move Static Head Content to App.razor**

Content that applies to **all pages** should move from MasterPage to `App.razor`:

**Before (Site.Master):**
```html
<head runat="server">
    <link href="~/Content/bootstrap.css" rel="stylesheet" />
    <link href="~/Content/site.css" rel="stylesheet" />
    <script src="~/Scripts/jquery.js"></script>
</head>
```

**After (App.razor):**
```razor
<head>
    <link rel="stylesheet" href="css/bootstrap.css" />
    <link rel="stylesheet" href="css/site.css" />
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="js/jquery.js"></script>
</body>
```

#### 2. **Convert ContentPlaceHolder in Head to HeadContent**

If your MasterPage has a ContentPlaceHolder in the `<head>`:

**Before (Site.Master):**
```html
<head runat="server">
    <asp:ContentPlaceHolder ID="HeadContent" runat="server">
    </asp:ContentPlaceHolder>
</head>
```

**Before (MyPage.aspx):**
```html
<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/css/page-specific.css" rel="stylesheet" />
</asp:Content>
```

**After (MyPage.razor):**
```razor
@page "/mypage"

<HeadContent>
    <link href="css/page-specific.css" rel="stylesheet" />
</HeadContent>
```

#### 3. **Convert Page.Title to PageTitle Component**

**Before (Site.Master):**
```html
<head runat="server">
    <title><%: Page.Title %> - My Site</title>
</head>
```

**Before (MyPage.aspx):**
```html
<%@ Page Title="Home" MasterPageFile="~/Site.Master" %>
```

**After (MyPage.razor):**
```razor
@page "/"

<PageTitle>Home - My Site</PageTitle>
```

#### 4. **Script Management Strategies**

**Option A: Global Scripts in App.razor** (Recommended)
```razor
<!-- App.razor -->
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="js/jquery.js"></script>
    <script src="js/site.js"></script>
</body>
```

**Option B: Page-Specific Scripts with HeadContent**
```razor
<!-- MyPage.razor -->
<HeadContent>
    <script src="js/page-specific.js"></script>
</HeadContent>
```

**Option C: Dynamic Script Loading with IJSRuntime**
```razor
@inject IJSRuntime JS

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("loadScript", "js/dynamic.js");
        }
    }
}
```

### Important Limitations

1. **Layouts Don't Control HTML Structure** - In Blazor, layouts only control the body content. The `<html>`, `<head>`, and `<body>` tags are defined in `App.razor`, not layouts.

2. **No `<head runat="server">`** - Blazor doesn't have server-side head manipulation like Web Forms. Use `<HeadContent>` components instead.

3. **Script Placement** - Scripts are typically placed at the bottom of `App.razor` rather than in layouts or individual pages for better performance.

4. **HeadOutlet is Required** - You must include `<HeadOutlet />` in your `App.razor` for `<PageTitle>` and `<HeadContent>` to work.

### Example: Complete Head Migration

**Before - Web Forms:**

Site.Master:
```html
<%@ Master Language="C#" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title><%: Page.Title %> - Contoso</title>
    <link href="~/Content/bootstrap.css" rel="stylesheet" />
    <link href="~/Content/site.css" rel="stylesheet" />
    <asp:ContentPlaceHolder ID="HeadContent" runat="server">
    </asp:ContentPlaceHolder>
</head>
<body>
    <asp:ContentPlaceHolder ID="MainContent" runat="server">
    </asp:ContentPlaceHolder>
    <script src="~/Scripts/jquery.js"></script>
    <script src="~/Scripts/site.js"></script>
</body>
</html>
```

Products.aspx:
```html
<%@ Page Title="Products" MasterPageFile="~/Site.Master" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/products.css" rel="stylesheet" />
    <meta name="description" content="Our products" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1>Our Products</h1>
</asp:Content>
```

**After - Blazor:**

App.razor:
```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="css/bootstrap.css" />
    <link rel="stylesheet" href="css/site.css" />
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <script src="js/jquery.js"></script>
    <script src="js/site.js"></script>
</body>
</html>
```

MainLayout.razor:
```razor
@inherits LayoutComponentBase

<div class="page">
    @Body
</div>
```

Products.razor:
```razor
@page "/products"

<PageTitle>Products - Contoso</PageTitle>

<HeadContent>
    <link href="css/products.css" rel="stylesheet" />
    <meta name="description" content="Our products" />
</HeadContent>

<h1>Our Products</h1>
```

## Named Sections in Blazor

Blazor supports named sections similar to multiple ContentPlaceHolders:

**Layout:**
```razor
@inherits LayoutComponentBase

<header>
    @RenderSection("Header", required: false)
</header>

<main>
    @Body
</main>

<aside>
    @RenderSection("Sidebar", required: false)
</aside>
```

**Page:**
```razor
@page "/mypage"

@section Header {
    <h1>Page Title</h1>
}

@section Sidebar {
    <p>Sidebar content</p>
}

<p>Main content here</p>
```

## Best Practices

1. **Use Native Blazor Layouts** - For new development, use Blazor's built-in layout system instead of MasterPage components
2. **Convert During Migration** - When migrating from Web Forms, take the opportunity to convert to Blazor layouts
3. **Share Components** - Extract shared UI into reusable Blazor components instead of relying solely on layouts
4. **Cascading Values** - Use cascading parameters to share data between layouts and pages
5. **Consider RouteView** - Blazor's `RouteView` component provides additional flexibility for layout assignment

## Example: Complete Migration

**Before - Web Forms:**

Site.Master:
```html
<%@ Master Language="C#" %>
<html>
<body>
    <div class="header">Site Header</div>
    <asp:ContentPlaceHolder ID="MainContent" runat="server">
    </asp:ContentPlaceHolder>
    <div class="footer">Footer</div>
</body>
</html>
```

Default.aspx:
```html
<%@ Page Title="Home" MasterPageFile="~/Site.Master" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1>Welcome</h1>
</asp:Content>
```

**After - Blazor:**

MainLayout.razor:
```razor
@inherits LayoutComponentBase

<div class="header">Site Header</div>
<main>
    @Body
</main>
<div class="footer">Footer</div>
```

Index.razor:
```razor
@page "/"
@layout MainLayout

<PageTitle>Home</PageTitle>
<h1>Welcome</h1>
```

## Web Forms Features NOT Supported

The following MasterPage features don't have direct equivalents in Blazor:

- **`<head runat="server">`** - Blazor layouts don't control the HTML head. Use `<HeadContent>` components in pages/layouts, and define static head content in `App.razor`
- **Page.Title property** - Use `<PageTitle>` component instead
- **ContentPlaceHolder in `<head>`** - Use `<HeadContent>` component in pages instead
- **Master property** - Not needed; layouts are applied via directive
- **MasterPageFile in web.config** - Use `@layout` directive or configure in `App.razor`
- **MasterType directive** - Not applicable in Blazor
- **FindControl across master/content** - Use component references and cascading parameters
- **Server-side script registration** - Use `<HeadContent>` with `<script>` tags or IJSRuntime for dynamic loading

## Additional Resources

- [Blazor Layout Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/layouts)
- [ASP.NET Core Blazor routing and navigation](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing)
- [Blazor Component Libraries](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries)

## Conclusion

While the BlazorWebFormsComponents library provides MasterPage components for compatibility, the recommended approach for Blazor development is to use native layouts. These components are most valuable during migration to ease the transition from Web Forms to Blazor, but should be replaced with standard Blazor layouts as part of the modernization process.

