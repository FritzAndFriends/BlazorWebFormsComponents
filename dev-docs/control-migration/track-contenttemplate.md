# Track: ContentTemplate Unwrapper Transform

**Priority:** P1 (Transforms)  
**Owner:** Bishop (CLI Development)  
**Status:** Planned  
**Validation:** Layer 1 build + Layer 1→L2 repair reduction

## Problem Statement

ASCX files often use `<ContentTemplate>` or similar templated markup blocks to allow parent pages to customize content. Currently, these templates are left unchanged in Layer 1 output, requiring manual conversion to Blazor `RenderFragment` parameters.

**Example from real ASCX:**
```razor
<%@ Control Language="C#" CodeBehind="RepeaterTemplate.ascx.cs" Inherits="WingtipToys.RepeaterTemplate" %>

<asp:Repeater ID="rptItems" runat="server">
    <HeaderTemplate>
        <ul class="item-list">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <strong><%# Eval("ProductName") %></strong>
            <p>Price: <%# Eval("Price", "{0:C}") %></p>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
```

When migrated without unwrapping, this stays as-is in the `.razor` file, breaking because:
1. `<asp:Repeater>` becomes `<Repeater>` ✓
2. But `<HeaderTemplate>` remains as HTML, not recognized as a Razor template parameter

The unwrapper must convert this to a Blazor-compatible format:

```razor
<Repeater ID="rptItems" Items="Items">
    <HeaderTemplate>
        <ul class="item-list">
    </HeaderTemplate>
    <ItemTemplate Context="item">
        <li>
            <strong>@item.ProductName</strong>
            <p>Price: @item.Price.ToString("C")</p>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</Repeater>
```

## Scope

Transform ASCX template markup inside data-bound controls:

1. **Detect templated control blocks** (`<asp:Repeater>`, `<asp:DataList>`, `<asp:GridView>`, etc.)
2. **Extract template children** (`<HeaderTemplate>`, `<ItemTemplate>`, etc.)
3. **Rewrite template expressions** (`<%# Eval(...) %>` → `@item.Property`)
4. **Add `Context=` parameter** to item template when needed
5. **Preserve non-template markup** (labels, inputs)

## Deliverables

### 1. Create `ContentTemplateUnwrapperTransform`

**Location:** `src/BlazorWebFormsComponents.Cli/Transforms/Markup/`

```csharp
public class ContentTemplateUnwrapperTransform : IMarkupTransform
{
    private readonly AscxDescriptorAnalyzer _descriptorAnalyzer;
    
    public string Name => "ContentTemplateUnwrapper";
    
    public string Transform(string markup, FileMetadata metadata)
    {
        // 1. Find all <asp:Repeater>, <asp:DataList>, etc. with templates
        // 2. Rewrite <%# Eval(...) %> expressions
        // 3. Add Context parameter to ItemTemplate
        // 4. Convert to Blazor equivalent
        
        return transformedMarkup;
    }
}
```

### 2. Template expression rewriter

Convert Web Forms data-binding syntax to Blazor:

- `<%# Eval("PropertyName") %>` → `@item.PropertyName`
- `<%# Eval("Price", "{0:C}") %>` → `@item.Price.ToString("C")`
- `<%# GetValue(Container.DataItem) %>` → `@GetValue(item)`
- `<%# XPath("//node/@attr") %>` → (unsupported, emit warning)

### 3. Integrate into `MigrationPipeline`

Register transform in `Program.cs`:

```csharp
services.AddSingleton<IMarkupTransform, ContentTemplateUnwrapperTransform>();
```

Register in test pipeline (`TestHelpers.cs`):

```csharp
var markupTransforms = new List<IMarkupTransform>
{
    // ... existing transforms ...
    new ContentTemplateUnwrapperTransform(descriptorAnalyzer),
};
```

### 4. Create unit tests

**Location:** `tests/BlazorWebFormsComponents.Cli.Tests/Transforms/Markup/ContentTemplateUnwrapperTransformTests.cs`

Test cases:

- ✓ Unwrap `<HeaderTemplate>` / `<ItemTemplate>` / `<FooterTemplate>`
- ✓ Rewrite `<%# Eval(...) %>` expressions
- ✓ Rewrite `<%# GetValue(...) %>` method calls
- ✓ Add `Context="item"` parameter when missing
- ✓ Preserve nested HTML markup
- ✓ Handle Repeater, DataList, GridView templates
- ✓ Skip non-template controls (no-op)
- ✓ Handle escaped/nested template syntax
- ✓ Emit warning for unsupported XPath expressions

### 5. Integration test

Test with real ASCX control:

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- \
  migrate -i samples/WingtipToys -o /tmp/test-output
```

Verify `Repeater`/`DataList` in output `.razor` files use proper template syntax.

## Validation Criteria

✅ Template markup is unwrapped and converted to Blazor equivalents  
✅ Data-binding expressions (`<%# ... %>`) are rewritten to `@item.Property`  
✅ `Context=` parameter is added to item templates  
✅ Non-template markup is preserved  
✅ Warnings are emitted for unsupported patterns (XPath, custom formatters)  
✅ Unit tests pass: `dotnet test tests/BlazorWebFormsComponents.Cli.Tests --filter "ContentTemplate"`  
✅ Integration test passes on WingtipToys/ContosoUniversity with reduced Layer 2 repair time

## Dependencies

- Requires `AscxDescriptorAnalyzer` (P0) to understand template item types
- Requires Web Forms control knowledge (Repeater, DataList, GridView)

## Unblocks

- FindControl transform (P1) — parent of FindControl rewrites depends on this
- ASCX binding/lifecycle transforms (P2) — binding patterns flow through templates

## Not in Scope

- Custom template types (only standard Repeater/DataList/GridView)
- XPath template expressions (emit warning, skip)
- Template event handlers inside ItemTemplate (`OnItemCommand`, etc. — handled by separate transform)
- AJAX toolkit templated controls (P2 extension)

## Edge Cases

| Case | Handling | Status |
|------|----------|--------|
| Missing Context parameter | Infer from control descriptor; add Context="item" | OK |
| Nested Eval calls | `Eval(Eval(...))` unsupported; warn and skip | Warn |
| Container.DataItem | Convert to context parameter (e.g., `item` in DataList) | OK |
| No templates (static content) | No transformation needed | OK |
| Self-referential binding | `<%# Eval("ParentID", "Parent-{0}") %>` → handle as format string | OK |
| XPath expressions | Not supported in Blazor; emit warning | Warn |
