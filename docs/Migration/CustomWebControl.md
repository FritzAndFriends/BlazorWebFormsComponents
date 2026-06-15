# Custom WebControl Migration

Original Microsoft documentation: [WebControl Class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.webcontrol?view=netframework-4.8)

## Overview

Many Web Forms applications contain custom server controls that inherit from `System.Web.UI.WebControls.WebControl` and render HTML using `HtmlTextWriter`. These controls use the **imperative rendering pattern**:

- Override `TagKey` to set the outer HTML element
- Override `AddAttributesToRender(HtmlTextWriter)` to add attributes
- Override `RenderContents(HtmlTextWriter)` to write inner HTML

BlazorWebFormsComponents provides a `WebControl` base class with an `HtmlTextWriter` shim that lets this code run **unchanged** in Blazor.

## Migration Steps

1. Change the `using` statement:

```csharp
// Before (Web Forms)
using System.Web.UI.WebControls;

// After (Blazor + BWFC)
using BlazorWebFormsComponents.CustomControls;
```

2. Add `[Parameter]` attributes to public properties (Blazor requirement)

3. That's it. Your `RenderContents`, `TagKey`, and `AddAttributesToRender` code works unchanged.

## Web Forms Pattern

```csharp
using System.Web.UI.WebControls;

public class StatusBadge : WebControl
{
    public string Status { get; set; }
    public string Label { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
        base.AddAttributesToRender(writer);
        writer.AddAttribute("data-status", Status?.ToLowerInvariant() ?? "unknown");
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Strong);
        writer.Write(Label ?? Status);
        writer.RenderEndTag();
    }
}
```

## Blazor Syntax

```csharp
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class StatusBadge : WebControl
{
    [Parameter]
    public string Status { get; set; }

    [Parameter]
    public string Label { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

    protected override void AddAttributesToRender(HtmlTextWriter writer)
    {
        base.AddAttributesToRender(writer);
        writer.AddAttribute("data-status", Status?.ToLowerInvariant() ?? "unknown");
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Strong);
        writer.Write(Label ?? Status);
        writer.RenderEndTag();
    }
}
```

Usage in a `.razor` page:

```razor
<StatusBadge Status="Active" Label="Online" CssClass="badge bg-success" />
```

## Features Supported in Blazor

- `TagKey` — outer HTML element selection
- `RenderContents(HtmlTextWriter)` — imperative inner content rendering
- `AddAttributesToRender(HtmlTextWriter)` — custom attribute injection
- `Render(HtmlTextWriter)` — full rendering override
- `RenderBeginTag` / `RenderEndTag` — outer tag customization
- `CssClass`, `ID`, `ToolTip`, `Enabled`, `Visible` — inherited from `BaseStyledComponent`
- `HtmlTextWriter` methods: `Write`, `WriteLine`, `RenderBeginTag`, `RenderEndTag`, `AddAttribute`, `AddStyleAttribute`
- `HtmlTextWriterTag` enum — all standard HTML elements
- `HtmlTextWriterAttribute` enum — all standard HTML attributes
- `HtmlTextWriterStyle` enum — all standard CSS properties

## Web Forms Features NOT Supported

- `ViewState` persistence between renders (use component state instead)
- `CreateChildControls()` (use `CompositeControl` base class or Razor templates)
- `INamingContainer` automatic ID generation (use explicit `ID` parameter)
- Server-side event postback from rendered HTML (use Blazor `@onclick` etc.)

## How It Works

The BWFC `WebControl` class overrides Blazor's `BuildRenderTree`:

1. Creates an `HtmlTextWriter` instance (backed by `StringBuilder`)
2. Calls `AddAttributesToRender(writer)` — adds ID, class, style, tooltip, disabled
3. Calls `Render(writer)` — which by default calls `RenderBeginTag` → `RenderContents` → `RenderEndTag`
4. Emits the captured HTML via `builder.AddMarkupContent(0, writer.GetHtml())`

This means your imperative rendering code produces the **same HTML output** it did in Web Forms.

## Examples

### Simple Label

```csharp
public class HelloLabel : WebControl
{
    [Parameter]
    public string Text { get; set; }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.Write(Text);
    }
}
```

Renders: `<span>Hello World</span>`

### Card Component with Nested Elements

```csharp
public class InfoCard : WebControl
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Body { get; set; }

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void RenderContents(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-header");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        writer.RenderBeginTag(HtmlTextWriterTag.H3);
        writer.Write(Title);
        writer.RenderEndTag(); // h3
        writer.RenderEndTag(); // header div

        writer.AddAttribute(HtmlTextWriterAttribute.Class, "card-body");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        writer.Write(Body);
        writer.RenderEndTag(); // body div
    }
}
```

### Button with Custom Attributes

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

## CLI Migration

The migration CLI automatically:

1. Converts `.ascx.cs` files with `WebControl` base classes → preserves the base class
2. Adds `using BlazorWebFormsComponents.CustomControls;`
3. Injects `@inherits WebControl` into the `.razor` file
4. Adds `[Parameter]` to public properties (via `ParameterAttributeTransform`)

No manual editing of rendering code is required.

## Usage Notes

!!! tip
    If your control uses `CreateChildControls()`, consider the `CompositeControl` base class instead, which supports child component composition.

!!! note
    The `HtmlTextWriter` shim runs synchronously during `BuildRenderTree`. Complex async operations should be performed in lifecycle methods (`OnInitializedAsync`, etc.) and stored in fields that `RenderContents` reads.

!!! warning
    Controls that depend on `Page.Request` or other page-level features should also inherit from `WebFormsPageBase` or inject the appropriate shims via DI.
