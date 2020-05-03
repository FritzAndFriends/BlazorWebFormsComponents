# FormView

The FormView component is meant to emulate the asp:FormView control in markup and is defined in the [System.Web.UI.WebControls.FormView class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.formview?view=netframework-4.8)

[Usage Notes](#usage-notes) | [Web Forms Syntax](#web-forms-declarative-syntax) | [Blazor Syntax](#blazor-syntax)

## Features supported in Blazor

  - Readonly Form
  - Numerical Pager
  - OnDataBinding and OnDataBound events trigger

##### [Back to top](#formview)

## Usage Notes



## Web Forms Declarative Syntax

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

##### [Back to top](#formview)

## Blazor Syntax

##### [Back to top](#formview)
