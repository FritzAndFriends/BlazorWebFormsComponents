# MasterPage

The **MasterPage** component emulates the ASP.NET Web Forms `<asp:MasterPage>` directive and provides master page template functionality in Blazor. A MasterPage defines a consistent layout and structure across pages, with customizable regions defined by ContentPlaceHolder components. Child pages inject content using Content controls.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.masterpage

## Features Supported in Blazor

- `ChildContent` — the master page template layout containing ContentPlaceHolder controls
- `Head` — optional head content automatically wrapped in HeadContent component (for `<title>`, `<meta>`, `<link>`, etc.)
- `Visible` — controls whether the entire master page is rendered
- Content and ContentPlaceHolder registration and management
- Support for multiple content sections via named ContentPlaceHolders
- Automatic `<title>` element handling in Head content

## Web Forms Features NOT Supported

- `Title` property — use Blazor's `PageTitle` component or `@page` directive instead
- `MasterPageFile` property — nested master pages use nested layouts instead
- Direct theme/skin support — use CSS styling instead

## Syntax Comparison

=== "Web Forms"

    ```html
    <!-- Site.Master -->
    <%@ Master Language="C#" %>
    <!DOCTYPE html>
    <html>
    <head runat="server">
        <title>My Site</title>
        <link rel="stylesheet" href="~/css/site.css" />
    </head>
    <body>
        <form runat="server">
            <div class="header">
                <h1>My Website</h1>
            </div>
            
            <asp:ContentPlaceHolder ID="MainContent" runat="server">
                <p>Default content</p>
            </asp:ContentPlaceHolder>
            
            <div class="footer">
                <p>&copy; 2024 My Company</p>
            </div>
        </form>
    </body>
    </html>

    <!-- MyPage.aspx -->
    <%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <h2>Welcome to my page!</h2>
    </asp:Content>
    ```

=== "Blazor"

    ```razor
    <!-- MasterLayout.razor -->
    <MasterPage>
        <Head>
            <title>My Site</title>
            <link rel="stylesheet" href="css/site.css" />
        </Head>
        
        <ChildContent>
            <div class="header">
                <h1>My Website</h1>
            </div>
            
            <ContentPlaceHolder ID="MainContent">
                <p>Default content</p>
            </ContentPlaceHolder>
            
            <div class="footer">
                <p>&copy; 2024 My Company</p>
            </div>
        </ChildContent>
    </MasterPage>

    <!-- MyPage.razor -->
    <MasterPage>
        <Head>
            <title>My Site - Home</title>
            <link rel="stylesheet" href="css/site.css" />
        </Head>
        
        <ChildContent>
            <div class="header">
                <h1>My Website</h1>
            </div>
            
            <ContentPlaceHolder ID="MainContent">
                <p>Default content</p>
            </ContentPlaceHolder>
            
            <div class="footer">
                <p>&copy; 2024 My Company</p>
            </div>
        </ChildContent>
        
        <Content ContentPlaceHolderID="MainContent">
            <h2>Welcome to my page!</h2>
        </Content>
    </MasterPage>
    ```

## Blazor Usage Examples

### Basic MasterPage with Layout Template

```razor
<MasterPage>
    <ChildContent>
        <div class="header">
            <h1>My Website</h1>
        </div>
        
        <ContentPlaceHolder ID="MainContent">
            <p>Default content</p>
        </ContentPlaceHolder>
        
        <div class="footer">
            <p>&copy; 2024 My Company</p>
        </div>
    </ChildContent>
</MasterPage>
```

### MasterPage with Head Content

The `Head` parameter allows you to define head content (title, meta tags, stylesheets, etc.) that is rendered in the document's `<head>` section via `HeadOutlet`.

```razor
<MasterPage>
    <Head>
        <title>My Website - Home</title>
        <meta name="description" content="My awesome website" />
        <link rel="stylesheet" href="css/site.css" />
    </Head>
    
    <ChildContent>
        <header>
            <h1>My Website</h1>
            <nav>
                <a href="/">Home</a>
                <a href="/about">About</a>
            </nav>
        </header>
        
        <ContentPlaceHolder ID="MainContent">
            <p>Default content goes here</p>
        </ContentPlaceHolder>
        
        <footer>
            <p>&copy; 2024 My Company</p>
        </footer>
    </ChildContent>
</MasterPage>
```

### MasterPage with Multiple Content Sections

```razor
<MasterPage>
    <Head>
        <title>My Site</title>
    </Head>
    
    <ChildContent>
        <header>
            <ContentPlaceHolder ID="PageTitle">
                <h1>Default Title</h1>
            </ContentPlaceHolder>
        </header>
        
        <aside>
            <ContentPlaceHolder ID="Sidebar">
                <nav>
                    <ul>
                        <li>Default Nav</li>
                    </ul>
                </nav>
            </ContentPlaceHolder>
        </aside>
        
        <main>
            <ContentPlaceHolder ID="MainContent">
                <p>Default main content</p>
            </ContentPlaceHolder>
        </main>
    </ChildContent>
    
    <Content ContentPlaceHolderID="PageTitle">
        <h1>Article Title</h1>
    </Content>
    
    <Content ContentPlaceHolderID="Sidebar">
        <nav>
            <ul>
                <li><a href="/">Home</a></li>
                <li><a href="/articles">Articles</a></li>
            </ul>
        </nav>
    </Content>
    
    <Content ContentPlaceHolderID="MainContent">
        <article>
            <p>Article content here</p>
        </article>
    </Content>
</MasterPage>
```

