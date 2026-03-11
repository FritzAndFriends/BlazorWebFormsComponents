# bwfc-migrate.ps1 Output — Run 3

**Duration:** 2.3 seconds

```
============================================================
  BWFC Migration Tool - Layer 1: Mechanical Transforms
============================================================
  Source:  samples\WingtipToys\WingtipToys\
  Output:  samples\MigrationRun3\
  Project: WingtipToys

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
     ProductDetails.aspx: Unconverted code block: <%#: String.Format("{0:c}", Item.UnitPrice) %>
     ProductList.aspx: Unconverted code block: <%#: GetRouteUrl("ProductByNameRoute", ...) %>
     ProductList.aspx: Unconverted code block: <%#: GetRouteUrl("ProductByNameRoute", ...) %>
     ProductList.aspx: Unconverted code block: <%#:String.Format("{0:c}", Item.UnitPrice)%>
     ShoppingCart.aspx: Unconverted code block: <%#: String.Format("{0:c}", ...) %>
     Site.Master: Unconverted code block: <%#: GetRouteUrl("ProductsByCategoryRoute", ...) %>
     Account\Manage.aspx: Unconverted code block: <% } %>
     Account\Manage.aspx: Unconverted code block: <% } %>
     Account\ManageLogins.aspx: Unconverted code block: <%# "Remove this " + Item.LoginProvider + ... %>
     Account\ManageLogins.aspx: Unconverted code block: <%# CanRemoveExternalLogins %>
     Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %> (x3)
     Checkout\CheckoutReview.aspx: Unconverted code block: <%#: Eval("Total", "{0:C}") %>
  [Register] (4 item(s)):
     Site.Mobile.Master, Account\Login.aspx, Account\Manage.aspx, Account\ManageLogins.aspx

Migration complete. Next steps:
  1. Review items flagged above for manual attention
  2. Use the BWFC Copilot skill for code-behind transforms (Layer 2)
  3. Build and test: dotnet build && dotnet run
```
