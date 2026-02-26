<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.AdRotator.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>AdRotator Control Samples</h2>
    
    <h3>AdRotator with XML Advertisement File</h3>
    <div data-audit-control="AdRotator">
    <asp:AdRotator ID="AdRotator1" AdvertisementFile="ads.xml" runat="server" />
    </div>
</asp:Content>
