using System;

namespace MyApp
{
    public partial class TC33_ClientScript : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Pattern 1: RegisterStartupScript with inline script
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "InitializeUI",
                "$(function() { console.log('ready'); });",
                true);

            // Pattern 2: RegisterClientScriptInclude with URL
            Page.ClientScript.RegisterClientScriptInclude(
                "jqueryUI",
                ResolveUrl("~/Scripts/jquery-ui.min.js"));

            // Pattern 3: GetPostBackEventReference
            var postbackRef = Page.ClientScript.GetPostBackEventReference(btnSubmit, "validate");

            // Pattern 4: RegisterClientScriptBlock
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "block1", "<script>var x = 1;</script>");

            // Pattern 5: ScriptManager.RegisterStartupScript
            ScriptManager.RegisterStartupScript(this, this.GetType(), "smScript", "alert('hello');", true);

            // Pattern 6: ScriptManager.GetCurrent
            var sm = ScriptManager.GetCurrent(Page);
        }
    }
}
