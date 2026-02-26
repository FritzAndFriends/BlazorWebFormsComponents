<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ImageMap.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>ImageMap Control Samples</h2>

    <h3>ImageMap with Hot Spots</h3>
    <div data-audit-control="ImageMap">
    <asp:ImageMap ID="ImageMap1" ImageUrl="~/Content/Images/banner.png" AlternateText="Navigate" runat="server">
        <asp:RectangleHotSpot HotSpotMode="Navigate" NavigateUrl="https://bing.com" AlternateText="Go to Bing" Top="0" Left="0" Bottom="50" Right="100" />
        <asp:RectangleHotSpot HotSpotMode="Navigate" NavigateUrl="https://github.com" AlternateText="Go to GitHub" Top="0" Left="100" Bottom="50" Right="200" />
    </asp:ImageMap>
    </div>
</asp:Content>
