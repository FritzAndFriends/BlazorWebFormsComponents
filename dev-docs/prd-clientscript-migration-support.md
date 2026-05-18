# PRD: ClientScript Migration Support for BWFC

**Author:** Beast (Technical Writer)  
**Date:** 2026-07-30  
**Status:** Ready for Implementation  
**Requested by:** Jeffrey T. Fritz  
**Related Issues:** [Forge CLI Gap Analysis §1.2](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues)

---

## 1. Purpose & Goals

### 1.1 What Is This?

This PRD defines how BlazorWebFormsComponents should support migration of ASP.NET Web Forms `Page.ClientScript` and `ScriptManager` JavaScript registration patterns to Blazor equivalents.

`ClientScript` (also called `ClientScriptManager`) is a fundamental Web Forms API that enables server-side code to:
- Register inline scripts that run on page load
- Include external script files
- Generate postback event handlers (dynamic `__doPostBack` calls)
- Manage script versions and includes

In Blazor, the equivalent is `IJSRuntime` for interop and component lifecycle events (`OnAfterRenderAsync`, `OnInitializedAsync`) for initialization hooks.

### 1.2 Who Uses It?

| Audience | What They Need |
|----------|---------------|
| **Migrating developers** | Clear patterns for converting `RegisterClientScriptBlock`, `RegisterStartupScript`, and script includes to Blazor equivalents |
| **Forge (CLI Lead)** | Diagnostic rules and safe CLI transforms to detect and handle common ClientScript patterns |
| **Cyclops (Component Lead)** | Runtime shim support for simple script startup cases; clarity on what NOT to automate |
| **Beast (Technical Writer)** | Documentation and migration guides showing before/after ClientScript→IJSRuntime patterns |

### 1.3 What Decisions Does It Inform?

- What ClientScript patterns are safe to detect and auto-transform in the CLI?
- What patterns require manual developer guidance via TODO comments?
- Should BWFC provide a `ClientScriptManager` compatibility shim, or delegate entirely to `IJSRuntime`?
- Which patterns (if any) can use the existing `UpdatePanel` + `ScriptManager` components as fallback?

### 1.4 Why This Matters

**Impact: HIGH**

From Forge's 2026-07-25 CLI Gap Analysis (§1.2):
> *ClientScript / RegisterClientScriptBlock — NO TRANSFORM. Pattern: `ClientScriptManager.RegisterClientScriptBlock()`, `Page.ClientScript`, `ScriptManager.RegisterStartupScript()`. Current: Code compiles but fails at runtime (no ClientScript property). Impact: HIGH — any app with JavaScript integration.*

Approximately **80% of real Web Forms applications** use `ClientScript` for at least one of:
- Startup scripts (jQuery initialization, third-party lib setup)
- Form validation scripts
- Client-side event handlers
- Dynamic script includes

Without migration support, developers face:
- ❌ Compile errors or runtime failures
- ❌ Manual rewrite of JavaScript initialization logic
- ❌ Potential UI breakage if client-side scripts are critical

---

## 2. Problem Statement & Current Gaps

### 2.1 The Problem: Page.ClientScript Does Not Exist in Blazor

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // Register startup script
        Page.ClientScript.RegisterStartupScript(
            type: this.GetType(),
            key: "InitializeUI",
            script: "$(function() { console.log('Page loaded'); });",
            addScriptTags: true);
        
        // Register external script
        Page.ClientScript.RegisterClientScriptInclude(
            key: "myCustom",
            url: "~/Scripts/custom.js");
    }
}
```

**Current Blazor Attempt (Broken):**
```csharp
// ❌ No Page.ClientScript property — compile error
// ❌ Developer is stuck

