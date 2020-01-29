<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.TreeView._default" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

    <h2>Treeview control homepage</h2>

    <div>
      <%--
      Other usage samples:  <a href="FlowLayout.aspx">FlowLayout Sample</a> <a href="StyleAttributes.aspx">Styles</a>
      --%>
    </div>

    <p>Here is a simple treeview with inline nodes.  This sample is lifted DIRECTLY from the documentation at:
    <a href="https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.treeview?view=netframework-4.8#examples">https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.treeview?view=netframework-4.8#examples</a></a>
    </p>

      <asp:TreeView id="SampleTreeView"
        ShowExpandCollapse="true"
        ShowCheckBoxes="All"
        ShowLines="true"
        CssClass="Foo"
        runat="server">
         
        <Nodes>
        
          <asp:TreeNode Value="Home"
            ImageToolTip="This is the home image tooltip"
            ImageUrl="~/Content/Images/csharp_56.png"
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
            
          </asp:TreeNode>
        
        </Nodes>
        
      </asp:TreeView>

      <h3>Generates the following HTML:</h3>

      <pre>
&lt;table cellpadding="0" cellspacing="0" style="border-width: 0;"&gt;
&lt;tr&gt;
	&lt;td&gt;&lt;a id="MainContent_SampleTreeViewn0" href="javascript:TreeView_ToggleNode(MainContent_SampleTreeView_Data,0,document.getElementById(&amp;#39;MainContent_SampleTreeViewn0&amp;#39;),&amp;#39; &amp;#39;,document.getElementById(&amp;#39;MainContent_SampleTreeViewn0Nodes&amp;#39;))"&gt;
		&lt;img src="/WebResource.axd?d=-n2KnhPEOy8tY2UhU96Dm0IozP6lbVClbcDt3yTGXz60VtmHzn9tFLOAH4Ur-gDnFyl7e8QCuh-B3btggGUmyRmDSboKSz9xPqeF8XvugHU1&amp;amp;t=637100650300000000" alt="Collapse Home" title="Collapse Home" style="border-width: 0;" /&gt;&lt;/a&gt;&lt;/td&gt;
	&lt;td style="white-space: nowrap;"&gt;&lt;a class="MainContent_SampleTreeView_0" href="Home.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,&amp;#39;MainContent_SampleTreeViewt0&amp;#39;);" id="MainContent_SampleTreeViewt0"&gt;Home&lt;/a&gt;&lt;/td&gt;
&lt;/tr&gt;
&lt;/table&gt;
&lt;div id="MainContent_SampleTreeViewn0Nodes" style="display: block;"&gt;
&lt;table cellpadding="0" cellspacing="0" style="border-width: 0;"&gt;
	&lt;tr&gt;
		&lt;td&gt;
			&lt;div style="width: 20px; height: 1px"&gt;&lt;/div&gt;
		&lt;/td&gt;
		&lt;td&gt;&lt;a id="MainContent_SampleTreeViewn1" href="javascript:TreeView_ToggleNode(MainContent_SampleTreeView_Data,1,document.getElementById(&amp;#39;MainContent_SampleTreeViewn1&amp;#39;),&amp;#39; &amp;#39;,document.getElementById(&amp;#39;MainContent_SampleTreeViewn1Nodes&amp;#39;))"&gt;
			&lt;img src="/WebResource.axd?d=-n2KnhPEOy8tY2UhU96Dm0IozP6lbVClbcDt3yTGXz60VtmHzn9tFLOAH4Ur-gDnFyl7e8QCuh-B3btggGUmyRmDSboKSz9xPqeF8XvugHU1&amp;amp;t=637100650300000000" alt="Collapse Page1" title="Collapse Page1" style="border-width: 0;" /&gt;&lt;/a&gt;&lt;/td&gt;
		&lt;td style="white-space: nowrap;"&gt;&lt;a class="MainContent_SampleTreeView_0" href="Page1.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,&amp;#39;MainContent_SampleTreeViewt1&amp;#39;);" id="MainContent_SampleTreeViewt1"&gt;Page1&lt;/a&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;
