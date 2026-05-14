---
name: bwfc-migration
description: "**WORKFLOW SKILL** — Migrate ASP.NET Web Forms applications (.aspx/.ascx/.master) to Blazor Server using the webforms-to-blazor CLI and BlazorWebFormsComponents shims. WHEN: \"migrate aspx\", \"convert web forms\", \"web forms to blazor\", \"run migration\", \"L2 repair\". INVOKES: webforms-to-blazor CLI. FOR SINGLE OPERATIONS: use bwfc-identity-migration for auth, bwfc-data-migration for EF/architecture."
---

# Web Forms → Blazor Migration with BWFC

## Overview

Three-layer migration architecture:

| Layer | Executor | Coverage |
|-------|----------|----------|
| **L1** | `webforms-to-blazor` CLI | ~70% — 27 transforms, scaffolding, config |
| **L2** | Copilot (this skill) | ~15–20% — TODO-driven semantic transforms |
| **L3** | Developer | ~10–15% — business logic, custom controls |

**Related:** `/bwfc-identity-migration` (auth), `/bwfc-data-migration` (EF/architecture)

## Prerequisites

- .NET 10 SDK, `dotnet tool install -g Fritz.WebFormsToBlazor`

## Critical Rules

1. **USE THE SHIMS** — `WebFormsPageBase` gives every page `Session`, `Response`, `Request`, `Server`, `Cache`, `ViewState`, `ClientScript`. No `[Inject]` needed.
2. **NEVER replace data controls** — `ListView`, `FormView`, `GridView`, `DataList`, `Repeater` → use BWFC component of same name. Repair BWFC markup, never flatten to HTML.
3. **Preserve original patterns** — if Web Forms used `Session["CartId"]`, keep `Session["CartId"]`. The shim handles it.

## Anti-Patterns (DO NOT)

| ❌ Wrong | ✅ Correct |
|----------|-----------|
| `[Inject] IHttpContextAccessor` | `Request.Cookies["key"]` via RequestShim |
| `[Inject] NavigationManager` for redirects | `Response.Redirect("~/path.aspx")` via ResponseShim |
| `HttpContext.Response.Cookies.Append()` | `Session["key"] = value` via SessionShim |
| `app.MapPost("/api/action", ...)` for page actions | Keep as component `@onclick` methods |
| `[ExcludeFromInteractiveRouting]` | Shims work in interactive mode |
| Cookie-based state management | `Session["key"]` via SessionShim |

## Decision Tree

- `Response.Redirect()` → keep, ResponseShim handles ✅
- `Session["key"]` → keep, SessionShim handles ✅
- `Request.QueryString["key"]` → keep, RequestShim handles ✅
- `Request.Cookies["key"]` → SSR: RequestShim; Interactive: use Session instead
- `HttpContext.Current.Session` → replace with `Session` property ✅
- `Request.Form["key"]` → wrap in `<WebFormsForm>` ✅
- `Server.MapPath()` / `Cache["key"]` / `ViewState["key"]` / `ClientScript.*` → all have shims ✅

**Golden Rule:** Preserve the original pattern. Don't reinvent — use the shims.

## Companion Documents

Read these during L2 work:

| Document | Content |
|----------|---------|
| **[REPAIR-PLAYBOOK.md](REPAIR-PLAYBOOK.md)** | Expert triage sequence — fix errors in this order for fastest repair |
| **[WORKFLOW.md](WORKFLOW.md)** | L1→L2→L3 migration phases, CLI options, TODO categories, per-page checklist |
| **[CONFIG-REFERENCE.md](CONFIG-REFERENCE.md)** | Project setup, shim table, WebFormsForm, ClientScript, PostBack |
| **[PATTERNS.md](PATTERNS.md)** | Expression/file/directive conversion, Master Page → Shell, gotchas, troubleshooting |
| **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** | 58 component translation tables |
| **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)** | Lifecycle, event handlers, data binding |
| **[AJAX-TOOLKIT.md](AJAX-TOOLKIT.md)** | Ajax Control Toolkit migration (14 components) |

## L2 Break-Fix Playbook

**Start here → [REPAIR-PLAYBOOK.md](REPAIR-PLAYBOOK.md)** — step-by-step triage order encoding expert repair sequence. Fix errors in the prescribed order to minimize backtracking.

Load only the recipes matching your errors — keep context lean.

| Error Signature | Recipe File |
|---|---|
| `CS7036: no argument ... 'options' of 'XxxContext'` | `recipes/new-dbcontext-to-di.md` |
| `CS0103` on `@ref` fields, no `.razor.cs` | `recipes/missing-code-behind.md` |
| `CS1061: 'GridView<T>' ... 'Rows'/'FindControl'` | `recipes/gridview-row-findcontrol.md` |
| `CS1061: ... 'InnerText'` | `recipes/innertext-to-markup.md` |
| `CS1503: SelectMethod ... 'string' to 'SelectHandler'` | `recipes/selectmethod-string-binding.md` |
| CSS/layout visual regression | `recipes/layout-css-body-class.md` |
| `CS1061: 'RequestShim' ... 'IsLocal'` | `recipes/request-shim-gaps.md` |
| `CS0103` on OAuth fields | `recipes/oauth-page-stubs.md` |
| `CS0246: 'IDatabaseInitializer'` | `recipes/database-seed-initializer.md` |
| `Session.SetString(key, = null)` garbled syntax | `recipes/session-transform-garbling.md` |
| Circular DI: class injects itself | `recipes/circular-self-injection.md` |
| `CS1503`/`CS0123`: EventCallback signature | `recipes/eventcallback-signature-mismatch.md` |
| `CS0542`: nested class same name as outer | `recipes/nested-class-collision.md` |
