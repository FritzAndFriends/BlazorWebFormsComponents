# Control Translation Reference

Reference tables for mapping ASP.NET Web Forms server controls to their BWFC Blazor equivalents.

**Parent skill:** `SKILL.md` (bwfc-migration)

---

## Control Translation Table

### Simple Controls (Trivial Migration)

| Web Forms | BWFC | Changes |
|-----------|------|---------|
| `<asp:Label ID="x" runat="server" Text="Hello" CssClass="title" />` | `<Label @ref="x" Text="Hello" CssClass="title" />` | Remove `asp:`, `runat`; `ID` → `@ref` (if referenced) |
| `<asp:Literal ID="x" runat="server" Text="Hello" />` | `<Literal Text="Hello" />` | Remove `asp:`, `runat` |
| `<asp:HyperLink NavigateUrl="~/About" Text="About" runat="server" />` | `<HyperLink NavigateUrl="/About" Text="About" />` | `~/` → `/` |
| `<asp:Image ImageUrl="~/images/logo.png" runat="server" />` | `<Image ImageUrl="/images/logo.png" />` | `~/` → `/` |
| `<asp:Panel CssClass="container" runat="server">` | `<Panel CssClass="container">` | Remove `asp:`, `runat` |
| `<asp:PlaceHolder runat="server">` | `<PlaceHolder>` | Remove `asp:`, `runat` |
| `<asp:HiddenField Value="x" runat="server" />` | `<HiddenField Value="x" />` | Remove `asp:`, `runat` |

### Form Controls (Easy Migration)

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:TextBox ID="Name" runat="server" />` | `<TextBox @bind-Text="model.Name" />` | Add `@bind-Text` |
| `<asp:TextBox TextMode="Password" runat="server" />` | `<TextBox TextMode="Password" @bind-Text="model.Password" />` | TextMode preserved |
| `<asp:TextBox TextMode="MultiLine" Rows="5" runat="server" />` | `<TextBox TextMode="Multiline" Rows="5" @bind-Text="model.Notes" />` | Note: `Multiline` not `MultiLine` |
| `<asp:DropDownList ID="Category" runat="server" />` | `<DropDownList @bind-SelectedValue="model.Category" Items="categories" />` | Bind items + selected value |
| `<asp:CheckBox ID="Active" runat="server" Checked="true" />` | `<CheckBox @bind-Checked="model.Active" />` | `@bind-Checked` |
| `<asp:RadioButton GroupName="G" runat="server" />` | `<RadioButton GroupName="G" />` | Same attributes |
| `<asp:FileUpload ID="Upload" runat="server" />` | `<FileUpload />` | Uses `InputFile` internally |
| `<asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />` | `<Button Text="Submit" OnClick="Submit_Click" />` | `OnClick` is `EventCallback` |
| `<asp:LinkButton Text="Edit" CommandName="Edit" runat="server" />` | `<LinkButton Text="Edit" CommandName="Edit" />` | Same attributes |
| `<asp:ImageButton ImageUrl="~/btn.png" OnClick="Btn_Click" runat="server" />` | `<ImageButton ImageUrl="/btn.png" OnClick="Btn_Click" />` | `~/` → `/` |

### Validation Controls (Easy Migration)

| Web Forms | BWFC |
|-----------|------|
| `<asp:RequiredFieldValidator ControlToValidate="Name" ErrorMessage="Required" runat="server" />` | `<RequiredFieldValidator ControlToValidate="Name" ErrorMessage="Required" />` |
| `<asp:CompareValidator ControlToCompare="Password" ControlToValidate="Confirm" runat="server" />` | `<CompareValidator ControlToCompare="Password" ControlToValidate="Confirm" />` |
| `<asp:RangeValidator MinimumValue="1" MaximumValue="100" Type="Integer" runat="server" />` | `<RangeValidator MinimumValue="1" MaximumValue="100" Type="Integer" />` |
| `<asp:RegularExpressionValidator ValidationExpression="\d+" runat="server" />` | `<RegularExpressionValidator ValidationExpression="\d+" />` |
| `<asp:CustomValidator OnServerValidate="Validate" runat="server" />` | `<CustomValidator OnServerValidate="Validate" />` |
| `<asp:ValidationSummary DisplayMode="BulletList" runat="server" />` | `<ValidationSummary DisplayMode="BulletList" />` |
| `<asp:ModelErrorMessage ModelStateKey="key" runat="server" />` | `<ModelErrorMessage ModelStateKey="key" />` |

### Data Controls (Medium Migration)

#### GridView

```xml
<!-- Web Forms -->
<asp:GridView ID="ProductGrid" runat="server"
    ItemType="WingtipToys.Models.Product"
    SelectMethod="GetProducts"
    AutoGenerateColumns="false"
    AllowPaging="true" PageSize="10">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Product" />
        <asp:TemplateField HeaderText="Price">
            <ItemTemplate><%#: Item.UnitPrice.ToString("C") %></ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
