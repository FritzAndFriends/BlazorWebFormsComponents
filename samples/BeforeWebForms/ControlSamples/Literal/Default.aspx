<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Literal.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Literal Control Samples</h2>
    
    <h3>PassThrough Mode</h3>
    <div data-audit-control="Literal-1">
    <asp:Literal ID="litPassThrough" Text="This is <b>literal</b> content." Mode="PassThrough" runat="server" />
    </div>
    
    <h3>Encode Mode</h3>
    <div data-audit-control="Literal-2">
    <asp:Literal ID="litEncoded" Text="This is &lt;b&gt;encoded&lt;/b&gt; content." Mode="Encode" runat="server" />
    </div>
    
    <h3>Default (Simple Text)</h3>
    <div data-audit-control="Literal-3">
    <asp:Literal ID="litSimple" Text="Simple text content" runat="server" />
    </div>
</asp:Content>
