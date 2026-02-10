# PageService

In ASP.NET Web Forms, the `Page` object provided a central place for page-level properties and functionality. Key properties included `Page.Title`, `Page.MetaDescription`, and `Page.MetaKeywords`, which allowed developers to programmatically set page metadata that appears in the browser title bar and search engine results.

```csharp
// Web Forms code-behind
protected void Page_Load(object sender, EventArgs e)
{
    Page.Title = "My Dynamic Page Title";
    Page.MetaDescription = "Description for search engines";
    Page.MetaKeywords = "keyword1, keyword2, keyword3";
}
```

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.page.title?view=netframework-4.8

## Blazor Implementation

BlazorWebFormsComponents provides `IPageService` and `PageService` to replicate this functionality in Blazor. The service is registered as a scoped service (one instance per circuit/request) and can be injected into any component or page. A companion `<Page />` component renders the dynamic `<PageTitle>` and `<meta>` tags.

### Key Components

| Component | Purpose |
|-----------|---------|
| `IPageService` | Interface defining `Title`, `MetaDescription`, `MetaKeywords` properties and change events |
| `PageService` | Default implementation registered as a scoped service |
| `<Page />` | Blazor component that renders `<PageTitle>` and `<HeadContent>` meta tags reactively |

## Setup

### Step 1: Register the Service

The service is automatically registered when you call `AddBlazorWebFormsComponents()`:

```csharp
// Program.cs
builder.Services.AddBlazorWebFormsComponents();
```

This registers `IPageService` as a scoped service that can be injected into any component.

### Step 2: Add the Page Component

Add the `<Page />` component to your layout or to individual pages where you need dynamic metadata:

```razor
@inject IPageService PageService

<Page />
```

The `<Page />` component listens for property changes on `IPageService` and automatically re-renders the `<PageTitle>` and `<meta>` tags.

## Usage

### Setting Page Metadata

```razor
@page "/products"
@inject IPageService PageService

<Page />

<h1>Products</h1>

@code {
    protected override void OnInitialized()
    {
        PageService.Title = "Products - My Store";
        PageService.MetaDescription = "Browse our product catalog";
        PageService.MetaKeywords = "products, catalog, store";
    }
}
```

### Dynamic Metadata Based on Data

```razor
@page "/customer/{Id:int}"
@inject IPageService PageService

<Page />

<h1>@customerName</h1>

@code {
    [Parameter]
    public int Id { get; set; }

    private string customerName = "";

    protected override async Task OnInitializedAsync()
    {
        var customer = await GetCustomer(Id);
        customerName = customer.Name;

        PageService.Title = $"Customer Details - {customerName}";
        PageService.MetaDescription = $"View details for {customerName}";
        PageService.MetaKeywords = "customer, details, crm";
    }
}
```

### Updating Metadata on User Action

```razor
@page "/editor"
@inject IPageService PageService

<Page />

<input @bind="newTitle" placeholder="Enter page title" />
<Button Text="Update Title" OnClick="UpdateTitle" />

@code {
    private string newTitle = "";

    protected override void OnInitialized()
    {
        PageService.Title = "Editor - My App";
    }

    void UpdateTitle()
    {
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            PageService.Title = newTitle;
        }
    }
}
```

## IPageService Reference

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Page title displayed in the browser tab. Equivalent to `Page.Title` in Web Forms |
| `MetaDescription` | `string` | Meta description for SEO. Equivalent to `Page.MetaDescription` (.NET 4.0+) |
| `MetaKeywords` | `string` | Meta keywords for SEO. Equivalent to `Page.MetaKeywords` (.NET 4.0+) |

| Event | Type | Description |
|-------|------|-------------|
| `TitleChanged` | `EventHandler<string>` | Raised when the `Title` property changes |
| `MetaDescriptionChanged` | `EventHandler<string>` | Raised when `MetaDescription` changes |
| `MetaKeywordsChanged` | `EventHandler<string>` | Raised when `MetaKeywords` changes |

## HTML Output

When `PageService.Title`, `MetaDescription`, and `MetaKeywords` are set, the `<Page />` component renders:

```html
<title>My Dynamic Page Title</title>
<meta name="description" content="Description for search engines" />
<meta name="keywords" content="keyword1, keyword2, keyword3" />
```

The output uses Blazor's built-in `<PageTitle>` and `<HeadContent>` components, so the tags are injected into the `<head>` section of the document.

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Register the service** — Call `builder.Services.AddBlazorWebFormsComponents()` in `Program.cs`
2. **Add `<Page />` component** — Place it in your layout or page
3. **Inject `IPageService`** — Replace `Page.Title` with `PageService.Title`
4. **No `Page_Load` equivalent** — Set properties in `OnInitialized` or `OnInitializedAsync`

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
@page "/customer/{Id:int}"
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
        PageService.MetaDescription = $"View details for {customerName}";
        PageService.MetaKeywords = "customer, details, crm";
    }
}
```

## Key Differences from Web Forms

| Web Forms | Blazor with PageService | Notes |
|-----------|------------------------|-------|
| `Page.Title` | `PageService.Title` | Same concept, different access pattern |
| `Page.MetaDescription` | `PageService.MetaDescription` | Available in Web Forms .NET 4.0+ |
| `Page.MetaKeywords` | `PageService.MetaKeywords` | Available in Web Forms .NET 4.0+ |
| Available automatically | Must inject `IPageService` | Standard Blazor DI pattern |
| Scoped to HTTP request | Scoped to circuit/request | Similar lifecycle |

## Moving On

While `PageService` provides familiar Web Forms compatibility, consider these Blazor-native approaches for new code:

### Static Metadata

Use the built-in components directly:

```razor
@page "/about"

<PageTitle>About Us - My Company</PageTitle>
<HeadContent>
    <meta name="description" content="Learn about our company" />
    <meta name="keywords" content="about, company, team" />
</HeadContent>
```

### Dynamic Metadata

The `PageService` approach is appropriate when metadata depends on runtime data or user actions. For simpler scenarios, bind directly to variables:

```razor
<PageTitle>@currentTitle</PageTitle>
<HeadContent>
    <meta name="description" content="@currentDescription" />
</HeadContent>

@code {
    private string currentTitle = "Default Title";
    private string currentDescription = "Default description";
}
```

!!! tip "Best Practices"
    - Set title in `OnInitialized` or `OnInitializedAsync` to ensure it's available before first render
    - Keep titles under 60 characters for optimal browser tab and search result display
    - Keep meta descriptions between 150–160 characters for search engines
    - Use a consistent title format across your application (e.g., "Page Name - Site Name")

## See Also

- [Microsoft Docs: Page.Title Property](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.page.title?view=netframework-4.8)
- [Blazor PageTitle Component](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing#page-title)
