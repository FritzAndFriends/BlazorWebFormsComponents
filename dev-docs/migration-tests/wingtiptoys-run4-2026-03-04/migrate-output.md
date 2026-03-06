
============================================================
  BWFC Migration Tool — Layer 1: Mechanical Transforms
============================================================
  Source:  D:\BlazorWebFormsComponents\samples\WingtipToys\WingtipToys
  Output:  samples\MigrationRun4
  Project: WingtipToysCreated output directory: samples\MigrationRun4
Generating project scaffold...
VERBOSE: Performing the operation "Create project scaffold" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4".
VERBOSE:   [Scaffold] Generated WingtipToys.csproj
VERBOSE:   [Scaffold] Generated _Imports.razor
VERBOSE:   [Scaffold] Generated Program.cs
VERBOSE:   [Scaffold] Generated Properties/launchSettings.json
VERBOSE: Performing the operation "Create App.razor and Routes.razor scaffold" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Components".
VERBOSE:   [Scaffold] Generated Components/App.razor
VERBOSE:   [Scaffold] Generated Components/Routes.razorDiscovering Web Forms files...
Found 32 Web Forms file(s) to transform.

Applying transforms...
VERBOSE: Processing: About.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/About"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\About.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\About.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\About.razor.cs
VERBOSE: Processing: AddToCart.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/AddToCart"
VERBOSE:   [Form] Removed <form runat="server"> and </form>
VERBOSE:   [Attribute] Removed 1 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\AddToCart.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\AddToCart.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\AddToCart.razor.cs
VERBOSE: Processing: Contact.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Contact"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Contact.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Contact.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Contact.razor.cs
VERBOSE: Processing: Default.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Default.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Default.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Default.razor.c
cs
VERBOSE: Processing: ErrorPage.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ErrorPage"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 6 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 2 closing tag(s)
VERBOSE:   [Attribute] Removed 6 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\ErrorPage.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\ErrorPage.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ErrorPage.razor
r.cs
VERBOSE: Processing: ProductDetails.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ProductDetails"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 String.Format(Item.) to interpolated string
VERBOSE:   [Expression] Converted 5 Item binding(s) to @context
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 1 'runat' attribute(s)
VERBOSE:   [Attribute] Converted 1 ItemType to TItem
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\ProductDetails.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\ProductDetails.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ProductDetails.
.razor.cs
VERBOSE: Processing: ProductList.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ProductList"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 String.Format(Item.) to interpolated string
VERBOSE:   [Expression] Converted 3 Item binding(s) to @context
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 5 'runat' attribute(s)
VERBOSE:   [Attribute] Converted 1 ItemType to TItem
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\ProductList.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\ProductList.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ProductList.raz
zor.cs
VERBOSE: Processing: ShoppingCart.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ShoppingCart"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 Item binding(s) to @context
VERBOSE:   [TagPrefix] Removed asp: prefix from 13 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 8 closing tag(s)
VERBOSE:   [Attribute] Removed 8 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 1 'EnableViewState' attribute(s)
VERBOSE:   [Attribute] Converted 1 ItemType to TItem
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\ShoppingCart.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\ShoppingCart.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ShoppingCart.ra
azor.cs
VERBOSE: Processing: Site.Master
VERBOSE:   [Directive] Removed <%@ Master %>
VERBOSE:   [MasterPage] Removed <asp:ScriptManager> block
VERBOSE:   [MasterPage] Extracted 4 head element(s) into <HeadContent>
VERBOSE:   [MasterPage] Removed <head> section
VERBOSE:   [MasterPage] Stripped document wrapper (DOCTYPE, html, body)
VERBOSE:   [MasterPage] ContentPlaceHolder MainContent → @Body
VERBOSE:   [Form] Removed <form runat="server"> and </form>
VERBOSE:   [Expression] Converted 1 Item binding(s) to @context
VERBOSE:   [Expression] Converted 3 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 4 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 2 closing tag(s)
VERBOSE:   [Attribute] Removed 15 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 1 'ViewStateMode' attribute(s)
VERBOSE:   [Attribute] Converted 1 ItemType to TItem
VERBOSE:   [URL] Converted 12 href ~/ reference(s) to /
VERBOSE:   [URL] Converted 1 ImageUrl ~/ reference(s) to /
VERBOSE:   [Rename] .master → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Components\Layout\MainLayout.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Components\Layout\MainLayout.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Components\Layo
out\MainLayout.razor.cs
VERBOSE: Processing: Site.Mobile.Master
VERBOSE:   [Directive] Removed <%@ Master %>
VERBOSE:   [MasterPage] Extracted 2 head element(s) into <HeadContent>
VERBOSE:   [MasterPage] Removed <head> section
VERBOSE:   [MasterPage] Stripped document wrapper (DOCTYPE, html, body)
VERBOSE:   [MasterPage] ContentPlaceHolder MainContent → @Body (self-closing)
VERBOSE:   [Directive] Removed <%@ Register %> (review component references)
VERBOSE:   [Form] Removed <form runat="server"> and </form>
VERBOSE:   [Attribute] Removed 1 'runat' attribute(s)
VERBOSE:   [Rename] .master → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Components\Layout\Site.MobileLayout.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Components\Layout\Site.MobileLayout.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Components\Layo
out\Site.MobileLayout.razor.cs
VERBOSE: Processing: ViewSwitcher.ascx
VERBOSE:   [Directive] Removed <%@ Control %>
VERBOSE:   [Expression] Converted 3 encoded expression(s)
VERBOSE:   [Rename] .ascx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\ViewSwitcher.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\ViewSwitcher.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ViewSwitcher.ra
azor.cs
VERBOSE: Processing: Account\AddPhoneNumber.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/AddPhoneNumber"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 6 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 6 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\AddPhoneNumber.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\AddPhoneNumber.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\AddPhon
neNumber.razor.cs
VERBOSE: Processing: Account\Confirm.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Confirm"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 3 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 3 closing tag(s)
VERBOSE:   [Attribute] Removed 3 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 2 'ViewStateMode' attribute(s)
VERBOSE:   [URL] Converted 1 NavigateUrl ~/ reference(s) to /
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\Confirm.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\Confirm.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Confirm
m.razor.cs
VERBOSE: Processing: Account\Forgot.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Forgot"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 8 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 4 closing tag(s)
VERBOSE:   [Attribute] Removed 8 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\Forgot.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\Forgot.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Forgot.
.razor.cs
VERBOSE: Processing: Account\Lockout.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Lockout"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\Lockout.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\Lockout.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Lockout
t.razor.cs
VERBOSE: Processing: Account\Login.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Login"
VERBOSE:   [Directive] Removed <%@ Register %> (review component references)
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 comment(s) to Razor syntax
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 13 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 6 closing tag(s)
VERBOSE:   [Attribute] Removed 14 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 2 'ViewStateMode' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\Login.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\Login.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Login.r
razor.cs
VERBOSE: Processing: Account\Manage.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Manage"
VERBOSE:   [Directive] Removed <%@ Register %> (review component references)
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 4 comment(s) to Razor syntax
VERBOSE:   [Expression] Converted 3 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 10 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 10 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 1 'ViewStateMode' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\Manage.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\Manage.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Manage.
.razor.cs
VERBOSE: Processing: Account\ManageLogins.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ManageLogins"
VERBOSE:   [Directive] Removed <%@ Register %> (review component references)
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 Item binding(s) to @context
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 3 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 2 closing tag(s)
VERBOSE:   [Attribute] Removed 5 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 1 'ViewStateMode' attribute(s)
VERBOSE:   [Attribute] Converted 1 ItemType to TItem
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\ManageLogins.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\ManageLogins.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ManageL
Logins.razor.cs
VERBOSE: Processing: Account\ManagePassword.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ManagePassword"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 24 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 7 closing tag(s)
VERBOSE:   [Attribute] Removed 24 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\ManagePassword.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\ManagePassword.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ManageP
Password.razor.cs
VERBOSE: Processing: Account\OpenAuthProviders.ascx
VERBOSE:   [Directive] Removed <%@ Control %>
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 1 'runat' attribute(s)
VERBOSE:   [Attribute] Removed 1 'ViewStateMode' attribute(s)
VERBOSE:   [Attribute] Converted 1 ItemType to TItem
VERBOSE:   [Rename] .ascx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\OpenAuthProviders.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\OpenAuthProviders.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\OpenAut
thProviders.razor.cs
VERBOSE: Processing: Account\Register.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/Register"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 13 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 3 closing tag(s)
VERBOSE:   [Attribute] Removed 13 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\Register.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\Register.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Registe
er.razor.cs
VERBOSE: Processing: Account\RegisterExternalLogin.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/RegisterExternalLogin"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 2 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 7 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 2 closing tag(s)
VERBOSE:   [Attribute] Removed 7 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\RegisterExternalLogin.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\RegisterExternalLogin.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Registe
erExternalLogin.razor.cs
VERBOSE: Processing: Account\ResetPassword.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ResetPassword"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 13 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 3 closing tag(s)
VERBOSE:   [Attribute] Removed 13 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\ResetPassword.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\ResetPassword.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ResetPa
assword.razor.cs
VERBOSE: Processing: Account\ResetPasswordConfirmation.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/ResetPasswordConfirmation"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 1 'runat' attribute(s)
VERBOSE:   [URL] Converted 1 NavigateUrl ~/ reference(s) to /
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\ResetPasswordConfirmation.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\ResetPasswordConfirmation.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ResetPa
asswordConfirmation.razor.cs
VERBOSE: Processing: Account\TwoFactorAuthenticationSignIn.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/TwoFactorAuthenticationSignIn"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 12 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 4 closing tag(s)
VERBOSE:   [Attribute] Removed 12 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\TwoFactorAuthenticationSignIn.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\TwoFactorAuthenticationSignIn.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\TwoFact
torAuthenticationSignIn.razor.cs
VERBOSE: Processing: Account\VerifyPhoneNumber.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/VerifyPhoneNumber"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 encoded expression(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 7 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 7 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Account\VerifyPhoneNumber.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Account\VerifyPhoneNumber.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\VerifyP
PhoneNumber.razor.cs
VERBOSE: Processing: Admin\AdminPage.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/AdminPage"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 21 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 18 closing tag(s)
VERBOSE:   [Attribute] Removed 21 'runat' attribute(s)
VERBOSE:   [Attribute] Converted 2 ItemType to TItem
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Admin\AdminPage.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Admin\AdminPage.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Admin\AdminPage
e.razor.cs
VERBOSE: Processing: Checkout\CheckoutCancel.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/CheckoutCancel"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Checkout\CheckoutCancel.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Checkout\CheckoutCancel.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\Checko
outCancel.razor.cs
VERBOSE: Processing: Checkout\CheckoutComplete.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/CheckoutComplete"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 2 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 1 closing tag(s)
VERBOSE:   [Attribute] Removed 2 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Checkout\CheckoutComplete.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Checkout\CheckoutComplete.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\Checko
outComplete.razor.cs
VERBOSE: Processing: Checkout\CheckoutError.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/CheckoutError"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 3 unencoded expression(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Checkout\CheckoutError.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Checkout\CheckoutError.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\Checko
outError.razor.cs
VERBOSE: Processing: Checkout\CheckoutReview.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/CheckoutReview"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Expression] Converted 1 Eval() with format string to @context.ToString()
VERBOSE:   [Expression] Converted 6 Eval() binding(s) to @context
VERBOSE:   [TagPrefix] Removed asp: prefix from 15 opening tag(s)
VERBOSE:   [TagPrefix] Removed asp: prefix from 10 closing tag(s)
VERBOSE:   [Attribute] Removed 10 'runat' attribute(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Checkout\CheckoutReview.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Checkout\CheckoutReview.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\Checko
outReview.razor.cs
VERBOSE: Processing: Checkout\CheckoutStart.aspx
VERBOSE:   [Directive] <%@ Page %> → @page "/CheckoutStart"
VERBOSE:   [Content] Removed asp:Content open tag
VERBOSE:   [Content] Removed 1 </asp:Content> closing tag(s)
VERBOSE:   [Rename] .aspx → .razor
VERBOSE: Performing the operation "Write transformed Razor file" on target "D:\BlazorWebFormsComponents\samples\Migratio
onRun4\Checkout\CheckoutStart.razor".
VERBOSE: Performing the operation "Copy code-behind with TODO annotations" on target "D:\BlazorWebFormsComponents\sample
es\MigrationRun4\Checkout\CheckoutStart.razor.cs".
VERBOSE:   [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\Checko
outStart.razor.cs

Copying 79 static file(s)...
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\favico
on.ico".

VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalog\Images\boatbig.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\boatpaper.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\boatsail.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\busdouble.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\busgreen.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\busred.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\carconvert.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\carearly.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\carfast.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\carfaster.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\carracer.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\planeace.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\planeglider.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\planepaper.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\planeprop.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\rocket.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\truckbig.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\truckearly.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\truckfire.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\boatbig.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\boatpaper.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\boatsail.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\busdouble.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\busgreen.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\busred.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\carconvert.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\carearly.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\carfast.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\carfaster.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\carracer.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\planeace.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\planeglider.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\planepaper.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\planeprop.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\rocket.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\truckbig.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\truckearly.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Catalo
og\Images\Thumbs\truckfire.png".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Conten
nt\bootstrap-original.css".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Conten
nt\bootstrap-original.min.css".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Conten
nt\bootstrap.css".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Conten
nt\bootstrap.min.css".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Conten
nt\Site.css".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\fonts\
\glyphicons-halflings-regular.eot".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\fonts\
\glyphicons-halflings-regular.svg".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\fonts\
\glyphicons-halflings-regular.ttf".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\fonts\
\glyphicons-halflings-regular.woff".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Images
s\logo.jpg".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\_references.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\bootstrap.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\bootstrap.min.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\jquery-1.10.2.intellisense.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\jquery-1.10.2.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\jquery-1.10.2.min.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\modernizr-2.6.2.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\respond.js".

VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Scripts\respond.min.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\DetailsView.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\Focus.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\GridView.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\Menu.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MenuStandards.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\SmartNav.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\TreeView.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\WebForms.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\WebParts.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\WebUIValidation.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjax.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxApplicationServices.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxComponentModel.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxCore.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxGlobalization.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxHistory.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxNetwork.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxSerialization.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxTimer.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxWebForms.js".
VERBOSE: Performing the operation "Copy static file" on target "D:\BlazorWebFormsComponents\samples\MigrationRun4\Script
ts\WebForms\MSAjax\MicrosoftAjaxWebServices.js".

============================================================
  Migration Summary
============================================================
  Files processed:       32
  Transforms applied:    289
  Static files copied:   79
  Items needing review:  18

--- Items Needing Manual Attention ---
  [CodeBlock] (11 item(s)):
    • ProductList.aspx: Unconverted code block: <%#: GetRouteUrl("ProductByNameRoute", new {productName = Item.ProductNa
ame}) %>
    • ProductList.aspx: Unconverted code block: <%#: GetRouteUrl("ProductByNameRoute", new {productName = Item.ProductNa
