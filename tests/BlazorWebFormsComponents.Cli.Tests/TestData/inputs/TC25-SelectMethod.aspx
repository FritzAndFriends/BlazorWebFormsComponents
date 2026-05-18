<%@ Page Title="Products" Language="C#" %>
<asp:ListView ID="ProductList" runat="server"
    ItemType="WebApp.Models.Product"
    SelectMethod="GetProducts">
    <ItemTemplate>
        <tr>
            <td><%# Item.Name %></td>
        </tr>
    </ItemTemplate>
</asp:ListView>
<asp:FormView ID="DetailView" runat="server"
    ItemType="WebApp.Models.Product"
    SelectMethod="GetProduct"
    InsertMethod="InsertProduct"
    UpdateMethod="UpdateProduct"
    DeleteMethod="DeleteProduct">
    <ItemTemplate>
        <span><%# Item.Description %></span>
    </ItemTemplate>
</asp:FormView>
