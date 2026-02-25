# Feature Comparison Audit — Planning Docs

This folder contains one document per ASP.NET Web Forms control, comparing the original .NET Framework 4.8 API surface against our Blazor component implementation.

## Purpose

Systematically identify every gap between the original Web Forms controls and our Blazor components — properties, events, methods, and HTML output — so we can prioritize and close those gaps.

## Document Template

Each `{ControlName}.md` follows this structure:

```markdown
# {ControlName} — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.{controllower}?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.{ControlName}`
**Implementation Status:** ✅ Implemented | 🔴 Not Started | ⚠️ Partial

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text     | string        | ✅ Match      |       |
| CssClass | string        | ⚠️ Partial   | Missing X |
| Visible  | bool          | 🔴 Missing    |       |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Click | EventHandler      | ✅ Match      |       |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus()  | void           | N/A           | Not applicable in Blazor |
| DataBind() | void        | N/A           | Blazor uses parameter binding |

## HTML Output Comparison

Brief notes on rendered HTML differences (if any).

## Summary

- **Matching:** X properties, Y events
- **Needs Work:** X properties, Y events
- **Missing:** X properties, Y events
- **N/A (server-only):** X items
```

## Status Categories

| Status | Meaning |
|--------|---------|
| ✅ Match | Feature exists in Blazor and works the same as Web Forms |
| ⚠️ Needs Work | Feature exists but is incomplete, buggy, or behaves differently |
| 🔴 Missing | Feature does not exist in the Blazor component |
| N/A | Feature is server-side only and doesn't apply to Blazor (ViewState, PostBack, etc.) |

## Controls to Audit

### Editor Controls (28)
AdRotator, BulletedList, Button, Calendar, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TextBox, View, Xml

### Data Controls (9)
Chart, DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater

### Validation Controls (6)
CompareValidator, CustomValidator, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary

### Navigation Controls (3)
Menu, SiteMapPath, TreeView

### Login Controls (7)
ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery

### Supporting Components (15+)
BoundField, ButtonField, HyperLinkField, TemplateField, TableRow, TableCell, TableHeaderRow, TableHeaderCell, TableFooterRow, MenuItem, TreeNode, Content, ContentPlaceHolder, RoleGroup, Page
