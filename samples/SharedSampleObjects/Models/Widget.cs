using System;
using System.Linq;
using System.Text;

namespace SharedSampleObjects.Models
{
	public class Widget
	{

		public int Id { get; set; }

		public string Name { get; set; }

		public decimal Price { get; set; }

		public DateTime LastUpdate { get; set; }

		public static Widget[] EmptyWidgetList { get; } = new Widget[] { };

		public static Widget[] Widgets(int n)
		{
			var rnd = new Random();
			var words = "one;two;three;four;five;six;seven;eight;nine;ten;eleven;twelve;thirteen;fourteen;fifteen;sixteen;seventeen;eighteen;nineteen;twenty;twenty-one;twenty-two;twenty-three;twenty-four;twenty-five;twenty-six;twenty-seven;twenty-eight;twenty-nine;thirty;thirty-one;thirty-two;thirty-three;thirty-five;thirty-six;thirty-seven;thirty-eight;thirty-nine;forty;forty-one;forty-two;forty-three;forty-four;forty-five;forty-six;forty-seven;forty-eight;forty-nine;fifty;fifty-one;fifty-two;fifty-three;fifty-four;fifty-five;fifty-six;fifty-seven;fifty-eight;fifty-nine;sixty;sixty-one;sixty-two;sixty-three;sixty-four;sixty-five;sixty-six;In Words;sixty-seven;sixty-eight;sixty-nine;seventy;seventy-one;seventy-two;seventy-three;seventy-four;seventy-five;seventy-six;seventy-seven;seventy-eight;seventy-nine;eighty;eighty-one;eighty-two;eighty-three;eighty-four;eighty-five;eighty-six;eighty-seven;eighty-eight;eighty-nine;ninety;ninety-one;ninety-two;ninety-three;ninety-four;ninety-five;ninety-six;ninety-seven;ninety-eight;ninety-nine;one hundred"
			            .Split(';');
			var widgets = Enumerable.Range(0, n).Select(i => new Widget()
			{
				Id = i+1,
				Name = $"{words[i % 100]} Widget",
				Price = (rnd.Next(100, 10000) / 100),
				LastUpdate = DateTime.Today.AddHours(-rnd.Next(0, 300))
			}).ToArray();
			return widgets;
		}

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
