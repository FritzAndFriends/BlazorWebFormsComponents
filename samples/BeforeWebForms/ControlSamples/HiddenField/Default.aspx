<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.HiddenField.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>HiddenField Control Samples</h2>
    
    <h3>Basic HiddenField</h3>
    <div data-audit-control="HiddenField">
    <asp:HiddenField ID="HiddenField1" Value="secret-value-123" runat="server" />
    </div>
</asp:Content>
