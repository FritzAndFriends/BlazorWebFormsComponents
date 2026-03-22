# P1–P5 Custom Controls Drop-In Replacement Framework

**Author:** Beast (Technical Writer), based on implementation by Cyclops  
**Date:** 2026-03-18  
**Status:** IMPLEMENTED  
**Issues:** #490 (P1), #491 (P4), #492 (P2), #493 (P3), #494 (P5), #495 (docs), #496 (FindControl)

---

## 1. Executive Summary

The P1–P5 Custom Controls framework extends BWFC's `CustomControls/` namespace to provide a **drop-in replacement** path for migrating ASP.NET Web Forms custom controls to Blazor. The core tenet: developers who inherited from `System.Web.UI.WebControls.WebControl`, `DataBoundControl`, or `CompositeControl` in Web Forms should be able to inherit from BWFC equivalents with the same API shape — same method names, same rendering pipeline, same override points.

### What Was Built

| Phase | Issue | What | Key File(s) |
|-------|-------|------|-------------|
| P2 | #492 | `TagKey` + `AddAttributesToRender` on `WebControl` | `WebControl.cs` |
| P3 | #493 | HtmlTextWriter enum expansion (HTML5, ARIA, CSS3) | `HtmlTextWriter.cs` |
| P1 | #490 | `DataBoundWebControl` + `DataBoundWebControl<T>` | `DataBoundWebControl.cs` |
| P4 | #491 | `CompositeControl` fix + shim types | `CompositeControl.cs`, `LiteralControl.cs`, `ShimControls.cs` |
| P5 | #494 | `TemplatedWebControl` (ITemplate → RenderFragment bridge) | `TemplatedWebControl.cs` |
| — | #496 | `FindControlRecursive` on `BaseWebFormsComponent` | `BaseWebFormsComponent.cs` |

### Validation

- **40 new bUnit tests** across 4 test files (TagKeyTests, DataBoundWebControlTests, ShimControlTests, TemplatedWebControlTests)
- **16 test components** in `TestComponents/`
- 2515 total tests pass, 0 failures
- 5 of 7 DepartmentPortal custom controls can be drop-in shimmed

---

## 2. Architecture

### Class Hierarchy

```
BaseWebFormsComponent                        (BWFC base — ID, Controls, FindControlRecursive)
  └── BaseStyledComponent                    (CssClass, Style, Enabled, Visible, ToolTip, ForeColor, etc.)
        └── WebControl                       (TagKey, Render pipeline, AddAttributesToRender)
              ├── DataBoundWebControl         (DataSource, PerformDataBinding, OnDataBound)
              │     └── DataBoundWebControl<T> (TypedDataItems)
              ├── CompositeControl            (CreateChildControls, RenderChildren)
              ├── TemplatedWebControl         (RenderTemplate, placeholder interleaving)
              ├── LiteralControl              (raw text, no outer tag)
              │     └── Literal               (alias)
              └── [Shim Controls]
                    ├── Panel                 (div container)
                    ├── PlaceHolder           (invisible container)
                    └── HtmlGenericControl    (any tag by string name)

INamingContainer                              (marker interface)
```

### WebControl Rendering Pipeline

The rendering pipeline mirrors Web Forms `System.Web.UI.WebControls.WebControl`:

```
BuildRenderTree(builder)
  │
  ├── if (!Visible) return
  │
  ├── new HtmlTextWriter()
  │
  ├── AddAttributesToRender(writer)     ← adds ID, CssClass, Style, ToolTip, Enabled
  │
  ├── Render(writer)                    ← default calls the three methods below
  │     ├── RenderBeginTag(writer)      ← opens tag from TagKey, consumes pending attributes
  │     ├── RenderContents(writer)      ← override point for inner content
  │     └── RenderEndTag(writer)        ← closes tag
  │
  └── builder.AddMarkupContent(html)    ← converts HtmlTextWriter buffer to Blazor render tree
```

**Two override patterns:**

1. **Override `Render()`** — Full control. The developer writes all open/close tags manually. Pending attributes from `AddAttributesToRender` are consumed by the first `RenderBeginTag` call.

2. **Override `TagKey` + `RenderContents()`** — Web Forms pipeline. The base class handles the outer tag automatically. The developer only writes inner content.

