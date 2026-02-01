# WebForms to Blazor CLI Tool - Conversion Examples

This document shows examples of how the `webforms-to-blazor` CLI tool converts ASP.NET Web Forms user controls to Blazor Razor components.

## Example 1: Simple View Switcher

**Before (ViewSwitcher.ascx):**
```aspx
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSwitcher.ascx.cs" Inherits="BeforeWebForms.ViewSwitcher" %>
<div id="viewSwitcher">
    <%: CurrentView %> view | <a href="<%: SwitchUrl %>" data-ajax="false">Switch to <%: AlternateView %></a>
</div>
```

**After (ViewSwitcher.razor):**
```razor
@inherits BeforeWebForms.ViewSwitcher

<div id="viewSwitcher">
    @(CurrentView) view | <a href="@(SwitchUrl)" data-ajax="false">Switch to @(AlternateView)</a>
</div>
```

**Conversions Made:**
- `<%@ Control ... Inherits="..." %>` → `@inherits ...`
- `<%: expression %>` → `@(expression)`

## Example 2: Product Card with Data Binding

**Before (ProductCard.ascx):**
```aspx
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCard.ascx.cs" Inherits="MyApp.Controls.ProductCard" %>
<div class="product-card">
    <asp:Image ID="imgProduct" ImageUrl='<%# Item.ImageUrl %>' AlternateText='<%# Item.Name %>' runat="server" />
    <div class="product-info">
        <h3><asp:Literal ID="litName" Text='<%# Item.Name %>' runat="server" /></h3>
        <p class="price">$<%: Item.Price %></p>
        <asp:Button ID="btnAddToCart" Text="Add to Cart" OnClick="AddToCart_Click" CssClass="btn-primary" runat="server" />
    </div>
</div>
```

**After (ProductCard.razor):**
```razor
@inherits MyApp.Controls.ProductCard

<div class="product-card">
    <Image ID="imgProduct" ImageUrl='@(context.ImageUrl)' AlternateText='@(context.Name)' />
    <div class="product-info">
        <h3><Literal ID="litName" Text='@(context.Name)' /></h3>
        <p class="price">$@(context.Price)</p>
        <Button ID="btnAddToCart" Text="Add to Cart" OnClick="AddToCart_Click" CssClass="btn-primary" />
    </div>
</div>
```

**Conversions Made:**
- `<asp:Image ... />` → `<Image ... />`
- `<asp:Literal ... />` → `<Literal ... />`
- `<asp:Button ... />` → `<Button ... />`
- `runat="server"` removed
- `<%# Item.Property %>` → `@(context.Property)` (fixes Item vs context issue)
- `<%: expression %>` → `@(expression)`

## Usage

To convert these files, use:

```bash
# Single file
webforms-to-blazor --input ProductCard.ascx

# Directory (all .ascx files)
webforms-to-blazor --input ./Controls --recursive --overwrite
```

## What Still Needs Manual Work

After conversion, you'll typically need to:

1. **Update event handlers** to use Blazor's event system
2. **Convert data binding** to use `@bind` syntax where appropriate
3. **Add @using directives** for namespaces
4. **Migrate code-behind** logic to `@code` blocks or separate `.razor.cs` files
5. **Replace ViewState** with component state or parameters
6. **Update postback logic** to use Blazor's component lifecycle

See the [main README](README.md) for full documentation.
