The FormView component is meant to emulate the asp:FormView control in markup and is defined in the [System.Web.UI.WebControls.FormView class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.formview?view=netframework-4.8)

## Features supported in Blazor

  - Readonly Form
  - Numerical Pager
  - OnDataBinding and OnDataBound events trigger
  - ModeChanging and ModeChanged events
  - Insert, Edit, Update, Delete actions and supporting events:
    - **OnItemDeleting** / **OnItemDeleted** — before and after delete operations
    - **OnItemInserting** / **OnItemInserted** — before and after insert operations
    - **OnItemUpdating** / **OnItemUpdated** — before and after update operations
  - **ItemCommand** event — fires when any command button within the FormView is clicked
  - **ItemCreated** event — fires when the FormView is first created and data-bound
  - **PageIndexChanging** / **PageIndexChanged** events — fires when paging occurs (cancellable)
  - **HeaderText** / **HeaderTemplate** - Renders a header row above the form content. `HeaderTemplate` takes precedence over `HeaderText` when both are specified.
  - **FooterText** / **FooterTemplate** - Renders a footer row below the form content. `FooterTemplate` takes precedence over `FooterText` when both are specified.
  - **EmptyDataText** / **EmptyDataTemplate** - Displays content when the data source is empty or null. `EmptyDataTemplate` takes precedence over `EmptyDataText` when both are specified.
  - **PagerTemplate** — custom pager UI replacing the default numeric pager
  - **Caption** / **CaptionAlign** — renders a `<caption>` element for table accessibility
  - **Style sub-components** (RowStyle, EditRowStyle, InsertRowStyle, HeaderStyle, FooterStyle, EmptyDataRowStyle, PagerStyle)
  - **[PagerSettings](PagerSettings.md)** — configurable pager button modes, text, images, and position

## Web Forms Features NOT Supported

- **DataSourceID** — Blazor does not use server-side data source controls; bind data directly via `DataSource`
- **ViewState / EnableViewState** — Not needed; Blazor preserves component state natively
- **Theming / SkinID / EnableTheming** — Not applicable to Blazor
- **RenderTable** — Not implemented; the FormView always renders a table

## Usage Notes

