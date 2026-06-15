---
name: bwfc-custom-control-migration
description: "Migrate custom ASP.NET Web Forms server controls (WebControl, CompositeControl) to Blazor using BlazorWebFormsComponents. Covers RenderContents/HtmlTextWriter preservation, TagKey mapping, AddAttributesToRender, CreateChildControls, and the one-line-change migration pattern. WHEN: 'migrate custom control', 'webcontrol to blazor', 'rendercontents migration', 'htmltextwriter blazor', 'custom server control'. FOR SINGLE OPERATIONS: use /bwfc-ascx-migration for .ascx user controls, /bwfc-migration for full page migration."
---

# Custom WebControl → Blazor Migration

## Overview

ASP.NET Web Forms custom server controls inherit from `System.Web.UI.WebControls.WebControl` and render HTML imperatively via `HtmlTextWriter`. BlazorWebFormsComponents provides a **drop-in replacement** that lets this code work unchanged in Blazor.

**The key principle: change one `using` statement, keep your code.**

```csharp
// Before (Web Forms)
using System.Web.UI.WebControls;

// After (Blazor + BWFC)
using BlazorWebFormsComponents.CustomControls;
```

## How It Works

The BWFC `WebControl` class overrides Blazor's `BuildRenderTree`:

1. Creates an `HtmlTextWriter` instance (backed by `StringBuilder`)
2. Calls `AddAttributesToRender(writer)` — emits ID, class, style, tooltip, disabled
3. Calls `Render(writer)` → `RenderBeginTag` → `RenderContents` → `RenderEndTag`
4. Captures the HTML string via `writer.GetHtml()`
5. Emits it via `builder.AddMarkupContent(0, html)`

Your imperative rendering code produces the **exact same HTML** it did in Web Forms.

## Supported Patterns

### Pattern 1: TagKey + RenderContents (Most Common)

The classic Web Forms pattern — override `TagKey` for the outer element, `RenderContents` for the inner HTML:

```csharp
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class StatusBadge : WebControl
{
    [Parameter]  // Only addition for Blazor
    public string Status { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Strong);
        writer.Write(Status);
        writer.RenderEndTag();
    }
}
```

### Pattern 2: Full Render Override

For controls that need complete control over output:

```csharp
public class CustomButton : WebControl
{
    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public string ButtonType { get; set; } = "button";

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Type, ButtonType);
        writer.RenderBeginTag(HtmlTextWriterTag.Button);
        writer.Write(Text);
        writer.RenderEndTag();
    }
}
```

### Pattern 3: AddAttributesToRender (Custom Attributes)

Add data attributes, ARIA attributes, or other custom attributes to the outer tag:

```csharp
public class DataPanel : WebControl
{
    [Parameter]
    public string DataId { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
        base.AddAttributesToRender(writer);  // ID, CssClass, Style, ToolTip, Enabled
        writer.AddAttribute("data-panel-id", DataId);
        writer.AddAttribute("role", "region");
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.Write("Panel content here");
    }
}
```

### Pattern 4: CompositeControl (Child Controls)

For controls that compose multiple child controls:

```csharp
public class SearchBox : CompositeControl
{
    [Parameter]
    public string Placeholder { get; set; } = "Search...";

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-container");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);

        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
        writer.AddAttribute(HtmlTextWriterAttribute.Placeholder, Placeholder);
        writer.RenderBeginTag(HtmlTextWriterTag.Input);
        writer.RenderEndTag();

        writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
        writer.RenderBeginTag(HtmlTextWriterTag.Button);
        writer.Write("Go");
        writer.RenderEndTag();

        writer.RenderEndTag(); // div
    }
}
```

## Migration Steps

### Step 1: Change the Using

```diff
- using System.Web.UI;
- using System.Web.UI.WebControls;
+ using BlazorWebFormsComponents.CustomControls;
+ using Microsoft.AspNetCore.Components;
```

### Step 2: Add [Parameter] to Public Properties

```diff
+ [Parameter]
  public string Title { get; set; }

+ [Parameter]
  public bool ShowHeader { get; set; } = true;
```

### Step 3: Remove Web Forms-Only Members

Remove or stub these if present (they have no Blazor equivalent):

| Member | Action |
|--------|--------|
| `ViewState` property bags | Replace with private fields |
| `CreateChildControls()` | Move logic to `Render` or `RenderContents` |
| `EnsureChildControls()` | Remove (no lazy initialization needed) |
| `INamingContainer` | Remove (Blazor handles scoping differently) |
| `IPostBackDataHandler` | Remove (use Blazor events instead) |
| `IPostBackEventHandler` | Remove (use `EventCallback`) |

