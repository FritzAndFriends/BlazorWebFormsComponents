using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public partial class Pager : BaseUserControl
    {
        protected LinkButton lnkPrevious;
        protected LinkButton lnkNext;
        protected Repeater rptPages;
        public event EventHandler<int> PageChanged;

        public int CurrentPage
        {
            get
            {
                object val = ViewState["CurrentPage"];
                return val != null ? (int)val : 1;
            }
            set { ViewState["CurrentPage"] = value; }
        }

        public int TotalPages
        {
            get
            {
                object val = ViewState["TotalPages"];
                return val != null ? (int)val : 1;
            }
            set { ViewState["TotalPages"] = value; }
        }

        public int PageSize
        {
            get
            {
                object val = ViewState["PageSize"];
                return val != null ? (int)val : 10;
            }
            set { ViewState["PageSize"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindPager();
        }

        private void BindPager()
        {
            lnkPrevious.Enabled = CurrentPage > 1;
            lnkNext.Enabled = CurrentPage < TotalPages;

            var pages = new List<int>();
            for (int i = 1; i <= TotalPages; i++)
            {
                pages.Add(i);
            }
            rptPages.DataSource = pages;
            rptPages.DataBind();
        }

        protected void lnkPrevious_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                OnPageChanged(CurrentPage);
            }
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                OnPageChanged(CurrentPage);
            }
        }

        protected void lnkPage_Click(object sender, EventArgs e)
        {
            var link = (LinkButton)sender;
            int page = int.Parse(link.CommandArgument);
            CurrentPage = page;
            OnPageChanged(CurrentPage);
        }

        protected virtual void OnPageChanged(int page)
        {
            PageChanged?.Invoke(this, page);
            LogActivity("Page changed to: " + page);
        }
    }
}