- **ItemType attribute** - Required to specify the type of items in the collection
- **Context attribute** - For Web Forms compatibility, use `Context="Item"` to access the current item as `@Item` in templates (ItemTemplate, EditItemTemplate, InsertItemTemplate) instead of Blazor's default `@context`
- **ID** - Use `@ref` instead of `ID` when referencing the component in code
- **Template precedence** - When both a text property and its corresponding template are set, the template always takes precedence (e.g., `HeaderTemplate` overrides `HeaderText`)

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:FormView
        AccessKey="string"
        AllowPaging="True|False"
        BackColor="color name|#dddddd"
        BackImageUrl="uri"
        BorderColor="color name|#dddddd"
        BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
            Inset|Outset"
        BorderWidth="size"
        Caption="string"
        CaptionAlign="NotSet|Top|Bottom|Left|Right"
        CellPadding="integer"
        CellSpacing="integer"
        CssClass="string"
        DataKeyNames="string"
        DataMember="string"
        DataSource="string"
        DataSourceID="string"
        DefaultMode="ReadOnly|Edit|Insert"
        EmptyDataText="string"
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
        FooterText="string"
        ForeColor="color name|#dddddd"
        GridLines="None|Horizontal|Vertical|Both"
        HeaderText="string"
        Height="size"
        HorizontalAlign="NotSet|Left|Center|Right|Justify"
        ID="string"
        OnDataBinding="DataBinding event handler"
        OnDataBound="DataBound event handler"
        OnDisposed="Disposed event handler"
        OnInit="Init event handler"
        OnItemCommand="ItemCommand event handler"
        OnItemCreated="ItemCreated event handler"
        OnItemDeleted="ItemDeleted event handler"
        OnItemDeleting="ItemDeleting event handler"
        OnItemInserted="ItemInserted event handler"
        OnItemInserting="ItemInserting event handler"
        OnItemUpdated="ItemUpdated event handler"
        OnItemUpdating="ItemUpdating event handler"
        OnLoad="Load event handler"
        OnModeChanged="ModeChanged event handler"
        OnModeChanging="ModeChanging event handler"
        OnPageIndexChanged="PageIndexChanged event handler"
        OnPageIndexChanging="PageIndexChanging event handler"
        OnPreRender="PreRender event handler"
        OnUnload="Unload event handler"
        PageIndex="integer"
        PagerSettings-FirstPageImageUrl="uri"
        PagerSettings-FirstPageText="string"
        PagerSettings-LastPageImageUrl="uri"
        PagerSettings-LastPageText="string"
        PagerSettings-Mode="NextPrevious|Numeric|NextPreviousFirstLast|
            NumericFirstLast"
        PagerSettings-NextPageImageUrl="uri"
        PagerSettings-NextPageText="string"
        PagerSettings-PageButtonCount="integer"
        PagerSettings-Position="Bottom|Top|TopAndBottom"
        PagerSettings-PreviousPageImageUrl="uri"
        PagerSettings-PreviousPageText="string"
        PagerSettings-Visible="True|False"
        RenderTable="True|False"
        runat="server"
        SkinID="string"
        Style="string"
        TabIndex="integer"
        ToolTip="string"
        Visible="True|False"
        Width="size"
    >
            <EditItemTemplate>
                <!-- child controls -->
            </EditItemTemplate>
            <EditRowStyle />
            <EmptyDataRowStyle />
            <EmptyDataTemplate>
                <!-- child controls -->
            </EmptyDataTemplate>
            <FooterStyle />
            <FooterTemplate>
                <!-- child controls -->
            </FooterTemplate>
            <HeaderStyle />
            <HeaderTemplate>
                <!-- child controls -->
            </HeaderTemplate>
            <InsertItemTemplate>
                <!-- child controls -->
            </InsertItemTemplate>
            <InsertRowStyle />
            <ItemTemplate>
                <!-- child controls -->
            </ItemTemplate>
            <PagerSettings
                FirstPageImageUrl="uri"
                FirstPageText="string"
                LastPageImageUrl="uri"
                LastPageText="string"
                Mode="NextPrevious|Numeric|NextPreviousFirstLast|
                    NumericFirstLast"
                NextPageImageUrl="uri"
                NextPageText="string"
                OnPropertyChanged="PropertyChanged event handler"
                PageButtonCount="integer"
                Position="Bottom|Top|TopAndBottom"
                PreviousPageImageUrl="uri"
                PreviousPageText="string"
                Visible="True|False"
            />
            <PagerStyle />
            <PagerTemplate>
                <!-- child controls -->
            </PagerTemplate>
            <RowStyle />
    </asp:FormView>
    ```

=== "Blazor"

    ```razor
    <FormView
        DataSource=IEnumerable
        DefaultMode="ReadOnly|Edit|Insert"
        Caption=string
        CaptionAlign="NotSet|Top|Bottom|Left|Right"
        EmptyDataText=string
        HeaderText=string
        FooterText=string
        ItemType=Type
        ItemCommand=EventCallback<FormViewCommandEventArgs>
        ItemCreated=EventCallback
        ModeChanging=EventCallback<FormViewModeEventArgs>
        ModeChanged=EventCallback<FormViewModeEventArgs>
        OnDataBinding=EventCallBack
        OnDataBound=EventCallBack
        OnItemDeleting=EventCallBack<FormViewDeleteEventArgs>
        OnItemDeleted=EventCallBack<FormViewDeletedEventArgs>
        OnItemInserting=EventCallBack<FormViewInsertEventArgs>
        OnItemInserted=EventCallBack<FormViewInsertEventArgs>
        OnItemUpdating=EventCallBack<FormViewUpdateEventArgs>
        OnItemUpdated=EventCallBack<FormViewUpdatedEventArgs>
        PageIndexChanging=EventCallback<PageChangedEventArgs>
        PageIndexChanged=EventCallback<PageChangedEventArgs>
        SelectMethod=SelectHandler
        Visible=bool
    >
        <PagerSettings Mode="PagerButtons.Numeric"
                       NextPageText="string"
                       PreviousPageText="string"
                       Position="PagerPosition.Bottom" />
        <RowStyle BackColor=string ForeColor=string CssClass=string ... />
        <EditRowStyle BackColor=string ForeColor=string ... />
        <InsertRowStyle BackColor=string ForeColor=string ... />
        <HeaderStyle BackColor=string ForeColor=string ... />
        <FooterStyle BackColor=string ForeColor=string ... />
        <EmptyDataRowStyle BackColor=string ForeColor=string ... />
        <PagerStyle BackColor=string ForeColor=string ... />
        <HeaderTemplate>
            <!-- custom header content -->
        </HeaderTemplate>
        <ItemTemplate Context="Item">
            <!-- read-only display -->
        </ItemTemplate>
        <EditItemTemplate Context="Item">
            <!-- edit form -->
        </EditItemTemplate>
        <InsertItemTemplate Context="Item">
            <!-- insert form -->
        </InsertItemTemplate>
        <EmptyDataTemplate>
            <!-- content shown when data source is empty -->
        </EmptyDataTemplate>
        <PagerTemplate>
            <!-- custom pager content -->
        </PagerTemplate>
        <FooterTemplate>
            <!-- custom footer content -->
        </FooterTemplate>
    </FormView>
    ```

## Style Sub-Components

The FormView supports style sub-components that control the appearance of different sections. Each is specified as a child element:

| Style Component | Description |
|----------------|-------------|
| `RowStyle` | Styles the data row in ReadOnly mode |
| `EditRowStyle` | Styles the data row in Edit mode |
| `InsertRowStyle` | Styles the data row in Insert mode |
| `HeaderStyle` | Styles the header row |
| `FooterStyle` | Styles the footer row |
| `EmptyDataRowStyle` | Styles the empty data row |
| `PagerStyle` | Styles the pager row |

Each style component accepts standard style properties: `BackColor`, `ForeColor`, `CssClass`, `Font-Bold`, `Font-Italic`, `Font-Size`, `HorizontalAlign`, `VerticalAlign`, `Width`, `Height`, etc.

## Examples

### Basic FormView with Header, Footer, and Empty State

```razor
<FormView DataSource="@Employees"
          ItemType="Employee"
          HeaderText="Employee Details"
          FooterText="End of record"
          EmptyDataText="No employees found.">
    <ItemTemplate Context="Item">
        <p><strong>Name:</strong> @Item.Name</p>
        <p><strong>Title:</strong> @Item.Title</p>
    </ItemTemplate>
