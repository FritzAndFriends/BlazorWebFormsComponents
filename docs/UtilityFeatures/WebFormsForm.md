# WebFormsForm

The **WebFormsForm** component wraps HTML form elements and provides `Request.Form` support in interactive Blazor Server mode. It bridges the gap between traditional Web Forms form submission (HTTP POST with `Request.Form`) and modern interactive Blazor (WebSocket-based with no HTTP POST).

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.htmlcontrols.htmlform?view=netframework-4.8

## Background

In ASP.NET Web Forms, form submissions were HTTP POST requests, and `Request.Form` was populated from the POST body:

```csharp
// Web Forms code-behind
protected void btnSubmit_Click(object sender, EventArgs e)
{
    string username = Request.Form["username"];
    string[] colors = Request.Form.GetValues("colors");
}
```

In interactive Blazor (WebSocket-based), there is **no HTTP POST** — all communication is bidirectional WebSocket. This means `Request.Form` would be empty. The `<WebFormsForm>` component captures form data via JavaScript interop and injects it into the `Request.Form` shim, enabling migrated code-behind to work unchanged.

## Use Cases

- **Interactive Blazor Server pages** using `<WebFormsForm>` to enable `Request.Form` access
- **SSR pages** that can continue using standard `<form>` (native HTTP POST, `Request.Form` works natively)
- **Gradual migration** from Web Forms forms to Blazor `EditForm` components

## Rendering Mode Behavior

| Mode | Approach | Request.Form Support |
|------|----------|----------------------|
| **SSR (Static Server Rendering)** | Use standard `<form>` with `[ExcludeFromInteractiveRouting]` | ✅ Native — HTTP POST populates `Request.Form` directly |
| **Interactive Blazor Server** | Use `<WebFormsForm>` component | ✅ JS interop captures form data, injected into `Request.Form` shim |
| **Target state (modern Blazor)** | Use `EditForm` + `@bind` | ✅ Typed binding — no need for `Request.Form` |

