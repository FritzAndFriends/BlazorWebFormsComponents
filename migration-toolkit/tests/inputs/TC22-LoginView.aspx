<%@ Page Title="Login View Test" Language="C#" %>
<asp:LoginView ID="LoginView1" runat="server">
    <AnonymousTemplate>
        <p>You are not logged in.</p>
        <a href="~/Login.aspx">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <p>Welcome back!</p>
    </LoggedInTemplate>
    <RoleGroups>
        <asp:RoleGroup Roles="Admin">
            <ContentTemplate>
                <span>Admin Panel</span>
            </ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>
