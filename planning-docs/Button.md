>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Button — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.button?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Button`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | string | ✅ Match | From ButtonBaseComponent |
| CommandName | string | ✅ Match | From ButtonBaseComponent |
| CommandArgument | object | ✅ Match | From ButtonBaseComponent |
| CausesValidation | bool | ✅ Match | From ButtonBaseComponent; defaults to true |
| ValidationGroup | string | ✅ Match | From ButtonBaseComponent |
| PostBackUrl | string | ✅ Match | Marked obsolete — not supported in Blazor |
| OnClientClick | string | ✅ Match | From ButtonBaseComponent |
| UseSubmitBehavior | bool | ✅ Match | Marked obsolete — behaves same either way in Blazor |
| ToolTip | string | ✅ Match | Rendered as `title` attribute |

### WebControl Inherited Properties (from BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent; adds `aspNetDisabled` when disabled |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; renders `disabled` attribute |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent; rendered as `id` |
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
| Click | EventHandler | ✅ Match | `EventCallback<MouseEventArgs> OnClick` |
| Command | CommandEventHandler | ✅ Match | `EventCallback<CommandEventArgs> OnCommand` |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders `<input type="submit" ... />` by default (or `<button>` with `UseSubmitBehavior=false`). The Blazor component renders `<button type="submit">` (or `type="button"` when `CausesValidation=false`). The button text is rendered as child content.

The `disabled` attribute is rendered when `Enabled=false`, and the `aspNetDisabled` CSS class is appended — matching Web Forms behavior.

Minor difference: Web Forms uses `<input type="submit" value="...">` while Blazor uses `<button type="submit">Text</button>`.

## Summary

- **Matching:** 22 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 1 property (AccessKey), 0 events
- **N/A (server-only):** 7 items