### Attribute Flow

```
AddAttributesToRender(writer)
  │
  ├── writer.AddAttribute("id", ClientID)       ← if ID is set
  ├── writer.AddAttribute("class", CssClass)     ← if CssClass is set
  ├── writer.AddAttribute("style", Style)        ← if Style is set
  ├── writer.AddAttribute("title", ToolTip)      ← if ToolTip is set
  └── writer.AddAttribute("disabled", "disabled") ← if !Enabled
  │
  ▼
  Attributes stored in writer._pendingAttributes
  │
  ▼
  RenderBeginTag(tag)
    └── Flushes _pendingAttributes into <tag attr="val">
```

This means `AddAttributesToRender` is called **before** `Render`, and the pending attributes are consumed by whichever `RenderBeginTag` call comes first — whether that's the default `RenderBeginTag(TagKey)` or a custom `RenderBeginTag(HtmlTextWriterTag.Div)` inside an overridden `Render()`.

---

## 3. API Reference

### 3.1 WebControl

**File:** `src/BlazorWebFormsComponents/CustomControls/WebControl.cs`  
**Inherits:** `BaseStyledComponent`  
**Namespace:** `BlazorWebFormsComponents.CustomControls`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `TagKey` | `HtmlTextWriterTag` | `protected virtual` | Outer tag type. Default: `Span`. Override to change (e.g., `Div`, `Table`). |
| `TagName` | `string` | `public virtual` | String tag name derived from `TagKey` via `ResolveTagName()`. |
| `Render(HtmlTextWriter)` | `void` | `protected virtual` | Main render method. Default calls `RenderBeginTag → RenderContents → RenderEndTag`. |
| `RenderContents(HtmlTextWriter)` | `void` | `protected virtual` | Override point for inner content. Default is empty. |
| `RenderBeginTag(HtmlTextWriter)` | `void` | `public virtual` | Opens outer tag from `TagKey`. Consumes pending attributes. |
| `RenderEndTag(HtmlTextWriter)` | `void` | `public virtual` | Closes outer tag. |
| `RenderControl(HtmlTextWriter)` | `void` | `internal` | Entry point used by `CompositeControl.RenderChildren`. |
| `AddAttributesToRender(HtmlTextWriter)` | `void` | `protected virtual` | Adds ID, CssClass, Style, ToolTip, Enabled. Override to add custom attributes. |
| `BuildRenderTree(RenderTreeBuilder)` | `void` | `protected override` | Blazor integration. Creates `HtmlTextWriter`, calls `AddAttributesToRender` then `Render`, emits markup. |

**Inherited from `BaseStyledComponent`:** `CssClass`, `Style`, `Enabled`, `Visible`, `ToolTip`, `ForeColor`, `BackColor`, `Font`, `Height`, `Width`, `BorderColor`, `BorderStyle`, `BorderWidth`

**Inherited from `BaseWebFormsComponent`:** `ID`, `ClientID`, `Controls`, `Parent`, `FindControlRecursive(string)`

### 3.2 DataBoundWebControl

**File:** `src/BlazorWebFormsComponents/CustomControls/DataBoundWebControl.cs`  
**Inherits:** `WebControl`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `DataSource` | `object` | `[Parameter] public virtual` | The data source. Set as a Blazor parameter. |
| `DataSourceID` | `string` | `[Parameter, Obsolete] public virtual` | Web Forms compat stub. Not functional in Blazor. |
| `DataMember` | `string` | `[Parameter] public virtual` | Data member name for binding. |
| `OnDataBound` | `EventCallback<EventArgs>` | `[Parameter]` | Fires after data binding completes. |
| `DataItems` | `IEnumerable` | `protected` (get only) | Populated from `DataSource` in `OnParametersSet`. |
| `PerformDataBinding(IEnumerable)` | `void` | `protected virtual` | Override to process bound data. Called in `OnParametersSet`. |

**Lifecycle:** `OnParametersSet` casts `DataSource` to `IEnumerable`, stores in `DataItems`, calls `PerformDataBinding`, fires `OnDataBound`.

### 3.3 DataBoundWebControl\<T\>

