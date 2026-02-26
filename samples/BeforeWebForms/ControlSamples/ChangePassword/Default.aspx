<%@ Page Title="ChangePassword Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ChangePassword.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>ChangePassword Sample</h2>

    <h3>Default ChangePassword Control</h3>
    <div data-audit-control="ChangePassword-1">
        <asp:ChangePassword ID="ChangePassword1" runat="server" />
    </div>

    <h3>Styled ChangePassword Control</h3>
    <div data-audit-control="ChangePassword-2">
        <asp:ChangePassword ID="ChangePassword2" runat="server"
            ChangePasswordTitleText="Update Your Password"
            ChangePasswordButtonText="Update Password"
            CancelButtonText="Cancel"
            ConfirmNewPasswordLabelText="Confirm New Password:"
            NewPasswordLabelText="New Password:"
            PasswordLabelText="Current Password:"
            BorderWidth="1"
            BorderColor="Gray"
            BorderStyle="Solid"
            BackColor="#F7F6F3"
            Font-Names="Verdana"
            Font-Size="10pt">
            <TitleTextStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="true" />
        </asp:ChangePassword>
    </div>
</asp:Content>
