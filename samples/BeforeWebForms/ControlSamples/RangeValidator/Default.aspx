<%@ Page Title="RangeValidator Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.RangeValidator.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>RangeValidator Sample</h2>

    <h3>Integer Range (1-100)</h3>
    <div data-audit-control="RangeValidator-1">
        Enter a number between 1 and 100:
        <asp:TextBox ID="txtAge" runat="server" />
        <asp:RangeValidator ID="rvAge" runat="server"
            ControlToValidate="txtAge"
            MinimumValue="1"
            MaximumValue="100"
            Type="Integer"
            ErrorMessage="Value must be between 1 and 100."
            Display="Dynamic" />
    </div>

    <h3>Date Range</h3>
    <div data-audit-control="RangeValidator-2">
        Enter a date in 2024:
        <asp:TextBox ID="txtDate" runat="server" />
        <asp:RangeValidator ID="rvDate" runat="server"
            ControlToValidate="txtDate"
            MinimumValue="01/01/2024"
            MaximumValue="12/31/2024"
            Type="Date"
            ErrorMessage="Date must be in 2024."
            Display="Dynamic" />
    </div>

    <h3>Currency Range</h3>
    <div data-audit-control="RangeValidator-3">
        Enter a price ($1.00 - $999.99):
        <asp:TextBox ID="txtPrice" runat="server" />
        <asp:RangeValidator ID="rvPrice" runat="server"
            ControlToValidate="txtPrice"
            MinimumValue="1.00"
            MaximumValue="999.99"
            Type="Currency"
            ErrorMessage="Price must be between $1.00 and $999.99."
            Display="Dynamic" />
    </div>

    <div data-audit-control="RangeValidator-Submit">
        <asp:Button ID="btnSubmit" runat="server" Text="Validate" />
    </div>
</asp:Content>
