# ViewState and PostBack Shim

!!! warning "Migration Shim — Not a Destination"
    ViewState and IsPostBack are **migration compatibility features**. They exist so your Web Forms code-behind logic compiles and runs correctly in Blazor with minimal changes. Once your application is running, you should **refactor toward native Blazor patterns** — `[Parameter]` properties, component fields, and cascading values. See [Graduating Off ViewState](#graduating-off-viewstate) below.

## Overview

The ViewState and PostBack shim features enable seamless migration of ASP.NET Web Forms applications to Blazor by emulating the familiar `ViewState` dictionary and `IsPostBack` pattern. These features bridge the gap between traditional stateless HTTP POST workflows and Blazor's component-based stateful architecture.

### Key Features

- **ViewStateDictionary** — A state management class that persists data across postbacks, with automatic serialization for SSR and in-memory storage for ServerInteractive mode
- **Mode-Adaptive IsPostBack** — A property that automatically detects postback scenarios based on your render mode
- **Hidden Field Persistence** — Automatic round-tripping of ViewState through protected form fields in SSR
- **Form State Continuity** — Seamless state management across SSR and ServerInteractive transitions

### How This Differs from Web Forms ViewState

The original Web Forms ViewState was a source of well-known problems: page bloat, security vulnerabilities, and invisible performance costs. Our implementation is fundamentally different:

| Concern | Web Forms ViewState | BWFC ViewState Shim |
|---------|-------------------|-------------------|
| **Default behavior** | On for every control, always serialized | **Off by default** — opt-in per component |
| **Scope** | Single `__VIEWSTATE` blob for the entire page | **Per-component** isolated fields (`__bwfc_viewstate_{ID}`) |
| **Serialization** | Every render, even unchanged controls | **Dirty tracking** — skips serialization when nothing changed |
| **Security** | Unencrypted until .NET 4.5.2 MAC patch | **Encrypted + signed by default** (ASP.NET Core Data Protection, AES-256) |
| **Format** | Opaque binary (`LosFormatter`) | **JSON** — human-readable, debuggable |
| **Visibility** | No insight into payload size | **Size warnings** logged when threshold exceeded |
| **Interactive mode** | N/A | **In-memory only** — no serialization overhead |

---

## ViewStateDictionary

### What It Is

`ViewStateDictionary` is a dictionary-based state store that emulates the ASP.NET Web Forms `ViewState` pattern. It implements `IDictionary<string, object?>` with additional convenience methods for type-safe access.

In Web Forms, ViewState was serialized and embedded as a hidden field. In BlazorWebFormsComponents, ViewStateDictionary adapts to your rendering mode:

- **SSR (StaticSSR)** — Serializes to a protected hidden form field for round-tripping through HTTP POSTs
- **ServerInteractive** — Persists in component instance memory (equivalent to a private field)

### API Surface

#### Indexer (Dictionary Pattern)

```csharp
// Store a value
viewState["foo"] = bar;

// Retrieve a value (returns null if missing, no KeyNotFoundException)
object? value = viewState["foo"];
```

#### Type-Safe Convenience Methods

```csharp
// Store with automatic type conversion
ViewState.Set<string>("Name", "John");

// Retrieve with optional default value
string name = ViewState.GetValueOrDefault<string>("Name", "Anonymous");
int count = ViewState.GetValueOrDefault<int>("Count", 0);
```

#### State Tracking

```csharp
// Returns true if the dictionary has been modified
internal bool IsDirty { get; }

// Resets the dirty flag after serialization
internal void MarkClean();
```

#### Serialization (SSR Hidden Field Round-Trip)

```csharp
// Serialize to protected string for hidden form field
internal string Serialize(IDataProtector protector);

// Deserialize from protected string after form POST
internal static ViewStateDictionary Deserialize(string protectedPayload, IDataProtector protector);

// Merge state from another dictionary
internal void LoadFrom(ViewStateDictionary other);
```

### Usage Examples

#### Simple Counter (ServerInteractive)

```razor
@page "/counter"
@inherits BaseWebFormsComponent

<div>
    <p>Count: @(ViewState.GetValueOrDefault<int>("Counter", 0))</p>
    <Button Text="Increment" @onclick="OnIncrement" />
</div>

@code {
    private void OnIncrement()
    {
        int count = ViewState.GetValueOrDefault<int>("Counter", 0);
        ViewState.Set<int>("Counter", count + 1);
    }
}
```

**Migration Note:** In Blazor, prefer a simple `int` field instead:

```csharp
private int counter = 0;

private void OnIncrement()
{
    counter++;
}
```

#### Form with Hidden Field Persistence (SSR)

This example shows how ViewStateDictionary round-trips through SSR form POSTs.

**Web Forms (Before):**

```aspx
<%@ Page Language="C#" %>
<form runat="server">
    <asp:TextBox ID="ProductNameTextBox" runat="server" />
    <asp:Button ID="AddButton" Text="Add" runat="server" OnClick="AddButton_Click" />
    <asp:GridView ID="ProductsGrid" runat="server" />
    
    <script runat="server">
        protected void AddButton_Click(object sender, EventArgs e)
        {
            var products = (List<Product>)ViewState["Products"] ?? new();
            products.Add(new Product { Name = ProductNameTextBox.Text });
            ViewState["Products"] = products;
            ProductsGrid.DataSource = products;
            ProductsGrid.DataBind();
        }
    </script>
</form>
```

**Blazor SSR (After):**

```razor
@page "/products"
@inherits WebFormsPageBase

<form method="post">
    <TextBox ID="ProductNameTextBox" @bind-Value="_productName" />
    <Button ID="AddButton" Text="Add" @onclick="OnAddProduct" />
    <GridView ID="ProductsGrid" DataSource="@_products" AutoGenerateColumns="true" />
    
    <RenderViewStateField />
</form>

@code {
    private string _productName = "";
    private List<Product> _products = new();

    protected override void OnInitialized()
    {
        // Load products from ViewState if this is a postback
        if (IsPostBack)
        {
            var stored = ViewState.GetValueOrDefault<List<Product>>("Products");
            if (stored != null)
            {
                _products = stored;
            }
        }
    }

    private void OnAddProduct()
    {
        if (!string.IsNullOrEmpty(_productName))
        {
            _products.Add(new Product { Name = _productName });
            ViewState.Set("Products", _products);
            _productName = "";
        }
    }

    public class Product
    {
        public string Name { get; set; }
    }
}
```

#### Multi-Step Wizard with IsPostBack

```razor
@page "/wizard"
@inherits BaseWebFormsComponent

@if (CurrentStep == 1)
{
    <WizardStep1 @ref="_step1" />
}
else if (CurrentStep == 2)
{
    <WizardStep2 @ref="_step2" />
}
else
{
    <WizardSummary Data="@_wizardData" />
}

<div>
    @if (CurrentStep > 1)
    {
        <Button Text="Back" @onclick="GoBack" />
    }
    @if (CurrentStep < 3)
    {
        <Button Text="Next" @onclick="GoNext" />
    }
</div>

@code {
    private WizardData _wizardData = new();
    private int CurrentStep
    {
        get => ViewState.GetValueOrDefault<int>("CurrentStep", 1);
        set => ViewState.Set("CurrentStep", value);
    }

    protected override void OnInitialized()
    {
        // On first render (not postback), step is 1
        // On postback, step is restored from ViewState
        if (!IsPostBack)
        {
            _wizardData = new WizardData();
            ViewState.Set("WizardData", _wizardData);
        }
        else
        {
            _wizardData = ViewState.GetValueOrDefault<WizardData>("WizardData", new());
        }
    }

    private void GoNext()
    {
        CurrentStep++;
        ViewState.Set("WizardData", _wizardData);
    }

    private void GoBack()
    {
        CurrentStep--;
    }

    public class WizardData
    {
        public string Step1Data { get; set; }
        public string Step2Data { get; set; }
    }
}
```

---

## IsPostBack — Mode-Adaptive Detection

### What It Is

`IsPostBack` is a boolean property that indicates whether the current render is a postback or an initial render. It automatically adapts to your rendering mode:

- **SSR (StaticSSR)** — Returns `true` when the HTTP request method is POST (form submission)
- **ServerInteractive** — Returns `true` after the first initialization, indicating a re-render triggered by user interaction or state change

### Usage

```csharp
protected override void OnInitialized()
{
    if (!IsPostBack)
    {
        // First render: load initial data
        LoadData();
    }
    else
    {
        // Postback / re-render: restore from ViewState
        RestoreState();
    }
}
```

### API Reference

#### BaseWebFormsComponent

```csharp
/// <summary>
/// Returns <c>true</c> when the current request is a postback (form POST in SSR mode)
/// or after the first initialization (in ServerInteractive mode).
/// Matches the ASP.NET Web Forms <c>Page.IsPostBack</c> semantics.
/// </summary>
public bool IsPostBack { get; }
```

#### WebFormsPageBase

```csharp
/// <summary>
/// Always returns false. Blazor has no postback model.
/// Exists so that if (!IsPostBack) { ... } compiles and executes correctly —
/// the guarded block always runs, which is the correct behavior for
/// OnInitialized (first-render) context.
/// </summary>
public bool IsPostBack => false;
```

> **Note:** `WebFormsPageBase.IsPostBack` always returns `false` because pages in Blazor don't have HTTP-level postbacks. For page-level logic, use `OnInitialized` to detect first render instead.

### IsPostBack Detection Mechanisms

#### In SSR Mode

When running in SSR (pre-render or StaticSSR), IsPostBack checks the HTTP request method:

```csharp
if (HttpContextAccessor?.HttpContext is { } context)
    return HttpMethods.IsPost(context.Request.Method);
```

This allows forms submitted with `<form method="post">` to be detected as postbacks.

#### In ServerInteractive Mode

When running in ServerInteractive (interactive WebSocket mode), IsPostBack tracks component initialization:

```csharp
private bool _hasInitialized = false;

public bool IsPostBack => _hasInitialized;

protected override void OnInitialized()
{
    if (!_hasInitialized)
    {
        _hasInitialized = true;
    }
}
```

This enables the first render check (`if (!IsPostBack)`) to work as in Web Forms, guarding one-time initialization code.

---

## Hidden Field Persistence (SSR)

In SSR mode, ViewState is automatically serialized to a protected hidden form field and round-tripped through HTTP POSTs. This happens transparently without requiring manual form field management.

### How It Works

1. **Component Renders** — During SSR pre-render, ViewStateDictionary serializes its contents
2. **Hidden Field Emitted** — A protected hidden input is rendered (encrypted, HMAC-signed)
3. **Form Submits** — User submits `<form method="post">`
4. **Hidden Field Recovered** — The form POST is processed, hidden field contents are decrypted and verified
5. **ViewState Restored** — The deserialized ViewState is loaded into the component instance
6. **Component Re-renders** — Business logic runs with ViewState state restored

### Security

ViewState is protected using `IDataProtectionProvider`:

- **Encryption** — AES-256 (via IDataProtector)
- **Authentication** — HMAC-SHA256 signature (via IDataProtector)
- **No Tampering** — Encrypted payload fails decryption if modified

If decryption fails (corrupted or tampered data), a graceful fallback occurs and an empty ViewState is used.

### Manual Rendering (Advanced)

In rare cases, you may need to manually emit the ViewState hidden field. Use the `RenderViewStateField` utility:

```razor
<form method="post">
    <TextBox ID="Name" @bind-Value="_name" />
    <Button Text="Submit" @onclick="OnSubmit" />
    
    <!-- Manually emit protected ViewState hidden field -->
    <RenderViewStateField />
</form>

@code {
    private string _name = "";

    private void OnSubmit()
    {
        // Access ViewState as normal
        ViewState.Set("LastName", _name);
    }
}
```

> **Note:** `RenderViewStateField` is typically called automatically by the framework. Manual use is only needed for custom form layouts.

---

## Form State Continuity (SSR ↔ ServerInteractive)

A common migration pattern is to start with SSR (traditional form posts) and gradually add interactive regions via ServerInteractive. ViewStateDictionary enables seamless state sharing:

### Pattern: Progressive Enhancement

1. **Start with SSR Form** — Traditional HTML form with server-side POST handlers
2. **Add Interactive Button** — Replace a submit button with an interactive button
3. **Share State** — Both SSR form fields and interactive components read/write to ViewState
4. **Transition Gradually** — Move more logic to ServerInteractive over time

### Example: SSR Form + Interactive Counter

```razor
@page "/order"
@inherits BaseWebFormsComponent
@rendermode InteractiveServer

<form method="post">
    <div>
        <Label Text="Item Count:" />
        <!-- Update ViewState when user submits form -->
        <TextBox ID="ItemCount" @bind-Value="_itemCountText" />
    </div>

    <div>
        <!-- Or use interactive component to update ViewState -->
        <Label Text="Quick Add:" />
        <Button Text="+" @onclick="OnQuickAdd" />
        <Label Text="@(ViewState.GetValueOrDefault<int>("QuickCount", 0))" />
    </div>

    <Button ID="SubmitButton" Text="Place Order" @onclick="OnSubmit" Type="submit" />
</form>

@code {
    private string _itemCountText = "1";

    protected override void OnInitialized()
    {
        if (IsPostBack)
        {
            // Form was submitted: read values posted by form fields
            if (int.TryParse(_itemCountText, out var count))
            {
                ViewState.Set("ItemCount", count);
            }
        }
        else
        {
            // First render: restore from ViewState or use defaults
            var itemCount = ViewState.GetValueOrDefault<int>("ItemCount", 1);
            _itemCountText = itemCount.ToString();
        }
    }

    private void OnQuickAdd()
    {
        var count = ViewState.GetValueOrDefault<int>("QuickCount", 0);
        ViewState.Set("QuickCount", count + 1);
    }

    private void OnSubmit()
    {
        // Save state for next postback
        if (int.TryParse(_itemCountText, out var count))
        {
            ViewState.Set("ItemCount", count);
        }
    }
}
```

---

## Migration Path: From Web Forms ViewState

### Before (Web Forms)

```csharp
// Page code-behind
public partial class ProductPage : System.Web.UI.Page
{
    protected void Page_Load()
    {
        if (!IsPostBack)
        {
            ViewState["Products"] = LoadProducts();
        }
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        var products = (List<Product>)ViewState["Products"];
        products.Add(new Product { Name = ProductNameTextBox.Text });
        ViewState["Products"] = products;
    }
}
```

### After (Blazor SSR — Minimal Changes)

```razor
@page "/products"
@inherits WebFormsPageBase

<form method="post">
    <TextBox ID="ProductNameTextBox" @bind-Value="_productName" />
    <Button ID="AddButton" Text="Add" @onclick="OnAddProduct" />
    <RenderViewStateField />
</form>

@code {
    private string _productName = "";

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            ViewState["Products"] = LoadProducts();
        }
    }

    private void OnAddProduct()
    {
        var products = (List<Product>)ViewState["Products"];
        products.Add(new Product { Name = _productName });
        ViewState["Products"] = products;
    }

    private List<Product> LoadProducts() => new();

    public class Product
    {
        public string Name { get; set; }
    }
}
```

### After (Blazor ServerInteractive — Refactored)

```razor
@page "/products"
@inherits BaseWebFormsComponent
@rendermode InteractiveServer

<div>
    <TextBox ID="ProductNameTextBox" @bind-Value="_productName" />
    <Button Text="Add" @onclick="OnAddProduct" />
</div>

@code {
    private string _productName = "";
    private List<Product> _products = new();

    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            _products = LoadProducts();
        }
    }

    private void OnAddProduct()
    {
        if (!string.IsNullOrEmpty(_productName))
        {
            _products.Add(new Product { Name = _productName });
            _productName = "";
        }
    }

    private List<Product> LoadProducts() => new();

    public class Product
    {
        public string Name { get; set; }
    }
}
```

---

## Best Practices

### ✅ Do

- **Use ViewState for state that survives postbacks** — Ideal for SSR migration where forms round-trip
- **Use IsPostBack to guard one-time initialization** — Load data on first render, restore on postback
- **Prefer typed fields for new code** — `private int counter` is clearer than `ViewState["counter"]`
- **Scope ViewState to the component** — Each component instance has its own ViewState
- **Use `GetValueOrDefault<T>`** — Type-safe retrieval with optional defaults

### ❌ Don't

- **Don't use ViewState for large objects** — Serialization overhead grows with data size
- **Don't store non-serializable objects** — ViewState uses JSON serialization
- **Don't rely on ViewState in ServerInteractive after navigation** — ViewState is per-component-instance
- **Don't mix Web Forms Page-level ViewState with component ViewState** — They are separate
- **Don't assume ViewState survives component disposal** — Values are lost when component unmounts

---

## Rendering Modes Reference

| Aspect | SSR (StaticSSR) | ServerInteractive |
|---|---|---|
| **HTTP Context** | Available | Unavailable |
| **ViewState Storage** | Protected hidden field | Component memory |
| **ViewState Serialization** | Automatic JSON + encryption | In-memory only |
| **IsPostBack Detection** | `HttpMethods.IsPost()` | `_hasInitialized` flag |
| **Form Submission** | Traditional HTML POST | JavaScript event, no form submission |
| **Use Case** | Legacy form migration | Interactive features |

---

## Graduating Off ViewState

ViewState gets your Web Forms code running in Blazor. The next step is refactoring to native Blazor patterns. Here's how to migrate each common ViewState usage:

### Simple Values → Component Fields

=== "ViewState (Migration)"

    ```csharp
    // Web Forms pattern preserved during migration
    public int SelectedDepartmentId
    {
        get => ViewState.GetValueOrDefault<int>("SelectedDepartmentId");
        set => ViewState.Set("SelectedDepartmentId", value);
    }
    ```

=== "Native Blazor (Target)"

    ```csharp
    // Refactored: simple field, no serialization overhead
    private int _selectedDepartmentId;
    ```

### First-Load Guards → OnInitialized

=== "ViewState (Migration)"

    ```csharp
    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            ViewState["Products"] = LoadProducts();
        }
    }
    ```

=== "Native Blazor (Target)"

    ```csharp
    // OnInitialized already runs once per component instance
    protected override void OnInitialized()
    {
        _products = LoadProducts();
    }
    ```

### Cross-Component State → Cascading Values or DI Services

=== "ViewState (Migration)"

    ```csharp
    // Parent stores state in ViewState, child reads it
    ViewState["SelectedCategory"] = category;
    ```

=== "Native Blazor (Target)"

    ```razor
    <!-- Parent cascades value to children -->
    <CascadingValue Value="@_selectedCategory">
        @ChildContent
    </CascadingValue>
    ```

### When ViewState Is Still Appropriate

ViewState remains useful for **SSR form round-trips** where you need state to survive an HTTP POST without JavaScript. This is a legitimate pattern in SSR Blazor — similar to hidden fields in any web framework. The key difference from Web Forms: you're choosing to use it, not having it imposed on every control.

---

## See Also

- [WebFormsPage](WebFormsPage.md) — Page-level wrapper combining naming scope and theming
- [ResponseRedirect](ResponseRedirect.md) — Response.Redirect shim for page navigation
- [Migration Guides](../Migration/) — Step-by-step Web Forms to Blazor conversion patterns
- [Blazor Render Modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/render-modes) — Microsoft Learn guide
