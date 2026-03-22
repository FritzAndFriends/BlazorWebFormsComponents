using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class AnnouncementsPage : BasePage
    {
        protected Repeater AnnouncementsRepeater;
        protected Panel NoResultsPanel;
        
        private const int PageSize = 10;
        private int CurrentPageIndex
        {
            get { return ViewState["CurrentPageIndex"] != null ? (int)ViewState["CurrentPageIndex"] : 0; }
            set { ViewState["CurrentPageIndex"] = value; }
        }
        
        private string SearchQuery
        {
            get { return ViewState["SearchQuery"] as string ?? string.Empty; }
            set { ViewState["SearchQuery"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindAnnouncements();
            }
        }
        
        protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
        {
            SearchQuery = e.SearchTerm;
            CurrentPageIndex = 0;
            BindAnnouncements();
        }
        
        protected void PagerControl_PageChanged(object sender, int pageNumber)
        {
            CurrentPageIndex = pageNumber - 1;
            BindAnnouncements();
        }
        
        private void BindAnnouncements()
        {
            var allAnnouncements = PortalDataProvider.GetAnnouncements()
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.PublishDate);
            
            // Apply search filter
            var filteredAnnouncements = allAnnouncements.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredAnnouncements = filteredAnnouncements.Where(a => 
                    a.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Body.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Author.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            var announcementList = filteredAnnouncements.ToList();
            
            // Set up pager
            var pager = (DepartmentPortal.Controls.Pager)FindControl("PagerControl");
            if (pager != null)
            {
                pager.TotalPages = (int)Math.Ceiling((double)announcementList.Count / PageSize);
                pager.CurrentPage = CurrentPageIndex + 1;
            }
            
            // Get page of announcements
            var pagedAnnouncements = announcementList
                .Skip(CurrentPageIndex * PageSize)
                .Take(PageSize)
                .ToList();
            
            if (pagedAnnouncements.Count > 0)
            {
                AnnouncementsRepeater.DataSource = pagedAnnouncements;
                AnnouncementsRepeater.DataBind();
                NoResultsPanel.Visible = false;
            }
            else
            {
                AnnouncementsRepeater.DataSource = null;
                AnnouncementsRepeater.DataBind();
                NoResultsPanel.Visible = true;
            }
        }
    }
}