</FormView>

@code {
    private List<Employee> Employees = new()
    {
        new Employee { Name = "Jane Smith", Title = "Developer" }
    };

    public class Employee
    {
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
    }
}
```

### Custom Header and Footer Templates

```razor
<FormView DataSource="@Products" ItemType="Product">
    <HeaderTemplate>
        <div class="form-header">
            <h3>Product Information</h3>
            <hr />
        </div>
    </HeaderTemplate>
    <ItemTemplate Context="Item">
        <p><strong>Product:</strong> @Item.Name</p>
        <p><strong>Price:</strong> @Item.Price.ToString("C")</p>
    </ItemTemplate>
    <FooterTemplate>
        <div class="form-footer">
            <hr />
            <small>Last updated: @DateTime.Now.ToShortDateString()</small>
        </div>
    </FooterTemplate>
</FormView>

@code {
    private List<Product> Products = new()
    {
        new Product { Name = "Widget", Price = 9.99m }
    };

    public class Product
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }
}
```

### Empty Data Template

```razor
@* Shows custom content when no data is available *@
<FormView DataSource="@FilteredItems" ItemType="Order">
    <ItemTemplate Context="Item">
        <p>Order #@Item.Id — @Item.Description</p>
    </ItemTemplate>
    <EmptyDataTemplate>
        <div class="alert alert-info">
            <strong>No orders found.</strong> Try adjusting your search criteria.
        </div>
    </EmptyDataTemplate>
