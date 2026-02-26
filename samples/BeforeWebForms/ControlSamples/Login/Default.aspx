<%@ Page Title="Login Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Login.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Login Sample</h2>

    <h3>Default Login Control</h3>
    <div data-audit-control="Login-1">
        <asp:Login ID="Login1" runat="server" />
    </div>

    <h3>Styled Login Control</h3>
    <div data-audit-control="Login-2">
        <asp:Login ID="Login2" runat="server"
            TitleText="Member Login"
            UserNameLabelText="Username:"
            PasswordLabelText="Password:"
            LoginButtonText="Sign In"
            RememberMeText="Keep me logged in"
            FailureText="Login failed. Please try again."
            BorderWidth="1"
            BorderColor="Gray"
            BorderStyle="Solid"
            BackColor="#F7F6F3"
            Font-Names="Verdana"
            Font-Size="10pt">
            <TitleTextStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="true" Font-Size="13pt" />
            <LoginButtonStyle BackColor="#FFFBFF" ForeColor="#284775" BorderColor="#CCCCCC" BorderWidth="1" BorderStyle="Solid" Font-Names="Verdana" Font-Size="10pt" />
        </asp:Login>
    </div>
</asp:Content>
