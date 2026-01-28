# HTML Output Matching Guide

This guide explains how to ensure Blazor components render identical HTML to their ASP.NET Web Forms counterparts.

## Why HTML Output Matching Matters

1. **CSS Compatibility** - Existing stylesheets targeting Web Forms HTML structure continue to work
2. **JavaScript Compatibility** - Client-side scripts that query or manipulate DOM elements remain functional
3. **Visual Consistency** - Users see no visual difference after migration
4. **Test Verification** - Enables automated comparison between Web Forms and Blazor output

## Reference Sources for Web Forms HTML Output

### Primary Reference: .NET Framework 4.8 Documentation

The official documentation provides control rendering details:

- **Base URL**: `https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols?view=netframework-4.8`
- **Control-specific**: `https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.{controlname}?view=netframework-4.8`

Example for Button:
```
https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.button?view=netframework-4.8
```

### Secondary Reference: .NET Framework Reference Source

The actual rendering code is available at:
- **GitHub**: `https://github.com/microsoft/referencesource`
- **Path**: `referencesource/System.Web/UI/WebControls/{ControlName}.cs`

Look for these methods in the source:
- `Render()` - Main rendering entry point
- `RenderContents()` - Inner content rendering
- `AddAttributesToRender()` - HTML attributes added to the tag
- `RenderBeginTag()` / `RenderEndTag()` - Tag structure

### Practical Approach: Create a Web Forms Test Page

The most reliable method is to create a test page in the `samples/BeforeWebForms` project:

```aspx
<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head><title>HTML Output Test</title></head>
<body>
    <form id="form1" runat="server">
        <!-- Test your control here -->
        <asp:Button ID="btn1" Text="Click Me" CssClass="my-class" runat="server" />
    </form>
</body>
</html>
```

Run the page and view source to capture the exact HTML output.

## Common HTML Patterns by Control Type

### Button Controls

**Web Forms Output:**
```html
<input type="submit" name="btn1" value="Click Me" id="btn1" class="my-class" />
```

**Key Observations:**
- Uses `<input type="submit">` NOT `<button>` element
- `Text` property becomes `value` attribute
- `ID` becomes both `name` and `id` attributes
- `CssClass` becomes `class` attribute

### Label Controls

**Web Forms Output:**
```html
<span id="lbl1" class="my-class">Label Text</span>
```

**Key Observations:**
- Renders as `<span>` element
- `Text` property becomes inner content
- `AssociatedControlID` adds `for` attribute (changes to `<label>` tag)

### HyperLink Controls

**Web Forms Output:**
```html
<a id="link1" class="my-class" href="https://example.com" target="_blank">Link Text</a>
```

### Panel Controls

**Web Forms Output:**
```html
<div id="panel1" class="my-class">
    <!-- Child content -->
</div>
```

### Data Controls (GridView, DataList, Repeater)

**GridView Output:**
```html
<table id="grid1" class="my-class" cellspacing="0" rules="all" border="1">
    <tr>
        <th scope="col">Header1</th>
        <th scope="col">Header2</th>
    </tr>
    <tr>
        <td>Data1</td>
        <td>Data2</td>
    </tr>
</table>
```

**DataList Output:**
```html
<table id="datalist1" cellspacing="0" border="0">
    <tr>
        <td><!-- Item content --></td>
    </tr>
</table>
```

**Repeater Output:**
- No wrapper element - renders only the templates
- Most flexible for custom HTML

### Validation Controls

**Web Forms Output:**
```html
<span id="val1" class="my-class" style="color:Red;visibility:hidden;">Error message</span>
```

**Key Observations:**
- Renders as `<span>` element
- `ForeColor="Red"` becomes `style="color:Red;"`
- Initially hidden with `visibility:hidden` or `display:none`
- `ErrorMessage` becomes inner content when invalid

## Attribute Mapping Reference

| Web Forms Property | HTML Attribute | Notes |
|-------------------|----------------|-------|
| `ID` | `id`, `name` | Both attributes set |
| `CssClass` | `class` | Direct mapping |
| `ToolTip` | `title` | Direct mapping |
| `Enabled="false"` | `disabled="disabled"` | Boolean attribute |
| `TabIndex` | `tabindex` | Direct mapping |
| `AccessKey` | `accesskey` | Direct mapping |
| `BackColor` | `style="background-color:X"` | Inline style |
| `ForeColor` | `style="color:X"` | Inline style |
| `BorderColor` | `style="border-color:X"` | Inline style |
| `BorderWidth` | `style="border-width:X"` | Inline style |
| `BorderStyle` | `style="border-style:X"` | Inline style |
| `Height` | `style="height:X"` | Inline style |
| `Width` | `style="width:X"` | Can be attribute or style |
| `Font-Bold` | `style="font-weight:bold"` | Inline style |
| `Font-Size` | `style="font-size:X"` | Inline style |
| `Visible="false"` | (not rendered) | Element not in DOM |

## Style Building Pattern

Use the existing `ToStyle()` extension method pattern:

```csharp
// In component .razor file
<button style="@this.ToStyle().Build().NullIfEmpty()">

// ToStyle() builds from IStyle properties:
// - BackColor → background-color
// - ForeColor → color
// - Font properties → font-* styles
// - Height/Width → height/width
// - Border properties → border-* styles
```

## Testing HTML Output

### bUnit Markup Comparison

```razor
@inherits BunitContext

@code {
    [Fact]
    public void Button_WithCssClass_RendersCorrectHtml()
    {
        var cut = Render(@<Button Text="Click" CssClass="btn-primary" />);

        // Exact match
        cut.MarkupMatches(@<input type="submit" value="Click" class="btn-primary" />);
    }

    [Fact]
    public void Button_WithStyles_RendersInlineStyle()
    {
        var cut = Render(@<Button Text="Click" BackColor="Red" ForeColor="White" />);

        var button = cut.Find("input");
        button.GetAttribute("style").ShouldContain("background-color");
        button.GetAttribute("style").ShouldContain("color");
    }
}
```

### Comparison Testing Strategy

1. Create identical markup in `samples/BeforeWebForms`
2. Capture rendered HTML output
3. Create equivalent Blazor component usage in test
4. Use `MarkupMatches()` or attribute assertions to verify

## Edge Cases and Considerations

### ClientID vs ID

Web Forms generates `ClientID` which may differ from `ID` in nested controls:
```html
<!-- Web Forms with naming container -->
<span id="container1_label1">Text</span>
```

In Blazor, we typically use the simpler `@ref` pattern and don't replicate naming containers.

### ViewState Hidden Field

Web Forms pages include `__VIEWSTATE` hidden field - this is NOT replicated as Blazor has different state management.

### Event Validation

Web Forms includes `__EVENTVALIDATION` hidden field - NOT replicated in Blazor.

### AutoPostBack

Controls with `AutoPostBack="true"` add JavaScript `onclick`/`onchange` handlers to trigger postback. In Blazor, use proper event binding instead:

```razor
<!-- Blazor equivalent of AutoPostBack -->
<DropDownList @bind-SelectedValue="selectedValue" OnSelectedIndexChanged="HandleChange" />
```

## Checklist for New Components

- [ ] Identify the HTML element type (span, div, input, table, etc.)
- [ ] Map all relevant properties to HTML attributes
- [ ] Implement style properties using `ToStyle().Build()`
- [ ] Handle `Visible="false"` by not rendering
- [ ] Handle `Enabled="false"` with `disabled` attribute
- [ ] Write bUnit tests comparing expected HTML
- [ ] Test with existing CSS from Web Forms project
- [ ] Document any intentional deviations from Web Forms output
