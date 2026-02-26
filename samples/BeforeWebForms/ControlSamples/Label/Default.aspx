<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Label.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Label Control Samples</h2>
    
    <h3>Basic Label</h3>
    <div data-audit-control="Label-1">
    <asp:Label ID="lblBasic" Text="Hello World" runat="server" />
    </div>
    
    <h3>Styled Label</h3>
    <div data-audit-control="Label-2">
    <asp:Label ID="lblStyled" Text="Styled Label" CssClass="text-primary" 
               ForeColor="Blue" Font-Bold="true" runat="server" />
    </div>
    
    <h3>Label with HTML</h3>
    <div data-audit-control="Label-3">
    <asp:Label ID="lblHtml" Text="<em>Emphasized</em>" runat="server" />
    </div>
</asp:Content>
