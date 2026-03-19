
============================================================
  BWFC Migration Tool - Layer 1: Mechanical Transforms
============================================================
  Source:  D:\BlazorWebFormsComponents\samples\WingtipToys\WingtipToys
  Output:  samples/MigrationRun2
  Project: WingtipToys

Created output directory: samples/MigrationRun2
Generating project scaffold...

Discovering Web Forms files...
Found 32 Web Forms file(s) to transform.

Applying transforms...

Copying 79 static file(s)...

============================================================
  Migration Summary
============================================================
  Files processed:       32
  Transforms applied:    277
  Static files copied:   79
  Items needing review:  18

--- Items Needing Manual Attention ---
  [CodeBlock] (14 item(s)):
     ProductDetails.aspx: Unconverted code block: <%#: String.Format("{0:c}", Item.UnitPrice) %>
     ProductList.aspx: Unconverted code block: <%#: GetRouteUrl("ProductByNameRoute", new {productName = Item.ProductName}) %>
     ProductList.aspx: Unconverted code block: <%#: GetRouteUrl("ProductByNameRoute", new {productName = Item.ProductName}) %>
     ProductList.aspx: Unconverted code block: <%#:String.Format("{0:c}", Item.UnitPrice)%>
     ShoppingCart.aspx: Unconverted code block: <%#: String.Format("{0:c}", ((Convert.ToDouble(Item.Quantity)) *  Convert.ToDoub
     Site.Master: Unconverted code block: <%#: GetRouteUrl("ProductsByCategoryRoute", new {categoryName = Item.CategoryNam
     Account\Manage.aspx: Unconverted code block: <% } %>
     Account\Manage.aspx: Unconverted code block: <% } %>
     Account\ManageLogins.aspx: Unconverted code block: <%# "Remove this " + Item.LoginProvider + " login from your account" %>
     Account\ManageLogins.aspx: Unconverted code block: <%# CanRemoveExternalLogins %>
     Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %>
     Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %>
     Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %>
     Checkout\CheckoutReview.aspx: Unconverted code block: <%#: Eval("Total", "{0:C}") %>
  [Register] (4 item(s)):
     Site.Mobile.Master: Removed Register directive - verify component tag prefixes: <%@ Register Src="~/ViewSwitcher.ascx" TagPrefix="friendlyUrls" TagName="ViewSwitcher" %>
     Account\Login.aspx: Removed Register directive - verify component tag prefixes: <%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>
     Account\Manage.aspx: Removed Register directive - verify component tag prefixes: <%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>
     Account\ManageLogins.aspx: Removed Register directive - verify component tag prefixes: <%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

Migration complete. Next steps:
  1. Review items flagged above for manual attention
  2. Use the BWFC Copilot skill for code-behind transforms (Layer 2)
  3. Build and test: dotnet build && dotnet run