The component auto-detects the rendering mode and behaves appropriately.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Method` | `FormMethod` | `Post` | HTTP method (`Get` or `Post`) |
| `Action` | `string?` | `null` | Form action URL; `null` submits to the current page |
| `OnSubmit` | `EventCallback<FormSubmitEventArgs>` | — | Callback fired when the form is submitted; receives `FormSubmitEventArgs` containing `FormData` (name-value pairs) |
| `ChildContent` | `RenderFragment?` | — | Form content (input fields, buttons, etc.) |
| *(unmatched)* | — | — | Any additional HTML attributes pass through to the `<form>` element (e.g., `enctype`, `data-*`) |

## Syntax Comparison

=== "Web Forms"

    ```html
    <form runat="server">
        <asp:TextBox ID="txtUsername" runat="server" />
        <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
        <asp:CheckBox ID="chkRemember" runat="server" />
        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Login" />
    </form>
    ```

    ```csharp
    // Code-behind
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string username = Request.Form["txtUsername"];
        string password = Request.Form["txtPassword"];
        string remember = Request.Form["chkRemember"];
        
        // Process login...
    }
    ```

=== "Blazor (Interactive)"

    ```razor
    <WebFormsForm OnSubmit="HandleSubmit">
        <input type="text" name="txtUsername" />
        <input type="password" name="txtPassword" />
        <input type="checkbox" name="chkRemember" />
        <button type="submit">Login</button>
    </WebFormsForm>

    @code {
        private void HandleSubmit(FormSubmitEventArgs e)
        {
            SetRequestFormData(e);
            
            string username = Request.Form["txtUsername"];
            string password = Request.Form["txtPassword"];
            string remember = Request.Form["chkRemember"];
            
            // Process login...
        }
    }
    ```

## Migration Path

1. **Phase 1 — SSR with standard forms** (quickest path)
   - Keep existing `<form>` elements
   - Mark page with `[ExcludeFromInteractiveRouting]` to stay in SSR mode
   - `Request.Form` works natively via HTTP POST
   - No code changes needed

2. **Phase 2 — Interactive with WebFormsForm** (gradual adoption)
   - Remove `[ExcludeFromInteractiveRouting]`
   - Replace `<form>` with `<WebFormsForm>`
   - Add `OnSubmit` callback
   - Existing code-behind logic using `Request.Form` continues to work
   - Minimal rewrite

3. **Phase 3 — Modern Blazor** (target state)
   - Replace `<WebFormsForm>` with `<EditForm>`
   - Use `@bind` for two-way data binding
   - Eliminate `Request.Form` shim entirely
   - Full type safety and Blazor best practices

## Example: Login Form

=== "Web Forms"

    ```html
    <!-- LoginPage.aspx -->
    <form runat="server">
        <div>
            <label for="username">Username:</label>
            <asp:TextBox ID="username" runat="server" />
        </div>
        <div>
            <label for="password">Password:</label>
            <asp:TextBox ID="password" TextMode="Password" runat="server" />
        </div>
        <div>
            <asp:CheckBox ID="rememberMe" runat="server" Text="Remember me" />
        </div>
        <asp:Button ID="btnLogin" runat="server" OnClick="LoginClick" Text="Login" />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />
    </form>
    ```

    ```csharp
    // LoginPage.aspx.cs
    public partial class LoginPage : Page
    {
        protected void LoginClick(object sender, EventArgs e)
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string remember = Request.Form["rememberMe"];

            if (ValidateCredentials(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, remember == "on");
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                lblError.Text = "Invalid credentials";
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // Validation logic...
            return true;
        }
    }
    ```

=== "Blazor (Interactive)"

    ```razor
    <!-- Login.razor -->
    @page "/login"
    @inherits WebFormsPageBase
    @inject NavigationManager Nav
    @inject AuthenticationStateProvider AuthStateProvider

    <div class="login-form">
        <h2>Login</h2>

        <WebFormsForm OnSubmit="LoginClick">
            <div>
                <label for="username">Username:</label>
                <input type="text" id="username" name="username" required />
            </div>
            <div>
                <label for="password">Password:</label>
                <input type="password" id="password" name="password" required />
            </div>
            <div>
                <input type="checkbox" id="rememberMe" name="rememberMe" />
                <label for="rememberMe">Remember me</label>
            </div>
            <button type="submit">Login</button>
        </WebFormsForm>

        @if (!string.IsNullOrEmpty(_errorMessage))
        {
            <p style="color: red;">@_errorMessage</p>
        }
    </div>

    @code {
        private string _errorMessage = "";

        private async Task LoginClick(FormSubmitEventArgs e)
        {
            SetRequestFormData(e);

            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string remember = Request.Form["rememberMe"];

            if (ValidateCredentials(username, password))
            {
                // Sign in user...
                Nav.NavigateTo("/", forceLoad: true);
            }
            else
            {
                _errorMessage = "Invalid credentials";
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            // Validation logic...
            return true;
        }
    }
    ```

## How It Works

1. **Form Submission** — User submits the form via HTML submit button or programmatic submit
2. **JavaScript Interop** — `<WebFormsForm>` captures form data (all input fields) via `FormData` API
3. **OnSubmit Callback** — Captured data is passed to the `OnSubmit` event callback as `FormSubmitEventArgs`
4. **SetRequestFormData** — Call `SetRequestFormData(e)` to inject the captured data into the `Request.Form` shim
5. **Access via Request.Form** — Code-behind logic using `Request.Form["fieldName"]` works as expected

## Dual-Mode Support

**SSR Pages:**
```razor
@page "/form-page"
@attribute [ExcludeFromInteractiveRouting]

<form method="post" action="/form-page">
    <input type="text" name="username" />
    <button type="submit">Submit</button>
</form>
```

In SSR mode, the form submits via HTTP POST and `Request.Form` is automatically populated. No `<WebFormsForm>` or JavaScript interop needed.

**Interactive Pages:**
```razor
@page "/form-page"

<WebFormsForm OnSubmit="HandleSubmit">
    <input type="text" name="username" />
    <button type="submit">Submit</button>
</WebFormsForm>
```

In interactive mode, `<WebFormsForm>` captures data and injects it into `Request.Form`.

## Notes

- **HTML Attributes** — Unmatched HTML attributes (like `enctype`, `data-*`, `autocomplete`) are passed through to the rendered `<form>` element
- **File Uploads** — When using `enctype="multipart/form-data"`, ensure file inputs have `name` attributes so they are captured
- **Default Action** — If `Action` is `null`, the form submits to the current page (no full-page reload in interactive mode)
- **Event Bubbling** — The form does not automatically validate child components; add validation as needed in the `OnSubmit` callback
- **Accessibility** — Use standard form semantics (`<label>`, `for` attributes, `required`, `aria-*`) for accessibility

## Related Documentation

- [Request Shim](RequestShim.md) — Details on `Request.QueryString`, `Request.Cookies`, and `Request.Form` access
- [WebFormsPage](WebFormsPage.md) — Base page class providing `Request`, `Response`, and other Web Forms compatibility features
- [EditForm](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components) — Modern Blazor form component (target state)
