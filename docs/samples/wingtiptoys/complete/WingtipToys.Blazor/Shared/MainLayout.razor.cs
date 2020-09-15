using System.Collections.Generic;
using WingtipToys;
using WingtipToys.Models;
using WingtipToys.Lib;
using BlazorWebFormsComponents.DataBinding;
using System.Linq;

namespace WingtipToys.Blazor.Shared {

	public partial class MainLayout {

		public void Unnamed_LoggingOut() {
		}

		public IQueryable<Category> GetCategories(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount) {
      totalRowCount = 0;
			return null;
		}

	}

}
