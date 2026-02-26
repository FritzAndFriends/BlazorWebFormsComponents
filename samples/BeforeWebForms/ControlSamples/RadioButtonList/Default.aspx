<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.RadioButtonList.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>RadioButtonList Control Samples</h2>

    <h3>Vertical Layout</h3>
    <div data-audit-control="RadioButtonList-1">
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Vertical">
            <asp:ListItem>Excellent</asp:ListItem>
            <asp:ListItem Selected="True">Good</asp:ListItem>
            <asp:ListItem>Average</asp:ListItem>
            <asp:ListItem>Poor</asp:ListItem>
        </asp:RadioButtonList>
    </div>

    <h3>Horizontal Layout with RepeatColumns</h3>
    <div data-audit-control="RadioButtonList-2">
        <asp:RadioButtonList ID="RadioButtonList2" runat="server" RepeatDirection="Horizontal" RepeatColumns="2">
            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
            <asp:ListItem>Maybe</asp:ListItem>
        </asp:RadioButtonList>
    </div>

</asp:Content>
