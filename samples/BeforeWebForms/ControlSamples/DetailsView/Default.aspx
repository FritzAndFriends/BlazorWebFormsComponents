<%@ Page Title="DetailsView Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DetailsView.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DetailsView Control</h2>

    <p>A DetailsView displays a single record at a time from a data source, with built-in paging to navigate between records.</p>

    <h3>Auto-Generated Rows</h3>
    <p>When AutoGenerateRows is true (the default), the DetailsView creates a row for each field in the data source.</p>

    <div data-audit-control="DetailsView">
    <asp:DetailsView ID="AutoRowsDetailsView"
        runat="server"
        AutoGenerateRows="true"
        AllowPaging="true"
        OnPageIndexChanging="AutoRowsDetailsView_PageIndexChanging"
        HeaderText="Product Details (Auto Rows)"
        BackColor="White"
        BorderColor="#999999"
        BorderWidth="1px"
        CellPadding="4"
        GridLines="Vertical"
        Width="400px">
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#F7F7DE" />
        <PagerStyle BackColor="#507CD1" ForeColor="White" HorizontalAlign="Center" />
        <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" FirstPageText="First" LastPageText="Last" />
    </asp:DetailsView>
    </div>

    <h3>Explicit Fields with BoundField</h3>
    <p>Use Fields to control which columns appear, their order, and formatting.</p>

    <div data-audit-control="DetailsView">
    <asp:DetailsView ID="ExplicitFieldsDetailsView"
        runat="server"
        AutoGenerateRows="false"
        AllowPaging="true"
        OnPageIndexChanging="ExplicitFieldsDetailsView_PageIndexChanging"
        HeaderText="Product Details (Explicit Fields)"
        BackColor="White"
        BorderColor="#CCCCCC"
        BorderWidth="1px"
        CellPadding="4"
        Width="400px">
        <Fields>
            <asp:BoundField DataField="Name" HeaderText="Product Name" />
            <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
            <asp:BoundField DataField="Category" HeaderText="Category" />
            <asp:BoundField DataField="InStock" HeaderText="In Stock" />
        </Fields>
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <FieldHeaderStyle BackColor="#E8E8E8" Font-Bold="True" Width="120px" />
        <RowStyle BackColor="White" />
        <AlternatingRowStyle BackColor="#F0F0F0" />
        <PagerStyle BackColor="#006699" ForeColor="White" HorizontalAlign="Center" />
        <PagerSettings Mode="NextPreviousFirstLast"
            FirstPageText="&laquo; First"
            LastPageText="Last &raquo;"
            NextPageText="Next &gt;"
            PreviousPageText="&lt; Prev" />
    </asp:DetailsView>
    </div>

</asp:Content>
