# Inline C# Expression Migration

Web Forms uses several inline expression syntaxes in ASPX and ASCX markup. Each syntax serves a different purpose and has direct Blazor/Razor equivalents. This guide covers the migration of all inline expression types from Web Forms to Blazor.

!!! note "Expression Overview"
    Web Forms supported five inline expression syntaxes: `<%= %>`, `<%: %>`, `<%# %>`, `<% %>`, and `<%$ %>`. Each has a specific role. Understanding these differences is key to successful migration.

---

## Code Render Blocks (`<%= ... %>`)

### What It Was

Code render blocks output the result of a C# expression directly to the page as **raw, unencoded HTML**:

```html
<td><%= Request.QueryString["id"] %></td>
<span><%= DateTime.Now.Year %></span>
<div><%= "<strong>Bold Text</strong>" %></div>
```

The result is inserted directly into the HTML output without any encoding.

### Blazor Equivalent

In Blazor/Razor, the `@()` expression syntax replaces `<%= %>`. **Important:** `@()` performs **HTML encoding by default**, which is safer than Web Forms' raw output:

=== "Web Forms"

    ```html
    <td><%= Request.QueryString["id"] %></td>
    <span><%= DateTime.Now.Year %></span>
    <div><%= "<strong>Bold Text</strong>" %></div>
    ```

=== "Blazor"

    ```razor
    <td>@(NavigationManager.Uri.GetQueryParameter("id"))</td>
    <span>@DateTime.Now.Year</span>
    <div>@("<strong>Bold Text</strong>")</div>
    ```

!!! warning "XSS Risk and HTML Encoding"
    `<%= %>` outputs **raw unencoded HTML**. This is a security risk if the output comes from user input. Blazor's `@()` is **HTML-encoded by default** — safe for user data.
    
    If you intentionally need raw HTML (e.g., from a trusted source like a database), use `@((MarkupString)rawHtml)`:
    
    ```razor
    @((MarkupString)userGeneratedHtml)  <!-- Only use with trusted content! -->
    ```

### Request Object Access

Web Forms' `Request` object gives access to query strings, form data, and cookies. In Blazor:

=== "Web Forms"

    ```html
    <!-- Query String -->
    <span><%= Request.QueryString["id"] %></span>
    
    <!-- Form Data -->
    <span><%= Request.Form["username"] %></span>
    
    <!-- Cookies -->
    <span><%= Request.Cookies["sessionid"]?.Value %></span>
    ```

=== "Blazor"

    ```razor
    <!-- Query String with NavigationManager -->
    <span>@(NavigationManager.Uri.Contains("id=") ? NavigationManager.Uri.Split("id=")[1].Split("&")[0] : "")</span>
    
    <!-- Or use [SupplyParameterFromQuery] in code -->
    @code {
        [SupplyParameterFromQuery]
        public string? Id { get; set; }
    }
    
    <!-- Form Data: Use @bind with input elements -->
    <input type="text" @bind="username" />
    
    <!-- Cookies with IHttpContextAccessor -->
    @inject IHttpContextAccessor HttpContextAccessor
    
    @{
        var sessionId = HttpContextAccessor?.HttpContext?.Request.Cookies["sessionid"];
    }
    <span>@sessionId</span>
    ```

!!! tip "Prefer Parameter Binding"
    Instead of parsing `Request.QueryString` manually, use `[SupplyParameterFromQuery]` attributes on component parameters. This is cleaner, type-safe, and more performant.

---

## HTML-Encoded Output (`<%: ... %>`)

### What It Was

HTML-encoded output blocks automatically HTML-encode the expression result before rendering:

```html
<p><%: userComment %></p>
<span><%: "3 < 5" %></span>
```

This was a safer alternative to `<%= %>` because it prevented XSS attacks by encoding special characters.

### Blazor Equivalent

In Blazor, `@()` is **HTML-encoded by default**. This means you can use `@()` for the same security benefit:

=== "Web Forms"

    ```html
    <p><%: userComment %></p>
    <span><%: "3 < 5" %></span>
    <div><%: "<script>alert('xss')</script>" %></div>
    ```

