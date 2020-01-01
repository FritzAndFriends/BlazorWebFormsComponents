<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataList.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DataList control homepage</h2>

    <div>
      Other usage samples:  <a href="FlowLayout.aspx">FlowLayout Sample</a>
    </div>

    <p>Here is a simple datalist bound to a collection of widgets.  By default, the RepeatLayout
      is a Table.
    </p>

    <asp:DataList ID="simpleDataList" 
                  runat="server" 
                  Enabled="true"
                  HeaderStyle-BackColor=""
                  RepeatLayout="Table"
                  RepeatColumns="2"
                  ItemType="SharedSampleObjects.Models.Widget">
        <HeaderTemplate>
          My Widget List
        </HeaderTemplate>
        <ItemTemplate>
          <%# Item.Name %> <br /> <%# Item.Price.ToString("c") %>
        </ItemTemplate>
    </asp:DataList>

</asp:Content>