protected override async Task OnInitializedAsync()
{
    // What now? How do I run JavaScript on page load?
}
```

### 2.2 Key Constraints in Blazor Lifecycle

Understanding *why* some ClientScript patterns are difficult in Blazor:

| Constraint | Impact | Example |
|-----------|--------|---------|
| **No `IsPostBack` concept** | Can't distinguish first load from re-renders | Every component field assignment runs on every render cycle — must use `firstRender` guard in `OnAfterRenderAsync` |
| **No automatic DOM lifecycle** | Scripts must explicitly wait for elements to exist | jQuery's `$(function())` equivalent is `OnAfterRenderAsync(firstRender=true)` |
| **Renderer owns the DOM** | Can't inject arbitrary `<script>` tags; must use `IJSRuntime` | Static scripts in `index.html` are OK; dynamic ones must flow through JS interop |
| **Re-render behavior** | Code paths can execute multiple times per render unless guarded; dedupe is the caller's responsibility | If code runs twice in one render cycle, guard with `if (firstRender)` or track state. SignalR does not automatically dedupe JS interop calls. |
| **Prerendering incompatibility** | Server-side prerendering can't invoke `IJSRuntime` | `OnAfterRenderAsync` must check `if (firstRender)` to do DOM-touching JS only after interactivity is present |
| **Form validation timing** | Web Forms postback validation happens server-side first; Blazor uses `EditContext` | Script-based validation patterns don't translate directly |
| **__doPostBack emulation** | AJAX postback event generation is framework-specific | Blazor has no `__doPostBack()` equivalent — events are component method calls |

### 2.3 Existing BWFC Artifacts (Incomplete)

**`PageClientScriptUsageAnalyzer.cs` (BWFC022)**  
- ✅ Detects `Page.ClientScript` usage
- ✅ Reports warning with message about `IJSRuntime`
- ❌ Provides no automated transform or TODO guidance for the CLI
- ❌ No documentation of the migration path

**`IPostBackEventHandlerUsageAnalyzer.cs` (BWFC023)**  
- ✅ Detects `IPostBackEventHandler` implementation
- ✅ Reports warning about `EventCallback<T>`
- ❌ No guidance on how postback handlers should be rewritten

**`ScriptManager.razor.cs`**  
- ✅ Exists as a markup-only stub for compatibility
- ❌ No runtime integration with script registration
- ❌ Parameters like `EnablePartialRendering` are accepted but ignored

**`UpdatePanel.razor.cs`**  
- ✅ Renders as a `<div>` wrapper
- ❌ `UpdateMode`, `Triggers`, `AsyncPostBackTrigger` have no effect
- ⚠️ Developers may expect AJAX partial page updates (not available in Blazor without architectural change)

### 2.4 Gap Categories

#### 2.4.1 Startup Scripts (Automatable)
**Pattern:** `RegisterStartupScript()` with simple inline JavaScript  
**Complication:** Must run after component renders  
**Opportunity:** CLI can detect and generate TODO with `OnAfterRenderAsync(IJSRuntime)` skeleton

#### 2.4.2 Script Includes (Partially Automatable)
**Pattern:** `RegisterClientScriptInclude()`, external `.js` file references  
**Complication:** Path resolution (Web Forms `~/` vs. Blazor root paths)  
**Opportunity:** CLI can detect includes and suggest `<script src="">` in HTML layout or as `IJSRuntime.InvokeAsync()` calls

#### 2.4.3 Postback Validation Scripts (Not Automatable)
**Pattern:** `__doPostBack()`, form validation callbacks  
**Complication:** Fundamentally tied to Web Forms postback cycle  
**Recommendation:** EMIT TODO + DOCUMENTATION that explains Blazor `EditContext` approach

#### 2.4.4 Custom Event Binding (Not Automatable)
**Pattern:** Dynamic event handler registration, `GetPostBackEventReference()`  
**Complication:** Web Forms postback is HTTP POST; Blazor is component method call  
**Recommendation:** EMIT TODO pointing to `@onclick` or `EventCallback` patterns

#### 2.4.5 ScriptManager Code-Behind Usage (Partially Automatable)
**Pattern:** `ScriptManager.GetCurrent()`, `ScriptManager.RegisterAsyncPostBackControl()`, `ScriptManager.SetFocus()`  
**Complication:** UpdatePanel async postback concept doesn't exist in Blazor  
**Recommendation:** Detect and emit TODO

---

## 3. Guiding Principles & Guardrails

### 3.1 BWFC Migration Philosophy

**Principle 1: Preserve the minimum necessary markup compatibility.**

We recreate Web Forms controls with the same names and attributes so developers can migrate markup with minimal changes. For JavaScript, we follow the same principle: provide enough tooling and guidance that developers can understand the migration, but **do not attempt to emulate Web Forms postback semantics**.

**Principle 2: Prefer runtime helpers for simple, deterministic cases.**

If a pattern is simple, occurs frequently, and can be safely handled at runtime, provide a shim or helper. Examples:
- ✅ Inline startup scripts (simple enough to detect and wrap)
- ✅ Static script includes (safe to reference from HTML)
- ❌ `__doPostBack()` emulation (fundamentally incompatible)

**Principle 3: Emit explicit TODO guidance for ambiguous patterns.**

If a pattern could have multiple valid interpretations in Blazor, don't guess. Emit a clear TODO comment that:
1. Names the pattern
2. Explains why it's ambiguous
3. Provides references to migration documentation
4. Leaves implementation to the developer

### 3.2 What We Will NOT Do (But What We MAY Do)

**The shim strategy is NOT a full `ClientScriptManager` compatibility wrapper:**

❌ **Do not attempt to emulate `__doPostBack()`** — this is Web Forms' event validation mechanism. Blazor has no equivalent.

❌ **Do not try to replicate UpdatePanel AJAX postback semantics** — the concept doesn't exist in Blazor. UpdatePanel in BWFC is a structural wrapper only.

❌ **Do not auto-transform complex validation scripts** — form validation in Web Forms is postback-driven; Blazor uses `EditContext`. These are fundamentally different patterns.

❌ **Do not promise that `ScriptManager` code-behind calls will "just work"** — `RegisterAsyncPostBackControl()`, `SetFocus()`, and other postback-specific methods have no Blazor equivalent.

❌ **Do not generate wrapper shims for every Web Forms `ClientScript` method** — maintainability risk, and most use cases are better served by direct `IJSRuntime` usage.

**However, a minimal internal helper/registry IS acceptable:**

✅ **A small runtime helper CAN be provided for common deterministic cases:**
- Queued startup scripts that need to run once on first interactivity
- Key-based deduplication of scripts (to prevent double-execution)
- First-render flushing of queued script operations

This is **not** a compatibility wrapper, but rather a thin **helper** for simple, safe patterns that occur frequently (e.g., inline startup script initialization). The helper would be **internal and optional** — developers can always use `IJSRuntime` directly if they prefer.

### 3.3 Safe Automation Boundary

**Automatable (Deterministic):**
- Detect `Page.ClientScript.RegisterStartupScript()` with simple inline scripts
- Suggest wrapping in `OnAfterRenderAsync(IJSRuntime jsRuntime)` with `if (firstRender)` guard
- Detect `RegisterClientScriptInclude()` and suggest static `<script>` tag or `IJSRuntime.InvokeAsync("import", ...)`

**Requires Guidance (TODO):**
- Any postback validation or event binding
- Dynamic script generation (depends on what data is available at Blazor compile time)
- ScriptManager code-behind patterns
- IPostBackEventHandler interface implementations

**Helper/Registry (Phase 3, Optional):**
- A minimal internal helper can cache and dedupe queued startup scripts
- Helper must be deterministic and safe: simple string scripts, key-based lookup, first-render flushing
- This helper is **not** a compatibility layer, just a convenience for the common case

---

## 4. In Scope vs Out of Scope

### 4.1 IN SCOPE: Deliverables for This PRD

| Item | Owner | Phase |
|------|-------|-------|
| Update `PageClientScriptUsageAnalyzer` (BWFC022) with TODO guidance template | Cyclops | P1 |
| Add `ClientScriptTransform.cs` to CLI for simple pattern detection | Forge | P1 |
| Create `ClientScriptMigrationGuide.md` documentation | Beast | P1 |
| Add test cases: TC36 (RegisterStartupScript), TC37 (RegisterInclude) | Cyclops (or Forge) | P1 |
| Scaffold `OnAfterRenderAsync(IJSRuntime)` pattern for startup scripts | Forge (CLI) | P1 |
| Document `IPostBackEventHandler` migration path | Beast | P2 |
| Add sample page showing ClientScript→IJSRuntime conversion | Jubilee | P2 |

### 4.2 OUT OF SCOPE: Non-Goals

| Item | Why Not | Alternative |
|------|---------|-------------|
| Full `ClientScriptManager` shim/wrapper class | Maintainability burden; `IJSRuntime` is the right API | Use `IJSRuntime` directly |
| Emulate `__doPostBack()` | Fundamentally incompatible with Blazor component model | Rewrite as component method calls or `onclick` handlers |
| UpdatePanel async postback support | Would require SignalR protocol changes; defeats Blazor's design | Use Blazor's native `@bind` and `EventCallback` |
| Postback validation script conversion | Complex, app-specific; depends on form structure | Migrate to `EditContext`-based validation |
| ScriptManager full API surface | Only a few methods are commonly used; others are framework internals | Detect usage and emit TODO |

---

## 5. User Scenarios & Real-World Patterns

### 5.1 Scenario 1: Simple Startup Script (High Volume)

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            "InitializeTheme",
            "$(document).ready(function() { applyTheme('dark'); });",
            addScriptTags: true);
    }
}
```

