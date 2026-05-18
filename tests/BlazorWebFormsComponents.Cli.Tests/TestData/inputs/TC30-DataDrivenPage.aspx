<%@ Page Title="Product Catalog" Language="C#" MasterPageFile="~/Site.Master" %>
<%@ Import Namespace="ContosoStore.Models" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Product Catalog</h1>
    <%-- Product search and filter controls --%>
    <div class="search-bar">
        <asp:TextBox ID="txtSearch" CssClass="form-control" runat="server" />
        <asp:Button ID="btnSearch" Text="Search" OnClick="Search_Click" CssClass="btn btn-primary" runat="server" />
    </div>
    <asp:GridView ID="gvProducts" DataSourceID="dsProducts" AutoGenerateColumns="False"
        AllowPaging="True" AllowSorting="True" PageSize="10"
        CssClass="table table-striped" GridLines="None" runat="server">
        <Columns>
            <asp:BoundField DataField="ProductName" HeaderText="Product" SortExpression="ProductName" />
            <asp:BoundField DataField="UnitPrice" HeaderText="Price" DataFormatString="{0:C}" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="dsProducts" runat="server"
        ConnectionString="<%$ ConnectionStrings:StoreDB %>"
        SelectCommand="SELECT ProductName, UnitPrice FROM Products" />
    <h2>Featured Products</h2>
    <asp:Repeater ID="rptFeatured" ItemType="ContosoStore.Models.Product" SelectMethod="GetFeaturedProducts" runat="server">
        <ItemTemplate>
            <div class="product-card">
                <h4><%#: Eval("ProductName") %></h4>
                <span class="price"><%#: Item.UnitPrice %></span>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
