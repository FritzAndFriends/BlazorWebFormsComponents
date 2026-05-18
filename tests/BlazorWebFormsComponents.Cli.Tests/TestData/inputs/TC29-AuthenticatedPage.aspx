<%@ Page Title="Account Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
<%@ Import Namespace="System.Web.Security" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Account Dashboard</h1>
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <div class="alert alert-info">
                <p>Please <a href="~/Account/Login.aspx">sign in</a> to access your dashboard.</p>
                <asp:HyperLink NavigateUrl="~/Account/Register.aspx" CssClass="btn btn-primary" runat="server">
                    Create an Account
                </asp:HyperLink>
            </div>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3>Welcome, <asp:LoginName ID="lblUserName" FormatString="{0}" runat="server" />!</h3>
                </div>
                <div class="panel-body">
                    <p>You are logged in. <a href="~/Account/Manage.aspx">Manage your account</a>.</p>
                    <asp:Button ID="btnViewOrders" Text="View Orders" OnClick="ViewOrders_Click" CssClass="btn btn-default" runat="server" />
                </div>
            </div>
        </LoggedInTemplate>
        <RoleGroups>
            <asp:RoleGroup Roles="Admin">
                <ContentTemplate>
                    <div class="panel panel-danger">
                        <div class="panel-heading"><h3>Admin Dashboard</h3></div>
                        <div class="panel-body">
                            <asp:Button ID="btnManageUsers" Text="Manage Users" OnClick="ManageUsers_Click" CssClass="btn btn-danger" runat="server" />
                            <asp:HyperLink NavigateUrl="~/Admin/Reports.aspx" CssClass="btn btn-warning" runat="server">View Reports</asp:HyperLink>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
    </asp:LoginView>
</asp:Content>
