<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms2.ControlSamples.GridView.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <h2>GridView control homepage</h2>

  <p>This is just a simple example of a GridView with a selectMethod</p>

  <asp:gridview id="CustomersGridView" 
    autogeneratecolumns="False"
    emptydatatext="No data available."
    selectMethod="GetCustomers"
    ItemType="BeforeWebForms2.ControlSamples.GridView.Customer"
    runat="server" DataKeyNames="CustomerID">
      <Columns>
          <asp:BoundField DataField="CustomerID" HeaderText="CustomerID" 
              InsertVisible="False" ReadOnly="True" SortExpression="CustomerID" />
          <asp:BoundField DataField="CompanyName" HeaderText="CompanyName" 
              SortExpression="CompanyName" />
          <asp:BoundField DataField="FirstName" HeaderText="FirstName" 
              SortExpression="FirstName" />
          <asp:BoundField DataField="LastName" HeaderText="LastName" 
              SortExpression="LastName" />
        <asp:TemplateField>
			        <ItemTemplate>
				        <button type="button">Click Me!</button>
			         </ItemTemplate>
		    </asp:TemplateField>
      </Columns>
  </asp:gridview>
</asp:Content>
