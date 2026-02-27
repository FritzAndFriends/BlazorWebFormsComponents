# M17 AJAX / Migration Controls — Web Forms Fidelity Audit

**Auditor:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-28
**Milestone:** M17 — AJAX & Migration Helper Controls
**Source:** Microsoft Learn .NET Framework 4.8.1 API Reference

---

## Coverage Summary

| Control | Properties | Events | HTML Match | Migration Score | Overall |
|---------|-----------|--------|-----------|----------------|---------|
| Timer | 2/2 (100%) | 1/1 (100%) | ✅ N/A (no WF output) | **High** | ✅ |
| ScriptManager | 7/17 (41%) | 0/4 (0%) | ✅ No-op stub | **High** | ⚠️ |
| ScriptManagerProxy | 2/4 (50%) | 0/1 (0%) | ✅ No-op stub | **High** | ⚠️ |
| UpdatePanel | 4/5 (80%) | 0/0 (—) | ✅ Match | **High** | ✅ |
| UpdateProgress | 4/4 (100%) | 0/0 (—) | ⚠️ Near-match | **High** | ⚠️ |
| Substitution | 1/1 (100%) | 0/0 (—) | ✅ N/A (no wrapper) | **Medium** | ✅ |

> **Note on property counts:** Only *control-specific* (non-inherited-from-`Control`) public properties are counted. Inherited `Control` base properties like `ID`, `Visible`, `EnableViewState`, etc. are handled by `BaseWebFormsComponent` and are not re-audited per control.

---

## 1. Timer (`System.Web.UI.Timer`)

**Web Forms:** `System.Web.UI.Timer` — Performs asynchronous or synchronous Web page postbacks at a defined interval. Inherits from `Control`, implements `IPostBackEventHandler`, `IScriptControl`.

**Blazor:** `Timer : BaseWebFormsComponent, IDisposable` — Uses `System.Threading.Timer` for server-side ticking.

### 1.1 Property Audit

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| `Enabled` | `bool` (default `true`) | ✅ Implemented | Inherited from `BaseWebFormsComponent`. Controls whether timer ticks. |
| `Interval` | `int` (default `60000`) | ✅ Implemented | Default 60000ms matches Web Forms exactly. |

### 1.2 Event Audit

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| `Tick` | `EventHandler` | ✅ Implemented | Mapped to `OnTick` (`EventCallback`). Follows project `On` prefix convention. |

### 1.3 HTML Output Comparison

- **Web Forms:** Renders a `<span>` element containing a `<script>` block that creates the client-side `Sys.UI._Timer` component. The `<span>` has the control's `id` and a `style="visibility:hidden;display:none;"`.
- **Blazor:** Renders **nothing** (empty `.razor` template with `@inherits BaseWebFormsComponent`).
- **Assessment:** ✅ **Acceptable.** Timer is a behavioral component. Web Forms renders a hidden `<span>` only as a script host — Blazor's server-side timer needs no DOM presence. No migration CSS or JS depends on Timer's DOM output.

### 1.4 Migration Impact Score: **High** (drop-in replacement)

Developers remove `asp:` prefix and change `OnTick` handler to a Blazor `EventCallback`. Interval and Enabled work identically. The only conceptual difference is server-side vs. client-side ticking, which is transparent to the migrating developer.

---

## 2. ScriptManager (`System.Web.UI.ScriptManager`)

**Web Forms:** Manages ASP.NET Ajax script libraries and script files, partial-page rendering, and client proxy class generation. Inherits from `Control`. Implements `IPostBackDataHandler`, `IPostBackEventHandler`.

**Blazor:** No-op migration stub. Renders nothing. Exists solely to prevent compilation errors during migration.

### 2.1 Property Audit