### MasterPage with Conditional Visibility

```razor
<MasterPage Visible="@isVisible">
    <Head>
        <title>Conditional Master</title>
    </Head>
    
    <ChildContent>
        <header>
            <h1>Site Header</h1>
        </header>
        
        <ContentPlaceHolder ID="MainContent">
            <p>Content</p>
        </ContentPlaceHolder>
    </ChildContent>
</MasterPage>

@code {
    private bool isVisible = true;
}
```

## HTML Output

MasterPage renders its ChildContent, with ContentPlaceHolder regions replaced by either Content provided by child pages or default ChildContent if no custom Content is provided. The Head content is rendered in the document's `<head>` section via `HeadOutlet`.

**Blazor Input:**
```razor
<MasterPage>
    <Head>
        <title>My Site</title>
    </Head>
    
    <ChildContent>
        <header>
            <h1>Header</h1>
        </header>
        <ContentPlaceHolder ID="Main">
            <p>Default</p>
        </ContentPlaceHolder>
    </ChildContent>
    
    <Content ContentPlaceHolderID="Main">
        <p>Custom</p>
    </Content>
</MasterPage>
```

**Rendered HTML:**
```html
<!-- In <head> -->
<title>My Site</title>

<!-- In <body> -->
<header>
    <h1>Header</h1>
</header>
<p>Custom</p>
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | RenderFragment | null | The master page template layout containing ContentPlaceHolder controls |
| `Head` | RenderFragment | null | Optional head content (title, meta, link, etc.) to be rendered in the document's `<head>` section via `HeadOutlet` |
| `Visible` | bool | true | Controls whether the entire master page is rendered (inherited from BaseWebFormsComponent) |

## Obsolete Parameters

The following parameters exist for backward compatibility but are obsolete in Blazor:

| Parameter | Type | Notes |
|-----------|------|-------|
| `Title` | string | Use `PageTitle` component or `@page` directive instead |
| `MasterPageFile` | string | Nested layouts use the `@layout` directive in child layouts instead |

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Rename Master file** — Web Forms `.Master` files become `.razor` Blazor components
2. **Remove directives** — Remove `<%@ Master %>` and `runat="server"` attributes
3. **Use Head parameter** — Move `<head runat="server">` content into the `Head` parameter
4. **Use ChildContent** — The template layout goes into the `ChildContent` parameter
5. **Keep ID attributes** — ContentPlaceHolder `ID` attributes work the same way
6. **Nest Content controls** — Content controls are placed as child components of the MasterPage
7. **Set page title** — Use `PageTitle` component in the `Head` section instead of ASP.NET's dynamic title setting

=== "Web Forms"

    ```html
    <!-- Site.Master -->
    <%@ Master Language="C#" %>
    <!DOCTYPE html>
    <html>
    <head runat="server">
        <title>My Site</title>
        <link rel="stylesheet" href="~/css/site.css" />
    </head>
    <body>
        <form runat="server">
            <header>
                <h1>My Website</h1>
            </header>
            
            <main>
                <asp:ContentPlaceHolder ID="MainContent" runat="server">
                    <p>Default content</p>
                </asp:ContentPlaceHolder>
            </main>
            
            <footer>
                <p>&copy; 2024</p>
            </footer>
        </form>
    </body>
    </html>

    <!-- MyPage.aspx -->
    <%@ Page Title="Home" MasterPageFile="~/Site.Master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <h2>Welcome!</h2>
        <p>This is my page.</p>
    </asp:Content>
    ```

=== "Blazor"

    ```razor
    <!-- MasterLayout.razor -->
    <MasterPage>
        <Head>
            <title>My Site</title>
            <link rel="stylesheet" href="css/site.css" />
        </Head>
        
        <ChildContent>
            <header>
                <h1>My Website</h1>
            </header>
            
            <main>
                <ContentPlaceHolder ID="MainContent">
                    <p>Default content</p>
                </ContentPlaceHolder>
            </main>
            
            <footer>
                <p>&copy; 2024</p>
            </footer>
        </ChildContent>
    </MasterPage>

    <!-- MyPage.razor -->
    <MasterPage>
        <Head>
            <title>My Site - Home</title>
            <link rel="stylesheet" href="css/site.css" />
        </Head>
        
        <ChildContent>
            <header>
                <h1>My Website</h1>
            </header>
            
            <main>
                <ContentPlaceHolder ID="MainContent">
                    <p>Default content</p>
                </ContentPlaceHolder>
            </main>
            
            <footer>
                <p>&copy; 2024</p>
            </footer>
        </ChildContent>
        
        <Content ContentPlaceHolderID="MainContent">
            <h2>Welcome!</h2>
            <p>This is my page.</p>
        </Content>
    </MasterPage>
    ```

## See Also

- [Content](Content.md) — Provides content to fill ContentPlaceHolder regions
- [ContentPlaceHolder](ContentPlaceHolder.md) — Defines placeholder regions in master pages
- Migration: [Master Pages](../Migration/MasterPages.md) — Comprehensive master page migration guide
- Blazor Layouts: documentation coming soon
