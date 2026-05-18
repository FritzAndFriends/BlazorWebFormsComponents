<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DepartmentPortal.LoginPage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-6 col-md-offset-3">
            <div class="section-panel">
                <h2>Sign In</h2>
                <p>Select your user account to sign in to the Department Portal.</p>
                <div class="form-group">
                    <label for="UserDropDown">Select User:</label>
                    <asp:DropDownList ID="UserDropDown" runat="server" CssClass="form-control"
                        DataTextField="Name" DataValueField="Id" />
                </div>
                <div class="form-group">
                    <asp:Button ID="LoginButton" runat="server" Text="Sign In"
                        CssClass="btn btn-primary btn-block" OnClick="LoginButton_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