ame}) %>
    • ShoppingCart.aspx: Unconverted code block: <%#: String.Format("{0:c}", ((Convert.ToDouble(Item.Quantity)) *  Conve
ert.ToDoub
    • Site.Master: Unconverted code block: <%#: GetRouteUrl("ProductsByCategoryRoute", new {categoryName = Item.Category
yNam
    • Account\Manage.aspx: Unconverted code block: <% } %>
    • Account\Manage.aspx: Unconverted code block: <% } %>
    • Account\ManageLogins.aspx: Unconverted code block: <%# "Remove this " + Item.LoginProvider + " login from your acc
count" %>
    • Account\ManageLogins.aspx: Unconverted code block: <%# CanRemoveExternalLogins %>
    • Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %>
    • Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %>
    • Account\OpenAuthProviders.ascx: Unconverted code block: <%#: Item %>
  [ContentPlaceHolder] (1 item(s)):
    • Site.Mobile.Master: Non-MainContent ContentPlaceHolder ID='FeaturedContent' needs manual conversion (self-closing)
  [LoginView] (1 item(s)):
    • Site.Master: LoginView requires conversion to AuthorizeView with Authorized/NotAuthorized templates
  [Register] (4 item(s)):
    • Site.Mobile.Master: Removed Register directive — verify component tag prefixes: <%@ Register Src="~/ViewSwitcher.a
