<%@ Page Title="CreateUserWizard Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.CreateUserWizard.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>CreateUserWizard Sample</h2>

    <h3>Default CreateUserWizard</h3>
    <div data-audit-control="CreateUserWizard-1">
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" />
    </div>

    <h3>Styled CreateUserWizard</h3>
    <div data-audit-control="CreateUserWizard-2">
        <asp:CreateUserWizard ID="CreateUserWizard2" runat="server"
            CreateUserButtonText="Register"
            CompleteSuccessText="Your account has been created."
            RequireEmail="true"
            BorderWidth="1"
            BorderColor="Gray"
            BorderStyle="Solid"
            BackColor="#F7F6F3"
            Font-Names="Verdana"
            Font-Size="10pt">
            <TitleTextStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="true" />
        </asp:CreateUserWizard>
    </div>
</asp:Content>
