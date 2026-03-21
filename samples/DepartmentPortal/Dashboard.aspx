<%@ Page Title="Dashboard" Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="DepartmentPortal.DashboardPage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1>Dashboard</h1>
        <p class="lead">Welcome, <asp:Label ID="WelcomeNameLabel" runat="server" />!</p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <div class="widget">
                <h3><span class="glyphicon glyphicon-bullhorn"></span> Recent Announcements</h3>
                <asp:Repeater ID="RecentAnnouncementsRepeater" runat="server">
                    <ItemTemplate>
                        <div class="announcement-card">
                            <h4><%# Eval("Title") %></h4>
                            <small><%# Eval("PublishDate", "{0:MMM d, yyyy}") %> by <%# Eval("Author") %></small>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <a href="Announcements.aspx" class="btn btn-default btn-sm">View All</a>
            </div>
        </div>
        <div class="col-md-4">
            <div class="widget">
                <h3><span class="glyphicon glyphicon-education"></span> Training Courses</h3>
                <asp:Repeater ID="RecentCoursesRepeater" runat="server">
                    <ItemTemplate>
                        <div class="announcement-card">
                            <h4><%# Eval("CourseName") %></h4>
                            <small><%# Eval("Category") %> &middot; <%# Eval("DurationHours") %> hours</small>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <a href="Training.aspx" class="btn btn-default btn-sm">Browse Courses</a>
            </div>
        </div>
        <div class="col-md-4">
            <div class="widget">
                <h3><span class="glyphicon glyphicon-stats"></span> Quick Stats</h3>
                <div class="quick-stats">
                    <div class="stat-item">
                        <span class="stat-number"><asp:Label ID="StatEmployeesLabel" runat="server" /></span>
                        <span class="stat-label">Employees</span>
                    </div>
                    <div class="stat-item">
                        <span class="stat-number"><asp:Label ID="StatDeptLabel" runat="server" /></span>
                        <span class="stat-label">Departments</span>
                    </div>
                    <div class="stat-item">
                        <span class="stat-number"><asp:Label ID="StatCoursesLabel" runat="server" /></span>
                        <span class="stat-label">Courses</span>
                    </div>
                    <div class="stat-item">
                        <span class="stat-number"><asp:Label ID="StatResourcesLabel" runat="server" /></span>
                        <span class="stat-label">Resources</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
