<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StyleAttributes.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataList.StyleAttributes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DataList Styles</h2>

    <div>
      Other usage samples: <a href="Default.aspx">Simple Layout Sample</a>  <a href="FlowLayout.aspx">FlowLayout Sample</a>
    </div>

    <p>Here is a simple datalist bound to a collection of widgets.  We're testing and showing the various
		style attributes that can be set on the DataList
    </p>

	<asp:DataList ID="simpleDataList"
		runat="server"
		RepeatColumns="2"
		ToolTip="This is my tooltip"
		AccessKey="S"
		BackColor="Gray"
		BorderStyle="Solid"
		BorderWidth="2px"
		BorderColor="Firebrick"
		Font-Bold="true"
		UseAccessibleHeader="true"
		ItemType="SharedSampleObjects.Models.Widget">
		<HeaderTemplate>
			My Widget List
		</HeaderTemplate>
		<FooterTemplate>End of Line</FooterTemplate>
		<ItemTemplate>
			<%# Item.Name %>
			<br />
			<%# Item.Price.ToString("c") %>
		</ItemTemplate>
	</asp:DataList>

</asp:Content>
