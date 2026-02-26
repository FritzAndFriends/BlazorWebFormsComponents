<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WithGridLines.aspx.cs" Inherits="BeforeWebForms.ControlSamples.TreeView.WithGridLines" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Treeview With GridLines</h2>

    <div>
      Other usage samples:  <a href="default.aspx">Simple Sample</a>
    </div>

    <p>Here is a treeview with inline nodes and the ShowLines turned on
    </p>

      <div data-audit-control="TreeView">
      <asp:TreeView id="SampleTreeView"
        ShowExpandCollapse="true"
        ShowCheckBoxes="All"
        ShowLines="true"
        CssClass="Foo"
        runat="server">
         
        <Nodes>
        
          <asp:TreeNode Value="Home"
            ImageToolTip="This is the home image tooltip"
            NavigateUrl="Home.aspx" 
            Text="Home"
            Target="Content"
            Checked="true"
            ShowCheckBox="true"
            Expanded="true">
             
            <asp:TreeNode Value="Page 1 Value" 
              NavigateUrl="Page1.aspx" 
              Text="Page1"
              Target="Content"
              Expanded="false"
              ToolTip="ToolTop" ImageToolTip="ImageToolTip">
               
              <asp:TreeNode Value="Section 1" 
                NavigateUrl="Section1.aspx" 
                Text="Section 1"
                Target="Content"/>
                 
            </asp:TreeNode>              
            
            <asp:TreeNode Value="Page 2" 
              NavigateUrl="Page2.aspx"
              Text="Page 2"
              Target="Content">
               
            </asp:TreeNode> 
            
            <asp:TreeNode Value="Page 1 Value" 
              NavigateUrl="Page1.aspx" 
              Text="Page1"
              Target="Content"
              Expanded="false"
              ToolTip="ToolTop" ImageToolTip="ImageToolTip">
               
              <asp:TreeNode Value="Section 1" 
                NavigateUrl="Section1.aspx" 
                Text="Section 1"
                Target="Content"/>
                 
            </asp:TreeNode>              
            
            <asp:TreeNode Value="Page 2" 
              NavigateUrl="Page2.aspx"
              Text="Page 2"
              Target="Content">
               
            </asp:TreeNode> 
            
            <asp:TreeNode Value="Page 1 Value" 
              NavigateUrl="Page1.aspx" 
              Text="Page1"
              Target="Content"
              Expanded="false"
              ToolTip="ToolTop" ImageToolTip="ImageToolTip">
               
              <asp:TreeNode Value="Section 1" 
                NavigateUrl="Section1.aspx" 
                Text="Section 1"
                Target="Content"/>
                 
            </asp:TreeNode>              
            
            <asp:TreeNode Value="Page 2" 
              NavigateUrl="Page2.aspx"
              Text="Page 2"
              Target="Content">
               
            </asp:TreeNode> 
            
            <asp:TreeNode Value="Page 1 Value" 
              NavigateUrl="Page1.aspx" 
              Text="Page1"
              Target="Content"
              Expanded="false"
              ToolTip="ToolTop" ImageToolTip="ImageToolTip">
               
              <asp:TreeNode Value="Section 1" 
                NavigateUrl="Section1.aspx" 
                Text="Section 1"
                Target="Content"/>
                 
            </asp:TreeNode>              
            
            <asp:TreeNode Value="Page 2" 
              NavigateUrl="Page2.aspx"
              Text="Page 2"
              Target="Content">
               
            </asp:TreeNode> 
            
            <asp:TreeNode Value="Page 1 Value" 
              NavigateUrl="Page1.aspx" 
              Text="Page1"
              Target="Content"
              Expanded="false"
              ToolTip="ToolTop" ImageToolTip="ImageToolTip">
               
              <asp:TreeNode Value="Section 1" 
                NavigateUrl="Section1.aspx" 
                Text="Section 1"
                Target="Content"/>
                 
            </asp:TreeNode>              
            
            <asp:TreeNode Value="Page 2" 
              NavigateUrl="Page2.aspx"
              Text="Page 2"
              Target="Content">
               
            </asp:TreeNode> 

            <asp:TreeNode Value="Page 1 Value" 
              NavigateUrl="Page1.aspx" 
              Text="Page1"
              Target="Content"
              Expanded="false"
              ToolTip="ToolTop" ImageToolTip="ImageToolTip">
               
              <asp:TreeNode Value="Section 1" 
                NavigateUrl="Section1.aspx" 
                Text="Section 1"
                Target="Content"/>
                 
            </asp:TreeNode>              
            
            
          </asp:TreeNode>
        
        </Nodes>
        
      </asp:TreeView>
      </div>

</asp:Content>