```

```razor
<!-- Blazor with BWFC -->
<GridView SelectMethod="@productService.GetProducts" ItemType="Product"
    AutoGenerateColumns="false"
    AllowPaging="true" PageSize="10">
    <Columns>
        <BoundField DataField="Name" HeaderText="Product" />
        <TemplateField HeaderText="Price">
            <ItemTemplate Context="Item">@Item.UnitPrice.ToString("C")</ItemTemplate>
        </TemplateField>
    </Columns>
</GridView>
```

**Key changes:** `ItemType` preserved as-is, `SelectMethod` preserved — convert string method name to `SelectHandler<ItemType>` delegate, add `Context="Item"` to templates.

#### ListView

```xml
<!-- Web Forms -->
<asp:ListView ID="ProductList" runat="server"
    ItemType="WingtipToys.Models.Product" SelectMethod="GetProducts">
    <ItemTemplate>
        <div class="product">
            <h3><%#: Item.ProductName %></h3>
            <asp:Image ImageUrl="<%#: Item.ImagePath %>" runat="server" />
            <p><%#: Item.UnitPrice.ToString("C") %></p>
        </div>
    </ItemTemplate>
</asp:ListView>
```

```razor
<!-- Blazor with BWFC -->
<ListView SelectMethod="@productService.GetProducts" ItemType="Product">
    <ItemTemplate Context="Item">
        <div class="product">
            <h3>@Item.ProductName</h3>
            <Image ImageUrl="@Item.ImagePath" />
            <p>@Item.UnitPrice.ToString("C")</p>
        </div>
    </ItemTemplate>
</ListView>
```

#### ListView with GroupItemCount

Web Forms `ListView` supports `GroupItemCount` for grid-style layouts (e.g., 4 products per row). BWFC preserves this attribute and the `GroupTemplate`/`LayoutTemplate` structure.

```xml
<!-- Web Forms — ListView with GroupItemCount for 4-column grid -->
<asp:ListView ID="productList" runat="server"
    ItemType="WingtipToys.Models.Product" SelectMethod="GetProducts"
    GroupItemCount="4">
    <LayoutTemplate>
        <table><tr runat="server" id="groupPlaceholder" /></table>
    </LayoutTemplate>
    <GroupTemplate>
        <tr><td runat="server" id="itemPlaceholder" /></tr>
    </GroupTemplate>
    <ItemTemplate>
        <td>
            <a href="<%#: GetRouteUrl("ProductRoute", new { productId = Item.ProductID }) %>">
                <img src="<%#: Item.ImagePath %>" alt="<%#: Item.ProductName %>" />
            </a>
            <span><%#: Item.ProductName %></span>
            <span><%#: Item.UnitPrice.ToString("C") %></span>
        </td>
    </ItemTemplate>
</asp:ListView>
```

```razor
@* Blazor — BWFC ListView preserves GroupItemCount and templates *@
<ListView SelectMethod="@productService.GetProducts" ItemType="Product" GroupItemCount="4">
    <LayoutTemplate>@context</LayoutTemplate>
    <GroupTemplate>@context</GroupTemplate>
    <ItemTemplate>
        <td>
            <a href="@($"/Products/{context.ProductID}")">
                <img src="@context.ImagePath" alt="@context.ProductName" />
            </a>
            <span>@context.ProductName</span>
            <span>@context.UnitPrice.ToString("C")</span>
        </td>
    </ItemTemplate>
</ListView>
```

**Key changes:** `GroupItemCount` preserved as-is. `LayoutTemplate` and `GroupTemplate` use `@context` as the placeholder (BWFC renders the table/tr structure). `ItemTemplate` uses `@context.Property` instead of `<%#: Item.Property %>`.

#### FormView

