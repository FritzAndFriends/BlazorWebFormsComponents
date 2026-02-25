>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Substitution — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.substitution?view=netframework-4.8
**Blazor Component:** N/A
**Implementation Status:** 🔴 Not Started

## Overview

The Web Forms `Substitution` control specifies a section on an output-cached Web page that is exempt from caching. At runtime, it calls a static method (identified by `MethodName`) that returns a string to replace the control's position in the output. This is tightly coupled to the ASP.NET output caching pipeline.

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| MethodName | `string` | 🔴 Missing | Name of callback method for dynamic content; the core feature |
| ID | `string` | 🔴 Missing | Control.ID |
| Visible | `bool` | 🔴 Missing | Control.Visible |
| EnableTheming | `bool` | N/A | Throws `NotSupportedException` in Web Forms — not supported at all |
| EnableViewState | `bool` | N/A | Server-side only |
| ViewState | `StateBag` | N/A | Server-side only |
| SkinID | `string` | N/A | Throws `NotSupportedException` in Web Forms |

Note: `Substitution` inherits from `Control`, **not** `WebControl`. It has **no** style properties (no CssClass, BackColor, etc.).

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Init | `EventHandler` | 🔴 Missing | Control lifecycle |
| Load | `EventHandler` | 🔴 Missing | Control lifecycle |
| PreRender | `EventHandler` | 🔴 Missing | Control lifecycle |
| Unload | `EventHandler` | 🔴 Missing | Control lifecycle |
| Disposed | `EventHandler` | 🔴 Missing | Control lifecycle |
| DataBinding | `EventHandler` | 🔴 Missing | Control lifecycle |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | Throws `NotSupportedException` in Web Forms |
| Focus() | `void Focus()` | N/A | Throws `NotSupportedException` in Web Forms |

## HTML Output Comparison

**Web Forms** renders the return value of the specified `MethodName` callback directly as HTML text — no wrapper element.

**Blazor** — No implementation exists.

## Implementation Notes

The `Substitution` control is deeply tied to the ASP.NET output caching mechanism (`HttpResponse.WriteSubstitution`). Blazor has no equivalent output caching pipeline. A Blazor equivalent would need to:

1. Accept a `RenderFragment` or `Func<string>` delegate instead of a method name string
2. Possibly integrate with Blazor's `@key` or streaming rendering for partial updates
3. Consider whether any caching layer (e.g., output caching middleware in ASP.NET Core) makes this control relevant

**Recommendation:** This control may be permanently deferred. The output caching concept doesn't map to Blazor's component rendering model. Migration guidance should recommend using Blazor's built-in component rendering lifecycle instead.

## Features That Would Need Implementing

- `MethodName` property pointing to a static method
- Dynamic content callback invocation
- Integration with any caching mechanism
- Base `Control` properties (ID, Visible)
- Lifecycle events (Init, Load, PreRender, Unload, Disposed)

## Summary

- **Matching:** 0 properties, 0 events
- **Needs Work:** 0
- **Missing:** 3 properties, 6 events (entire control)
- **N/A (server-only):** 4 items
