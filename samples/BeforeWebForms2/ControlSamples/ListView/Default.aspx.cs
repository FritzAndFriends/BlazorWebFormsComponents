using SharedSampleObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.ListView
{
  public partial class Default : System.Web.UI.Page
  {

    protected void Page_Load(object sender, EventArgs e)
    {

      simpleListView.DataSource = Widget.SimpleWidgetList;
      simpleListView.DataBind();

    }

  }
}