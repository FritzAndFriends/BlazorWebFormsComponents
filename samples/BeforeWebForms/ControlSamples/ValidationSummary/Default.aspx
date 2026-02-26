<%@ Page Title="ValidationSummary Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ValidationSummary.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>ValidationSummary Sample</h2>

    <h3>Form with Multiple Validators</h3>
    <p>
        Name: <asp:TextBox ID="txtName" runat="server" />
        <asp:RequiredFieldValidator ID="rfvName" runat="server"
            ControlToValidate="txtName"
            ErrorMessage="Name is required."
            Text="*"
            Display="Dynamic" />
    </p>
    <p>
        Email: <asp:TextBox ID="txtEmail" runat="server" />
        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
            ControlToValidate="txtEmail"
            ErrorMessage="Email is required."
            Text="*"
            Display="Dynamic" />
    </p>
    <p>
        Age: <asp:TextBox ID="txtAge" runat="server" />
        <asp:RequiredFieldValidator ID="rfvAge" runat="server"
            ControlToValidate="txtAge"
            ErrorMessage="Age is required."
            Text="*"
            Display="Dynamic" />
        <asp:RangeValidator ID="rvAge" runat="server"
            ControlToValidate="txtAge"
            MinimumValue="1"
            MaximumValue="120"
            Type="Integer"
            ErrorMessage="Age must be between 1 and 120."
            Text="*"
            Display="Dynamic" />
    </p>

    <h3>BulletList Display</h3>
    <div data-audit-control="ValidationSummary-1">
        <asp:ValidationSummary ID="vsBullet" runat="server"
            DisplayMode="BulletList"
            HeaderText="Please fix the following errors:" />
    </div>

    <h3>List Display</h3>
    <div data-audit-control="ValidationSummary-2">
        <asp:ValidationSummary ID="vsList" runat="server"
            DisplayMode="List"
            HeaderText="Errors (List):" />
    </div>

    <h3>SingleParagraph Display</h3>
    <div data-audit-control="ValidationSummary-3">
        <asp:ValidationSummary ID="vsParagraph" runat="server"
            DisplayMode="SingleParagraph"
            HeaderText="Errors (Paragraph):" />
    </div>

    <div data-audit-control="ValidationSummary-Submit">
        <asp:Button ID="btnSubmit" runat="server" Text="Submit Form" />
    </div>
</asp:Content>
