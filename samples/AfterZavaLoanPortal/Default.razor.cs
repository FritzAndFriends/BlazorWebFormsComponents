// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → auto-wired on WebFormsPageBase via SessionShim/CacheShim
//   TODO(bwfc-navigation): Response.Redirect → auto-wired on WebFormsPageBase via ResponseShim
//   TODO(bwfc-form): Request.Form["key"] → auto-wired on WebFormsPageBase via FormShim (use <WebFormsForm> for interactive mode)
//   TODO(bwfc-server): Server.MapPath/HtmlEncode → auto-wired on WebFormsPageBase via ServerShim
//   TODO(bwfc-config): ConfigurationManager.AppSettings → BWFC shim (call app.UseConfigurationManagerShim() in Program.cs)
//   TODO(bwfc-general): ClientScript.RegisterStartupScript → auto-wired on WebFormsPageBase via ClientScriptShim
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → use ScriptManagerShim via ScriptManager.GetCurrent(this)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================
using System;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
namespace ZavaLoanPortal
{
    public partial class Default : WebFormsPageBase
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Server Utility Migration ---
    // TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase.
    // Methods found: UrlEncode, HtmlEncode
    // For non-page classes, inject ServerShim via DI.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    private DropDownList<object> ddlLoanProduct = default!;
    private GridView<object> gvLoanHistory = default!;
    private Label lblSubmitStepMessage = default!;
    private WizardStep s1 = default!;
    private WizardStep s2 = default!;
    private WizardStep s3 = default!;
    private WizardStep s4 = default!;
    private WizardStep s5 = default!;
    private TextBox txtAnnualIncome = default!;
    private TextBox txtCustomerId = default!;
    private TextBox txtEmail = default!;
    private TextBox txtEmployer = default!;
    private TextBox txtFirstName = default!;
    private TextBox txtJobTitle = default!;
    private TextBox txtLastName = default!;
    private TextBox txtPurpose = default!;
    private TextBox txtRequestedAmount = default!;
    private TextBox txtTermMonths = default!;
    private TextBox txtYearsEmployed = default!;
    private UpdatePanel updLoanWizard = default!;
    private Wizard wizLoanApplication = default!;
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.
    // AppSettings keys found: LoanOriginationApiUrl
    // Add these to appsettings.json under "AppSettings" section or as top-level keys.
    // ConnectionString names found: ZavaBankDb
    // Add these to appsettings.json under "ConnectionStrings" section.

        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();

            if (Context?.User?.Identity?.IsAuthenticated != true)
            {
                Response.Redirect("/Login?ReturnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery), false);
                return;
            }

                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            try { BindLoanProducts(); } catch { /* DB unavailable */ }
                        try { BindLoanHistory(); } catch { /* DB unavailable */ }
                        txtCustomerId?.Text = "1";
                        txtTermMonths?.Text = "60";
        }

        protected void wizLoanApplication_ActiveStepChanged(EventArgs e)
        {
            if (wizLoanApplication.ActiveStepIndex == 3)
            {
                _lblReviewSummary_Text = "<b>Customer:</b> " +
                    Server.HtmlEncode((txtFirstName.Text + " " + txtLastName.Text).Trim()) +
                    "<br/><b>Amount:</b> " +
                    Server.HtmlEncode(txtRequestedAmount.Text) +
                    "<br/><b>Term:</b> " +
                    Server.HtmlEncode(txtTermMonths.Text) +
                    " months";
            }
        }

        protected void wizLoanApplication_NextButtonClick(WizardNavigationEventArgs e)
        {
            decimal amount;
            int term;

            if (wizLoanApplication.ActiveStepIndex == 2 &&
                (!decimal.TryParse(txtRequestedAmount.Text, out amount) ||
                 !int.TryParse(txtTermMonths.Text, out term)))
            {
                e.Cancel = true;
                _lblSubmissionStatus_Text = "Enter valid loan amount and term.";
            }
        }

