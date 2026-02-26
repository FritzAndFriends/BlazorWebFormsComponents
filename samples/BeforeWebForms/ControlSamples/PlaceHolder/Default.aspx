<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.PlaceHolder.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>PlaceHolder Control Samples</h2>
    
    <h3>PlaceHolder with Programmatic Content</h3>
    <div data-audit-control="PlaceHolder">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server" />
    </div>
</asp:Content>