**Blazor (Desired):**
```csharp
@inject IJSRuntime JS

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("eval", "applyTheme('dark');");
            // or better: await JS.InvokeVoidAsync("initializeTheme");
        }
    }
}
```

**Automation Opportunity:** CLI can detect `RegisterStartupScript` with inline script and generate skeleton with TODO to refactor script into a proper JavaScript module.

---

### 5.2 Scenario 2: External Script Include (Medium Volume)

**Web Forms:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    Page.ClientScript.RegisterClientScriptInclude(
        "jquery-ui",
        ResolveUrl("~/lib/jquery-ui.min.js"));
}
```

**Blazor (Desired):**
```html
<!-- In layout or index.html -->
<script src="_framework/lib/jquery-ui.min.js"></script>
```

**Automation Opportunity:** CLI can detect `RegisterClientScriptInclude`, extract the script path, and suggest adding a static `<script>` tag to the layout.

---

### 5.3 Scenario 3: Postback Validation (Complex, Not Automatable)

**Web Forms:**
```csharp
public string GetPostBackValidation()
{
    return Page.ClientScript.GetPostBackEventReference(
        new PostBackOptions(btnSubmit, "validate") 
        { 
            PerformValidation = true 
        });
}
```

**Blazor (Requires Rewrite):**
```razor
<Button Text="Submit" OnClick="HandleValidate" />

@code {
    private EditContext editContext;
    
    private async Task HandleValidate()
    {
        if (editContext.Validate())
        {
            // Proceed
        }
    }
}
```

**Automation:** CLI should detect and emit TODO pointing to documentation.

---

### 5.4 Scenario 4: IPostBackEventHandler Implementation (Moderate Volume)

**Web Forms:**
```csharp
public partial class MyControl : UserControl, IPostBackEventHandler
{
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "delete")
            OnDeleteRequested?.Invoke(this, EventArgs.Empty);
    }
    
    public event EventHandler OnDeleteRequested;
}
```

**Blazor (Requires Rewrite):**
```csharp
public partial class MyControl : BaseWebFormsComponent
{
    [Parameter]
    public EventCallback OnDeleteRequested { get; set; }
    
