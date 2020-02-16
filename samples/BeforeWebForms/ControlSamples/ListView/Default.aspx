<%@ Page Title="ListView Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.ListView.Default" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

    <h2>ListView control homepage</h2>

    <div>
      <a href="Default.aspx">Default</a> | <a href="Grouping.aspx">Grouping</a> | <a href="ModelBinding.aspx">ModelBinding</a>
    </div>

    <p>Here is a simple listview bound to a collection of widgets.</p>

    <asp:ListView ID="simpleListView" 
                  runat="server" 
                  Enabled="true"
                  ItemType="SharedSampleObjects.Models.Widget">
        <LayoutTemplate>
            <table>
                <thead>
                    <tr>
                        <td>Id</td>
                        <td>Name</td>
                        <td>Price</td>
                        <td>Last Update</td>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="itemPlaceHolder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <AlternatingItemTemplate>
            <tr class="table-dark">
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("c") %></td>
                <td><%# Item.LastUpdate.ToString("d") %></td>
            </tr>
        </AlternatingItemTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Item.Id %></td>
                <td><%# Item.Name %></td>
                <td><%# Item.Price.ToString("c") %></td>
                <td><%# Item.LastUpdate.ToString("d") %></td>
            </tr>
        </ItemTemplate>
        <ItemSeparatorTemplate>
          <tr>
            <td colspan="4" style="border-bottom: 1px solid black;">&nbsp;</td>
          </tr>
        </ItemSeparatorTemplate>
        <EmptyDataTemplate>
          <tr>
            <td colspan="4">No widgets available</td>
          </tr>
        </EmptyDataTemplate>
        <EmptyItemTemplate></EmptyItemTemplate>
    </asp:ListView>

    <code>&lt;table&gt;<br />
                &lt;thead&gt;<br />
                    &lt;tr&gt;<br />
                        &lt;td&gt;Id&lt;/td&gt;<br />
                        &lt;td&gt;Name&lt;/td&gt;<br />
                        &lt;td&gt;Price&lt;/td&gt; <br />
                        &lt;td&gt;Last Update&lt;/td&gt;<br />
                    &lt;/tr&gt;<br />
                &lt;/thead&gt;<br />
                &lt;tbody&gt;<br />
                    
                    &lt;tr&gt;<br />
                        &lt;td&gt;1&lt;/td&gt;<br />
                        &lt;td&gt;First Widget&lt;/td&gt;<br />
                        &lt;td&gt;$7.99&lt;/td&gt;<br />
                        &lt;td&gt;12/5/2019&lt;/td&gt;<br />
                    &lt;/tr&gt;<br />
        
                    &lt;tr&gt;<br />
                        &lt;td&gt;2&lt;/td&gt;<br />
                        &lt;td&gt;Second Widget&lt;/td&gt;<br />
                        &lt;td&gt;$13.99&lt;/td&gt;<br />
                        &lt;td&gt;12/5/2019&lt;/td&gt;<br />
                    &lt;/tr&gt;<br />
        
                    &lt;tr&gt;<br />
                        &lt;td&gt;3&lt;/td&gt;<br />
                        &lt;td&gt;Third Widget&lt;/td&gt;<br />
                        &lt;td&gt;$100.99&lt;/td&gt;<br />
                        &lt;td&gt;12/5/2019&lt;/td&gt;<br />
                    &lt;/tr&gt;<br />
        
                &lt;/tbody&gt;<br />
            &lt;/table&gt; 

    </code>

</asp:Content>
