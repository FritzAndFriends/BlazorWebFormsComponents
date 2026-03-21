using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class ResourceBrowser : BaseUserControl
    {
        protected SearchBox ctlSearchBox;
        protected Breadcrumb ctlBreadcrumb;
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlCategories;
        protected System.Web.UI.WebControls.Repeater rptCategories;
        protected System.Web.UI.WebControls.Repeater rptResources;

        public event EventHandler<int> ResourceSelected;

        public int CategoryId
        {
            get
            {
                object val = ViewState["CategoryId"];
                return val != null ? (int)val : 0;
            }
            set { ViewState["CategoryId"] = value; }
        }

        public bool ShowCategories
        {
            get
            {
                object val = ViewState["ShowCategories"];
                return val != null ? (bool)val : true;
            }
            set { ViewState["ShowCategories"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ctlSearchBox.Search += CtlSearchBox_Search;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlCategories.Visible = ShowCategories;
            ctlBreadcrumb.CurrentPath = "Resources";

            BindData();
        }

        private void BindData()
        {
            var resources = PortalDataProvider.GetResources();

            if (ShowCategories)
            {
                var categories = resources
                    .Select(r => new { r.CategoryId, r.CategoryName })
                    .Distinct()
                    .ToList();
                rptCategories.DataSource = categories;
                rptCategories.DataBind();
            }

            if (CategoryId > 0)
            {
                resources = resources.Where(r => r.CategoryId == CategoryId).ToList();
                ctlBreadcrumb.CurrentPath = "Resources/Category";
            }

            rptResources.DataSource = resources;
            rptResources.DataBind();

            LogActivity("ResourceBrowser bound with CategoryId: " + CategoryId);
        }

        private void CtlSearchBox_Search(object sender, SearchEventArgs args)
        {
            var resources = PortalDataProvider.GetResources();

            if (!string.IsNullOrEmpty(args.SearchTerm))
            {
                resources = resources.Where(r =>
                    r.Title.IndexOf(args.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    r.Description.IndexOf(args.SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            rptResources.DataSource = resources;
            rptResources.DataBind();
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectCategory")
            {
                CategoryId = int.Parse(e.CommandArgument.ToString());
                BindData();
            }
        }

        protected void rptResources_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectResource")
            {
                int resourceId = int.Parse(e.CommandArgument.ToString());
                OnResourceSelected(resourceId);
            }
        }

        protected virtual void OnResourceSelected(int resourceId)
        {
            ResourceSelected?.Invoke(this, resourceId);
            LogActivity("Resource selected: " + resourceId);
        }
    }
}
