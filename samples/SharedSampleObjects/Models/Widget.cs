using System;
using System.Text;

namespace SharedSampleObjects.Models
{
	public class Widget
  {

    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public DateTime LastUpdate { get; set; }

    public static Widget[] SimpleWidgetList
    {
      get
      {
        return new Widget[]
        {
          new Widget { Id=1, Name="First Widget", Price=7.99M, LastUpdate=DateTime.Today.AddHours(6)},
          new Widget { Id=2, Name="Second Widget", Price=13.99M, LastUpdate=DateTime.Today.AddHours(7)},
          new Widget { Id=3, Name="Third Widget", Price=100.99M, LastUpdate=DateTime.Today.AddHours(14)},
        };
      }
    }

  }
}