| Property | Web Forms Type | Default | Blazor Status | Notes |
|----------|---------------|---------|---------------|-------|
| `AllowCustomErrorsRedirect` | `bool` | `true` | ❌ Missing | Server-side error redirect. No Blazor equivalent. Low priority. |
| `AsyncPostBackErrorMessage` | `string` | `""` | ❌ Missing | Error message for async postbacks. No Blazor equivalent. |
| `AsyncPostBackTimeout` | `int` | `90` | ✅ Implemented | Default 90 matches. |
| `EnableCdn` | `bool` | `false` | ✅ Implemented | No-op but accepted for migration. |
| `EnablePageMethods` | `bool` | `false` | ✅ Implemented | No-op but accepted for migration. |
| `EnablePartialRendering` | `bool` | `true` | ⚠️ Partial | **Default mismatch:** WF defaults `true`, Blazor defaults `false` (bare `bool`). |
| `EnableScriptGlobalization` | `bool` | `false` | ✅ Implemented | No-op but accepted for migration. |
| `EnableScriptLocalization` | `bool` | `false` | ✅ Implemented | No-op but accepted for migration. |
| `IsDebuggingEnabled` | `bool` | — | ❌ Missing | Read-only in WF. Not needed for migration markup. |
| `IsInAsyncPostBack` | `bool` | — | ❌ Missing | Read-only. No equivalent in Blazor. |
| `LoadScriptsBeforeUI` | `bool` | `true` | ❌ Missing | Script loading order. No Blazor equivalent. |
| `ScriptMode` | `ScriptMode` | `Auto` | ✅ Implemented | Enum values match: Auto=0, Inherit=1, Debug=2, Release=3. |
| `ScriptPath` | `string` | `""` | ❌ Missing | Physical script path. No Blazor equivalent. Low priority. |
| `Scripts` | `ScriptReferenceCollection` | — | ❌ Missing | Collection property. Only on ScriptManagerProxy in Blazor. See note. |
| `Services` | `ServiceReferenceCollection` | — | ❌ Missing | Collection property. Only on ScriptManagerProxy in Blazor. |
| `SupportsPartialRendering` | `bool` | — | ❌ Missing | Read-only browser capability check. No Blazor equivalent. |
| `EnableHistory` | `bool` | `false` | ❌ Missing | Browser history integration. No Blazor equivalent. |

**Totals:** 7/17 implemented (41%). Appropriate for a no-op stub — only properties commonly set in declarative markup are included.

### 2.2 Event Audit

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| `AsyncPostBackError` | `EventHandler<AsyncPostBackErrorEventArgs>` | ❌ Missing | Server-side error handling. No equivalent needed. |
| `Navigate` | `EventHandler<HistoryEventArgs>` | ❌ Missing | Browser history navigation. No equivalent. |
| `ResolveCompositeScriptReference` | `EventHandler<CompositeScriptReferenceEventArgs>` | ❌ Missing | Script resolution. No equivalent. |
| `ResolveScriptReference` | `EventHandler<ScriptReferenceEventArgs>` | ❌ Missing | Script resolution. No equivalent. |

**Totals:** 0/4 events implemented. **Acceptable** — these are all AJAX-infrastructure events with no Blazor equivalent.

### 2.3 HTML Output Comparison

- **Web Forms:** Renders `<script>` blocks for the Microsoft AJAX Library, client-side framework initialization, and partial rendering infrastructure.
- **Blazor:** Renders **nothing**.
- **Assessment:** ✅ **Correct.** Blazor has its own SignalR/WebSocket infrastructure. ScriptManager's output is entirely replaced by Blazor's built-in framework scripts.

### 2.4 Migration Impact Score: **High** (drop-in replacement)

The purpose of this stub is to let `<asp:ScriptManager ... />` compile without errors. Developers remove the `asp:` prefix and all attributes are silently accepted. The component renders nothing, which is the correct behavior since Blazor doesn't need AJAX script management.

### 2.5 Known Issues

1. **`EnablePartialRendering` default mismatch:** Web Forms defaults to `true`. Blazor stub uses bare `bool` (defaults `false`). If migrating code reads this property, it gets the wrong value. **Recommendation:** Change to `= true`.
2. **Missing `Scripts` collection:** Web Forms ScriptManager can host `<Scripts>` children. Only ScriptManagerProxy has this in Blazor. Low-priority since most apps use ScriptManagerProxy for content-page scripts.

---

## 3. ScriptManagerProxy (`System.Web.UI.ScriptManagerProxy`)

**Web Forms:** Enables nested components (content pages, user controls) to add script and service references when a ScriptManager already exists in a parent element. Inherits from `Control`.

