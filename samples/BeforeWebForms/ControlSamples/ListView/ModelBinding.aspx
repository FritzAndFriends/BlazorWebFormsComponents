<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModelBinding.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ListView.ModelBinding" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

    <h2>ListView control homepage</h2>

    <div>
      <a href="Default.aspx">Default</a> | <a href="Grouping.aspx">Grouping</a> | <a href="ModelBinding.aspx">ModelBinding</a>
    </div>

    <p>Here is a listview using ModelBinding to interact with widgets.</p>

    <div data-audit-control="ListView">
    <asp:ListView ID="simpleListView" 
                  runat="server" 
                  SelectMethod="simpleListView_GetData"
                  ItemType="SharedSampleObjects.Models.Widget">
        <LayoutTemplate>
            <table>
                <thead>
                    <tr>
                        <td>Id</td>
                        <td>Name</td>
                        <td>Price</td>
                        <td>Last Update</td>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="itemPlaceHolder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <AlternatingItemTemplate>
            <tr class="table-dark">
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("c") %></td>
                <td><%# Item.LastUpdate.ToString("d") %></td>
            </tr>
        </AlternatingItemTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("c") %></td>
                <td><%# Item.LastUpdate.ToString("d") %></td>
            </tr>
        </ItemTemplate>
        <ItemSeparatorTemplate>
          <tr>
            <td colspan="4" style="border-bottom: 1px solid black;">&nbsp;</td>
          </tr>
        </ItemSeparatorTemplate>
        <EmptyDataTemplate>
          <tr>
            <td colspan="4">No widgets available</td>
          </tr>
        </EmptyDataTemplate>
        <EmptyItemTemplate></EmptyItemTemplate>
    </asp:ListView>
    </div>


</asp:Content>
