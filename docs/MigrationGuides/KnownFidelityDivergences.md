# Known HTML Fidelity Divergences

## Overview

BlazorWebFormsComponents (BWFC) aims to produce **identical HTML output** to the original ASP.NET Web Forms controls. This is a core design goal — matching the rendered DOM structure means existing CSS styles, JavaScript selectors, and visual layouts continue to work after migration.

In practice, however, some structural differences exist between BWFC output and the original Web Forms output. This page catalogs those known divergences so that developers can plan for them during migration.

!!! note "Living Document"
    This page is updated as components are audited and refined. If you discover a divergence not listed here, please open an issue on GitHub.

## ListView

**Severity**: :red_circle: High — May break CSS/JS targeting

The BWFC `ListView` renders a **fundamentally different DOM structure** than the original Web Forms `ListView` control. This is the most significant fidelity divergence in the library.

### What's Different

| Aspect | Web Forms | BWFC |
|--------|-----------|------|
| Group/Item wrapping | `<table>` with `<tr>`/`<td>` wrapping for GroupTemplate, ItemTemplate, and LayoutTemplate | Templates rendered more directly with `<div>` wrappers |
| Structural depth | Deeper nesting due to table-based layout scaffolding | Flatter structure with fewer wrapper elements |
| Output diff | — | ~158-line structural diff in rendered output |

In Web Forms, the `ListView` generates a table-based layout scaffold around templates, using `<tr>` and `<td>` elements to position group and item content. BWFC renders the user-provided templates more directly, producing a cleaner but structurally different DOM.

### Impact

- CSS selectors targeting `table > tr > td` inside a ListView will not match
- JavaScript that traverses the DOM hierarchy (e.g., `parentNode.parentNode`) may break
- Layout behavior may differ if CSS relies on table display semantics

### Workaround

- Use CSS that targets **content classes and attributes** rather than structural element selectors
- Add `data-` attributes in your templates for reliable JS targeting
- Prefer class-based selectors (`.my-item`) over structural selectors (`table tr td`)

```css
/* Avoid — fragile structural selector */
#myListView table tr td .item { ... }

/* Prefer — class-based selector */
#myListView .item { ... }
```

## Calendar Sub-element IDs

**Severity**: :yellow_circle: Medium — Partial impact

The BWFC `Calendar` component renders an `id` attribute on the outer `<table>` element, but it does **not** generate the hierarchical sub-element IDs that Web Forms produces.

### What's Different

In Web Forms, the Calendar control generates IDs for individual structural elements using a hierarchical naming pattern:

```html
<!-- Web Forms output -->
<table id="Calendar1">
  <tr>
    <td id="Calendar1_Day1_1">1</td>
    <td id="Calendar1_Day1_2">2</td>
    ...
  </tr>
</table>
```

BWFC renders the outer table ID but does not generate sub-element IDs:

```html
<!-- BWFC output -->
<table id="Calendar1">
  <tr>
    <td>1</td>
    <td>2</td>
    ...
  </tr>
</table>
```

Individual day cells, navigation links, and other internal elements lack the hierarchical IDs that Web Forms generates.

### Impact

- JavaScript targeting specific day cells by ID (e.g., `document.getElementById('Calendar1_Day1_1')`) will not work
- CSS using `#Calendar1_Day1_1` selectors will not match
- The outer container ID still works for broad targeting

### Workaround

- Use CSS selectors targeting `td` elements within the Calendar table:

```css
/* Target all day cells */
#Calendar1 td { ... }

/* Target specific rows */
#Calendar1 tr:nth-child(3) td { ... }
```

- Add `data-` attributes via Calendar templates for individual cell targeting
- Use positional selectors (`:nth-child`) for specific cells when needed

## Label Element Selection

**Severity**: :green_circle: Low — Minimal impact

The `Label` component renders different HTML elements depending on configuration, which matches Web Forms behavior but may surprise developers.

### What's Different

| Configuration | Rendered Element |
|--------------|-----------------|
| Default (no `AssociatedControlID`) | `<span>` |
| With `AssociatedControlID` set | `<label for="...">` |

This is actually **correct behavior** — BWFC matches Web Forms exactly here. The Web Forms `Label` control always renders as a `<span>` unless `AssociatedControlID` is specified, at which point it renders as a `<label>` element.

### Impact

- Developers who expect `<Label>` to always render an HTML `<label>` element may be surprised
- Accessibility audits may flag the `<span>` output when a `<label>` was intended

### Recommendation

- Always set `AssociatedControlID` when the label is associated with a form control — this produces the correct `<label for="...">` HTML and improves accessibility
- If you need an HTML `<label>` without `AssociatedControlID`, use a plain `<label>` element in your Razor markup

```razor
@* Renders <span>Name:</span> *@
<Label Text="Name:" />

@* Renders <label for="txtName">Name:</label> *@
<Label Text="Name:" AssociatedControlID="txtName" />
```

## FormView Non-Table Path

**Severity**: :green_circle: Low — Expected behavior

