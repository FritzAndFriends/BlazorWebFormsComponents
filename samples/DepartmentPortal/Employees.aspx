<%@ Page Title="Employee Directory" Language="C#" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="DepartmentPortal.EmployeesPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DepartmentFilter.ascx" TagName="DepartmentFilter" TagPrefix="uc" %>
<%@ Register Src="~/Controls/EmployeeList.ascx" TagName="EmployeeList" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Pager.ascx" TagName="Pager" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Employee Directory" Description="Find and connect with your colleagues" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Employees" />
    
    <div class="row">
        <div class="col-md-3">
            <uc:SearchBox ID="SearchBoxControl" runat="server" OnSearch="SearchBoxControl_Search" Placeholder="Search employees..." />
        </div>
        <div class="col-md-3">
            <uc:DepartmentFilter ID="DepartmentFilterControl" runat="server" OnDepartmentChanged="DepartmentFilterControl_DepartmentChanged" />
        </div>
    </div>
    
    <div class="employee-grid-container">
        <h3>Employees (<asp:Label ID="EmployeeCountLabel" runat="server" />)</h3>
        <local:EmployeeDataGrid ID="EmployeeDataGridControl" runat="server" />
        
        <uc:EmployeeList ID="EmployeeListControl" runat="server" />
    </div>
    
    <uc:Pager ID="PagerControl" runat="server" OnPageChanged="PagerControl_PageChanged" />
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
