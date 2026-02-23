# DropDownList — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.dropdownlist?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.DropDownList<TItem>`
**Implementation Status:** ✅ Implemented

## Properties

### ListControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Items | ListItemCollection | ✅ Match | Via `StaticItems` parameter |
| SelectedValue | string | ✅ Match | Two-way bindable via SelectedValueChanged |
| SelectedIndex | int | ✅ Match | Two-way bindable via SelectedIndexChanged |
| SelectedItem | ListItem | ✅ Match | Read-only computed property |
| DataTextField | string | ✅ Match | Maps data to text |
| DataValueField | string | ✅ Match | Maps data to value |
| DataTextFormatString | string | 🔴 Missing | No format string support |
| DataSource | object | ✅ Match | Via DataBoundComponent |
| DataMember | string | ✅ Match | Via DataBoundComponent |
| DataSourceID | string | 🔴 Missing | No server-side DataSource controls |
| AppendDataBoundItems | bool | 🔴 Missing | Always appends static + data items |
| AutoPostBack | bool | ✅ Match | Marked obsolete |
| CausesValidation | bool | 🔴 Missing | Not implemented |
| ValidationGroup | string | 🔴 Missing | Not implemented |

### WebControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in IStyle or base |
| BackColor | Color | ✅ Match | Via IStyle implementation |
| BorderColor | Color | ✅ Match | Via IStyle implementation |
| BorderStyle | BorderStyle | ✅ Match | Via IStyle implementation |
| BorderWidth | Unit | ✅ Match | Via IStyle implementation |
| CssClass | string | ✅ Match | Via IStyle implementation |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; renders `disabled` attribute |
| Font | FontInfo | ✅ Match | Via IStyle implementation |
| ForeColor | Color | ✅ Match | Via IStyle implementation |
| Height | Unit | ✅ Match | Via IStyle implementation |
| Width | Unit | ✅ Match | Via IStyle implementation |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| ToolTip | string | 🔴 Missing | Not implemented on this component |
| Style | CssStyleCollection | ✅ Match | Computed from IStyle |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| EnableViewState | bool | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| Page | Page | N/A | Server-only |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| SelectedIndexChanged | EventHandler | ✅ Match | `EventCallback<ChangeEventArgs> OnSelectedIndexChanged` |
| SelectedValueChanged | — | ✅ Match | `EventCallback<string> SelectedValueChanged` (Blazor two-way) |
| SelectedIndexChanged (binding) | — | ✅ Match | `EventCallback<int> SelectedIndexChanged` (Blazor two-way) |
| TextChanged | EventHandler | 🔴 Missing | Not implemented |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<select>` with `<option>` children. The Blazor component matches this:
```html
<select class="..." style="..." disabled>
  <option value="val1" selected>Text 1</option>
  <option value="val2">Text 2</option>
</select>
```

The `selected` attribute is set on the option matching `SelectedValue`. The `disabled` attribute is rendered when `Enabled=false`.

Note: The Blazor component does not render `id` or `name` attributes on the `<select>` element, unlike Web Forms which renders both.

## Summary

- **Matching:** 20 properties, 7 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 7 properties (AccessKey, ToolTip, DataTextFormatString, DataSourceID, AppendDataBoundItems, CausesValidation, ValidationGroup), 1 event (TextChanged)
- **N/A (server-only):** 7 items
