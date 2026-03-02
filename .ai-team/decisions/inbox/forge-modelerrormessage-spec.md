### 2026-03-02: ModelErrorMessage component specification
**By:** Forge
**What:** Component spec for ModelErrorMessage — the last missing BWFC control
**Why:** Jeff's directive: the migration promise ("remove asp: and it works") requires this control to exist. The WingtipToys migration analysis identified ModelErrorMessage as the sole control without a BWFC equivalent (28/29 had coverage). It appears in ManagePassword.aspx and RegisterExternalLogin.aspx.

---

## 1. Web Forms Reference

**Namespace:** `System.Web.ModelBinding`
**Class:** `System.Web.UI.WebControls.ModelErrorMessage`
**Inherits:** `Label` → `WebControl` → `Control`

ModelErrorMessage is a specialized Label that renders only when `Page.ModelState[ModelStateKey]` contains errors. It was introduced in ASP.NET 4.5 alongside Model Binding for Web Forms. It does **not** perform validation — it is a passive error *display* control.

### Observed usage in WingtipToys

```aspx
<%-- ManagePassword.aspx, line 23-24 --%>
<asp:ModelErrorMessage runat="server"
    ModelStateKey="NewPassword"
    AssociatedControlID="password"
    CssClass="text-danger"
    SetFocusOnError="true" />

<%-- RegisterExternalLogin.aspx, line 22 --%>
<asp:ModelErrorMessage runat="server"
    ModelStateKey="email"
    CssClass="text-error" />
```

### Web Forms rendered HTML

When `ModelState["NewPassword"]` has an error:
```html
<span class="text-danger">The password must be at least 6 characters long.</span>
```

When `ModelState["NewPassword"]` has no error:
```html
<!-- nothing rendered (Display="Dynamic" behavior, which is the default) -->
```

Key point: unlike validators, ModelErrorMessage defaults to rendering nothing when there is no error. It does not reserve space.

---

## 2. Component Design

### File location

```
src/BlazorWebFormsComponents/Validations/ModelErrorMessage.razor
src/BlazorWebFormsComponents/Validations/ModelErrorMessage.razor.cs
```

Rationale: although ModelErrorMessage is not a validator, it lives in `System.Web.UI.WebControls` alongside validators in Web Forms, and conceptually belongs with validation infrastructure. Placing it in `Validations/` keeps it discoverable alongside `RequiredFieldValidator`, `CompareValidator`, and `AspNetValidationSummary`.

### Base class

**`BaseStyledComponent`** — NOT `BaseValidator<T>`.

Justification:
- ModelErrorMessage inherits from `Label` in Web Forms, which inherits from `WebControl` (styled, not validating).
- It does **not** implement `IValidator`. It performs no validation logic.
- `BaseValidator<T>` requires an abstract `Validate(string)` method, a `ControlToValidate` parameter, and hooks into `EditContext.OnValidationRequested` — none of which apply.
- `BaseStyledComponent` provides exactly what's needed: `CssClass`, `Style`, `Visible`, `Enabled`, `ToolTip`, `ID`, font properties, and `AdditionalAttributes`.
- Precedent: `AspNetValidationSummary` also inherits `BaseStyledComponent` (not `BaseValidator<T>`) because it displays errors without performing validation.

### Parameters

| Parameter | Type | Default | Web Forms Equivalent | Notes |
|-----------|------|---------|---------------------|-------|
| `ModelStateKey` | `string` | **(required)** | `ModelStateKey` | The key to look up in the error source. Maps to a field identifier on `EditContext`. |
| `AssociatedControlID` | `string` | `null` | `AssociatedControlID` | The ID of the control this error relates to. Used by `SetFocusOnError` to know which element to focus. |
| `SetFocusOnError` | `bool` | `false` | `SetFocusOnError` | When `true` and an error exists, focuses the associated control via JS interop. |
| `CssClass` | `string` | `null` | `CssClass` | Inherited from `BaseStyledComponent`. |
| `Visible` | `bool` | `true` | `Visible` | Inherited from `BaseWebFormsComponent`. |
| `Enabled` | `bool` | `true` | `Enabled` | Inherited from `BaseWebFormsComponent`. |
| `ID` | `string` | `null` | `ID` | Inherited from `BaseWebFormsComponent`. |
| `ToolTip` | `string` | `null` | `ToolTip` | Inherited from `BaseStyledComponent`. |

