<%@ Page Title="Employee Details" Language="C#" AutoEventWireup="true" CodeBehind="EmployeeDetail.aspx.cs" Inherits="DepartmentPortal.EmployeeDetailPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Employee Details" />
    
    <asp:Panel ID="EmployeeDetailsPanel" runat="server" Visible="false">
        <div class="row">
            <div class="col-md-8">
                <local:EmployeeCard ID="EmployeeCardControl" runat="server" />
                
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Contact Information</h3>
                    </div>
                    <div class="panel-body">
                        <dl class="dl-horizontal">
                            <dt>Email:</dt>
                            <dd><asp:Label ID="EmailLabel" runat="server" /></dd>
                            <dt>Phone:</dt>
                            <dd><asp:Label ID="PhoneLabel" runat="server" /></dd>
                            <dt>Department:</dt>
                            <dd><asp:Label ID="DepartmentLabel" runat="server" /></dd>
                            <dt>Hire Date:</dt>
                            <dd><asp:Label ID="HireDateLabel" runat="server" /></dd>
                        </dl>
                    </div>
                </div>
                
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Performance Rating</h3>
                    </div>
                    <div class="panel-body">
                        <local:StarRating ID="PerformanceRatingControl" runat="server" MaxRating="5" />
                    </div>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Quick Actions</h3>
                    </div>
                    <div class="panel-body">
                        <asp:HyperLink ID="SendEmailLink" runat="server" CssClass="btn btn-primary btn-block" Text="Send Email" />
                        <asp:HyperLink ID="ViewScheduleLink" runat="server" CssClass="btn btn-default btn-block" Text="View Schedule" NavigateUrl="#" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false">
        <div class="alert alert-warning">
            <strong>Employee not found.</strong> The employee you are looking for does not exist.
        </div>
        <asp:HyperLink runat="server" NavigateUrl="~/Employees.aspx" CssClass="btn btn-default">
            <span class="glyphicon glyphicon-arrow-left"></span> Back to Employee Directory
        </asp:HyperLink>
    </asp:Panel>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