&lt;div id="MainContent_SampleTreeViewn1Nodes" style="display: block;"&gt;
	&lt;table cellpadding="0" cellspacing="0" style="border-width: 0;"&gt;
		&lt;tr&gt;
			&lt;td&gt;
				&lt;div style="width: 20px; height: 1px"&gt;&lt;/div&gt;
			&lt;/td&gt;
			&lt;td&gt;
				&lt;div style="width: 20px; height: 1px"&gt;&lt;/div&gt;
			&lt;/td&gt;
			&lt;td&gt;
				&lt;img src="/WebResource.axd?d=Vnw6Nul5Gi8dLBSg8ZsULStGxlfL_JMpGTZlN-lhkJ1hgUnYHh421fevEYeMCK6ZXnXATjjNaRQ_Pl3rfBFEY7KWNp6hg5jRUuIZOxLlK3g1&amp;amp;t=637100650300000000" alt="" /&gt;&lt;/td&gt;
			&lt;td style="white-space: nowrap;"&gt;&lt;a class="MainContent_SampleTreeView_0" href="Section1.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,&amp;#39;MainContent_SampleTreeViewt2&amp;#39;);" id="MainContent_SampleTreeViewt2"&gt;Section 1&lt;/a&gt;&lt;/td&gt;
		&lt;/tr&gt;
	&lt;/table&gt;
&lt;/div&gt;
&lt;table cellpadding="0" cellspacing="0" style="border-width: 0;"&gt;
	&lt;tr&gt;
		&lt;td&gt;
			&lt;div style="width: 20px; height: 1px"&gt;&lt;/div&gt;
		&lt;/td&gt;
		&lt;td&gt;
			&lt;img src="/WebResource.axd?d=Vnw6Nul5Gi8dLBSg8ZsULStGxlfL_JMpGTZlN-lhkJ1hgUnYHh421fevEYeMCK6ZXnXATjjNaRQ_Pl3rfBFEY7KWNp6hg5jRUuIZOxLlK3g1&amp;amp;t=637100650300000000" alt="" /&gt;&lt;/td&gt;
		&lt;td style="white-space: nowrap;"&gt;&lt;a class="MainContent_SampleTreeView_0" href="Page2.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,&amp;#39;MainContent_SampleTreeViewt3&amp;#39;);" id="MainContent_SampleTreeViewt3"&gt;Page 2&lt;/a&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;
    </pre>

    <h3>It also rendered some inline JavaScript</h3>
    <pre>
&lt;script type="text/javascript"&gt;
	//&lt;![CDATA[
	var MainContent_SampleTreeView_ImageArray = new Array('', '', '', '/WebResource.axd?d=Vnw6Nul5Gi8dLBSg8ZsULStGxlfL_JMpGTZlN-lhkJ1hgUnYHh421fevEYeMCK6ZXnXATjjNaRQ_Pl3rfBFEY7KWNp6hg5jRUuIZOxLlK3g1&amp;t=637100650300000000', '/WebResource.axd?d=Wt-oJLHV-dsfkxJN06L-WaQPlBCfc-teQ-3qNc2vy4TFJSyiiZJWaSNj5TKUr_YwToZ42wpGVd6AZdWiGd1NZFeKUv2kYc_8gmrr_fMqWUk1&amp;t=637100650300000000', '/WebResource.axd?d=-n2KnhPEOy8tY2UhU96Dm0IozP6lbVClbcDt3yTGXz60VtmHzn9tFLOAH4Ur-gDnFyl7e8QCuh-B3btggGUmyRmDSboKSz9xPqeF8XvugHU1&amp;t=637100650300000000');
//]]&gt;
&lt;/script&gt;
    </pre>
    <pre>
&lt;script type="text/javascript"&gt;
//&lt;![CDATA[

var callBackFrameUrl='/WebResource.axd?d=beToSAE3vdsL1QUQUxjWdQMlAi2sjCyKVUkOzz3Xr8GnHGqqFIzJ0xhT7HgleY88L0MKBhTpfsyp7htwwVbIsg2&amp;t=637100650300000000';
WebForm_InitCallback();var MainContent_SampleTreeView_Data = new Object();
MainContent_SampleTreeView_Data.images = MainContent_SampleTreeView_ImageArray;
MainContent_SampleTreeView_Data.collapseToolTip = "Collapse {0}";
MainContent_SampleTreeView_Data.expandToolTip = "Expand {0}";
MainContent_SampleTreeView_Data.expandState = theForm.elements['MainContent_SampleTreeView_ExpandState'];
MainContent_SampleTreeView_Data.selectedNodeID = theForm.elements['MainContent_SampleTreeView_SelectedNode'];
(function() {
  for (var i=0;i&lt;6;i++) {
  var preLoad = new Image();
  if (MainContent_SampleTreeView_ImageArray[i].length &gt; 0)
    preLoad.src = MainContent_SampleTreeView_ImageArray[i];
  }
})();
MainContent_SampleTreeView_Data.lastIndex = 4;
MainContent_SampleTreeView_Data.populateLog = theForm.elements['MainContent_SampleTreeView_PopulateLog'];
MainContent_SampleTreeView_Data.treeViewID = 'ctl00$MainContent$SampleTreeView';
MainContent_SampleTreeView_Data.name = 'MainContent_SampleTreeView_Data';
//]]&gt;
&lt;/script&gt;
    </pre>


</asp:Content>
