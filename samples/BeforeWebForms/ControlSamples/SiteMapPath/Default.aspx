<%@ Page Title="SiteMapPath Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.SiteMapPath.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>SiteMapPath Sample</h2>

    <h3>Default SiteMapPath</h3>
    <div data-audit-control="SiteMapPath-1">
        <asp:SiteMapPath ID="SiteMapPath1" runat="server" />
    </div>

    <h3>Styled SiteMapPath</h3>
    <div data-audit-control="SiteMapPath-2">
        <asp:SiteMapPath ID="SiteMapPath2" runat="server"
            PathSeparator=" &gt; "
            RenderCurrentNodeAsLink="false">
            <CurrentNodeStyle ForeColor="Gray" Font-Bold="true" />
            <NodeStyle ForeColor="Blue" />
            <RootNodeStyle ForeColor="DarkBlue" Font-Bold="true" />
            <PathSeparatorStyle ForeColor="Gray" />
        </asp:SiteMapPath>
    </div>
</asp:Content>
