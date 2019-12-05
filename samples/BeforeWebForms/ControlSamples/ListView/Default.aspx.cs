using BeforeWebForms.Models;
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

      var widgets = new Widget[]
      {
        new Widget { Id=1, Name="First Widget", Price=7.99M, LastUpdate=DateTime.Today.AddHours(6)},
        new Widget { Id=2, Name="Second Widget", Price=13.99M, LastUpdate=DateTime.Today.AddHours(7)},
        new Widget { Id=3, Name="Third Widget", Price=100.99M, LastUpdate=DateTime.Today.AddHours(14)},

      };
      simpleListView.DataSource = widgets;
      simpleListView.DataBind();

    }
  }
}