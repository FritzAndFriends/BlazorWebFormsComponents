<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Grouping.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ListView.Grouping" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

    <h2>ListView control homepage</h2>

    <div>
        <a href="Default.aspx">Default</a> | <a href="Grouping.aspx">Grouping</a> | <a href="ModelBinding.aspx">ModelBinding</a>
    </div>

    <p>Here is a listview using Grouping to interact with widgets.</p>

    <div data-audit-control="ListView">
    <asp:ListView ID="simpleListView"
        runat="server"
        GroupItemCount="5"
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
                    <tr runat="server" id="groupPlaceHolder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <GroupTemplate>
						<tr><td colspan="4">GroupStart</td></tr>
						<data runat="server" id="itemPlaceHolder" /> 
						<tr><td colspan="4">GroupEnd</td></tr>
        </GroupTemplate>
        <GroupSeparatorTemplate>
            <tr runat="server"> <td colspan="4">GroupingSeperator</td> </tr>
        </GroupSeparatorTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("c") %></td>
                <td><%# Item.LastUpdate.ToString("d") %></td>
            </tr>
        </ItemTemplate>
        <ItemSeparatorTemplate><tr><td colspan="4">---</td></tr></ItemSeparatorTemplate>
      	<AlternatingItemTemplate>
          <tr>
            <td><em><%# Item.Id %></em></td>
            <td><%# Item.Name %></td>
            <td><%# Item.Price %></td>
            <td><%# Item.LastUpdate.ToString("d") %></td>
          </tr>
				</AlternatingItemTemplate>
        <EmptyDataTemplate>
            <tr>
                <td colspan="4">No widgets available</td>
            </tr>
        </EmptyDataTemplate>
        <EmptyItemTemplate></EmptyItemTemplate>
    </asp:ListView>
    </div>


</asp:Content>
