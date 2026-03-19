<%@ Page Title="Mixed Content" Language="C#" MasterPageFile="~/Site.Master" %>
<h1>Welcome</h1>
<div class="form-group">
    <asp:Label Text="Enter your name:" ID="lblName" />
    <asp:TextBox ID="txtName" CssClass="form-control" />
</div>
<p>This is static HTML content.</p>
<asp:Button Text="Go" ID="btnGo" CssClass="btn btn-primary" />
<%= DateTime.Now %>
