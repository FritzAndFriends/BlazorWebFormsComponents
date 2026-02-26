<%@ Page Title="RegularExpressionValidator Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.RegularExpressionValidator.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>RegularExpressionValidator Sample</h2>

    <h3>Email Validation</h3>
    <div data-audit-control="RegularExpressionValidator-1">
        Email:
        <asp:TextBox ID="txtEmail" runat="server" />
        <asp:RegularExpressionValidator ID="revEmail" runat="server"
            ControlToValidate="txtEmail"
            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
            ErrorMessage="Please enter a valid email address."
            Display="Dynamic" />
    </div>

    <h3>US Phone Number</h3>
    <div data-audit-control="RegularExpressionValidator-2">
        Phone (xxx-xxx-xxxx):
        <asp:TextBox ID="txtPhone" runat="server" />
        <asp:RegularExpressionValidator ID="revPhone" runat="server"
            ControlToValidate="txtPhone"
            ValidationExpression="\d{3}-\d{3}-\d{4}"
            ErrorMessage="Please enter a valid phone number (xxx-xxx-xxxx)."
            Display="Dynamic" />
    </div>

    <h3>US Zip Code</h3>
    <div data-audit-control="RegularExpressionValidator-3">
        Zip Code:
        <asp:TextBox ID="txtZip" runat="server" />
        <asp:RegularExpressionValidator ID="revZip" runat="server"
            ControlToValidate="txtZip"
            ValidationExpression="\d{5}(-\d{4})?"
            ErrorMessage="Please enter a valid zip code."
            Display="Dynamic" />
    </div>

    <div data-audit-control="RegularExpressionValidator-Submit">
        <asp:Button ID="btnSubmit" runat="server" Text="Validate" />
    </div>
</asp:Content>
