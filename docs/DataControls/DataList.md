# DataList

The **DataList** component emulates the ASP.NET Web Forms `asp:DataList` control. It is defined in the [System.Web.UI.WebControls.DataList class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datalist?view=netframework-4.8).

## Features Supported in Blazor

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

## Usage Notes

- The following Web Forms features are ignored
    - `runat="server"`
    - `EnableViewState`
- `ID` should be converted to `@ref` if the component is referenced in code
- `ItemType` MUST be defined as an attribute
- **For Web Forms compatibility, use `Context="Item"`** to access the current data item in templates with the name `@Item` instead of Blazor's default `@context`

## Syntax Comparison

=== "Web Forms"

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

=== "Blazor"

    ```razor
    <DataList ItemType="MyItem"
        SelectMethod="GetItems"
        RepeatLayout="RepeatLayout.Table"
        RepeatDirection="RepeatDirection.Vertical"
        RepeatColumns="1"
        ShowHeader="true"
        ShowFooter="true"
        UseAccessibleHeader="true"
        Visible="true">
        <HeaderStyle BackColor="#CCCCCC" Font-Bold="true" />
        <ItemStyle BackColor="#FFFFFF" />
        <AlternatingItemStyle BackColor="#F5F5F5" />
        <FooterStyle BackColor="#CCCCCC" />
        <HeaderTemplate>
            <strong>Items</strong>
        </HeaderTemplate>
        <ItemTemplate Context="Item">
            @Item.Name
        </ItemTemplate>
        <FooterTemplate>
            <em>End of list</em>
        </FooterTemplate>
    </DataList>
    ```

## Migration Notes

1. **Remove the `asp:` prefix** — `<asp:DataList>` becomes `<DataList>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Add `ItemType`** — The `ItemType` attribute is required in the Blazor version
4. **Use `SelectMethod`** — Replace code-behind `DataSource`/`DataBind()` with `SelectMethod`
5. **Use `Context="Item"`** — For Web Forms compatibility, use `Context="Item"` in templates to access the current item as `@Item`
6. **Style sub-components** — `HeaderStyle`, `ItemStyle`, `AlternatingItemStyle`, `FooterStyle` work the same way
7. **Layout properties** — `RepeatLayout`, `RepeatDirection`, and `RepeatColumns` map directly
