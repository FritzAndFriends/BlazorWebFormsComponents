---
name: "blazor-form-submission"
description: "Provides workaround patterns for Blazor Server enhanced navigation silently stripping onclick attributes from button elements, causing form submissions to fail. Covers anchor-based form submission for HTTP POST endpoints and EditForm for in-component handling. Use when a form submit button does nothing on click, when building auth forms that POST to minimal API endpoints, or when choosing between HTML form POST and Blazor EditForm for data entry pages."
domain: "migration"
confidence: "low"
source: "earned"
---

## Context

Discovered in WingtipToys Run 7, Iteration 3. Blazor's enhanced navigation intercepts and strips `onclick` attributes from `<button>` elements during page enhancement. This causes form submissions to silently fail — no error, no submission, the button just does nothing.

This is a first observation from Run 7. Confidence is low — the pattern works but may not be the canonical long-term solution. Monitor Blazor framework updates for native fixes.

---

## ⚠️ Core Problem

Blazor Server's enhanced navigation processes DOM elements after render. During this process, `onclick` attributes on `<button>` elements are stripped. If your form relies on `<button onclick="...">` to submit, it silently breaks.

**Symptoms:**
- Form has a submit button that renders correctly in HTML
- Clicking the button does nothing — no navigation, no POST, no error
- DevTools shows the `onclick` attribute is missing from the live DOM

---

## Patterns

### Pattern 1: Anchor-Based Form Submission (for HTTP POST endpoints)

Use `<a role="button">` with inline `onclick` for forms that POST to minimal API endpoints:

```html
<form id="registerForm" method="post" action="/Account/DoRegister">
    <input type="text" name="email" placeholder="Email" />
    <input type="password" name="password" placeholder="Password" />
    <a role="button" class="btn btn-primary"
       onclick="document.getElementById('registerForm').submit()">
        Register
    </a>
</form>
```

**Why this works:** Blazor's enhanced navigation targets `<button>` elements specifically. Anchor elements with `onclick` are not stripped.

**When to use:** Auth forms (register, login, logout) that POST to minimal API endpoints for cookie handling. Any form that needs a traditional HTTP POST/redirect flow.

### Pattern 2: Blazor EditForm (for in-component handling)

Use Blazor's `EditForm` with `OnValidSubmit` for forms handled entirely within the Blazor component:

```razor
<EditForm Model="@_model" OnValidSubmit="HandleSubmit" FormName="checkout">
    <DataAnnotationsValidator />
    <InputText @bind-Value="_model.Name" />
    <ValidationSummary />
    <button type="submit" class="btn btn-primary">Submit</button>
</EditForm>

@code {
    private CheckoutModel _model = new();

    private async Task HandleSubmit()
    {
        // Handle form data in-component
        await SaveOrder(_model);
        NavigationManager.NavigateTo("/confirmation");
    }
}
```

**When to use:** Data editing forms, settings pages, any form where the result stays within the Blazor component tree.

---

## When to Use Which Pattern

| Scenario | Pattern | Why |
|----------|---------|-----|
| Auth forms (register, login) | Pattern 1 — anchor + minimal API POST | Needs HTTP cookie handling via ASP.NET Core middleware |
| Logout | Pattern 1 — anchor + minimal API POST | Must clear HTTP-only auth cookies server-side |
| Shopping cart update | Pattern 2 — EditForm | Data stays in component, no cookie handling needed |
| Profile/settings edit | Pattern 2 — EditForm | In-component data flow |
| Payment form with redirect | Pattern 1 — anchor + POST | External redirect after POST |

---

## Anti-Patterns

### ❌ Button with onclick for Form Submission

```html
@* WRONG — Blazor enhanced navigation strips onclick from buttons *@
<form id="loginForm" method="post" action="/Account/DoLogin">
    <button type="button" onclick="document.getElementById('loginForm').submit()">
        Log In
    </button>
</form>
```

### ❌ JavaScript Event Listener on Button

```html
@* WRONG — same problem, Blazor strips the handler during DOM enhancement *@
<button id="submitBtn">Submit</button>
<script>
    document.getElementById('submitBtn').addEventListener('click', function() {
        document.getElementById('myForm').submit();
    });
</script>
```

**Note:** The JavaScript listener approach may work inconsistently depending on when Blazor's enhancement runs vs. when the script executes. It's unreliable.

---

## Helper Script Pattern

Run 7 used a `form-submit.js` helper to centralize the submit logic:

```javascript
// wwwroot/js/form-submit.js
function submitForm(formId) {
    var form = document.getElementById(formId);
    if (form) form.submit();
}
```

```html
<a role="button" class="btn btn-primary" onclick="submitForm('registerForm')">
    Register
</a>
```

This keeps the `onclick` attribute clean and the logic reusable across auth forms.

---

## BWFC WebFormsForm Component (Preferred for Migration)

Since this skill was written, BWFC added a `<WebFormsForm>` component that captures form POST data via JS interop and exposes it through `Request.Form["key"]` on `WebFormsPageBase`. This is now the **preferred approach** for migrating Web Forms pages that use `Request.Form`:

```razor
<WebFormsForm OnSubmit="SetRequestFormData">
    <input type="text" name="email" />
    <button type="submit">Submit</button>
</WebFormsForm>

@code {
    protected override void OnFormSubmitted()
    {
        var email = Request.Form["email"];
        // Process form data using familiar Web Forms patterns
    }
}
```

**When to use WebFormsForm vs other patterns:**
- **WebFormsForm** — Migrating Web Forms pages that use `Request.Form[]` or `Page_Load` with form data. Minimal code changes.
- **Pattern 1 (Anchor + POST)** — Auth forms that need HTTP cookie handling via minimal API endpoints.
- **Pattern 2 (EditForm)** — New Blazor-native forms or forms being modernized beyond compile-compat.