ascx" TagPrefix="friendlyUrls" TagName="ViewSwitcher" %>
    • Account\Login.aspx: Removed Register directive — verify component tag prefixes: <%@ Register Src="~/Account/OpenAu
uthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>
    • Account\Manage.aspx: Removed Register directive — verify component tag prefixes: <%@ Register Src="~/Account/OpenA
AuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>
    • Account\ManageLogins.aspx: Removed Register directive — verify component tag prefixes: <%@ Register Src="~/Account
t/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>
  [SelectMethod] (1 item(s)):
    • Site.Master: SelectMethod requires manual data-binding conversion (OnInitializedAsync)

--- Detailed Transform Log ---
  About.aspx:
    [Directive] <%@ Page %> → @page "/About"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [Rename] .aspx → .razor
  About.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\About.razor.cs        
  Account\AddPhoneNumber.aspx:
    [Directive] <%@ Page %> → @page "/AddPhoneNumber"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 6 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 6 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\AddPhoneNumber.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\AddPhoneNumber
r.razor.cs
  Account\Confirm.aspx:
    [Directive] <%@ Page %> → @page "/Confirm"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 3 opening tag(s)
    [TagPrefix] Removed asp: prefix from 3 closing tag(s)
    [Attribute] Removed 3 'runat' attribute(s)
    [Attribute] Removed 2 'ViewStateMode' attribute(s)
    [URL] Converted 1 NavigateUrl ~/ reference(s) to /
    [Rename] .aspx → .razor
  Account\Confirm.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Confirm.razor.
