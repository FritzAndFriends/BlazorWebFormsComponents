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

- **Page.Title property** - Use `<PageTitle>` component instead
- **Master property** - Not needed; layouts are applied via directive
- **MasterPageFile in web.config** - Use `@layout` directive or configure in `App.razor`
- **MasterType directive** - Not applicable in Blazor
- **FindControl across master/content** - Use component references and cascading parameters

## Additional Resources

- [Blazor Layout Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/layouts)
- [ASP.NET Core Blazor routing and navigation](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing)
- [Blazor Component Libraries](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries)

## Conclusion

While the BlazorWebFormsComponents library provides MasterPage components for compatibility, the recommended approach for Blazor development is to use native layouts. These components are most valuable during migration to ease the transition from Web Forms to Blazor, but should be replaced with standard Blazor layouts as part of the modernization process.