```razor
<!-- Blazor with BWFC -->
<FormView DataItem="product" ItemType="Product" RenderOuterTable="false">
    <ItemTemplate Context="Item">
        <h2>@Item.ProductName</h2>
        <p>@Item.Description</p>
        <p>Price: @Item.UnitPrice.ToString("C")</p>
    </ItemTemplate>
</FormView>
```

**Key changes:** `SelectMethod` preserved as delegate for both single records and collections. For `FormView` with a single record, alternatively use `DataItem="product"` with data loaded in `OnInitializedAsync`.

### Navigation Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Menu>` | `<Menu>` | Same structure, same sub-elements |
| `<asp:TreeView>` | `<TreeView>` | Same node structure |
| `<asp:SiteMapPath>` | `<SiteMapPath>` | Same attributes |

### AJAX Controls (No-Op Migration)

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:ScriptManager runat="server" />` | `<ScriptManager />` | Renders nothing — migration compatibility |
| `<asp:ScriptManagerProxy runat="server" />` | `<ScriptManagerProxy />` | Renders nothing |
| `<asp:UpdatePanel runat="server">` | `<UpdatePanel>` | Passes through children; supports `<ContentTemplate>` as a render fragment |
| `<asp:UpdateProgress runat="server">` | `<UpdateProgress>` | Passes through children |
| `<asp:Timer Interval="5000" runat="server" />` | `<Timer Interval="5000" />` | Fires OnTick events |

#### UpdatePanel with ContentTemplate

Web Forms `<asp:UpdatePanel>` supports `<ContentTemplate>` to wrap content. BWFC now provides a `ContentTemplate` RenderFragment parameter, enabling L1 migration to output clean Blazor markup with no RZ10012 warnings.

```xml
<!-- Web Forms -->
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />
        <asp:Label Text="Result" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
```

```razor
<!-- Blazor with BWFC (L1 output) — now compiles cleanly -->
<UpdatePanel>
    <ContentTemplate>
        <Button Text="Submit" OnClick="Submit_Click" />
        <Label Text="Result" />
    </ContentTemplate>
</UpdatePanel>
```

**Key points:**
- `<ContentTemplate>` is now recognized as a BWFC child element — no warnings or errors
- UpdatePanel inherits `BaseStyledComponent` (supports `BackColor`, `ForeColor`, `CssClass`, `Style`, etc.)
- UpdatePanel supports `RenderMode` enum values (`Block` for `<div>`, `Inline` for `<span>`) — set at the component level
- Render mode is determined by the application's global `@rendermode` directive in `App.razor`, not by UpdatePanel — UpdatePanel does NOT force InteractiveServer
- Both `RenderMode` property and render fragment are optional — use as needed for your migration

---

## Component Coverage Summary

**58 components** across 6 categories:

| Category | Count | Components |
|----------|-------|-----------|
| **Editor Controls** | 28 | AdRotator, BulletedList, Button, Calendar, Chart, CheckBox, CheckBoxList, DropDownList, FileUpload, HiddenField, HyperLink, Image, ImageButton, ImageMap, Label, LinkButton, ListBox, Literal, Localize, MultiView, Panel, PlaceHolder, RadioButton, RadioButtonList, Substitution, Table, TextBox, View |
| **Data Controls** | 8 | DataGrid, DataList, DataPager, DetailsView, FormView, GridView, ListView, Repeater |
| **Validation** | 7 | CompareValidator, CustomValidator, ModelErrorMessage, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary |
| **Navigation** | 3 | Menu, SiteMapPath, TreeView |
| **Login** | 7 | ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery |
| **AJAX** | 5 | ScriptManager, ScriptManagerProxy, Timer, UpdatePanel, UpdateProgress |

### Page Base Class

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `System.Web.UI.Page` (base class) | `WebFormsPageBase` | `@inherits WebFormsPageBase` in `_Imports.razor`; `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`, `IsPostBack` work unchanged |

### Structural & Infrastructure Components

These BWFC components support the migration infrastructure — they aren't direct control replacements but provide essential bridging capabilities:

| Component | Purpose | Usage |
|-----------|---------|-------|
| `WebFormsPage` | Unified layout wrapper combining NamingContainer + ThemeProvider + head rendering | `<WebFormsPage>@Body</WebFormsPage>` in `MainLayout.razor` — single component replaces multiple wrappers |
| `Page` | Standalone head renderer (renders `<PageTitle>` + `<meta>` from `IPageService`) | `<BlazorWebFormsComponents.Page />` in layout — use if you don't need `WebFormsPage` |
| `NamingContainer` | Establishes naming scope for child component IDs (emulates `INamingContainer`) | `<NamingContainer ID="MainContent">` — children get prefixed IDs like `MainContent_ButtonName` |
| `MasterPage` | Emulates Web Forms MasterPage with ContentPlaceHolder support | Gradual migration path — allows preserving `<MasterPage>` / `<Content>` / `<ContentPlaceHolder>` markup |
| `Content` | Provides content for a `ContentPlaceHolder` in a `MasterPage` | `<Content ContentPlaceHolderID="MainContent">...</Content>` |
| `ContentPlaceHolder` | Defines a replaceable region in a `MasterPage` | `<ContentPlaceHolder ID="MainContent" />` |
| `EmptyLayout` | Minimal layout component (`@inherits LayoutComponentBase` + `@Body`) | Use for pages that need no layout chrome |

### DataBinder.Eval Compatibility Shim

BWFC provides a `DataBinder` static class for legacy code that uses `DataBinder.Eval()`. This is marked `[Obsolete]` — use direct property access (`@context.Property`) instead.

```csharp
// Web Forms — DataBinder.Eval
<%# DataBinder.Eval(Container.DataItem, "ProductName") %>

