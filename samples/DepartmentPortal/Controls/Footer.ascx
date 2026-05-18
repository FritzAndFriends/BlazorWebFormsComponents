<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="DepartmentPortal.Controls.Footer" %>

<footer class="site-footer">
    <div class="footer-content">
        <p>&copy; <asp:Literal ID="litYear" runat="server" /> Department Portal. All rights reserved.</p>
        <div runat="server" id="pnlLinks" visible="false" class="footer-links">
            <a href="/Default.aspx">Home</a> |
            <a href="/Dashboard.aspx">Dashboard</a> |
            <a href="#">Privacy Policy</a> |
            <a href="#">Contact Us</a>
        </div>
    </div>
</footer>
