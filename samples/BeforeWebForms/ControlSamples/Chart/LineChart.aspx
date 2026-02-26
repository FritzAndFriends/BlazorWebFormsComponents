<%@ Page Title="Chart - Line Chart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LineChart.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Chart.LineChart" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Chart Control - Line Chart with Multiple Series</h2>

    <div>
        <a href="Default.aspx">Column Chart</a> | <a href="PieChart.aspx">Pie Chart</a> | <a href="LineChart.aspx">Line Chart</a> | <a href="DataBinding.aspx">Data Binding</a>
    </div>

    <p>This is an example of a Line Chart displaying multiple data series for comparison (Actual vs Target sales).</p>

    <div data-audit-control="Chart">
        <asp:Chart ID="Chart1" runat="server" Width="700px" Height="400px" 
            BackColor="WhiteSmoke" BorderColor="#999999" BorderWidth="1" 
            BorderDashStyle="Solid">
            <Series>
                <asp:Series Name="ActualSales" ChartType="Line" 
                    Color="Blue" BorderWidth="3" MarkerStyle="Circle" MarkerSize="8">
                </asp:Series>
                <asp:Series Name="TargetSales" ChartType="Line" 
                    Color="Green" BorderWidth="3" MarkerStyle="Diamond" MarkerSize="8" BorderDashStyle="Dash">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1" 
                    BackColor="White" 
                    BorderColor="Black" 
                    BorderWidth="1" 
                    BorderDashStyle="Solid">
                    <AxisX Title="Quarter" Interval="1">
                        <MajorGrid LineColor="LightGray" />
                    </AxisX>
                    <AxisY Title="Sales ($1000s)">
                        <MajorGrid LineColor="LightGray" />
                    </AxisY>
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Text="Quarterly Sales: Actual vs Target" Font="Arial, 14pt, style=Bold" />
            </Titles>
            <Legends>
                <asp:Legend Name="Legend1" Docking="Bottom" Alignment="Center" />
            </Legends>
        </asp:Chart>
    </div>

</asp:Content>
