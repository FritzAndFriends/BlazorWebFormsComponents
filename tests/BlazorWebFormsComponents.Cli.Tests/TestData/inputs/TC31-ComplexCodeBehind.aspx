<%@ Page Title="Admin Dashboard" Language="C#" AutoEventWireup="true" CodeBehind="TC31-ComplexCodeBehind.aspx.cs" Inherits="ContosoAdmin.Dashboard" %>
<h1>Admin Dashboard</h1>
<asp:Label ID="lblStatus" Text="Loading..." runat="server" />
<asp:GridView ID="gvUsers" AutoGenerateColumns="False" CssClass="table" GridLines="Both"
    OnRowCommand="Users_RowCommand" runat="server">
    <Columns>
        <asp:BoundField DataField="UserName" HeaderText="User" />
        <asp:BoundField DataField="Email" HeaderText="Email" />
    </Columns>
</asp:GridView>
<asp:Button ID="btnExport" Text="Export Report" OnClick="Export_Click" runat="server" />
<asp:Button ID="btnRefresh" Text="Refresh" OnClick="Refresh_Click" runat="server" />
