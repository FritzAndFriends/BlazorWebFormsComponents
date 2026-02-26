<%@ Page Title="RequiredFieldValidator Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.RequiredFieldValidator.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>RequiredFieldValidator Sample</h2>

    <h3>Basic Required Field</h3>
    <div data-audit-control="RequiredFieldValidator-1">
        <asp:TextBox ID="txtName" runat="server" />
        <asp:RequiredFieldValidator ID="rfvName" runat="server"
            ControlToValidate="txtName"
            ErrorMessage="Name is required."
            Text="*"
            Display="Static" />
    </div>

    <h3>Dynamic Display</h3>
    <div data-audit-control="RequiredFieldValidator-2">
        <asp:TextBox ID="txtEmail" runat="server" />
        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
            ControlToValidate="txtEmail"
            ErrorMessage="Email is required."
            Text="(required)"
            Display="Dynamic" />
    </div>

    <h3>Display None (summary only)</h3>
    <div data-audit-control="RequiredFieldValidator-3">
        <asp:TextBox ID="txtPhone" runat="server" />
        <asp:RequiredFieldValidator ID="rfvPhone" runat="server"
            ControlToValidate="txtPhone"
            ErrorMessage="Phone is required."
            Display="None" />
    </div>

    <div data-audit-control="RequiredFieldValidator-Submit">
        <asp:Button ID="btnSubmit" runat="server" Text="Validate" />
    </div>
</asp:Content>
