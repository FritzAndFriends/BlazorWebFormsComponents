<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FlowLayout.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataList.FlowLayout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DataList Flow Layout Sample</h2>

    <div>
      Other usage samples:  <a href="Default.aspx">Simple Layout Sample</a>
    </div>

    <p>Here is a simple datalist bound to a collection of widgets with RepeatLayout set to Flow</p>

    <asp:DataList ID="simpleDataList" 
                  runat="server" 
                  Enabled="true"
                  ToolTip="This is my tooltip"
                  RepeatLayout="Flow"
                  ItemType="SharedSampleObjects.Models.Widget">
        <HeaderTemplate>
          My Widget List
        </HeaderTemplate>
        <ItemTemplate>
          <%# Item.Name %> <br /> <%# Item.Price.ToString("c") %>
        </ItemTemplate>
    </asp:DataList>

</asp:Content>
