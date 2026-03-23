using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class ResourcesPage : BasePage
    {
        protected DepartmentPortal.Controls.SectionPanel DocumentsSection;
        protected DepartmentPortal.Controls.SectionPanel TemplatesSection;
        protected DepartmentPortal.Controls.SectionPanel ToolsSection;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindResources();
        }
        
        private void BindResources()
        {
            var allResources = PortalDataProvider.GetResources();
            
            // Group by file type for practical categorization
            var documents = allResources.Where(r => r.FileType == "PDF").ToList();
            var templates = allResources.Where(r => 
                r.FileType == "DOCX" || r.FileType == "XLSX" || r.FileType == "PPTX").ToList();
            var tools = allResources.Where(r => 
                r.FileType != "PDF" && r.FileType != "DOCX" && 
                r.FileType != "XLSX" && r.FileType != "PPTX").ToList();
            
            // Force SectionPanel template instantiation before FindControl
            DocumentsSection.EnsureChildControls();
            var docsRepeater = DocumentsSection.FindControl("DocumentsRepeater") as Repeater;
            if (docsRepeater != null)
            {
                docsRepeater.DataSource = documents;
                docsRepeater.DataBind();
            }
            
            TemplatesSection.EnsureChildControls();
            var templatesRepeater = TemplatesSection.FindControl("TemplatesRepeater") as Repeater;
            if (templatesRepeater != null)
            {
                templatesRepeater.DataSource = templates;
                templatesRepeater.DataBind();
            }
            
            ToolsSection.EnsureChildControls();
            var toolsRepeater = ToolsSection.FindControl("ToolsRepeater") as Repeater;
            if (toolsRepeater != null)
            {
                toolsRepeater.DataSource = tools;
                toolsRepeater.DataBind();
            }
        }
    }
}
