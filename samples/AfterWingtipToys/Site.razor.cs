using System.Linq;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class Site
    {
        [Inject] private ProductContext Db { get; set; } = default!;

        private Image Image1 = default!;
        private ListView<Category> categoryList = default!;
        private ContentPlaceHolder MainContent = default!;

        public IQueryable<Category> GetCategories() => Db.Categories.AsQueryable();

        private void Unnamed_LoggingOut()
        {
            Response.Redirect("/");
        }
    }
}
