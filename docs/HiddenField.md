# HiddenField

The HiddenField component is meant to emulate the asp:HiddenField control in markup and is defined in the [System.Web.UI.WebControls.HiddenField class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.hiddenfield?view=netframework-4.8)

## Blazor Features Supported

- `Value` the value of the hidden field component.
- `OnValueChanged` Occurs when the value of the HiddenField component changes between posts to the server.

## WebForms Syntax

```html
<asp:HiddenField
    EnableTheming="True|False"
    EnableViewState="True|False"
    ID="string"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    OnValueChanged="ValueChanged event handler"
    runat="server"
    SkinID="string"
    Value="string"
    Visible="True|False"
/>
```
