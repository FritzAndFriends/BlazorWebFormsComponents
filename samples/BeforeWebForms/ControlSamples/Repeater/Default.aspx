<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Repeater.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Repeater control homepage</h2>

    <div>
<%--      Other usage samples:  <a href="ModelBinding.aspx">ModelBinding Sample</a>--%>
    </div>

    <p>Here is a simple repeater bound to a collection of widgets.</p>

    <div data-audit-control="Repeater">
        <asp:Repeater ID="repeaterControl" runat="server" 
          ItemType="SharedSampleObjects.Models.Widget"> 
          <HeaderTemplate>
            This is a list of widgets
          </HeaderTemplate>
          <ItemTemplate>
            <li><%# Item.Name %></li>
          </ItemTemplate>
          <FooterTemplate>
            This is the footer of the control
          </FooterTemplate>
          <SeparatorTemplate><hr /></SeparatorTemplate>
        </asp:Repeater>
    </div>

</asp:Content>
