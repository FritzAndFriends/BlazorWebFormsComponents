# Shim-First Toolkit Improvements — Forge's Proposal

**Date:** 2026-07-30  
**By:** Forge (Lead / Web Forms Reviewer)  
**Status:** Proposed  
**Requested by:** Jeffrey T. Fritz  
**Builds on:** forge-shim-review.md (same date)

---

## Executive Summary

The shim-first approach is proven (~95% compile-as-is for page classes), but three detection gaps and one silent risk leave non-trivial migration failures at compile time and runtime. This proposal adds **4 CLI transform enhancements**, **2 skill documentation patches**, and **1 new non-page class detection strategy** — all surgical changes that extend the existing architecture without introducing new base classes or breaking changes.

---

## 1. Proposed Changes — CLI Transforms

### 1.1 ServerShimTransform: Detect Transfer/GetLastError/ClearError

**File:** `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/ServerShimTransform.cs`  
**Gap:** Server.Transfer(), Server.GetLastError(), Server.ClearError() are undetected; WingtipToys ErrorPage.aspx.cs, Default.aspx.cs, Global.asax.cs won't compile  
**Priority:** P0 (must-have — compile failure)  
**Complexity:** Small

**What to change:**

Add three new regex patterns after line 28:

```csharp
// Server.Transfer("page.aspx") — no Blazor equivalent; must become NavigationManager.NavigateTo
private static readonly Regex ServerTransferRegex = new(
    @"\bServer\.Transfer\s*\(",
    RegexOptions.Compiled);

// Server.GetLastError() — no direct equivalent; use IExceptionHandlerFeature or middleware
private static readonly Regex ServerGetLastErrorRegex = new(
    @"\bServer\.GetLastError\s*\(\s*\)",
    RegexOptions.Compiled);

// Server.ClearError() — no direct equivalent; errors flow through middleware pipeline
private static readonly Regex ServerClearErrorRegex = new(
    @"\bServer\.ClearError\s*\(\s*\)",
    RegexOptions.Compiled);
```

Update `Apply()` to detect these and emit specific TODO guidance:

```
// TODO(bwfc-server): Server.Transfer() has NO shim — replace with Response.Redirect() or NavigationManager.NavigateTo().
//   Server.Transfer preserves URL; for same effect use forceLoad:true on NavigateTo.
// TODO(bwfc-server): Server.GetLastError() has NO shim — use IExceptionHandlerFeature in error-handling middleware.
// TODO(bwfc-server): Server.ClearError() has NO shim — error handling uses middleware pipeline, remove this call.
```

**Key detail:** These are NOT shimmable. The guidance must clearly say "manual rewrite required" — not "works via shim."

---

### 1.2 SessionDetectTransform: Detect HttpContext.Current.Session

**File:** `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/SessionDetectTransform.cs`  
**Gap:** `HttpContext.Current.Session["key"]` pattern (5 occurrences in ShoppingCartActions.cs) not caught by existing `\bSession\s*\[` regex  
**Priority:** P0 (must-have — compile failure)  
**Complexity:** Small

**What to change:**

Add new regex after line 22:

```csharp
// HttpContext.Current.Session["key"] — common in non-page helper classes
private static readonly Regex HttpContextSessionRegex = new(
    @"\bHttpContext\.Current\.Session\s*\[",
    RegexOptions.Compiled);
```

Update `Apply()`:
1. Check `HttpContextSessionRegex.IsMatch(content)` alongside existing `hasSession`
2. When matched, **replace** `HttpContext.Current.Session[` → `Session[` (safe textual transform)
3. Emit specific TODO: `// TODO(bwfc-session-state): HttpContext.Current.Session replaced with Session. This class needs [Inject] SessionShim or constructor injection — see non-page class guidance below.`

Also detect `HttpContext.Current.` broadly for awareness:

```csharp
private static readonly Regex HttpContextCurrentRegex = new(
    @"\bHttpContext\.Current\b",
    RegexOptions.Compiled);
```

---

### 1.3 ResponseRedirectTransform: Detect ThreadAbortException dead code

**File:** `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/ResponseRedirectTransform.cs`  
**Gap:** `Response.Redirect(url, true)` silently ignores `endResponse` param; `catch (ThreadAbortException)` blocks become unreachable dead code  
**Priority:** P1 (should-have — runtime risk, not compile failure)  
**Complexity:** Small

**What to change:**

Add regex after line 28:

```csharp
// catch (ThreadAbortException) — dead code in Blazor; Web Forms threw this on Redirect(url, true)
private static readonly Regex ThreadAbortCatchRegex = new(
    @"catch\s*\(\s*ThreadAbortException\b",
    RegexOptions.Compiled);

// Response.Redirect(url, true) — endResponse parameter silently ignored
private static readonly Regex RedirectEndResponseRegex = new(
    @"Response\.Redirect\s*\([^,]+,\s*true\s*\)",
    RegexOptions.Compiled);
```

Emit warnings:
```
// TODO(bwfc-navigation): Response.Redirect(url, true) — the endResponse parameter is silently ignored.
//   In Web Forms, true caused ThreadAbortException to end page processing.
//   In Blazor, code after Redirect() always continues. Review control flow.
// TODO(bwfc-navigation): catch (ThreadAbortException) is dead code — Blazor never throws this. Remove the catch block.
```

---

### 1.4 New: NonPageClassDetectTransform

**File:** `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/NonPageClassDetectTransform.cs` (NEW)  
**Gap:** Classes like ShoppingCartActions.cs that use Session/Response/Server but don't inherit WebFormsPageBase get shim guidance that says "works via WebFormsPageBase" — which is wrong for them  
**Priority:** P1 (should-have)  
**Complexity:** Medium

**What to change:**

Create a new transform (Order: 50, runs early) that detects whether a class inherits from a page/component base:

```csharp
// Detect if class inherits WebFormsPageBase, ComponentBase, LayoutComponentBase, etc.
private static readonly Regex PageBaseClassRegex = new(
    @"class\s+\w+\s*:\s*(?:.*?)(?:WebFormsPageBase|ComponentBase|LayoutComponentBase|Page)\b",
    RegexOptions.Compiled);
```

When a file uses shim-relevant APIs (`Session[`, `Response.Redirect`, `Server.MapPath`, `Cache[`) but does NOT inherit a page base class, inject a **different** guidance block:

```
// --- Non-Page Class: Manual DI Required ---
// TODO(bwfc-non-page): This class uses Web Forms APIs but does NOT inherit WebFormsPageBase.
//   Shims are NOT automatically available. You must inject them:
//   
//   Option A: Constructor injection (recommended for services)
//     public class ShoppingCartActions
//     {
//         private readonly SessionShim _session;
//         public ShoppingCartActions(SessionShim session) => _session = session;
//     }
//   
//   Option B: [Inject] attribute (for Blazor components only)
//     [Inject] SessionShim Session { get; set; }
//   
//   Register in Program.cs: builder.Services.AddBlazorWebFormsComponents();
```

Store the `IsPageClass` result in `FileMetadata` (add a bool property) so downstream transforms can adjust their guidance between "works via WebFormsPageBase" vs. "needs manual DI."

---

## 2. Proposed Changes — Migration Skills

### 2.1 bwfc-migration SKILL.md: Add gap patterns

**File:** `migration-toolkit/skills/bwfc-migration/SKILL.md`  
**Priority:** P1  
**Complexity:** Small

Add a new section after the shim table (after line 57):

```markdown
### ⚠️ Server Methods NOT Covered by ServerShim

These `Server.*` methods have NO shim equivalent and require manual rewrite:

| Pattern | Replacement |
|---------|------------|
| `Server.Transfer("page.aspx")` | `Response.Redirect("/page")` or `NavigationManager.NavigateTo("/page", forceLoad: true)` |
| `Server.GetLastError()` | Error-handling middleware with `IExceptionHandlerFeature` |
| `Server.ClearError()` | Remove — error handling is middleware-based |
| `Server.Execute("page.aspx")` | Not applicable in Blazor — decompose into shared components |

### ⚠️ Non-Page Classes Need Manual DI Wiring

Classes that are not Blazor components (services, helpers, utilities) do NOT inherit WebFormsPageBase. They cannot use `Session["key"]` or `Response.Redirect()` directly.

**Pattern to detect:** Any `.cs` file that uses `HttpContext.Current.Session`, `HttpContext.Current.Response`, or `HttpContext.Current.Server` — these must be refactored to accept shims via constructor injection.

**Example (ShoppingCartActions.cs):**
```csharp
// Before: HttpContext.Current.Session["CartId"]
// After:
public class ShoppingCartActions
{
    private readonly SessionShim _session;
    public ShoppingCartActions(SessionShim session) => _session = session;
    
    public string GetCartId() => _session["CartId"]?.ToString();
}
```
```

### 2.2 bwfc-data-migration SKILL.md: Add ThreadAbortException warning