**Blazor:** No-op migration stub. Renders nothing. Accepts `Scripts` and `Services` collections for markup compatibility.

### 3.1 Property Audit

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| `Scripts` | `ScriptReferenceCollection` | ✅ Implemented | `List<ScriptReference>` with Name, Path, Assembly. |
| `Services` | `ServiceReferenceCollection` | ✅ Implemented | `List<ServiceReference>` with Path, InlineScript. |
| `AuthenticationService` | `AuthenticationServiceManager` | ❌ Missing | Complex service configuration. No Blazor equivalent. |
| `CompositeScript` | `CompositeScriptReference` | ❌ Missing | Script bundling. No Blazor equivalent. |
| `ProfileService` | `ProfileServiceManager` | ❌ Missing | Profile service config. No Blazor equivalent. |
| `RoleService` | `RoleServiceManager` | ❌ Missing | Role service config. No Blazor equivalent. |

> Counting only the two commonly-used markup properties (Scripts, Services): **2/2 (100%)**.
> Including all service configuration properties: **2/6 (33%)**.
> The 4 missing properties are deep AJAX infrastructure (AuthenticationService, CompositeScript, ProfileService, RoleService) that have no Blazor equivalent and are rarely set declaratively.

### 3.2 Event Audit

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| `Navigate` | `EventHandler<HistoryEventArgs>` | ❌ Missing | Browser history. No equivalent. |

**Totals:** 0/1 events. Acceptable for a no-op stub.

### 3.3 HTML Output Comparison

- **Web Forms:** Renders nothing visible. Acts as a server-side registration point.
- **Blazor:** Renders nothing.
- **Assessment:** ✅ **Exact match.** Both render no visible HTML.

### 3.4 Migration Impact Score: **High** (drop-in replacement)

Same pattern as ScriptManager. Remove `asp:` prefix, keep `<Scripts>` and `<Services>` children. They compile and are silently ignored.

---

## 4. UpdatePanel (`System.Web.UI.UpdatePanel`)

**Web Forms:** Enables sections of a page to be partially rendered without a full postback. Inherits from `Control`. Implements `IAttributeAccessor` (.NET 4.5+).

**Blazor:** Renders a `<div>` (Block mode) or `<span>` (Inline mode) wrapper around `ChildContent`. Partial rendering is native to Blazor, so the wrapper provides structural/markup compatibility.

### 4.1 Property Audit

| Property | Web Forms Type | Default | Blazor Status | Notes |
|----------|---------------|---------|---------------|-------|
| `ChildrenAsTriggers` | `bool` | `true` | ✅ Implemented | Default `true` matches. |
| `ContentTemplate` | `ITemplate` | — | 🔄 Adapted | Blazor uses `ChildContent` (`RenderFragment`) per Blazor conventions. Intentional adaptation. |
| `IsInPartialRendering` | `bool` | — | ❌ Missing | Read-only runtime state. No Blazor equivalent (always partial). |
| `RenderMode` | `UpdatePanelRenderMode` | `Block` | ✅ Implemented | Enum values match: Block=0, Inline=1. |
| `Triggers` | `UpdatePanelTriggerCollection` | — | ❌ Missing | Blazor doesn't need explicit triggers — all rendering is partial by default. |
| `UpdateMode` | `UpdatePanelUpdateMode` | `Always` | ✅ Implemented | Enum values match: Always=0, Conditional=1. |

**Migration-relevant totals:** 4/5 (80%). `Triggers` is the only omission, and it's deliberately excluded because Blazor's rendering model makes it unnecessary.

### 4.2 Event Audit

No control-specific events on Web Forms `UpdatePanel` (only inherited `Control` lifecycle events).

### 4.3 HTML Output Comparison

- **Web Forms (Block mode):** `<div id="UpdatePanel1">...content...</div>`
- **Blazor (Block mode):** `<div id="UpdatePanel1">...content...</div>`
- **Web Forms (Inline mode):** `<span id="UpdatePanel1">...content...</span>`
- **Blazor (Inline mode):** `<span id="UpdatePanel1">...content...</span>`
- **Assessment:** ✅ **Match.** The outer element and id attribute are identical. Web Forms may include additional attributes during async postback that Blazor won't render, but static HTML is equivalent.

