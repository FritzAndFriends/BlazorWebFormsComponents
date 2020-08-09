<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebProject._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">
        <div class="col-md-4">
            <h2>BlazorWebFormsComponents - .NET Standard Sample 1</h2>
            <p>
                This is an example showing a Web Forms project that references a class library and how to migrate that library to .NET Standard in preparation for migration to Blazor
            </p>
            <p>
                <a class="btn btn-default" href="https://github.com/fritzandfriends/blazorwebformscomponents">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Calculate 6% Tax</h2>
            <p>
                One accounting feature is the ability to calculate 6% sales tax for our shopping cart
            </p>
            <p>
              <label runat="server">Current Price:</label> <input type="number" runat="server" id="CurrentPrice" />
              <br />
              <label runat="server">6% Tax:</label> <label runat="server" id="TaxValue" />
              <br />
              <asp:Button runat="server" ID="CalculateTax" OnClick="CalculateTax_Click" Text="Calculate 6% Sales Tax" />

            </p>
        </div>
        <div class="col-md-4">
            <h2>Calculate Totals for our cart</h2>
            <p>
                The library can also calculate the total number of items in the cart and the total price of the items in the cart
            </p>

             <asp:GridView runat="server" ID="LineItemList" AutoGenerateColumns="true">
             </asp:GridView>

              <label>Total Price:</label> <label runat="server" id="TotalPrice" />
              <br />
              <label>Total # Items:</label> <label runat="server" id="CountItems" />
              <br />
              <asp:Button runat="server" ID="CalculateTotals" OnClick="CalculateTotals_Click" Text="Calculate Totals" />

        </div>
    </div>

</asp:Content>
