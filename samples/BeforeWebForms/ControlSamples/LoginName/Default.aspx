<%@ Page Title="LoginName Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.LoginName.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>LoginName Sample</h2>

    <h3>Default LoginName</h3>
    <div data-audit-control="LoginName-1">
        <asp:LoginName ID="LoginName1" runat="server" />
    </div>

    <h3>LoginName with FormatString</h3>
    <div data-audit-control="LoginName-2">
        <asp:LoginName ID="LoginName2" runat="server"
            FormatString="Welcome, {0}!" />
    </div>
</asp:Content>
