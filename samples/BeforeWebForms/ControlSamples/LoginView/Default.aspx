<%@ Page Title="LoginView Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.LoginView.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>LoginView Sample</h2>

    <h3>LoginView with Templates</h3>
    <div data-audit-control="LoginView-1">
        <asp:LoginView ID="LoginView1" runat="server">
            <AnonymousTemplate>
                <p>You are not logged in. Please <a href="#">sign in</a> to access member content.</p>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <p>Welcome back! You are logged in as <asp:LoginName ID="LoginName1" runat="server" />.</p>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
</asp:Content>