        protected void wizLoanApplication_FinishButtonClick(WizardNavigationEventArgs e)
        {
            int customerId = int.Parse(txtCustomerId.Text);
            int productId = int.Parse(ddlLoanProduct.SelectedValue);
            decimal amount = decimal.Parse(txtRequestedAmount.Text);
            int term = int.Parse(txtTermMonths.Text);

            SubmitApi("<loanApplication><customerId>" + customerId + "</customerId><loanProductId>" + productId + "</loanProductId><requestedAmount>" + amount.ToString("F2") + "</requestedAmount><termMonths>" + term + "</termMonths></loanApplication>");

            using (var c = new SqlConnection(ConfigurationManager.ConnectionStrings["ZavaBankDb"].ConnectionString))
            using (var cmd = new SqlCommand("INSERT INTO LoanApplications (CustomerID,LoanProductID,RequestedAmount,TermMonths,Purpose,Status,ApplicationDate,AssignedOfficer,CreatedDate,ModifiedDate) VALUES (@c,@p,@a,@t,@u,'Submitted',GETDATE(),@o,GETDATE(),GETDATE());", c))
            {
                cmd.Parameters.AddWithValue("@c", customerId);
                cmd.Parameters.AddWithValue("@p", productId);
                cmd.Parameters.AddWithValue("@a", amount);
                cmd.Parameters.AddWithValue("@t", term);
                cmd.Parameters.AddWithValue("@u", (object)(txtPurpose.Text ?? ""));
                cmd.Parameters.AddWithValue("@o", Context?.User?.Identity?.Name ?? "unknown");
                c.Open();
                cmd.ExecuteNonQuery();
            }

            _lblSubmissionStatus_Text = "Application submitted.";
            BindLoanHistory();
            wizLoanApplication.ActiveStepIndex = 0;
        }

        protected void gvLoanHistory_RowCommand(GridViewCommandEventArgs e)
        {
            if (string.Equals(e.CommandName, "ReviewApplication", StringComparison.OrdinalIgnoreCase))
            {
                _lblSubmissionStatus_Text = "Application selected for review.";
            }
        }

        private void BindLoanProducts()
        {
            var t = new DataTable();

            using (var c = new SqlConnection(ConfigurationManager.ConnectionStrings["ZavaBankDb"].ConnectionString))
            using (var cmd = new SqlCommand("SELECT LoanProductID, ProductName FROM LoanProducts WHERE IsActive=1 ORDER BY ProductName;", c))
            using (var a = new SqlDataAdapter(cmd))
            {
                c.Open();
                a.Fill(t);
            }

            ddlLoanProduct.DataSource = t;
            ddlLoanProduct.DataTextField = "ProductName";
            ddlLoanProduct.DataValueField = "LoanProductID";
        }

        private void BindLoanHistory()
        {
            var t = new DataTable();

            using (var c = new SqlConnection(ConfigurationManager.ConnectionStrings["ZavaBankDb"].ConnectionString))
            using (var cmd = new SqlCommand("SELECT TOP 25 ApplicationID, CustomerID, RequestedAmount, TermMonths, Status, ApplicationDate FROM LoanApplications ORDER BY ApplicationDate DESC;", c))
            using (var a = new SqlDataAdapter(cmd))
            {
                c.Open();
                a.Fill(t);
            }

            gvLoanHistory.DataSource = t;
        }

        private void SubmitApi(string payload)
        {
            var url = ConfigurationManager.AppSettings["LoanOriginationApiUrl"] ?? "";
            if (url.Length == 0)
            {
                return;
            }

            try
            {
                var r = (HttpWebRequest)WebRequest.Create(url);
                r.Method = "POST";
                r.ContentType = "application/xml";

                var b = Encoding.UTF8.GetBytes(payload);
                using (var s = r.GetRequestStream())
                {
                    s.Write(b, 0, b.Length);
                }

                using (var resp = (HttpWebResponse)r.GetResponse())
                using (var rd = new StreamReader(resp.GetResponseStream()))
                {
                    rd.ReadToEnd();
                }
            }
            catch
            {
            }
        }
    

    private object? _lblReviewSummary_Text; // TODO: migrate from Web Forms code-behind

    private object? _lblSubmissionStatus_Text; // TODO: migrate from Web Forms code-behind
}
}
