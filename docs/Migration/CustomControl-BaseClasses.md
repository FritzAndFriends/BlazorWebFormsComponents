# Custom Control Base Classes and Planned Improvements

The BlazorWebFormsComponents library provides a set of base classes and utilities that make it easier to migrate ASP.NET Web Forms custom controls to Blazor. This guide documents the current inventory, explains how each maps to Web Forms equivalents, and outlines the five planned improvements (P1–P5) that will further close the gap.

---

## Current BWFC Base Class Inventory

### BaseWebFormsComponent

The foundation of all BWFC compatibility components. It provides:

- **Core Web Forms properties:** `ID`, `CssClass`, `Style`
- **Control tree emulation:** `FindControl(string id)` for searching children
- **Enabled/Visible state:** Controls rendering based on these properties
- **HtmlTextWriter integration:** Automatic base attribute application

**Maps to Web Forms:** `System.Web.UI.Control`

**Usage:**
```csharp
public class MyControl : BaseWebFormsComponent
{
    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        writer.Write("Hello");
        writer.RenderEndTag();
    }
}
```

### BaseStyledComponent

Extends `BaseWebFormsComponent` with comprehensive CSS styling support:

- **Inherits:** All from `BaseWebFormsComponent`
- **Adds:** Full CSS style properties (Color, BackColor, BorderWidth, Font, etc.)
- **Provides:** Helper methods for building styled CSS classes
- **Calculated properties:** `CalculatedCssClass`, `CalculatedStyle` for computed CSS

**Maps to Web Forms:** `System.Web.UI.WebControls.WebControl`

**Usage:**
```csharp
public class StyledButton : BaseStyledComponent
{
    [Parameter]
    public string Text { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CalculatedCssClass);
        writer.AddAttribute(HtmlTextWriterAttribute.Style, CalculatedStyle);
        writer.RenderBeginTag(HtmlTextWriterTag.Button);
        writer.Write(Text);
        writer.RenderEndTag();
    }
}
```

### DataBoundComponent<T>

For components that render lists of items (like a Repeater or GridView):

- **Generic parameter `<T>`:** The item type being rendered
- **Automatic child control creation:** Maintains a Controls collection based on item data
- **Item lifetime management:** Handles instantiation and cleanup per item
- **Supports child control discovery:** FindControl searches across all item children

**Maps to Web Forms:** `System.Web.UI.WebControls.DataBoundControl`

**Usage:**
```csharp
public class MyRepeater : DataBoundComponent<Employee>
{
    protected override void CreateChildControls()
    {
        // Called for each item in the data source
        // Build controls for the current item
    }

    public override void DataBind()
    {
        // Called when Items parameter changes
        base.DataBind();
    }
}
```

### WebControl (CustomControls namespace)

A base class for simple controls that render custom HTML without child controls:

- **Inherits from:** `BaseStyledComponent`
- **Provides:** Automatic base attribute rendering (ID, Class, Style)
- **Pattern:** Override `Render(HtmlTextWriter)` to generate HTML

**Maps to Web Forms:** `System.Web.UI.WebControls.WebControl`

**Usage:**
```csharp
public class Badge : WebControl
{
    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public string BadgeType { get; set; } = "info";

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"badge badge-{BadgeType}");
        writer.RenderBeginTag(HtmlTextWriterTag.Span);
        writer.Write(Text);
        writer.RenderEndTag();
    }
}
```

### CompositeControl

A base class for controls that contain child controls:

- **Inherits from:** `WebControl`
- **Provides:** `Controls` collection for child control management
- **Supports:** `CreateChildControls()` pattern for child control creation
- **Limitation:** Currently only supports WebControl-based children

**Maps to Web Forms:** `System.Web.UI.WebControls.CompositeControl`

**Usage:**
```csharp
public class SearchForm : CompositeControl
{
    private Label label;
    private TextBox textBox;
    private Button button;

    protected override void CreateChildControls()
    {
        label = new SimpleLabel { Text = "Search:" };
        textBox = new SimpleTextBox { ID = "query" };
        button = new SimpleButton { Text = "Go" };

        Controls.Add(label);
        Controls.Add(textBox);
        Controls.Add(button);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-form");
        writer.RenderBeginTag(HtmlTextWriterTag.Form);
        RenderChildren(writer);
        writer.RenderEndTag();
    }
}
```

