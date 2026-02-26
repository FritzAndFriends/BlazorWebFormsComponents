<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.LinkButton.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>LinkButton Control Samples</h2>

    <h3>Basic LinkButton</h3>
    <div data-audit-control="LinkButton-1">
    <asp:LinkButton ID="LinkButton1" Text="Click Me" CssClass="btn btn-primary" runat="server" />
    </div>

    <h3>LinkButton with Click Handler</h3>
    <div data-audit-control="LinkButton-2">
    <asp:LinkButton ID="LinkButton2" Text="Submit Form" OnClick="LinkButton2_Click" runat="server" />
    </div>

    <h3>Disabled LinkButton</h3>
    <div data-audit-control="LinkButton-3">
    <asp:LinkButton ID="LinkButton3" Text="Disabled Link" Enabled="false" runat="server" />
    </div>
</asp:Content>
