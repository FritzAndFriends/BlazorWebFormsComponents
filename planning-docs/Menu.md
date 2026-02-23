# Menu — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.menu?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.Menu`
**Implementation Status:** ⚠️ Partial

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited; rendered on root `<div>` |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| DataSource | object | ✅ Match | Direct parameter; supports XmlDocument |
| DisappearAfter | int | ✅ Match | Passed to JS Menu constructor |
| StaticDisplayLevels | int | ✅ Match | Direct parameter |
| StaticSubmenuIndent | int | ✅ Match | Used in CSS `padding-left` |
| Items | Items | ✅ Match | Direct parameter (Menu items) |
| DynamicHoverStyle | DynamicHoverStyle | ✅ Match | Style object; rendered in `<style>` block |
| DynamicMenuStyle | DynamicMenuStyle | ✅ Match | Style object; rendered in `<style>` block |
| DynamicMenuItemStyle | DynamicMenuItemStyle | ✅ Match | Style object; rendered in `<style>` block |
| DynamicSelectedStyle | DynamicSelectedStyle | ✅ Match | Style object; rendered in `<style>` block |
| StaticHoverStyle | StaticHoverStyle | ✅ Match | Style object; rendered in `<style>` block |
| StaticMenuItemStyle | StaticMenuItemStyle | ✅ Match | Style object; rendered in `<style>` block |
| ChildContent | RenderFragment | ✅ Match | Blazor composition |
| DataBindings | — | ⚠️ Needs Work | Implemented via `AddMenuItemBinding()` internally, not as a direct Parameter |
| BackColor | Color | 🔴 Missing | Not BaseStyledComponent; inherits BaseWebFormsComponent |
| BorderColor | Color | 🔴 Missing | |
| BorderStyle | BorderStyle | 🔴 Missing | |
| BorderWidth | Unit | 🔴 Missing | |
| CssClass | string | 🔴 Missing | |
| Font | FontInfo | 🔴 Missing | |
| ForeColor | Color | 🔴 Missing | |
| Height | Unit | 🔴 Missing | |
| Width | Unit | 🔴 Missing | |
| DynamicBottomSeparatorImageUrl | string | 🔴 Missing | |
| DynamicEnableDefaultPopOutImage | bool | 🔴 Missing | |
| DynamicHorizontalOffset | int | 🔴 Missing | |
| DynamicItemFormatString | string | 🔴 Missing | |
| DynamicItemTemplate | ITemplate | 🔴 Missing | |
| DynamicPopOutImageUrl | string | 🔴 Missing | |
| DynamicTopSeparatorImageUrl | string | 🔴 Missing | |
| DynamicVerticalOffset | int | 🔴 Missing | |
| IncludeStyleBlock | bool | 🔴 Missing | Always includes style block |
| ItemWrap | bool | 🔴 Missing | |
| LevelMenuItemStyles | MenuItemStyleCollection | 🔴 Missing | |
| LevelSelectedStyles | MenuItemStyleCollection | 🔴 Missing | |
| LevelSubMenuStyles | SubMenuStyleCollection | 🔴 Missing | |
| MaximumDynamicDisplayLevels | int | 🔴 Missing | |
| Orientation | Orientation | 🔴 Missing | Hardcoded to vertical in JS |
| PathSeparator | char | 🔴 Missing | |
| RenderingMode | MenuRenderingMode | 🔴 Missing | |
| ScrollDownImageUrl | string | 🔴 Missing | |
| ScrollDownText | string | 🔴 Missing | |
| ScrollUpImageUrl | string | 🔴 Missing | |
| ScrollUpText | string | 🔴 Missing | |
| SelectedItem | MenuItem | 🔴 Missing | |
| SelectedValue | string | 🔴 Missing | |
| SkipLinkText | string | 🔴 Missing | |
| StaticBottomSeparatorImageUrl | string | 🔴 Missing | |
| StaticEnableDefaultPopOutImage | bool | 🔴 Missing | |
| StaticItemFormatString | string | 🔴 Missing | |
| StaticItemTemplate | ITemplate | 🔴 Missing | |
| StaticMenuStyle | MenuItemStyle | 🔴 Missing | |
| StaticPopOutImageUrl | string | 🔴 Missing | |
| StaticSelectedStyle | MenuItemStyle | 🔴 Missing | |
| StaticTopSeparatorImageUrl | string | 🔴 Missing | |
| Target | string | 🔴 Missing | |
| ToolTip | string | 🔴 Missing | |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| DataBound | EventHandler | ✅ Match | `OnDataBound` EventCallback |
| DataBinding | EventHandler | ✅ Match | `OnDataBinding` inherited |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |
| MenuItemClick | MenuEventHandler | 🔴 Missing | |
| MenuItemDataBound | MenuEventHandler | 🔴 Missing | |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | ✅ Match | Internal DataBind() handles XmlDocument |
| FindItem() | MenuItem | 🔴 Missing | |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | `<div id="...">` | `<div id="@ID">` ✅ |
| Menu structure | `<ul class="level1">` | `<ul class="level1">` ✅ |
| Menu items | `<li>` with `<a>` | MenuItem component renders `<li>` with `<a>` ✅ |
| Submenus | `<ul class="dynamic">` | Nested `<ul>` via MenuItem ChildContent ✅ |
| Style block | Inline `<style>` | Inline `<style>` with CSS rules ✅ |
| CSS classes | `.static`, `.dynamic`, `.highlighted` | `.static`, `.dynamic`, `.highlighted` ✅ |
| JS behavior | Client-side menu script | `Menu.js` loaded via JS interop ✅ |

HTML output fidelity is good. The Menu component generates the same `<ul>/<li>/<a>` structure with matching CSS classes and inline styles.

## Summary

- **Matching:** 16 properties, 7 events, 1 method (DataBind)
- **Needs Work:** 1 property (DataBindings internal)
- **Missing:** ~35 properties (base styles, dynamic/static image URLs, orientation, scrolling, selection, level styles), 2 events (MenuItemClick, MenuItemDataBound)
- **N/A (server-only):** ~5 items

Menu has solid core rendering with dynamic/static style support and XML data binding (including SiteMap). The JS interop for dynamic menu behavior is working. Main gaps are base WebControl style properties (CssClass, BackColor, etc.), Orientation (hardcoded to vertical), level-specific styles, selection tracking, and MenuItemClick event. The MenuItemDataBound event is noted as a TODO in the source code.
