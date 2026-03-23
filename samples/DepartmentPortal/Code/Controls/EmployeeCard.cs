using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public class EmployeeCard : CompositeControl
    {
        public int EmployeeId
        {
            get { return (int)(ViewState["EmployeeId"] ?? 0); }
            set { ViewState["EmployeeId"] = value; }
        }

        public string EmployeeName
        {
            get { return (string)(ViewState["EmployeeName"] ?? string.Empty); }
            set { ViewState["EmployeeName"] = value; }
        }

        public string Title
        {
            get { return (string)(ViewState["Title"] ?? string.Empty); }
            set { ViewState["Title"] = value; }
        }

        public string Department
        {
            get { return (string)(ViewState["Department"] ?? string.Empty); }
            set { ViewState["Department"] = value; }
        }

        public string PhotoUrl
        {
            get { return (string)(ViewState["PhotoUrl"] ?? string.Empty); }
            set { ViewState["PhotoUrl"] = value; }
        }

        public bool ShowContactInfo
        {
            get { return (bool)(ViewState["ShowContactInfo"] ?? false); }
            set { ViewState["ShowContactInfo"] = value; }
        }

        public bool EnableDetailsLink
        {
            get { return (bool)(ViewState["EnableDetailsLink"] ?? false); }
            set { ViewState["EnableDetailsLink"] = value; }
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            Panel cardPanel = new Panel();
            cardPanel.CssClass = "employee-card";

            if (!string.IsNullOrEmpty(PhotoUrl))
            {
                Image photo = new Image();
                photo.ImageUrl = PhotoUrl;
                photo.CssClass = "employee-photo";
                photo.AlternateText = EmployeeName;
                cardPanel.Controls.Add(photo);
            }

            Panel infoPanel = new Panel();
            infoPanel.CssClass = "employee-info";

            Label nameLabel = new Label();
            nameLabel.Text = EmployeeName;
            nameLabel.CssClass = "employee-name";
            infoPanel.Controls.Add(nameLabel);

            Label titleLabel = new Label();
            titleLabel.Text = Title;
            titleLabel.CssClass = "employee-title";
            infoPanel.Controls.Add(titleLabel);

            Label departmentLabel = new Label();
            departmentLabel.Text = Department;
            departmentLabel.CssClass = "employee-department";
            infoPanel.Controls.Add(departmentLabel);

            if (ShowContactInfo)
            {
                Literal contactInfo = new Literal();
                contactInfo.Text = "<div class='employee-contact'>Contact info available</div>";
                infoPanel.Controls.Add(contactInfo);
            }

            cardPanel.Controls.Add(infoPanel);

            if (EnableDetailsLink)
            {
                HyperLink detailsLink = new HyperLink();
                detailsLink.Text = "View Details";
                detailsLink.NavigateUrl = "~/EmployeeDetails.aspx?id=" + EmployeeId;
                detailsLink.CssClass = "employee-details-link";
                cardPanel.Controls.Add(detailsLink);
            }

            Controls.Add(cardPanel);
        }
    }
}
