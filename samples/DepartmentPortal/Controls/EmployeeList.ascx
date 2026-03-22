<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeList.ascx.cs" Inherits="DepartmentPortal.Controls.EmployeeList" %>

<div class="employee-list">
    <asp:GridView ID="gvEmployees" runat="server"
        AutoGenerateColumns="false"
        CssClass="table employee-grid"
        AllowPaging="true"
        OnPageIndexChanging="gvEmployees_PageIndexChanging"
        EmptyDataText="No employees found.">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Title" HeaderText="Title" />
            <asp:BoundField DataField="Department" HeaderText="Department" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:BoundField DataField="Phone" HeaderText="Phone" />
        </Columns>
    </asp:GridView>
</div>