Parameters inherited from `BaseStyledComponent` (BackColor, ForeColor, BorderColor, BorderStyle, BorderWidth, Height, Width, Font, Style) are available but unlikely to be used in practice.

**NOT included:**
- `Text` — In Web Forms, `ModelErrorMessage.Text` is the fallback display text when there's no error key match (inherited from Label). In practice it's never set in any sample. The component gets its display text from the error message in ModelState. Omitted to keep the component focused. Can be added later if a migration scenario requires it.
- `Display` (ValidatorDisplay enum) — Web Forms ModelErrorMessage doesn't have a `Display` property. It simply renders nothing when there's no error.
- `ValidationGroup` — ModelErrorMessage is not a validator and doesn't participate in validation groups. It reads from the full model state.

### Blazor-side error source: `EditContext` (Option A — recommended)

**Decision: Use `[CascadingParameter] EditContext` to read validation messages.**

Rationale:

1. **Best migration story.** In Web Forms, the code-behind calls `ModelState.AddModelError("NewPassword", "Too short")`. In Blazor, the code-behind calls `messageStore.Add(editContext.Field("NewPassword"), "Too short")`. Same key, same pattern, same mental model. The developer changes one line of C# and the markup "just works."

2. **Already how BWFC works.** Every existing validator (`RequiredFieldValidator`, `CompareValidator`, `RangeValidator`, etc.) and `AspNetValidationSummary` already use `[CascadingParameter] EditContext`. ModelErrorMessage plugging into the same system means it participates in `EditContext.OnValidationStateChanged` notifications and sees errors added by BWFC validators or by developer code.

3. **No new abstractions.** Options B (`ErrorMessage` string parameter) and C (`Dictionary<string, string> ModelState`) force the developer to wire up plumbing that Web Forms handled automatically. Option D (custom validation system) doesn't exist — BWFC validators already use Blazor's `EditContext` + `ValidationMessageStore`.

4. **Consistent with Blazor's `<ValidationMessage>`.** Blazor ships `<ValidationMessage For="@(() => model.Email)" />` which also reads from `EditContext`. Our component does the same thing but keyed by string name instead of lambda expression — matching the Web Forms migration pattern where developers use string IDs, not expressions.

**How it works:**

```csharp
[CascadingParameter] EditContext CurrentEditContext { get; set; }
```

On `EditContext.OnValidationStateChanged`, the component calls:
```csharp
var field = CurrentEditContext.Field(ModelStateKey);
var messages = CurrentEditContext.GetValidationMessages(field);
```

If messages exist, render the `<span>`. If not, render nothing.

**Migration pattern for developers:**

Web Forms code-behind:
```csharp
ModelState.AddModelError("NewPassword", "Password too short.");
```

Blazor code-behind (equivalent):
```csharp
@inject IJSRuntime JS
// In the component or page:
private ValidationMessageStore _messageStore;
protected override void OnInitialized()
{
    _messageStore = new ValidationMessageStore(editContext);
}
private void SetPassword_Click()
{
    _messageStore.Clear();
    if (password.Length < 6)
    {
        _messageStore.Add(editContext.Field("NewPassword"), "Password too short.");
    }
    editContext.NotifyValidationStateChanged();
}
```

The markup migration is:
```diff
- <asp:ModelErrorMessage runat="server" ModelStateKey="NewPassword"
-     AssociatedControlID="password" CssClass="text-danger" SetFocusOnError="true" />
+ <ModelErrorMessage ModelStateKey="NewPassword"
+     AssociatedControlID="password" CssClass="text-danger" SetFocusOnError="true" />
```

Zero markup changes beyond removing `asp:` and `runat="server"`.

---

## 3. Rendered HTML

### When error exists for ModelStateKey

```html
<span class="text-danger">The password must be at least 6 characters long.</span>
```

With ID set (`ID="pwdError"`):
```html
<span id="pwdError" class="text-danger">The password must be at least 6 characters long.</span>
```

With multiple errors for the same key, concatenate with `<br>` (Web Forms renders each error on a separate line within the span):
```html
<span class="text-danger">Password too short.<br>Password must contain a number.</span>
```

### When no error exists

Nothing is rendered (the component returns `null` / empty fragment). This matches Web Forms behavior where ModelErrorMessage does not reserve DOM space.

