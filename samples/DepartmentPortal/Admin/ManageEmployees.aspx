<%@ Page Title="Manage Employees" Language="C#" AutoEventWireup="true" CodeFile="ManageEmployees.aspx.cs" Inherits="DepartmentPortal.Admin.ManageEmployeesPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Manage Employees" Description="Employee administration and management" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Manage Employees" />
    
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Employee Directory</h3>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-6">
                            <uc:SearchBox ID="SearchBoxControl" runat="server" OnSearch="SearchBoxControl_Search" Placeholder="Search employees..." />
                        </div>
                        <div class="col-md-6 text-right">
                            <asp:Button ID="AddNewEmployeeButton" runat="server" Text="Add New Employee" CssClass="btn btn-primary" OnClick="AddNewEmployeeButton_Click" />
                        </div>
                    </div>
                    
                    <p class="text-muted">Total Employees: <asp:Label ID="EmployeeCountLabel" runat="server" /></p>
                    
                    <local:EmployeeDataGrid ID="EmployeeDataGridControl" runat="server" 
                        OnRowCommand="EmployeeDataGridControl_RowCommand" 
                        AutoGenerateColumns="false">
                    </local:EmployeeDataGrid>
                    
                    <asp:GridView ID="EmployeeGridView" runat="server" 
                        AutoGenerateColumns="false" 
                        CssClass="table table-striped table-hover" 
                        OnRowCommand="EmployeeGridView_RowCommand"
                        DataKeyNames="Id">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" />
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Title" HeaderText="Title" />
                            <asp:BoundField DataField="Phone" HeaderText="Phone" />
                            <asp:BoundField DataField="HireDate" HeaderText="Hire Date" DataFormatString="{0:MM/dd/yyyy}" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CommandName="EditEmployee" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-default">
                                        <span class="glyphicon glyphicon-edit"></span> Edit
                                    </asp:LinkButton>
                                    <asp:HyperLink runat="server" NavigateUrl='<%# "~/EmployeeDetail.aspx?id=" + Eval("Id") %>' CssClass="btn btn-sm btn-info">
                                        <span class="glyphicon glyphicon-eye-open"></span> View
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            
            <asp:Panel ID="EditEmployeePanel" runat="server" Visible="false" CssClass="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <asp:Label ID="EditEmployeePanelTitle" runat="server" Text="Add Employee" />
                    </h3>
                </div>
                <div class="panel-body">
                    <asp:HiddenField ID="EditEmployeeId" runat="server" Value="0" />
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Name</label>
                                <asp:TextBox ID="NameTextBox" runat="server" CssClass="form-control" MaxLength="100" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="NameTextBox" ErrorMessage="Name is required" CssClass="text-danger" Display="Dynamic" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Email</label>
                                <asp:TextBox ID="EmailTextBox" runat="server" TextMode="Email" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic" />
                            </div>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Title</label>
                                <asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Phone</label>
                                <asp:TextBox ID="PhoneTextBox" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Department</label>
                                <asp:DropDownList ID="DepartmentDropDownList" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Hire Date</label>
                                <asp:TextBox ID="HireDateTextBox" runat="server" TextMode="Date" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                    
                    <asp:Button ID="SaveEmployeeButton" runat="server" Text="Save" CssClass="btn btn-success" OnClick="SaveEmployeeButton_Click" />
                    <asp:Button ID="CancelEmployeeButton" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="CancelEmployeeButton_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
