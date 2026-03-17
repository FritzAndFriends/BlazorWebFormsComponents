using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account;

// TODO: Requires ASP.NET Core Identity migration — external login management needs UserManager
public partial class ManageLogins
{
    private string SuccessMessage { get; set; } = "";

    private IQueryable<UserLoginInfo> GetLogins(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        // TODO: Implement with UserManager to return user's external logins
        totalRowCount = 0;
        return Array.Empty<UserLoginInfo>().AsQueryable();
    }
}