### With style properties set

```html
<span class="text-danger" style="color:Red;" title="Error tooltip">The password must be at least 6 characters long.</span>
```

---

## 4. Component Implementation Sketch

### ModelErrorMessage.razor

```razor
@inherits BaseStyledComponent

@if (Enabled && Visible && _hasErrors)
{
    <span class="@CssClass" style="@Style" title="@ToolTip" @attributes="AdditionalAttributes">
        @((MarkupString)_errorHtml)
    </span>
}
```

### ModelErrorMessage.razor.cs

```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Validations
{
    public partial class ModelErrorMessage : BaseStyledComponent, IDisposable
    {
        private EditContext _previousEditContext;
        private bool _hasErrors;
        private string _errorHtml = string.Empty;

        [CascadingParameter] EditContext CurrentEditContext { get; set; }

        /// <summary>
        /// The key in the model state / EditContext to display errors for.
        /// Maps to a field name on the EditContext model.
        /// </summary>
        [Parameter] public string ModelStateKey { get; set; }

        /// <summary>
        /// The ID of the associated input control.
        /// Used with SetFocusOnError to focus the control when an error is displayed.
        /// </summary>
        [Parameter] public string AssociatedControlID { get; set; }

        /// <summary>
        /// When true, focuses the associated control when an error is displayed.
        /// </summary>
        [Parameter] public bool SetFocusOnError { get; set; }

        protected override void OnParametersSet()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ModelErrorMessage)} requires a cascading parameter of type " +
                    $"{nameof(EditContext)}. Use {nameof(ModelErrorMessage)} inside an EditForm.");
            }

            if (CurrentEditContext != _previousEditContext)
            {
                DetachListener();
                CurrentEditContext.OnValidationStateChanged += OnValidationStateChanged;
                _previousEditContext = CurrentEditContext;
            }

            UpdateErrorState();
            base.OnParametersSet();
        }

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            UpdateErrorState();
            StateHasChanged();
        }

        private void UpdateErrorState()
        {
            if (string.IsNullOrEmpty(ModelStateKey) || CurrentEditContext == null)
            {
                _hasErrors = false;
                _errorHtml = string.Empty;
                return;
            }

            var field = CurrentEditContext.Field(ModelStateKey);
            var messages = CurrentEditContext.GetValidationMessages(field)
                .Select(StripValidatorMetadata)
                .Where(m => !string.IsNullOrEmpty(m))
                .ToList();

            _hasErrors = messages.Count > 0;
            _errorHtml = _hasErrors
                ? string.Join("<br>", messages.Select(System.Net.WebUtility.HtmlEncode))
                : string.Empty;

            if (_hasErrors && SetFocusOnError && !string.IsNullOrEmpty(AssociatedControlID))
            {
                _ = JsRuntime.InvokeVoidAsync(
                    "bwfc.Validation.SetFocus", AssociatedControlID);
            }
        }

        /// <summary>
        /// BWFC validators encode messages as "Text,ErrorMessage\x1FValidationGroup".
        /// ModelErrorMessage should display the ErrorMessage portion, not the raw encoded string.
        /// </summary>
        private static string StripValidatorMetadata(string raw)
        {
            // Strip validation group suffix
            var groupSep = raw.IndexOf('\x1F');
            var fieldMsg = groupSep >= 0 ? raw.Substring(0, groupSep) : raw;

            // If it contains the Text,ErrorMessage pattern, take the ErrorMessage part
            var commaIndex = fieldMsg.IndexOf(',');
            if (commaIndex >= 0)
            {
                return fieldMsg.Substring(commaIndex + 1);
            }

            return fieldMsg;
        }

        public void Dispose()
        {
            DetachListener();
        }

        private void DetachListener()
        {
            if (_previousEditContext != null)
            {
                _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
            }
        }
    }
}
```

---

## 5. Edge Cases

