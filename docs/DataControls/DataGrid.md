# DataGrid

The DataGrid component is meant to emulate the asp:DataGrid control in markup and is defined in the [System.Web.UI.WebControls.DataGrid class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datagrid?view=netframework-4.8)

## Features supported in Blazor

- Readonly grid
- Bound, Button, Hyperlink, and Template columns
- Auto-generate columns
- Show/Hide header row
- Empty data text

### Blazor Notes

- The `ItemCommand.CommandSource` object will be populated with the `ButtonField` object
- DataGrid uses the same column types as GridView (BoundField, ButtonField, etc.)
- DataGrid is a legacy control superseded by GridView in ASP.NET 2.0, but is provided for compatibility
- **ItemType cascading** - The `ItemType` parameter is automatically cascaded from the DataGrid to child columns. You only need to specify it once on the DataGrid, and all child columns (BoundField, TemplateField, HyperLinkField, ButtonField) will automatically infer the type. For backward compatibility, you can still explicitly specify `ItemType` on individual columns if desired.

## Web Forms Features NOT Supported

The following DataGrid features from Web Forms are not currently supported:

- Paging (AllowPaging, PageSize, CurrentPageIndex)
- Sorting (AllowSorting)
- Editing (EditItemIndex, OnEditCommand, OnUpdateCommand, OnCancelCommand)
- Selection
- Custom paging
- Footer templates

## Web Forms Declarative Syntax

```html
<asp:DataGrid
    AllowCustomPaging="True|False"
    AllowPaging="True|False"
    AllowSorting="True|False"
    AutoGenerateColumns="True|False"
    BackColor="color name|#dddddd"
    BackImageUrl="uri"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
    BorderWidth="size"
    CellPadding="integer"
    CellSpacing="integer"
    CssClass="string"
    CurrentPageIndex="integer"
    DataKeyField="string"
    DataMember="string"
    DataSource="string"
    DataSourceID="string"
    EditItemIndex="integer"
    EnableViewState="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Overline="True|False"
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|Large|X-Large|XX-Large"
    Font-Strikeout="True|False"
    Font-Underline="True|False"
    ForeColor="color name|#dddddd"
    GridLines="None|Horizontal|Vertical|Both"
    HeaderStyle-BackColor="color name|#dddddd"
    HeaderStyle-Font-Bold="True|False"
    HorizontalAlign="NotSet|Left|Center|Right|Justify"
    ID="string"
    OnCancelCommand="CancelCommand event handler"
    OnDataBinding="DataBinding event handler"
    OnDeleteCommand="DeleteCommand event handler"
    OnEditCommand="EditCommand event handler"
    OnItemCommand="ItemCommand event handler"
    OnItemCreated="ItemCreated event handler"
    OnItemDataBound="ItemDataBound event handler"
    OnPageIndexChanged="PageIndexChanged event handler"
    OnSortCommand="SortCommand event handler"
    OnUpdateCommand="UpdateCommand event handler"
    PageSize="integer"
    runat="server"
    SelectedIndex="integer"
    ShowFooter="True|False"
    ShowHeader="True|False"
    Visible="True|False"
>
    <AlternatingItemStyle />
    <Columns>
        <asp:BoundColumn
            DataField="string"
            DataFormatString="string"
            FooterText="string"
            HeaderImageUrl="uri"
            HeaderText="string"
            ReadOnly="True|False"
            SortExpression="string"
            Visible="True|False"
        >
            <FooterStyle />
            <HeaderStyle />
            <ItemStyle />
        </asp:BoundColumn>
        <asp:ButtonColumn
            ButtonType="LinkButton|PushButton"
            CausesValidation="True|False"
            CommandName="string"
            DataTextField="string"
            DataTextFormatString="string"
            FooterText="string"
            HeaderImageUrl="uri"
            HeaderText="string"
            SortExpression="string"
            Text="string"
            ValidationGroup="string"
            Visible="True|False"
        />
        <asp:HyperLinkColumn
            DataNavigateUrlField="string"
            DataNavigateUrlFormatString="string"
            DataTextField="string"
            DataTextFormatString="string"
            FooterText="string"
            HeaderImageUrl="uri"
            HeaderText="string"
            NavigateUrl="uri"
            SortExpression="string"
            Target="string"
            Text="string"
            Visible="True|False"
        />
        <asp:TemplateColumn
            FooterText="string"
            HeaderImageUrl="uri"
            HeaderText="string"
            SortExpression="string"
            Visible="True|False"
        >
            <EditItemTemplate>
                <!-- template content -->
            </EditItemTemplate>
            <FooterTemplate>
                <!-- template content -->
            </FooterTemplate>
            <HeaderTemplate>
                <!-- template content -->
            </HeaderTemplate>
            <ItemTemplate>
                <!-- template content -->
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
    <EditItemStyle />
    <FooterStyle />
    <HeaderStyle />
    <ItemStyle />
    <PagerStyle Mode="NextPrev|NumericPages" />
    <SelectedItemStyle />
</asp:DataGrid>
```

