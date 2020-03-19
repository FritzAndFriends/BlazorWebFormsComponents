# Button

It may seem strange that we have a Button component when there already is an HTML button and Blazor has features that enable C# interactions with that button, but we need to activate other features that were once present in Web Forms.  Original Web Forms documentation is at:  https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.button?view=netframework-4.8

## Blazor Features Supported

- OnClick event handler
- OnClientClick JavaScript pointer
- OnCommand event handler with event bubbling

## WebForms Syntax

```html
<asp:Button
    AccessKey="string"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    CausesValidation="True|False"
    CommandArgument="string"
    CommandName="string"
    CssClass="string"
    Enabled="True|False"
    EnableTheming="True|False"
    EnableViewState="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Overline="True|False"
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|
        Large|X-Large|XX-Large"
    Font-Strikeout="True|False"
    Font-Underline="True|False"
    ForeColor="color name|#dddddd"
    Height="size"
    ID="string"
    OnClick="Click event handler"
    OnClientClick="string"
    OnCommand="Command event handler"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    PostBackUrl="uri"
    runat="server"
    SkinID="string"
    Style="string"
    TabIndex="integer"
    Text="string"
    ToolTip="string"
    UseSubmitBehavior="True|False"
    ValidationGroup="string"
    Visible="True|False"
    Width="size"
/>
```
