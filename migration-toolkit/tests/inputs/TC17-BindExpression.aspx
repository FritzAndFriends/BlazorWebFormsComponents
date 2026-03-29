<%@ Page Title="Bind Test" Language="C#" %>
<asp:FormView ID="FormView1" runat="server">
    <EditItemTemplate>
        <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("ProductName") %>' />
        <asp:TextBox ID="txtPrice" runat="server" Text='<%# Bind("Price") %>' />
        <asp:DropDownList ID="ddlCategory" runat="server" SelectedValue='<%# Bind("CategoryId") %>' />
    </EditItemTemplate>
</asp:FormView>
