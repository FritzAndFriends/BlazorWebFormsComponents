using System;
using System.Collections.Generic;
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
					new Widget { Id=4, Name="Fourth Widget", Price=10.99M, LastUpdate=DateTime.Today.AddHours(4)},
					new Widget { Id=5, Name="Fifth Widget", Price=5.99M, LastUpdate=DateTime.Today.AddHours(5)},
					new Widget { Id=6, Name="Sixth Widget", Price=6.99M, LastUpdate=DateTime.Today.AddHours(6)},
					new Widget { Id=7, Name="Seventh Widget", Price=12.99M, LastUpdate=DateTime.Today.AddHours(7)},
					new Widget { Id=8, Name="Eighth Widget", Price=8.99M, LastUpdate=DateTime.Today.AddHours(8)},
					new Widget { Id=9, Name="Ninth Widget", Price=2.99M, LastUpdate=DateTime.Today.AddHours(9)},
					new Widget { Id=10, Name="Tenth Widget", Price=3.99M, LastUpdate=DateTime.Today.AddHours(10)},
					new Widget { Id=11, Name="Eleventh Widget", Price=16.99M, LastUpdate=DateTime.Today.AddHours(11)},
					new Widget { Id=12, Name="Fritz's Widget", Price=52.70M, LastUpdate=DateTime.Today.AddHours(8)},
				};
      }
    }

  }
}
