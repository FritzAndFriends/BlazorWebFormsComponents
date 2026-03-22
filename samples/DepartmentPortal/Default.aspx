<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DepartmentPortal.DefaultPage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
        <h1>Department Portal</h1>
        <p class="lead">Welcome to the Contoso Corporation internal portal. Access employee information, announcements, training, and company resources.</p>
    </div>

    <asp:Panel ID="LoggedOutPanel" runat="server" Visible="true">
        <div class="row">
            <div class="col-md-6 col-md-offset-3 text-center">
                <h3>Please sign in to access the portal</h3>
                <a href="Login.aspx" class="btn btn-primary btn-lg">Sign In</a>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="LoggedInPanel" runat="server" Visible="false">
        <div class="row">
            <div class="col-md-4">
                <div class="section-panel">
                    <h3><span class="glyphicon glyphicon-user"></span> Employees</h3>
                    <p><asp:Label ID="EmployeeCountLabel" runat="server" /> employees in the directory.</p>
                    <a href="Employees.aspx" class="btn btn-default">View Directory</a>
                </div>
            </div>
            <div class="col-md-4">
                <div class="section-panel">
                    <h3><span class="glyphicon glyphicon-bullhorn"></span> Announcements</h3>
                    <p><asp:Label ID="AnnouncementCountLabel" runat="server" /> active announcements.</p>
                    <a href="Announcements.aspx" class="btn btn-default">View All</a>
                </div>
            </div>
            <div class="col-md-4">
                <div class="section-panel">
                    <h3><span class="glyphicon glyphicon-education"></span> Training</h3>
                    <p><asp:Label ID="CourseCountLabel" runat="server" /> courses available.</p>
                    <a href="Training.aspx" class="btn btn-default">Browse Courses</a>
                </div>
            </div>
        </div>
        <div class="row" style="margin-top: 20px;">
            <div class="col-md-12 text-center">
                <a href="Dashboard.aspx" class="btn btn-success btn-lg">Go to Dashboard</a>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