### DataBoundControl

A base class for data-bound controls that uses traditional Web Forms data binding:

- **Inherits from:** `WebControl`
- **Provides:** `DataSource` property and `DataBind()` method
- **Pattern:** Populate `Controls` collection in `CreateChildControls()` based on bound data
- **Limitation:** Does not integrate with HtmlTextWriter rendering

**Maps to Web Forms:** `System.Web.UI.WebControls.DataBoundControl`

**Note:** This class exists but is rarely used in BWFC. The newer `DataBoundComponent<T>` is preferred for most scenarios.

### HtmlTextWriter

A familiar API for rendering HTML that buffers output and converts it to Blazor's render tree:

- **Key methods:** `RenderBeginTag()`, `RenderEndTag()`, `Write()`, `AddAttribute()`, `AddStyleAttribute()`
- **Supported enums:** `HtmlTextWriterTag`, `HtmlTextWriterAttribute`, `HtmlTextWriterStyle`
- **Automatic ID rendering:** If set via the `ID` property, it's rendered on the outer tag
- **Limitation:** HTML5 tags and attributes are incomplete (see P3 below)

**Maps to Web Forms:** `System.Web.UI.HtmlTextWriter`

**Usage:**
```csharp
protected override void Render(HtmlTextWriter writer)
{
    writer.AddAttribute(HtmlTextWriterAttribute.Id, ID);
    writer.AddAttribute(HtmlTextWriterAttribute.Class, "card");
    writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "10px");
    writer.RenderBeginTag(HtmlTextWriterTag.Div);
    
    writer.RenderBeginTag(HtmlTextWriterTag.H3);
    writer.Write(Title);
    writer.RenderEndTag();
    
    writer.Write(Content);
    writer.RenderEndTag();  // Close div
}
```

---

## Web Forms → BWFC Base Class Mapping

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `System.Web.UI.Control` | `BaseWebFormsComponent` | Core functionality; ID, CssClass, Style; FindControl support |
| `System.Web.UI.WebControls.WebControl` | `BaseStyledComponent` or `WebControl` | Full CSS styling properties; choose BaseStyledComponent for more features |
| `System.Web.UI.WebControls.CompositeControl` | `CompositeControl` | Child control management; currently limited to WebControl children |
| `System.Web.UI.WebControls.DataBoundControl` | `DataBoundComponent<T>` | Data-bound rendering with full child control lifecycle |
| `System.Web.UI.HtmlTextWriter` | `HtmlTextWriter` (BWFC version) | Familiar API for rendering; missing some HTML5 tags/attributes |

---

## The Five Planned Improvements (P1–P5)

Analysis of DepartmentPortal's custom controls revealed five key gaps in the current BWFC implementation. These improvements are prioritized by adoption impact and complexity.

### P1: DataBoundWebControl<T> — Data-Bound Rendering with HtmlTextWriter

**Current State:**
- `DataBoundControl` exists for traditional data binding, but it doesn't integrate with HtmlTextWriter
- `DataBoundComponent<T>` exists for component-based data binding, but doesn't support HtmlTextWriter rendering
- No single base class bridges both patterns

**What's Missing:**
A `DataBoundWebControl<T>` base class that:
- Inherits from `WebControl`
- Accepts a generic data source of type `<T>`
- Provides `CreateChildControls()` pattern for HtmlTextWriter-based rendering per item
- Handles item control instantiation and lifecycle
- Allows custom HTML rendering via HtmlTextWriter for each item

**Example Use Case (DepartmentPortal):**

The `EmployeeDataGrid` control renders a table with custom formatting:

```csharp
public class EmployeeDataGrid : DataBoundWebControl<Employee>
{
    [Parameter]
    public IEnumerable<Employee> Employees { get; set; }

    protected override void CreateChildControls()
    {
        var writer = new HtmlTextWriter();
        writer.RenderBeginTag(HtmlTextWriterTag.Table);
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "table");
        
        foreach (var emp in Employees)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write($"{emp.FirstName} {emp.LastName}");
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
        
        writer.RenderEndTag();
    }
}
```