**File:** `migration-toolkit/skills/bwfc-data-migration/SKILL.md`  
**Priority:** P2  
**Complexity:** Small

In the "Static Helpers with HttpContext" section (line 492), add:

```markdown
### ThreadAbortException Is Dead Code

In Web Forms, `Response.Redirect(url, true)` threw `ThreadAbortException` to halt page processing. Any `catch (ThreadAbortException)` blocks are dead code in Blazor — remove them. Review control flow after `Response.Redirect()` calls, as code after the redirect WILL execute in Blazor (it doesn't halt like Web Forms).
```

---

## 3. Proposed Changes — Copilot Instructions Template

### 3.1 copilot-instructions-template.md: Add gap warnings

**File:** `migration-toolkit/copilot-instructions-template.md`  
**Priority:** P1  
**Complexity:** Small

Add to the "Common Gotchas" section (after item 10, line 216):

```markdown
11. **Server.Transfer has no shim** — Replace with `Response.Redirect()` or `NavigationManager.NavigateTo()`. Server.Transfer preserved the URL; use `forceLoad: true` for similar behavior.
12. **Non-page classes need DI** — Service/helper classes that used `HttpContext.Current.Session` cannot use shims directly. Inject `SessionShim`, `ResponseShim`, etc. via constructor.
13. **ThreadAbortException is dead code** — `catch (ThreadAbortException)` blocks after `Response.Redirect(url, true)` never execute in Blazor. Remove them and review control flow.
14. **IdentityHelper.RedirectToReturnUrl** — This helper takes `System.Web.HttpResponse`, not `ResponseShim`. Rewrite to accept `NavigationManager` or `ResponseShim` parameter.
```

---

## 4. What NOT to Do

1. **❌ Do NOT create a new base class for non-page classes** (e.g., `WebFormsServiceBase`). Constructor DI is the correct pattern for services. A base class would create artificial coupling and fight against ASP.NET Core's DI model.

2. **❌ Do NOT add Server.Transfer to ServerShim as a method.** Transfer implies server-side URL rewriting without browser redirect — a fundamentally incompatible concept in Blazor's component model. A `Transfer()` method on ServerShim would give false confidence.

3. **❌ Do NOT auto-remove ThreadAbortException catch blocks.** The CLI should emit TODO guidance only — auto-removal could delete meaningful error-handling logic that the developer wrapped inside the catch block.

4. **❌ Do NOT auto-inject `[Inject] SessionShim` into non-page classes.** The CLI cannot determine the correct DI pattern (constructor vs. property injection, scoping, lifetime). Emit guidance and let the developer or L2 Copilot decide.

5. **❌ Do NOT add `HttpContext.Current` as a shim.** `HttpContext.Current` is a static accessor pattern that fundamentally doesn't work in async/Blazor contexts. The correct fix is to replace it with DI, not to shim it.

---

## 5. Implementation Order

| Order | Item | Priority | Depends On | Est. Effort |
|-------|------|----------|------------|-------------|
| 1 | SessionDetectTransform: `HttpContext.Current.Session` regex | P0 | None | 1 hour |
| 2 | ServerShimTransform: Transfer/GetLastError/ClearError detection | P0 | None | 1 hour |
| 3 | ResponseRedirectTransform: ThreadAbortException detection | P1 | None | 1 hour |
| 4 | NonPageClassDetectTransform: new transform + FileMetadata.IsPageClass | P1 | #1-3 for full benefit | 3 hours |
| 5 | bwfc-migration SKILL.md: gap patterns section | P1 | None | 30 min |
| 6 | copilot-instructions-template.md: gotcha items | P1 | None | 15 min |
| 7 | bwfc-data-migration SKILL.md: ThreadAbort warning | P2 | None | 15 min |

Items 1-3 are independent and can be done in parallel. Item 4 benefits from 1-3 being done first (so the downstream transforms can check `IsPageClass`). Items 5-7 are documentation-only and can ship independently.

---

## 6. Verification Plan

- **Unit tests:** Add test cases to existing CLI transform test suite for each new regex pattern:
  - `Server.Transfer("ErrorPage.aspx")` → detected, TODO emitted
  - `HttpContext.Current.Session["CartId"]` → detected, rewritten, TODO emitted
  - `catch (ThreadAbortException)` → detected, TODO emitted
  - Non-page class with `Session[` → different guidance than page class
- **Integration test:** Run CLI against WingtipToys source, verify all 3 gaps produce TODO comments
- **Regression:** Existing 373 tests must continue passing
