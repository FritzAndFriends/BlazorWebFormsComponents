# Run 5 Migration Script Output

## Actual Run Summary (non-WhatIf)

`
Script duration: 3.25 seconds
Files processed: 32
Transforms applied: 309
Static files copied: 79
Items needing review: 30

--- Items Needing Manual Attention ---
  [CodeBlock] (14 items):
    ProductList.aspx: Unconverted code block: GetRouteUrl calls (2)
    ShoppingCart.aspx: Unconverted code block: String.Format with Item
    Site.Master: Unconverted code block: GetRouteUrl call
    Account/Manage.aspx: Unconverted code blocks (2)
    Account/ManageLogins.aspx: Unconverted code blocks (2)
    Checkout/CheckoutReview.aspx: Unconverted code blocks (6 - context.FirstName etc)
  [ContentPlaceHolder] (1 item):
    Site.Mobile.Master: Non-MainContent ContentPlaceHolder 'FeaturedContent'
  [GetRouteUrl] (2 items):
    ProductList.aspx: Add @inject GetRouteUrlHelper
    Site.Master: Add @inject GetRouteUrlHelper
  [RegisterDirective] (4 items):
    Site.Mobile.Master, Account/Login, Account/Manage, Account/ManageLogins
  [SelectMethod] (9 items):
    ProductDetails, ProductList, ShoppingCart, Site.Master,
    Account/ManageLogins, Account/OpenAuthProviders,
    Admin/AdminPage (2)
`

## New Enhancement Coverage

| Enhancement | Fired? | Details |
|-------------|--------|---------|
| LoginView  AuthorizeView | ✅ | Site.Master LoginView converted |
| GetRouteUrl flagging |  | 2 files flagged with inject hint |
| SelectMethod  TODO |  | 9 instances annotated |
| Bare Item  context |  | Item references in templates converted |
| Register cleanup |  | 4 Register directives removed |
| uc: prefix | N/A | No uc: prefixes in WingtipToys source |

## WhatIf Dry-Run Output
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:377 char:101
+ ... h -Transform 'Content' -Detail 'HeadContent placeholder â+' <HeadCont ...
+                                                                 ~
The '<' operator is reserved for future use.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:507 char:37
+     $mainCphSelfRegex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID ...
+                                     ~
The '<' operator is reserved for future use.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:518 char:118
+ ...  TODO: ContentPlaceHolder '$($m.Groups[1].Value)' â?" convert to a se ...
+                                                           ~~~~~~~
Unexpected token 'convert' in expression or statement.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:520 char:81
+ ... gex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"([^"]+)"[^ ...
+                                                                  ~
Missing type name after '['.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:528 char:93
+ ... RelPath -Category 'LoginView-RoleGroups' -Detail 'LoginView <RoleGrou ...
+                                                                 ~
The '<' operator is reserved for future use.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:560 char:60
+     $evalFmtRegex = [regex]'<%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\) ...
+                                                            ~
Missing type name after '['.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:560 char:61
+     $evalFmtRegex = [regex]'<%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\) ...
+                                                             ~
Missing closing ')' in expression.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:560 char:67
+ ... evalFmtRegex = [regex]'<%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\)\s*%> ...
+                                                                  ~
Missing ')' in method call.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:560 char:67
+ ... valFmtRegex = [regex]'<%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\)\s*%>'
+                                                                 ~~~~~~~~~
Unexpected token '"\)\s*%>'
    $evalFmtMatches = $evalFmtRegex.Matches($Content)
    if ($evalFmtMatches.Count -gt 0) {
        $Content = $evalFmtRegex.Replace($Content, '@context.$1.ToString("' in expression or statement.
At D:\BlazorWebFormsComponents\scripts\bwfc-migrate.ps1:563 char:75
+ ... ntent = $evalFmtRegex.Replace($Content, '@context.$1.ToString("$2")')
+                                                                    ~~~~~~
Unexpected token '$2")')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted' in expression or statement.
Not all parse errors were reported.  Correct the reported errors and try again.
    + CategoryInfo          : ParserError: (:) [], ParseException
    + FullyQualifiedErrorId : RedirectionNotSupported
 
