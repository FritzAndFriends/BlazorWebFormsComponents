<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Panel.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Panel Control Samples</h2>
    
    <h3>Basic Panel with GroupingText</h3>
    <div data-audit-control="Panel-1">
    <asp:Panel ID="pnlUserInfo" GroupingText="User Info" runat="server">
        <asp:Label ID="lblName" Text="Name:" runat="server" />
        <asp:TextBox ID="txtName" runat="server" />
    </asp:Panel>
    </div>
    
    <h3>Panel with ScrollBars</h3>
    <div data-audit-control="Panel-2">
    <asp:Panel ID="pnlScrollable" ScrollBars="Auto" Height="100px" runat="server">
        <p>First paragraph of content inside the scrollable panel.</p>
        <p>Second paragraph of content inside the scrollable panel.</p>
        <p>Third paragraph of content inside the scrollable panel.</p>
        <p>Fourth paragraph of content inside the scrollable panel.</p>
    </asp:Panel>
    </div>
    
    <h3>Panel with DefaultButton</h3>
    <div data-audit-control="Panel-3">
    <asp:Panel ID="pnlForm" DefaultButton="btnSubmit" runat="server">
        <asp:TextBox ID="txtInput" runat="server" />
        <asp:Button ID="btnSubmit" Text="Submit" runat="server" />
    </asp:Panel>
    </div>
</asp:Content>
