using System.Collections.Generic;
using System.Linq;

namespace SharedSampleObjects.Models
{
	public class Customer
	{
		public int CustomerID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string CompanyName { get; set; }

		public bool Active { get; set; }

		public static IQueryable<Customer> GetCustomers(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
		{
			var customers = new List<Customer>();
			var c1 = new Customer
			{
				CustomerID = 1,
				FirstName = "John",
				LastName = "Smith",
				CompanyName = "Virus",
				Active=true
			};

			var c2 = new Customer
			{
				CustomerID = 2,
				FirstName = "Jose",
				LastName = "Rodriguez",
				CompanyName = "Boring"
			};


			var c3 = new Customer
			{
				CustomerID = 3,
				FirstName = "Jason",
				LastName = "Ramirez",
				CompanyName = "Fun Machines"
			};

			customers.Add(c1);
			customers.Add(c2);
			customers.Add(c3);

			totalRowCount = customers.Count();
			return customers.AsQueryable();
		}

	}
}
