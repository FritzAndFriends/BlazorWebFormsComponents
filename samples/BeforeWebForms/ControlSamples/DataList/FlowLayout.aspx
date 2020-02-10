<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FlowLayout.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataList.FlowLayout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<h2>DataList Flow Layout Sample</h2>

	<div>
		Other usage samples:  <a href="Default.aspx">Simple Layout Sample</a>  <a href="StyleAttributes.aspx">Styles</a>
	</div>

	<p>Here is a simple datalist bound to a collection of widgets with RepeatLayout set to Flow</p>

	<asp:DataList ID="simpleDataList"
		runat="server"
		Enabled="true"
		Caption="This is my caption"
		CaptionAlign="Top"
		CellPadding="2"
		CellSpacing="3"
		ToolTip="This is my tooltip"
		UseAccessibleHeader="true"
		RepeatLayout="Flow"
		ItemType="SharedSampleObjects.Models.Widget">
		<HeaderStyle />
		<HeaderTemplate>
			My Widget List
		</HeaderTemplate>
		<ItemTemplate>
			<%# Item.Name %>
			<br />
			<%# Item.Price.ToString("c") %>
		</ItemTemplate>
	</asp:DataList>

</asp:Content>
