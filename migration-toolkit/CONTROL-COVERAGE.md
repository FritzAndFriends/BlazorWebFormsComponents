# Control Coverage Reference

**Can I migrate this control?** This is the complete reference for all 52 BWFC components and the Web Forms controls that are *not* covered.

For the full control translation rules (attribute mappings, code examples, before/after), see the [Copilot migration skill](../.github/skills/webforms-migration/SKILL.md).

---

## Coverage Summary

| Metric | Value |
|---|---|
| BWFC components available | **52** |
| Web Forms control categories covered | **7** (Editor, Data, Validation, Navigation, Login, AJAX, Migration Helper) |
| WingtipToys PoC coverage | **96.6%** (28 of 29 control types used) |
| Controls with no BWFC equivalent | See [Not Supported](#controls-not-supported-by-bwfc) |

---

## Complexity Rating Guide

| Rating | Meaning | Typical Effort |
|---|---|---|
| **Trivial** | Remove `asp:` and `runat="server"`. Done. | < 1 minute |
| **Easy** | Remove prefixes + add `@bind` or adjust one attribute. | 1–5 minutes |
| **Medium** | Data binding rewiring, template context variables, lifecycle method changes. | 5–30 minutes |
| **Complex** | Architecture decisions required — data access, auth, state management. | 30+ minutes |

---

## Editor Controls (25 components)

| Control | BWFC? | Complexity | Key Changes | Gotchas |
|---|---|---|---|---|
| **AdRotator** | ✅ | Easy | Remove `asp:`, `runat` | Configure ad data via component properties |
| **BulletedList** | ✅ | Easy | Remove `asp:`, `runat`; bind `Items` | `DisplayMode` and `BulletStyle` preserved |
| **Button** | ✅ | Trivial | Remove `asp:`, `runat` | `OnClick` is now `EventCallback` — no `(sender, e)` signature |
| **Calendar** | ✅ | Easy | Remove `asp:`, `runat` | `SelectionMode` is an enum — use `CalendarSelectionMode.Day` |
| **CheckBox** | ✅ | Easy | Remove `asp:`, `runat`; add `@bind-Checked` | Two-way binding requires `@bind-Checked` |
| **CheckBoxList** | ✅ | Easy | Remove `asp:`, `runat`; bind `Items` | Same list binding pattern as DropDownList |
| **DropDownList** | ✅ | Easy | Remove `asp:`, `runat`; bind `Items` + `@bind-SelectedValue` | Bind both the items collection and selected value |
| **FileUpload** | ✅ | Easy | Remove `asp:`, `runat` | Uses Blazor `InputFile` internally — `HasFile`, `SaveAs()` work |
| **HiddenField** | ✅ | Trivial | Remove `asp:`, `runat` | `Value` property preserved |
| **HyperLink** | ✅ | Trivial | Remove `asp:`, `runat`; `~/` → `/` | URL prefix conversion is Layer 1 automated |
| **Image** | ✅ | Trivial | Remove `asp:`, `runat`; `~/` → `/` | `ImageUrl` preserved |
| **ImageButton** | ✅ | Trivial | Remove `asp:`, `runat`; `~/` → `/` | `OnClick` is `EventCallback` |
| **Label** | ✅ | Trivial | Remove `asp:`, `runat` | `Text`, `CssClass`, `AssociatedControlID` preserved |
| **LinkButton** | ✅ | Trivial | Remove `asp:`, `runat` | `CommandName`/`CommandArgument` preserved |
| **ListBox** | ✅ | Easy | Remove `asp:`, `runat`; bind `Items` | Same binding pattern as DropDownList |
| **Literal** | ✅ | Trivial | Remove `asp:`, `runat` | `Text` and `Mode` preserved |
| **Localize** | ✅ | Trivial | Remove `asp:`, `runat` | Resource-based text |
| **MultiView** | ✅ | Easy | Remove `asp:`, `runat` | Use with `<View>` child components |
| **Panel** | ✅ | Trivial | Remove `asp:`, `runat` | Renders `<div>` — same as Web Forms |
| **PlaceHolder** | ✅ | Trivial | Remove `asp:`, `runat` | Renders no HTML — structural container only |
| **RadioButton** | ✅ | Easy | Remove `asp:`, `runat` | `GroupName` preserved |
| **RadioButtonList** | ✅ | Easy | Remove `asp:`, `runat`; bind `Items` | Same list binding pattern |
| **Table** | ✅ | Easy | Remove `asp:`, `runat` | Use with `<TableRow>` and `<TableCell>` children |
| **TextBox** | ✅ | Easy | Remove `asp:`, `runat`; add `@bind-Text` | `TextMode` preserved — note `Multiline` (not `MultiLine`) |
| **View** | ✅ | Trivial | Remove `asp:`, `runat` | Used inside `MultiView` |

---

## Data Controls (8 components)

| Control | BWFC? | Complexity | Key Changes | Gotchas |
|---|---|---|---|---|
| **DataGrid** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `Items` | Built-in numeric pager — does not use PagerSettings |
| **DataList** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `Items` | Template `Context="Item"` required |
| **DataPager** | ✅ | Medium | Remove `asp:`, `runat` | Works with PageableData controls |
| **DetailsView** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `DataItem` | Single-record control — uses `DataItem`, not `Items` |
| **FormView** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `DataItem` | `RenderOuterTable="false"` supported; single-record control |
| **GridView** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `Items` | Add `Context="Item"` to templates; `BoundField`/`TemplateField` preserved |
| **ListView** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `Items` | `LayoutTemplate`, `GroupTemplate`, `GroupItemCount` all supported |
| **Repeater** | ✅ | Medium | `ItemType` → `TItem`; `SelectMethod` → `Items` | `SeparatorTemplate` supported |

### Data Control Migration Pattern

All data controls follow the same pattern:

```xml
<!-- Web Forms -->
<asp:GridView ItemType="MyApp.Models.Product" SelectMethod="GetProducts" runat="server">
```

```razor
<!-- Blazor -->
<GridView TItem="Product" Items="products">
```

Load data in the code-behind:

```csharp
protected override async Task OnInitializedAsync()
{
    products = await ProductService.GetProductsAsync();
}
```

- **Collection controls** (GridView, ListView, Repeater, DataList, DataGrid): use `Items` parameter
- **Single-record controls** (FormView, DetailsView): use `DataItem` parameter

---

## Validation Controls (7 components)

| Control | BWFC? | Complexity | Key Changes | Gotchas |
|---|---|---|---|---|
| **CompareValidator** | ✅ | Trivial | Remove `asp:`, `runat` | `ControlToCompare`, `ControlToValidate` preserved |
| **CustomValidator** | ✅ | Easy | Remove `asp:`, `runat` | `OnServerValidate` is `EventCallback` |
| **ModelErrorMessage** | ✅ | Trivial | Remove `asp:`, `runat` | `ModelStateKey` preserved |
| **RangeValidator** | ✅ | Trivial | Remove `asp:`, `runat` | `MinimumValue`, `MaximumValue`, `Type` preserved |
| **RegularExpressionValidator** | ✅ | Trivial | Remove `asp:`, `runat` | `ValidationExpression` preserved |
| **RequiredFieldValidator** | ✅ | Trivial | Remove `asp:`, `runat` | `ControlToValidate`, `ErrorMessage` preserved |
| **ValidationSummary** | ✅ | Trivial | Remove `asp:`, `runat` | `DisplayMode` preserved |

Validation controls are the easiest migration — nearly 1:1 attribute compatibility. Wrap validated forms in `<EditForm>` for full integration.

---

## Navigation Controls (3 components)

| Control | BWFC? | Complexity | Key Changes | Gotchas |
|---|---|---|---|---|
| **Menu** | ✅ | Medium | Remove `asp:`, `runat` | MenuItem structure preserved; dual rendering modes (horizontal/vertical) |
| **SiteMapPath** | ✅ | Medium | Remove `asp:`, `runat` | Provide `SiteMapNode` data programmatically |
| **TreeView** | ✅ | Medium | Remove `asp:`, `runat` | Node expansion state managed by component |

---

## Login Controls (7 components)

| Control | BWFC? | Complexity | Key Changes | Gotchas |
|---|---|---|---|---|
| **ChangePassword** | ✅ | Complex | Remove `asp:`, `runat` | Wire auth provider via service; Orientation/TextLayout enums |
| **CreateUserWizard** | ✅ | Complex | Remove `asp:`, `runat` | Multi-step wizard; requires Identity service wiring |
| **Login** | ✅ | Complex | Remove `asp:`, `runat` | Wire auth provider via service |
| **LoginName** | ✅ | Easy | Remove `asp:`, `runat` | Uses `AuthenticationState` |
| **LoginStatus** | ✅ | Easy | Remove `asp:`, `runat` | Uses `AuthenticationState` |
| **LoginView** | ✅ | Easy | Remove `asp:`, `runat` | Uses `AuthenticationState` for template switching |
| **PasswordRecovery** | ✅ | Complex | Remove `asp:`, `runat` | 3-step wizard; requires Identity service wiring |

> **Important:** BWFC provides the UI components, but the underlying authentication system must be migrated separately. ASP.NET Membership → ASP.NET Core Identity is an architecture decision (Layer 3).

---

## AJAX Controls (5 components)

| Control | BWFC? | Complexity | Key Changes | Gotchas |
|---|---|---|---|---|
| **ScriptManager** | ✅ | Trivial | Remove `asp:`, `runat` | **Migration stub** — renders nothing. Include during migration, remove when stable. |
| **ScriptManagerProxy** | ✅ | Trivial | Remove `asp:`, `runat` | **Migration stub** — renders nothing. Use `IJSRuntime` for script registration. |
| **Timer** | ✅ | Easy | Remove `asp:`, `runat` | Interval-based tick events; no ScriptManager dependency in Blazor |
| **UpdatePanel** | ✅ | Trivial | Remove `asp:`, `runat` | Renders `<div>` or `<span>` — Blazor already does partial rendering |
| **UpdateProgress** | ✅ | Easy | Remove `asp:`, `runat` | Replace automatic UpdatePanel association with explicit `bool IsLoading` state |

> **Note:** ScriptManager and ScriptManagerProxy are intentional no-op stubs. They accept Web Forms attributes silently so your markup compiles, but they don't do anything. Blazor's rendering model replaces the need for AJAX partial postback infrastructure.

---

## Controls NOT Supported by BWFC

These Web Forms controls have **no BWFC equivalent**. Each requires a different migration approach:

### DataSource Controls — Replace with Service Injection

| Control | Migration Approach |
|---|---|
| `SqlDataSource` | Replace with an injected service using EF Core or Dapper |
| `ObjectDataSource` | Replace with an injected service calling your business layer |
| `EntityDataSource` | Replace with an injected `DbContext` via DI |
| `LinqDataSource` | Replace with LINQ queries in an injected service |
| `SiteMapDataSource` | Build navigation data programmatically or from config |
| `XmlDataSource` | Load XML in a service; bind to component properties |

**Pattern:**
```csharp
// Instead of <asp:SqlDataSource SelectCommand="SELECT * FROM Products">
// Inject a service:
@inject IProductService ProductService

@code {
    private List<Product> products = new();

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetProductsAsync();
    }
}
```

### Other Unsupported Controls

| Control | Migration Approach |
|---|---|
| **Wizard** | Implement as a multi-step Blazor component with state tracking |
| **DynamicData** controls | Redesign — Blazor has no DynamicData equivalent |
| **Web Parts** (WebPartManager, WebPartZone, etc.) | Redesign as Blazor components with drag-and-drop libraries |
| **AJAX Control Toolkit** extenders | Find Blazor-native replacements (e.g., Radzen, MudBlazor) or build custom components |
| **ContentPlaceHolder** | Maps directly to Blazor's `@Body` — this is a framework concept, not a component gap |

---

## Coverage by Category — Visual Summary

```
Editor Controls (25)     ████████████████████████████████████████ 100% covered
Data Controls (8)        ████████████████████████████████████████ 100% covered
Validation Controls (7)  ████████████████████████████████████████ 100% covered
Navigation Controls (3)  ████████████████████████████████████████ 100% covered
Login Controls (7)       ████████████████████████████████████████ 100% covered
AJAX Controls (5)        ████████████████████████████████████████ 100% covered
DataSource Controls (6)  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░   0% (by design)
```

DataSource controls are deliberately not covered. They represent a Web Forms-specific pattern (declarative data access in markup) that has no place in Blazor's service-injection architecture.

---

## Cross-References

- [QUICKSTART.md](QUICKSTART.md) — step-by-step migration guide
- [METHODOLOGY.md](METHODOLOGY.md) — why these complexity ratings matter
- [copilot-instructions-template.md](copilot-instructions-template.md) — condensed rules for Copilot
- [Full migration skill](../.github/skills/webforms-migration/SKILL.md) — complete attribute mappings and code examples
