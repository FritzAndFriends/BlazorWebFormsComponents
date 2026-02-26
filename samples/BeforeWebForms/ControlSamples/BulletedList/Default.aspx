<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.BulletedList.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>BulletedList Control Samples</h2>
    
    <h3>Disc Bullets</h3>
    <div data-audit-control="BulletedList-1">
    <asp:BulletedList ID="blDisc" BulletStyle="Disc" DisplayMode="Text" runat="server">
        <asp:ListItem>Apple</asp:ListItem>
        <asp:ListItem>Banana</asp:ListItem>
        <asp:ListItem>Cherry</asp:ListItem>
        <asp:ListItem>Date</asp:ListItem>
    </asp:BulletedList>
    </div>
    
    <h3>Numbered Bullets</h3>
    <div data-audit-control="BulletedList-2">
    <asp:BulletedList ID="blNumbered" BulletStyle="Numbered" DisplayMode="Text" runat="server">
        <asp:ListItem>First</asp:ListItem>
        <asp:ListItem>Second</asp:ListItem>
        <asp:ListItem>Third</asp:ListItem>
    </asp:BulletedList>
    </div>
    
    <h3>Square HyperLink Bullets</h3>
    <div data-audit-control="BulletedList-3">
    <asp:BulletedList ID="blSquare" BulletStyle="Square" DisplayMode="HyperLink" runat="server">
        <asp:ListItem Value="https://example.com">Example Site</asp:ListItem>
        <asp:ListItem Value="https://example.org">Example Org</asp:ListItem>
    </asp:BulletedList>
    </div>
</asp:Content>
