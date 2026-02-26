<%@ Page Title="Chart - Data Binding" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="DataBinding.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Chart.DataBinding" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Chart Control - Data Binding Example</h2>

    <div>
        <a href="Default.aspx">Column Chart</a> | <a href="PieChart.aspx">Pie Chart</a> | <a href="LineChart.aspx">Line Chart</a> | <a href="DataBinding.aspx">Data Binding</a>
    </div>

    <p>This example demonstrates binding a Chart control to a collection of objects (similar to GridView data binding).</p>

    <div data-audit-control="Chart">
        <asp:Chart ID="Chart1" runat="server" Width="700px" Height="400px" 
            BackColor="WhiteSmoke" BorderColor="#999999" BorderWidth="1" 
            BorderDashStyle="Solid" DataSourceID="ObjectDataSource1">
            <Series>
                <asp:Series Name="CustomerSales" ChartType="Bar" 
                    Color="MediumSeaGreen" BorderColor="Black" BorderWidth="1"
                    XValueMember="CompanyName" YValueMembers="TotalSales">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1" 
                    BackColor="White" 
                    BorderColor="Black" 
                    BorderWidth="1" 
                    BorderDashStyle="Solid">
                    <AxisX Title="Company" IsLabelAutoFit="true">
                        <MajorGrid LineColor="LightGray" />
                    </AxisX>
                    <AxisY Title="Total Sales ($)">
                        <MajorGrid LineColor="LightGray" />
                    </AxisY>
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Text="Customer Sales Report" Font="Arial, 14pt, style=Bold" />
            </Titles>
        </asp:Chart>
    </div>

    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
        TypeName="BeforeWebForms.ControlSamples.Chart.DataBinding" 
        SelectMethod="GetCustomerSales">
    </asp:ObjectDataSource>

</asp:Content>
