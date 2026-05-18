using System;

namespace MyApp
{
    public partial class TC27_IsPostBackElse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInitialData();
            }
            else
            {
                ProcessPostBack();
                UpdateStatus();
            }
        }

        private void LoadInitialData()
        {
            // Load data
        }

        private void ProcessPostBack()
        {
            // Handle postback
        }

        private void UpdateStatus()
        {
            // Update UI
        }
    }
}