</FormView>

@code {
    private List<Order> FilteredItems = new(); // empty list

    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
    }
}
```

### Migration Example: Header and Footer

=== "Web Forms"

    ```aspx
    <asp:FormView ID="FormView1" runat="server"
                  HeaderText="Customer Record"
                  FooterText="End of record"
                  EmptyDataText="No customer data available.">
        <ItemTemplate>
            <p><%# Eval("Name") %></p>
        </ItemTemplate>
    </asp:FormView>
    ```

=== "Blazor"

    ```razor
    <FormView DataSource="@Customers"
              ItemType="Customer"
              HeaderText="Customer Record"
              FooterText="End of record"
              EmptyDataText="No customer data available.">
        <ItemTemplate Context="Item">
            <p>@Item.Name</p>
        </ItemTemplate>
    </FormView>
    ```

### Migration Example: With Templates

=== "Web Forms"

    ```aspx
    <asp:FormView ID="FormView1" runat="server">
        <HeaderTemplate>
            <h3>Customer Record</h3>
        </HeaderTemplate>
        <ItemTemplate>
            <p><%# Eval("Name") %></p>
        </ItemTemplate>
        <EmptyDataTemplate>
            <p>No data found.</p>
        </EmptyDataTemplate>
        <FooterTemplate>
            <small>End of record</small>
        </FooterTemplate>
    </asp:FormView>
    ```

=== "Blazor"

    ```razor
    <FormView DataSource="@Customers" ItemType="Customer">
        <HeaderTemplate>
            <h3>Customer Record</h3>
        </HeaderTemplate>
        <ItemTemplate Context="Item">
            <p>@Item.Name</p>
        </ItemTemplate>
        <EmptyDataTemplate>
            <p>No data found.</p>
        </EmptyDataTemplate>
        <FooterTemplate>
            <small>End of record</small>
        </FooterTemplate>
    </FormView>
    ```


### CRUD Event Handling

```razor
<FormView DataSource="@Customers" ItemType="Customer" DefaultMode="FormViewMode.ReadOnly"
          OnItemUpdating="HandleUpdating"
          OnItemUpdated="HandleUpdated"
          OnItemDeleting="HandleDeleting"
          ItemCommand="HandleCommand">
    <ItemTemplate Context="Item">
        <p>@Item.Name — @Item.Email</p>
    </ItemTemplate>
    <EditItemTemplate Context="Item">
        <p>Editing: @Item.Name</p>
    </EditItemTemplate>
</FormView>

@code {
    private List<Customer> Customers = new();

    private void HandleUpdating(FormViewUpdateEventArgs e)
    {
        // Validate and apply changes; set e.Cancel = true to abort
    }

    private void HandleUpdated(FormViewUpdatedEventArgs e)
    {
        // Post-update logic (e.g., refresh data, show notification)
    }

    private void HandleDeleting(FormViewDeleteEventArgs e)
    {
        // Perform delete; e.RowIndex gives the current position
    }

    private void HandleCommand(FormViewCommandEventArgs e)
    {
        // Fires for any command button — check e.CommandName
    }
}
```

## See Also

- [GridView](GridView.md) — For tabular data display
- [DetailsView](DetailsView.md) — Similar single-record display
- [DataList](DataList.md) — For repeating data templates
- [ListView](ListView.md) — Full-featured list with CRUD and paging
- [DataPager](DataPager.md) — Shared paging for multiple controls
- [PagerSettings](PagerSettings.md) — Shared pager configuration
