using System;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class ResourcesPage : BasePage
    {
        protected Repeater DocumentsRepeater;
        protected Repeater TemplatesRepeater;
        protected Repeater ToolsRepeater;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindResources();
            }
        }
        
        private void BindResources()
        {
            var allResources = PortalDataProvider.GetResources();
            
            // Group by category name
            var documents = allResources.Where(r => r.CategoryName == "Document").ToList();
            var templates = allResources.Where(r => r.CategoryName == "Template").ToList();
            var tools = allResources.Where(r => r.CategoryName == "Tool").ToList();
            
            DocumentsRepeater.DataSource = documents;
            DocumentsRepeater.DataBind();
            
            TemplatesRepeater.DataSource = templates;
            TemplatesRepeater.DataBind();
            
            ToolsRepeater.DataSource = tools;
            ToolsRepeater.DataBind();
        }
    }
}