=== "Blazor"

    ```razor
    <p>@userComment</p>
    <span>@("3 < 5")</span>
    <div>@("<script>alert('xss')</script>")</div>
    ```

!!! tip "Default Safety"
    Blazor's default behavior (`@value`) provides the safety of `<%: %>` without extra syntax. Always use `@value` for user-generated content and reserve `@((MarkupString)value)` only for trusted sources.

---

## Data-Binding Expressions (`<%# ... %>`)

### What It Was

Data-binding expressions were used in templates (ItemTemplate, EditTemplate, etc.) to output data from the current item in a data-bound control:

```html
<asp:Repeater DataSource="<%# Products %>">
  <ItemTemplate>
    <tr>
      <td><%# Eval("ProductName") %></td>
      <td><%# Eval("Price", "{0:C}") %></td>
      <td><%# Item.StockLevel %></td>
      <td>
        <asp:TextBox Text='<%# Bind("ProductName") %>' runat="server" />
      </td>
    </tr>
  </ItemTemplate>
</asp:Repeater>
```

The `Eval()` method performed one-way data binding (output only), while `Bind()` performed two-way binding (output + update).

### Blazor Equivalent: Output Only

For repeating controls like `<Repeater>`, use the implicit `@context` parameter to access the current item:

=== "Web Forms (Eval)"

    ```html
    <asp:Repeater DataSource="<%# Products %>">
      <ItemTemplate>
        <tr>
          <td><%# Eval("ProductName") %></td>
          <td><%# Eval("Price", "{0:C}") %></td>
          <td><%# Container.DataItem %></td>
        </tr>
      </ItemTemplate>
    </asp:Repeater>
    ```

=== "Blazor"

    ```razor
    <Repeater Items="Products">
      <ItemTemplate>
        <tr>
          <td>@context.ProductName</td>
          <td>@context.Price.ToString("C")</td>
          <td>@context</td>
        </tr>
      </ItemTemplate>
    </Repeater>
    ```

!!! note "Context Parameter"
    By default, the current item in a template is accessed via the `@context` variable. You can rename it with `Context="Item"` if you prefer: `<Repeater Context="Item">` → `@Item.ProductName`.

### Blazor Equivalent: Two-Way Binding

Replace `Bind()` with the `@bind-Value` directive:

=== "Web Forms (Bind)"

    ```html
    <asp:Repeater DataSource="<%# Products %>">
      <ItemTemplate>
        <tr>
          <td>
            <asp:TextBox Text='<%# Bind("ProductName") %>' runat="server" />
          </td>
        </tr>
      </ItemTemplate>
    </asp:Repeater>
    ```

=== "Blazor"

    ```razor
    <Repeater Items="Products" Context="Item">
      <ItemTemplate>
        <tr>
          <td>
            <TextBox @bind-Value="Item.ProductName" />
          </td>
        </tr>
      </ItemTemplate>
    </Repeater>
    ```

### Complex Formatting

For complex formatting beyond a simple format string, use C# methods:

=== "Web Forms"

    ```html
    <%# string.Format("{0:D2}/{1:D2}/{2}", 
        Eval("Month"), Eval("Day"), Eval("Year")) %>
    
    <%# Eval("Price", "{0:C}") %>
    ```

=== "Blazor"

    ```razor
    @($"{context.Month:D2}/{context.Day:D2}/{context.Year}")
    
    @context.Price.ToString("C")
    ```

---

## Code Blocks (`<% ... %>`)

### What It Was

Code blocks executed arbitrary C# code without outputting anything:

```html
<% if (User.IsInRole("Admin")) { %>
  <button>Delete</button>
<% } %>

<% foreach (var item in Items) { %>
  <div><%# item.Name %></div>
<% } %>
```

This pattern mixed logic with markup, leading to difficult-to-maintain code.

### Blazor Equivalent

Blazor provides `@if`, `@foreach`, and other control flow directives:

=== "Web Forms"

    ```html
    <% if (User.IsInRole("Admin")) { %>
      <button>Delete</button>
    <% } %>
    
    <% foreach (var item in Items) { %>
      <div><%# item.Name %></div>
    <% } %>
    
    <% for (int i = 0; i < 5; i++) { %>
      <span><%# i %></span>
    <% } %>
    ```

