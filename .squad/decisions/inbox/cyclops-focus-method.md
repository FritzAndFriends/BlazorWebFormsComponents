### Focus() Method Added to BaseWebFormsComponent

**By:** Cyclops (Component Dev)

**What:** Added `public virtual void Focus()` to `BaseWebFormsComponent`, matching the ASP.NET Web Forms `Control.Focus()` signature. The method uses fire-and-forget JS interop (`_ = JsRuntime.InvokeVoidAsync(...)`) to call `bwfc.Page.Focus(clientId)` which does `document.getElementById(id).focus()`. Null-guards JsRuntime for SSR pre-render. Added the JS function to both `Basepage.js` and `Basepage.module.js`.

**Why this matters for the team:**
- Any component inheriting from `BaseWebFormsComponent` (or its subclasses) now has `Focus()` available — no per-component work needed.
- Migration scripts can translate `control.Focus()` calls directly since the method signature matches Web Forms.
- The existing `Validation.SetFocus` in validators is left untouched — it uses a different code path (field name, not ClientID).
- The method is `virtual` so components with special focus needs (e.g., composite controls) can override it.
