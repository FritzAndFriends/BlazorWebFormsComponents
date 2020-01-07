# DataList

The DataList component is meant to emulate the asp:DataList control in markup and is defined in the [System.Web.UI.WebControls.DataList class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datalist?view=netframework-4.8)

[Usage Notes](#usage-notes) | [Web Forms Syntax](#web-forms-declarative-syntax) | [Blazor Syntax](#blazor-syntax)

## Features supported in Blazor

- Flow Layout
    - Empty List
    - FooterStyle
    - FooterTemplate
    - HeaderStyle
    - HeaderTemplate
    - Tooltip
    - Single Column
 - Table Layout
    - Accessible Headers
    - Empty List
    - FooterStyle
    - FooterTemplate
    - HeaderStyle
    - HeaderTemplate
    - Tooltip
    - Single Column
    - Tooltip

##### [Back to top](#datalist)

## Usage Notes

- The following Web Forms features are ignored
    - `runat="server"`
    - `EnableViewState`
- `ID` should be converted to `@ref` if the component is referenced in code
- `ItemType` MUST be defined as an attribute
- `Context` should be used to define the object used in templates.  If not defined, the default `<INSERT>` will be available. 

##### [Back to top](#datalist)

## Web Forms Declarative Syntax

```html
<asp:DataList  
    AccessKey="string"  
    BackColor="color name|#dddddd"  
    BorderColor="color name|#dddddd"  
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|  
        Inset|Outset"  
    BorderWidth="size"  
    Caption="string"  
    CaptionAlign="NotSet|Top|Bottom|Left|Right"  
    CellPadding="integer"  
    CellSpacing="integer"  
    CssClass="string"  
    DataKeyField="string"  
    DataMember="string"  
    DataSource="string"  
    DataSourceID="string"  
    EditItemIndex="integer"  
    Enabled="True|False"  
    EnableTheming="True|False"  
    EnableViewState="True|False"  
    ExtractTemplateRows="True|False"  
    Font-Bold="True|False"  
    Font-Italic="True|False"  
    Font-Names="string"  
    Font-Overline="True|False"  
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|  
        Large|X-Large|XX-Large"  
    Font-Strikeout="True|False"  
    Font-Underline="True|False"  
    ForeColor="color name|#dddddd"  
    GridLines="None|Horizontal|Vertical|Both"  
    Height="size"  
    HorizontalAlign="NotSet|Left|Center|Right|Justify"  
    ID="string"  
    OnCancelCommand="CancelCommand event handler"  
    OnDataBinding="DataBinding event handler"  
    OnDeleteCommand="DeleteCommand event handler"  
    OnDisposed="Disposed event handler"  
    OnEditCommand="EditCommand event handler"  
    OnInit="Init event handler"  
    OnItemCommand="ItemCommand event handler"  
    OnItemCreated="ItemCreated event handler"  
    OnItemDataBound="ItemDataBound event handler"  
    OnLoad="Load event handler"  
    OnPreRender="PreRender event handler"  
    OnSelectedIndexChanged="SelectedIndexChanged event handler"  
    OnUnload="Unload event handler"  
    OnUpdateCommand="UpdateCommand event handler"  
    RepeatColumns="integer"  
    RepeatDirection="Horizontal|Vertical"  
    RepeatLayout="Table|Flow"  
    runat="server"  
    SelectedIndex="integer"  
    ShowFooter="True|False"  
    ShowHeader="True|False"  
    SkinID="string"  
    Style="string"  
    TabIndex="integer"  
    ToolTip="string"  
    UseAccessibleHeader="True|False"  
    Visible="True|False"  
    Width="size"  
>  
        <AlternatingItemStyle />  
        <AlternatingItemTemplate>  
            <!-- child controls -->  
        </AlternatingItemTemplate>  
        <EditItemStyle />  
        <EditItemTemplate>  
            <!-- child controls -->  
        </EditItemTemplate>  
        <FooterStyle />  
        <FooterTemplate>  
            <!-- child controls -->  
        </FooterTemplate>  
        <HeaderStyle />  
        <HeaderTemplate>  
            <!-- child controls -->  
        </HeaderTemplate>  
        <ItemStyle />  
        <ItemTemplate>  
            <!-- child controls -->  
        </ItemTemplate>  
        <SelectedItemStyle />  
        <SelectedItemTemplate>  
            <!-- child controls -->  
        </SelectedItemTemplate>  
        <SeparatorStyle />  
        <SeparatorTemplate>  
            <!-- child controls -->  
        </SeparatorTemplate>  
</asp:DataList>
```

##### [Back to top](#datalist)

## Blazor Syntax

##### [Back to top](#datalist)