    private async Task HandleDelete()
    {
        await OnDeleteRequested.InvokeAsync();
    }
}
```

**Automation:** Detect interface implementation and emit TODO with `EventCallback` guidance.

---

### 5.5 Scenario 5: ScriptManager Code-Behind (Low-Medium Volume)

**Web Forms:**
```csharp
ScriptManager sm = ScriptManager.GetCurrent(Page);
sm.RegisterAsyncPostBackControl(MyGridView);
sm.SetFocus(txtSearch);
```

**Blazor (Requires Rewrite):**
- `RegisterAsyncPostBackControl()` → Remove (no UpdatePanel async in Blazor)
- `SetFocus()` → Use `@ref` + `JS.InvokeVoidAsync("focus")` in `OnAfterRenderAsync`

**Automation:** Detect and emit TODO with pattern guidance.

---

## 6. Product Requirements

### 6.1 Analyzer / Diagnostic Improvements

#### 6.1.1 BWFC022 Enhancement: ClientScript Usage Guidance

**Requirement:** Expand `PageClientScriptUsageAnalyzer` to provide nuanced TODO guidance.

**Current behavior:**
```
Warning BWFC022: Page.ClientScript is not available in Blazor. Use IJSRuntime for JavaScript interop.
```

**Desired behavior:**
```
Warning BWFC022: Page.ClientScript is not available in Blazor. Use IJSRuntime for JavaScript interop.

Migration path depends on the pattern:
- If RegisterStartupScript(): Wrap in OnAfterRenderAsync(IJSRuntime) with firstRender guard.
- If RegisterClientScriptInclude(): Add <script> tag to layout or invoke via JS.InvokeAsync("import", ...).
- If GetPostBackEventReference() or form validation: Migrate to EditContext validation patterns.

See: docs/Migration/ClientScriptMigrationGuide.md
```

**Implementation detail:** Analyzer can emit different messages based on method call name (optional enhancement; see Acceptance Criteria).

---

#### 6.1.2 New Analyzer: ScriptManager Code-Behind Usage (BWFC024)

**Requirement:** Detect and warn on problematic `ScriptManager` code-behind patterns.

**Patterns to detect:**
- `ScriptManager.GetCurrent(Page)` / `ScriptManager.GetCurrent(this)`
- `.RegisterAsyncPostBackControl(...)`
- `.SetFocus(...)`
- `.RegisterUpdateProgress(...)`

**Warning message:**
```
Warning BWFC024: ScriptManager.GetCurrent() and related methods are not available in Blazor. 

SetFocus: Use JavaScript interop: await JS.InvokeVoidAsync("focus", @ref element).
RegisterAsyncPostBackControl: Not applicable — Blazor does not use UpdatePanel postback model.
RegisterUpdateProgress: Not applicable — use component state instead.

See: docs/Migration/ClientScriptMigrationGuide.md (ScriptManager section)
```

**Diagnostic ID:** BWFC024  
**Severity:** Warning  
**Category:** Migration

---

#### 6.1.3 Enhanced IPostBackEventHandler Analyzer (BWFC023)

**Requirement:** Improve `IPostBackEventHandlerUsageAnalyzer` with transformation guidance.

**Current:** Warns about interface implementation.  
**Enhanced:** Also suggest EventCallback pattern.

**Updated message:**
```
Warning BWFC023: IPostBackEventHandler is not available in Blazor. Use EventCallback<T> for event handling.

Instead of implementing IPostBackEventHandler and RaisePostBackEvent(), define EventCallback<T> parameters:

[Parameter]
public EventCallback<string> OnPostBackEvent { get; set; }

private async Task RaiseEvent(string eventArg)
{
    await OnPostBackEvent.InvokeAsync(eventArg);
}