| Scenario | Behavior |
|----------|----------|
| `ModelStateKey` is null or empty | Render nothing. No error. Silently no-op. |
| No `EditContext` cascading parameter | Throw `InvalidOperationException` with guidance (same pattern as `AspNetValidationSummary`). |
| No errors for the given key | Render nothing (empty fragment). |
| Multiple errors for the same key | Render all messages joined with `<br>` inside a single `<span>`. |
| `SetFocusOnError="true"` but `AssociatedControlID` not set | No focus attempt. `SetFocusOnError` is ignored when there's no target. No error thrown. |
| `SetFocusOnError="true"` and error exists | JS interop calls `bwfc.Validation.SetFocus` with `AssociatedControlID` (reuses existing BWFC JS). |
| `Visible="false"` | Render nothing (handled by `BaseWebFormsComponent`). |
| `Enabled="false"` | Render nothing (consistent with validator pattern). |
| Error messages contain HTML | Messages are HTML-encoded before rendering (security). The `<br>` separator between multiple messages is injected by us, not user content. |
| BWFC validator metadata in messages | Stripped by `StripValidatorMetadata()`. Messages from BWFC validators use `\x1F` separator and `Text,ErrorMessage` encoding — ModelErrorMessage parses this to extract the display-friendly error message. |
| Messages from `ValidationMessageStore.Add()` directly | Passed through as-is (no `\x1F` or comma encoding). |
| EditContext changes (e.g., re-render with new model) | Detaches from old EditContext, attaches to new one (same pattern as `AspNetValidationSummary`). |

---

## 6. Comparison with Blazor's Built-in `<ValidationMessage>`

| Feature | Blazor `<ValidationMessage>` | BWFC `<ModelErrorMessage>` |
|---------|-------|------|
| Field reference | Lambda: `For="@(() => model.Password)"` | String: `ModelStateKey="NewPassword"` |
| HTML output | `<div class="validation-message">` | `<span class="text-danger">` |
| Base class | `ComponentBase` | `BaseStyledComponent` (CssClass, Style, fonts) |
| Focus on error | Not supported | `SetFocusOnError` + `AssociatedControlID` |
| Multiple errors | Each on separate line in `<div>` | `<br>`-separated in single `<span>` |
| Migration story | Requires rewriting to lambda expressions | Drop `asp:` and `runat="server"`, done |

The key difference: `<ValidationMessage>` requires a strongly-typed lambda expression (`For`), which forces developers to refactor their markup. `<ModelErrorMessage>` keeps the string-keyed pattern from Web Forms, enabling zero-markup-change migration.

---

## 7. Testing Plan

### bUnit tests (Rogue)
1. **No error state** — renders nothing when EditContext has no messages for key.
2. **Single error** — renders `<span>` with CssClass and message text.
3. **Multiple errors** — renders all messages with `<br>` separator.
4. **CssClass rendering** — verifies `class` attribute on span.
5. **Style properties** — verifies inline style from BaseStyledComponent.
6. **SetFocusOnError** — verifies JS interop call when error exists and AssociatedControlID is set.
7. **SetFocusOnError without AssociatedControlID** — no JS interop call.
8. **Missing EditContext** — throws InvalidOperationException.
9. **Null/empty ModelStateKey** — renders nothing.
10. **BWFC validator metadata stripping** — messages with `\x1F` encoding display correctly.
11. **Visible="false"** — renders nothing.
12. **EditContext changes** — properly detaches/reattaches listener.

### Integration test (Colossus)
- Sample page with `EditForm` + `TextBox` + `ModelErrorMessage`, triggers error via button click, verifies span appears/disappears.

### Sample page (Jubilee)
- `Components/Pages/ControlSamples/ModelErrorMessage/Default.razor` — demonstrates password validation with model error display.

### Documentation (Beast)
- `docs/Validations/ModelErrorMessage.md` — Web Forms syntax, Blazor syntax, migration before/after, HTML output.

---

## 8. Open Questions (for Jeff)

1. **`Text` property**: Web Forms `ModelErrorMessage` inherits `Label.Text` as a fallback display value. No WingtipToys sample uses it. Should we include it for completeness, or defer until a migration scenario needs it? **Recommendation:** defer.

2. **`Display` property (Static/Dynamic/None)**: Web Forms ModelErrorMessage does not have this property (it's on validators only), but some developers might expect it since it's adjacent to validators in markup. **Recommendation:** omit — keep to the original control's API surface.

3. **Namespace placement**: The component lives in `Validations/` alongside validators. Should we add a `@using BlazorWebFormsComponents.Validations` to `_Imports.razor` if it's not already there? **Recommendation:** verify — if validators are already importable by tag name, this will be too.
