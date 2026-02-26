<%@ Page Title="DataPager Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataPager.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DataPager Control</h2>

    <p>The DataPager provides paging functionality for data-bound controls that implement IPageableItemContainer, such as ListView.</p>

    <h3>ListView with DataPager — Numeric + Next/Previous</h3>
    <p>This example shows a ListView with a DataPager that combines NumericPagerField and NextPreviousPagerField.</p>

    <div data-audit-control="DataPager">
    <asp:ListView ID="ProductListView"
        runat="server"
        ItemType="BeforeWebForms.ControlSamples.DataPager.Product">
        <LayoutTemplate>
            <table cellpadding="4" border="1" style="border-collapse: collapse;">
                <thead>
                    <tr style="background-color: #507CD1; color: white;">
                        <th>ID</th>
                        <th>Name</th>
                        <th>Price</th>
                        <th>Category</th>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="itemPlaceHolder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("C") %></td>
                <td><%# Item.Category %></td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background-color: #F0F0F0;">
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("C") %></td>
                <td><%# Item.Category %></td>
            </tr>
        </AlternatingItemTemplate>
        <EmptyDataTemplate>
            <p>No products found.</p>
        </EmptyDataTemplate>
    </asp:ListView>

    <br />

    <asp:DataPager ID="ProductDataPager"
        runat="server"
        PagedControlID="ProductListView"
        PageSize="3">
        <Fields>
            <asp:NextPreviousPagerField
                ShowFirstPageButton="true"
                ShowLastPageButton="false"
                ShowPreviousPageButton="true"
                ShowNextPageButton="false"
                FirstPageText="&laquo; First"
                PreviousPageText="&lt; Prev" />
            <asp:NumericPagerField
                ButtonCount="5" />
            <asp:NextPreviousPagerField
                ShowFirstPageButton="false"
                ShowLastPageButton="true"
                ShowPreviousPageButton="false"
                ShowNextPageButton="true"
                NextPageText="Next &gt;"
                LastPageText="Last &raquo;" />
        </Fields>
    </asp:DataPager>
    </div>

</asp:Content>
