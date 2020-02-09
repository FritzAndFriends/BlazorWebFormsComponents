<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RepeatColumns.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataList.RepeatColumns" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DataList Repeat Columns Sample</h2>

    <div>
        usage samples <a href="Default.aspx">Default Sample</a>|<a href="FlowLayout.aspx">FlowLayout Sample</a>|<a href="StyleAttributes.aspx">Styles</a>|<a href="RepeatColumns.aspx">Repeat Columns Sample</a>
    </div>

    <p>Here is a simple datalist bound to a collection of widgets with RepeatLayout set to Flow</p>

    <asp:DataList ID="simpleDataList"
        runat="server"
        Enabled="true"
        Caption="This is my caption"
        CaptionAlign="Top"
        CellPadding="2"
        CellSpacing="3"
        ToolTip="This is my tooltip"
        UseAccessibleHeader="true"
        GridLines="Vertical"
        RepeatLayout="Flow"
        RepeatDirection="Vertical"
        RepeatColumns="4"
        ItemType="SharedSampleObjects.Models.Widget">
        <HeaderStyle />
        <HeaderTemplate>
            My Widget List
        </HeaderTemplate>
        <ItemTemplate>
            <%# Item.Id %>
        </ItemTemplate>
    </asp:DataList>

</asp:Content>
