<%@ Page Title="Table Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Table.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Table Sample</h2>

    <h3>Declarative Table</h3>
    <div data-audit-control="Table-1">
        <asp:Table ID="Table1" runat="server"
            GridLines="Both"
            CellPadding="5"
            CellSpacing="0"
            BorderWidth="1"
            BorderColor="Black">
            <asp:TableHeaderRow>
                <asp:TableHeaderCell>Name</asp:TableHeaderCell>
                <asp:TableHeaderCell>Category</asp:TableHeaderCell>
                <asp:TableHeaderCell>Price</asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell>Widget A</asp:TableCell>
                <asp:TableCell>Hardware</asp:TableCell>
                <asp:TableCell>$9.99</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Widget B</asp:TableCell>
                <asp:TableCell>Software</asp:TableCell>
                <asp:TableCell>$19.99</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>Widget C</asp:TableCell>
                <asp:TableCell>Hardware</asp:TableCell>
                <asp:TableCell>$14.99</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

    <h3>Styled Table</h3>
    <div data-audit-control="Table-2">
        <asp:Table ID="Table2" runat="server"
            GridLines="Horizontal"
            CellPadding="8"
            BackColor="#FFFFCC"
            BorderWidth="1"
            BorderColor="#999999"
            CssClass="styled-table">
            <asp:TableHeaderRow BackColor="#5D7B9D" ForeColor="White">
                <asp:TableHeaderCell>ID</asp:TableHeaderCell>
                <asp:TableHeaderCell>Product</asp:TableHeaderCell>
                <asp:TableHeaderCell>In Stock</asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell>1</asp:TableCell>
                <asp:TableCell>Alpha</asp:TableCell>
                <asp:TableCell>Yes</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow BackColor="#F7F6F3">
                <asp:TableCell>2</asp:TableCell>
                <asp:TableCell>Beta</asp:TableCell>
                <asp:TableCell>No</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>3</asp:TableCell>
                <asp:TableCell>Gamma</asp:TableCell>
                <asp:TableCell>Yes</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

    <h3>Programmatic Table</h3>
    <div data-audit-control="Table-3">
        <asp:Table ID="Table3" runat="server"
            GridLines="Both"
            CellPadding="5"
            BorderWidth="1" />
    </div>
</asp:Content>