// BWFC shim (compiles, but marked obsolete)
@DataBinder.Eval(context, "ProductName")

// Recommended — direct property access
@context.ProductName
```

### Theming Infrastructure

BWFC includes a theming system for migrating Web Forms skin files:

| Component | Purpose |
|-----------|---------|
| `ThemeProvider` (in `Theming/`) | Cascades `ThemeConfiguration` to child components |
| `ThemeConfiguration` | Defines theme settings (skin mappings) |
| `ControlSkin` / `SkinBuilder` | Internal — apply skin properties to controls |

> **Note:** Theming is an advanced feature. Most migrations can ignore it initially and add theming later if needed.

### Custom Control Base Classes

For migrating custom Web Forms controls that extend `System.Web.UI.WebControls.WebControl`:

| BWFC Class | Purpose |
|-----------|---------|
| `WebControl` (in `CustomControls/`) | Base class shim for custom controls |
| `CompositeControl` | Base class for controls that contain child controls |
| `HtmlTextWriter` | Shim for `Render(HtmlTextWriter)` patterns — use for gradual migration |

### Not Covered by BWFC

| Control | Alternative |
|---------|------------|
| `SqlDataSource` | Injected service + EF Core (see `/bwfc-data-migration`) |
| `ObjectDataSource` | Injected service |
| `EntityDataSource` | Injected service + EF Core |
| `Wizard` | Multi-step form with component state |
| `Web Parts` | Redesign as Blazor components |

### Ajax Control Toolkit Extenders

**Ajax Control Toolkit extenders are now supported** by the companion **BlazorAjaxToolkitComponents** package. The 14 supported extender and container components are:

| Component | Type |
|-----------|------|
| `Accordion` / `AccordionPane` | Container — Collapsible panes |
| `AutoCompleteExtender` | Extender — Typeahead suggestions |
| `CalendarExtender` | Extender — Popup date picker |
| `CollapsiblePanelExtender` | Extender — Collapse/expand animation |
| `ConfirmButtonExtender` | Extender — Browser confirmation dialog |
| `FilteredTextBoxExtender` | Extender — Character filtering |
| `HoverMenuExtender` | Extender — Hover-triggered popup |
| `MaskedEditExtender` | Extender — Input mask formatting |
| `ModalPopupExtender` | Extender — Modal dialog with overlay |
| `NumericUpDownExtender` | Extender — Numeric spinner |
| `PopupControlExtender` | Extender — Click-triggered popup |
| `SliderExtender` | Extender — Range slider |
| `TabContainer` / `TabPanel` | Container — Tabbed panels |
| `ToggleButtonExtender` | Extender — Image-based checkbox toggle |

**Migration is mechanical:** Strip the `ajaxToolkit:` prefix, remove `runat="server"`, remove `<ajaxToolkit:ToolkitScriptManager>`, and preserve all other properties. For details on installation, Layer 1 automation, and Layer 2 manual work (ServiceMethod wiring for AutoCompleteExtender), see **[AJAX-TOOLKIT.md](AJAX-TOOLKIT.md)**.

Unsupported Ajax Control Toolkit controls (e.g., `DragPanelExtender`, `ResizableControlExtender`) require manual JavaScript interop replacement — see AJAX-TOOLKIT.md for strategies.
