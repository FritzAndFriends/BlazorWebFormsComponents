<%@ Page Title="PasswordRecovery Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.PasswordRecovery.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>PasswordRecovery Sample</h2>

    <h3>Default PasswordRecovery</h3>
    <div data-audit-control="PasswordRecovery-1">
        <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" />
    </div>

    <h3>Styled PasswordRecovery</h3>
    <div data-audit-control="PasswordRecovery-2">
        <asp:PasswordRecovery ID="PasswordRecovery2" runat="server"
            UserNameTitleText="Forgot Password"
            UserNameLabelText="Enter your username:"
            SubmitButtonText="Recover Password"
            BorderWidth="1"
            BorderColor="Gray"
            BorderStyle="Solid"
            BackColor="#F7F6F3"
            Font-Names="Verdana"
            Font-Size="10pt">
            <TitleTextStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="true" />
        </asp:PasswordRecovery>
    </div>
</asp:Content>
