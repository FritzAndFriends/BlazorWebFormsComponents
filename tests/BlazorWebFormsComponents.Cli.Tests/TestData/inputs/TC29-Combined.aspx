<%@ Page Title="Combined Test" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="TC29-Combined.aspx.cs" %>
<%@ Import Namespace="System.Collections.Generic" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%: PageTitle %></h1>
    <asp:GridView ID="ProductGrid" runat="server" AutoGenerateColumns="False" CssClass="table">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
        </Columns>
    </asp:GridView>
    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" CssClass="btn" />
    <asp:HyperLink ID="HelpLink" runat="server" NavigateUrl="~/Help.aspx" Text="Help" />
</asp:Content>
