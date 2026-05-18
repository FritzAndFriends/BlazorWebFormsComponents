<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentFilter.ascx.cs" Inherits="DepartmentPortal.Controls.DepartmentFilter" %>

<div class="department-filter">
    <asp:Label ID="lblDepartment" runat="server" Text="Department:" AssociatedControlID="ddlDepartments" CssClass="filter-label" />
    <asp:DropDownList ID="ddlDepartments" runat="server"
        CssClass="filter-dropdown"
        OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged">
    </asp:DropDownList>
</div>
