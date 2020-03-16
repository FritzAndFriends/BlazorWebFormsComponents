﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.GridView
{

	public partial class Default : System.Web.UI.Page
  {

	protected void Page_Load(object sender, EventArgs e)
	{

	}

	// The return type can be changed to IEnumerable, however to support
	// paging and sorting, the following parameters must be added:
	//     int maximumRows
	//     int startRowIndex
	//     out int totalRowCount
	//     string sortByExpression
	public List<Customer> GetCustomers()
	{
			var customers = new List<Customer>();
			var c1 = new Customer
			{
				CustomerID = 1,
				FirstName = "John",
				LastName = "Smith",
				CompanyName = "Virus"
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

			return customers;
	}
  }
}