### 4.4 Migration Impact Score: **High** (drop-in replacement)

Replace `<asp:UpdatePanel>` with `<UpdatePanel>`, replace `<ContentTemplate>` with Blazor's implicit `ChildContent`. Remove `<Triggers>` section (not needed). All meaningful properties carry over.

---

## 5. UpdateProgress (`System.Web.UI.UpdateProgress`)

**Web Forms:** Provides visual feedback when UpdatePanel contents are being updated. Inherits from `Control`. Implements `IScriptControl`, `IAttributeAccessor` (.NET 4.5+).

**Blazor:** Renders a `<div>` with `display:none` (DynamicLayout=true) or `visibility:hidden` (DynamicLayout=false). Inherits from `BaseStyledComponent` for CssClass/style support.

### 5.1 Property Audit

| Property | Web Forms Type | Default | Blazor Status | Notes |
|----------|---------------|---------|---------------|-------|
| `AssociatedUpdatePanelID` | `string` | `""` | ✅ Implemented | — |
| `DisplayAfter` | `int` | `500` | ✅ Implemented | Default 500ms matches. |
| `DynamicLayout` | `bool` | `true` | ✅ Implemented | Default `true` matches. |
| `ProgressTemplate` | `ITemplate` | — | ✅ Implemented | Mapped to `RenderFragment ProgressTemplate`. |

**Totals:** 4/4 (100%) of control-specific properties.

### 5.2 Event Audit

No control-specific events on Web Forms `UpdateProgress` (only inherited `Control` lifecycle events).

### 5.3 HTML Output Comparison

- **Web Forms (DynamicLayout=true):** `<div id="UpdateProgress1" style="display:none;">...template...</div>`
- **Blazor (DynamicLayout=true):** `<div id="..." style="display:none;">...template...</div>`
- **Web Forms (DynamicLayout=false):** `<div id="UpdateProgress1" style="display:block;visibility:hidden;">...template...</div>`
- **Blazor (DynamicLayout=false):** `<div id="..." style="visibility:hidden;">...template...</div>`
- **Assessment:** ⚠️ **Near-match.** Two issues:
  1. **DynamicLayout=false:** Web Forms renders `display:block;visibility:hidden;`, Blazor renders only `visibility:hidden;`. Functionally equivalent (div is block by default) but not byte-identical.
  2. **CssClass not rendered:** `UpdateProgress` inherits `BaseStyledComponent` so it accepts `CssClass`, but the `.razor` template doesn't apply `class="@CssClass"` to the `<div>`. Migrating `<asp:UpdateProgress CssClass="loading">` silently drops the class.

### 5.4 Migration Impact Score: **High** (drop-in replacement, with caveats)

Core functionality works: replace `<asp:UpdateProgress>` with `<UpdateProgress>`, keep `<ProgressTemplate>`. The CssClass gap is a fidelity bug that should be fixed in a follow-up PR.

### 5.5 Known Issues

1. **CssClass not rendered on `<div>`** — The `.razor` template should include `class="@CssClass"` on the output `<div>` element.
2. **Missing `display:block;` in non-dynamic mode** — Web Forms explicitly sets `display:block;visibility:hidden;`. Our template uses only `visibility:hidden;`. Functionally identical but not byte-identical.

---

## 6. Substitution (`System.Web.UI.WebControls.Substitution`)

**Web Forms:** Specifies a section on an output-cached Web page that is exempt from caching. Dynamic content is retrieved via a static callback method. Inherits from `Control`.

**Blazor:** Migration helper that accepts a `MethodName` (string, for reference) and a `SubstitutionCallback` (`Func<HttpContext, string>`) for actual Blazor execution.

### 6.1 Property Audit

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| `MethodName` | `string` | ✅ Implemented | Preserved for migration reference. In WF, this names a static callback method. |
| — | — | 🔄 `SubstitutionCallback` | New Blazor-specific property. `Func<HttpContext, string>` replaces WF's static method resolution. |

**Totals:** 1/1 (100%) of Web Forms properties. Plus 1 Blazor adaptation.

### 6.2 Event Audit

No control-specific events on Web Forms `Substitution` (only inherited `Control` lifecycle events).

