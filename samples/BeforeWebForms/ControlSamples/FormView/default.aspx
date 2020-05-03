<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.FormView._default" MasterPageFile="~/Site.Master" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

	<h2>FormView control homepage</h2>

	<div>
		<%--
      Other usage samples:  <a href="FlowLayout.aspx">FlowLayout Sample</a> <a href="StyleAttributes.aspx">Styles</a>
		--%>
	</div>

	<p>Here is a simple formview for a Widget from the SharedSampleObjects project</p>

	<asp:FormView ID="WidgetFormView"
		AllowPaging="true"
		DataKeyNames="Id"
		OnPageIndexChanging="WidgetFormView_PageIndexChanging"
		OnModeChanging="WidgetFormView_ModeChanging"
		runat="server">

		<InsertItemTemplate>

			INSERT MODE

			<br />

			<asp:LinkButton runat="server" ID="CancelButton" Text="Cancel" CommandName="Cancel"></asp:LinkButton>

		</InsertItemTemplate>

		<ItemTemplate>

			<table>
				<tr>
					<td>

						<h3><%# Eval("Name") %>&nbsp;<%# Eval("Price", "{0:C}") %>  &nbsp;&nbsp;(<%# Eval("Id") %>)</h3>

					</td>
				</tr>
				<tr>
          <td colspan="2">
            <asp:linkbutton id="NewButton"
              text="New"
              commandname="New"
              runat="server"/> 
          </td>
        </tr>
			</table>

		</ItemTemplate>

	</asp:FormView>

	Generates this source code:
	<pre>
&lt;table cellspacing="0" id="MainContent_WidgetFormView" style="border-collapse:collapse;"&gt;
	&lt;tr&gt;
		&lt;td colspan="2"&gt;
          &lt;table&gt;
            &lt;tr&gt;
              &lt;td&gt;
                &lt;h3&gt;First Widget&amp;nbsp;$7.99  &amp;nbsp;&amp;nbsp;(1)&lt;/h3&gt;      
              &lt;/td&gt;
            &lt;/tr&gt;
          &lt;/table&gt;
        &lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td colspan="2"&gt;&lt;table&gt;
			&lt;tr&gt;
				&lt;td&gt;&lt;span&gt;1&lt;/span&gt;&lt;/td&gt;
				&lt;td&gt;&lt;a href="javascript:__doPostBack(&amp;#39;ctl00$MainContent$WidgetFormView&amp;#39;,&amp;#39;Page$2&amp;#39;)"&gt;2&lt;/a&gt;&lt;/td&gt;
				&lt;td&gt;&lt;a href="javascript:__doPostBack(&amp;#39;ctl00$MainContent$WidgetFormView&amp;#39;,&amp;#39;Page$3&amp;#39;)"&gt;3&lt;/a&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;
</pre>


</asp:Content>