**File:** `src/BlazorWebFormsComponents/CustomControls/DataBoundWebControl.cs` (same file)  
**Inherits:** `DataBoundWebControl`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `TypedDataItems` | `IEnumerable<T>` | `protected` (get only) | Casts `base.DataItems` via `Cast<T>()`. Returns `Enumerable.Empty<T>()` if null. |

> **Important:** `DataBoundWebControl<T>` does **not** redeclare `DataSource` with `[Parameter]`. See [Design Decisions §4.3](#43-why-databoundwebcontrolt-doesnt-redeclare-datasource).

### 3.4 CompositeControl

**File:** `src/BlazorWebFormsComponents/CustomControls/CompositeControl.cs`  
**Inherits:** `WebControl`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `Controls` | `List<BaseWebFormsComponent>` | `public new` | Child control collection. Accepts any `BaseWebFormsComponent`. |
| `CreateChildControls()` | `void` | `protected virtual` | Override to create and add child controls. |
| `EnsureChildControls()` | `void` | `protected` | Calls `CreateChildControls` if not yet called. |
| `RenderChildren(HtmlTextWriter)` | `void` | `protected` | Iterates `Controls`, calls `RenderControl` on `WebControl` children, `ToString()` fallback for others. |
| `RenderContents(HtmlTextWriter)` | `void` | `protected override` | Calls `EnsureChildControls` then `RenderChildren`. |
| `BuildRenderTree(RenderTreeBuilder)` | `void` | `protected override` | If all children are non-`WebControl`, renders as Blazor components. Otherwise delegates to `base.BuildRenderTree`. |

**P4 fix:** `RenderChildren` no longer throws `NotSupportedException` for non-`WebControl` children — it gracefully falls back to `writer.Write(control.ToString())`.

### 3.5 TemplatedWebControl

**File:** `src/BlazorWebFormsComponents/CustomControls/TemplatedWebControl.cs`  
**Inherits:** `WebControl`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `ChildContent` | `RenderFragment` | `[Parameter]` | Captures implicit content. Prevents whitespace leakage. |
| `RenderTemplate(HtmlTextWriter, RenderFragment)` | `void` | `protected` | Inserts a RenderFragment placeholder into writer output. Null template = no-op. |
| `BuildRenderTree(RenderTreeBuilder)` | `void` | `protected override` | Splits writer HTML on placeholders, interleaves `AddMarkupContent` and `builder.AddContent(seq, renderFragment)`. |

**Placeholder mechanism:**
1. `RenderTemplate` writes `<!--BWFC_TPL_N-->` into the HtmlTextWriter output and stores the `RenderFragment` in a slot list.
2. `BuildRenderTree` splits the final HTML string on these placeholders.
3. For each segment: static markup before the placeholder → `AddMarkupContent`; the RenderFragment itself → `builder.AddContent`.
4. Result: Blazor render tree interleaves static HtmlTextWriter HTML with live Blazor component trees.

### 3.6 LiteralControl + Literal

**File:** `src/BlazorWebFormsComponents/CustomControls/LiteralControl.cs`  
**Inherits:** `WebControl`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `Text` | `string` | `[Parameter]` | Text/HTML content to render. Default: `string.Empty`. |
| `Render(HtmlTextWriter)` | `void` | `protected override` | Writes `Text` directly — no outer tag. |

**`Literal`** is an empty subclass: `public class Literal : LiteralControl { }`. This provides the `System.Web.UI.WebControls.Literal` name for migration compatibility.

### 3.7 Shim Controls (ShimControls.cs)

**File:** `src/BlazorWebFormsComponents/CustomControls/ShimControls.cs`

#### Panel

**Inherits:** `WebControl`  
**Web Forms equivalent:** `System.Web.UI.WebControls.Panel`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `TagKey` | `HtmlTextWriterTag` | `protected override` | Returns `HtmlTextWriterTag.Div`. |
| `Controls` | `List<WebControl>` | `public new` | Child controls rendered inside the div. |
| `RenderContents(HtmlTextWriter)` | `void` | `protected override` | Iterates `Controls`, calls `RenderControl` on each. |

#### PlaceHolder

**Inherits:** `WebControl`  
**Web Forms equivalent:** `System.Web.UI.WebControls.PlaceHolder`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| `Controls` | `List<WebControl>` | `public new` | Child controls. |
| `Render(HtmlTextWriter)` | `void` | `protected override` | Renders children only — no wrapper element. |

#### HtmlGenericControl

**Inherits:** `WebControl`  
**Web Forms equivalent:** `System.Web.UI.HtmlControls.HtmlGenericControl`

| Member | Type | Access | Description |
|--------|------|--------|-------------|
| Constructor | `(string tag = "span")` | `public` | Specifies the HTML tag to render. |
| `Controls` | `List<WebControl>` | `public new` | Child controls. |
| `Render(HtmlTextWriter)` | `void` | `protected override` | Calls `AddAttributesToRender`, opens custom tag, renders children + contents, closes tag. |

#### INamingContainer

**Type:** `interface`  
**Web Forms equivalent:** `System.Web.UI.INamingContainer`

Marker interface. Controls implementing this create a naming scope for child control IDs.

### 3.8 FindControlRecursive

**File:** `src/BlazorWebFormsComponents/BaseWebFormsComponent.cs` (line ~382)  
**Added to:** `BaseWebFormsComponent`

```csharp
public BaseWebFormsComponent FindControlRecursive(string controlId)
```

- Returns first control with matching `ID`, or `null`.
- Checks direct children first (via `Controls.Find`), then recurses.
- Crosses naming container boundaries (unlike Web Forms `FindControl`).
- Null/empty `controlId` returns `null`.

### 3.9 HtmlTextWriter Enums

**File:** `src/BlazorWebFormsComponents/CustomControls/HtmlTextWriter.cs`

#### HtmlTextWriterTag (78 members)

**Original (24):** A, Button, Div, Span, Input, Label, P, Table, Tr, Td, Th, Tbody, Thead, Ul, Li, Select, Option, Img, H1–H6, Form

**P3 additions (54):** HTML5 semantic (Nav, Section, Article, Header, Footer, Main, Aside), structural (Figure, Figcaption, Details, Summary, Dialog, Template, Fieldset, Legend), text (Em, Strong, Small, Code, Pre, Blockquote, Abbr, Cite, Samp, Mark, Sub, Sup, Var), media (Video, Audio, Canvas, Iframe), form (Textarea, Datalist, Output, Meter, Progress), list (Ol, Dl, Dt, Dd), table (Caption, Col, Colgroup, Tfoot), misc (Br, Hr, Map, Area, Ruby, Rt, Rp, Time, Wbr, Address)

#### HtmlTextWriterAttribute (55 members)

**Original (14):** Id, Class, Style, Href, Src, Alt, Name, Type, Value, Title, Width, Height, Disabled, Readonly

**P3 additions (41):** Form (Placeholder, Required, Autofocus, Pattern, Min, Max, Step, Maxlength, Minlength, Multiple, Autocomplete, Action, Method, Enctype), table (Colspan, Rowspan, Scope, Headers, For), state (Checked, Selected, Open), accessibility (Role, Tabindex, AriaLabel, AriaHidden, AriaExpanded, AriaDescribedby, AriaLabelledby, AriaLive, AriaControls, AriaSelected, AriaDisabled), global (Target, Rel, Download, Contenteditable, Draggable, Hidden, Lang, Dir)

#### HtmlTextWriterStyle (77 members)

**Original (15):** BackgroundColor, Color, FontFamily, FontSize, FontWeight, FontStyle, Height, Width, BorderColor, BorderStyle, BorderWidth, Margin, Padding, TextAlign, Display

**P3 additions (62):** Flexbox (FlexDirection, JustifyContent, AlignItems, AlignContent, AlignSelf, FlexWrap, FlexGrow, FlexShrink, FlexBasis, Gap), grid (GridTemplateColumns, GridTemplateRows, GridColumn, GridRow, GridGap), visual (Transform, Transition, Animation, Opacity, BoxShadow, BorderRadius), position (Position, Top, Right, Bottom, Left, ZIndex, Overflow, Float, Clear), text (TextDecoration, TextTransform, TextOverflow, WhiteSpace, WordWrap, LetterSpacing, LineHeight, VerticalAlign), sizing (MinWidth, MaxWidth, MinHeight, MaxHeight, BoxSizing), spacing (MarginTop/Right/Bottom/Left, PaddingTop/Right/Bottom/Left), background (BackgroundImage, BackgroundPosition, BackgroundRepeat, BackgroundSize), outline (OutlineColor, OutlineStyle, OutlineWidth), list (ListStyleType, ListStylePosition), misc (Cursor, Visibility)

**Fallback:** All three switch expressions use `_ => ToString().ToLowerInvariant()` instead of throwing, so unknown future enum values gracefully degrade.

---

## 4. Design Decisions

### 4.1 Why TagKey (enum) Instead of String-Based Tags

Web Forms uses `TagKey` (enum) as the primary mechanism, with `TagName` (string) as a derived convenience property. We mirror this because:

1. **Type safety at compile time** — `HtmlTextWriterTag.Div` catches typos that `"div"` doesn't.
2. **IntelliSense discovery** — developers see all supported tags in the dropdown.
3. **Fallback via `ResolveTagName`** — the switch expression's `_ => ToString().ToLowerInvariant()` ensures any enum value maps to a valid tag name, so extending the enum is always safe.
4. **String tag escape hatch** — `HtmlGenericControl(string tag)` and `HtmlTextWriter.RenderBeginTag(string tagName)` accept arbitrary tag strings when the enum isn't sufficient.

### 4.2 Why Placeholder Approach for Templates

The challenge: `HtmlTextWriter` produces a flat HTML string, but Blazor `RenderFragment` content must be emitted via `builder.AddContent()` — it can't be serialized into a string.

**Approach:** Insert HTML comment placeholders (`<!--BWFC_TPL_0-->`) into the HtmlTextWriter output, then split the final HTML on those placeholders in `BuildRenderTree`. For each segment:
- Static HTML before the placeholder → `builder.AddMarkupContent()`
- The RenderFragment → `builder.AddContent(seq, renderFragment)`

**Why this works:**
- HTML comments are invisible to the browser if somehow leaked.
- The prefix `BWFC_TPL_` is unique enough to never collide with real content.
- Splitting a string is O(n) and allocation-light compared to alternatives like two-pass rendering.
- It allows templates and HtmlTextWriter output to be **interleaved** in any order.

**Alternatives considered:**
- *Two-pass rendering* — render HtmlTextWriter first, then patch in fragments. More complex, same result.
- *Dual builder* — maintain both HtmlTextWriter and RenderTreeBuilder simultaneously. Fragile, ordering bugs.

### 4.3 Why DataBoundWebControl\<T\> Doesn't Redeclare DataSource

Blazor performs **case-insensitive** parameter matching. If `DataBoundWebControl<T>` redeclared:

```csharp
[Parameter] public new IEnumerable<T> DataSource { get; set; }
```

Blazor would see two `DataSource` parameters (one on the base, one on the derived class) with the same name and throw at runtime. Instead, the base class declares `DataSource` as `object`, and the generic subclass provides `TypedDataItems` as a **read-only typed view** via `base.DataItems?.Cast<T>()`.

### 4.4 Why the Literal Alias Exists Despite Name Collision

BWFC already ships a `BlazorWebFormsComponents.Literal` component (the existing editor control). The new `BlazorWebFormsComponents.CustomControls.Literal` is in a different namespace. This means:

- **Custom control authors** who `@using BlazorWebFormsComponents.CustomControls` get the shim `Literal`.
- **BWFC library users** who `@using BlazorWebFormsComponents` get the existing editor `Literal`.
- **Both namespaces imported** — requires disambiguation (`CustomControls.Literal` vs `BlazorWebFormsComponents.Literal`).

The alias exists because Web Forms developers migrating `new LiteralControl(...)` code will search for `Literal` or `LiteralControl`. Having both names available in the `CustomControls` namespace makes migration mechanical.

### 4.5 Why AddAttributesToRender Is Called Before Render

In Web Forms, the call order is: `AddAttributesToRender → RenderBeginTag → RenderContents → RenderEndTag`. Attributes are added to a pending collection, then consumed by `RenderBeginTag`.

BWFC mirrors this exactly: `BuildRenderTree` calls `AddAttributesToRender(writer)` first, which populates `writer._pendingAttributes`. Then `Render(writer)` is called. When `Render` (or the default `RenderBeginTag`) opens the first tag, those pending attributes are flushed into the `<tag>` markup.

This ensures backward compatibility: controls that override `Render()` directly (writing their own `RenderBeginTag`) automatically pick up the base attributes (ID, CssClass, Style, etc.) without any changes.

---

## 5. Migration Patterns

### Pattern 1: Override TagKey + RenderContents (Recommended)

Use this when your Web Forms control overrides `TagKey` and/or `RenderContents`. This is the **simplest migration path** — the base class handles the outer tag, attributes, and visibility automatically.

**Web Forms (before):**

```csharp
public class StarRating : WebControl
{
    public int Rating { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
        base.AddAttributesToRender(writer);
        writer.AddAttribute("data-rating", Rating.ToString());
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        for (int i = 0; i < Rating; i++)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("★");
            writer.RenderEndTag();
        }
    }
}
```

**Blazor (after) — near-identical:**

```csharp
public class StarRating : WebControl  // ← BlazorWebFormsComponents.CustomControls.WebControl
{
    [Parameter]                         // ← add [Parameter]
    public int Rating { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;  // ← unchanged

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
        base.AddAttributesToRender(writer);                                // ← unchanged
        writer.AddAttribute("data-rating", Rating.ToString());
    }

    protected override void RenderContents(HtmlTextWriter writer)          // ← unchanged
    {
        for (int i = 0; i < Rating; i++)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("★");
            writer.RenderEndTag();
        }
    }
}
```

**Changes required:**
1. Change `using System.Web.UI.WebControls` → `using BlazorWebFormsComponents.CustomControls`
2. Add `[Parameter]` to public properties
3. Everything else is identical

### Pattern 2: Override Render for Full Control

Use this when your Web Forms control overrides `Render()` directly. Pending attributes from `AddAttributesToRender` are automatically consumed by the first `RenderBeginTag` call.

**Blazor:**

```csharp
public class NotificationBell : WebControl
{
    [Parameter] public int Count { get; set; }
    [Parameter] public string Icon { get; set; } = "🔔";

    protected override void Render(HtmlTextWriter writer)
    {
        // First RenderBeginTag consumes pending attributes (ID, CssClass, etc.)
        writer.RenderBeginTag(HtmlTextWriterTag.Div);

        writer.AddAttribute(HtmlTextWriterAttribute.Class, "bell-icon");
        writer.RenderBeginTag(HtmlTextWriterTag.Span);
        writer.Write(Icon);
        writer.RenderEndTag();

        if (Count > 0)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "badge");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(Count.ToString());
            writer.RenderEndTag();
        }

        writer.RenderEndTag(); // </div>
    }
}
```

### Pattern 3: Data-Bound Control

**Blazor:**

```csharp
public class EmployeeDataGrid : DataBoundWebControl<Employee>
{
    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Table;

    protected override void RenderContents(HtmlTextWriter writer)
    {
        // TypedDataItems provides IEnumerable<Employee>
        foreach (var emp in TypedDataItems)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(emp.Name);
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(emp.Department);
            writer.RenderEndTag();

            writer.RenderEndTag(); // </tr>
        }
    }
}
```

**Usage:**

```razor
<EmployeeDataGrid DataSource="@employees" CssClass="emp-grid" ID="grdEmployees" />
```

### Pattern 4: Templated Control (ITemplate → RenderFragment)

**Blazor:**

```csharp
public class SectionPanel : TemplatedWebControl
{
    [Parameter] public RenderFragment HeaderTemplate { get; set; }
    [Parameter] public RenderFragment ContentTemplate { get; set; }
    [Parameter] public RenderFragment FooterTemplate { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "header");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        RenderTemplate(writer, HeaderTemplate);  // ← inserts RenderFragment
        writer.RenderEndTag();

        writer.AddAttribute(HtmlTextWriterAttribute.Class, "content");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        RenderTemplate(writer, ContentTemplate);
        writer.RenderEndTag();

        if (FooterTemplate != null)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "footer");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            RenderTemplate(writer, FooterTemplate);
            writer.RenderEndTag();
        }
    }
}
```

**Usage:**

```razor
<SectionPanel CssClass="panel">
    <HeaderTemplate><h2>Employees</h2></HeaderTemplate>
    <ContentTemplate>
        <EmployeeDataGrid DataSource="@employees" />
    </ContentTemplate>
</SectionPanel>
```

### Pattern 5: Composite Control

**Blazor:**

```csharp
public class SearchBox : CompositeControl
{
    [Parameter] public string Placeholder { get; set; } = "Search...";

    protected override void CreateChildControls()
    {
        var label = new LiteralControl { Text = $"<label>{Placeholder}</label>" };
        Controls.Add(label);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        RenderChildren(writer);
        writer.RenderEndTag();
    }
}
```

---

## 6. What Can't Be Shimmed

The following Web Forms features have **no Blazor equivalent** and cannot be emulated by this framework:

| Feature | Why | Migration Path |
|---------|-----|----------------|
| **ViewState** | Blazor has no ViewState. Component state is managed via parameters, fields, and DI. | Use `[Parameter]` properties and component state fields. |
| **PostBack** | No server-side form post model in Blazor. | Use `EventCallback<T>` and Blazor event binding (`@onclick`, `@onchange`). |
| **DataSourceID binding** | Web Forms DataSource controls (`SqlDataSource`, `ObjectDataSource`) don't exist in Blazor. | Use `DataSource` parameter with injected services. The `DataSourceID` property is marked `[Obsolete]`. |
| **Server-side events (OnInit, OnLoad, etc.)** | Web Forms page lifecycle doesn't exist. | Map to Blazor lifecycle: `OnInitialized`, `OnParametersSet`, `OnAfterRender`. |
| **Focus() method** | Requires JS interop from server. | Use Blazor's `ElementReference.FocusAsync()`. |
| **MailDefinition** | Email is a server concern, not a component concern. | Use a server-side email service injected via DI. |
| **EnableTheming / SkinID** | ASP.NET Themes are obsolete. | Use CSS, CascadingValue `ThemeProvider`, or CSS variables. |

> **Diagnostic support:** The BWFC002 analyzer warns when code references `DataSourceID` or other unsupported patterns. See the analyzer documentation for details.

---

## 7. DepartmentPortal Validation

The DepartmentPortal sample application contains 7 custom controls. Testing against the P1–P5 framework:

| Control | Migration Strategy | Shimmed? | Notes |
|---------|--------------------|----------|-------|
| **StarRating** | Pattern 1 (TagKey + RenderContents) | ✅ Drop-in | Override `TagKey => Div`, `AddAttributesToRender` for `data-rating`, `RenderContents` for star spans. |
| **NotificationBell** | Pattern 2 (Override Render) | ✅ Drop-in | Full `Render()` override. Pending attributes consumed by first `RenderBeginTag`. |
| **EmployeeDataGrid** | Pattern 3 (DataBoundWebControl\<T\>) | ✅ Drop-in | Inherits `DataBoundWebControl<Employee>`. Uses `TypedDataItems` in `RenderContents`. |
| **EmployeeCard** | Pattern 5 (CompositeControl) | ✅ Drop-in | Mixed children (LiteralControl + WebControl). P4 fix allows non-WebControl fallback. |
| **SectionPanel** | Pattern 4 (TemplatedWebControl) | ✅ Drop-in | `HeaderTemplate`, `ContentTemplate`, `FooterTemplate` as `RenderFragment` parameters. |
| **DepartmentBreadcrumb** | ❌ Manual rewrite | ❌ | PostBack-dependent navigation. Requires conversion to Blazor `NavigationManager` + `NavLink`. |
| **PollQuestion** | ❌ Manual rewrite | ❌ | PostBack form submission + ViewState for vote tracking. Requires full Blazor event model rewrite. |

**Result:** 5 of 7 (71%) can be migrated as drop-in replacements with only `[Parameter]` annotations and namespace changes.

---

## 8. Test Coverage Map

### New Test Files (P1–P5)

| Test File | Class(es) Tested | Test Count |
|-----------|------------------|------------|
| `TagKeyTests.razor` | `WebControl` (TagKey, AddAttributesToRender, RenderBeginTag, RenderEndTag, Render override compat) | 15 |
| `DataBoundWebControlTests.razor` | `DataBoundWebControl`, `DataBoundWebControl<T>`, OnDataBound | 10 |
| `ShimControlTests.razor` | `LiteralControl`, `Literal`, `Panel`, `CompositeControl` (mixed children) | 7 |
| `TemplatedWebControlTests.razor` | `TemplatedWebControl`, `RenderTemplate`, placeholder interleaving | 8 |
| **Total new tests** | | **40** |

### Pre-Existing Test Files

| Test File | Class(es) Tested | Test Count |
|-----------|------------------|------------|
| `WebControlTests.razor` | `WebControl` (basic rendering, style, CssClass) | 7 |
| `HtmlTextWriterTests.razor` | `HtmlTextWriter` (Write, tags, attributes, styles) | 9 |
| `CompositeControlTests.razor` | `CompositeControl` (CreateChildControls, RenderChildren) | 7 |
| **Total pre-existing** | | **23** |

### Test Components (16 total)

| Component | Purpose |
|-----------|---------|
| `TagKeySpan.cs` | Default TagKey (Span) test |
| `TagKeyDiv.cs` | TagKey override → Div |
| `TagKeyTable.cs` | TagKey override → Table |
| `CustomAttributeControl.cs` | AddAttributesToRender override |
| `HelloLabel.cs` | Render() override backward compat |
| `CustomButton.cs` | Render() override with button |
| `SimpleLabel.cs` | Basic WebControl |
| `SimpleButton.cs` | Basic button WebControl |
| `StyledDiv.cs` | Style attribute testing |
| `SimpleDataList.cs` | Non-generic DataBoundWebControl |
| `TypedEmployeeTable.cs` | Generic DataBoundWebControl\<T\> (includes TestEmployee model) |
| `PanelComposite.cs` | CompositeControl with LiteralControl children |
| `SearchBox.cs` | CompositeControl with mixed children |
| `FormGroup.cs` | CompositeControl with label + input |
| `SimpleSectionPanel.cs` | TemplatedWebControl (3 templates) |
| `SingleTemplateControl.cs` | TemplatedWebControl (1 template + HtmlTextWriter content) |

---

## 9. Upstream Issues

| Issue | Title | Phase | Status |
|-------|-------|-------|--------|
| [#490](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/490) | P1: DataBoundWebControl + DataBoundWebControl\<T\> | P1 (impl order: 3rd) | ✅ Implemented |
| [#491](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/491) | P4: CompositeControl fix + shim types | P4 (impl order: 4th) | ✅ Implemented |
| [#492](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/492) | P2: TagKey + AddAttributesToRender on WebControl | P2 (impl order: 1st) | ✅ Implemented |
| [#493](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/493) | P3: HtmlTextWriter enum expansion | P3 (impl order: 2nd) | ✅ Implemented |
| [#494](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/494) | P5: TemplatedWebControl (ITemplate → RenderFragment bridge) | P5 (impl order: 5th) | ✅ Implemented |
| [#495](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/495) | Documentation for P1–P5 framework | Docs | 🔄 In Progress |
| [#496](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/496) | FindControlRecursive | Utility | ✅ Implemented |

---

## Appendix A: File Inventory

All new/modified files in the `CustomControls/` namespace:

| File | Status | Lines |
|------|--------|-------|
| `src/BlazorWebFormsComponents/CustomControls/WebControl.cs` | Modified (P2) | ~208 |
| `src/BlazorWebFormsComponents/CustomControls/HtmlTextWriter.cs` | Modified (P3) | ~691 |
| `src/BlazorWebFormsComponents/CustomControls/DataBoundWebControl.cs` | New (P1) | ~130 |
| `src/BlazorWebFormsComponents/CustomControls/TemplatedWebControl.cs` | New (P5) | ~131 |
| `src/BlazorWebFormsComponents/CustomControls/LiteralControl.cs` | New (P4) | ~30 |
| `src/BlazorWebFormsComponents/CustomControls/ShimControls.cs` | New (P4) | ~100 |
| `src/BlazorWebFormsComponents/CustomControls/CompositeControl.cs` | Modified (P4) | ~161 |
| `src/BlazorWebFormsComponents/BaseWebFormsComponent.cs` | Modified (#496) | (FindControlRecursive added) |
