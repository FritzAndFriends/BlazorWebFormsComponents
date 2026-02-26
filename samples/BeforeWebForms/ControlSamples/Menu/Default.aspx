﻿<%@ Page Title="Menu Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Menu.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div class="container">
		<div class="row">
			<div class="col-md-6">
				<h2>Declarative</h2>
				<div class="row">
					<h4>Horizontal Menu Using a Table Layout</h4>
					<div data-audit-control="Menu-1">
					<asp:Menu ID="HorizontalMenuTable"
						DisappearAfter="2000"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Horizontal"
						Font-Names="Arial"
						Target="_blank"
						RenderingMode="Table"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />

						<Items>
							<asp:MenuItem NavigateUrl="Home.aspx" Text="Home" ToolTip="Home">
								<asp:MenuItem NavigateUrl="Music.aspx" Text="Music" ToolTip="Music">
									<asp:MenuItem NavigateUrl="Classical.aspx" Text="Classical" ToolTip="Classical" />
									<asp:MenuItem NavigateUrl="Rock.aspx" Text="Rock" ToolTip="Rock" />
									<asp:MenuItem NavigateUrl="Jazz.aspx" Text="Jazz" ToolTip="Jazz" />
								</asp:MenuItem>
								<asp:MenuItem NavigateUrl="Movies.aspx" Text="Movies" ToolTip="Movies">
									<asp:MenuItem NavigateUrl="Action.aspx" Text="Action" ToolTip="Action" />
									<asp:MenuItem NavigateUrl="Drama.aspx" Text="Drama" ToolTip="Drama" />
									<asp:MenuItem NavigateUrl="Musical.aspx" Text="Musical" ToolTip="Musical" />
								</asp:MenuItem>
							</asp:MenuItem>
						</Items>
					</asp:Menu>
					</div>
				</div>
				<div class="row">
					<h4>Horizontal Menu Using a List Layout</h4>
					<div data-audit-control="Menu-2">
					<asp:Menu ID="HorizontalMenuList"
						DisappearAfter="2000"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Horizontal"
						Font-Names="Arial"
						Target="_blank"
						RenderingMode="List"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />

						<Items>
							<asp:MenuItem NavigateUrl="Home.aspx" Text="Home" ToolTip="Home">
								<asp:MenuItem NavigateUrl="Music.aspx" Text="Music" ToolTip="Music">
									<asp:MenuItem NavigateUrl="Classical.aspx" Text="Classical" ToolTip="Classical" />
									<asp:MenuItem NavigateUrl="Rock.aspx" Text="Rock" ToolTip="Rock" />
									<asp:MenuItem NavigateUrl="Jazz.aspx" Text="Jazz" ToolTip="Jazz" />
								</asp:MenuItem>
								<asp:MenuItem NavigateUrl="Movies.aspx" Text="Movies" ToolTip="Movies">
									<asp:MenuItem NavigateUrl="Action.aspx" Text="Action" ToolTip="Action" />
									<asp:MenuItem NavigateUrl="Drama.aspx" Text="Drama" ToolTip="Drama" />
									<asp:MenuItem NavigateUrl="Musical.aspx" Text="Musical" ToolTip="Musical" />
								</asp:MenuItem>
							</asp:MenuItem>
						</Items>
					</asp:Menu>
					</div>
				</div>
				<div class="row">
					<h4>Vertical Menu Using a Table Layout</h4>
					<div data-audit-control="Menu-3">
					<asp:Menu ID="VerticalMenuTable"
						DisappearAfter="2000"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Vertical"
						Font-Names="Arial"
						Target="_blank"
						RenderingMode="Table"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />

						<Items>
							<asp:MenuItem NavigateUrl="Home.aspx" Text="Home" ToolTip="Home">
								<asp:MenuItem NavigateUrl="Music.aspx" Text="Music" ToolTip="Music">
									<asp:MenuItem NavigateUrl="Classical.aspx" Text="Classical" ToolTip="Classical" />
									<asp:MenuItem NavigateUrl="Rock.aspx" Text="Rock" ToolTip="Rock" />
									<asp:MenuItem NavigateUrl="Jazz.aspx" Text="Jazz" ToolTip="Jazz" />
								</asp:MenuItem>
								<asp:MenuItem NavigateUrl="Movies.aspx" Text="Movies" ToolTip="Movies">
									<asp:MenuItem NavigateUrl="Action.aspx" Text="Action" ToolTip="Action" />
									<asp:MenuItem NavigateUrl="Drama.aspx" Text="Drama" ToolTip="Drama" />
									<asp:MenuItem NavigateUrl="Musical.aspx" Text="Musical" ToolTip="Musical" />
								</asp:MenuItem>
							</asp:MenuItem>
						</Items>
					</asp:Menu>
					</div>
				</div>
				<div class="row">
					<h4>Vertical Menu Using a List Layout</h4>
					<div data-audit-control="Menu-4">
					<asp:Menu ID="VerticalMenuList"
						DisappearAfter="2000"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Vertical"
						Font-Names="Arial"
						Target="_blank"
						RenderingMode="List"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />

						<Items>
							<asp:MenuItem NavigateUrl="Home.aspx" Text="Home" ToolTip="Home">
								<asp:MenuItem NavigateUrl="Music.aspx" Text="Music" ToolTip="Music">
									<asp:MenuItem NavigateUrl="Classical.aspx" Text="Classical" ToolTip="Classical" />
									<asp:MenuItem NavigateUrl="Rock.aspx" Text="Rock" ToolTip="Rock" />
									<asp:MenuItem NavigateUrl="Jazz.aspx" Text="Jazz" ToolTip="Jazz" />
								</asp:MenuItem>
								<asp:MenuItem NavigateUrl="Movies.aspx" Text="Movies" ToolTip="Movies">
									<asp:MenuItem NavigateUrl="Action.aspx" Text="Action" ToolTip="Action" />
									<asp:MenuItem NavigateUrl="Drama.aspx" Text="Drama" ToolTip="Drama" />
									<asp:MenuItem NavigateUrl="Musical.aspx" Text="Musical" ToolTip="Musical" />
								</asp:MenuItem>
							</asp:MenuItem>
						</Items>
					</asp:Menu>
					</div>
				</div>
			</div>
			<div class="col-md-6">
				<h2>Xml DataBinding</h2>
				<div class="row">
					<h4>Horizontal Menu Using a Table Layout</h4>

					<!-- Bind the Menu control to a XmlDataSource control.  -->
					<div data-audit-control="Menu-5">
					<asp:Menu ID="NavigationMenuTableDataSource"
						DisappearAfter="2000"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Horizontal"
						Font-Names="Arial"
						Target="_blank"
						DataSourceID="MenuSource"
						RenderingMode="Table"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />
						<DataBindings>
							<asp:MenuItemBinding DataMember="MapHomeNode" Depth="0" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="1" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="2" TextField="title" NavigateUrlField="url" />
						</DataBindings>
					</asp:Menu>
					</div>
				</div>
				<div class="row">
					<h4>Horizontal Menu Using a List Layout</h4>

					<!-- Bind the Menu control to a XmlDataSource control.  -->
					<div data-audit-control="Menu-6">
					<asp:Menu ID="NavigationMenuListDataSource"
						DisappearAfter="2000"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Horizontal"
						Font-Names="Arial"
						Target="_blank"
						DataSourceID="MenuSource"
						RenderingMode="List"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />
						<DataBindings>
							<asp:MenuItemBinding DataMember="MapHomeNode" Depth="0" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="1" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="2" TextField="title" NavigateUrlField="url" />
						</DataBindings>
					</asp:Menu>
					</div>
				</div>
				<div class="row">
					<h4>Vertical Menu Using a Table Layout</h4>
					<div data-audit-control="Menu-7">
					<asp:Menu ID="VerticalMenuTableDataSource"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Vertical"
						Target="_blank"
						DataSourceID="MenuSource"
						RenderingMode="Table"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />

						<DataBindings>
							<asp:MenuItemBinding DataMember="MapHomeNode" Depth="0" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="1" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="2" TextField="title" NavigateUrlField="url" />
						</DataBindings>
					</asp:Menu>
					</div>
				</div>
				<div class="row">
					<h4>Vertical Menu Using a List Layout</h4>
					<div data-audit-control="Menu-8">
					<asp:Menu ID="VerticalMenuListDataSource"
						StaticDisplayLevels="2"
						StaticSubMenuIndent="10"
						Orientation="Vertical"
						Target="_blank"
						DataSourceID="MenuSource"
						RenderingMode="List"
						runat="server">

						<StaticMenuItemStyle BackColor="LightSteelBlue" ForeColor="Black" />
						<StaticHoverStyle BackColor="LightSkyBlue" />
						<DynamicMenuItemStyle BackColor="Black" ForeColor="Silver" />
						<DynamicHoverStyle BackColor="LightSkyBlue" ForeColor="Black" />

						<DataBindings>
							<asp:MenuItemBinding DataMember="MapHomeNode" Depth="0" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="1" TextField="title" NavigateUrlField="url" />
							<asp:MenuItemBinding DataMember="MapNode" Depth="2" TextField="title" NavigateUrlField="url" />
						</DataBindings>
					</asp:Menu>
					</div>
				</div>
			</div>
		</div>
		<div class="row">
			<h2>Menu styled using bootstrap css</h2>
			<div class="navbar navbar-inverse">
				<div class="container-fluid">
					<div class="navbar-header">
						<button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1"
							aria-expanded="false">
							<span class="sr-only">Toggle navigation</span> <span class="icon-bar"></span><span
								class="icon-bar"></span><span class="icon-bar"></span>
						</button>
						<a class="navbar-brand" href="#">Menu Styled With Css Example</a>
					</div>
					<!-- Everything you want hidden at 940px or less, place within here -->
					<div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">

						<div data-audit-control="Menu-9">
						<asp:Menu ID="Menu1"
							Orientation="Horizontal"
							RenderingMode="list"
							IncludeStyleBlock="false"
							StaticMenuStyle-CssClass="nav navbar-nav"
							DynamicMenuStyle-CssClass="dropdown-menu"
							StaticEnableDefaultPopOutImage="true"
							runat="server">
							<Items>
								<asp:MenuItem Text="Home" ToolTip="Home"></asp:MenuItem>
								<asp:MenuItem Text="Music" ToolTip="Music">
									<asp:MenuItem Text="Classical" ToolTip="Classical" />
									<asp:MenuItem Text="Rock" ToolTip="Rock" />
									<asp:MenuItem Text="Jazz" ToolTip="Jazz" />
								</asp:MenuItem>
								<asp:MenuItem Text="Movies" ToolTip="Movies">
									<asp:MenuItem Text="Action" ToolTip="Action" />
									<asp:MenuItem Text="Drama" ToolTip="Drama" />
									<asp:MenuItem Text="Musical" ToolTip="Musical" />
								</asp:MenuItem>
							</Items>
						</asp:Menu>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
	<br />
	<a href="https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.menu?view=netframework-4.8">Example Source Microsoft Docs [Menu Class]</a><br />
	<a href="https://www.aspsnippets.com/Articles/Implement-Bootstrap-Navbar-Toggle-Collapsible-Menu-in-ASPNet.aspx">Source for the example of the Navbar using Bootstrap with a Menu control</a><br />
	<a href="https://www.aspsnippets.com/Articles/Implement-Bootstrap-Navbar-Toggle-Collapsible-Menu-in-ASPNet.aspx">Source for the script that adds the caret for the dropdown menues</a>

	<asp:XmlDataSource ID="MenuSource" runat="server" DataFile="Menue.xml" />

	<script type="text/javascript">
       $(function () {
           //to fix collapse mode width issue
           $(".nav li,.nav li a,.nav li ul").removeAttr('style');

           //for dropdown menu
           $(".dropdown-menu").parent().removeClass().addClass('dropdown');
           $(".dropdown>a").removeClass().addClass('dropdown-toggle').append('<b class="caret"></b>').attr('data-toggle', 'dropdown');

           //remove default click redirect effect
           $('.dropdown-toggle').attr('onclick', '').off('click');

       });
	</script>
</asp:Content>
