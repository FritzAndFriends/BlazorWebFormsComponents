<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ListBox.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>ListBox Control Samples</h2>

    <h3>Single Selection</h3>
    <div data-audit-control="ListBox-1">
        <asp:ListBox ID="ListBox1" runat="server" Rows="5">
            <asp:ListItem>Red</asp:ListItem>
            <asp:ListItem>Orange</asp:ListItem>
            <asp:ListItem>Yellow</asp:ListItem>
            <asp:ListItem>Green</asp:ListItem>
            <asp:ListItem>Blue</asp:ListItem>
            <asp:ListItem>Purple</asp:ListItem>
        </asp:ListBox>
    </div>

    <h3>Multiple Selection</h3>
    <div data-audit-control="ListBox-2">
        <asp:ListBox ID="ListBox2" runat="server" SelectionMode="Multiple" Rows="4">
            <asp:ListItem>Cat</asp:ListItem>
            <asp:ListItem Selected="True">Dog</asp:ListItem>
            <asp:ListItem Selected="True">Fish</asp:ListItem>
            <asp:ListItem>Bird</asp:ListItem>
        </asp:ListBox>
    </div>

</asp:Content>