See: docs/Migration/ClientScriptMigrationGuide.md (Postback Events section)
```

---

### 6.2 Runtime Shim / Helper Support

#### 6.2.1 Recommendation: NO Full Compatibility Shim, BUT Minimal Internal Helper IS Acceptable

**Decision:** Do NOT create a full `ClientScriptManager` compatibility shim.

**Rationale:**
- Every `ClientScript` method has a different Blazor equivalent (or no equivalent)
- A wrapper would create a false sense of compatibility
- Developers need to understand the underlying pattern (IJSRuntime, EditContext, EventCallback)
- Wrapper adds maintenance burden with minimal adoption benefit

**However, a minimal internal helper IS acceptable for Phase 3:**
- A small registry/helper for queued startup scripts (deterministic, safe)
- Deduplication by key for common cases
- First-render flushing to ensure scripts run at the right lifecycle point
- **This is NOT a wrapper**, just a convenience for simple patterns

**Alternative for Phase 1:** Provide clear documentation and TODO guidance; let developers use `IJSRuntime` directly.

#### 6.2.2 Support: ScriptManager as Structural Stub (Already Exists)

**Status:** ✅ Already provided  
**Behavior:** Renders as empty component; accepts parameters for compatibility  
**Limitation:** Parameters have no effect (by design)

**Documentation needed:** Add note to `ScriptManager.md` that code-behind calls have no effect in Blazor.

---

### 6.3 Safe CLI Transform Opportunities

#### 6.3.1 ClientScriptTransform: Simple Startup Script Detection

**Goal:** Detect `RegisterStartupScript` calls and generate TODO skeleton.

**Transform Logic:**

```csharp
// Detect: Page.ClientScript.RegisterStartupScript(...)
// Generate:
//
// protected override async Task OnAfterRenderAsync(bool firstRender)
// {
//     if (firstRender)
//     {
//         // TODO(bwfc-clientscript): Convert to IJSRuntime.InvokeVoidAsync()
//         await JS.InvokeVoidAsync("eval", @"YOUR_SCRIPT_HERE");
//         // Better: Define function in JavaScript file and call:
//         // await JS.InvokeVoidAsync("initializeUI");
//     }
// }
```

**Criteria for automation:**
- ✅ Detectable pattern: `RegisterStartupScript` with literal string or simple interpolation
- ✅ Safe injection point: `OnAfterRenderAsync` with `firstRender` guard
- ❌ Do NOT attempt: Script with complex logic, conditionals, or form-dependent behavior

**Test case:** TC36-ClientScript-RegisterStartupScript.aspx

**Phase:** P1

---

#### 6.3.2 ClientScriptIncludeTransform: Script File References

**Goal:** Detect `RegisterClientScriptInclude` and suggest static script tags or JS import.

**Transform Logic:**

```csharp
// Detect: Page.ClientScript.RegisterClientScriptInclude("key", url)
// OR:     ClientScript.RegisterClientScriptInclude(typeof(...), key, url)
//
// Generate TODO suggesting:
// 1. Add to layout HTML: <script src="~/lib/jquery-ui.min.js"></script>
// 2. Or in component: await JS.InvokeAsync("import", "./lib/jquery-ui.min.js");
//
// TODO(bwfc-clientscript-include): Add <script src="lib/jquery-ui.min.js"></script> to App layout
```

**Criteria:**
- ✅ Detectable URL pattern: literal string or simple ResolveUrl()
- ❌ Do NOT attempt: Dynamic URLs, conditional includes

**Test case:** TC37-ClientScript-RegisterInclude.aspx

**Phase:** P1

---

#### 6.3.3 PostBackEventTransform: Detect (Not Transform)

**Goal:** Flag `Page.ClientScript.GetPostBackEventReference()` calls.

**Transform Logic:**

```csharp
// Detect: Page.ClientScript.GetPostBackEventReference(...)
// Generate TODO:
//
// TODO(bwfc-clientscript-postback): GetPostBackEventReference() is not available in Blazor.
// This pattern registers a dynamic postback event handler. In Blazor, use EventCallback<T> instead.
// See: docs/Migration/ClientScriptMigrationGuide.md
```

**Criteria:**
- ✅ Detectable pattern: simple method call
- ❌ Do NOT attempt: Transformation (pattern is inherently postback-based)

**Test case:** TC38-ClientScript-PostBackReference.aspx

**Phase:** P1

---

### 6.4 TODO / Manual Rewrite Guidance

#### 6.4.1 Postback Validation Patterns

**Patterns:**
- `Page.Validate(validationGroup)`
- `Page.IsValid` checks
- `ClientScript` + form validation

**TODO Template:**
```csharp
// TODO(bwfc-clientscript-validation): Web Forms validation is postback-driven.
// Blazor uses EditContext-based validation instead.
//
// Web Forms:
//   if (!Page.IsValid) { ... }
//
// Blazor equivalent:
//   <EditForm Model="@model" OnValidSubmit="@HandleSubmit">
//       <DataAnnotationsValidator />
//       <!-- inputs with @bind-Value -->
//   </EditForm>
//
// See: docs/Migration/ClientScriptMigrationGuide.md (Validation section)
```

#### 6.4.2 IPostBackEventHandler Implementation

**TODO Template:**
```csharp
// TODO(bwfc-clientscript-ipostback): IPostBackEventHandler is not available in Blazor.
// Replace with EventCallback<T>:
//
//   [Parameter]
//   public EventCallback<string> OnPostBackEvent { get; set; }
//
//   private async Task RaiseEvent(string argument)
//   {
//       await OnPostBackEvent.InvokeAsync(argument);
//   }
//
// See: docs/Migration/ClientScriptMigrationGuide.md
```

#### 6.4.3 ScriptManager Code-Behind Patterns

**TODO Template (SetFocus):**
```csharp
// TODO(bwfc-clientscript-setfocus): ScriptManager.SetFocus() is not available in Blazor.
// Use JavaScript interop to focus an element:
//
//   @ref element="@inputRef"
//   protected ElementReference inputRef;
//   
//   await JS.InvokeVoidAsync("focus", inputRef);
```

**TODO Template (RegisterAsyncPostBackControl):**
```csharp
// TODO(bwfc-clientscript-asyncpostback): RegisterAsyncPostBackControl() is not available in Blazor.
// Blazor does not use UpdatePanel async postback model.
// Remove this call. If you need partial updates, use component parameter binding instead.
```

---

### 6.5 Documentation & Testing Requirements

#### 6.5.1 New Documentation: ClientScriptMigrationGuide.md

**Location:** `docs/Migration/ClientScriptMigrationGuide.md`

**Sections:**
1. **Overview** — Why ClientScript patterns differ in Blazor
2. **StartupScript Migration** — Simple startup scripts with `OnAfterRenderAsync` example
3. **Script Includes** — Static `<script>` tags vs. JS dynamic import
4. **Form Validation** — Web Forms postback validation → Blazor `EditContext`
5. **Event Handlers** — IPostBackEventHandler → EventCallback<T>
6. **ScriptManager Code-Behind** — SetFocus, RegisterAsyncPostBackControl patterns
7. **Dynamic Script Generation** — When and how to use `IJSRuntime.InvokeAsync()`
8. **Common Pitfalls** — prerendering, re-render deduplication, DOM timing

**Format:** Tabbed (Web Forms / Blazor) as per BWFC documentation standard

**Audience:** Developers with Web Forms experience migrating to Blazor

**Estimated length:** 1,200–1,500 lines with examples

**Phase:** P1

---

#### 6.5.2 Updated Documentation: PageClientScriptUsageAnalyzer Reference

**File:** `docs/Analyzers/BWFC022-PageClientScriptUsage.md`

**Content:**
- What the analyzer detects
- Why it matters
- Migration patterns for different `ClientScript` methods
- Link to `ClientScriptMigrationGuide.md`

**Phase:** P1

---

#### 6.5.3 New Analyzer Documentation: BWFC024-ScriptManagerCodeBehind

**File:** `docs/Analyzers/BWFC024-ScriptManagerCodeBehind.md`

**Content:**
- What patterns are detected
- Why each pattern is problematic in Blazor
- Recommended alternatives
- Code examples

**Phase:** P1

---

#### 6.5.4 Sample Page: ClientScript Migration Example

**Location:** `samples/BlazorWebFormsComponents.Web/Components/ClientScriptExample.razor`

**Content:**
- Before (Web Forms code-behind with `RegisterStartupScript`)
- After (Blazor component with `OnAfterRenderAsync` + `IJSRuntime`)
- Before (Web Forms `ScriptManager` + UpdatePanel)
- After (Blazor component with `@bind` or `EventCallback`)

**Phase:** P2

---

#### 6.5.5 Test Cases

| Test Case | Pattern | Priority | Owner |
|-----------|---------|----------|-------|
| TC36-ClientScript-RegisterStartupScript | Simple startup script → `OnAfterRenderAsync` | P0 | Cyclops / Forge |
| TC37-ClientScript-RegisterInclude | External script include detection | P0 | Cyclops / Forge |
| TC38-ClientScript-PostBackReference | `GetPostBackEventReference()` detection | P1 | Cyclops / Forge |
| TC39-ScriptManager-GetCurrent | `ScriptManager.GetCurrent()` detection | P1 | Cyclops / Forge |
| TC40-ScriptManager-SetFocus | `SetFocus()` detection with TODO | P1 | Cyclops / Forge |
| TC41-IPostBackEventHandler | Interface implementation detection | P1 | Cyclops / Forge |
| TC42-ClientScript-FormValidation | `Page.IsValid` + `ClientScript` pattern | P2 | Cyclops / Forge |

**Phase:** P0–P1

---

## 7. Acceptance Criteria

### 7.1 Analyzer Acceptance Criteria

- [ ] **BWFC022 Enhanced:** `PageClientScriptUsageAnalyzer` detects `Page.ClientScript` and emits warning with migration path guidance
  - [ ] Warning message mentions `IJSRuntime` as solution
  - [ ] Diagnostic includes reference to `ClientScriptMigrationGuide.md` docs
  - [ ] Builds without regression in existing tests
  
- [ ] **BWFC023 Enhanced:** `IPostBackEventHandlerUsageAnalyzer` detects interface and suggests `EventCallback<T>`
  - [ ] Warning message includes `EventCallback` pattern suggestion
  - [ ] Diagnostic includes documentation reference
  - [ ] Builds without regression

- [ ] **BWFC024 New:** `ScriptManagerCodeBehindUsageAnalyzer` detects `ScriptManager.GetCurrent()` and method calls
  - [ ] Detects `GetCurrent(Page)` and `GetCurrent(this)` calls
  - [ ] Detects `.SetFocus()`, `.RegisterAsyncPostBackControl()`, `.RegisterUpdateProgress()`
  - [ ] Emits clear TODO guidance for each pattern
  - [ ] Test coverage: At least 3 test cases for different method calls

---

### 7.2 CLI Transform Acceptance Criteria

- [ ] **ClientScriptTransform:** Detects `RegisterStartupScript` and generates `OnAfterRenderAsync` skeleton
  - [ ] Parses call with literal script string
  - [ ] Injects `OnAfterRenderAsync(bool firstRender)` with `if (firstRender)` guard
  - [ ] Includes `await JS.InvokeVoidAsync("eval", @"SCRIPT");` or similar
  - [ ] Adds TODO comment: "Convert to IJSRuntime.InvokeVoidAsync()"
  - [ ] Test: TC36-ClientScript-RegisterStartupScript.aspx

- [ ] **ClientScriptIncludeTransform:** Detects `RegisterClientScriptInclude` and emits TODO
  - [ ] Parses call with URL string
  - [ ] Generates TODO with suggestion to add `<script>` tag to layout
  - [ ] Alternative: Suggest `JS.InvokeAsync("import", url)`
  - [ ] Test: TC37-ClientScript-RegisterInclude.aspx

- [ ] **PostBackEventTransform:** Detects `GetPostBackEventReference` and emits TODO
  - [ ] Parses method call
  - [ ] Emits TODO: "Not available in Blazor; use EventCallback or direct onclick"
  - [ ] Test: TC38-ClientScript-PostBackReference.aspx

---

### 7.3 Documentation Acceptance Criteria

- [ ] **ClientScriptMigrationGuide.md** delivered and reviewed
  - [ ] 5+ sections covering major patterns
  - [ ] Tabbed Web Forms / Blazor examples
  - [ ] Code examples are accurate and testable
  - [ ] mkdocs.yml updated with navigation entry
  - [ ] Cross-links verified

- [ ] **BWFC022 Reference Page** (PageClientScriptUsage.md)
  - [ ] Documents the analyzer
  - [ ] Links to `ClientScriptMigrationGuide.md`
  - [ ] Code examples provided

- [ ] **BWFC024 Reference Page** (ScriptManagerCodeBehind.md)
  - [ ] Documents the analyzer
  - [ ] Covers `SetFocus()`, `RegisterAsyncPostBackControl()`, etc.
  - [ ] Provides alternatives for each pattern

---

### 7.4 Integration Acceptance Criteria

- [ ] All new analyzers compile without warnings
- [ ] All CLI transforms integrate into `ShimGenerator` or `CodeBehindTransformPipeline`
- [ ] Existing analyzer tests continue to pass (no regressions)
- [ ] New test cases pass (TC36–TC42)
- [ ] Documentation builds without errors: `mkdocs build`
- [ ] Sample page renders without errors

---

## 8. Risks, Non-Goals & Open Questions

### 8.1 Risks

#### 8.1.1 Risk: Over-Ambition — Attempting Full Postback Emulation
**Severity:** HIGH  
**Mitigation:** Explicitly define boundaries (§3.2) and stick to them. Do NOT auto-transform `__doPostBack()` or postback validation scripts.

#### 8.1.2 Risk: Incomplete Analysis — Missing Common Patterns
**Severity:** MEDIUM  
**Mitigation:** Conduct audit of top 50 Web Forms apps in Forge's test suite. Identify 80/20 patterns and prioritize transform coverage.

#### 8.1.3 Risk: TODO Overload — Developers Frustrated by Too Many TODOs
**Severity:** MEDIUM  
**Mitigation:** Keep TODO messages specific and actionable. Reference documentation and provide code examples, not generic warnings.

#### 8.1.4 Risk: JavaScript Interop Complexity — Developers Struggle with IJSRuntime
**Severity:** MEDIUM  
**Mitigation:** Provide detailed sample page with runnable example. Document common JS interop patterns in migration guide.

### 8.2 Non-Goals (Explicitly OUT of Scope)

❌ **Full Web Forms postback emulation** — Blazor component model is fundamentally different  
❌ **UpdatePanel async postback support** — Would require architectural changes  
❌ **Automatic `__doPostBack()` conversion** — Not possible without full control flow analysis  
❌ **FormAuthentication / Membership integration** — Separate (larger) migration effort  
❌ **Global.asax Application_Error integration with ClientScript** — Separate concern  

---

### 8.3 Open Questions (To Resolve Before Implementation)

1. **Question:** Should BWFC provide a JavaScript module (`blazor-web-forms.js`) with helper functions like `applyTheme()`, `focus()`, etc., to reduce boilerplate?
   - **Option A:** Yes — provide utility functions and document their usage
   - **Option B:** No — expect developers to write JavaScript modules themselves
   - **Recommendation:** Option A with caveats: provide *simple* utilities only (focus, basic event handling); don't replicate full Web Forms semantics

2. **Question:** Should BWFC provide a minimal internal helper/registry for queued startup scripts?
   - **Option A:** Yes — a small registry that dedupes scripts by key, queues them, and flushes on first render
   - **Option B:** No — developers use `IJSRuntime` directly in all cases
   - **Recommendation:** Option A for Phase 3 (optional). This is **not** a full compatibility shim, just a convenience for simple deterministic patterns. Bounded in scope: key-based deduplication, inline script execution, first-render timing only.

3. **Question:** How should we handle dynamic script includes that depend on user state?
   - **Example:** `if (User.IsAdmin) { RegisterClientScriptInclude("admin-tools.js"); }`
   - **Recommendation:** Emit TODO pointing to conditional `<script>` loading in Blazor or component-level conditionals

4. **Question:** Should we add an analyzer warning for `<script>` tags in `.aspx` markup that should become `IJSRuntime.InvokeAsync()`?
   - **Recommendation:** No — static scripts in HTML are fine; only flag inline `ClientScript` calls

---

## 9. Implementation Roadmap

### Phase 1 (P1) — Core Diagnostics & Basic Transforms
**Duration:** 2 weeks  
**Owner:** Forge (CLI) + Cyclops (Analyzers)

- [ ] Enhance `BWFC022` analyzer with TODO guidance
- [ ] Add `BWFC024` analyzer for `ScriptManager` code-behind
- [ ] Implement `ClientScriptTransform` (startup scripts)
- [ ] Implement `ClientScriptIncludeTransform` (script includes)
- [ ] Add TC36, TC37, TC38 test cases
- [ ] Write `ClientScriptMigrationGuide.md` (Beast)
- [ ] Write analyzer reference pages (Beast)

**Deliverables:**
- ✅ 3 analyzers (BWFC022 enhanced, BWFC023 enhanced, BWFC024 new)
- ✅ 2 CLI transforms (ClientScript, ClientScriptInclude)
- ✅ 3 test cases
- ✅ 1,200+ lines documentation

---

### Phase 2 (P2) — Enhanced Guidance & Samples
**Duration:** 1 week  
**Owner:** Beast (Docs) + Jubilee (Samples)

- [ ] Create sample page: `ClientScriptExample.razor` with before/after
- [ ] Add "Common Pitfalls" section to migration guide
- [ ] Add `.squad/decisions/` entry documenting ClientScript strategy
- [ ] Add TC39–TC42 test cases for advanced patterns

**Deliverables:**
- ✅ Sample page with runnable example
- ✅ Enhanced documentation
- ✅ 4 additional test cases

---

### Phase 3 (P3) — Runtime Helpers (Optional, Lower Priority)
**Duration:** 2 weeks  
**Owner:** Cyclops (Components)

- [ ] Evaluate implementing a **minimal internal helper/registry** for startup scripts (NOT a full compatibility shim)
- [ ] Helper features: key-based deduplication, first-render flushing, queued script execution
- [ ] Consider JavaScript utility module (`blazor-web-forms.js`) with safe helper functions (focus, event binding)
- [ ] Document lifecycle constraints and when to use helper vs. direct `IJSRuntime`

**Key constraint:** Helper is **deterministic and bounded** — simple inline scripts only, no complex patterns.

**Deliverables:**
- ⚠️ Minimal internal helper/registry (optional; may not be implemented)
- ⚠️ Utility JavaScript module (optional)
- ✅ Lifecycle and usage documentation

---

## 10. Success Metrics

### 10.1 Adoption Metrics

- **Analyzer adoption:** All 3 analyzers (BWFC022, BWFC023, BWFC024) enabled by default in new projects
- **Transform usage:** CLI reports `ClientScriptTransform` ran and generated TODOs in at least 80% of test migrations
- **Documentation engagement:** `ClientScriptMigrationGuide.md` viewed by 100+ developers (internal telemetry if available)

### 10.2 Quality Metrics

- **Test coverage:** All 8 test cases (TC36–TC42) pass, including edge cases
- **Zero regressions:** Existing analyzer tests continue to pass with 100% success rate
- **Documentation completeness:** 5+ sections in migration guide, 80+ total examples across all docs

### 10.3 Developer Experience Metrics

- **Reduced confusion:** Support questions about ClientScript → IJSRuntime decrease by 30%
- **Reduced manual effort:** Developers report ClientScript migration taking <2 hours per app (vs. 4+ previously)
- **Positive feedback:** Feedback on ClientScript guidance > 4/5 stars in community surveys

---

## Appendix: Technical Details

### A.1 Lifecycle Constraints Summary

| Constraint | Why | Workaround |
|-----------|------|-----------|
| No `IsPostBack` | Blazor is stateful, not postback-driven | Check `firstRender` in `OnAfterRenderAsync` |
| No automatic DOM lifecycle | Renderer owns the DOM | Use `@ref` + `JS.InvokeVoidAsync()` |
| No `__doPostBack` | Component method calls replace postback events | Use `@onclick`, `EventCallback`, or button handlers |
| Prerendering incompatibility | Server-side prerender can't invoke JS | Check `!firstRender` in `OnAfterRenderAsync` |
| Re-render deduplication | SignalR optimizes away duplicate updates | Limit side effects in `OnAfterRenderAsync` |

### A.2 Analyzer Diagnostic IDs

| ID | Name | Category | Severity |
|----|----|----------|----------|
| BWFC022 | PageClientScriptUsage | Migration | Warning |
| BWFC023 | IPostBackEventHandlerUsage | Migration | Warning |
| BWFC024 | ScriptManagerCodeBehindUsage | Migration | Warning |

### A.3 Web Forms ClientScript API Reference

**Common methods:**

```csharp
// Startup script (inline)
Page.ClientScript.RegisterStartupScript(
    type: Type,
    key: string,
    script: string,
    addScriptTags: bool = true);

