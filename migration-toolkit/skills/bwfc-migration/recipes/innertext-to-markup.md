# Recipe: InnerText/InnerHtml → Blazor Markup Binding

## Error Signature

```
CS1061: 'HtmlGenericControl' does not contain a definition for 'InnerText'
CS0103: The name 'ElementId' does not exist in the current context
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "\.InnerText\b|\.InnerHtml\b"
```

## Root Cause

Web Forms allows `runat="server"` on plain HTML elements (e.g., `<h2 id="Title" runat="server">`), creating `HtmlGenericControl` instances accessible in code-behind. Code sets `element.InnerText = "value"` to update content dynamically.

Blazor doesn't have server-side HTML element controls. Instead, use Razor expressions to bind content directly in markup.

## Fix Pattern

**Step 1 — In markup**, replace the server-element with a Razor expression:

```razor
@* BEFORE (Web Forms): *@
<h2 id="ShoppingCartTitle" runat="server">Shopping Cart</h2>

@* AFTER (Blazor): *@
<h2>@ShoppingCartTitle</h2>
```

**Step 2 — In code-behind**, replace `element.InnerText = "value"` with a string property:

```csharp
// BEFORE:
ShoppingCartTitle.InnerText = "Shopping Cart is Empty";

// AFTER:
private string ShoppingCartTitle { get; set; } = "Shopping Cart";
// Then in logic:
ShoppingCartTitle = "Shopping Cart is Empty";
```

## Common Variants

### InnerHtml (raw HTML)
```csharp
// BEFORE:
MessageDiv.InnerHtml = "<strong>Error:</strong> " + errorMsg;

// AFTER — use MarkupString for raw HTML:
private MarkupString MessageContent { get; set; }
MessageContent = new MarkupString($"<strong>Error:</strong> {errorMsg}");
```

```razor
@* In markup: *@
<div>@MessageContent</div>
```

### Multiple Elements on Same Page
When a page has several HTML server elements, convert each to a property:
```csharp
private string PageTitle { get; set; } = "Default Title";
private string StatusMessage { get; set; } = "";
private bool ShowDetails { get; set; } = true;
```

## Verification

After converting all `InnerText`/`InnerHtml` references to properties, the page should compile. The dynamic content updates through Blazor's standard rendering cycle instead of server-element manipulation.
