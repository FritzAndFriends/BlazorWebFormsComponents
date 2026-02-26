<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ImageButton.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>ImageButton Control Samples</h2>

    <h3>Basic ImageButton</h3>
    <div data-audit-control="ImageButton-1">
    <asp:ImageButton ID="ImageButton1" ImageUrl="~/Content/Images/banner.png" AlternateText="Submit" runat="server" />
    </div>

    <h3>ImageButton with CSS Class</h3>
    <div data-audit-control="ImageButton-2">
    <asp:ImageButton ID="ImageButton2" ImageUrl="~/Content/Images/banner.png" AlternateText="Click here" CssClass="img-button" runat="server" />
    </div>
</asp:Content>
