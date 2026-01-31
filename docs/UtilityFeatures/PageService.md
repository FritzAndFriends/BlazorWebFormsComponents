# PageService

## Background

In ASP.NET Web Forms, the `Page` object provided a central place for page-level properties and functionality. Key properties included `Page.Title`, `Page.MetaDescription`, and `Page.MetaKeywords`, which allowed developers to programmatically set page metadata that appears in the browser and search engine results.

```csharp
// Web Forms code-behind
protected void Page_Load(object sender, EventArgs e)
{
    Page.Title = "My Dynamic Page Title";
    Page.MetaDescription = "Description for search engines";
    Page.MetaKeywords = "keyword1, keyword2, keyword3";
}

protected void UpdateButton_Click(object sender, EventArgs e)
{
    Page.Title = "Title Updated - " + DateTime.Now.ToString();
    Page.MetaDescription = GetDescriptionFromDatabase();
}
```

This pattern was essential for:
- Setting page titles dynamically based on data or user actions
- Implementing SEO-friendly titles and descriptions for content pages
- Providing context-specific metadata in master page scenarios
- Improving search engine visibility and social media sharing

## Web Forms Usage

In Web Forms, the `Page` object was automatically available in all code-behind files:

```aspx
<%@ Page Language="C#" Title="Static Title" 
         MetaDescription="Page description" 
         MetaKeywords="keyword1, keyword2" %>
```

```csharp
// Code-behind (.aspx.cs)
public partial class MyPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Dynamically set page metadata
        Page.Title = GetTitleFromDatabase();
        Page.MetaDescription = GetDescriptionFromDatabase();
        Page.MetaKeywords = GetKeywordsFromDatabase();
    }
    
    private string GetTitleFromDatabase()
    {
        // Fetch from database
        return "Dynamic Title from DB";
    }
    
    private string GetDescriptionFromDatabase()
    {
        return "This is a dynamic description for SEO";
    }
    
    private string GetKeywordsFromDatabase()
    {
        return "blazor, webforms, migration, seo";
    }
}
```

These properties would automatically update the HTML `<title>` and `<meta>` elements in the rendered page.

## Blazor Implementation

BlazorWebFormsComponents provides `IPageService` and `PageService` to replicate this functionality in Blazor. The service is registered as a scoped service (one instance per request/render cycle) and can be injected into any component or page.

### Key Components

1. **IPageService Interface** - Defines the contract for page-level services
2. **PageService Class** - Default implementation providing `Title`, `MetaDescription`, and `MetaKeywords` properties
3. **Page Component** - Blazor component that renders the dynamic `<PageTitle>` and `<meta>` tags

### Registration

The service is automatically registered when you call `AddBlazorWebFormsComponents()`:

```csharp
// Program.cs
builder.Services.AddBlazorWebFormsComponents();
```

This registers `IPageService` as a scoped service that can be injected into components.

### Usage in Blazor

**Step 1: Add the Page component to your page or layout**

```razor
@inject IPageService PageService

<Page />  @* This renders the dynamic <PageTitle> and meta tags *@
```

**Step 2: Set page properties programmatically**

```razor
@code {
    protected override void OnInitialized()
    {
        PageService.Title = "My Dynamic Page Title";
        PageService.MetaDescription = "Description for search engines";
        PageService.MetaKeywords = "blazor, webforms, migration";
    }
    
    private void UpdateMetadata()
    {
        PageService.Title = "Updated Title - " + DateTime.Now.ToString();
        PageService.MetaDescription = "Updated description based on user action";
    }
}
```

### Complete Example

```razor
@page "/MyPage"
@inject IPageService PageService

<Page />

<h1>My Page</h1>

<div>
    <label>New Title:</label>
    <input @bind="newTitle" />
    <label>New Description:</label>
    <textarea @bind="newDescription"></textarea>
    <button @onclick="UpdatePageMetadata">Update Metadata</button>
</div>

@code {
    private string newTitle = "";
    private string newDescription = "";
    
    protected override void OnInitialized()
    {
        PageService.Title = "My Page - BlazorWebFormsComponents";
        PageService.MetaDescription = "A sample page demonstrating PageService";
        PageService.MetaKeywords = "blazor, sample, demo";
    }
    
    private void UpdatePageMetadata()
    {
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            PageService.Title = newTitle;
            PageService.MetaDescription = newDescription;
        }
    }
}
```

