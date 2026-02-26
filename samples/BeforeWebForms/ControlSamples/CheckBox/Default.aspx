<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.CheckBox.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>CheckBox Control Samples</h2>
    
    <h3>Unchecked CheckBox</h3>
    <div data-audit-control="CheckBox-1">
    <asp:CheckBox ID="chkTerms" Text="Accept Terms" runat="server" />
    </div>
    
    <h3>Checked CheckBox</h3>
    <div data-audit-control="CheckBox-2">
    <asp:CheckBox ID="chkSubscribe" Text="Subscribe" Checked="true" runat="server" />
    </div>
    
    <h3>With AutoPostBack</h3>
    <div data-audit-control="CheckBox-3">
    <asp:CheckBox ID="chkFeature" Text="Enable Feature" AutoPostBack="true" runat="server" />
    </div>
</asp:Content>