**Proposed API:**
```csharp
public abstract class DataBoundWebControl<T> : WebControl
{
    [Parameter]
    public IEnumerable<T> DataSource { get; set; }

    protected IEnumerable<T> Items => DataSource;

    // Template for each item
    protected virtual void CreateItemControls(T item, HtmlTextWriter writer)
    {
        // Override to render each item
    }

    protected sealed override void Render(HtmlTextWriter writer)
    {
        foreach (var item in Items)
        {
            CreateItemControls(item, writer);
        }
    }
}
```

**Impact:** Enables migration of many enterprise controls (DataGrid, Repeater with custom formatting, custom list controls).

---

### P2: TagKey + AddAttributesToRender — Auto-Rendering Outer Tag

**Current State:**
- `WebControl` requires manual outer tag management in the `Render()` method
- No automatic rendering of a container tag with attributes
- Developers must remember to add ID, Class, Style attributes manually

**What's Missing:**
- A `TagKey` property that specifies the outer HTML tag (e.g., `HtmlTextWriterTag.Div`)
- An `AddAttributesToRender()` method that collects all attributes to render
- Automatic outer tag rendering that calls `AddAttributesToRender()` before yielding to derived class

**Example Use Case (DepartmentPortal):**

The `StarRating` and `NotificationBell` controls are simple wrappers around HTML elements:

```csharp
public class StarRating : WebControl
{
    [Parameter]
    public int Rating { get; set; }

    [Parameter]
    public int MaxRating { get; set; } = 5;

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
        base.AddAttributesToRender(writer);  // Adds ID, Class, Style
        writer.AddAttribute("data-rating", Rating.ToString());
        writer.AddAttribute("aria-label", $"Rating: {Rating} out of {MaxRating}");
    }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(TagKey);
        for (int i = 0; i < MaxRating; i++)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, i < Rating ? "star-filled" : "star-empty");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("★");
            writer.RenderEndTag();
        }
        writer.RenderEndTag();
    }
}
```

**Current workaround (verbose):**
```csharp
protected override void Render(HtmlTextWriter writer)
{
    writer.AddAttribute(HtmlTextWriterAttribute.Id, ID);
    writer.AddAttribute(HtmlTextWriterAttribute.Class, CalculatedCssClass);
    writer.AddAttribute(HtmlTextWriterAttribute.Style, CalculatedStyle);
    writer.AddAttribute("data-rating", Rating.ToString());
    writer.RenderBeginTag(HtmlTextWriterTag.Div);
    // ... content rendering
    writer.RenderEndTag();
}
```

**Impact:** Simplifies 80% of custom control migrations by eliminating boilerplate attribute handling.

---

### P3: HtmlTextWriter Enum Expansion — HTML5 Tags, Attributes, and Styles

**Current State:**
- `HtmlTextWriterTag` enum covers most HTML4 tags but misses modern HTML5 semantics
- `HtmlTextWriterAttribute` lacks data-* attributes, ARIA roles, and accessibility attributes
- `HtmlTextWriterStyle` is missing modern CSS properties (flexbox, grid, transforms, transitions)

**What's Missing:**

**HTML5 Tags:**
```
Nav, Section, Article, Header, Footer, Main, Figure, FigCaption,
Details, Summary, Mark, Time, Dialog, Output, Progress, Meter
```

**HTML5+ Attributes:**
```
data-* (dynamic attributes)
aria-* (accessibility)
role, placeholder, autocomplete, disabled, readonly, required,
crossorigin, integrity, async, defer, type (on script), 
rel (on link), itemprop, itemscope, itemtype, itemref
```

**Modern CSS Properties:**
```
Flex, FlexDirection, FlexWrap, JustifyContent, AlignItems,
Grid, GridTemplateColumns, GridTemplateRows, GridGap,
Transform, Transition, Animation, Opacity, ScaleX, ScaleY,
Rotate, SkewX, SkewY, Perspective
```

**Example Use Case (DepartmentPortal):**

The redesigned navigation component uses semantic HTML:

```csharp
// Current limitation — no Nav, no data attributes
protected override void Render(HtmlTextWriter writer)
{
    writer.RenderBeginTag(HtmlTextWriterTag.Div);  // Should be Nav
    writer.Write("<nav>");  // Workaround: raw HTML string
    // ...
    writer.Write("</nav>");
    writer.RenderEndTag();
}

// With P3 — cleaner, type-safe
protected override void Render(HtmlTextWriter writer)
{
    writer.AddAttribute("data-role", "navigation");  // Currently must use raw string
    writer.AddAttribute("aria-label", "Main navigation");
    writer.RenderBeginTag(HtmlTextWriterTag.Nav);  // Will exist after P3
    // ...
    writer.RenderEndTag();
}
```