.cs
  Account\Forgot.aspx:
    [Directive] <%@ Page %> → @page "/Forgot"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 8 opening tag(s)
    [TagPrefix] Removed asp: prefix from 4 closing tag(s)
    [Attribute] Removed 8 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\Forgot.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Forgot.razor.c
cs
  Account\Lockout.aspx:
    [Directive] <%@ Page %> → @page "/Lockout"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Rename] .aspx → .razor
  Account\Lockout.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Lockout.razor.
.cs
  Account\Login.aspx:
    [Directive] <%@ Page %> → @page "/Login"
    [Directive] Removed <%@ Register %> (review component references)
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 comment(s) to Razor syntax
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 13 opening tag(s)
    [TagPrefix] Removed asp: prefix from 6 closing tag(s)
    [Attribute] Removed 14 'runat' attribute(s)
    [Attribute] Removed 2 'ViewStateMode' attribute(s)
    [Rename] .aspx → .razor
  Account\Login.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Login.razor.cs
  Account\Manage.aspx:
    [Directive] <%@ Page %> → @page "/Manage"
    [Directive] Removed <%@ Register %> (review component references)
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 4 comment(s) to Razor syntax
    [Expression] Converted 3 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 10 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 10 'runat' attribute(s)
    [Attribute] Removed 1 'ViewStateMode' attribute(s)
    [Rename] .aspx → .razor
  Account\Manage.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Manage.razor.c
