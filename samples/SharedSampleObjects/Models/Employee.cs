using System.Collections.Generic;

namespace SharedSampleObjects.Models
{
	public class Employee
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Department { get; set; }

		public static List<Employee> GetEmployees()
		{
			return new List<Employee>
			{
				new Employee { Id = 1, Name = "Alice Johnson", Department = "Engineering" },
				new Employee { Id = 2, Name = "Bob Smith", Department = "Marketing" },
				new Employee { Id = 3, Name = "Carol White", Department = "Engineering" },
				new Employee { Id = 4, Name = "Dave Brown", Department = "Sales" }
			};
		}
	}
}