## Blazor Syntax

Currently, not every syntax element of Web Forms DataGrid is supported. In the meantime, the following DataGrid in Blazor syntax will only include the implemented ones. **Non-implemented elements will be included later**.

```html
<DataGrid
    AutoGenerateColumns=bool
    CssClass=string
    DataKeyField=string
    DataSource=IEnumerable
    EmptyDataText=string
    Enabled=bool
    ID=string
    Items=IEnumerable
    ItemType=Type
    OnDataBinding=EventCallBack
    OnDataBound=EventCallBack
    OnItemCommand=EventCallBack<DataGridCommandEventArgs>
    OnEditCommand=EventCallBack<DataGridCommandEventArgs>
    OnCancelCommand=EventCallBack<DataGridCommandEventArgs>
    OnUpdateCommand=EventCallBack<DataGridCommandEventArgs>
    OnDeleteCommand=EventCallBack<DataGridCommandEventArgs>
    OnInit=EventCallBack
    OnLoad=EventCallBack
    OnPreRender=EventCallBack
    OnUnload=EventCallBack
    OnDisposed=EventCallBack
    SelectMethod=SelectHandler
    ShowHeader=bool
    TabIndex=int
    Visible=bool
>
    <Columns>
        <BoundField
            DataField=string
            DataFormatString=string
            HeaderText=string
            Visible=bool
        />
        <HyperLinkField
            DataNavigateUrlFields=string
            DataNavigateUrlFormatString=string
            DataTextField=string
            DataTextFormatString=string
            HeaderText=string
            NavigateUrl=string
            Target=string
            Text=string
            Visible=bool
        />
        <ButtonField
            ButtonType=ButtonType
            CommandName=string
            DataTextField=string
            DataTextFormatString=string
            HeaderText=string
            ImageUrl=string
            Text=string
            Visible=bool
        />
        <TemplateField
            HeaderText=string
            Visible=bool
        >
            <ItemTemplate>
                <!-- template content -->
            </ItemTemplate>
        </TemplateField>
    </Columns>
</DataGrid>
```

## Examples

### Basic DataGrid with Manual Columns

```razor
<DataGrid ItemType="Customer"
          AutoGenerateColumns="false"
          DataKeyField="CustomerID"
          SelectMethod="GetCustomers"
          EmptyDataText="No data available">
    <Columns>
        <BoundField ItemType="Customer" DataField="CustomerID" HeaderText="ID" />
        <BoundField ItemType="Customer" DataField="CompanyName" HeaderText="Company" />
        <BoundField ItemType="Customer" DataField="FirstName" HeaderText="First Name"/>
        <BoundField ItemType="Customer" DataField="LastName" HeaderText="Last Name"/>
    </Columns>
</DataGrid>
```

### DataGrid with Auto-Generated Columns

```razor
<DataGrid ItemType="Customer"
          DataKeyField="CustomerID"
          SelectMethod="GetCustomers"
          AutoGenerateColumns="true">
</DataGrid>
```

### DataGrid without Header

```razor
<DataGrid ItemType="Customer"
          AutoGenerateColumns="false"
          ShowHeader="false"
          SelectMethod="GetCustomers">
    <Columns>
        <BoundField ItemType="Customer" DataField="CustomerID" HeaderText="ID" />
        <BoundField ItemType="Customer" DataField="CompanyName" HeaderText="Company" />
    </Columns>
</DataGrid>
```

## Comparison with GridView

DataGrid and GridView are very similar in Blazor. The main differences are:

- DataGrid uses `DataKeyField` (singular) while GridView uses `DataKeyNames`
- DataGrid event names use "Item" (OnItemCommand) while GridView uses "Row" (OnRowCommand)
- DataGrid is a legacy control from ASP.NET 1.x, while GridView was introduced in ASP.NET 2.0
- For new projects, GridView is recommended as it has more features and better design-time support

Both controls use the same column types (BoundField, ButtonField, HyperLinkField, TemplateField) and render similar HTML.

## Migration from Web Forms

When migrating DataGrid from Web Forms to Blazor:

1. Replace `<asp:DataGrid>` with `<DataGrid>`
2. Replace `runat="server"` (not needed in Blazor)
3. Change `DataKeyNames` to `DataKeyField` if using
4. Update event handler names (e.g., `OnItemCommand` instead of event code-behind)
5. Use `SelectMethod` instead of setting DataSource in code-behind
6. Consider migrating to GridView for better feature support

## See Also

- [GridView](GridView.md) - The recommended data grid control for new projects
- [DataList](DataList.md) - For custom layout of repeating data
- [Repeater](Repeater.md) - For lightweight data repetition
