using System;

namespace MyApp
{
    public partial class TC16_IsPostBackGuard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDropDownLists();
                InitializeData();
            }
        }

        private void LoadDropDownLists()
        {
            // Load data
        }

        private void InitializeData()
        {
            // Initialize
        }
    }
}