cs
  Account\ManageLogins.aspx:
    [Directive] <%@ Page %> → @page "/ManageLogins"
    [Directive] Removed <%@ Register %> (review component references)
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 Item binding(s) to @context
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 3 opening tag(s)
    [TagPrefix] Removed asp: prefix from 2 closing tag(s)
    [Attribute] Removed 5 'runat' attribute(s)
    [Attribute] Removed 1 'ViewStateMode' attribute(s)
    [Attribute] Converted 1 ItemType to TItem
    [Rename] .aspx → .razor
  Account\ManageLogins.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ManageLogins.r
razor.cs
  Account\ManagePassword.aspx:
    [Directive] <%@ Page %> → @page "/ManagePassword"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 24 opening tag(s)
    [TagPrefix] Removed asp: prefix from 7 closing tag(s)
    [Attribute] Removed 24 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\ManagePassword.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ManagePassword
d.razor.cs
  Account\OpenAuthProviders.ascx:
    [Directive] Removed <%@ Control %>
    [TagPrefix] Removed asp: prefix from 1 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 1 'runat' attribute(s)
    [Attribute] Removed 1 'ViewStateMode' attribute(s)
    [Attribute] Converted 1 ItemType to TItem
    [Rename] .ascx → .razor
  Account\OpenAuthProviders.ascx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\OpenAuthProvid
