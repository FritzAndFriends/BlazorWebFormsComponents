<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.CheckBoxList.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>CheckBoxList Control Samples</h2>

    <h3>Vertical Layout</h3>
    <div data-audit-control="CheckBoxList-1">
        <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatDirection="Vertical">
            <asp:ListItem>Reading</asp:ListItem>
            <asp:ListItem>Sports</asp:ListItem>
            <asp:ListItem Selected="True">Music</asp:ListItem>
            <asp:ListItem>Travel</asp:ListItem>
        </asp:CheckBoxList>
    </div>

    <h3>Horizontal Layout with RepeatColumns</h3>
    <div data-audit-control="CheckBoxList-2">
        <asp:CheckBoxList ID="CheckBoxList2" runat="server" RepeatDirection="Horizontal" RepeatColumns="2">
            <asp:ListItem>Small</asp:ListItem>
            <asp:ListItem>Medium</asp:ListItem>
            <asp:ListItem>Large</asp:ListItem>
            <asp:ListItem>XLarge</asp:ListItem>
        </asp:CheckBoxList>
    </div>

</asp:Content>
