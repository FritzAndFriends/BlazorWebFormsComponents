<%@ Page Title="CustomValidator Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.CustomValidator.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>CustomValidator Sample</h2>

    <h3>Server-Side Validation (Even Number)</h3>
    <div data-audit-control="CustomValidator-1">
        Enter an even number:
        <asp:TextBox ID="txtEven" runat="server" />
        <asp:CustomValidator ID="cvEven" runat="server"
            ControlToValidate="txtEven"
            ErrorMessage="Please enter an even number."
            Display="Dynamic"
            OnServerValidate="cvEven_ServerValidate" />
    </div>

    <h3>Custom Validator Without ControlToValidate</h3>
    <div data-audit-control="CustomValidator-2">
        <asp:CheckBox ID="chkAgree" runat="server" Text="I agree to the terms" />
        <asp:CustomValidator ID="cvAgree" runat="server"
            ErrorMessage="You must agree to the terms."
            Display="Dynamic"
            OnServerValidate="cvAgree_ServerValidate" />
    </div>

    <div data-audit-control="CustomValidator-Submit">
        <asp:Button ID="btnSubmit" runat="server" Text="Validate" />
    </div>
</asp:Content>
