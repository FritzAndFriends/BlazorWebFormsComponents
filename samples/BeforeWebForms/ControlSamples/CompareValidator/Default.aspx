<%@ Page Title="CompareValidator Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.CompareValidator.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>CompareValidator Sample</h2>

    <h3>Compare Two Fields (Equal)</h3>
    <div data-audit-control="CompareValidator-1">
        Password: <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" /><br />
        Confirm: <asp:TextBox ID="txtConfirm" TextMode="Password" runat="server" />
        <asp:CompareValidator ID="cvPasswords" runat="server"
            ControlToValidate="txtConfirm"
            ControlToCompare="txtPassword"
            Operator="Equal"
            Type="String"
            ErrorMessage="Passwords do not match."
            Display="Dynamic" />
    </div>

    <h3>Compare to Value (GreaterThan)</h3>
    <div data-audit-control="CompareValidator-2">
        Enter a number greater than 10:
        <asp:TextBox ID="txtNumber" runat="server" />
        <asp:CompareValidator ID="cvNumber" runat="server"
            ControlToValidate="txtNumber"
            Operator="GreaterThan"
            Type="Integer"
            ValueToCompare="10"
            ErrorMessage="Value must be greater than 10."
            Display="Dynamic" />
    </div>

    <h3>Data Type Check</h3>
    <div data-audit-control="CompareValidator-3">
        Enter a date:
        <asp:TextBox ID="txtDate" runat="server" />
        <asp:CompareValidator ID="cvDate" runat="server"
            ControlToValidate="txtDate"
            Operator="DataTypeCheck"
            Type="Date"
            ErrorMessage="Please enter a valid date."
            Display="Dynamic" />
    </div>

    <div data-audit-control="CompareValidator-Submit">
        <asp:Button ID="btnSubmit" runat="server" Text="Validate" />
    </div>
</asp:Content>
