<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Pager.ascx.cs" Inherits="DepartmentPortal.Controls.Pager" %>

<div class="pager">
    <asp:LinkButton ID="lnkPrevious" runat="server" Text="&laquo; Previous" CssClass="pager-link pager-prev"
        OnClick="lnkPrevious_Click" />
    <asp:Repeater ID="rptPages" runat="server">
        <ItemTemplate>
            <asp:LinkButton ID="lnkPage" runat="server"
                Text='<%# Container.DataItem %>'
                CommandArgument='<%# Container.DataItem %>'
                CssClass='<%# Convert.ToInt32(Container.DataItem) == CurrentPage ? "pager-link active" : "pager-link" %>'
                OnClick="lnkPage_Click" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:LinkButton ID="lnkNext" runat="server" Text="Next &raquo;" CssClass="pager-link pager-next"
        OnClick="lnkNext_Click" />
</div>