ders.razor.cs
  Account\Register.aspx:
    [Directive] <%@ Page %> → @page "/Register"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 13 opening tag(s)
    [TagPrefix] Removed asp: prefix from 3 closing tag(s)
    [Attribute] Removed 13 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\Register.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\Register.razor
r.cs
  Account\RegisterExternalLogin.aspx:
    [Directive] <%@ Page %> → @page "/RegisterExternalLogin"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 2 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 7 opening tag(s)
    [TagPrefix] Removed asp: prefix from 2 closing tag(s)
    [Attribute] Removed 7 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\RegisterExternalLogin.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\RegisterExtern
nalLogin.razor.cs
  Account\ResetPassword.aspx:
    [Directive] <%@ Page %> → @page "/ResetPassword"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 13 opening tag(s)
    [TagPrefix] Removed asp: prefix from 3 closing tag(s)
    [Attribute] Removed 13 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\ResetPassword.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ResetPassword.
.razor.cs
  Account\ResetPasswordConfirmation.aspx:
    [Directive] <%@ Page %> → @page "/ResetPasswordConfirmation"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 1 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 1 'runat' attribute(s)
    [URL] Converted 1 NavigateUrl ~/ reference(s) to /
    [Rename] .aspx → .razor
  Account\ResetPasswordConfirmation.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\ResetPasswordC
Confirmation.razor.cs
  Account\TwoFactorAuthenticationSignIn.aspx:
    [Directive] <%@ Page %> → @page "/TwoFactorAuthenticationSignIn"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 12 opening tag(s)
    [TagPrefix] Removed asp: prefix from 4 closing tag(s)
    [Attribute] Removed 12 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\TwoFactorAuthenticationSignIn.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\TwoFactorAuthe
enticationSignIn.razor.cs
  Account\VerifyPhoneNumber.aspx:
    [Directive] <%@ Page %> → @page "/VerifyPhoneNumber"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 7 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 7 'runat' attribute(s)
    [Rename] .aspx → .razor
  Account\VerifyPhoneNumber.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Account\VerifyPhoneNum
