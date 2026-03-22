using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public class SectionPanel : Control, INamingContainer
    {
        public string Title
        {
            get { return (string)(ViewState["Title"] ?? string.Empty); }
            set { ViewState["Title"] = value; }
        }

        public string CssClass
        {
            get { return (string)(ViewState["CssClass"] ?? "section-panel"); }
            set { ViewState["CssClass"] = value; }
        }

        [TemplateContainer(typeof(SectionPanel))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate HeaderTemplate { get; set; }

        [TemplateContainer(typeof(SectionPanel))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ContentTemplate { get; set; }

        [TemplateContainer(typeof(SectionPanel))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate FooterTemplate { get; set; }

        private PlaceHolder headerPlaceholder;
        private PlaceHolder contentPlaceholder;
        private PlaceHolder footerPlaceholder;

        protected override void CreateChildControls()
        {
            Controls.Clear();

            Panel mainPanel = new Panel();
            mainPanel.CssClass = CssClass;

            Panel headerPanel = new Panel();
            headerPanel.CssClass = "section-header";
            
            if (HeaderTemplate != null)
            {
                headerPlaceholder = new PlaceHolder();
                HeaderTemplate.InstantiateIn(headerPlaceholder);
                headerPanel.Controls.Add(headerPlaceholder);
            }
            else if (!string.IsNullOrEmpty(Title))
            {
                Literal titleLiteral = new Literal();
                titleLiteral.Text = "<h3>" + System.Web.HttpUtility.HtmlEncode(Title) + "</h3>";
                headerPanel.Controls.Add(titleLiteral);
            }
            mainPanel.Controls.Add(headerPanel);

            Panel contentPanel = new Panel();
            contentPanel.CssClass = "section-content";
            
            if (ContentTemplate != null)
            {
                contentPlaceholder = new PlaceHolder();
                ContentTemplate.InstantiateIn(contentPlaceholder);
                contentPanel.Controls.Add(contentPlaceholder);
            }
            mainPanel.Controls.Add(contentPanel);

            if (FooterTemplate != null)
            {
                Panel footerPanel = new Panel();
                footerPanel.CssClass = "section-footer";
                footerPlaceholder = new PlaceHolder();
                FooterTemplate.InstantiateIn(footerPlaceholder);
                footerPanel.Controls.Add(footerPlaceholder);
                mainPanel.Controls.Add(footerPanel);
            }

            Controls.Add(mainPanel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            base.Render(writer);
        }
    }
}