### Step 4: Verify Rendering

The control should render identical HTML. Test with bUnit:

```razor
@inherits BlazorWebFormsTestContext

@code {
    [Fact]
    public void MyControl_RendersExpectedHtml()
    {
        var cut = Render(@<MyControl Title="Hello" />);
        cut.Markup.ShouldContain("<div");
        cut.Markup.ShouldContain("Hello");
    }
}
```

## Available Base Classes

| BWFC Class | Inherits | Use When |
|-----------|----------|----------|
| `WebControl` | `BaseStyledComponent` | Controls with `RenderContents`/`HtmlTextWriter` rendering |
| `CompositeControl` | `WebControl` | Controls that compose multiple child elements |
| `UserControl` | `BaseStyledComponent` | ASCX code-behind classes (markup-driven) |
| `Control` | `BaseWebFormsComponent` | Bare controls with no styling |

## HtmlTextWriter API Reference

The BWFC `HtmlTextWriter` shim supports:

| Method | Description |
|--------|-------------|
| `Write(string)` | Write raw text |
| `WriteLine(string)` | Write text + newline |
| `RenderBeginTag(HtmlTextWriterTag)` | Open an HTML element |
| `RenderBeginTag(string)` | Open an HTML element by name |
| `RenderEndTag()` | Close the current element |
| `AddAttribute(string, string)` | Add attribute to next tag |
| `AddAttribute(HtmlTextWriterAttribute, string)` | Add attribute by enum |
| `AddStyleAttribute(string, string)` | Add inline style to next tag |
| `AddStyleAttribute(HtmlTextWriterStyle, string)` | Add style by enum |

All standard `HtmlTextWriterTag`, `HtmlTextWriterAttribute`, and `HtmlTextWriterStyle` enum values are supported.

## Inherited Properties (from BaseStyledComponent)

These work automatically on all `WebControl`-derived components:

| Property | Renders As |
|----------|-----------|
| `CssClass` | `class="..."` |
| `ID` | `id="..."` |
| `ToolTip` | `title="..."` |
| `Enabled="false"` | `disabled="disabled"` |
| `Visible="false"` | No output |
| `Style` | `style="..."` |

## What Does NOT Work

| Feature | Reason | Alternative |
|---------|--------|-------------|
| `Page.Controls.Add(ctrl)` | No dynamic control tree in Blazor | Use `RenderFragment` or `DynamicComponent` |
| `ViewState["key"]` | No ViewState persistence | Use private fields or cascading parameters |
| `PostBack events` | No postback in Blazor | Use `EventCallback` or Blazor events |
| `Designer support` | No designer in Blazor | N/A |
| `Async in RenderContents` | Runs synchronously in `BuildRenderTree` | Fetch data in lifecycle, render from fields |

## CLI Automation

The `webforms-to-blazor` CLI automatically:

1. Detects `.cs` files inheriting `System.Web.UI.WebControls.WebControl`
2. Strips the `System.Web.UI` namespace prefix → bare `WebControl`
3. Adds `using BlazorWebFormsComponents.CustomControls;`
4. Injects `@inherits WebControl` into paired `.razor` files
5. Adds `[Parameter]` to public properties

## Checklist

For each custom WebControl being migrated:

- [ ] Using changed from `System.Web.UI.WebControls` → `BlazorWebFormsComponents.CustomControls`
- [ ] `using Microsoft.AspNetCore.Components;` added
- [ ] Public properties have `[Parameter]` attribute
- [ ] `ViewState` usage replaced with private fields
- [ ] `CreateChildControls()` logic moved to `Render`/`RenderContents`
- [ ] `IPostBackDataHandler`/`IPostBackEventHandler` removed
- [ ] Control renders expected HTML (verified with bUnit test)
- [ ] Control usable from `.razor` pages: `<MyControl Property="value" />`

## Reference

- [Custom WebControl Migration Guide](../../docs/Migration/CustomWebControl.md)
- [WebControl source](../../src/BlazorWebFormsComponents/CustomControls/WebControl.cs)
- [HtmlTextWriter source](../../src/BlazorWebFormsComponents/CustomControls/HtmlTextWriter.cs)
- [CompositeControl source](../../src/BlazorWebFormsComponents/CustomControls/CompositeControl.cs)
- [Sample page](../../samples/AfterBlazorServerSide/Components/Pages/ControlSamples/Migration/CustomWebControlDemo.razor)