When `RenderOuterTable="false"`, the `FormView` renders content without any wrapper element — which means no `id` attribute is emitted on the output.

### What's Different

| Setting | Web Forms | BWFC |
|---------|-----------|------|
| `RenderOuterTable="true"` (default) | Wraps content in `<table>` with `id` | Wraps content in `<table>` with `id` |
| `RenderOuterTable="false"` | Renders content directly, no wrapper | Renders content directly, no wrapper |

BWFC matches Web Forms here — this is correct behavior. When there is no wrapper element, there is no place to render the `id` attribute.

### Impact

- Developers using `RenderOuterTable="false"` cannot use the `ID` parameter for CSS/JS targeting on the FormView itself
- Content inside the FormView is still targetable via its own IDs or classes

### Recommendation

- Use `RenderOuterTable="true"` (the default) if you need the `id` attribute for CSS or JavaScript targeting
- When using `RenderOuterTable="false"`, target content elements directly rather than the FormView container

## ID Rendering Coverage

BWFC supports `id` rendering via the `ID` parameter across a wide range of controls. The rendered `id` attribute uses the value from the `ClientID` property, which follows the Web Forms naming hierarchy.

### Controls Supporting ID Rendering

The following controls emit `id="@ClientID"` when the `ID` parameter is set:

| Category | Controls |
|----------|----------|
| **Editor Controls** | Button, BulletedList, Calendar, CheckBox, DropDownList, FileUpload, HiddenField, Label, LinkButton, Panel, TextBox |
| **Data Controls** | DataGrid, DataList, DetailsView, FormView, GridView |

### ClientIDMode Support

All components support `ClientIDMode` matching the Web Forms behavior:

| Mode | Behavior |
|------|----------|
| `Static` | ID is rendered exactly as specified |
| `Predictable` | ID uses a predictable pattern based on parent hierarchy |
| `AutoID` | ID includes the full naming container path (e.g., `Parent_Child`) |

### Notable Exception: ListView

The `ListView` component **cannot render a root-level ID** because it uses a developer-provided `LayoutTemplate` for its outer markup. The ListView has no single root element that it controls — the outer structure is entirely defined by the template.

To add an ID to a ListView, include it in your `LayoutTemplate`:

```razor
<ListView DataSource="@Items">
    <LayoutTemplate>
        <div id="myListView">
            <PlaceHolder ID="itemPlaceholder" />
        </div>
    </LayoutTemplate>
    <ItemTemplate Context="item">
        <div>@item.Name</div>
    </ItemTemplate>
</ListView>
```

### Opt-in Behavior

ID rendering is **opt-in** — the `id` attribute is only emitted when the developer explicitly sets the `ID` parameter on a component. Components without an `ID` render no `id` attribute in the HTML output.

For comprehensive details, see the [ID Rendering](../UtilityFeatures/IDRendering.md) documentation.

## General Recommendations

### CSS Targeting

When migrating CSS that targets Web Forms output:

1. **Prefer class selectors** over element/structural selectors — classes survive DOM restructuring
2. **Avoid deep descendant selectors** like `table > tbody > tr > td > div` — these are fragile
3. **Use the rendered ID** where available — BWFC generates matching IDs on most controls
4. **Test with browser DevTools** — inspect the rendered HTML to verify selectors match

```css
/* Fragile — depends on exact DOM structure */
#GridView1 > table > tbody > tr:first-child > th { font-weight: bold; }

/* Resilient — targets semantic class */
#GridView1 .header-row { font-weight: bold; }
```

### JavaScript Targeting

When migrating JavaScript that targets Web Forms output:

1. **Use `document.getElementById()`** with the control's `ID` — this is the most reliable approach
2. **Add `data-` attributes** in templates for elements that lack IDs
3. **Avoid DOM traversal** (`parentNode`, `nextSibling`) — the structural nesting may differ
4. **Use `querySelector`** with class or attribute selectors as a fallback

```javascript
// Reliable — uses rendered ID
var grid = document.getElementById('GridView1');

// Reliable — uses data attribute
var items = document.querySelectorAll('[data-item-id]');

// Fragile — assumes specific DOM depth
var cell = element.parentNode.parentNode.firstChild;
```

### Verifying Fidelity

To compare BWFC output against Web Forms output for a specific control:

1. Render the control in both environments with the same properties
2. Compare the HTML output using a diff tool
3. Check that your CSS selectors match both outputs
4. Verify JavaScript interactions work against both DOMs

## See Also

- [Component Health Dashboard](../dashboard.md) — Current coverage and health status for all tracked components
- [ID Rendering](../UtilityFeatures/IDRendering.md) — Detailed documentation on ID rendering and JavaScript integration
- [NamingContainer](../UtilityFeatures/NamingContainer.md) — How hierarchical IDs are generated
- [Styling Components](../UtilityFeatures/StylingComponents.md) — CSS guidance for BWFC components
- [Migration Getting Started](../Migration/readme.md) — Overall migration guide
