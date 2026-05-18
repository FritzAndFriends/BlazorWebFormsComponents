using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class SearchBox : BaseUserControl
    {
        protected TextBox txtSearch;
        public event EventHandler<SearchEventArgs> Search;

        public string Placeholder
        {
            get { return (string)ViewState["Placeholder"] ?? "Search..."; }
            set { ViewState["Placeholder"] = value; }
        }

        public string SearchText
        {
            get { return (string)ViewState["SearchText"] ?? string.Empty; }
            set { ViewState["SearchText"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtSearch.Attributes["placeholder"] = Placeholder;

            if (!IsPostBack && !string.IsNullOrEmpty(SearchText))
            {
                txtSearch.Text = SearchText;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchText = txtSearch.Text;
            OnSearch(new SearchEventArgs
            {
                SearchTerm = txtSearch.Text,
                Category = string.Empty
            });
        }

        protected virtual void OnSearch(SearchEventArgs args)
        {
            Search?.Invoke(this, args);
            LogActivity("Search performed: " + args.SearchTerm);
        }
    }
}