// Client script block (inline, at top of page)
Page.ClientScript.RegisterClientScriptBlock(
    type: Type,
    key: string,
    script: string,
    addScriptTags: bool = true);

// External script include
Page.ClientScript.RegisterClientScriptInclude(
    key: string,
    url: string);

// Postback event reference (dynamic __doPostBack)
string postBackRef = Page.ClientScript.GetPostBackEventReference(
    control: Control,
    argument: string);

// Postback event reference with validation
string postBackRef = Page.ClientScript.GetPostBackEventReference(
    postbackOptions: PostBackOptions);
```

### A.4 Blazor Equivalents

```csharp
// Startup script → OnAfterRenderAsync with IJSRuntime
@inject IJSRuntime JS

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await JS.InvokeVoidAsync("eval", "YOUR_SCRIPT");
        // Better: await JS.InvokeAsync("initializeUI");
    }
}

// Event handling → @onclick or EventCallback
<Button @onclick="HandleClick" />

private void HandleClick()
{
    // Equivalent to __doPostBack event
}

// JS interop → IJSRuntime methods
await JS.InvokeVoidAsync("methodName", param1, param2);
var result = await JS.InvokeAsync<string>("methodName");
```

---

**Document Status:** Ready for review by Forge, Cyclops, and Jeffrey T. Fritz  
**Next Steps:** Prioritize Phase 1 work; assign to Forge and Cyclops; schedule kickoff meeting


