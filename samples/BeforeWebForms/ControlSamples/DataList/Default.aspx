﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DataList.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>DataList control homepage</h2>

    <div>
      Other usage samples:  <a href="FlowLayout.aspx">FlowLayout Sample</a>
    </div>

    <p>Here is a simple datalist bound to a collection of widgets.  By default, the RepeatLayout
      is a Table.
    </p>

	<asp:DataList ID="simpleDataList"
		runat="server"
		RepeatColumns="2"
		ItemType="SharedSampleObjects.Models.Widget">
		<HeaderStyle CssClass="myClass" Font-Bold="true" Font-Italic="true" Font-Names="arial black" Font-Overline="true" Font-Size="X-Large" Font-Strikeout="true" Font-Underline="true" />
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
