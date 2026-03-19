# ContentPlaceHolder

The **ContentPlaceHolder** component emulates the ASP.NET Web Forms `<asp:ContentPlaceHolder>` control. A ContentPlaceHolder defines a region in a master page where child pages inject content using Content controls. This component bridges Web Forms master page syntax with Blazor's layout system.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.contentplaceholder

## Features Supported in Blazor

- `ID` — identifier to match with Content control's ContentPlaceHolderID
- `ChildContent` — default content to display if no Content control provides a replacement
- Auto-registration with parent MasterPage component
- Renders content provided by child pages via Content controls, or falls back to default ChildContent

## Web Forms Features NOT Supported

- Multiple master pages cascading placeholders (use nested layouts in Blazor instead)
- Master page inheritance chains with placeholders

## Web Forms Declarative Syntax

```html
<!-- Site.Master -->
<%@ Master Language="C#" %>
<html>
<head>
    <title><%: Page.Title %></title>
</head>
<body>
    <div class="header">
        <h1>My Website</h1>
    </div>
    
    <asp:ContentPlaceHolder ID="MainContent" runat="server">
        <p>Default content if no page overrides this placeholder</p>
    </asp:ContentPlaceHolder>
    
    <div class="footer">
        <p>&copy; 2024 My Company</p>
    </div>
</body>
</html>
```

## Blazor Syntax

ContentPlaceHolder components are placed within the ChildContent of a MasterPage component. They automatically register with the parent MasterPage.

### Basic ContentPlaceHolder with Default Content

```razor
<MasterPage>
    <ChildContent>
        <div class="header">
            <h1>My Website</h1>
        </div>
        
        <ContentPlaceHolder ID="MainContent">
            <p>This is the default content for the main area.</p>
        </ContentPlaceHolder>
        
        <div class="footer">
            <p>&copy; 2024 My Company</p>
        </div>
    </ChildContent>
</MasterPage>
```

### Multiple ContentPlaceHolders

```razor
<MasterPage>
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
                        <li><a href="/">Home</a></li>
                        <li><a href="/about">About</a></li>
                    </ul>
                </nav>
            </ContentPlaceHolder>
        </aside>
        
        <main>
            <ContentPlaceHolder ID="MainContent">
                <p>Main content goes here</p>
            </ContentPlaceHolder>
        </main>
    </ChildContent>
</MasterPage>
```

### Empty ContentPlaceHolder with Content Requirement

```razor
<MasterPage>
    <ChildContent>
        <div class="container">
            <ContentPlaceHolder ID="Content">
                <!-- No default content - pages must provide content -->
            </ContentPlaceHolder>
        </div>
    </ChildContent>
</MasterPage>
```

### ContentPlaceHolder with Styled Default Content

```razor
<MasterPage>
    <ChildContent>
        <div class="main">
            <ContentPlaceHolder ID="MainContent">
                <div class="alert alert-info">
                    <p>No custom content provided. This is the default message.</p>
                </div>
            </ContentPlaceHolder>
        </div>
    </ChildContent>
</MasterPage>
```

## HTML Output

ContentPlaceHolder renders either the content provided by a child page's Content control, or its default ChildContent if no matching Content is found.

**Blazor Input (with matching Content):**
```razor
<MasterPage>
    <ChildContent>
        <ContentPlaceHolder ID="Main">
            <p>Default</p>
        </ContentPlaceHolder>
    </ChildContent>
    <Content ContentPlaceHolderID="Main">
        <p>Custom Content</p>
    </Content>
</MasterPage>
```

**Rendered HTML:**
```html
<p>Custom Content</p>
```

**Blazor Input (no matching Content):**
```razor
<MasterPage>
    <ChildContent>
        <ContentPlaceHolder ID="Main">
            <p>Default</p>
        </ContentPlaceHolder>
    </ChildContent>
</MasterPage>
```

**Rendered HTML:**
```html
<p>Default</p>
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ID` | string | null | The unique identifier for this placeholder; used by Content controls to match ContentPlaceHolderID |
| `ChildContent` | RenderFragment | null | The default content to render if no Content control provides a replacement |
| `Visible` | bool | true | Controls whether the placeholder and its content are rendered (inherited from BaseWebFormsComponent) |

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix and `runat="server"`** — Change `<asp:ContentPlaceHolder>` to `<ContentPlaceHolder>`
2. **Keep the ID** — The ID attribute is used the same way to match placeholders with Content controls
3. **Place within MasterPage** — ContentPlaceHolder must be a child of a MasterPage component
4. **Default content becomes ChildContent** — What was inside the Web Forms tag becomes the ChildContent of the component

### Before (Web Forms)

```html
<!-- Site.Master -->
<%@ Master Language="C#" %>
<html>
<body>
    <div class="sidebar">
        <asp:ContentPlaceHolder ID="SidebarContent" runat="server">
            <p>Default sidebar</p>
        </asp:ContentPlaceHolder>
    </div>
    
    <div class="main">
        <asp:ContentPlaceHolder ID="MainContent" runat="server">
            <p>Default main area</p>
        </asp:ContentPlaceHolder>
    </div>
</body>
</html>

<!-- MyPage.aspx -->
<%@ Page MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>My Page</h1>
    <p>My content here</p>
</asp:Content>
```

### After (Blazor)

```razor
<!-- MasterLayout.razor -->
<MasterPage>
    <ChildContent>
        <div class="sidebar">
            <ContentPlaceHolder ID="SidebarContent">
                <p>Default sidebar</p>
            </ContentPlaceHolder>
        </div>
        
        <div class="main">
            <ContentPlaceHolder ID="MainContent">
                <p>Default main area</p>
            </ContentPlaceHolder>
        </div>
    </ChildContent>
</MasterPage>

<!-- MyPage.razor -->
<MasterPage>
    <ChildContent>
        <div class="sidebar">
            <ContentPlaceHolder ID="SidebarContent">
                <p>Default sidebar</p>
            </ContentPlaceHolder>
        </div>
        
        <div class="main">
            <ContentPlaceHolder ID="MainContent">
                <p>Default main area</p>
            </ContentPlaceHolder>
        </div>
    </ChildContent>
    
    <Content ContentPlaceHolderID="MainContent">
        <h1>My Page</h1>
        <p>My content here</p>
    </Content>
</MasterPage>
```

## See Also

- [Content](Content.md) — Provides content to fill ContentPlaceHolder regions
- [MasterPage](MasterPage.md) — Container for master page layouts
- Migration: [Master Pages](../Migration/MasterPages.md) — Comprehensive master page migration guide
