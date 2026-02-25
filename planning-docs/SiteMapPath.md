>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# SiteMapPath — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.sitemappath?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.SiteMapPath`
**Implementation Status:** ⚠️ Partial (good coverage)

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited; rendered on root `<span>` |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| CssClass | string | ✅ Match | Inherited from BaseStyledComponent |
| BackColor | WebColor | ✅ Match | Inherited from BaseStyledComponent |
| BorderColor | WebColor | ✅ Match | Inherited from BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | Inherited from BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | Inherited from BaseStyledComponent |
| ForeColor | WebColor | ✅ Match | Inherited from BaseStyledComponent |
| Font | FontInfo | ✅ Match | Inherited from BaseStyledComponent |
| Height | Unit | ✅ Match | Inherited from BaseStyledComponent |
| Width | Unit | ✅ Match | Inherited from BaseStyledComponent |
| SiteMapProvider | SiteMapNode | ✅ Match | Root node parameter (named `SiteMapProvider`) |
| PathSeparator | string | ✅ Match | Defaults to " > " |
| PathSeparatorTemplate | RenderFragment | ✅ Match | Custom separator template |
| PathDirection | PathDirection | ✅ Match | RootToCurrent or CurrentToRoot |
| RenderCurrentNodeAsLink | bool | ✅ Match | Defaults to false |
| ShowToolTips | bool | ✅ Match | Defaults to true |
| ParentLevelsDisplayed | int | ✅ Match | -1 = all; limits parent nodes shown |
| CurrentNodeTemplate | RenderFragment<SiteMapNode> | ✅ Match | Template for current node |
| NodeTemplate | RenderFragment<SiteMapNode> | ✅ Match | Template for regular nodes |
| RootNodeTemplate | RenderFragment<SiteMapNode> | ✅ Match | Template for root node |
| CurrentNodeStyle | CurrentNodeStyle | ✅ Match | Style class for current node |
| NodeStyle | NodeStyle | ✅ Match | Style class for regular nodes |
| RootNodeStyle | RootNodeStyle | ✅ Match | Style class for root node |
| PathSeparatorStyle | PathSeparatorStyle | ✅ Match | Style class for separator |
| CurrentUrl | string | ✅ Match | Blazor-specific; replaces HttpContext-based detection |
| ChildContent | RenderFragment | ✅ Match | Blazor composition |
| Provider | SiteMapProvider | ⚠️ Needs Work | Web Forms uses `SiteMapProvider` (ASP.NET provider); Blazor takes a `SiteMapNode` directly |
| SkipLinkText | string | 🔴 Missing | Accessibility skip link |
| ToolTip | string | 🔴 Missing | On the control itself |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |
| ItemCreated | SiteMapNodeItemEventHandler | 🔴 Missing | |
| ItemDataBound | SiteMapNodeItemEventHandler | 🔴 Missing | |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | `<span id="...">` | `<span id="@ID" class="@CssClass" style="@Style">` ✅ |
| Path nodes | `<a href="...">` links | `<a href="@node.Url">` links ✅ |
| Current node | `<span>` (non-link) | `<span>` when `!RenderCurrentNodeAsLink` ✅ |
| Current node as link | `<a href="...">` | `<a href="...">` when `RenderCurrentNodeAsLink` ✅ |
| Separator | `<span>` with separator text | `<span style="...">` with separator ✅ |
| Tooltips | `title` attribute | `title="@GetTooltip(node)"` ✅ |
| Node styles | Inline styles | Inline styles via Style classes ✅ |

HTML output fidelity is excellent. The breadcrumb structure matches Web Forms output closely.

## Summary

- **Matching:** 27 properties, 5 events
- **Needs Work:** 1 property (Provider model differs)
- **Missing:** 2 properties (SkipLinkText, ToolTip), 2 events (ItemCreated, ItemDataBound)
- **N/A (server-only):** ~5 items

SiteMapPath is one of the best-implemented navigation controls. Nearly all meaningful properties are present: path direction, separator customization, node templates for all positions (root/regular/current), node styles, parent level limiting, and tooltip support. The main adaptation is using a direct `SiteMapNode` parameter instead of the ASP.NET SiteMapProvider infrastructure. SkipLinkText (accessibility) is the only notable missing property.