=== "Blazor"

    ```razor
    @if (User?.IsInRole("Admin") == true)
    {
      <button>Delete</button>
    }
    
    @foreach (var item in Items)
    {
      <div>@item.Name</div>
    }
    
    @for (int i = 0; i < 5; i++)
    {
      <span>@i</span>
    }
    ```

!!! tip "Code Organization"
    Blazor makes it easy to move complex logic to methods in the `@code` block instead of embedding it in markup. This improves readability and testability.

### Conditional Rendering

Web Forms used code blocks for conditional rendering. Blazor uses `@if`:

=== "Web Forms"

    ```html
    <% if (Product.InStock) { %>
      <button>Add to Cart</button>
    <% } else { %>
      <p>Out of Stock</p>
    <% } %>
    ```

=== "Blazor"

    ```razor
    @if (Product.InStock)
    {
      <button>Add to Cart</button>
    }
    else
    {
      <p>Out of Stock</p>
    }
    ```

---

## Expression Builders (`<%$ ... %>`)

### What It Was

Expression builders accessed application configuration at compile time:

```html
<!-- Connection Strings -->
<%$ ConnectionStrings:DefaultConnection %>

<!-- App Settings -->
<%$ AppSettings:SiteTitle %>

<!-- Resources (localization) -->
<%$ Resources:Labels, WelcomeMessage %>
```

### Blazor Equivalent

Blazor uses **dependency injection** and the **`IConfiguration` service** instead:

=== "Web Forms"

    ```html
    <!-- Connection String -->
    <%$ ConnectionStrings:DefaultConnection %>
    
    <!-- App Setting -->
    <%$ AppSettings:SiteTitle %>
    
    <!-- In code-behind: -->
    string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    ```

=== "Blazor"

    ```razor
    @inject IConfiguration Configuration
    
    <!-- Connection String -->
    @Configuration.GetConnectionString("DefaultConnection")
    
    <!-- App Setting -->
    @Configuration["SiteTitle"]
    
    <!-- In code block: -->
    @code {
        private string connectionString = "";
        
        protected override void OnInitialized()
        {
            connectionString = Configuration.GetConnectionString("DefaultConnection");
        }
    }
    ```

### Localization (Resources)

For localized strings, use `IStringLocalizer`:

=== "Web Forms"

    ```html
    <%$ Resources:Labels, WelcomeMessage %>
    ```

=== "Blazor"

    ```razor
    @inject IStringLocalizer<App> Localizer
    
    @Localizer["WelcomeMessage"]
    ```

---

## Page Properties and Global Objects

### Page.Title

=== "Web Forms"

    ```csharp
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "Product Details";
    }
    ```

=== "Blazor"

    ```razor
    @page "/product/{id}"
    
    <PageTitle>Product Details</PageTitle>
    ```

### User Identity

=== "Web Forms"

    ```html
    <span><%= User.Identity.Name %></span>
    <% if (User.IsInRole("Admin")) { %>
      <button>Manage</button>
    <% } %>
    ```

=== "Blazor"

    ```razor
    @inject AuthenticationStateProvider AuthenticationStateProvider
    
    @if (authState?.User?.Identity?.IsAuthenticated == true)
    {
      <span>@authState.User.Identity.Name</span>
    }
    
    @if (authState?.User?.IsInRole("Admin") == true)
    {
      <button>Manage</button>
    }
    
    @code {
        private AuthenticationState authState;
        
        protected override async Task OnInitializedAsync()
        {
            authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        }
    }
    ```

---

## Automated Migration with `bwfc-migrate.ps1`

The [automated migration script](AutomatedMigration.md) handles many expression conversions automatically:

| Expression | Before | After |
|-----------|--------|-------|
| Code render | `<%= value %>` | `@(value)` |
| HTML-encoded | `<%: value %>` | `@(value)` |
| Data binding | `<%# Eval("Prop") %>` | `@context.Prop` |
| Data binding with format | `<%# Eval("Price", "{0:C}") %>` | `@context.Price.ToString("C")` |
| Code blocks | `<% if (...) { %>` | `@if (...) {` |
| Bind method | `<%# Bind("Prop") %>` | `@bind-Value="@context.Prop"` |
| Comments | `<%-- text --%>` | `@* text *@` |

!!! note "Script Limitations"
    The automated script handles simple, direct expression conversions. Complex expressions, method calls, and custom logic may require manual adjustment. Always review migrated code for correctness.

---

## Common Patterns and Gotchas

### Ternary Expressions

=== "Web Forms"

    ```html
    <span><%= Product.InStock ? "Available" : "Out of Stock" %></span>
    <span><%: Product.Price > 100 ? "Premium" : "Standard" %></span>
    ```

=== "Blazor"

    ```razor
    <span>@(Product.InStock ? "Available" : "Out of Stock")</span>
    <span>@(Product.Price > 100 ? "Premium" : "Standard")</span>
    ```

### String Concatenation

=== "Web Forms"

    ```html
    <a href="<%= "/products/detail?id=" + Product.Id %>">
      <%= Product.Name %>
    </a>
    ```

=== "Blazor"

    ```razor
    <a href="@($"/products/detail?id={Product.Id}")">
      @Product.Name
    </a>
    ```

### Method Calls in Markup

=== "Web Forms"

    ```html
    <span><%# GetFormattedPrice(container.DataItem) %></span>
    <span><%= CalculateTotal(items) %></span>
    ```

=== "Blazor"

    ```razor
    <span>@GetFormattedPrice(context)</span>
    <span>@CalculateTotal(items)</span>
    
    @code {
        private string GetFormattedPrice(Product product)
        {
            return product.Price.ToString("C");
        }
        
        private decimal CalculateTotal(IEnumerable<Product> items)
        {
            return items.Sum(i => i.Price);
        }
    }
    ```

### Accessing Container Properties

=== "Web Forms"

    ```html
    <asp:Repeater DataSource="<%# Items %>">
      <ItemTemplate>
        <span><%# Container.ItemIndex %></span>
        <span><%# Container.DataItem %></span>
      </ItemTemplate>
    </asp:Repeater>
    ```

=== "Blazor"

    ```razor
    @foreach (var item in Items)
    {
      <span>@Items.IndexOf(item)</span>
      <span>@item</span>
    }
    
    <!-- Or with Repeater context: -->
    <Repeater Items="Items">
      <ItemTemplate>
        <!-- context is the current item -->
      </ItemTemplate>
    </Repeater>
    ```

---

## What Requires Manual Migration

Some patterns cannot be fully automated and require manual attention:

### Session State in Expressions

```html
<!-- Web Forms -->
<span><%= (string)Session["UserName"] %></span>

<!-- Blazor: Inject a custom service or use distributed caching -->
@inject ISessionService SessionService

<span>@(await SessionService.GetAsync<string>("UserName"))</span>
```

### Complex LINQ Queries in Markup

Move complex queries to the code block:

```razor
@code {
    private List<Product> FilteredProducts => 
        Products.Where(p => p.InStock && p.Price < 100).ToList();
}

@foreach (var product in FilteredProducts)
{
    <div>@product.Name</div>
}
```

### DataSource Controls

Web Forms `<asp:SqlDataSource>` and similar controls have no Blazor equivalent. Replace with **injected services**:

```razor
@inject ProductService ProductService

@code {
    private List<Product> Products = new();
    
    protected override async Task OnInitializedAsync()
    {
        Products = await ProductService.GetProductsAsync();
    }
}
```

### Custom Expression Builders

If you created custom expression builders, you'll need to migrate this logic to **IConfiguration** or custom services.

---

## See Also

- [AutomatedMigration.md](AutomatedMigration.md) — Script capabilities and conversion table
- [Strategies.md](Strategies.md) — Broader migration patterns
- [Databinder.md](../UtilityFeatures/Databinder.md) — Data-binding in detail
- [DeprecationGuidance.md](DeprecationGuidance.md) — Other deprecated Web Forms patterns