### 6.3 HTML Output Comparison

- **Web Forms:** Renders **nothing** — the control outputs the return value of the callback method directly into the response stream with no wrapper element.
- **Blazor:** Renders the return value of `SubstitutionCallback` as a `MarkupString` with no wrapper element.
- **Assessment:** ✅ **Match.** Both render raw content with no wrapper element.

### 6.4 Migration Impact Score: **Medium** (minor adaptation required)

Developers must:
1. Remove `asp:` prefix
2. Replace `MethodName="GetDynamicContent"` with `SubstitutionCallback="@MyCallback"` where `MyCallback` is a `Func<HttpContext, string>`
3. Adapt the static method signature from `HttpResponseSubstitutionCallback` to `Func<HttpContext, string>`

This requires code changes beyond simple find-and-replace, hence **Medium** rather than High.

---

## Supporting Types Audit

### Enums

| Enum | Web Forms Values | Blazor Values | Status |
|------|-----------------|---------------|--------|
| `ScriptMode` | Auto=0, Inherit=1, Debug=2, Release=3 | Auto=0, Inherit=1, Debug=2, Release=3 | ✅ Exact match |
| `UpdatePanelRenderMode` | Block=0, Inline=1 | Block=0, Inline=1 | ✅ Exact match |
| `UpdatePanelUpdateMode` | Always=0, Conditional=1 | Always=0, Conditional=1 | ✅ Exact match |

### Helper Classes

| Class | Web Forms Original | Blazor Implementation | Status |
|-------|-------------------|----------------------|--------|
| `ScriptReference` | `System.Web.UI.ScriptReference` — Name, Path, Assembly, ScriptMode, ResourceUICultures, etc. | Name, Path, Assembly | ⚠️ Partial (3 of ~8 properties) |
| `ServiceReference` | `System.Web.UI.ServiceReference` — Path, InlineScript | Path, InlineScript | ✅ Complete |

---

## Consolidated Findings

### What Passes Clean ✅

1. **All 6 component names** match Web Forms originals exactly
2. **Timer** — Full property/event coverage, correct defaults, proper `IDisposable` implementation
3. **UpdatePanel** — All markup-relevant properties present with correct defaults and enum values
4. **Substitution** — `MethodName` preserved, `SubstitutionCallback` is a clean Blazor adaptation
5. **All 3 enums** — Exact int value assignments matching Web Forms
6. **ServiceReference** — Complete property coverage
7. **Base class choices** — Appropriate: stubs → `BaseWebFormsComponent`, UpdateProgress → `BaseStyledComponent`
8. **No-op stubs** (ScriptManager, ScriptManagerProxy) — Correct pattern: accept-but-ignore for compilation compatibility

### Issues Requiring Follow-Up ⚠️

| # | Severity | Control | Issue | Recommendation |
|---|----------|---------|-------|----------------|
| 1 | **Low** | ScriptManager | `EnablePartialRendering` defaults `false`, WF defaults `true` | Change to `= true` |
| 2 | **Low** | ScriptManager | Missing `Scripts` collection | Add `List<ScriptReference> Scripts` for markup parity with WF |
| 3 | **Medium** | UpdateProgress | `CssClass` not rendered on `<div>` output | Add `class="@CssClass"` to template |
| 4 | **Low** | UpdateProgress | Non-dynamic mode missing `display:block;` | Add `display:block;` before `visibility:hidden;` |
| 5 | **Low** | ScriptReference | Only 3 of ~8 WF properties implemented | Add `ScriptMode`, `NotifyScriptLoaded` if needed |

### Architecture Notes

The M17 controls split cleanly into two categories:

1. **Functional components** (Timer, UpdatePanel, UpdateProgress) — These change Blazor behavior and render HTML. They need full property/event coverage.
2. **Pure migration stubs** (ScriptManager, ScriptManagerProxy, Substitution) — These exist to prevent compilation errors during migration. They render nothing (or delegate to a callback). Low property coverage is acceptable because the properties are never functionally used.

---

*Report generated by Forge — Web Forms Fidelity Auditor*
*Reference: Microsoft Learn .NET Framework 4.8.1 API Documentation*
