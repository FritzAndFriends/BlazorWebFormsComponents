# Repeater — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.repeater?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.Repeater<ItemType>`
**Implementation Status:** ⚠️ Partial (lean but correct)

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited; used in razor (`@if (Visible && Enabled)`) |
| Enabled | bool | ✅ Match | Inherited; used in razor |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| ItemTemplate | RenderFragment<T> | ✅ Match | Core template for each item |
| AlternatingItemTemplate | RenderFragment<T> | ✅ Match | Alternating items |
| HeaderTemplate | RenderFragment | ✅ Match | Before items |
| FooterTemplate | RenderFragment | ✅ Match | After items |
| SeparatorTemplate | RenderFragment | ✅ Match | Between items |
| Items | IEnumerable<T> | ✅ Match | Inherited from DataBoundComponent<T> |
| DataSource | object | ✅ Match | Inherited |
| DataMember | string | ✅ Match | Inherited from DataBoundComponent<T> |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |

> Note: The Web Forms Repeater inherits from `Control`, not `WebControl`, so it intentionally has NO style properties (no BackColor, CssClass, Font, etc.). This is correct behavior — the Blazor implementation correctly mirrors this.

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| DataBinding | EventHandler | ✅ Match | Inherited |
| DataBound | EventHandler | ✅ Match | Inherited from BaseDataBoundComponent |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |
| ItemCommand | RepeaterCommandEventHandler | 🔴 Missing | |
| ItemCreated | RepeaterItemEventHandler | 🔴 Missing | |
| ItemDataBound | RepeaterItemEventHandler | 🔴 Missing | |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Wrapper element | None (no wrapper) | None ✅ |
| Header | HeaderTemplate rendered first | HeaderTemplate rendered first ✅ |
| Items | ItemTemplate per item | ItemTemplate per item ✅ |
| Alternating | AlternatingItemTemplate | AlternatingItemTemplate ✅ |
| Separator | Between items | Between items ✅ |
| Footer | FooterTemplate rendered last | FooterTemplate rendered last ✅ |

HTML output is controlled entirely by templates. The Repeater adds no HTML of its own, matching Web Forms behavior exactly.

## Summary

- **Matching:** 12 properties, 7 events
- **Needs Work:** 0
- **Missing:** 0 properties, 3 events (ItemCommand, ItemCreated, ItemDataBound)
- **N/A (server-only):** ~5 items

Repeater is one of the most complete components relative to its Web Forms API surface. The Web Forms Repeater is intentionally minimal (no styles, no paging, no editing) — it's just templates + data. The Blazor implementation mirrors this perfectly. The only gaps are three item-level events. This is a solid migration target.