mber.razor.cs
  AddToCart.aspx:
    [Directive] <%@ Page %> → @page "/AddToCart"
    [Form] Removed <form runat="server"> and </form>
    [Attribute] Removed 1 'runat' attribute(s)
    [Rename] .aspx → .razor
  AddToCart.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\AddToCart.razor.cs    
  Admin\AdminPage.aspx:
    [Directive] <%@ Page %> → @page "/AdminPage"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [TagPrefix] Removed asp: prefix from 21 opening tag(s)
    [TagPrefix] Removed asp: prefix from 18 closing tag(s)
    [Attribute] Removed 21 'runat' attribute(s)
    [Attribute] Converted 2 ItemType to TItem
    [Rename] .aspx → .razor
  Admin\AdminPage.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Admin\AdminPage.razor.
.cs
  Checkout\CheckoutCancel.aspx:
    [Directive] <%@ Page %> → @page "/CheckoutCancel"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Rename] .aspx → .razor
  Checkout\CheckoutCancel.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\CheckoutCance
el.razor.cs
  Checkout\CheckoutComplete.aspx:
    [Directive] <%@ Page %> → @page "/CheckoutComplete"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [TagPrefix] Removed asp: prefix from 2 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 2 'runat' attribute(s)
    [Rename] .aspx → .razor
  Checkout\CheckoutComplete.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\CheckoutCompl
lete.razor.cs
  Checkout\CheckoutError.aspx:
    [Directive] <%@ Page %> → @page "/CheckoutError"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 3 unencoded expression(s)
    [Rename] .aspx → .razor
  Checkout\CheckoutError.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\CheckoutError
r.razor.cs
  Checkout\CheckoutReview.aspx:
    [Directive] <%@ Page %> → @page "/CheckoutReview"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 Eval() with format string to @context.ToString()
    [Expression] Converted 6 Eval() binding(s) to @context
    [TagPrefix] Removed asp: prefix from 15 opening tag(s)
    [TagPrefix] Removed asp: prefix from 10 closing tag(s)
    [Attribute] Removed 10 'runat' attribute(s)
    [Rename] .aspx → .razor
  Checkout\CheckoutReview.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\CheckoutRevie
ew.razor.cs
  Checkout\CheckoutStart.aspx:
    [Directive] <%@ Page %> → @page "/CheckoutStart"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Rename] .aspx → .razor
  Checkout\CheckoutStart.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Checkout\CheckoutStart
t.razor.cs
  Contact.aspx:
    [Directive] <%@ Page %> → @page "/Contact"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [Rename] .aspx → .razor
  Contact.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Contact.razor.cs      
  D:\BlazorWebFormsComponents\samples\MigrationRun4\_Imports.razor:
    [Scaffold] Generated _Imports.razor
  D:\BlazorWebFormsComponents\samples\MigrationRun4\Components\App.razor:
    [Scaffold] Generated Components/App.razor
  D:\BlazorWebFormsComponents\samples\MigrationRun4\Components\Routes.razor:
    [Scaffold] Generated Components/Routes.razor
  D:\BlazorWebFormsComponents\samples\MigrationRun4\Program.cs:
    [Scaffold] Generated Program.cs
  D:\BlazorWebFormsComponents\samples\MigrationRun4\Properties\launchSettings.json:
    [Scaffold] Generated Properties/launchSettings.json
  D:\BlazorWebFormsComponents\samples\MigrationRun4\WingtipToys.csproj:
    [Scaffold] Generated WingtipToys.csproj
  Default.aspx:
    [Directive] <%@ Page %> → @page "/"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 encoded expression(s)
    [Rename] .aspx → .razor
  Default.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Default.razor.cs      
  ErrorPage.aspx:
    [Directive] <%@ Page %> → @page "/ErrorPage"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [TagPrefix] Removed asp: prefix from 6 opening tag(s)
    [TagPrefix] Removed asp: prefix from 2 closing tag(s)
    [Attribute] Removed 6 'runat' attribute(s)
    [Rename] .aspx → .razor
  ErrorPage.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ErrorPage.razor.cs    
  ProductDetails.aspx:
    [Directive] <%@ Page %> → @page "/ProductDetails"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 String.Format(Item.) to interpolated string
    [Expression] Converted 5 Item binding(s) to @context
    [TagPrefix] Removed asp: prefix from 1 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 1 'runat' attribute(s)
    [Attribute] Converted 1 ItemType to TItem
    [Rename] .aspx → .razor
  ProductDetails.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ProductDetails.razor.c