## Migration Path

### Before (Web Forms)

```aspx
<%@ Page Language="C#" MasterPageFile="~/Site.Master" 
         MetaDescription="Customer details page" 
         MetaKeywords="customer, details, crm" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        var customerName = GetCustomerName();
        Page.Title = "Customer Details - " + customerName;
        Page.MetaDescription = $"View details for {customerName}";
    }
</script>
```

### After (Blazor)

```razor
@page "/customer/{id:int}"
@inject IPageService PageService

<Page />

<h1>Customer Details</h1>

@code {
    [Parameter]
    public int Id { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        var customerName = await GetCustomerName(Id);
        PageService.Title = $"Customer Details - {customerName}";
        PageService.MetaDescription = $"View detailed information for {customerName}";
        PageService.MetaKeywords = "customer, details, crm";
    }
}
```

## Features

### Title Property

- **Get/Set**: Read and write the page title dynamically
- **Event-Driven**: `TitleChanged` event fires when title is updated
- **Reactive**: The `Page` component automatically updates the browser title when the property changes

### MetaDescription Property

- **Get/Set**: Read and write the meta description dynamically
- **SEO-Friendly**: Appears in search engine results (recommended 150-160 characters)
- **Event-Driven**: `MetaDescriptionChanged` event fires when description is updated
- **Reactive**: The `Page` component automatically updates the meta tag when the property changes

### MetaKeywords Property

- **Get/Set**: Read and write the meta keywords dynamically
- **SEO Support**: Helps categorize page content for search engines
- **Event-Driven**: `MetaKeywordsChanged` event fires when keywords are updated
- **Reactive**: The `Page` component automatically updates the meta tag when the property changes

### Future Extensibility

The `IPageService` interface can be extended in future versions to support additional `Page` object features:

- Open Graph meta tags for social media
- Page-level client script registration
- Page-level CSS registration
- Custom meta tags

## Key Differences from Web Forms

| Web Forms | Blazor with PageService | Notes |
|-----------|------------------------|-------|
| `Page.Title` property | `PageService.Title` property | Same concept, different access pattern |
| `Page.MetaDescription` property | `PageService.MetaDescription` property | Available in Web Forms .NET 4.0+ |
| `Page.MetaKeywords` property | `PageService.MetaKeywords` property | Available in Web Forms .NET 4.0+ |
| Available automatically | Must inject `IPageService` | Standard Blazor DI pattern |
| Synchronous | Synchronous | No change needed |
| Scoped to request | Scoped to render cycle | Similar lifecycle |

## Moving On

While `PageService` provides familiar Web Forms compatibility, consider these Blazor-native approaches:

### For Static Metadata

Use the built-in components directly:

```razor
@page "/about"

<PageTitle>About Us - My Company</PageTitle>
<HeadContent>
    <meta name="description" content="Learn about our company" />
    <meta name="keywords" content="about, company, team" />
</HeadContent>

<h1>About Us</h1>
```

### For Dynamic Metadata

The `PageService` approach is appropriate when:
- Title depends on data loaded asynchronously
- Title changes based on user actions
- Title is set in response to events
- You want Web Forms-style programmatic control

For simpler scenarios, you can use `<PageTitle>` with bound variables:

```razor
<PageTitle>@currentTitle</PageTitle>

@code {
    private string currentTitle = "Default Title";
    
    private void UpdateTitle(string newTitle)
    {
        currentTitle = newTitle;
    }
}
```

## Best Practices

1. **Set Title Early**: Set the title in `OnInitializedAsync` or `OnParametersSet` to ensure it's available before first render
2. **SEO Considerations**: Provide meaningful, descriptive titles for better search engine optimization
3. **User Context**: Include relevant context in the title (e.g., customer name, product name)
4. **Length**: Keep titles under 60 characters for optimal display in browser tabs and search results
5. **Consistent Pattern**: Use a consistent title format across your application (e.g., "Page Name - Site Name")

## See Also

- [Live Sample](https://blazorwebformscomponents.azurewebsites.net/UtilityFeatures/PageService)
- [Microsoft Docs: Page.Title Property](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.page.title?view=netframework-4.8)
- [Blazor PageTitle Component](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing#page-title)