**Proposed Changes:**

```csharp
public enum HtmlTextWriterTag
{
    // Existing tags...
    
    // HTML5 Semantic tags
    Nav,
    Section,
    Article,
    Header,
    Footer,
    Main,
    Figure,
    FigCaption,
    Details,
    Summary,
    Mark,
    Time,
    Dialog,
    Output,
    Progress,
    Meter
}

public enum HtmlTextWriterAttribute
{
    // Existing attributes...
    
    // ARIA attributes
    AriaLabel,
    AriaLabelledBy,
    AriaDescribedBy,
    AriaHidden,
    AriaPressed,
    AriaChecked,
    AriaSelected,
    AriaExpanded,
    AriaLevel,
    AriaLive,
    AriaAtomic,
    AriaRelevant,
    AriaRequired,
    AriaInvalid,
    
    // Standard attributes
    Role,
    Placeholder,
    AutoComplete,
    Disabled,
    ReadOnly,
    Required,
    CrossOrigin,
    Integrity,
    Async,
    Defer,
    ItemProp,
    ItemScope,
    ItemType,
    ItemRef,
    
    // Data attributes (special handling for data-*)
    Data  // Use: writer.AddAttribute("data-toggle", "modal")
}

public enum HtmlTextWriterStyle
{
    // Existing styles...
    
    // Flexbox
    Display,  // Already exists, but needed for flex
    FlexDirection,
    FlexWrap,
    JustifyContent,
    AlignItems,
    AlignContent,
    Flex,
    
    // Grid
    Grid,
    GridTemplateColumns,
    GridTemplateRows,
    GridGap,
    GridColumnStart,
    GridColumnEnd,
    GridRowStart,
    GridRowEnd,
    
    // Transforms
    Transform,
    TransformOrigin,
    Perspective,
    PerspectiveOrigin,
    
    // Animations
    Transition,
    Animation,
    
    // Other
    Opacity,
    Filter,
    Cursor,
    UserSelect,
    ClipPath,
    MaskImage
}
```

**Impact:** Enables modern web design patterns without falling back to raw HTML strings; improves accessibility support.

---

### P4: CompositeControl Child Rendering — Support Mixed Child Types

**Current State:**
- `CompositeControl` requires all children to be `WebControl` descendants
- Throws `NotSupportedException` if a child is not a `WebControl`
- Cannot mix WebControl children with raw markup or other component types

**What's Missing:**
- Ability to render children of mixed types (WebControl, markup, native Blazor components)
- `RenderChildren()` method that intelligently handles different child types
- Support for `ChildContent` as well as programmatically added controls

**Example Use Case (DepartmentPortal):**

The `EmployeeCard` contains a mix of controls and custom markup:

```csharp
public class EmployeeCard : CompositeControl
{
    protected override void CreateChildControls()
    {
        Controls.Add(new Image { ImageUrl = emp.PhotoUrl });  // WebControl
        Controls.Add(new Label { Text = emp.Name });         // WebControl
        
        // Currently throws exception:
        var customDiv = new Control();  // Not a WebControl
        Controls.Add(customDiv);
        
        // Want to add raw markup:
        Controls.Add(new HtmlLiteral("<hr />"));  // Doesn't exist
    }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-card");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        RenderChildren(writer);  // Should handle all child types
        writer.RenderEndTag();
    }
}
```

**Proposed Solution:**

```csharp
public class CompositeControl : WebControl
{
    // Accept RenderFragment for mixed content
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Also support programmatic Controls collection with mixed types
    protected void RenderChildren(HtmlTextWriter writer)
    {
        foreach (var child in Controls)
        {
            if (child is WebControl webControl)
            {
                webControl.Render(writer);
            }
            else if (child is IHtmlContent htmlContent)
            {
                writer.Write(htmlContent.ToHtmlString());
            }
            else if (child is string text)
            {
                writer.Write(text);
            }
            else
            {
                throw new InvalidOperationException($"Child type {child.GetType().Name} is not supported");
            }
        }
    }
}
```

