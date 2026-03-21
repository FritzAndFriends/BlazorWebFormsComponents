<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchBox.ascx.cs" Inherits="DepartmentPortal.Controls.SearchBox" %>

<div class="search-box">
    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" />
    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-search" OnClick="btnSearch_Click" />
</div>
