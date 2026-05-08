using BlazorWebFormsComponents.LoginControls;
using WingtipToys.Models;

namespace WingtipToys;

public partial class Site
{
	private IQueryable<Category> GetCategories(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
	{
		using var db = new ProductContext();
		var items = db.Categories.ToList();
		totalRowCount = items.Count;
		return items.AsQueryable();
	}

	private Task Unnamed_LoggingOut(LoginCancelEventArgs args)
	{
		Response.Redirect("/");
		return Task.CompletedTask;
	}
}