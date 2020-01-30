using System.Collections.Generic;

namespace SharedSampleObjects.Models
{
	public class Category {

		public int Id { get; set; }

		public string Name { get; set; }

		public IEnumerable<Widget> Widgets { get; set; }

		public static Category[] SimpleCategoryList {

			get {

				return new Category[] {

					new Category {
						Id=1, Name="Category 1", Widgets=new Widget[]
						 {
							new Widget {Id=1, Name="Widget 1"},
							new Widget {Id=2, Name="Widget 2"},
							new Widget {Id=3, Name="Widget 3"},
						 }
					},
					new Category {
						Id=1, Name="Category 2", Widgets=new Widget[]
						 {
							new Widget {Id=4, Name="Widget 4"},
							new Widget {Id=5, Name="Widget 5"},
							new Widget {Id=6, Name="Widget 6"},
						 }
					},

				};

			}

		}

	}
}
