using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class AnnouncementsPage : BasePage
    {
        protected DepartmentPortal.Controls.SectionPanel AnnouncementsSectionPanel;
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

        protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
        {
            SearchQuery = e.SearchTerm;
            CurrentPageIndex = 0;
        }
        
        protected void PagerControl_PageChanged(object sender, int pageNumber)
        {
            CurrentPageIndex = pageNumber - 1;
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindAnnouncements();
        }
        
        private void BindAnnouncements()
        {
            var allAnnouncements = PortalDataProvider.GetAnnouncements()
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.PublishDate);
            
            var filteredAnnouncements = allAnnouncements.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredAnnouncements = filteredAnnouncements.Where(a => 
                    a.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Body.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Author.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            var announcementList = filteredAnnouncements.ToList();
            
            var pager = FindControl("PagerControl") as DepartmentPortal.Controls.Pager;
            if (pager != null)
            {
                pager.TotalPages = (int)Math.Ceiling((double)announcementList.Count / PageSize);
                pager.CurrentPage = CurrentPageIndex + 1;
            }
            
            var pagedAnnouncements = announcementList
                .Skip(CurrentPageIndex * PageSize)
                .Take(PageSize)
                .ToList();
            
            // Force SectionPanel to instantiate its templates before FindControl
            AnnouncementsSectionPanel.EnsureChildControls();
            var repeater = AnnouncementsSectionPanel.FindControl("AnnouncementsRepeater") as Repeater;
            var noResults = AnnouncementsSectionPanel.FindControl("NoResultsPanel") as Panel;
            
            if (repeater != null)
            {
                if (pagedAnnouncements.Count > 0)
                {
                    repeater.DataSource = pagedAnnouncements;
                    repeater.DataBind();
                    if (noResults != null) noResults.Visible = false;
                }
                else
                {
                    repeater.DataSource = null;
                    repeater.DataBind();
                    if (noResults != null) noResults.Visible = true;
                }
            }
        }
    }
}