cs
  ProductList.aspx:
    [Directive] <%@ Page %> → @page "/ProductList"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 String.Format(Item.) to interpolated string
    [Expression] Converted 3 Item binding(s) to @context
    [Expression] Converted 1 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 1 opening tag(s)
    [TagPrefix] Removed asp: prefix from 1 closing tag(s)
    [Attribute] Removed 5 'runat' attribute(s)
    [Attribute] Converted 1 ItemType to TItem
    [Rename] .aspx → .razor
  ProductList.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ProductList.razor.cs  
  ShoppingCart.aspx:
    [Directive] <%@ Page %> → @page "/ShoppingCart"
    [Content] Removed asp:Content open tag
    [Content] Removed 1 </asp:Content> closing tag(s)
    [Expression] Converted 1 Item binding(s) to @context
    [TagPrefix] Removed asp: prefix from 13 opening tag(s)
    [TagPrefix] Removed asp: prefix from 8 closing tag(s)
    [Attribute] Removed 8 'runat' attribute(s)
    [Attribute] Removed 1 'EnableViewState' attribute(s)
    [Attribute] Converted 1 ItemType to TItem
    [Rename] .aspx → .razor
  ShoppingCart.aspx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ShoppingCart.razor.cs 
  Site.Master:
    [Directive] Removed <%@ Master %>
    [MasterPage] Removed <asp:ScriptManager> block
    [MasterPage] Extracted 4 head element(s) into <HeadContent>
    [MasterPage] Removed <head> section
    [MasterPage] Stripped document wrapper (DOCTYPE, html, body)
    [MasterPage] ContentPlaceHolder MainContent → @Body
    [Form] Removed <form runat="server"> and </form>
    [Expression] Converted 1 Item binding(s) to @context
    [Expression] Converted 3 encoded expression(s)
    [TagPrefix] Removed asp: prefix from 4 opening tag(s)
    [TagPrefix] Removed asp: prefix from 2 closing tag(s)
    [Attribute] Removed 15 'runat' attribute(s)
    [Attribute] Removed 1 'ViewStateMode' attribute(s)
    [Attribute] Converted 1 ItemType to TItem
    [URL] Converted 12 href ~/ reference(s) to /
    [URL] Converted 1 ImageUrl ~/ reference(s) to /
    [Rename] .master → .razor
  Site.Master.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Components\Layout\Main
nLayout.razor.cs
  Site.Mobile.Master:
    [Directive] Removed <%@ Master %>
    [MasterPage] Extracted 2 head element(s) into <HeadContent>
    [MasterPage] Removed <head> section
    [MasterPage] Stripped document wrapper (DOCTYPE, html, body)
    [MasterPage] ContentPlaceHolder MainContent → @Body (self-closing)
    [Directive] Removed <%@ Register %> (review component references)
    [Form] Removed <form runat="server"> and </form>
    [Attribute] Removed 1 'runat' attribute(s)
    [Rename] .master → .razor
  Site.Mobile.Master.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\Components\Layout\Site
e.MobileLayout.razor.cs
  ViewSwitcher.ascx:
    [Directive] Removed <%@ Control %>
    [Expression] Converted 3 encoded expression(s)
    [Rename] .ascx → .razor
  ViewSwitcher.ascx.cs:
    [CodeBehind] Copied with TODO annotations → D:\BlazorWebFormsComponents\samples\MigrationRun4\ViewSwitcher.razor.cs 

Migration complete. Next steps:
  1. Review items flagged above for manual attention
  2. Use the BWFC Copilot skill for code-behind transforms (Layer 2)
  3. Build and test: dotnet build && dotnet run

___BEGIN___COMMAND_DONE_MARKER___0
PS D:\BlazorWebFormsComponents>