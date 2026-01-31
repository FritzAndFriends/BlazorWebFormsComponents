# PageService

## Background

In ASP.NET Web Forms, the `Page` object provided a central place for page-level properties and functionality. The most commonly used property was `Page.Title`, which allowed developers to programmatically set the HTML page title that appears in the browser's title bar or tab.

```csharp
// Web Forms code-behind
protected void Page_Load(object sender, EventArgs e)
{
    Page.Title = "My Dynamic Page Title";
}

protected void UpdateButton_Click(object sender, EventArgs e)
{
    Page.Title = "Title Updated - " + DateTime.Now.ToString();
}
```

This pattern was essential for:
- Setting page titles dynamically based on data or user actions
- Implementing SEO-friendly titles for content pages
- Providing context-specific titles in master page scenarios

## Web Forms Usage

In Web Forms, the `Page` object was automatically available in all code-behind files:

```aspx
<%@ Page Language="C#" Title="Static Title" %>
```

```csharp
// Code-behind (.aspx.cs)
public partial class MyPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Dynamically set the title
        Page.Title = GetTitleFromDatabase();
    }
    
    private string GetTitleFromDatabase()
    {
        // Fetch from database
        return "Dynamic Title from DB";
    }
}
```

The `Page.Title` property would automatically update the HTML `<title>` element in the rendered page.

## Blazor Implementation

BlazorWebFormsComponents provides `IPageService` and `PageService` to replicate this functionality in Blazor. The service is registered as a scoped service (one instance per request/render cycle) and can be injected into any component or page.

### Key Components

1. **IPageService Interface** - Defines the contract for page-level services
2. **PageService Class** - Default implementation providing `Title` property
3. **Page Component** - Blazor component that renders the dynamic `<PageTitle>` element

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

<Page />  @* This renders the dynamic <PageTitle> component *@
```

**Step 2: Set the title programmatically**

```razor
@code {
    protected override void OnInitialized()
    {
        PageService.Title = "My Dynamic Page Title";
    }
    
    private void UpdateTitle()
    {
        PageService.Title = "Updated Title - " + DateTime.Now.ToString();
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
    <button @onclick="UpdatePageTitle">Update Title</button>
</div>

@code {
    private string newTitle = "";
    
    protected override void OnInitialized()
    {
        PageService.Title = "My Page - BlazorWebFormsComponents";
    }
    
    private void UpdatePageTitle()
    {
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            PageService.Title = newTitle;
        }
    }
}
```

## Migration Path

### Before (Web Forms)

```aspx
<%@ Page Language="C#" MasterPageFile="~/Site.Master" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "Customer Details - " + GetCustomerName();
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
    }
}
```

## Features

### Title Property

- **Get/Set**: Read and write the page title dynamically
- **Event-Driven**: `TitleChanged` event fires when title is updated
- **Reactive**: The `Page` component automatically updates the browser title when the property changes

### Future Extensibility

The `IPageService` interface is designed to support additional `Page` object features in future versions:

- Meta tags (description, keywords, Open Graph tags)
- Page-level client script registration
- Page-level CSS registration
- Other page metadata

## Key Differences from Web Forms

| Web Forms | Blazor with PageService | Notes |
|-----------|------------------------|-------|
| `Page.Title` property | `PageService.Title` property | Same concept, different access pattern |
| Available automatically | Must inject `IPageService` | Standard Blazor DI pattern |
| Synchronous | Synchronous | No change needed |
| Scoped to request | Scoped to render cycle | Similar lifecycle |

## Moving On

While `PageService` provides familiar Web Forms compatibility, consider these Blazor-native approaches:

### For Static Titles

Use the built-in `<PageTitle>` component directly:

```razor
@page "/about"

<PageTitle>About Us - My Company</PageTitle>

<h1>About Us</h1>
```

### For Dynamic Titles

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
