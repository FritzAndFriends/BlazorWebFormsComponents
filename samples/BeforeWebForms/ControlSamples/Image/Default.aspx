<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Image.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Image Control Samples</h2>
    
    <h3>Basic Image</h3>
    <div data-audit-control="Image-1">
    <asp:Image ID="imgBasic" ImageUrl="~/Content/Images/banner.png" 
               AlternateText="Banner image" runat="server" />
    </div>
    
    <h3>Sized Image</h3>
    <div data-audit-control="Image-2">
    <asp:Image ID="imgSized" ImageUrl="~/Content/Images/banner.png" 
               AlternateText="Sized image" Width="200" Height="100" runat="server" />
    </div>
</asp:Content>