**Impact:** Enables migration of complex composite controls (card layouts, dashboard widgets, multi-section panels).

---

### P5: ITemplate → RenderFragment Bridge Pattern

**Current State:**
- Web Forms uses `ITemplate` for parameterized templates
- BWFC has no direct equivalent for Blazor's `RenderFragment<T>`
- Migrating ITemplate-based controls requires manual pattern translation

**What's Missing:**
- A bridge class that converts `ITemplate` interface to `RenderFragment<T>`
- Guidance on the new Blazor template pattern for custom controls
- Automated conversion examples

**Example Use Case (DepartmentPortal):**

The `SectionPanel` control uses `ITemplate` for flexible content:

**Web Forms:**
```html
<asp:SectionPanel runat="server">
    <HeaderTemplate>
        <h2>Announcements</h2>
    </HeaderTemplate>
    <ContentTemplate>
        <asp:Repeater ID="Announcements" runat="server" />
    </ContentTemplate>
</asp:SectionPanel>
```

**Current BWFC limitation:**
- `ITemplate` doesn't translate directly to Blazor components
- Must manually define `RenderFragment` parameters

**Proposed Bridge Solution:**

```csharp
public class SectionPanel : CompositeControl
{
    // Old Web Forms style (for migration compat)
    [Parameter]
    public ITemplate HeaderTemplate { get; set; }
    
    [Parameter]
    public ITemplate ContentTemplate { get; set; }

    // New Blazor style (recommended)
    [Parameter]
    public RenderFragment Header { get; set; }
    
    [Parameter]
    public RenderFragment Content { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Section);
        
        if (Header != null)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Header);
            // Render RenderFragment
            writer.RenderEndTag();
        }
        
        if (Content != null)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            // Render RenderFragment
            writer.RenderEndTag();
        }
        
        writer.RenderEndTag();
    }
}
```

**Usage Pattern Migration:**

**Before (Web Forms template):**
```html
<asp:SectionPanel runat="server">
    <HeaderTemplate>
        <h2>Announcements</h2>
    </HeaderTemplate>
    <ContentTemplate>
        <asp:Repeater ID="Announcements" runat="server">
            <ItemTemplate>
                <div><%# Eval("Title") %></div>
            </ItemTemplate>
        </asp:Repeater>
    </ContentTemplate>
</asp:SectionPanel>
```

**After (Blazor RenderFragment):**
```razor
<SectionPanel>
    <Header>
        <h2>Announcements</h2>
    </Header>
    <Content>
        @foreach (var ann in announcements)
        {
            <div>@ann.Title</div>
        }
    </Content>
</SectionPanel>
```

**Impact:** Enables migration of complex templated controls (dashboards, wizard steps, accordion panels with custom item layouts).

---

## Summary: P1–P5 Priority and Dependencies

| Priority | Feature | Impact | Dependencies |
|----------|---------|--------|--------------|
| **P1** | `DataBoundWebControl<T>` | High — bridges data binding + HtmlTextWriter | None; standalone |
| **P2** | `TagKey` + `AddAttributesToRender` | High — simplifies 80% of control migrations | Moderate refactor of `WebControl` |
| **P3** | HtmlTextWriter enum expansion | Medium — enables modern markup patterns | Low; additive to existing enums |
| **P4** | `CompositeControl` mixed children | Medium — unlocks complex control migration | Moderate; requires child type detection |
| **P5** | `ITemplate` → `RenderFragment` bridge | Low — legacy pattern; most new controls use RenderFragment | Low; guidance + optional helper class |

**Recommended Implementation Order:** P2 → P1 → P3 → P4 → P5

(P2 first because it unblocks the most migrations with the least effort.)

---

## See Also

- [Custom Controls Migration Guide](Custom-Controls.md) — Full migration patterns using current BWFC classes
- [User Controls Migration Guide](User-Controls.md) — ASCX → Razor component conversion
- [Deferred Controls](DeferredControls.md) — Controls with no Blazor equivalent

---

## References

- [Blazor Component Base Classes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/)
- [Web Forms WebControl](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.webcontrol)
- [Web Forms CompositeControl](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.compositecontrol)
- [Web Forms ITemplate](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.itemplate)
