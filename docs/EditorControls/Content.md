# Content

The **Content** component emulates the ASP.NET Web Forms `<asp:Content>` control. A Content control is used in child pages to provide content that fills a `ContentPlaceHolder` in a master page. This component bridges Web Forms master page syntax with Blazor's layout system.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.content

## Features Supported in Blazor

- `ChildContent` — the content to be rendered in the associated ContentPlaceHolder
- `ContentPlaceHolderID` — identifies which ContentPlaceHolder this content is for
- Registers named section content with the nearest MasterPage host
- Works with MasterPage and ContentPlaceHolder components

## Web Forms Features NOT Supported

- Direct `MasterPageFile` path resolution (use Blazor layout/component composition instead)

## Syntax Comparison

=== "Web Forms"

    ```html
    <!-- MyPage.aspx -->
    <%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <h2>Welcome to my page!</h2>
        <p>This is the page-specific content.</p>
    </asp:Content>
    ```

=== "Blazor"

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
                <p>&copy; 2024</p>
            </div>
        </ChildContent>
        <ChildComponents>
            <Content ContentPlaceHolderID="MainContent">
                <h2>Page Title</h2>
                <p>This content replaces the MainContent placeholder.</p>
            </Content>
        </ChildComponents>
    </MasterPage>
    ```

Content controls are typically placed inside `ChildComponents` so the shell in `ChildContent` stays separate from the page-level section overrides.

### Multiple Content Sections

```razor
<MasterPage>
    <ChildContent>
        <header>
            <h1>Site Header</h1>
            <ContentPlaceHolder ID="PageTitle">
                <p>Default Title</p>
            </ContentPlaceHolder>
        </header>
        
        <main>
            <ContentPlaceHolder ID="MainContent">
                <p>Default main content</p>
            </ContentPlaceHolder>
        </main>
    </ChildContent>
    
    <ChildComponents>
        <Content ContentPlaceHolderID="PageTitle">
            <h2>My Page Title</h2>
        </Content>
        
        <Content ContentPlaceHolderID="MainContent">
            <p>Page-specific main content goes here.</p>
        </Content>
    </ChildComponents>
</MasterPage>
```

### Content with HTML Elements

```razor
<MasterPage>
    <ChildContent>
        <ContentPlaceHolder ID="Body">
            No content provided
        </ContentPlaceHolder>
    </ChildContent>
    
    <ChildComponents>
        <Content ContentPlaceHolderID="Body">
            <article>
                <h1>Article Title</h1>
                <p>Article content here.</p>
                <button>Read More</button>
            </article>
        </Content>
    </ChildComponents>
</MasterPage>
```

## HTML Output

Content does **not render directly** — its ChildContent is injected into the corresponding ContentPlaceHolder. If no ContentPlaceHolder with a matching ID exists, the content is ignored.

**Blazor Input:**
```razor
<MasterPage>
    <ChildContent>
        <ContentPlaceHolder ID="Main">
            <p>Default</p>
        </ContentPlaceHolder>
    </ChildContent>
    <ChildComponents>
        <Content ContentPlaceHolderID="Main">
            <p>Custom Content</p>
        </Content>
    </ChildComponents>
</MasterPage>
```

**Rendered HTML:**
```html
<p>Custom Content</p>
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | RenderFragment | null | The content to be injected into the associated ContentPlaceHolder |
| `ContentPlaceHolderID` | string | null | The ID of the ContentPlaceHolder this content is for |

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix and `runat="server"`** — Change `<asp:Content>` to `<Content>`
2. **Keep ContentPlaceHolderID** — The ID attribute is used the same way to match content with placeholders
3. **Place under ChildComponents** — Keep Content controls in the page override area instead of mixing them with shell structure
4. **Rename Master page reference** — Remove `MasterPageFile` directive; instead nest Content controls in the MasterPage component

### Before (Web Forms)

```html
<!-- Site.Master -->
<%@ Master Language="C#" %>
<html>
<body>
    <header>
        <h1>My Site</h1>
    </header>
    
    <asp:ContentPlaceHolder ID="MainContent" runat="server">
        <p>Default content</p>
    </asp:ContentPlaceHolder>
</body>
</html>
```

```html
<!-- MyPage.aspx -->
<%@ Page MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome!</h2>
    <p>Page content goes here.</p>
</asp:Content>
```

### After (Blazor)

```razor
<!-- MasterLayout.razor -->
<MasterPage>
    <ChildContent>
        <header>
            <h1>My Site</h1>
        </header>
        
        <ContentPlaceHolder ID="MainContent">
            <p>Default content</p>
        </ContentPlaceHolder>
    </ChildContent>
</MasterPage>
```

```razor
<!-- MyPage.razor -->
<MasterPage>
    <ChildContent>
        <header>
            <h1>My Site</h1>
        </header>
        
        <ContentPlaceHolder ID="MainContent">
            <p>Default content</p>
        </ContentPlaceHolder>
    </ChildContent>
    
    <ChildComponents>
        <Content ContentPlaceHolderID="MainContent">
            <h2>Welcome!</h2>
            <p>Page content goes here.</p>
        </Content>
    </ChildComponents>
</MasterPage>
```

## See Also

- [ContentPlaceHolder](ContentPlaceHolder.md) — Defines placeholder regions in master pages
- [MasterPage](MasterPage.md) — Container for master page layouts
- Migration: [Master Pages](../Migration/MasterPages.md) — Comprehensive master page migration guide
