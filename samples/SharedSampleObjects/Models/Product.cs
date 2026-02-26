using System.Collections.Generic;

namespace SharedSampleObjects.Models
{
	public class Product
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public decimal Price { get; set; }

		public string Category { get; set; }

		public bool InStock { get; set; }

		public static List<Product> GetProducts()
		{
			return new List<Product>
			{
				new Product { Id = 1, Name = "Widget", Price = 9.99m, Category = "Tools", InStock = true },
				new Product { Id = 2, Name = "Gadget", Price = 19.99m, Category = "Electronics", InStock = true },
				new Product { Id = 3, Name = "Sprocket", Price = 4.50m, Category = "Hardware", InStock = false },
				new Product { Id = 4, Name = "Gizmo", Price = 29.99m, Category = "Electronics", InStock = true },
				new Product { Id = 5, Name = "Doohickey", Price = 14.75m, Category = "Tools", InStock = true },
				new Product { Id = 6, Name = "Thingamajig", Price = 7.25m, Category = "Hardware", InStock = false },
				new Product { Id = 7, Name = "Whatchamacallit", Price = 22.00m, Category = "Electronics", InStock = true },
				new Product { Id = 8, Name = "Contraption", Price = 35.50m, Category = "Tools", InStock = true },
				new Product { Id = 9, Name = "Apparatus", Price = 42.99m, Category = "Hardware", InStock = false },
				new Product { Id = 10, Name = "Mechanism", Price = 18.50m, Category = "Electronics", InStock = true }
			};
		}

		public static List<Product> GetProducts(int count)
		{
			var categories = new[] { "Tools", "Electronics", "Hardware" };
			var products = new List<Product>();
			for (var i = 1; i <= count; i++)
			{
				products.Add(new Product
				{
					Id = i,
					Name = $"Product {i}",
					Price = System.Math.Round(4.99m + (i * 1.5m), 2),
					Category = categories[(i - 1) % 3],
					InStock = i % 3 != 0
				});
			}
			return products;
		}
	}
}
