<%@ Page Title="Login Test" Language="C#" %>
<asp:LoginView ID="LoginView1" runat="server">
    <AnonymousTemplate>
        <p>Please log in to continue.</p>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <p>Welcome back!</p>
    </LoggedInTemplate>
    <RoleGroups>
        <asp:RoleGroup Roles="Admin">
            <ContentTemplate>
                <p>Admin panel</p>
            </ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>
