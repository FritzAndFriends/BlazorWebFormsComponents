<%@ Page Title="LoginStatus Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.LoginStatus.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>LoginStatus Sample</h2>

    <h3>Default LoginStatus</h3>
    <div data-audit-control="LoginStatus-1">
        <asp:LoginStatus ID="LoginStatus1" runat="server" />
    </div>

    <h3>LoginStatus with Custom Text</h3>
    <div data-audit-control="LoginStatus-2">
        <asp:LoginStatus ID="LoginStatus2" runat="server"
            LoginText="Sign In"
            LogoutText="Sign Out"
            LogoutAction="Refresh" />
    </div>
</asp:Content>
