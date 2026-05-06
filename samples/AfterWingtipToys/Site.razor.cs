using System.Linq;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys
{
    public partial class Site
    {
        private ListView<Category> categoryList = default!;
        private Image Image1 = default!;
        private ContentPlaceHolder MainContent = default!;

        protected IEnumerable<Category> Categories => new CatalogService().GetCategories();
    }
}
